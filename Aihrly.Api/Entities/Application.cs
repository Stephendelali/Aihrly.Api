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

    public string? CoverLetter { get; set; }
    public string Stage { get; set; } = "applied";
    public Guid CreatedById { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Job Job { get; set; } = null!;
    public ICollection<ApplicationNote> Notes { get; set; } = new List<ApplicationNote>();
    public ICollection<Score> Scores { get; set; } = new List<Score>();
    public ICollection<StageHistory> StageHistories { get; set; } = new List<StageHistory>();
}