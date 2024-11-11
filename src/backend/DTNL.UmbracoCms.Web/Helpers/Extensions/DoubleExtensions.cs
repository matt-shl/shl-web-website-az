using System.Globalization;

namespace DTNL.UmbracoCms.Web.Helpers.Extensions;

public static class DoubleExtensions
{
    public static string ToStringInvariant(this double value)
    {
        return value.ToString(CultureInfo.InvariantCulture);
    }
}
