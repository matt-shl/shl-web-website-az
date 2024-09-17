using System.Globalization;
using DTNL.UmbracoCms.Web.Infrastructure.DependencyInjection;
using Examine;
using Examine.Search;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Examine;

namespace DTNL.UmbracoCms.Web.Services;

public interface ISearchService
{
    IEnumerable<PublishedSearchResult> Search(
        string searchQuery,
        IEnumerable<int>? ids,
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

    public IEnumerable<PublishedSearchResult> Search(string searchQuery, IEnumerable<int>? ids, int page, int pageSize, out long totalRecords)
    {
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

        if (ids is not null)
        {
            queryBuilder = queryBuilder
                .And()
                .GroupedOr([ExamineFieldNames.ItemIdFieldName], ids.Select(id => $"{id}").ToArray());
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
