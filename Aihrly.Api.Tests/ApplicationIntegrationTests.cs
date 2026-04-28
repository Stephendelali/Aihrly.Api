using System.Net;
using System.Net.Http.Json;

namespace Aihrly.Api.Tests;

public class ApplicationIntegrationTests : IClassFixture<WebAppFactory>
{
    private readonly HttpClient _client;
    private const string TeamMemberId = "11111111-1111-1111-1111-111111111111";

    public ApplicationIntegrationTests(WebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<Guid> CreateJobAsync()
    {
        var response = await _client.PostAsJsonAsync("/api/jobs", new { title = "Software Engineer", description = "Build cool stuff", location = "Accra, Ghana" });
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<JobResponse>();
        return body!.Id;
    }

    [Fact]
    public async Task CreateJob_ReturnsCreated_WithJobDetails()
    {
        var response = await _client.PostAsJsonAsync("/api/jobs", new { title = "Product Manager", description = "Lead product strategy", location = "Remote" });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JobResponse>();
        Assert.NotNull(body);
        Assert.Equal("Product Manager", body.Title);
        Assert.Equal("open", body.Status);
    }

    [Fact]
    public async Task CreateApplication_ReturnsCreated_WithAppliedStage()
    {
        var jobId = await CreateJobAsync();
        var response = await _client.PostAsJsonAsync($"/api/jobs/{jobId}/applications", new { candidateName = "Kofi Mensah", candidateEmail = "kofi@example.com", coverLetter = "I am very interested in this role." });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ApplicationResponse>();
        Assert.NotNull(body);
        Assert.Equal("Kofi Mensah", body.CandidateName);
        Assert.Equal("applied", body.Stage);
    }

    [Fact]
    public async Task MoveStage_FromAppliedToScreening_ReturnsOk()
    {
        var jobId = await CreateJobAsync();
        var appResponse = await _client.PostAsJsonAsync($"/api/jobs/{jobId}/applications", new { candidateName = "Ama Owusu", candidateEmail = "ama@example.com", coverLetter = "Excited to apply!" });
        var app = await appResponse.Content.ReadFromJsonAsync<ApplicationResponse>();
        var request = new HttpRequestMessage(HttpMethod.Patch, $"/api/applications/{app!.Id}/stage") { Content = JsonContent.Create(new { stage = "screening", reason = "Looks promising" }) };
        request.Headers.Add("X-Team-Member-Id", TeamMemberId);
        var stageResponse = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.OK, stageResponse.StatusCode);
        var updated = await stageResponse.Content.ReadFromJsonAsync<ApplicationResponse>();
        Assert.Equal("screening", updated!.Stage);
    }

    [Fact]
    public async Task AddNote_ToApplication_ReturnsCreated()
    {
        var jobId = await CreateJobAsync();
        var appResponse = await _client.PostAsJsonAsync($"/api/jobs/{jobId}/applications", new { candidateName = "Yaw Asante", candidateEmail = "yaw@example.com", coverLetter = "Looking forward to this opportunity." });
        var app = await appResponse.Content.ReadFromJsonAsync<ApplicationResponse>();
        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/applications/{app!.Id}/notes") { Content = JsonContent.Create(new { type = "general", description = "Strong communication skills observed." }) };
        request.Headers.Add("X-Team-Member-Id", TeamMemberId);
        var noteResponse = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.Created, noteResponse.StatusCode);
        var note = await noteResponse.Content.ReadFromJsonAsync<NoteResponse>();
        Assert.NotNull(note);
        Assert.Equal("general", note.Type);
    }

    private record JobResponse(Guid Id, string Title, string Status);
    private record ApplicationResponse(Guid Id, string CandidateName, string Stage);
    private record NoteResponse(Guid Id, string Type, string Description);
}