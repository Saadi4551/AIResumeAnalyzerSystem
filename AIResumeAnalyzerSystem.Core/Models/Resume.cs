namespace AIResumeAnalyzerSystem.Core.Models;

public class Resume : BaseEntity
{
    public int UserId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string? ExtractedText { get; set; }
    public string Status { get; set; } = "Pending";

    // Navigation Properties
    public User User { get; set; } = null!;
    public ResumeAnalysis? Analysis { get; set; }
}