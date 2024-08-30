using DTNL.UmbracoCms.Web.Models.Globalization;
using DTNL.UmbracoCms.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class LanguageSelector : ViewComponentExtended
{
    private readonly IGlobalizationService _globalizationService;

    public LanguageSelector(IGlobalizationService globalizationService)
    {
        _globalizationService = globalizationService;
    }

    public required string CurrentLanguageName { get; set; }

    public required List<AlternateUrl> AlternateLanguageUrls { get; set; }

    public bool Mobile { get; set; }

    public IViewComponentResult Invoke(bool mobile = false)
    {
        Mobile = mobile;

        if (NodeProvider.CurrentNode is not ICompositionBasePage page ||
            _globalizationService.GetAlternateUrls(page) is not { Count: > 1 } alternateUrls)
        {
            return Content("");
        }

        AlternateLanguageUrls = alternateUrls;

        return View("LanguageSelector", this);
    }
}
