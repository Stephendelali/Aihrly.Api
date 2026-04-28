namespace Aihrly.Api.Entities;

public class StageHistory
{
    public Guid Id { get; set; }

    public Guid ApplicationId { get; set; }

    public string FromStage { get; set; }

    public string ToStage { get; set; }

    public Guid ChangedById { get; set; }

    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

    public string? Reason { get; set; }


    // Navigation properties
    public Application Application { get; set; } = null!;
    public TeamMember ChangedBy { get; set; } = null!;
}