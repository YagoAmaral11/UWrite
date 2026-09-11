using System;
using System.Collections.Generic;
using System.Text;

namespace UWrite.Models;


/// <summary>
///  Modelos de documentos/classificações criados pelo o usuário para o projeto, como personagens, locais, eventos, etc.
/// </summary>
public abstract class ProjectModel
{
    public string Name { get; set; }
}
