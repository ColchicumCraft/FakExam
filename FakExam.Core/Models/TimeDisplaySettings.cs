using System.Collections.ObjectModel;

namespace FakExam.Core.Models;

public class TimeDisplaySettings
{
    public TimeFormatSettings TimeFormat { get; set; } = new();
    public DateFormatSettings DateFormat { get; set; } = new();
    public FontSettings TimeFont { get; set; } = new();
    public FontSettings DateFont { get; set; } = new();

    public DisplayAlignmentSettings Alignment { get; set; } = new();
    public LayoutOrder LayoutOrder { get; set; } = LayoutOrder.DateOnTop;

    public ExamOverlaySettings ExamOverlay { get; set; } = new();
    public ExamLayoutPosition ExamLayoutPosition { get; set; } = ExamLayoutPosition.Bottom;

    public double ItemsSpacing { get; set; } = 20; // 垂直间距
    public int MainLayoutMarginLeft { get; set; } = 20;
    public int MainLayoutMarginTop { get; set; } = 20;
    public int MainLayoutMarginRight { get; set; } = 20;
    public int MainLayoutMarginBottom { get; set; } = 20;
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
    public string FontColor { get; set; } = "#000000";
}

public class ColorSettings
{
    public string TimeColor { get; set; } = "#000000";
    public string DateColor { get; set; } = "#000000";
    public string WeekColor { get; set; } = "#000000";
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

public class DisplayAlignmentSettings
{
    public Alignments TimeAlignment { get; set; } = Alignments.Center;
    public Alignments DateAlignment { get; set; } = Alignments.Center;
    public Alignments ExamAlignment { get; set; } = Alignments.Center;
}

public enum LayoutOrder
{
    DateOnTop,   // 日期在上，时间在下（默认）
    TimeOnTop    // 时间在上，日期在下
}
public enum ExamLayoutPosition
{
    Top,     // 在时间和日期之上
    Middle,  // 在时间和日期之间
    Bottom   // 在时间和日期之下
}

public enum Alignments
{
    Left,
    Center,
    Right,
    Hidden
}

public enum DisplayItemType
{
    Time,
    Date,
    Exam
}

public class AlignmentItem
{
    public string DisplayName
    {
        get; set;
    }
    public string Value
    {
        get; set;
    }

    public AlignmentItem(string displayName, string value)
    {
        DisplayName = displayName;
        Value = value;
    }
}

public class LayoutOrderItem
{
    public string DisplayName
    {
        get; set;
    }
    public string Value
    {
        get; set;
    }

    public LayoutOrderItem(string displayName, string value)
    {
        DisplayName = displayName;
        Value = value;
    }
}

public class ExamPositionItem
{
    public string DisplayName
    {
        get; set;
    }
    public ExamLayoutPosition Value
    {
        get; set;
    }

    public ExamPositionItem(string displayName, ExamLayoutPosition value)
    {
        DisplayName = displayName;
        Value = value;
    }
}

public class ExamOverlaySettings
{
    public FontSettings LabelFont { get; set; } = new()
    {
        FontFamily = "Segoe UI",
        FontSize = 12,
        FontWeight = 400,
        FontColor = "#8A8A8A"
    };

    public FontSettings StatusFont { get; set; } = new()
    {
        FontFamily = "Segoe UI",
        FontSize = 24,
        FontWeight = 600,
        FontColor = "#000000"
    };
    public FontSettings StartTimeFont { get; set; } = new()
    {
        FontFamily = "Segoe UI",
        FontSize = 24,
        FontWeight = 400,
        FontColor = "#000000"
    };
    public FontSettings NameFont { get; set; } = new()
    {
        FontFamily = "Segoe UI",
        FontSize = 24,
        FontWeight = 600,
        FontColor = "#000000"
    };
    public FontSettings EndTimeFont { get; set; } = new()
    {
        FontFamily = "Segoe UI",
        FontSize = 24,
        FontWeight = 400,
        FontColor = "#000000"
    };
    public FontSettings RemainingFont { get; set; } = new()
    {
        FontFamily = "Segoe UI",
        FontSize = 24,
        FontWeight = 600,
        FontColor = "#000000"
    };
}

// 下拉列表数据源
public static class DisplayDataSources
{
    public static ObservableCollection<string> FontFamilies
    {
        get;
    } = new()
    {
        "Segoe UI",
        "Microsoft YaHei UI",
        "Microsoft YaHei",
        "Arial",
        "Calibri",
        "Consolas",
        "Times New Roman",
        "SimSun",
        "SimHei",
        "KaiTi"
    };

    public static ObservableCollection<FormatItem> TimeFormats
    {
        get;
    } = new()
    {
        new FormatItem("24小时制 (HH:mm:ss)", "HH:mm:ss"),
        new FormatItem("24小时制 (HH:mm)", "HH:mm"),
        new FormatItem("12小时制 (h:mm:ss tt)", "h:mm:ss tt"),
        new FormatItem("12小时制 (h:mm tt)", "h:mm tt"),
        new FormatItem("自定义", "Custom")
    };

    public static ObservableCollection<FormatItem> DateFormats
    {
        get;
    } = new()
    {
        new FormatItem("yyyy年MM月dd日", "yyyy年MM月dd日"),
        new FormatItem("yyyy-MM-dd", "yyyy-MM-dd"),
        new FormatItem("MM/dd/yyyy", "MM/dd/yyyy"),
        new FormatItem("yyyy年M月d日", "yyyy年M月d日"),
        new FormatItem("自定义", "Custom")
    };

    public static ObservableCollection<FontWeightItem> FontWeights
    {
        get;
    } = new()
    {
        new FontWeightItem("Thin", "Thin"),
        new FontWeightItem("ExtraLight", "ExtraLight"),
        new FontWeightItem("Light", "Light"),
        new FontWeightItem("Normal", "Normal"),
        new FontWeightItem("Medium", "Medium"),
        new FontWeightItem("SemiBold", "SemiBold"),
        new FontWeightItem("Bold", "Bold"),
        new FontWeightItem("ExtraBold", "ExtraBold"),
        new FontWeightItem("Black", "Black")
    };

    public static ObservableCollection<AlignmentItem> AlignmentOptions
    {
        get;
    } = new()
    {
        new AlignmentItem("居中", "Center"),
        new AlignmentItem("靠左", "Left"),
        new AlignmentItem("靠右", "Right"),
        new AlignmentItem("隐藏", "Hidden")
    };

    public static ObservableCollection<LayoutOrderItem> LayoutOrderOptions
    {
        get;
    } = new()
    {
        new LayoutOrderItem("日期在上", "DateOnTop"),
        new LayoutOrderItem("时间在上", "TimeOnTop")
    };

    public static ObservableCollection<ExamPositionItem> ExamPositionOptions
    {
        get;
    } = new()
    {
        new ExamPositionItem("在上方", ExamLayoutPosition.Top),
        new ExamPositionItem("在中间", ExamLayoutPosition.Middle),
        new ExamPositionItem("在下方", ExamLayoutPosition.Bottom)
    };

    public static ObservableCollection<LayoutOrderItem> DashboardLayoutOrderOptions
    {
        get;
    } = new()
    {
        new LayoutOrderItem("状态面板在左，表格在右", "StatusOnLeft"),
        new LayoutOrderItem("表格在左，状态面板在右", "TableOnLeft")
    };
}
