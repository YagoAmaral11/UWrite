using System.Collections.ObjectModel;
using UWrite.Code;
using UWrite.Models;

namespace UWrite.ViewModels;

public class MainMenuViewModel : ViewModelBase
{
    private ObservableCollection<DiscoveredProject> projects = new();

    public bool HaveProjects => projects != null && projects.Count > 0;


    public MainMenuViewModel()
    {       
        LoadProjects();
    }

    public void LoadProjects()
    {
        var folderProjects = ProjectManager.ListProjects();

        projects.Clear();

        foreach (var project in folderProjects)
        {
            projects.Add(new DiscoveredProject(project.Value.metadata, project.Value.storageInfo.Path, project.Value.storageInfo.LastModifiedIn));
        }        
    }
}