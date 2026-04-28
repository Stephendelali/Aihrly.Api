using System.ComponentModel.DataAnnotations;

namespace Aihrly.Api.Entities;

public class Job
{
    public Guid Id { get; set; }

    [Required]
    public string Title { get; set; }

    [Required]
    public string Description { get; set; }

    public required string Location { get; set; }

    public string Status { get; set; } = "open"; // open / closed
}
