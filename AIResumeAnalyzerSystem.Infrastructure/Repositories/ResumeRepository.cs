using AIResumeAnalyzerSystem.Core.Interfaces.Repositories;
using AIResumeAnalyzerSystem.Core.Models;
using AIResumeAnalyzerSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AIResumeAnalyzerSystem.Infrastructure.Repositories;

public class ResumeRepository : IResumeRepository
{
    private readonly AppDbContext _context;

    public ResumeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Resume?> GetByIdAsync(int id)
        => await _context.Resumes
            .Include(r => r.Analysis)
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

    public async Task<IEnumerable<Resume>> GetByUserIdAsync(int userId)
        => await _context.Resumes
            .Include(r => r.Analysis)
            .Where(r => r.UserId == userId && !r.IsDeleted)
            .ToListAsync();

    public async Task<Resume> CreateAsync(Resume resume)
    {
        _context.Resumes.Add(resume);
        await _context.SaveChangesAsync();
        return resume;
    }

    public async Task<Resume> UpdateAsync(Resume resume)
    {
        resume.UpdatedAt = DateTime.UtcNow;
        _context.Resumes.Update(resume);
        await _context.SaveChangesAsync();
        return resume;
    }

    public async Task DeleteAsync(int id)
    {
        var resume = await GetByIdAsync(id);
        if (resume != null)
        {
            resume.IsDeleted = true;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<ResumeAnalysis> SaveAnalysisAsync(ResumeAnalysis analysis)
    {
        var existingAnalysis = await _context.ResumeAnalyses
            .FirstOrDefaultAsync(x => x.ResumeId == analysis.ResumeId);

        if (existingAnalysis != null)
        {
            existingAnalysis.OverallScore = analysis.OverallScore;
            existingAnalysis.Strengths = analysis.Strengths;
            existingAnalysis.Weaknesses = analysis.Weaknesses;
            existingAnalysis.Suggestions = analysis.Suggestions;
            existingAnalysis.SkillsFound = analysis.SkillsFound;
            existingAnalysis.JobTitleSuggestions = analysis.JobTitleSuggestions;
            existingAnalysis.AIFeedback = analysis.AIFeedback;

            await _context.SaveChangesAsync();

            return existingAnalysis;
        }

        _context.ResumeAnalyses.Add(analysis);
        await _context.SaveChangesAsync();

        return analysis;
    }
}