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

    // ✅ Register new user
    // Returns user info only - token is set in HttpOnly cookie
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var (result, token) = await _authService.RegisterAsync(dto);

        // ✅ Token goes into HttpOnly cookie - NOT in response body
        SetTokenCookie(token);

        return Ok(ApiResponse<AuthResponseDto>.Ok(result, "Registered successfully."));
    }

    // ✅ Login existing user
    // Returns user info only - token is set in HttpOnly cookie
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var (result, token) = await _authService.LoginAsync(dto);

        // ✅ Token goes into HttpOnly cookie - NOT in response body
        SetTokenCookie(token);

        return Ok(ApiResponse<AuthResponseDto>.Ok(result, "Login successful."));
    }

    // ✅ Logout - clears the JWT cookie
    // Frontend should call this on logout button click
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        // ✅ Delete the jwt cookie from browser
        Response.Cookies.Delete("jwt");
        return Ok(ApiResponse<string>.Ok("Logged out", "Logout successful."));
    }

    // ✅ Sets JWT as HttpOnly cookie
    // HttpOnly = JavaScript cannot read it (XSS protection)
    // Secure = only sent over HTTPS (production)
    // SameSite = None in production (cross-origin), Lax in development
    private void SetTokenCookie(string token)
    {
        var isProduction = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production";

        var cookieOptions = new CookieOptions
        {
            // ✅ JavaScript cannot access this cookie - XSS protection
            HttpOnly = true,

            // ✅ Only send over HTTPS in production
            Secure = isProduction,

            // ✅ None = allow cross-origin (needed for frontend on different domain)
            // Lax = local development (same origin)
            SameSite = isProduction ? SameSiteMode.None : SameSiteMode.Lax,

            // ✅ Cookie expires in 7 days
            Expires = DateTime.UtcNow.AddDays(7)
        };

        Response.Cookies.Append("jwt", token, cookieOptions);
    }
}