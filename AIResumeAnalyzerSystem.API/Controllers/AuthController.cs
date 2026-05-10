using AIResumeAnalyzerSystem.Core.Common;
using AIResumeAnalyzerSystem.Core.DTOs;
using AIResumeAnalyzerSystem.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace AIResumeAnalyzerSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var (result, token) = await _authService.RegisterAsync(dto);
        SetTokenCookie(token);
        return Ok(ApiResponse<AuthResponseDto>.Ok(result, "Registered successfully."));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var (result, token) = await _authService.LoginAsync(dto);
        SetTokenCookie(token);
        return Ok(ApiResponse<AuthResponseDto>.Ok(result, "Login successful."));
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("jwt");
        return Ok(ApiResponse<string>.Ok("Logged out", "Logout successful."));
    }

    private void SetTokenCookie(string token)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddDays(7)
        };
        Response.Cookies.Append("jwt", token, cookieOptions);
    }
}