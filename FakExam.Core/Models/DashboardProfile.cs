
using System;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace FakExam.Core.Models;

public class DashboardProfile
{
    [JsonProperty("examName")]
    public string ExamName
    {
        get; set;
    }

    [JsonProperty("message")]
    public string Message
    {
        get; set;
    }

    [JsonProperty("examInfos")]
    public ObservableCollection<ExamInfo> ExamInfos { get; set; } = new ObservableCollection<ExamInfo>();
}

public class ExamInfo
{
    [JsonProperty("name")]
    public string Name
    {
        get; set;
    }

    [JsonProperty("start")]
    public string Start
    {
        get; set;
    }

    [JsonProperty("end")]
    public string End
    {
        get; set;
    }

    [JsonProperty("alertTime")]
    public int AlertTime
    {
        get; set;
    }

    [JsonProperty("materials")]
    public ObservableCollection<ExamMaterial> Materials
    {
        get; set;
    }
        = new ObservableCollection<ExamMaterial>();

    [JsonIgnore]
    private static readonly string TimeFormat = "yyyy-MM-dd HH:mm:ss";


    [JsonIgnore]
    public DateTime StartTime => DateTime.ParseExact(Start, TimeFormat, null);

    [JsonIgnore]
    public DateTime EndTime => DateTime.ParseExact(End, TimeFormat, null);

    [JsonIgnore]
    public DateTime Date => StartTime.Date;


    [JsonIgnore]
    public string DisplayDate => Date.ToString("M月d日");

    [JsonIgnore]
    public string DisplayStartTime => StartTime.ToString("HH:mm");

    [JsonIgnore]
    public string DisplayEndTime => EndTime.ToString("HH:mm");
}

public class ExamMaterial
{
    [JsonProperty("name")]
    public string Name
    {
        get; set;
    }

    [JsonProperty("quantity")]
    public int Quantity
    {
        get; set;
    }

    [JsonProperty("unit")]
    public string Unit
    {
        get; set;
    }
}