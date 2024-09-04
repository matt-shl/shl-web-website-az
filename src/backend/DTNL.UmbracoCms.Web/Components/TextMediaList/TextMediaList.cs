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

    public string? CtaSupportText { get; set; }

    public string? Variant { get; set; }

    public string? MediaPosition { get; set; }

    public string? CssClasses { get; set; }

    public static TextMediaList Create(NestedBlockTextMediaList textMediaListBlock)
    {
        VideoMedia? videoMedia = textMediaListBlock.Video.GetSingleContentOrNull<VideoMedia>();
        Video? video = Video
            .Create(videoMedia)
            .With(v =>
            {
                v.Id = videoMedia!.Key.ToString();
                v.Description = videoMedia.Description;
                v.Autoplay = false;
                v.Muted = true;
                v.TotalTime = videoMedia.TotalTime;
            });

        NestedBlockImage? imageContent = textMediaListBlock.Image.GetSingleContentOrNull<NestedBlockImage>();
        Image? image = Image
            .Create(imageContent?.Image, style: "text-media-list")
            .With(i =>
            {
                i.Caption = imageContent?.Caption;
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
            });

        LinkList? linkList = LinkList.Create(textMediaListBlock.Items.GetSingleContentOrNull<NestedBlockTextMediaListLinks>());
        List<(string Text, Accordion.Item AccordionItem)>? accordions = textMediaListBlock.Items.GetSingleContentOrNull<NestedBlockTextMediaListAccordions>()?.Accordions
                .Using(i => i.Content as NestedBlockTextMediaListAccordionItem)
                .Select(i => (i.Text!.ToHtmlString()!, new Accordion.Item { Id = i.Key.ToString(), Title = i.Title, }))
                .ToList();

        return new TextMediaList
        {
            Title = textMediaListBlock.Title!,
            Text = textMediaListBlock.Text!.ToHtmlString(),
            Image = image,
            Video = video,
            MediaPosition = textMediaListBlock.MediaPosition is "left" ? "start" : "end",
            LinkList = linkList,
            AccordionItems = accordions ?? [],
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
            CtaSupportText = textMediaListBlock.ButtonsSupportText,
        };
    }
}
