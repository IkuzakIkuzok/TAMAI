
// (c) 2023 Kazuki KOHZUKI

using System.Reflection;

namespace TAMAI.Data;

/// <summary>
/// Provides extension methods to use <see cref="TextValueAttribute"/>.
/// </summary>
public static class TextValueManager
{
    /// <summary>
    /// Returns a string that represents the current object which is specified by the specified attribute.
    /// </summary>
    /// <typeparam name="T">aThe attribute that specifies the representation.</typeparam>
    /// <param name="value">The current object.</param>
    /// <returns>A string that represents the current object.</returns>
    public static string ToString<T>(this Enum value) where T : TextValueAttribute
    {
        var type = value.GetType();
        var fieldInfo = type.GetField(value.ToString());
        if (fieldInfo == null) return value.ToString();

        var attrs = fieldInfo.GetCustomAttributes<T>();

        try
        {
            return attrs?.Any() ?? false
                ? attrs.First(attr => attr?.IsValid() ?? false)!.Text
                : value.ToString();
        }
        catch (InvalidOperationException)
        {
            return attrs.FirstOrDefault(attr => attr?.IsDefault ?? false)?.Text ?? value.ToString();
        }
    } // public static string ToString<T> (this Enum) where T : TextValueAttribute 

    /// <summary>
    /// Gets an enum elememt whose string representation is equal to the specified one.
    /// </summary>
    /// <typeparam name="TEnum">A type of the enum.</typeparam>
    /// <typeparam name="TAttr">The attribute that specifies the representation.</typeparam>
    /// <param name="value">The current string instance.</param>
    /// <returns>An enum element with specific string representation.</returns>
    public static TEnum ToEnum<TEnum, TAttr>(this string value) where TEnum : Enum where TAttr : TextValueAttribute
    {
        foreach (TEnum val in Enum.GetValues(typeof(TEnum)))
        {
            if (val.ToString<TAttr>() == value) return val;
        }

        return default!;
    } // public static TEnum ToEnum<TEnum, TAttr> (this string) where TEnum : Enum where TAttr : TextValueAttribute
} // internal static class TextValueManager
