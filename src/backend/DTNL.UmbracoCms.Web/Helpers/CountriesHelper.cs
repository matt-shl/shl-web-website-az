using System.Globalization;

namespace DTNL.UmbracoCms.Web.Helpers;

public static class CountriesHelper
{
    public static List<string> CountryList()
    {
        List<string> cultureList = new();

        foreach (CultureInfo culture in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
        {
            RegionInfo regionInfo = new(culture.Name);
            if (!cultureList.Contains(regionInfo.EnglishName))
            {
                cultureList.Add(regionInfo.EnglishName);
            }
        }

        cultureList.Add("Select Country");
        cultureList.Sort();

        return cultureList;
    }
}
