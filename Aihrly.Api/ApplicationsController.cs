using Microsoft.AspNetCore.Mvc;
using Aihrly.Api.Data;
using Aihrly.Api.Entities;
using Aihrly.Api.DTOs;

namespace Aihrly.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApplicationsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ApplicationsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateApplicationDto dto)
    {
        var app = new Application
        {
            Id = Guid.NewGuid(),
            JobId = dto.JobId,
            CandidateName = dto.CandidateName,
            CandidateEmail = dto.CandidateEmail,
            CreatedById = dto.TeamMemberId
        };

        _context.Applications.Add(app);
        await _context.SaveChangesAsync();

        return Ok(app);
    }
}