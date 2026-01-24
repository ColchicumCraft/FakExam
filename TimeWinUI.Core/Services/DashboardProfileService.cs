
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TimeWinUI.Core.Contracts.Services;
using TimeWinUI.Core.Models;

namespace TimeWinUI.Core.Services
{
    public class DashboardProfileService : IDashboardProfileService, IDisposable
    {
        private readonly object _gate = new();
        private DashboardProfile? _profile;
        private string? _filePath;
        private ExamInfo? _inProgress;
        private ExamInfo? _upcoming;
        private readonly Timer _timer;

        public DashboardProfile? CurrentProfile => _profile;
        public string? CurrentFilePath => _filePath;
        public ExamInfo? CurrentInProgressExam => _inProgress;
        public ExamInfo? NextUpcomingExam => _upcoming;

        public DashboardProfileService()
        {
            _timer = new Timer(_ => RefreshNow(), null, 0, 1000);
        }

        public async Task LoadFromFileAsync(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                throw new FileNotFoundException("配置文件不存在", filePath);

            var json = await File.ReadAllTextAsync(filePath).ConfigureAwait(false);
            var profile = JsonConvert.DeserializeObject<DashboardProfile>(json) ?? new DashboardProfile();

            lock (_gate)
            {
                _profile = profile;
                _filePath = filePath;
            }

            RefreshNow();
        }

        public async Task<bool> SaveAsync(string? filePath = null)
        {
            DashboardProfile? snapshot;
            string? target;
            lock (_gate)
            {
                snapshot = _profile;
                target = filePath ?? _filePath;
            }
            if (snapshot == null || string.IsNullOrWhiteSpace(target)) return false;
            var json = JsonConvert.SerializeObject(snapshot, Formatting.Indented);
            await File.WriteAllTextAsync(target, json).ConfigureAwait(false);
            return true;
        }

        public void Reset()
        {
            lock (_gate)
            {
                _profile = null;
                _filePath = null;
                _inProgress = null;
                _upcoming = null;
            }
        }

        public void RefreshNow()
        {
            DashboardProfile? p;
            lock (_gate) p = _profile;

            if (p == null || p.ExamInfos == null || p.ExamInfos.Count == 0)
            {
                lock (_gate)
                {
                    _inProgress = null;
                    _upcoming = null;
                }
                return;
            }

            var now = DateTime.Now;
            var inProg = p.ExamInfos.FirstOrDefault(e => now >= e.StartTime && now < e.EndTime);
            var next = inProg == null
                ? p.ExamInfos.Where(e => e.StartTime > now).OrderBy(e => e.StartTime).FirstOrDefault()
                : null;

            lock (_gate)
            {
                _inProgress = inProg;
                _upcoming = next;
            }
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
