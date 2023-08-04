
// (c) 2023 Kazuki KOHZUKI

namespace TAMAI.Stats;

internal static class StatsUtils
{
    private static readonly TDist TDist = new();

    internal static double Variance(this IEnumerable<double> source, int ddof = 1)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        var s1 = .0;
        var s2 = .0;
        var n = -ddof;
        foreach (var x in source)
        {
            s1 += x;
            s2 += x * x;
            ++n;
        }
        var avg = s1 / n;
        return s2 / n - avg * avg;
    } // internal static double Variance (this IEnumerable<double>, [int])

    internal static double StandardDeviation(this IEnumerable<double> source, int ddof = 0)
        => Math.Sqrt(source.Variance(ddof));

    internal static List<int> SmirnovGrubbs(this IEnumerable<int> source, double alpha = 0.5)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        var list = source.ToList();
        while (true)
        {
            var n = list.Count;
            if (n <= 2) break;
            var t = TDist.InverseSurvivalFunction((alpha / n) / 2, n - 2);
            var tau = (n - 1) * t / Math.Sqrt(n * (n - 2) + n * t * t);
            var i_min = list.FindIndex(x => x == list.Min());
            var i_max = list.FindIndex(x => x == list.Max());
            var mu = list.Average();
            var std = list.Select(Convert.ToDouble).StandardDeviation();
            var i_far = Math.Abs(list[i_max] - mu) > Math.Abs(list[i_min] - mu) ? i_max : i_min;
            var tau_far = Math.Abs((list[i_far] - mu) / std);
            if (tau_far < tau) break;
            list.RemoveAt(i_far);
        }

        return list;
    } // internal static List<double> SmirnovGrubbs (IEnumerable<double>, [double])
} // internal static class StatsUtils
