using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;

namespace UWrite.Views.Controls;

public partial class MenuButtonView : UserControl
{
    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public IImage? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly Bitmap defaultImage = new Bitmap(AssetLoader.Open(new Uri("avares://UWrite/Assets/FlaticonIcons/list-flaticon.png")));

    public static readonly StyledProperty<string> TextProperty = AvaloniaProperty.Register<MenuButtonView, string>(nameof(Text), "Menu Option");       
    public static readonly StyledProperty<IImage?> IconProperty = AvaloniaProperty.Register<MenuButtonView, IImage?>(nameof(Icon), defaultImage);    

    // Funcionalidade interna de clique
    private void OnButtonClick(object? sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(ClickEvent));
    }

    // Evento de clique
    public static readonly RoutedEvent<RoutedEventArgs> ClickEvent = RoutedEvent.Register<MenuButtonView, RoutedEventArgs>(nameof(Click), RoutingStrategies.Bubble);

    public event EventHandler<RoutedEventArgs>? Click
    {
        add => AddHandler(ClickEvent, value);
        remove => RemoveHandler(ClickEvent, value);
    }

    public MenuButtonView()
    {
        InitializeComponent();
    }
}