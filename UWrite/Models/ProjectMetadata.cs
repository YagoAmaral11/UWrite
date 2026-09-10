using System;
using System.Collections.Generic;
using System.Text;

namespace UWrite.Models;

/*
 *      Guarda dados de metadados do projeto, como a versão da aplicação usado para esse projeto, etc.
 */

public struct ProjectMetadata
{
    public int ApplicationVersion { get; set; }
}