using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Aihrly.Api.Data;
using Aihrly.Api.DTOs;
using Aihrly.Api.Entities;

namespace Aihrly.Api.Controllers;

[ApiController]
[Route("api/applications/{id}/scores")]
public class ScoresController : ControllerBase
{
    private readonly AppDbContext _context;

    public ScoresController(AppDbContext context)
    {
        _context = context;
    }

    // PUT /api/applications/{id}/scores/culture-fit
    [HttpPut("culture-fit")]
    public Task<IActionResult> ScoreCultureFit(Guid id, [FromBody] CreateScoreDto dto)
        => UpsertScore(id, "culture-fit", dto);

    // PUT /api/applications/{id}/scores/interview
    [HttpPut("interview")]
    public Task<IActionResult> ScoreInterview(Guid id, [FromBody] CreateScoreDto dto)
        => UpsertScore(id, "interview", dto);

    // PUT /api/applications/{id}/scores/assessment
    [HttpPut("assessment")]
    public Task<IActionResult> ScoreAssessment(Guid id, [FromBody] CreateScoreDto dto)
        => UpsertScore(id, "assessment", dto);

    private async Task<IActionResult> UpsertScore(Guid applicationId, string dimension, CreateScoreDto dto)
    {
        var memberId = Request.Headers["X-Team-Member-Id"].FirstOrDefault();
        if (string.IsNullOrEmpty(memberId) || !Guid.TryParse(memberId, out var teamMemberId))
            return BadRequest("Missing or invalid X-Team-Member-Id header");

        var member = await _context.TeamMembers.FindAsync(teamMemberId);
        if (member is null) return BadRequest("Team member not found");

        var app = await _context.Applications.FindAsync(applicationId);
        if (app is null) return NotFound("Application not found");

        if (dto.Score < 1 || dto.Score > 5)
            return BadRequest("Score must be between 1 and 5");

        // Upsert — overwrite if exists for same dimension
        var existing = await _context.Scores
            .FirstOrDefaultAsync(s => s.ApplicationId == applicationId && s.Dimension == dimension);

        if (existing is null)
        {
            _context.Scores.Add(new Score
            {
                Id = Guid.NewGuid(),
                ApplicationId = applicationId,
                Dimension = dimension,
                Value = dto.Score,
                Comment = dto.Comment,
                ScoredById = teamMemberId,
                ScoredAt = DateTime.UtcNow
            });
        }
        else
        {
            existing.Value = dto.Score;
            existing.Comment = dto.Comment;
            existing.ScoredById = teamMemberId;
            existing.ScoredAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        return Ok(new
        {
            dimension,
            score = dto.Score,
            comment = dto.Comment,
            scoredBy = member.Name
        });
    }
}