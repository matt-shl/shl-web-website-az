using DTNL.UmbracoCms.Web.Components.BasePage;
using DTNL.UmbracoCms.Web.Helpers.Aliases;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class Footer : ViewComponentExtended
{
    public string? Text { get; set; }

    public string? LinkGroupsAriaLabel { get; set; }

    public required List<(Group<Button> LinkGroup, Accordion.Item AccordionItem)> LinkGroups { get; set; }

    public string? SocialLinksAriaLabel { get; set; }

    public SocialLinks? SocialLinks { get; set; }

    public Button? SocialMediaPolicyLink { get; set; }

    public string? HomePageLinkAriaLabel { get; set; }

    public string? HomePageUrl { get; set; }

    public string? CopyrightLabel { get; set; }

    public string? BottomLinksAriaLabel { get; set; }

    public required List<Button> BottomLinks { get; set; }

    public Slogan? FooterScrollingText { get; set; }

    public IViewComponentResult Invoke(SiteSettings? siteSettings, IPublishedContent? page)
    {
        FooterScrollingText = Slogan
            .Create((page as ICompositionBasePage)?.Slogan?.Select(block => block.Content).OfType<NestedBlockSlogan>().FirstOrDefault(), "footer__scrolling-text");

        Text = siteSettings?.FooterText;

        LinkGroupsAriaLabel = CultureDictionary.GetTranslation(TranslationAliases.Common.Footer.LinkGroupsLabel);
        LinkGroups = (siteSettings?.FooterLinkGroups?.Select(block => block.Content as NestedBlockLinks))
            .Using(linksBlock => Group<Button>
                .Create(
                    linksBlock,
                    link => Button.Create(link)
                        .With(b =>
                        {
                            b.Variant = "link";
                            b.Class = "footer__navigation-link";
                        })))
            .Where(linkGroup => linkGroup.Items.Count > 0)
            .Select(linkGroup => (linkGroup, new Accordion.Item { Id = linkGroup.Id, Title = linkGroup.Title }))
            .ToList();

        SocialLinksAriaLabel = CultureDictionary.GetTranslation(TranslationAliases.Common.SocialSharing);
        SocialLinks = SocialLinks.Create(siteSettings);
        SocialMediaPolicyLink = Button
            .Create(siteSettings?.SocialMediaPolicyLink)
            .With(b =>
            {
                b.Variant = "link";
                b.Class = "footer__sub-navigation-link";
            });

        HomePageLinkAriaLabel = CultureDictionary.GetTranslation(TranslationAliases.Common.Footer.HomePageLinkLabel);
        HomePageUrl = NodeProvider.HomePage?.Url();

        BottomLinksAriaLabel = CultureDictionary.GetTranslation(TranslationAliases.Common.Footer.BottomLinksLabel);
        BottomLinks = (siteSettings?.FooterBottomLinks)
            .Using(link => Button.Create(link)
                .With(b =>
                {
                    b.Variant = "link";
                    b.Class = "footer__sub-navigation-link";
                }))
            .ToList();

        CopyrightLabel = CultureDictionary.GetTranslation(TranslationAliases.Common.Footer.CopyrightLabel, TimeProvider.System.GetLocalNow().Date.Year);

        return View("Footer", this);
    }
}
