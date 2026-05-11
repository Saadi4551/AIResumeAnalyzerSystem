namespace AIResumeAnalyzerSystem.Core.DTOs;
using System.ComponentModel.DataAnnotations;

public class RegisterDto
{
    [Required]
    public string FullName { get; set; }

    [Required]
    [EmailAddress(ErrorMessage = "Valid email address required")]
    public string Email { get; set; }

    [Required]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    public string Password { get; set; }
}

public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class AuthResponseDto
{
    
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}