using System.Runtime.Serialization;
using DTNL.UmbracoCms.Web.Infrastructure.ApiClients.Ats.Models;

namespace DTNL.UmbracoCms.Web.Infrastructure.ApiClients.Ats;

[DataContract(Name = "source")]
public class AtsVacanciesResponse
{
    [DataMember(Name = "jobs")]
    public required List<AtsVacancy> Vacancies { get; set; }
}
