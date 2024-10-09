using System.Globalization;
using DTNL.UmbracoCms.Web.Infrastructure.DependencyInjection;
using DTNL.UmbracoCms.Web.Models.Filters;
using Examine;
using Examine.Search;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Examine;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace DTNL.UmbracoCms.Web.Services;

public interface ISearchService
{
    IEnumerable<PublishedSearchResult> Search(
        string? searchQuery,
        SearchFilters? filters,
        int page,
        int pageSize,
        out long totalRecords);
}

[Transient]
public class SearchService : ISearchService
{
    private static readonly HashSet<string> ReturnedQueryFields =
        [ExamineFieldNames.ItemIdFieldName, ExamineFieldNames.CategoryFieldName];

    private readonly IExamineManager _examineManager;
    private readonly IUmbracoContextAccessor _umbracoContextAccessor;

    public SearchService(IExamineManager examineManager, IUmbracoContextAccessor umbracoContextAccessor)
    {
        _examineManager = examineManager;
        _umbracoContextAccessor = umbracoContextAccessor;
    }

    public IEnumerable<PublishedSearchResult> Search(
        string? searchQuery,
        SearchFilters? filters,
        int page,
        int pageSize,
        out long totalRecords)
    {
        if (searchQuery.IsNullOrWhiteSpace() || searchQuery.Length < 3)
        {
            totalRecords = 0;
            return [];
        }

        int skip = (page - 1) * pageSize;
        string culture = CultureInfo.CurrentCulture.Name;
        const string indexName = Constants.UmbracoIndexes.ExternalIndexName;

        if (!_examineManager.TryGetIndex(indexName, out IIndex? index) || index is not IUmbracoIndex umbIndex)
        {
            throw new InvalidOperationException(
                $"No index found by name {indexName} or is not of type {typeof(IUmbracoIndex)}");
        }

        IQuery? query = umbIndex.Searcher.CreateQuery(IndexTypes.Content);

        // Only search the specified culture
        // Get all index fields suffixed with the culture name supplied
        string[] fields = umbIndex.GetCultureAndInvariantFields(culture).ToArray();
        IBooleanOperation? queryBuilder = query.ManagedQuery(searchQuery, fields);

        if (filters?.Ids is not null)
        {
            queryBuilder = queryBuilder
                .And()
                .GroupedOr([ExamineFieldNames.ItemIdFieldName], filters.Ids.Select(id => $"{id}").ToArray());
        }

        if (filters?.PageTypes is not null)
        {
            queryBuilder = queryBuilder
                .And()
                .GroupedOr([$"{nameof(ICompositionContentDetails.Type)}_{culture}".ToLowerInvariant()], filters.PageTypes.ToArray());
        }

        // Filter selected fields because results are loaded from the published snapshot based on these
        IOrdering? queryExecutor = queryBuilder.SelectFields(ReturnedQueryFields);

        ISearchResults? results = skip == 0 && pageSize == 0
            ? queryExecutor.Execute()
            : queryExecutor.Execute(QueryOptions.SkipTake(skip, pageSize));

        totalRecords = results.TotalItemCount;

        return results.ToPublishedSearchResults(_umbracoContextAccessor.GetRequiredUmbracoContext().PublishedSnapshot.Content);
    }
}
