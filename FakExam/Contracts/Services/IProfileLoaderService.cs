using System.Threading.Tasks;

namespace FakExam.Contracts.Services;

/// <summary>
/// 用于从外部选择/加载配置文件（界面相关实现放到 UI 工程）。
/// </summary>
public interface IProfileLoaderService
{
    Task<bool> PickAndLoadAsync();
}
