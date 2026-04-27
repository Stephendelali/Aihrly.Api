using System.ComponentModel.DataAnnotations;

namespace Aihrly.Api.DTOs;

public class CreateJobDto
{
    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;
}