using System.Xml.Serialization;
using DTNL.UmbracoCms.Web.Infrastructure.ApiClients.Ats.Models;

namespace DTNL.UmbracoCms.Web.Infrastructure.ApiClients.Ats;

[XmlRoot("source")]
public class AtsVacanciesResponse
{
    [XmlArray("jobs")]
    [XmlArrayItem("job")]
    public required List<AtsVacancy> Vacancies { get; set; }
}
