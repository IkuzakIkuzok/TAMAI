
// (c) 2023 Kazuki KOHZUKI

using System.IO;

namespace TAMAI.Win.Plugins;

internal static class PluginManager
{
    internal static string PluginDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TAMAI", "Plugin");

    internal static void LoadPlugin(string filename)
    {
        if (Path.GetDirectoryName(filename) != PluginDir)
        {
            var src = filename;
            filename = Path.Combine(PluginDir, Path.GetFileName(src));
            File.Copy(src, filename, true);
        }
    } // internal static void LoadPlugin (string)
} // internal static class PluginManager
