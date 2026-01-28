using Earmuffs.Helpers;
using Microsoft.UI.Xaml;
using Windows.System;

namespace Earmuffs.Views;

public sealed partial class SettingsView
{
    public SettingsView()
    {
        InitializeComponent();
        StartupToggle.IsOn = StartupHelper.IsStartupEnabled();
        StartupToggle.Toggled += StartupToggle_Toggled;
    }

    private async void StartupToggle_Toggled(object sender, RoutedEventArgs args)
    {
        if (StartupToggle.IsOn == true)
        {
            StartupHelper.EnableStartup();
        }
        else
        {
            StartupHelper.DisableStartup();
        }
        StartupToggle.IsOn = StartupHelper.IsStartupEnabled();
    }

    private async void donation_Click(object sender, RoutedEventArgs args)
    {
        await Launcher.LaunchUriAsync(new Uri("https://buymeacoffee.com/digitallyoli"));
    }

    private async void oli_Click(object sender, RoutedEventArgs args)
    {
        await Launcher.LaunchUriAsync(new Uri("https://oli.digital"));
    }

    private async void checkUpdates_Click(object sender, RoutedEventArgs args)
    {
        await Launcher.LaunchUriAsync(new Uri("https://github.com/digitallyoli/earmuffs/releases"));
    }

    private async void reportBug_Click(object sender, RoutedEventArgs args)
    {
        await Launcher.LaunchUriAsync(new Uri("https://github.com/digitallyoli/earmuffs/issues"));
    }

    private async void github_Click(object sender, RoutedEventArgs args)
    {
        await Launcher.LaunchUriAsync(new Uri("https://github.com/digitallyoli/earmuffs"));
    }
}
