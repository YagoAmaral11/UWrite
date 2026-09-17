using System;
using System.Collections.Generic;
using System.Text;

namespace UWrite.Models;

/// <summary>
/// Representa seções de texto que compõem um documento de texto do projeto
/// </summary>
public class TextSegment
{
    public ulong? Previous { get; set; }
    public ulong? Next {  get; set; }
    public string Text { get; set; }
}
