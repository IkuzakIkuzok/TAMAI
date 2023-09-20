
// (c) 2023 Kazuki KOHZUKI

using TAMAI.Analysis;

namespace TAMAI.Win.Analysis;

internal static class DecayFittingFunctions
{
    private static readonly List<Type> functions = new() { typeof(SingleExponentialDecay) };

    static DecayFittingFunctions()
    {
        // TODO: Load functions from plugins
    } // cctor ()

    internal static IEnumerable<KeyValuePair<Guid, IDecayFunctionModel>> Functions
        => functions.Select(f => new KeyValuePair<Guid, IDecayFunctionModel>(f.GUID, (IDecayFunctionModel)Activator.CreateInstance(f)!)).ToList();
} // internal static class DecayFittingFunctions
