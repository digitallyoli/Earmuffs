using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Earmuffs.Views;

public sealed partial class TrayIconView : UserControl
{
    public TrayIconView()
    {
        InitializeComponent();
        TrayIcon.LeftClickCommand = new RelayCommand(App.OpenMainWindow);
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        MenuFlyout.ShowAt(this);
        MenuFlyout.Hide();
    }

    private void QuitApplication()
    {
        TrayIcon.Dispose();
        App.QuitApplication();
    }
}
