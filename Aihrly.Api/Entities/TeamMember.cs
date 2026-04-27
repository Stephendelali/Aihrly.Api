using System.ComponentModel.DataAnnotations;

namespace Aihrly.Api.Entities;

public class TeamMember
{
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    public string Role { get; set; } // recruiter, hiring_manager
}