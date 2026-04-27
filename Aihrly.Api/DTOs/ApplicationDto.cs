namespace Aihrly.Api.DTOs;

public class ApplicationDto
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public string CandidateName { get; set; } = string.Empty;
    public string CandidateEmail { get; set; } = string.Empty;
    public string? CoverLetter { get; set; }
    public string Stage { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}