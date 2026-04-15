using BuzzIt.Models;
using BuzzIt.Requests;
using BuzzIt.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BuzzIt.Controllers.Api;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly JwtOptions _jwtOptions;

    public AuthController(
        IAuthService authService,
        IJwtTokenService jwtTokenService,
        IOptions<JwtOptions> jwtOptions)
    {
        _authService = authService;
        _jwtTokenService = jwtTokenService;
        _jwtOptions = jwtOptions.Value;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var result = await _authService.RegisterAsync(request.Username, request.Password);
        if (!result.Success || result.User == null)
        {
            return BadRequest(new { error = result.Error ?? "Registration failed." });
        }

        var token = _jwtTokenService.GenerateToken(result.User);

        return Ok(new LoginResponse
        {
            AccessToken = token,
            ExpiresInMinutes = _jwtOptions.ExpiresInMinutes,
            Username = result.User.Username,
            Role = result.User.Role
        });
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var user = await _authService.ValidateCredentialsAsync(request.Username, request.Password);
        if (user == null)
        {
            return Unauthorized(new { error = "Invalid username or password." });
        }

        var token = _jwtTokenService.GenerateToken(user);

        return Ok(new LoginResponse
        {
            AccessToken = token,
            ExpiresInMinutes = _jwtOptions.ExpiresInMinutes,
            Username = user.Username,
            Role = user.Role
        });
    }
}
