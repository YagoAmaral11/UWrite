using System;
using System.Collections.Generic;
using System.Text;

namespace UWrite.Models;

/// <summary>
/// Representa um documento de texto do projeto
/// </summary>
public class TextDocument
{
    public string Name { get; set; }
    public ulong NextID { get; set; } // O ID do próximo TextSegment a ser criado
    public uint RemovalCount { get; set; } // A quantia de TextSegment removidas; Usada depois para desfragmentar as seções
    public ulong? FirstTextContainer { get; set; }
    public ulong? LastTextContainer { get; set; }
}
