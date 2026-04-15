using BuzzIt.Data;
using BuzzIt.Models;
using BuzzIt.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BuzzIt.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher<ApplicationUser> _passwordHasher;

    public AuthService(
        ApplicationDbContext context,
        IPasswordHasher<ApplicationUser> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
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

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        return result == PasswordVerificationResult.Failed ? null : user;
    }

    public async Task<(bool Success, string? Error, ApplicationUser? User)> RegisterAsync(string username, string password)
    {
        var normalizedUsername = username.Trim();
        if (string.IsNullOrWhiteSpace(normalizedUsername))
        {
            return (false, "Username is required.", null);
        }

        var exists = await _context.Users.AnyAsync(existing =>
            existing.Username.ToLower() == normalizedUsername.ToLower());

        if (exists)
        {
            return (false, "That username is already taken.", null);
        }

        var user = new ApplicationUser
        {
            Username = normalizedUsername,
            Role = "User"
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return (true, null, user);
    }
}
