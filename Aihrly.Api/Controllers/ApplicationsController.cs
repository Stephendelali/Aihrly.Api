using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Aihrly.Api.Data;
using Aihrly.Api.DTOs;
using Aihrly.Api.Entities;

namespace Aihrly.Api.Controllers;

[ApiController]
public class ApplicationsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ApplicationsController(AppDbContext context)
    {
        _context = context;
    }

    // POST /api/jobs/{jobId}/applications
    [HttpPost("api/jobs/{jobId}/applications")]
    public async Task<IActionResult> Create(Guid jobId, [FromBody] CreateApplicationDto dto)
    {
        var job = await _context.Jobs.FindAsync(jobId);
        if (job is null) return NotFound("Job not found");

        // Prevent duplicate application (same email + same job)
        var duplicate = await _context.Applications
            .AnyAsync(a => a.JobId == jobId && a.CandidateEmail == dto.CandidateEmail);
        if (duplicate)
            return Conflict("A candidate with this email has already applied to this job");

        var app = new Application
        {
            Id = Guid.NewGuid(),
            JobId = jobId,
            CandidateName = dto.CandidateName,
            CandidateEmail = dto.CandidateEmail,
            CoverLetter = dto.CoverLetter,
            Stage = "applied",
            CreatedAt = DateTime.UtcNow
        };

        _context.Applications.Add(app);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = app.Id }, ToDto(app));
    }

    // GET /api/jobs/{jobId}/applications?stage=screening
    [HttpGet("api/jobs/{jobId}/applications")]
    public async Task<IActionResult> GetByJob(Guid jobId, [FromQuery] string? stage)
    {
        var job = await _context.Jobs.FindAsync(jobId);
        if (job is null) return NotFound("Job not found");

        var query = _context.Applications
            .Where(a => a.JobId == jobId)
            .AsQueryable();

        if (!string.IsNullOrEmpty(stage))
            query = query.Where(a => a.Stage == stage);

        var apps = await query.Select(a => ToDto(a)).ToListAsync();
        return Ok(apps);
    }

    // GET /api/applications/{id}
    [HttpGet("api/applications/{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var app = await _context.Applications.FindAsync(id);
        if (app is null) return NotFound();
        return Ok(ToDto(app));
    }

    
    [HttpPatch("api/applications/{id}/stage")]
    public async Task<IActionResult> MoveStage(Guid id, [FromBody] MoveStageDto dto)
    {
        var memberId = Request.Headers["X-Team-Member-Id"].FirstOrDefault();
        if (string.IsNullOrEmpty(memberId) || !Guid.TryParse(memberId, out var teamMemberId))
            return BadRequest("Missing or invalid X-Team-Member-Id header");

        var member = await _context.TeamMembers.FindAsync(teamMemberId);
        if (member is null) return BadRequest("Team member not found");

        var app = await _context.Applications.FindAsync(id);
        if (app is null) return NotFound();

        // Validate stage transition
        if (!IsValidTransition(app.Stage, dto.Stage))
            return BadRequest($"Invalid stage transition: {app.Stage} → {dto.Stage}");

        var history = new StageHistory
        {
            Id = Guid.NewGuid(),
            ApplicationId = id,
            FromStage = app.Stage,
            ToStage = dto.Stage,
            ChangedById = teamMemberId,
            ChangedAt = DateTime.UtcNow,
            Reason = dto.Reason
        };

        app.Stage = dto.Stage;

        _context.StageHistories.Add(history);
        await _context.SaveChangesAsync();

        return Ok(ToDto(app));
    }

    private static bool IsValidTransition(string from, string to)
    {
        var allowed = new Dictionary<string, string[]>
        {
            ["applied"] = ["screening", "rejected"],
            ["screening"] = ["interview", "rejected"],
            ["interview"] = ["offer", "rejected"],
            ["offer"] = ["hired", "rejected"],
            ["hired"] = [],
            ["rejected"] = []
        };

        return allowed.TryGetValue(from, out var targets) && targets.Contains(to);
    }

    private static ApplicationDto ToDto(Application app) => new()
    {
        Id = app.Id,
        JobId = app.JobId,
        CandidateName = app.CandidateName,
        CandidateEmail = app.CandidateEmail,
        CoverLetter = app.CoverLetter,
        Stage = app.Stage,
        CreatedAt = app.CreatedAt
    };
}