using System.Xml.Serialization;
using DTNL.UmbracoCms.Web.Infrastructure.ApiClients.Ats.Models;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Options;

namespace DTNL.UmbracoCms.Web.Infrastructure.ApiClients.Ats;

public interface IAtsApiClient
{
    Task<List<AtsVacancy>?> GetAllVacancies(CancellationToken cancellationToken);
}

public class AtsApiClient : IAtsApiClient
{
    private readonly AtsApiClientOptions _apiClientOptions;
    private readonly ILogger<AtsApiClient> _logger;

    public AtsApiClient(IOptions<AtsApiClientOptions> options, ILogger<AtsApiClient> logger)
    {
        _apiClientOptions = options.Value;
        _logger = logger;
    }

#pragma warning disable CA5369 // Suppress XmlSerializer insecure deserialization warning
    public async Task<List<AtsVacancy>?> GetAllVacancies(CancellationToken cancellationToken)
    {
        try
        {
            string atsVacanciesResponseResponseXml = await _apiClientOptions.HostName
                .AppendPathSegments(_apiClientOptions.Path)
                .GetStringAsync(cancellationToken: cancellationToken);

            XmlSerializer serializer = new(typeof(AtsVacanciesResponse));

            using StringReader reader = new(atsVacanciesResponseResponseXml);

            AtsVacanciesResponse? atsVacanciesResponse = serializer.Deserialize(reader) as AtsVacanciesResponse;

            return atsVacanciesResponse?.Vacancies;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unable to retrieve vacancies");

            return null;
        }
    }
#pragma warning restore CA5369
}
