namespace Aihrly.Api.DTOs;

public class CreateApplicationDto
{
    public Guid JobId { get; set; }
    public string CandidateName { get; set; } = string.Empty;
    public string CandidateEmail { get; set; } = string.Empty;
    public Guid TeamMemberId { get; set; }
}