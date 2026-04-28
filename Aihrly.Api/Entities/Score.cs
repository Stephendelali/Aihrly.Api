namespace Aihrly.Api.Entities;

public class Score
{
    public Guid Id { get; set; }

    public Guid ApplicationId { get; set; }

    public string Type { get; set; } // culture-fit, interview, assessment

    public int Value { get; set; }

    public string Comment { get; set; }

    public Guid UpdatedById { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Application Application { get; set; } = null!;
    public TeamMember ScoredBy { get; set; } = null!;
}