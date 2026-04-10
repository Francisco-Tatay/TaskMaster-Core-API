using TaskManagerPro.TaskMasterPro.Application.DTOs.Auth;
using TaskManagerPro.TaskMasterPro.Domain;
using Task = System.Threading.Tasks.Task;

namespace TaskManagerPro.TaskMasterPro.Application.Common.Interfaces;

public interface ITokenService
{
    Task<AuthResponseDto> GenerateTokensAsync(User user);
    
}