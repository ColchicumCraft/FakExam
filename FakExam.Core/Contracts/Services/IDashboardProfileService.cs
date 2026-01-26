
using System;
using System.Threading.Tasks;
using FakExam.Core.Models;

namespace FakExam.Core.Contracts.Services;

public interface IDashboardProfileService
{
    DashboardProfile? CurrentProfile
    {
        get;
    }
    string? CurrentFilePath
    {
        get;
    }

    /// <summary>当前“进行中”的考试；若无则为 null。</summary>
    ExamInfo? CurrentInProgressExam
    {
        get;
    }

    /// <summary>最近的未开始考试（当没有进行中时），可能为 null。</summary>
    ExamInfo? NextUpcomingExam
    {
        get;
    }

    /// <summary>配置被载入/保存/重置后触发。</summary>
    event EventHandler? ProfileChanged;

    /// <summary>进行中/下一场快照发生变化时触发（指针变化才触发，避免事件风暴）。</summary>
    event EventHandler? ExamStateChanged;

    Task LoadFromFileAsync(string filePath);
    Task<bool> SaveAsync(string? filePath = null);

    void Reset();

    /// <summary>手动触发一次计算。</summary>
    void RefreshNow();
}