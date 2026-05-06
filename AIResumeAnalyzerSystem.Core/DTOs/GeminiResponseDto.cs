using System.Text.Json.Serialization;

public class GeminiResponseDto
{
    [JsonPropertyName("overallScore")]
    public int OverallScore { get; set; }

    [JsonPropertyName("strengths")]
    public string Strengths { get; set; } = string.Empty;

    [JsonPropertyName("weaknesses")]
    public string Weaknesses { get; set; } = string.Empty;

    [JsonPropertyName("suggestions")]
    public string Suggestions { get; set; } = string.Empty;

    [JsonPropertyName("skillsFound")]
    public List<string> SkillsFound { get; set; } = new();

    [JsonPropertyName("jobTitleSuggestions")]
    public List<string> JobTitleSuggestions { get; set; } = new();

    [JsonPropertyName("aiFeedback")]
    public string AIFeedback { get; set; } = string.Empty;
}