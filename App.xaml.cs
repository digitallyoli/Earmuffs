using Earmuffs.Services;
using H.NotifyIcon;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using System.Diagnostics;
using System.Runtime.InteropServices;
using WinRT.Interop;

namespace Earmuffs;

public sealed partial class App : Application
{
    public static MainWindow? MainWindow { get; set; }
    public static VolumeService? VolumeService { get; set; }

    public App()
    {
        InitializeComponent();
        VolumeService = new();
        MainWindow = new();
        MainWindow.Closed += MainWindow_Closed;
        if (Environment.GetCommandLineArgs().Contains("/autostart") == false)
        {
            MainWindow?.Activate();
        }
        else
        {
            MainWindow?.Hide();
        }
    }

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        AppInstance current = AppInstance.GetCurrent();
        AppInstance main = AppInstance.FindOrRegisterForKey("Earmuffs");
        if (!main.IsCurrent)
        {
            await main.RedirectActivationToAsync(current.GetActivatedEventArgs());
            Process.GetCurrentProcess().Kill();
            return;
        }
        current.Activated += AppInstance_Activated;
        Debug.WriteLine(args.Arguments.ToString());
    }

    private static void AppInstance_Activated(object? sender, AppActivationArguments args)
    {
        OpenMainWindow();
    }

    public static void OpenMainWindow()
    {
        MainWindow?.Show();
        SetForegroundWindow(WindowNative.GetWindowHandle(MainWindow));
    }

    private static bool _forceQuit = false;

    private static void MainWindow_Closed(object sender, WindowEventArgs args)
    {
        MainWindow?.Hide();
        args.Handled = !_forceQuit;
    }

    public static void QuitApplication()
    {
        _forceQuit = true;
        MainWindow?.Close();
    }

    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);
}
