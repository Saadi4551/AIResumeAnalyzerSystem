namespace AIResumeAnalyzerSystem.Core.Models;

public class ResumeAnalysis : BaseEntity
{
    public int ResumeId { get; set; }
    public int OverallScore { get; set; }
    public string? Strengths { get; set; }
    public string? Weaknesses { get; set; }
    public string? Suggestions { get; set; }
    public string? SkillsFound { get; set; }
    public string? JobTitleSuggestions { get; set; }
    public string? AIFeedback { get; set; }

    // Navigation Property
    public Resume Resume { get; set; } = null!;
}