namespace AIResumeAnalyzerSystem.Core.DTOs;

public class ResumeResponseDto
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public ResumeAnalysisDto? Analysis { get; set; }
}

public class ResumeAnalysisDto
{
    public int OverallScore { get; set; }
    public string? Strengths { get; set; }
    public string? Weaknesses { get; set; }
    public string? Suggestions { get; set; }
    public List<string> SkillsFound { get; set; } = new();
    public List<string> JobTitleSuggestions { get; set; } = new();
    public string? AIFeedback { get; set; }
}