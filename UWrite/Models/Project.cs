using System;
using System.Collections.Generic;
using System.Text;

namespace UWrite.Models;

public class Project
{
    public string Name { get; set; }
    public ProjectMetadata Metadata { get; set; }

    public Project(string name)
    {
        Name = name;
        Metadata = new ProjectMetadata();
    }

}