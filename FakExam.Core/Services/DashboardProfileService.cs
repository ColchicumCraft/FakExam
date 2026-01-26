
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FakExam.Core.Contracts.Services;
using FakExam.Core.Models;

namespace FakExam.Core.Services;

/// <summary>
/// 统一管理考试配置与当前状态（进行中/下一场）。
/// 不依赖 UI；仅在 RefreshNow 或 Load 后计算一次，按需由外部调度。
/// </summary>
public sealed class DashboardProfileService : IDashboardProfileService
{
    private readonly IClockService _clock;

    public DashboardProfile? CurrentProfile { get; private set; }
    public string? CurrentFilePath { get; private set; }

    public ExamInfo? CurrentInProgressExam { get; private set; }
    public ExamInfo? NextUpcomingExam { get; private set; }

    public event EventHandler? ProfileChanged;
    public event EventHandler? ExamStateChanged;

    private ExamInfo? _lastInProgress;
    private ExamInfo? _lastUpcoming;

    public DashboardProfileService(IClockService clock)
    {
        _clock = clock;
        // 可选：订阅时钟，仅在秒级对齐时判断是否在边界上需要更新。
        _clock.Tick += (_, __) => RefreshNow();
    }

    public async Task LoadFromFileAsync(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            throw new FileNotFoundException("配置文件不存在", filePath);

        var json = await Task.Run(() => File.ReadAllText(filePath));
        var profile = JsonConvert.DeserializeObject<DashboardProfile>(json);
        CurrentProfile = profile ?? new DashboardProfile();
        CurrentFilePath = filePath;

        RefreshNow();
        ProfileChanged?.Invoke(this, EventArgs.Empty);
    }

    public async Task<bool> SaveAsync(string? filePath = null)
    {
        var path = filePath ?? CurrentFilePath;
        if (string.IsNullOrWhiteSpace(path) || CurrentProfile == null) return false;
        var json = JsonConvert.SerializeObject(CurrentProfile, Formatting.Indented);
        await Task.Run(() => File.WriteAllText(path!, json));
        ProfileChanged?.Invoke(this, EventArgs.Empty);
        return true;
    }

    public void Reset()
    {
        CurrentProfile = new DashboardProfile();
        CurrentFilePath = null;
        CurrentInProgressExam = null;
        NextUpcomingExam = null;
        _lastInProgress = null;
        _lastUpcoming = null;
        ProfileChanged?.Invoke(this, EventArgs.Empty);
        ExamStateChanged?.Invoke(this, EventArgs.Empty);
    }

    public void RefreshNow()
    {
        if (CurrentProfile == null || CurrentProfile.ExamInfos == null || CurrentProfile.ExamInfos.Count == 0)
        {
            SetPointers(null, null);
            return;
        }

        var now = _clock.Now;

        // 进行中：start <= now < end
        var inProgress = CurrentProfile.ExamInfos
            .Where(e => e.StartTime <= now && now < e.EndTime)
            .OrderBy(e => e.EndTime)
            .FirstOrDefault();

        // 下一场（未开始，最近的一场）
        var upcoming = inProgress == null
            ? CurrentProfile.ExamInfos
                .Where(e => now < e.StartTime)
                .OrderBy(e => e.StartTime)
                .FirstOrDefault()
            : null;

        SetPointers(inProgress, upcoming);
    }

    private void SetPointers(ExamInfo? inProgress, ExamInfo? upcoming)
    {
        bool changed = !ReferenceEquals(_lastInProgress, inProgress) || !ReferenceEquals(_lastUpcoming, upcoming);

        CurrentInProgressExam = inProgress;
        NextUpcomingExam = upcoming;

        if (changed)
        {
            _lastInProgress = inProgress;
            _lastUpcoming = upcoming;
            ExamStateChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
