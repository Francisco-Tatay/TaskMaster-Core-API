using System.Threading.Tasks;
using TaskManagerPro.TaskMasterPro.Domain;

namespace TaskManagerPro.TaskManagerPro.Interfaces;

public interface IUserRepository
{
    Task<UserEntity?> GetByEmailAsync(string email);
    Task AddAsync(UserEntity user);
}