using Microsoft.EntityFrameworkCore;
using TaskManagerPro.TaskMasterPro.Domain;
using TaskManagerPro.TaskMasterPro.Domain.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace TaskManagerPro.TaskMasterPro.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _context;

    // ReSharper disable once ConvertToPrimaryConstructor
    public RefreshTokenRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task SaveAsync(User user, RefreshToken refreshToken)
    {
        await _context.Set<RefreshToken>().AddAsync(refreshToken);
        await _context.SaveChangesAsync();
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _context.Set<RefreshToken>().Include(rt => rt.User).FirstOrDefaultAsync(rt => rt.Token == token);
    }

    public async Task<User?> DeleteTokensAsync(User user)
    {
        await _context.RefreshToken
            .Where(t => t.UserId == user.Id)
            .ExecuteDeleteAsync(); // Borra directamente en la DB sin cargar nada

        return user;
    }

    public async Task UpdateAsync(RefreshToken token)
    {
        _context.RefreshToken.Update(token);
         await _context.SaveChangesAsync();
    }
}