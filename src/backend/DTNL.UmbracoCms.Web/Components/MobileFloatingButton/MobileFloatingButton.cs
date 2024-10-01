using DTNL.UmbracoCms.Web.Helpers.Aliases;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class MobileFloatingButton : ViewComponentExtended
{
    public required Button Button { get; set; }

    public string? CssClasses { get; set; }

    public IViewComponentResult Invoke(IPublishedContent page)
    {
        Button? button = page switch
        {
            PageVacancy vacancyPage when !vacancyPage.ExternalUrl.IsNullOrWhiteSpace() => new Button
            {
                Label = CultureDictionary.GetTranslation(TranslationAliases.Vacancies.Apply),
                Url = vacancyPage.ExternalUrl,
                Variant = "primary",
                Icon = SvgAliases.Icons.ArrowTopRight,
                Class = "mobile-floating-button__button",
            },
            _ => null,
        };

        if (button is null)
        {
            return Content("");
        }

        Button = button;

        return View("MobileFloatingButton", this);
    }
}
