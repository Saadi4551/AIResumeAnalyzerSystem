using AIResumeAnalyzerSystem.Core.Models;

namespace AIResumeAnalyzerSystem.Core.Interfaces.Repositories;

public interface IResumeRepository
{
    Task<Resume?> GetByIdAsync(int id);
    Task<IEnumerable<Resume>> GetByUserIdAsync(int userId);
    Task<Resume> CreateAsync(Resume resume);
    Task<Resume> UpdateAsync(Resume resume);
    Task DeleteAsync(int id);
    Task<ResumeAnalysis> SaveAnalysisAsync(ResumeAnalysis analysis);
}