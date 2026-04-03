using Microsoft.EntityFrameworkCore;
using TaskManagerPro.TaskManagerPro.Interfaces;
using TaskManagerPro.TaskMasterPro.Domain;

namespace TaskManagerPro.TaskMasterPro.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    // Buscamos usando una expresión Lambda porque el Email no es la Llave Primaria
    public async Task<UserEntity?> GetByEmailAsync(string email) 
    {
        return await _context.User
            .FirstOrDefaultAsync(u => u.Email == email);
    }
    

    public async Task AddAsync(UserEntity user)
    {
        await _context.User.AddAsync(user);
        await _context.SaveChangesAsync();
    }
}