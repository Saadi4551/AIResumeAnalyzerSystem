using AIResumeAnalyzerSystem.Core.Interfaces.Repositories;
using AIResumeAnalyzerSystem.Core.Models;
using AIResumeAnalyzerSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AIResumeAnalyzerSystem.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(int id)
        => await _context.Users.FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);

    public async Task<User?> GetByEmailAsync(string email)
        => await _context.Users.FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);

    public async Task<IEnumerable<User>> GetAllAsync()
        => await _context.Users.Where(u => !u.IsDeleted).ToListAsync();

    public async Task<User> CreateAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User> UpdateAsync(User user)
    {
        user.UpdatedAt = DateTime.UtcNow;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task DeleteAsync(int id)
    {
        var user = await GetByIdAsync(id);
        if (user != null)
        {
            user.IsDeleted = true;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(string email)
        => await _context.Users.AnyAsync(u => u.Email == email && !u.IsDeleted);
}