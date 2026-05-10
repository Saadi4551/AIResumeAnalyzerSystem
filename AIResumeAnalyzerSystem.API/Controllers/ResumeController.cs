using AIResumeAnalyzerSystem.Core.Common;
using AIResumeAnalyzerSystem.Core.DTOs;
using AIResumeAnalyzerSystem.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AIResumeAnalyzerSystem.API.Controllers;

// ✅ [Authorize] on entire controller
// ALL endpoints require valid JWT cookie
// No login = 401 Unauthorized response
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ResumeController : ControllerBase
{
    private readonly IResumeService _resumeService;
    private readonly IWebHostEnvironment _environment;

    public ResumeController(IResumeService resumeService, IWebHostEnvironment environment)
    {
        _resumeService = resumeService;
        _environment = environment;
    }

    // ✅ Upload resume - requires login
    // Saves file to wwwroot/uploads folder
    // Links file to logged-in user via JWT cookie
    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(ApiResponse<string>.Fail("No file uploaded."));

        // ✅ Get userId from JWT cookie claims
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        // ✅ Save file to wwwroot/uploads
        var uploadsFolder = Path.Combine(_environment.WebRootPath ?? "wwwroot", "uploads");
        Directory.CreateDirectory(uploadsFolder);
        var filePath = Path.Combine(uploadsFolder, $"{Guid.NewGuid()}_{file.FileName}");

        using (var stream = new FileStream(filePath, FileMode.Create))
            await file.CopyToAsync(stream);

        var result = await _resumeService.UploadResumeAsync(userId, file.FileName, filePath);
        return Ok(ApiResponse<ResumeResponseDto>.Ok(result, "Resume uploaded successfully."));
    }

    // ✅ Get resume by ID - requires login
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _resumeService.GetResumeByIdAsync(id);
        return Ok(ApiResponse<ResumeResponseDto>.Ok(result));
    }

    // ✅ Get all resumes of logged-in user - requires login
    [HttpGet("my-resumes")]
    public async Task<IActionResult> GetMyResumes()
    {
        // ✅ Get userId from JWT cookie claims
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _resumeService.GetResumesByUserIdAsync(userId);
        return Ok(ApiResponse<IEnumerable<ResumeResponseDto>>.Ok(result));
    }

    // ✅ Analyze resume with Gemini AI - requires login
    // Max 3 analyses per user
    // Returns remainingAnalyses count for frontend to display
    [HttpPost("analyze/{id}")]
    public async Task<IActionResult> Analyze(int id)
    {
        // ✅ Get userId from JWT cookie claims
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        // ✅ Check remaining analyses before calling Gemini
        var remainingAnalyses = await _resumeService.GetRemainingAnalysesAsync(userId);

        if (remainingAnalyses <= 0)
        {
            // ✅ 429 = Too Many Requests
            return StatusCode(429, new
            {
                Success = false,
                Message = "Analysis limit reached! You have used all 3 free analyses.",
                RemainingAnalyses = 0
            });
        }

        var result = await _resumeService.AnalyzeResumeAsync(id, userId);

        return Ok(new
        {
            Success = true,
            StatusCode = 200,
            Message = "Resume analyzed successfully.",
            // ✅ Frontend should show this counter to user
            RemainingAnalyses = remainingAnalyses - 1,
            Data = result
        });
    }

    // ✅ Delete resume - requires login
    // If resume was analyzed, decrements user's analysis count
    // So user gets their slot back
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _resumeService.DeleteResumeAsync(id);
        return Ok(ApiResponse<string>.Ok("Deleted", "Resume deleted successfully."));
    }
}