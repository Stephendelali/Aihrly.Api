using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using Microsoft.OpenApi.Models;
using Aihrly.Api.Data;

Env.Load();

var builder = WebApplication.CreateBuilder(args);
var host = Environment.GetEnvironmentVariable("DB_HOST");
var port = Environment.GetEnvironmentVariable("DB_PORT");
var db = Environment.GetEnvironmentVariable("DB_NAME");
var user = Environment.GetEnvironmentVariable("DB_USER");
var password = Environment.GetEnvironmentVariable("DB_PASSWORD");

var connectionString =
    $"Host={host};Port={port};Database={db};Username={user};Password={password}";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("TeamMemberId", new OpenApiSecurityScheme
    {
        Name = "X-Team-Member-Id",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Description = "Team Member ID for authenticated actions"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "TeamMemberId"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (context.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
    {
        context.Database.Migrate();
    }

    if (!context.TeamMembers.Any())
    {
        context.TeamMembers.AddRange(
            new Aihrly.Api.Entities.TeamMember
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Alice Mensah",
                Email = "alice@aihrly.com",
                Role = "recruiter"
            },
            new Aihrly.Api.Entities.TeamMember
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "Bob Asante",
                Email = "bob@aihrly.com",
                Role = "hiring_manager"
            },
            new Aihrly.Api.Entities.TeamMember
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "Clara Owusu",
                Email = "clara@aihrly.com",
                Role = "recruiter"
            }
        );
        context.SaveChanges();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }
