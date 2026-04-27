using Aihrly.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aihrly.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Job> Jobs { get; set; }
    public DbSet<Application> Applications { get; set; }
    public DbSet<ApplicationNote> ApplicationNotes { get; set; }
    public DbSet<Score> Scores { get; set; }
    public DbSet<StageHistory> StageHistories { get; set; }
    public DbSet<TeamMember> TeamMembers { get; set; }
}