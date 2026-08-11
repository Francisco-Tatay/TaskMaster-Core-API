using Microsoft.AspNetCore.Identity;
using TaskManagerPro.TaskManagerPro.Interfaces;

namespace TaskManagerPro.TaskMasterPro.Infrastructure.Services;

// Le cambiamos el nombre a Hasher (el que hace la acción)
public class BCryptHasher : IPasswordHasher
{
    // Quitamos el Async porque BCrypt.Net es sincrónico por naturaleza
    public string Hash(string password)
    {
        // El factor 10 y el true (Enhanced Entropy) están perfectos
        return BCrypt.Net.BCrypt.HashPassword(password, 10, true);
    }

    public bool Verify(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}