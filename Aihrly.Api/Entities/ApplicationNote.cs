namespace Aihrly.Api.Entities;

public class ApplicationNote
{
    public Guid Id { get; set; }

    public Guid ApplicationId { get; set; }

    public required string Type { get; set; } // note, call, email

    public required string Description { get; set; }

    public Guid CreatedById { get; set; } // team member

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Application Application { get; set; } = null!;
    public TeamMember CreatedBy { get; set; } = null!;
}
