using System.Runtime.Serialization;
using System.Xml;
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

    public async Task<List<AtsVacancy>?> GetAllVacancies(CancellationToken cancellationToken)
    {
        try
        {
            string atsVacanciesResponseResponseXml = await _apiClientOptions.HostName
                .AppendPathSegments(_apiClientOptions.Path)
                .GetStringAsync(cancellationToken: cancellationToken);

            DataContractSerializer serializer = new(typeof(AtsVacanciesResponse));

            using StringReader reader = new(atsVacanciesResponseResponseXml);
            using XmlReader xmlReader = XmlReader.Create(reader);

            AtsVacanciesResponse? atsVacanciesResponse = serializer.ReadObject(xmlReader) as AtsVacanciesResponse;

            return atsVacanciesResponse?.Vacancies;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unable to retrieve vacancies");

            return null;
        }
    }
}
