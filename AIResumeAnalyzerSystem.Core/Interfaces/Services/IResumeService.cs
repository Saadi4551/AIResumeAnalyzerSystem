using AIResumeAnalyzerSystem.Core.DTOs;

namespace AIResumeAnalyzerSystem.Core.Interfaces.Services;

public interface IResumeService
{
    Task<ResumeResponseDto> UploadResumeAsync(int userId, string fileName, string filePath);
    Task<ResumeResponseDto> GetResumeByIdAsync(int id);
    Task<IEnumerable<ResumeResponseDto>> GetResumesByUserIdAsync(int userId);
    Task<ResumeResponseDto> AnalyzeResumeAsync(int resumeId, int  userId);
    Task DeleteResumeAsync(int id);
}