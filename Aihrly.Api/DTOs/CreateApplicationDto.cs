using System.ComponentModel.DataAnnotations;

namespace Aihrly.Api.DTOs;

public class CreateApplicationDto
{
    [Required]
    public string CandidateName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string CandidateEmail { get; set; } = string.Empty;

    public string? CoverLetter { get; set; }
}