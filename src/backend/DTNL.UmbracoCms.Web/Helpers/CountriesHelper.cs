using System.Globalization;

namespace DTNL.UmbracoCms.Web.Helpers;

public static class CountriesHelper
{
    public static List<string> CountryList()
    {
        List<string> CultureList = new List<string>();

        CultureInfo[] getCultureInfo = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

        foreach (CultureInfo culture in getCultureInfo)
        {
            RegionInfo regionInfo = new RegionInfo(culture.Name);
            if (!CultureList.Contains(regionInfo.EnglishName))
            {
                CultureList.Add(regionInfo.EnglishName);
            }
        }

        CultureList.Add("Select Country");
        CultureList.Sort();


        return CultureList;
    }
}
