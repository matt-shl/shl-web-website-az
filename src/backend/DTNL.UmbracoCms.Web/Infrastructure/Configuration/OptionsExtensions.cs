using Microsoft.Extensions.Options;

namespace DTNL.UmbracoCms.Web.Infrastructure.Configuration;

public static class OptionsExtensions
{
    /// <summary>
    /// Adds, binds and validates the provided <typeparamref name="TOptions"/> class using the default section name.
    /// </summary>
    public static OptionsBuilder<TOptions> AddOptions<TOptions>(this IServiceCollection services, IConfiguration configuration)
        where TOptions : class
    {
        return services.AddOptions<TOptions>().Bind(configuration.GetSection<TOptions>()).ValidateDataAnnotations().ValidateOnStart();
    }

    /// <summary>
    /// Gets the default configuration section for the provided <typeparamref name="TOptions"/>.
    /// </summary>
    public static IConfigurationSection GetSection<TOptions>(this IConfiguration configuration)
    {
        return configuration.GetSection(typeof(TOptions).Name.TrimEnd("Options"));
    }

    /// <summary>
    ///     Gets options from the default section based on the type.
    /// </summary>
    public static TOptions? GetFromDefaultSection<TOptions>(this IConfiguration configuration)
        where TOptions : class
    {
        return configuration
            .GetSection<TOptions>()
            .Get<TOptions>();
    }
}
