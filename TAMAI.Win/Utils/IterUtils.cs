
// (c) 2023 Kazuki KOHZUKI

namespace TAMAI.Win.Utils;

internal static class IterUtils
{
    internal static IEnumerable<(int index, T element)> Enumerate<T>(this IEnumerable<T> source, int offset = 0)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        var index = offset;
        foreach (var element in source)
            yield return (index++, element);
    } // internal static IEnumerable<(int index, T element)> Enumerate<T> (this IEnumerable<T>, [int])
} // internal static class IterUtils
