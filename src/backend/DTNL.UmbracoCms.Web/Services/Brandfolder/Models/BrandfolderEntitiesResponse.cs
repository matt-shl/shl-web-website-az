namespace DTNL.UmbracoCms.Web.Services.Brandfolder.Models;

public class BrandfolderEntitiesResponse
{
    public required List<BrandfolderEntity> Data { get; set; }

    public required BrandfolderEntitiesMeta Meta { get; set; }
}
