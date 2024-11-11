namespace DTNL.UmbracoCms.Web.Components;

public partial class Pagination
{
    // Change the values below to customize the range of pages to show on the side and center of the pagination
    private const int SideRange = 1;
    private const int CenterRange = 1;
    private const int NumberOfPagesToShowAll = 1 + (2 * SideRange) + (2 * CenterRange);

    private int _currentPage;

    public int CurrentPage
    {
        get => _currentPage;
        set => _currentPage = Math.Max(1, value);
    }

    public bool HideInMobile { get; set; }

    public int TotalPages { get; set; }

    public List<IPaginationItem> PageItems { get; init; } = [];

    public string? CssClasses { get; set; }

    public static Pagination? Create(int pageNumber, long totalItems, int pageSize)
    {
        int totalPages = CalculateTotalPages(totalItems, pageSize);
        List<IPaginationItem> pageItems = GetPageItems(totalPages, pageNumber);

        if (totalPages < 2 || totalItems == 0 || pageItems.Count == 0)
        {
            return null;
        }

        return new Pagination
        {
            CurrentPage = pageNumber,
            TotalPages = totalPages,
            PageItems = pageItems,
        };
    }

    public static int CalculateTotalPages(long totalItems, int pageSize)
    {
        return (int) ((totalItems + pageSize - 1) / pageSize);
    }

    private static List<IPaginationItem> GetPageItems(int totalPages, int pageNumber)
    {
        if (totalPages == 0)
        {
            return [];
        }

        if (pageNumber > totalPages)
        {
            pageNumber = totalPages;
        }

        List<IPaginationItem> pageItems = [];

        bool showAll = totalPages <= NumberOfPagesToShowAll;
        int centerRange = showAll ? totalPages : CenterRange;

        // Left range
        const int leftStart = 1;
        int leftEnd = Math.Min(SideRange, pageNumber - centerRange - 1);

        for (int i = leftStart; i <= leftEnd; i++)
        {
            pageItems.Add(new PaginationItem
            {
                Page = i,
            });
        }

        // Left separator
        bool showLeftSeparator = SideRange > 0 && pageNumber - centerRange > 1 + SideRange;
        if (showLeftSeparator)
        {
            pageItems.Add(new SeparatorItem());
        }

        // Center range
        int centerStart = Math.Max(1, pageNumber - centerRange);
        int centerEnd = Math.Min(totalPages, pageNumber + centerRange);

        for (int i = centerStart; i <= centerEnd; i++)
        {
            pageItems.Add(new PaginationItem
            {
                Page = i,
            });
        }

        // Right separator
        bool showRightSeparator = SideRange > 0 && totalPages - SideRange > pageNumber + centerRange;
        if (showRightSeparator)
        {
            pageItems.Add(new SeparatorItem());
        }

        // Right range
        int rightStart = Math.Max(totalPages - SideRange + 1, pageNumber + centerRange + 1);

        for (int i = rightStart; i <= totalPages; i++)
        {
            pageItems.Add(new PaginationItem
            {
                Page = i,
            });
        }

        return pageItems;
    }
}
