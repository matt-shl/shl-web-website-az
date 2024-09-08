using Umbraco.Cms.Core.Models.Blocks;

namespace Umbraco.Cms.Web.Common.PublishedModels;

public interface IPageHero
{
    BlockListModel? Hero { get; }
}

public partial interface ICompositionHero : IPageHero;

public partial interface ICompositionHeroPageHome : IPageHero;
