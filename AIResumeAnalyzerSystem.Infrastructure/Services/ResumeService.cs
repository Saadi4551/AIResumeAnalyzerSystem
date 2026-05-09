using AIResumeAnalyzerSystem.Core.DTOs;
using AIResumeAnalyzerSystem.Core.Interfaces.Repositories;
using AIResumeAnalyzerSystem.Core.Interfaces.Services;
using AIResumeAnalyzerSystem.Core.Models;
using System.Text.Json;
using UglyToad.PdfPig;
using System.Text;
using AIResumeAnalyzerSystem.Infrastructure.Services;

namespace AIResumeAnalyzerSystem.Infrastructure.Services;

public class ResumeService : IResumeService
{
    private readonly IResumeRepository _resumeRepository;
    private readonly IUserRepository _userRepository;
    private readonly GeminiService _geminiService;

    public ResumeService(IResumeRepository resumeRepository, IUserRepository userRepository, GeminiService geminiService)
    {
        _resumeRepository = resumeRepository;
        _userRepository = userRepository;
        _geminiService = geminiService;
    }

    public async Task<ResumeResponseDto> UploadResumeAsync(int userId, string fileName, string filePath)
    {
        var resume = new Resume
        {
            UserId = userId,
            FileName = fileName,
            FilePath = filePath,
            Status = "Pending"
        };

        var created = await _resumeRepository.CreateAsync(resume);
        return MapToDto(created);
    }

    public async Task<ResumeResponseDto> GetResumeByIdAsync(int id)
    {
        var resume = await _resumeRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Resume not found.");

        return MapToDto(resume);
    }

    public async Task<IEnumerable<ResumeResponseDto>> GetResumesByUserIdAsync(int userId)
    {
        var resumes = await _resumeRepository.GetByUserIdAsync(userId);
        return resumes.Select(MapToDto);
    }

    public async Task<ResumeResponseDto> AnalyzeResumeAsync(int resumeId, int userId)
    {
        // ✅ Step 1: Get User and Check Limit
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found.");

        if (user.AnalysisCount >= 3)
            throw new Exception("Analysis limit reached! You can only analyze 3 resumes.");

        // ✅ Step 2: Get Resume
        var resume = await _resumeRepository.GetByIdAsync(resumeId)
            ?? throw new KeyNotFoundException("Resume not found.");

        if (!File.Exists(resume.FilePath))
            throw new FileNotFoundException("Resume file not found.");

        // ✅ Step 3: Extract text from PDF
        var resumeText = ExtractTextFromPdf(resume.FilePath);

        if (string.IsNullOrWhiteSpace(resumeText))
            throw new Exception("Failed to extract text from resume.");

        // ✅ Step 4: Send to Gemini AI
        var aiResponse = await _geminiService.AnalyzeResumeTextAsync(resumeText);

        // ✅ Step 5: Clean JSON
        aiResponse = CleanJson(aiResponse);

        // ✅ Step 6: Deserialize
        var aiData = JsonSerializer.Deserialize<GeminiResponseDto>(aiResponse)
            ?? throw new Exception("Invalid AI response format.");

        // ✅ Step 7: Save Analysis
        var analysis = new ResumeAnalysis
        {
            ResumeId = resumeId,
            OverallScore = aiData.OverallScore,
            Strengths = aiData.Strengths,
            Weaknesses = aiData.Weaknesses,
            Suggestions = aiData.Suggestions,
            SkillsFound = JsonSerializer.Serialize(aiData.SkillsFound),
            JobTitleSuggestions = JsonSerializer.Serialize(aiData.JobTitleSuggestions),
            AIFeedback = aiData.AIFeedback
        };

        await _resumeRepository.SaveAnalysisAsync(analysis);

        // ✅ Step 8: Update Resume Status
        resume.Status = "Analyzed";
        await _resumeRepository.UpdateAsync(resume);

        // ✅ Step 9: Increment Analysis Count
        user.AnalysisCount++;
        await _userRepository.UpdateAsync(user);

        return MapToDto(resume);
    }

    public async Task DeleteResumeAsync(int id)
    {
        await _resumeRepository.DeleteAsync(id);
    }

    // 🔥 PDF Text Extraction
    private string ExtractTextFromPdf(string filePath)
    {
        using var pdf = PdfDocument.Open(filePath);
        var text = new StringBuilder();

        foreach (var page in pdf.GetPages())
        {
            text.AppendLine(page.Text);
        }

        return text.ToString();
    }

    // 🔥 Clean Gemini Response
    private string CleanJson(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        if (input.StartsWith("```"))
        {
            input = input.Replace("```json", "")
                         .Replace("```", "")
                         .Trim();
        }

        return input;
    }

    private static ResumeResponseDto MapToDto(Resume resume) => new()
    {
        Id = resume.Id,
        FileName = resume.FileName,
        Status = resume.Status,
        CreatedAt = resume.CreatedAt,
        Analysis = resume.Analysis == null ? null : new ResumeAnalysisDto
        {
            OverallScore = resume.Analysis.OverallScore,
            Strengths = resume.Analysis.Strengths,
            Weaknesses = resume.Analysis.Weaknesses,
            Suggestions = resume.Analysis.Suggestions,
            SkillsFound = JsonSerializer.Deserialize<List<string>>(resume.Analysis.SkillsFound ?? "[]") ?? new(),
            JobTitleSuggestions = JsonSerializer.Deserialize<List<string>>(resume.Analysis.JobTitleSuggestions ?? "[]") ?? new(),
            AIFeedback = resume.Analysis.AIFeedback
        }
    };
}