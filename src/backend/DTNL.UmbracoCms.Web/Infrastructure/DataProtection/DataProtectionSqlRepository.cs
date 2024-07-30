using System.Xml.Linq;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Umbraco.Cms.Infrastructure.Scoping;

namespace DTNL.UmbracoCms.Web.Infrastructure.DataProtection;

/// <summary>
/// DataProtection <see cref="IXmlRepository"/> implementation using Sql and the <see cref="IScopeProvider"/> from Umbraco.
/// </summary>
public class DataProtectionSqlRepository : IXmlRepository
{
    private readonly IScopeProvider _scopeProvider;

    public DataProtectionSqlRepository(IScopeProvider scopeProvider)
    {
        _scopeProvider = scopeProvider;
    }

    public IReadOnlyCollection<XElement> GetAllElements()
    {
        return GetAllElementsCore().Select(key => XElement.Parse(key.Xml!)).ToList().AsReadOnly();

        IEnumerable<DataProtectionKey> GetAllElementsCore()
        {
            using IScope scope = _scopeProvider.CreateScope(autoComplete: true);
            return scope.Database.Query<DataProtectionKey>($"SELECT * FROM {nameof(DataProtectionKey)}").Where(key => !string.IsNullOrEmpty(key.Xml)).ToList();
        }
    }

    public void StoreElement(XElement element, string friendlyName)
    {
        using IScope scope = _scopeProvider.CreateScope();
        DataProtectionKey? newKey = new()
        {
            Id = 0,
            FriendlyName = friendlyName,
            Xml = element.ToString(SaveOptions.DisableFormatting),
        };

        _ = scope.Database.InsertOrUpdate(newKey);
        _ = scope.Complete();
    }
}
