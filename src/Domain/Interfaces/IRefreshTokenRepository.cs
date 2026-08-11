namespace TaskManagerPro.TaskMasterPro.Domain.Interfaces;

public interface IRefreshTokenRepository
{
    System.Threading.Tasks.Task SaveAsync(User user, RefreshToken refreshToken);
    Task<RefreshToken?> GetByTokenAsync(string token);

    Task<User?> DeleteTokensAsync(User user);
    System.Threading.Tasks.Task UpdateAsync(RefreshToken token);
}