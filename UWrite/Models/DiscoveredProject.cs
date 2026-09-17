using System;
using System.Collections.Generic;
using System.Text;

namespace UWrite.Models;

public class DiscoveredProject(ProjectMetadata metadata, string path, DateTimeOffset lastEdit)
{
    public ProjectMetadata Metadata { get; set;  } = metadata;
    public string Path { get; set; } = path;
    public DateTimeOffset LastEdit { get; set;  } = lastEdit;
}
