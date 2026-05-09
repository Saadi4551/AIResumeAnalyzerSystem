using AIResumeAnalyzerSystem.Core.Common;
using AIResumeAnalyzerSystem.Core.DTOs;
using AIResumeAnalyzerSystem.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AIResumeAnalyzerSystem.API.Controllers;

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

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(ApiResponse<string>.Fail("No file uploaded."));

        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        // Save file
        var uploadsFolder = Path.Combine(_environment.WebRootPath ?? "wwwroot", "uploads");
        Directory.CreateDirectory(uploadsFolder);
        var filePath = Path.Combine(uploadsFolder, $"{Guid.NewGuid()}_{file.FileName}");

        using (var stream = new FileStream(filePath, FileMode.Create))
            await file.CopyToAsync(stream);

        var result = await _resumeService.UploadResumeAsync(userId, file.FileName, filePath);
        return Ok(ApiResponse<ResumeResponseDto>.Ok(result, "Resume uploaded successfully."));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _resumeService.GetResumeByIdAsync(id);
        return Ok(ApiResponse<ResumeResponseDto>.Ok(result));
    }

    [HttpGet("my-resumes")]
    public async Task<IActionResult> GetMyResumes()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _resumeService.GetResumesByUserIdAsync(userId);
        return Ok(ApiResponse<IEnumerable<ResumeResponseDto>>.Ok(result));
    }
    [HttpPost("analyze/{id}")]
    public async Task<IActionResult> Analyze(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _resumeService.AnalyzeResumeAsync(id, userId);
        return Ok(ApiResponse<ResumeResponseDto>.Ok(result, "Resume analyzed successfully."));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _resumeService.DeleteResumeAsync(id);
        return Ok(ApiResponse<string>.Ok("Deleted", "Resume deleted successfully."));
    }
}