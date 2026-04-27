using System.ComponentModel.DataAnnotations;

namespace Aihrly.Api.Entities;

public class Application
{
    public Guid Id { get; set; }

    [Required]
    public Guid JobId { get; set; }

    [Required]
    public string CandidateName { get; set; }

    [Required]
    [EmailAddress]
    public string CandidateEmail { get; set; }

    public string Stage { get; set; } = "applied";
}