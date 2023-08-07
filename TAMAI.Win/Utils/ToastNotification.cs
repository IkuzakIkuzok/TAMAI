
// (c) 2023 Kazuki KOHZUKI

using Microsoft.Toolkit.Uwp.Notifications;
using TAMAI.Win.Properties;
using ToastCallback = System.Action<System.Collections.Generic.IDictionary<string, object>>;

namespace TAMAI.Win.Utils;

internal class ToastNotification
{
    #region callbacks

    private static readonly Dictionary<string, ToastCallback> callbacks = new();

    static ToastNotification()

    {
        ToastNotificationManagerCompat.OnActivated += OnActivatedEventHandler;
    } // cctor ()

    private static void OnActivatedEventHandler(ToastNotificationActivatedEventArgsCompat e)
    {
        try
        {
            if (callbacks.TryGetValue(e.Argument, out var callback))
                callback.Invoke(e.UserInput);
        }
        catch (Exception ex)
        {
            ex.MakeCrashReport();
        }
        finally
        {
            RemoveCallbacks(e.Argument);
        }
    } // private static void OnActivatedEventHandler (ToastNotificationActivatedEventArgsCompat)

    private static void RemoveCallbacks(ToastArgument args)
    {
        var id = args.ToastId;

        var l = new HashSet<string>();
        foreach (var arg in callbacks.Keys)
        {
            if (((ToastArgument)arg).ToastId == id)
                l.Add(arg);
        }

        foreach (var arg in l)
            callbacks.Remove(arg);
    } // private static void RemoveCallbacks (ToastArgument)

    private class ToastArgument : Dictionary<string, string?>
    {
        internal string? ToastId
        {
            get
            {
                if (TryGetValue("toastId", out var id)) return id;
                return default;
            }
            set => this["toastId"] = value;
        }

        public static implicit operator ToastArgument(string arg)
        {
            var ta = new ToastArgument();

            var pairs = arg.Split('&');

            foreach (var pair in pairs)
            {
                var data = pair.Split('=');
                ta.Add(data[0], data[1]);
            }

            return ta;
        } // public static implicit operator ToastArgument (string)
    } // private class ToastArgument : Dictionary<string, string?>

    #endregion callbacks

    internal static int id = 0;

    internal int Id { get; }

    protected ToastContentBuilder contentBuilderInternal = new();

    internal ToastNotification()
    {
        this.Id = id++;
    } // ctor ()

    internal ToastNotification(string message) : this(message, Resources.ProductName) { }

    internal ToastNotification(string message, string title) : this()
    {
        AddText(title);
        AddText(message);
    } // ctor (string, string)

    internal void Show()
        => this.contentBuilderInternal.Show();

    internal static void Show(string message)
        => new ToastNotification(message)
        .AddButton("OK", null)
        .Show();

    internal static void Show(string message, string title)
        => new ToastNotification(message, title)
        .AddButton("OK", null)
        .Show();

    internal ToastNotification AddText(string text)
    {
        contentBuilderInternal.AddText(text);
        return this;
    } // internal ToastNotification AddText (string)

    internal ToastNotification AddButton(string text, ToastCallback? callback)
    {
        var buttonId = $"toastId={this.Id}&buttonId={id++}";
        if (callback != null)
            callbacks.Add(buttonId, callback);
        contentBuilderInternal.AddButton(new ToastButton(text, buttonId));
        return this;
    } // internal ToastNotification AddButton (string, ToastCallback?)
} // internal class ToastNotification
