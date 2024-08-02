using System.ComponentModel;

namespace DTNL.UmbracoCms.Web.Helpers.Extensions;

public static class EnumExtensions
{
    /// <summary>
    /// Returns the equivalent enum value for the specified type.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="defaultValue"></param>
    public static TEnum As<TEnum>(this Enum value, TEnum defaultValue = default) where TEnum : struct
    {
        return GetEnumOrDefault(value.ToString(), defaultValue);
    }

    /// <summary>
    /// Tries parsing the specified enum specified as string into the specified type.
    /// Throws an exception if this fails.
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="enumValueStr"></param>
    public static TEnum GetEnum<TEnum>(this string? enumValueStr) where TEnum : struct
    {
        if (enumValueStr.TryGetEnum(out TEnum enumValue))
        {
            return enumValue;
        }

        throw new ArgumentOutOfRangeException(nameof(enumValueStr), enumValueStr);
    }

    /// <summary>
    /// Tries parsing the specified enum specified as string into the specified type.
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="enumValueStr"></param>
    /// <param name="defaultValue"></param>
    public static TEnum GetEnumOrDefault<TEnum>(
        this string? enumValueStr,
        TEnum defaultValue = default) where TEnum : struct
    {
        if (enumValueStr.TryGetEnum(out TEnum enumValue))
        {
            return enumValue;
        }

        return defaultValue;
    }

    /// <summary>
    /// Tries parsing the specified enum specified as string into the specified type.
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="enumValueStr"></param>
    public static TEnum? GetEnumOrNull<TEnum>(
        this string? enumValueStr) where TEnum : struct
    {
        if (enumValueStr.TryGetEnum(out TEnum enumValue))
        {
            return enumValue;
        }

        return null;
    }

    /// <summary>
    /// Tries parsing the specified enum specified as string into the specified type.
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="enumValueStr"></param>
    /// <param name="enumValue"></param>
    /// <param name="defaultValue"></param>
    public static bool TryGetEnum<TEnum>(
        this string? enumValueStr,
        out TEnum enumValue,
        TEnum defaultValue = default) where TEnum : struct
    {
        if (!typeof(TEnum).IsEnum)
        {
            throw new InvalidEnumArgumentException();
        }

        if (Enum.TryParse(enumValueStr, true, out enumValue)
            && Enum.IsDefined(typeof(TEnum), enumValue))
        {
            return true;
        }

        enumValue = defaultValue;

        return false;
    }

    /// <summary>
    /// Tells if two enums have the same string value.
    /// </summary>
    /// <param name="value1"></param>
    /// <param name="value2"></param>
    public static bool IsEquivalentTo(this Enum value1, Enum value2)
    {
        return value1.IsEquivalentTo(value2.ToString());
    }

    /// <summary>
    /// Tells if the enum <paramref name="value"/> has the same string value specified in <paramref name="enumValueStr"/>
    /// </summary>
    /// <param name="value"></param>
    /// <param name="enumValueStr"></param>
    public static bool IsEquivalentTo(this Enum value, string enumValueStr)
    {
        return value.ToString().InvariantEquals(enumValueStr);
    }
}
