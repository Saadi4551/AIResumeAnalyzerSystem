using AIResumeAnalyzerSystem.Core.DTOs;

namespace AIResumeAnalyzerSystem.Core.Interfaces.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
}