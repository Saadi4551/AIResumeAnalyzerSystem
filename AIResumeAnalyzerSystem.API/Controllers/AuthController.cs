using AIResumeAnalyzerSystem.Core.Common;
using AIResumeAnalyzerSystem.Core.DTOs;
using AIResumeAnalyzerSystem.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

    // ✅ Anyone can register - no auth needed
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var (result, token) = await _authService.RegisterAsync(dto);
        SetTokenCookie(token);
        return Ok(ApiResponse<AuthResponseDto>.Ok(result, "Registered successfully."));
    }

    // ✅ Anyone can login - no auth needed
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var (result, token) = await _authService.LoginAsync(dto);
        SetTokenCookie(token);
        return Ok(ApiResponse<AuthResponseDto>.Ok(result, "Login successful."));
    }

    // ✅ Anyone can logout
    [AllowAnonymous]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        var isProduction = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production";

        // ✅ Properly delete cookie cross-origin by setting past expiry
        Response.Cookies.Append("jwt", "", new CookieOptions
        {
            HttpOnly = true,
            Secure = isProduction,
            SameSite = isProduction ? SameSiteMode.None : SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddDays(-1)
        });

        return Ok(ApiResponse<string>.Ok("Logged out", "Logout successful."));
    }
    // ✅ Only logged-in users - requires valid JWT cookie
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _authService.GetCurrentUserAsync(userId);
        return Ok(ApiResponse<AuthResponseDto>.Ok(result, "User fetched successfully."));
    }

    private void SetTokenCookie(string token)
    {
        var isProduction = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production";

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = isProduction,
            SameSite = isProduction ? SameSiteMode.None : SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddDays(7)
        };

        Response.Cookies.Append("jwt", token, cookieOptions);
    }
}