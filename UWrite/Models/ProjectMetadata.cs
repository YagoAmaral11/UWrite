using System;
using System.Collections.Generic;
using System.Text;

namespace UWrite.Models;

/// <summary>
/// Guarda dados de metadados do projeto, como a versão da aplicação usado para esse projeto, etc.
/// </summary>
public struct ProjectMetadata
{
    public int ApplicationVersion { get; set; }
}