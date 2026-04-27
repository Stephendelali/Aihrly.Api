using System.ComponentModel.DataAnnotations;

namespace Aihrly.Api.DTOs;

public class MoveStageDto
{
    [Required]
    public string Stage { get; set; } = string.Empty;
    public string? Reason { get; set; }
}