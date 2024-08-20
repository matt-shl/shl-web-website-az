using DTNL.UmbracoCms.Web.Helpers.Aliases;
using DTNL.UmbracoCms.Web.Helpers.Extensions;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class TextMediaList
{
    public required string Title { get; set; }

    public string? Text { get; set; }

    public LinkList? LinkList { get; set; }

    public required List<(string Text, Accordion.Item AccordionItem)> AccordionItems { get; set; }

    public Image? Image { get; set; }

    public Video? Video { get; set; }

    public Button? PrimaryLinkButton { get; set; }

    public Button? SecondaryLinkButton { get; set; }

    public string? Variant { get; set; }

    public string? MediaPosition { get; set; }

    public string? CssClasses { get; set; }

    public static TextMediaList Create(NestedBlockTextMediaList textMediaListBlock)
    {
        Video? video = Video.Create((VideoMedia?) textMediaListBlock.Video?.FirstOrDefault()?.Content)
         .With(v =>
         {
             v.Id = (textMediaListBlock?.Video?.FirstOrDefault()?.Content as VideoMedia)?.Title?.Trim().ToLower().Replace(" ", "-");
             v.Description = (textMediaListBlock?.Video?.FirstOrDefault()?.Content as VideoMedia)?.Description;
             v.Autoplay = false;
             v.Muted = true;
             v.TotalTime = (textMediaListBlock?.Video?.FirstOrDefault()?.Content as VideoMedia)?.TotalTime;
         });

        Image? image = Image.Create(textMediaListBlock.Image)
        .With(i =>
        {
            i.ImageStyle = "text-media-list";
            i.Caption = textMediaListBlock.MediaDescription;
            i.CardOverlay = video != null ? new CardOverlay
            {
                Video = video,
                Position = "start",
                Visible = true,
            } : null;
            i.ImageHolderButton = video != null;
            i.ImageHolderAttributes = new Dictionary<string, string?>
            {
                ["aria-label"] = "Open video Modal",
                ["aria-controls"] = video != null ? $"modal-video-{video.Id}" : null,
            };
            i.ObjectFit = true;
        });

        return new TextMediaList
        {
            Title = textMediaListBlock.Title!,
            Text = textMediaListBlock.Text!.ToHtmlString(),
            Image = image,
            Video = video,
            MediaPosition = textMediaListBlock.MediaPosition,
            LinkList = LinkList
                .Create(textMediaListBlock.Items.GetSingleContentOrNull<NestedBlockTextMediaListLinks>()),
            AccordionItems = textMediaListBlock.Items
                .Using(i => i.Content as NestedBlockTextMediaListAccordionItem)
                .Select(i => (i.Text!.ToHtmlString()!, new Accordion.Item { Id = i.Key.ToString(), Title = i.Title, }))
                .ToList(),
            PrimaryLinkButton = Button.Create(textMediaListBlock.PrimaryLink)
                .With(b =>
                {
                    b.Class = "text-media-list__cta1";
                    b.Variant = "primary";
                    b.Icon = SvgAliases.Icons.ArrowTopRight;
                }),
            SecondaryLinkButton = Button.Create(textMediaListBlock.SecondaryLink)
                .With(b =>
                {
                    b.Class = "text-media-list__cta2";
                    b.Variant = "secondary";
                    b.Icon = SvgAliases.Icons.ArrowTopRight;
                }),
        };
    }
}
