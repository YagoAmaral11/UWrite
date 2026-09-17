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
    private ProjectMetadata? bindedMetadata;
    private string? projectPath;
    private DateTimeOffset? lastModified;
    private static readonly IImage DefaultProjectImage = new Bitmap(AssetLoader.Open(new("avares://UWrite/Assets/FlaticonIcons/folder.png")));


    public IImage ProjectImage
    {
        get => DefaultProjectImage;
    }

    public string ProjectName
    {
        get
        {
            if (bindedMetadata.HasValue)
            {
                return bindedMetadata.Value.ProjectName;
            }
            else
            {
                return "Lorem Ipsum";
            }
        }
    }

    public string ProjectPath
    {
        get
        {
            if (projectPath == null)
            {
                return "UWrite/Projects/LoremIpsum.uwp";
            }
            else
            {
                return projectPath;
            }
        }
    }

    public string ProjectLastModified
    {
        get
        {
            if (lastModified.HasValue)
            {                
                return GetDateString(lastModified.Value);
            }
            else
            {                
                return GetDateString(DateTimeOffset.UnixEpoch);
            }
        }
    }



    public RecentProjectItemView()
    {        
        InitializeComponent();
    }

    private string GetDateString(DateTimeOffset dateTime)
    {
        var localTime = dateTime.ToLocalTime();
        return $"{localTime.ToString("g", CultureInfo.CurrentCulture)}";
    } 
}
