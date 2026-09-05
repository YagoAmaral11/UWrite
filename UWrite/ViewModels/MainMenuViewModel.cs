using System.Collections.ObjectModel;

namespace UWrite.ViewModels;

public class RecentProjectItem
{
    public string ProjectName { get; set; } = string.Empty;
    public string ProjectPath { get; set; } = string.Empty;
    public string ModificationDate { get; set; } = string.Empty;
    public string Icon { get; set; } = "📄";
}

public class MainMenuViewModel : ViewModelBase
{    
    public ObservableCollection<RecentProjectItem> RecentProjects;    

    public MainMenuViewModel()
    {
        // Dados de exemplo - você pode substituir por dados reais do seu sistema
        RecentProjects = new ObservableCollection<RecentProjectItem>
        {
            new()
            {
                ProjectName = "My Novel",
                ProjectPath = "C:\\Users\\Documents\\MyNovel",
                ModificationDate = "Today at 14:30",
                Icon = "📖"
            },
            new()
            {
                ProjectName = "Article Draft",
                ProjectPath = "C:\\Users\\Documents\\Articles\\Draft01",
                ModificationDate = "Yesterday at 10:15",
                Icon = "📄"
            },
            new()
            {
                ProjectName = "Poetry Collection",
                ProjectPath = "C:\\Users\\Documents\\Poetry\\2024",
                ModificationDate = "3 days ago",
                Icon = "✒️"
            }
        };
    }
}