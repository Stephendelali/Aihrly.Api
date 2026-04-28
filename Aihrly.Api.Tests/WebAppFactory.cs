using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Aihrly.Api.Data;

namespace Aihrly.Api.Tests;

public class WebAppFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove real DB
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor != null) services.Remove(descriptor);

            // Add in-memory DB
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("TestDb"));

            // Seed test data
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();

            if (!db.TeamMembers.Any())
            {
                db.TeamMembers.Add(new Aihrly.Api.Entities.TeamMember
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Name = "Alice Mensah",
                    Email = "alice@aihrly.com",
                    Role = "recruiter"
                });
                db.SaveChanges();
            }
        });
    }
}