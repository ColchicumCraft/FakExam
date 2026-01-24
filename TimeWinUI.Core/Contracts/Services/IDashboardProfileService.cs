
using System;
using System.Threading.Tasks;
using TimeWinUI.Core.Models;

namespace TimeWinUI.Core.Contracts.Services
{
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

        Task LoadFromFileAsync(string filePath);
        Task<bool> SaveAsync(string? filePath = null);

        void Reset();

        /// <summary>手动触发一次计算（服务内部也会每秒计算一次）。</summary>
        void RefreshNow();
    }
}