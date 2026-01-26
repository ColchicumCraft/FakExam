namespace FakExam.Contracts.Services;

/// <summary>
/// 负责根据考试进度编排页面切换（TimeShow ↔ Dashboard）。
/// 仅提供初始化入口，具体实现放在 UI 层。
/// </summary>
public interface IExamNavigationOrchestrator
{
    void Initialize();
}
