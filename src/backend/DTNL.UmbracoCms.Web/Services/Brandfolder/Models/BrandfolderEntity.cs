namespace DTNL.UmbracoCms.Web.Services.Brandfolder.Models;

public class BrandfolderEntity
{
    public required string Id { get; set; }

    public required BrandfolderEntityAttributes Attributes { get; set; }

    public BrandfolderReferences? Relationships { get; set; }
}
