using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;

namespace AIResumeAnalyzerSystem.Infrastructure.Services;

public class GeminiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _model;

    public GeminiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["GeminiSettings:ApiKey"]!;
        _model = configuration["GeminiSettings:Model"] ?? "gemini-1.5-flash";
    }

    public async Task<string> AnalyzeResumeTextAsync(string resumeText)
    {
        var prompt = "Analyze the following resume and respond ONLY in this exact JSON format, no extra text, no markdown:\n" +
                     "{\n" +
                     "  \"overallScore\": 85,\n" +
                     "  \"strengths\": \"describe strengths here\",\n" +
                     "  \"weaknesses\": \"describe weaknesses here\",\n" +
                     "  \"suggestions\": \"describe suggestions here\",\n" +
                     "  \"skillsFound\": [\"skill1\", \"skill2\"],\n" +
                     "  \"jobTitleSuggestions\": [\"job1\", \"job2\"],\n" +
                     "  \"aiFeedback\": \"overall feedback here\"\n" +
                     "}\n\n" +
                     "Resume:\n" + resumeText;

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            }
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:generateContent?key={_apiKey}";
        var response = await _httpClient.PostAsync(url, content);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseJson);
        var text = doc.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString();

        return text ?? throw new Exception("Empty response from Gemini.");
    }
}