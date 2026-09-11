using System;
using System.Collections.Generic;
using System.Text;
using UWrite.Code;

namespace UWrite.Models;

/// <summary>
/// Representa um projeto no UWrite, contendo suas informações e seu manager
/// </summary>
public class Project
{
    public string Name { get; set; }
    public ProjectManager ProjectManager;
    public ProjectMetadata Metadata => ProjectManager.GetProjectMetadata();

    public Project(string name)
    {
        Name = name;        
        ProjectManager = new ProjectManager(Name);
    }
}