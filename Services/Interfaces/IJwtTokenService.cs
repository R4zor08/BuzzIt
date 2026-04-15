using BuzzIt.Models;

namespace BuzzIt.Services.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(ApplicationUser user);
}
