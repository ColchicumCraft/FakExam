namespace TimeWinUI.Core.Models;

public class TimeDisplaySettings
{
    public TimeFormatSettings TimeFormat { get; set; } = new();
    public DateFormatSettings DateFormat { get; set; } = new();
    public FontSettings TimeFont { get; set; } = new();
    public FontSettings DateFont { get; set; } = new();
}

public class TimeFormatSettings
{
    public string Format { get; set; } = "HH:mm:ss";
    public bool Use24Hour { get; set; } = true;
    public bool ShowSeconds { get; set; } = true;
    public string CustomFormat { get; set; } = "HH:mm:ss";
}

public class DateFormatSettings
{
    public string Format { get; set; } = "yyyy年MM月dd日";
    public bool ShowWeek { get; set; } = true;
    public bool ShowYear { get; set; } = true;
    public string CustomFormat { get; set; } = "yyyy年MM月dd日";
}

public class FontSettings
{
    public string FontFamily { get; set; } = "Segoe UI";
    public double FontSize { get; set; } = 72;
    public int FontWeight { get; set; } = 700; // Bold = 700, Normal = 400
    public string FontColor { get; set; } = "#FFFFFF";
}

public class ColorSettings
{
    public string TimeColor { get; set; } = "#FFFFFF";
    public string DateColor { get; set; } = "#CCCCCC";
    public string WeekColor { get; set; } = "#AAAAAA";
}

// 辅助类
public class FormatItem
{
    public string DisplayName
    {
        get; set;
    }
    public string Value
    {
        get; set;
    }

    public FormatItem(string displayName, string value)
    {
        DisplayName = displayName;
        Value = value;
    }
}

public class FontWeightItem
{
    public string DisplayName
    {
        get; set;
    }
    public string Value
    {
        get; set;
    }

    public FontWeightItem(string displayName, string value)
    {
        DisplayName = displayName;
        Value = value;
    }
}

public class ColorItem
{
    public string DisplayName
    {
        get; set;
    }
    public string Value
    {
        get; set;
    }

    public ColorItem(string displayName, string value)
    {
        DisplayName = displayName;
        Value = value;
    }
}