using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Trees;
using Umbraco.Cms.Web.BackOffice.Trees;
using Umbraco.Cms.Web.Common.Attributes;

namespace DTNL.UmbracoCms.Web.Modules.BackgroundJobs.Section;

[Tree(BackgroundJobsConstants.SectionAlias, BackgroundJobsConstants.SectionAlias, IsSingleNodeTree = true)]
[PluginController(BackgroundJobsConstants.PluginAlias)]
public class BackgroundJobsTreeController : TreeController
{
    public BackgroundJobsTreeController(ILocalizedTextService localizedTextService, UmbracoApiControllerTypeCollection umbracoApiControllerTypeCollection, IEventAggregator eventAggregator)
        : base(localizedTextService, umbracoApiControllerTypeCollection, eventAggregator)
    {
    }

    protected override ActionResult<TreeNode?> CreateRootNode(FormCollection queryStrings)
    {
        ActionResult<TreeNode?> rootResult = base.CreateRootNode(queryStrings);
        if (rootResult.Result is not null || rootResult.Value is null)
        {
            return rootResult;
        }

        TreeNode root = rootResult.Value;
        root.RoutePath = $"{BackgroundJobsConstants.PluginAlias}/{BackgroundJobsConstants.SectionAlias}/overview";
        root.HasChildren = false;
        root.MenuUrl = null;

        return root;
    }

    protected override ActionResult<TreeNodeCollection> GetTreeNodes(string id, FormCollection queryStrings)
    {
        return TreeNodeCollection.Empty;
    }

    protected override ActionResult<MenuItemCollection> GetMenuForNode(string id, FormCollection queryStrings)
    {
        return null!;
    }
}
