namespace AIResumeAnalyzerSystem.Core.Models;

public class User : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "User";

    // Navigation Property
    
    public int AnalysisCount { get; set; } = 0;
    public ICollection<Resume> Resumes { get; set; } = new List<Resume>();
}