using BuzzIt.Data;
using BuzzIt.Models;
using BuzzIt.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuzzIt.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;

    public AuthService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApplicationUser?> ValidateCredentialsAsync(string username, string password)
    {
        var normalizedUsername = username.Trim().ToLowerInvariant();
        var user = await _context.Users.FirstOrDefaultAsync(existing =>
            existing.Username.ToLower() == normalizedUsername);

        if (user == null)
        {
            return null;
        }

        return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash) ? user : null;
    }

    public async Task<(bool Success, string? Error, ApplicationUser? User)> RegisterAsync(string username, string email, string password)
    {
        var normalizedUsername = username.Trim();
        if (string.IsNullOrWhiteSpace(normalizedUsername))
        {
            return (false, "Username is required.", null);
        }

        var normalizedEmail = email.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(normalizedEmail))
        {
            return (false, "Email is required.", null);
        }

        var exists = await _context.Users.AnyAsync(existing =>
            existing.Username.ToLower() == normalizedUsername.ToLower());

        if (exists)
        {
            return (false, "That username is already taken.", null);
        }

        var emailExists = await _context.Users.AnyAsync(existing =>
            existing.Email.ToLower() == normalizedEmail);

        if (emailExists)
        {
            return (false, "That email is already registered.", null);
        }

        var user = new ApplicationUser
        {
            Username = normalizedUsername,
            Email = normalizedEmail,
            Role = "User"
        };

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return (true, null, user);
    }
}
