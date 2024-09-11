using System.Runtime.Serialization;

namespace DTNL.UmbracoCms.Web.Infrastructure.ApiClients.Ats.Models;

[DataContract(Name = "job")]
public class AtsVacancy
{
    [DataMember(Name = "title")]
    public string? Title { get; set; }

    [DataMember(Name = "created")]
    public string? CreatedAt { get; set; }

    [DataMember(Name = "url")]
    public string? Url { get; set; }

    [DataMember(Name = "city")]
    public string? City { get; set; }

    [DataMember(Name = "state")]
    public string? State { get; set; }

    [DataMember(Name = "postalcode")]
    public string? PostalCode { get; set; }

    [DataMember(Name = "country")]
    public string? Country { get; set; }

    [DataMember(Name = "jobtype")]
    public string? JobType { get; set; }

    [DataMember(Name = "location")]
    public string? Location { get; set; }

    [DataMember(Name = "multilocation")]
    public string? MultiLocation { get; set; }

    [DataMember(Name = "department")]
    public string? Department { get; set; }

    [DataMember(Name = "dept")]
    public string? Dept { get; set; }

    [DataMember(Name = "facility")]
    public string? Facility { get; set; }

    [DataMember(Name = "ID")]
    public string? Id { get; set; }

    [DataMember(Name = "shifttype")]
    public string? ShiftType { get; set; }

    [DataMember(Name = "recruiterid")]
    public string? RecruiterId { get; set; }

    [DataMember(Name = "joblocale")]
    public string? JobLocale { get; set; }

    [DataMember(Name = "customfield1")]
    public string? CustomField1 { get; set; }

    [DataMember(Name = "customfield2")]
    public string? CustomField2 { get; set; }

    [DataMember(Name = "description")]
    public string? Description { get; set; }
}
