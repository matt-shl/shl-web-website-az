using DTNL.UmbracoCms.Web.Helpers.Aliases;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public partial class Map
{
    public class Region
    {
        public required string Id { get; set; }

        public required string Name { get; set; }

        public required List<Country> Countries { get; set; }

        public int OfficeCount => Countries.Sum(c => c.Offices.Count);

        public string OfficeCountLabel => OfficeCount == 1
            ? TranslationAliases.Common.Map.Location
            : TranslationAliases.Common.Map.Locations;

        public static Region Create(NestedBlockMapRegion region)
        {
            return new()
            {
                Id = region.RegionName!.ToLowerInvariant(),
                Name = region.RegionName,
                Countries = region.Countries
                    .Using(r => r.Content as NestedBlockMapCountry)
                    .Using(Country.Create)
                    .ToList(),
            };
        }

        public class Country
        {
            public required string Id { get; set; }

            public required string Name { get; set; }

            public decimal MapZoom { get; set; }

            public decimal MapX { get; set; }

            public decimal MapY { get; set; }

            public decimal TriggerX { get; set; }

            public decimal TriggerY { get; set; }

            public required List<Office> Offices { get; set; }

            public static Country Create(NestedBlockMapCountry country)
            {
                return new Country
                {
                    Id = country.CountryCode!,
                    Name = country.CountryName!,
                    MapZoom = country.MapZoom,
                    MapX = country.MapX,
                    MapY = country.MapY,
                    TriggerX = country.TriggerX,
                    TriggerY = country.TriggerY,
                    Offices = country.Offices
                        .Using(o => o.Content as NestedBlockMapOffice)
                        .Using(Office.Create)
                        .ToList(),
                };
            }

            public class Office
            {
                public required string Id { get; set; }

                public required string Name { get; set; }

                public string? Address { get; set; }

                public string? Email { get; set; }

                public string? Phone { get; set; }

                public Image? Image { get; set; }

                public string? Description { get; set; }

                public Button? LinkButton { get; set; }

                public decimal TriggerX { get; set; }

                public decimal TriggerY { get; set; }

                public static Office Create(NestedBlockMapOffice office)
                {
                    return new()
                    {
                        Id = office.Key.ToString(),
                        Name = office.OfficeName!,
                        Address = office.Address?.ToHtmlString(),
                        Email = office.Email,
                        Phone = office.Phone,
                        Image = Image.Create(office.Image, cssClasses: "office-modal__image", style: "office-modal"),
                        Description = office.Description,
                        LinkButton = Button.Create(office.LinkButton).With(b => b.Class = "office-modal__cta"),
                        TriggerX = office.TriggerX,
                        TriggerY = office.TriggerY,
                    };
                }
            }
        }
    }
}
