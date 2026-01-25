using System;

namespace TimeWinUI.Core.Contracts.Services;

/// <summary>
/// 全局时钟服务：每秒触发一次 Tick，提供统一的 Now。
/// 放在 Core 以避免 UI 依赖。
/// </summary>
public interface IClockService
{
    /// <summary>每秒触发一次，携带当前时间（本地）。</summary>
    event EventHandler<DateTime> Tick;

    /// <summary>当前时间（本地）。</summary>
    DateTime Now { get; }

    /// <summary>启动时钟（App 启动后调用一次）。</summary>
    void Start();

    /// <summary>停止时钟。</summary>
    void Stop();
}
