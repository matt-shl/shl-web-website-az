namespace DTNL.UmbracoCms.Web.Services.Brandfolder.Models;

public class BrandfolderReference
{
    public required BrandfolderReferenceData Data { get; init; }

    public class BrandfolderReferenceData
    {
        public required string Id { get; init; }
    }
}
