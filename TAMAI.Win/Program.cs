
// (c) 2023 Kazuki KOHZUKI

using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using TAMAI.Data;
using TAMAI.Win.Config;
using TAMAI.Win.Controls;

namespace TAMAI.Win;

internal static class Program
{
    internal static readonly string AppDataPath = Path.Combine(
               Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
               "TAMAI"
           );

    private static string? crushReportLocation;

    internal static AppConfig Config { get; } = AppConfig.Load();

    internal static Color GradientStart
    {
        get => Config.AppearanceConfig.ColorGradientConfig.StartColor.Color;
        set
        {
            Config.AppearanceConfig.ColorGradientConfig.StartColor.Color = value;
            Config.Save();
        }
    }

    internal static Color GradientEnd
    {
        get => Config.AppearanceConfig.ColorGradientConfig.EndColor.Color;
        set
        {
            Config.AppearanceConfig.ColorGradientConfig.EndColor.Color = value;
            Config.Save();
        }
    }

    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main(string[] args)
    {
#if !DEBUG
        Application.ThreadException += UnhandledExceptionHandler;
        Thread.GetDomain().UnhandledException += UnhandledExceptionHandler;
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
#endif

        crushReportLocation = ConfigurationManager.AppSettings["crash-report-location"]?.ExpandEnvironmentVariables();

        var usTasConfig = (NameValueCollection)ConfigurationManager.GetSection("usTAS");
        if (usTasConfig != null)
        {
            string? s;
            if (!string.IsNullOrEmpty(s = usTasConfig["suffix-a"])) MicroSecondTasData.ASignalSuffix = s;
            if (!string.IsNullOrEmpty(s = usTasConfig["suffix-b"])) MicroSecondTasData.BSignalSuffix = s;
            if (!string.IsNullOrEmpty(s = usTasConfig["suffix-a-b"])) MicroSecondTasData.DiffSignalSuffix = s;
            if (!string.IsNullOrEmpty(s = usTasConfig["suffix-a-b-tdm"])) MicroSecondTasData.SmoothedSignalSuffix = s;
            if (!string.IsNullOrEmpty(s = usTasConfig["suffix-wavelength"])) MicroSecondTasData.WavelengthSuffix = s;
        }

        typeof(Form).GetField("defaultIcon", BindingFlags.NonPublic | BindingFlags.Static)?.SetValue(null, Properties.Resources.Icon);

        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm(args.FirstOrDefault()));
    } // private static void Main (string[])

#if !DEBUG

    private static void UnhandledExceptionHandler(object? sender, ThreadExceptionEventArgs e)
        => UnhandledExceptionHandler(e.Exception);

    private static void UnhandledExceptionHandler(object? sender, UnhandledExceptionEventArgs e)
        => UnhandledExceptionHandler((Exception)e.ExceptionObject);

    private static void UnhandledExceptionHandler(Exception e)
    {
        e.MakeCrashReport();
        Environment.Exit(1);
    } // private static void UnhandledExceptionHandler (Exception)

#endif

    internal static void MakeCrashReport(this Exception e)
    {
        try
        {
            if (string.IsNullOrEmpty(crushReportLocation)) return;
            Directory.CreateDirectory(crushReportLocation);
            var filename = Path.Combine(
                crushReportLocation,
                $"cr-{DateTime.UtcNow:yyyy-MM-ddTHHmmss.fffffff}.txt"
            );
            using var sw = new StreamWriter(filename, false, Encoding.UTF8);

            sw.WriteLine(e.GetType().Name);
            sw.WriteLine(e.Message);
            sw.WriteLine(e.StackTrace);

            var inner = e.InnerException;
            var i = 0;
            while (inner != null)
            {
                sw.WriteLine($"=== Inner exception {++i} ===");
                sw.WriteLine(inner.GetType().Name);
                sw.WriteLine(inner.Message);
                sw.Write(inner.StackTrace);
                inner = inner.InnerException;
            }
        }
        catch
        {
            Debug.WriteLine(e.Message);
            Debug.WriteLine(e.StackTrace);
        }
    } // internal static void MakeCrashReport (this Exception)

    private static string ExpandEnvironmentVariables(this string s)
        => Environment.ExpandEnvironmentVariables(s);
} // internal static class Program
