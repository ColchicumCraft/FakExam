using System.Collections.ObjectModel;

namespace FakExam.Core.Models;

public class DashboardDisplaySettings
{
    // 状态面板设置
    public FontSettings TitleFont
    {
        get; set;
    } = new()
    {
        FontFamily = "Segoe UI",
        FontSize = 22,
        FontWeight = 600,
        FontColor = "#000000"
    };

    public FontSettings MessageFont
    {
        get; set;
    } = new()
    {
        FontFamily = "Segoe UI",
        FontSize = 14,
        FontWeight = 400,
        FontColor = "#6D6D6D"
    };

    public FontSettings StatusLabelFont
    {
        get; set;
    } = new()
    {
        FontFamily = "Segoe UI",
        FontSize = 14,
        FontWeight = 400,
        FontColor = "#6D6D6D"
    };

    // 四个状态值字体
    public FontSettings CurrentExamNameFont
    {
        get; set;
    } = new()
    {
        FontFamily = "Segoe UI",
        FontSize = 18,
        FontWeight = 600,
        FontColor = "#000000"
    };

    public FontSettings CurrentExamTimeRangeFont
    {
        get; set;
    } = new()
    {
        FontFamily = "Segoe UI",
        FontSize = 16,
        FontWeight = 400,
        FontColor = "#000000"
    };

    public FontSettings RemainingTimeTextFont
    {
        get; set;
    } = new()
    {
        FontFamily = "Segoe UI",
        FontSize = 16,
        FontWeight = 600,
        FontColor = "#000000"
    };

    public FontSettings CurrentStatusTextFont
    {
        get; set;
    } = new()
    {
        FontFamily = "Segoe UI",
        FontSize = 16,
        FontWeight = 600,
        FontColor = "#000000"
    };

    // 布局设置
    public DashboardLayoutOrder LayoutOrder { get; set; } = DashboardLayoutOrder.StatusOnLeft;

    public FontSettings CurrentTimeFont
    {
        get; set;
    } = new()
    {
        FontFamily = "Segoe UI",
        FontSize = 36,
        FontWeight = 700,
        FontColor = "#000000"
    };

    // 表格设置
    public FontSettings TableHeaderFont
    {
        get; set;
    } = new()
    {
        FontFamily = "Segoe UI",
        FontSize = 14,
        FontWeight = 600,
        FontColor = "#000000"
    };

    public FontSettings TableContentFont
    {
        get; set;
    } = new()
    {
        FontFamily = "Segoe UI",
        FontSize = 14,
        FontWeight = 400,
        FontColor = "#000000"
    };

    // 列可见性
    public ColumnVisibilitySettings ColumnVisibility { get; set; } = new();
}

public enum DashboardLayoutOrder
{
    StatusOnLeft,    // 状态面板在左，表格在右（默认）
    TableOnLeft      // 表格在左，状态面板在右
}

public class ColumnVisibilitySettings
{
    public bool ShowDateColumn { get; set; } = true;
    public bool ShowNameColumn { get; set; } = true;
    public bool ShowStartColumn { get; set; } = true;
    public bool ShowEndColumn { get; set; } = true;
    public bool ShowStatusColumn { get; set; } = true;
}