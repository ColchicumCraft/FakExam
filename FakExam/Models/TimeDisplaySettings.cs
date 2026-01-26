using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using FakExam.Core.Models;

namespace FakExam.Models;


public class DisplayItem
{
    public DisplayItemType Type
    {
        get; set;
    }

    // 时间相关属性
    public string TimeText
    {
        get; set;
    }
    public string TimeFontFamily
    {
        get; set;
    }
    public double TimeFontSize
    {
        get; set;
    }
    public string TimeFontColor
    {
        get; set;
    }
    public string TimeFontWeight
    {
        get; set;
    }

    // 日期相关属性
    public string DateText
    {
        get; set;
    }
    public string WeekText
    {
        get; set;
    }
    public string DateFontFamily
    {
        get; set;
    }
    public double DateFontSize
    {
        get; set;
    }
    public string DateFontColor
    {
        get; set;
    }
    public string DateFontWeight
    {
        get; set;
    }

    // 布局属性
    public HorizontalAlignment HorizontalAlignment
    {
        get; set;
    }
    public Visibility Visibility
    {
        get; set;
    }
}