
// (c) 2023 Kazuki KOHZUKI

using System.Numerics;

namespace TAMAI.Utils;

internal static class IterUtils
{
    internal static IEnumerable<(int index, T element)> Enumerate<T>(this IEnumerable<T> source, int offset = 0)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        var index = offset;
        foreach (var element in source)
            yield return (index++, element);
    } // internal static IEnumerable<(int index, T element)> Enumerate<T> (this IEnumerable<T>, [int])

    internal static int FindNearestIndex<T>(this IEnumerable<T> source, T searchValue, ValueSearchOption searchOption = ValueSearchOption.Nearest)
        where T : IPhysicalQuantity<T>
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        var opt = (int)searchOption;
        var candidates = source.Select((v, i) => new { Index = i, Value = v })
            .Where(x => x.Value.CompareTo(searchValue) * opt <= 0)
            .Select(x => new { x.Index, Value = x.Value - searchValue })
            .OrderBy(x => x.Value);

        return searchOption switch
        {
            ValueSearchOption.EqualOrGreater => candidates.First().Index,
            ValueSearchOption.EqualOrLess => candidates.Last().Index,
            _ => candidates.OrderBy(x => Math.Abs(new ScientificValue<T>(x.Value).ValueAsDouble)).First().Index,
        };
    } // internal static int FindNearestIndex<T> (this IEnumerable<T>, T, [ValueSearchOption]) where T : IPhysicalQuantity<T>

    internal static TResult Reduce<TSource, TResult>(this IEnumerable<TSource> source, Func<TResult, TSource, TResult> callback, TResult initialValue)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        var result = initialValue;
        foreach (var item in source)
            result = callback(result, item);

        return result;
    } // internal static TResult Reduce<TSource, TResult> (this IEnumerable<TSource>, Func<TResult, TSource, TResult>, TResult)

    internal static T Sum<T>(this IEnumerable<T> source, T zero) where T : IAdditionOperators<T, T, T>
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        return source.Reduce((acc, v) => acc + v, zero);
    } // internal static T Sum<T>(this IEnumerable<T>, T) where T : IAdditionOperators<T, T, T>

    internal static T Average<T>(this T[] arr, int firstIndex, int lastIndex)
        where T : IAdditionOperators<T, T, T>, IDivisionOperators<T, int, T>
    {
        if (arr == null) throw new ArgumentNullException(nameof(arr));

        if (firstIndex < 0) throw new ArgumentOutOfRangeException(nameof(firstIndex));
        if (lastIndex >= arr.Length) throw new ArgumentOutOfRangeException(nameof(lastIndex));
        if (firstIndex > lastIndex) throw new ArgumentException("lastIndex must be greater than or equal to firstIndex.");

        if (firstIndex  == lastIndex) return arr[firstIndex];

        return arr.Where((_, i) => firstIndex < i && i <= lastIndex).Sum(arr[firstIndex]) / (lastIndex - firstIndex + 1);
    } // internal static T Average<T> (this T[], int, int)

    internal static T Average<T>(this IEnumerable<T> values) where T : IAdditionOperators<T, T, T>, IDivisionOperators<T, int, T>
    {
        var arr = values.ToArray();
        return arr.Average(0, arr.Length - 1);
    } // internal static T Average<T> (this IEnumerable<T>)
} // internal static class IterUtils
