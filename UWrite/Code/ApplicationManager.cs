using System;
using System.Collections.Generic;
using System.Text;
using UWrite.Models;

namespace UWrite.Code;

/// <summary>
/// Gerencia os projetos carregados na aplicação, permitindo acessar os projetos carregados e seus metadados.
/// </summary>
public static class ApplicationManager
{
    private static readonly Dictionary<string, Project> loadedProjects = [];
    public static IReadOnlyDictionary<string, Project> Projects => loadedProjects;

}
