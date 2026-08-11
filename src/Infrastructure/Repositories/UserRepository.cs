using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using TaskManagerPro.TaskManagerPro.Interfaces;
using TaskManagerPro.TaskMasterPro.Application.DTOs.Auth;
using TaskManagerPro.TaskMasterPro.Domain;
using TaskManagerPro.TaskMasterPro.Domain.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace TaskManagerPro.TaskMasterPro.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{

    private readonly AppDbContext _context;
    private IPasswordHasher _passwordHasher;

    public UserRepository(AppDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    // Buscamos usando una expresión Lambda porque el Email no es la Llave Primaria
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.User
            .FirstOrDefaultAsync(u => u.Email == email);
    }


    public async Task AddAsync(User user)
    {
        await _context.User.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.User.FindAsync(id);
    }

    // 1. El tipo de retorno debe ser Task<User> porque vas a devolver al usuario si todo sale bien
    

   

    
}