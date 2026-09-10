using System;
using System.Collections.Generic;
using System.Text;

namespace UWrite.Models;

public class TextSection
{
    public ulong? Previous { get; set; }
    public ulong? Next {  get; set; }
    public string Text { get; set; }
}
