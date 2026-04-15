using BuzzIt.Models;

namespace BuzzIt.Services.Interfaces;

public interface IAuthService
{
    Task<ApplicationUser?> ValidateCredentialsAsync(string username, string password);
    Task<(bool Success, string? Error, ApplicationUser? User)> RegisterAsync(string username, string email, string password);
}
