
using System;

namespace TimeWinUI.Core.Models;

public class CurrentExamContext
{
    public string Name { get; set; } = string.Empty;
    public DateTime StartTime
    {
        get; set;
    }
    public DateTime EndTime
    {
        get; set;
    }
    public int AlertMinutes { get; set; } = 15;
}
