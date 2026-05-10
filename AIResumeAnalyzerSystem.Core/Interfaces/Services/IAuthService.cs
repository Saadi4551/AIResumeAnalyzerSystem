using AIResumeAnalyzerSystem.Core.DTOs;

namespace AIResumeAnalyzerSystem.Core.Interfaces.Services;

public interface IAuthService
{
    Task<(AuthResponseDto user, string token)> RegisterAsync(RegisterDto dto);
    Task<(AuthResponseDto user, string token)> LoginAsync(LoginDto dto);
}