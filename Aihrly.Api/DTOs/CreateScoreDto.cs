using System.ComponentModel.DataAnnotations;

namespace Aihrly.Api.DTOs;

public class CreateScoreDto
{
    [Required]
    [Range(1, 5)]
    public int Score { get; set; }

    public string? Comment { get; set; }
}