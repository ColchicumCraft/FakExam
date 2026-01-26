using System;
using System.Threading;
using System.Threading.Tasks;
using FakExam.Core.Contracts.Services;

namespace FakExam.Core.Services;

/// <summary>
/// 使用 PeriodicTimer 的轻量时钟服务（.NET 6+）。
/// </summary>
public sealed class ClockService : IClockService, IDisposable
{
    private readonly CancellationTokenSource _cts = new();
    private Task? _loop;
    public event EventHandler<DateTime>? Tick;

    public DateTime Now => DateTime.Now;

    public void Start()
    {
        if (_loop != null) return;
        _loop = Task.Run(async () =>
        {
            var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
            try
            {
                while (await timer.WaitForNextTickAsync(_cts.Token))
                {
                    Tick?.Invoke(this, DateTime.Now);
                }
            }
            catch (OperationCanceledException)
            {
                // ignored
            }
        }, _cts.Token);
    }

    public void Stop() => _cts.Cancel();

    public void Dispose() => _cts.Cancel();
}
