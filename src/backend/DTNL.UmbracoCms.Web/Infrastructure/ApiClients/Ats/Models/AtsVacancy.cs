using System.Xml.Serialization;

namespace DTNL.UmbracoCms.Web.Infrastructure.ApiClients.Ats.Models;

public class AtsVacancy
{
    [XmlElement("title")]
    public string? Title { get; set; }

    [XmlElement("created")]
    public string? CreatedAt { get; set; }

    [XmlElement("url")]
    public string? Url { get; set; }

    [XmlElement("city")]
    public string? City { get; set; }

    [XmlElement("state")]
    public string? State { get; set; }

    [XmlElement("postalcode")]
    public string? PostalCode { get; set; }

    [XmlElement("country")]
    public string? Country { get; set; }

    [XmlElement("jobtype")]
    public string? JobType { get; set; }

    [XmlElement("location")]
    public string? Location { get; set; }

    [XmlElement("multilocation")]
    public string? MultiLocation { get; set; }

    [XmlElement("department")]
    public string? Department { get; set; }

    [XmlElement("dept")]
    public string? Dept { get; set; }

    [XmlElement("facility")]
    public string? Facility { get; set; }

    [XmlElement("ID")]
    public string? Id { get; set; }

    [XmlElement("shifttype")]
    public string? ShiftType { get; set; }

    [XmlElement("recruiterid")]
    public string? RecruiterId { get; set; }

    [XmlElement("joblocale")]
    public string? JobLocale { get; set; }

    [XmlElement("customfield1")]
    public string? CustomField1 { get; set; }

    [XmlElement("customfield2")]
    public string? CustomField2 { get; set; }

    [XmlElement("description")]
    public string? Description { get; set; }
}
