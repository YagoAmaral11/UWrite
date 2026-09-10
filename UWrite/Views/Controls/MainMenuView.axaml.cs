using Avalonia.Controls;
using UWrite.ViewModels;

namespace UWrite.Views;

public partial class MainMenuView : UserControl
{
    public MainMenuView()
    {
        InitializeComponent();
        DataContext = new MainMenuViewModel();
    }
}
