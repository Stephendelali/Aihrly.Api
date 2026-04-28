using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Aihrly.Api.Data;
using Aihrly.Api.DTOs;
using Aihrly.Api.Entities;

namespace Aihrly.Api.Controllers;

[ApiController]
public class NotesController : ControllerBase
{
    private readonly AppDbContext _context;

    public NotesController(AppDbContext context)
    {
        _context = context;
    }

    // POST /api/applications/{id}/notes
    [HttpPost("api/applications/{id}/notes")]
    public async Task<IActionResult> AddNote(
        Guid id,
        [FromBody] CreateNoteDto dto,
        [FromHeader(Name = "X-Team-Member-Id")] Guid teamMemberId
    )

    {
        var member = await _context.TeamMembers.FindAsync(teamMemberId);
        if (member is null) return BadRequest("Team member not found");

        var app = await _context.Applications.FindAsync(id);
        if (app is null) return NotFound("Application not found");

        var validTypes = new[] { "general", "screening", "interview", "reference_check", "red_flag" };
        if (!validTypes.Contains(dto.Type))
            return BadRequest($"Invalid note type. Must be one of: {string.Join(", ", validTypes)}");

        var note = new ApplicationNote
        {
            Id = Guid.NewGuid(),
            ApplicationId = id,
            Type = dto.Type,
            Description = dto.Description,
            CreatedById = teamMemberId,
            CreatedAt = DateTime.UtcNow
        };

        _context.ApplicationNotes.Add(note);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetNotes), new { id }, new NoteDto
        {
            Id = note.Id,
            Type = note.Type,
            Description = note.Description,
            AuthorName = member.Name,
            CreatedAt = note.CreatedAt
        });
    }

    // GET /api/applications/{id}/notes
    [HttpGet("api/applications/{id}/notes")]
    public async Task<IActionResult> GetNotes(Guid id)
    {
        var app = await _context.Applications.FindAsync(id);
        if (app is null) return NotFound("Application not found");

        var notes = await _context.ApplicationNotes
            .Where(n => n.ApplicationId == id)
            .Include(n => n.CreatedBy)
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new NoteDto
            {
                Id = n.Id,
                Type = n.Type,
                Description = n.Description,
                AuthorName = n.CreatedBy.Name,
                CreatedAt = n.CreatedAt
            })
            .ToListAsync();

        return Ok(notes);
    }
}