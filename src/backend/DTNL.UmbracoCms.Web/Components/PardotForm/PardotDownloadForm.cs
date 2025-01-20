using DTNL.UmbracoCms.Web.Helpers.Aliases;
using DTNL.UmbracoCms.Web.Models.BrandfolderAssets;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Components;

public class PardotDownloadForm : PardotForm
{
    public override string ActionSubmitLabelKey => TranslationAliases.Forms.DownloadForm.Title;

    public override string ActionSuccessLabelKey => TranslationAliases.Forms.EventForm.SubmissionSuccessMessage;

    public override string ActionErrorLabelKey => TranslationAliases.Forms.EventForm.SubmissionErrorMessage;

    public override string ConsentFieldName => "cf_Consent Given";

    public required string FileUrl { get; set; }

    public required string FileName { get; set; }

    public static PardotDownloadForm? Create(NestedBlockDownloadItem downloadItem)
    {
        if (BrandfolderAttachment.Create(downloadItem.File) is not { } asset)
        {
            return null;
        }

        //TEMP SOLUTION = DTNL-17347
        Dictionary<string, string> fileToUrlMap = new()
        {
            // Download - WhitePaper
            { "Molly® Autoinjector White Paper", "https://go.shl-medical.com/l/1046193/2024-12-13/qll2" },
            { "Auto Injectors From Planning to Launch", "https://go.shl-medical.com/l/1046193/2024-12-13/qlk8" },

            // Download - Articles
            { "Revolutionizing Drug-Device Development with SHL Medical's Elexy", "https://go.shl-medical.com/l/1046193/2024-12-13/qm1v" },
            { "Revolutionizing autoinjector assembly with SHL’s Semi-Modular Automatic Robot Track", "https://go.shl-medical.com/l/1046193/2024-12-13/qm1r" },
            { "SHL Medical's Elexy™: Redefining Possibilities", "https://go.shl-medical.com/l/1046193/2024-12-13/qlyy" },
            { "Empowering self-treatment to tackle cardiometabolic diseases", "https://go.shl-medical.com/l/1046193/2024-12-13/qll5" },
            { "Automating high-mix, low-to-medium-volume autoinjector assembly", "https://go.shl-medical.com/l/1046193/2025-01-20/rrb8" },

            // Alliance partnerships - Thought leadership
            { "Developing large volume cartridge for Maggie 5.0","https://go.shl-medical.com/l/1046193/2025-01-17/rntc" },
            { "Delivering higher viscosities using 8 mm PFS with Molly 2.25","https://go.shl-medical.com/l/1046193/2025-01-17/rntg" },
            { "Improving patient adherence in clinical trials with Molly Connected Cap","https://go.shl-medical.com/l/1046193/2025-01-17/rntk" },
        };

        // The file name to search for
        string fileName = downloadItem.Title ?? string.Empty;

        // Try to get the URL from the dictionary
        string actionUrl = fileToUrlMap.TryGetValue(fileName, out string? url)
                                ? url
                                : "https://go.shl-medical.com/l/1046193/2024-11-08/nrq8"; // Default URL

        return new()
        {
            Id = Guid.NewGuid().ToString(),
            ActionUrl = actionUrl,
            FileUrl = asset.Url,
            FileName = asset.FileName ?? "",
            GtmAttributes = $"{{'event': 'receive_download','product_datasheet': '{asset.FileName}'}}",
        };
    }
}
