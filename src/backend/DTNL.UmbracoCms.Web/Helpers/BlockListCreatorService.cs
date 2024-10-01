using System.Reflection;
using Newtonsoft.Json;
using Umbraco.Cms.Core;

namespace DTNL.UmbracoCms.Web.Helpers;

public static class BlockListCreatorService
{
    /// <summary>
    /// Creates the Block List JSON from the passed in collection of items. Each item needs to have the <see cref="JsonPropertyAttribute"/> that corresponds to the property alias of the element doc type it is based on
    /// </summary>
    /// <typeparam name="T">The type of your item.</typeparam>
    /// <param name="items">The collection of items to create.</param>
    /// <param name="contentTypeKey">The GUID for the element type (tip: you can look this up in God Mode!).</param>
    /// <param name="settingsData">Optional settings data - defaults to empty settings.</param>
    /// <returns>JSON suitable for Block List.</returns>
    /// <exception cref="ArgumentNullException">Raised if items are null.</exception>
    public static string GetBlockListJsonFor<T>(IEnumerable<T> items, Guid contentTypeKey, List<Dictionary<string, string>>? settingsData = null)
        where T : class
    {
        List<Dictionary<string, string>> contentList = [];
        List<Dictionary<string, string>> dictionaryUdi = [];

        foreach (T item in items)
        {
            string udi = new GuidUdi("element", Guid.NewGuid()).ToString();

            PropertyInfo[] props = item.GetType().GetProperties();

            Dictionary<string, string> propertyValues = new()
                {
                    { "contentTypeKey", contentTypeKey.ToString() },
                    { "udi", udi },
                };

            foreach (PropertyInfo prop in props)
            {
                string propertyValue = prop.GetValue(item)?.ToString() ?? string.Empty;
                if (prop.PropertyType == typeof(bool))
                {
                    propertyValue = propertyValue switch
                    {
                        "False" => "0",
                        "True" => "1",
                        _ => "0",
                    };
                }

                propertyValues.Add(prop.Name, propertyValue);
            }

            contentList.Add(propertyValues);

            dictionaryUdi.Add(new Dictionary<string, string> { { "contentUdi", udi } });
        }

        BlockList blockListNew = new()
        {
            Layout = new BlockListUdi(dictionaryUdi),
            ContentData = contentList,
            SettingsData = settingsData ?? [],
        };

        return JsonConvert.SerializeObject(blockListNew);
    }

    public static string GetNestedBlockListJsonFor<T>(ICollection<T> items, Guid contentTypeKey, List<Dictionary<string, string>>? settingsData = null)
        where T : class
    {
        if (items.Count == 0)
        {
            return string.Empty;
        }

        List<Dictionary<string, string>> contentList = [];
        List<Dictionary<string, string>> dictionaryUdi = [];

        foreach (T item in items)
        {
            string udi = new GuidUdi("element", Guid.NewGuid()).ToString();

            PropertyInfo[] props = item.GetType().GetProperties();

            Dictionary<string, string> propertyValues = new()
                {
                    { "contentTypeKey", contentTypeKey.ToString() },
                    { "udi", udi },
                };

            foreach (PropertyInfo prop in props)
            {
                if (prop.PropertyType.IsArray)
                {
                    List<object> propValueList = new();

                    if (prop.GetValue(item) is Array propValueArray)
                    {
                        foreach (object? value in propValueArray)
                        {
                            if (value != null)
                            {
                                string arrayItemJson = GetNestedBlockListJsonFor(new[] { value }, contentTypeKey, settingsData);
                                propValueList.Add(arrayItemJson);
                            }
                        }
                    }

                    propertyValues.Add(prop.Name, JsonConvert.SerializeObject(propValueList));
                }
                else
                {
                    string propertyValue = prop.GetValue(item)?.ToString() ?? string.Empty;
                    if (prop.PropertyType == typeof(bool))
                    {
                        propertyValue = propertyValue switch
                        {
                            "False" => "0",
                            "True" => "1",
                            _ => "0",
                        };
                    }

                    propertyValues.Add(prop.Name, propertyValue);
                }
            }

            contentList.Add(propertyValues);

            dictionaryUdi.Add(new Dictionary<string, string> { { "contentUdi", udi } });
        }

        BlockList blockListNew = new()
        {
            Layout = new BlockListUdi(dictionaryUdi),
            ContentData = contentList,
            SettingsData = settingsData ?? new List<Dictionary<string, string>>(),
        };

        return JsonConvert.SerializeObject(blockListNew);
    }

    internal sealed class BlockList
    {
        [JsonProperty("layout")]
        public BlockListUdi Layout { get; set; } = null!;

        [JsonProperty("contentData")]
        public List<Dictionary<string, string>> ContentData { get; set; } = null!;

        [JsonProperty("settingsData")]
        public List<Dictionary<string, string>> SettingsData { get; set; } = null!;
    }

    internal sealed class BlockListUdi
    {
        public BlockListUdi(List<Dictionary<string, string>> items)
        {
            ContentUdi = items;
        }

        [JsonProperty("Umbraco.BlockList")]
        public List<Dictionary<string, string>> ContentUdi { get; set; }
    }
}
