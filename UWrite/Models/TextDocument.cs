using System;
using System.Collections.Generic;
using System.Text;

namespace UWrite.Models;

public class TextDocument
{
    public string Name { get; set; }
    public ulong NextID { get; set; } // O ID do próximo TextSection a ser criado
    public uint RemovalCount { get; set; } // A quantia de TextSection removidas; Usada depois para desfragmentar as seções
    public ulong? FirstTextContainer { get; set; }
    public ulong? LastTextContainer { get; set; }
}
