using Hangfire;
using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Server;
using Hangfire.States;
using Hangfire.Storage;

namespace DTNL.UmbracoCms.Web.Modules.BackgroundJobs.Hangfire.Filters;

/// <summary>
/// Tries to make sure that a job is not scheduled if it is already running.
/// </summary>
/// <remarks>
/// This is done in a best effort basis.
/// If this needs really needs to be prevented the <see cref="DisableConcurrentExecutionAttribute"/> should also be used.
/// </remarks>
public class SkipWhenPreviousJobIsRunningAttribute : JobFilterAttribute, IClientFilter, IApplyStateFilter, IElectStateFilter
{
    /// <inheritdoc />
    public void OnCreating(CreatingContext context)
    {
        if (context.Connection is not JobStorageConnection connection)
        {
            return;
        }

        // We should run this filter only for background jobs based on recurring ones
        if (!context.Parameters.TryGetValue("RecurringJobId", out object? value) || value is not string { Length: > 0 } recurringJobId)
        {
            return;
        }

        // Get last known serverId
        if (connection.GetValueFromHash($"recurring-job:{recurringJobId}", "LastServerId") is not { Length: > 0 } lastServerId)
        {
            return;
        }

        // The last server that was processing the job is no longer active
        if (lastServerId != EnqueuedState.StateName && context.Storage.GetMonitoringApi().Servers().All(s => s.Name != lastServerId))
        {
            return;
        }

        // The job must already be running
        context.Canceled = true;
    }

    /// <inheritdoc />
    public void OnCreated(CreatedContext context)
    {
        // Nothing to do here.
    }

    /// <inheritdoc />
    public void OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
    {
        string? recurringJobId = SerializationHelper.Deserialize<string>(context.Connection.GetJobParameter(context.BackgroundJob.Id, "RecurringJobId"));
        if (string.IsNullOrWhiteSpace(recurringJobId))
        {
            return;
        }

        switch (context.NewState)
        {
            case EnqueuedState:
                transaction.SetRangeInHash(
                    $"recurring-job:{recurringJobId}",
                    [new KeyValuePair<string, string>("LastServerId", EnqueuedState.StateName)]);
                break;
            case ProcessingState processingState:
                transaction.SetRangeInHash(
                    $"recurring-job:{recurringJobId}",
                    [new KeyValuePair<string, string>("LastServerId", processingState.ServerId)]);
                break;
            case FailedState:
            case not null when context.NewState.IsFinal:
                transaction.SetRangeInHash(
                    $"recurring-job:{recurringJobId}",
                    [new KeyValuePair<string, string>("LastServerId", "")]);
                break;
            default:
                break;
        }
    }

    /// <inheritdoc />
    public void OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
    {
        // Nothing to do here.
    }

    /// <inheritdoc />
    public void OnStateElection(ElectStateContext context)
    {
        if (context.CandidateState is not EnqueuedState || context.CurrentState == ProcessingState.StateName)
        {
            return;
        }

        if (context.Connection is not JobStorageConnection connection)
        {
            return;
        }

        // We should run this filter only for background jobs based on recurring ones
        string? recurringJobId = SerializationHelper.Deserialize<string>(context.Connection.GetJobParameter(context.BackgroundJob.Id, "RecurringJobId"));
        if (string.IsNullOrWhiteSpace(recurringJobId))
        {
            return;
        }

        // Get last known serverId
        if (connection.GetValueFromHash($"recurring-job:{recurringJobId}", "LastServerId") is not { Length: > 0 } lastServerId)
        {
            return;
        }

        // The last server that was processing the job is no longer active
        if (lastServerId != EnqueuedState.StateName && context.Storage.GetMonitoringApi().Servers().All(s => s.Name != lastServerId))
        {
            return;
        }

        // Cancel next execution attempts.
        context.SetJobParameter<int>("RetryCount", short.MaxValue);
        context.CandidateState = new FailedState(new JobAbortedException())
        {
            Reason = "Further retries cancelled as another job instance is already running.",
        };
    }
}
