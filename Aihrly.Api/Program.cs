using Microsoft.EntityFrameworkCore;
using Npgsql;
using DotNetEnv;
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
builder.Services.AddSwaggerGen();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod());
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