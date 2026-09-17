using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Globalization;
using System.Runtime.Serialization;
using UWrite.Models;

namespace UWrite.Views.Controls.MainMenu;

public partial class RecentProjectItemView : UserControl
{
    private DiscoveredProject Project
    {
        get
        {
            if (DataContext != null && DataContext is DiscoveredProject project)
                return project;
            else
                return DefaultPlaceholderProject;
        }
    }
    private static readonly IImage DefaultProjectImage = new Bitmap(AssetLoader.Open(new("avares://UWrite/Assets/FlaticonIcons/folder.png")));
    private static readonly DiscoveredProject DefaultPlaceholderProject = new(new(), "UWrite/Projects/Project.uwp", DateTimeOffset.UnixEpoch);    


    public string ProjectName => Project.Metadata.ProjectName;
    public string ProjectPath => Project.Path;
    public string ProjectLastModified => GetDateString(Project.LastEdit);
    public IImage ProjectImage
    {
        get => DefaultProjectImage;
    }


    public RecentProjectItemView()
    {        
        InitializeComponent();        
    }


    private string GetDateString(DateTimeOffset dateTime)
    {
        var localTime = dateTime;
        return $"{localTime.ToString("g", CultureInfo.CurrentCulture)}";
    } 
}
