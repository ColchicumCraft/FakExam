using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakExam.Core.Models;

public sealed class AutoLoadProfileSettings
{
    public bool Enabled
    {
        get; set;
    }
    public string? FilePath
    {
        get; set;
    }
}
