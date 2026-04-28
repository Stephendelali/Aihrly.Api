using Microsoft.EntityFrameworkCore;
using Npgsql;
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

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Controllers + swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod());
});

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("TeamMemberId", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "X-Team-Member-Id",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Team Member ID for authenticated actions"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "TeamMemberId"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Seed TeamMembers
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();

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

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

Console.WriteLine(connectionString);

app.Run();