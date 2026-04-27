using System.ComponentModel.DataAnnotations;

namespace Aihrly.Api.Entities;

public class Application
{
    public Guid Id { get; set; }

    [Required]
    public Guid JobId { get; set; }

    [Required]
    public required string CandidateName { get; set; }

    [Required]
    [EmailAddress]
    public required string CandidateEmail { get; set; }

    public string Stage { get; set; } = "applied";
    public Guid CreatedById { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}