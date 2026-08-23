using TaskManagerPro.TaskMasterPro.Application.DTOs.Auth;
using TaskManagerPro.TaskMasterPro.Domain;

namespace TaskManagerPro.Application.Common.Interfaces;

public interface ITokenService
{
    Task<AuthResponseDto> GenerateTokensAsync(User user);
    
}