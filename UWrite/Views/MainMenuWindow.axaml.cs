using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Collections.ObjectModel;
using UWrite.Code;
using UWrite.Models;
using UWrite.ViewModels;

namespace UWrite.Views;

public partial class MainMenuWindow : Window
{
    private MainMenuViewModel viewModel => DataContext as MainMenuViewModel;

    public MainMenuWindow()
    {        
        InitializeComponent();        
    }

}