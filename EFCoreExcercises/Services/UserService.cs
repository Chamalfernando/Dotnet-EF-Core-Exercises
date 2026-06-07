using EFCoreExcercises.Data;
using EFCoreExcercises.DTOs;
using EFCoreExcercises.Models;
using Microsoft.EntityFrameworkCore;

namespace EFCoreExcercises.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _db;

    public UserService(AppDbContext db) => _db = db;

    public async Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken ct = default)
    {
        // LINQ query against EF Core: project entities straight into DTOs in SQL.
        return await _db.Users
            .AsNoTracking()
            .OrderBy(u => u.Id)
            .Select(u => new UserDto(
                u.Id, u.FirstName, u.LastName, u.Email, u.CreatedAt, u.UpdatedAt))
            .ToListAsync(ct);
    }

    public async Task<UserDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _db.Users
            .AsNoTracking()
            .Where(u => u.Id == id)
            .Select(u => new UserDto(
                u.Id, u.FirstName, u.LastName, u.Email, u.CreatedAt, u.UpdatedAt))
            .FirstOrDefaultAsync(ct);
    }

    public async Task<UserDto> CreateAsync(CreateUserDto dto, CancellationToken ct = default)
    {
        var user = new User
        {
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            Email = dto.Email.Trim().ToLowerInvariant(),
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);

        return new UserDto(
            user.Id, user.FirstName, user.LastName, user.Email, user.CreatedAt, user.UpdatedAt);
    }

    public async Task<UserDto?> UpdateAsync(int id, UpdateUserDto dto, CancellationToken ct = default)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
        if (user is null)
            return null;

        user.FirstName = dto.FirstName.Trim();
        user.LastName = dto.LastName.Trim();
        user.Email = dto.Email.Trim().ToLowerInvariant();
        user.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);

        return new UserDto(
            user.Id, user.FirstName, user.LastName, user.Email, user.CreatedAt, user.UpdatedAt);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
        if (user is null)
            return false;

        _db.Users.Remove(user);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> EmailExistsAsync(string email, int? excludeId = null, CancellationToken ct = default)
    {
        var normalized = email.Trim().ToLowerInvariant();
        return await _db.Users
            .AnyAsync(u => u.Email == normalized && (excludeId == null || u.Id != excludeId), ct);
    }
}
