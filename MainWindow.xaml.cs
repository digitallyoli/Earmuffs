using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.Windows.ApplicationModel.Resources;
using Windows.UI.WindowManagement;

namespace Earmuffs;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        AppWindow.TitleBar.PreferredTheme = TitleBarTheme.UseDefaultAppMode;
        AppWindow.Resize(new Windows.Graphics.SizeInt32(640, 720));
        AppWindow.Title = "Earmuffs";
        AppWindow.SetIcon(new ResourceManager().MainResourceMap.GetSubtree("Files").GetValue(@"Assets/Logo.ico").ValueAsString);
        OverlappedPresenter presenter = OverlappedPresenter.Create();
        presenter.PreferredMinimumWidth = 640;
        presenter.PreferredMinimumHeight = 500;
        presenter.SetBorderAndTitleBar(true, true);
        AppWindow.SetPresenter(presenter);
    }
}