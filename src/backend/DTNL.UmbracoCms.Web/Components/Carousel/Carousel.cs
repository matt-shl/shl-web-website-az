namespace DTNL.UmbracoCms.Web.Components;

public class Carousel
{
    public string? Classes { get; set; }

    public bool Navigation { get; set; }

    public bool Overflow { get; set; }

    public bool MobileOnly { get; set; }

    public string? Id { get; set; }

    public double? SlidesMobile { get; set; }

    public double? SlidesTabletPortrait { get; set; }

    public double? SlidesTabletLandscape { get; set; }

    public double? SlidesDesktop { get; set; }

    public double? SpaceBetweenMobile { get; set; }

    public double? SpaceBetweenTabletPortrait { get; set; }

    public double? SpaceBetweenTabletLandscape { get; set; }

    public double? SpaceBetweenDesktop { get; set; }

    public double? SlidesOffsetBeforeMobile { get; set; }

    public double? SlidesOffsetAfterMobile { get; set; }

    public double? SlidesOffsetBeforeTabletPortrait { get; set; }

    public double? SlidesOffsetAfterTabletPortrait { get; set; }

    public double? SlidesOffsetBeforeTabletLandscape { get; set; }

    public double? SlidesOffsetAfterTabletLandscape { get; set; }

    public double? SlidesOffsetBeforeDesktop { get; set; }

    public double? SlidesOffsetAfterDesktop { get; set; }

    public bool Pagination { get; set; }

    public bool Loop { get; set; }

    public bool Scrollbar { get; set; }

    public bool DisableSwipingTabletPortraitAndBigger { get; set; }

    public string? CustomWrapperClass { get; set; }
}
