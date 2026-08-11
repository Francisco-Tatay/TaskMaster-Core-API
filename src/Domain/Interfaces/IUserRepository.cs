namespace TaskManagerPro.TaskMasterPro.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    System.Threading.Tasks.Task AddAsync(User user);
    Task<User?> GetByIdAsync(Guid id);
}