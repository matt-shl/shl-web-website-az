using Umbraco.Cms.Core.Models.Blocks;

namespace DTNL.UmbracoCms.Web.Helpers.Extensions;

public static class BlockListModelExtensions
{
    public static T? GetSingleContentOrNull<T>(this BlockListModel? blockList)
        where T : class
    {
        if (blockList.Using(v => v.Content as T).TryGetSingle(out T? block))
        {
            return block;
        }

        return null;
    }
}
