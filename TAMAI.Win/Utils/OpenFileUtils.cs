
// (c) 2023 Kazuki KOHZUKI

using System.Diagnostics;

namespace TAMAI.Win.Utils;

internal static class OpenFileUtils
{
    internal static Process? OpenUrl(string url)
    {
        var pi = new ProcessStartInfo(url) { UseShellExecute = true };
        return Process.Start(pi);
    } // internal static Process? OpenUrl (string url
} // internal static class OpenFileUtils
