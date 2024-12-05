using System.Xml.Serialization;

namespace DTNL.UmbracoCms.Web.Infrastructure.ApiClients.Ats.Models;

[XmlRoot("source")]
public class AtsVacanciesResponse
{
    [XmlArray("jobs")]
    [XmlArrayItem("job")]
    public required List<AtsVacancy> Vacancies { get; set; }
}
