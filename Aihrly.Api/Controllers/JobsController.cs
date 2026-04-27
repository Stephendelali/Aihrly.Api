using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Aihrly.Api.Data;
using Aihrly.Api.DTOs;
using Aihrly.Api.Entities;

namespace Aihrly.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    private readonly AppDbContext _context;

    public JobsController(AppDbContext context)
    {
        _context = context;
    }

    // POST /api/jobs
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateJobDto dto)
    {
        var job = new Job
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Description = dto.Description,
            Location = dto.Location,
            Status = "open"
        };

        _context.Jobs.Add(job);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = job.Id }, ToDto(job));
    }

    // GET /api/jobs?status=open&page=1&pageSize=20
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = _context.Jobs.AsQueryable();

        if (!string.IsNullOrEmpty(status))
            query = query.Where(j => j.Status == status);

        var total = await query.CountAsync();
        var jobs = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(j => ToDto(j))
            .ToListAsync();

        return Ok(new { total, page, pageSize, data = jobs });
    }

    // GET /api/jobs/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var job = await _context.Jobs.FindAsync(id);
        if (job is null) return NotFound();
        return Ok(ToDto(job));
    }

    private static JobDto ToDto(Job job) => new()
    {
        Id = job.Id,
        Title = job.Title,
        Description = job.Description,
        Location = job.Location,
        Status = job.Status
    };
}