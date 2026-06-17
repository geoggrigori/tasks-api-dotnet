using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TasksApi.Models;
using Xunit;

namespace TasksApi.Tests;

public class TaskEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public TaskEndpointsTests(WebApplicationFactory<Program> factory) => _factory = factory;

    private HttpClient CreateClient() => _factory.CreateClient();

    [Fact]
    public async Task Create_ReturnsCreatedWithLocation()
    {
        var client = CreateClient();

        var response = await client.PostAsJsonAsync("/tasks", new CreateTaskRequest("Buy milk"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var created = await response.Content.ReadFromJsonAsync<TaskResponse>();
        Assert.NotNull(created);
        Assert.Equal("Buy milk", created!.Title);
        Assert.False(created.Done);
        Assert.NotEqual(Guid.Empty, created.Id);
        Assert.Contains(created.Id.ToString(), response.Headers.Location!.ToString());
    }

    [Fact]
    public async Task Create_EmptyTitle_ReturnsBadRequest()
    {
        var client = CreateClient();

        var response = await client.PostAsJsonAsync("/tasks", new CreateTaskRequest("   "));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetById_ReturnsCreatedTask()
    {
        var client = CreateClient();

        var created = await (await client.PostAsJsonAsync("/tasks", new CreateTaskRequest("Read book")))
            .Content.ReadFromJsonAsync<TaskResponse>();

        var response = await client.GetAsync($"/tasks/{created!.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var fetched = await response.Content.ReadFromJsonAsync<TaskResponse>();
        Assert.Equal(created.Id, fetched!.Id);
        Assert.Equal("Read book", fetched.Title);
    }

    [Fact]
    public async Task GetById_Unknown_ReturnsNotFound()
    {
        var client = CreateClient();

        var response = await client.GetAsync($"/tasks/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task List_ReturnsCreatedTasks()
    {
        var client = CreateClient();

        await client.PostAsJsonAsync("/tasks", new CreateTaskRequest("List item A"));
        await client.PostAsJsonAsync("/tasks", new CreateTaskRequest("List item B"));

        var tasks = await client.GetFromJsonAsync<List<TaskResponse>>("/tasks");

        Assert.NotNull(tasks);
        Assert.Contains(tasks!, t => t.Title == "List item A");
        Assert.Contains(tasks!, t => t.Title == "List item B");
    }

    [Fact]
    public async Task List_DoneFilter_ReturnsMatchingTasks()
    {
        var client = CreateClient();

        await client.PostAsJsonAsync("/tasks", new CreateTaskRequest("Filter done", Done: true));
        await client.PostAsJsonAsync("/tasks", new CreateTaskRequest("Filter open", Done: false));

        var done = await client.GetFromJsonAsync<List<TaskResponse>>("/tasks?done=true");
        var open = await client.GetFromJsonAsync<List<TaskResponse>>("/tasks?done=false");

        Assert.All(done!, t => Assert.True(t.Done));
        Assert.All(open!, t => Assert.False(t.Done));
        Assert.Contains(done!, t => t.Title == "Filter done");
        Assert.Contains(open!, t => t.Title == "Filter open");
    }

    [Fact]
    public async Task Update_TogglesDone()
    {
        var client = CreateClient();

        var created = await (await client.PostAsJsonAsync("/tasks", new CreateTaskRequest("Toggle me")))
            .Content.ReadFromJsonAsync<TaskResponse>();

        var response = await client.PutAsJsonAsync($"/tasks/{created!.Id}",
            new UpdateTaskRequest("Toggle me", Done: true));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await response.Content.ReadFromJsonAsync<TaskResponse>();
        Assert.True(updated!.Done);
        Assert.Equal(created.Id, updated.Id);
    }

    [Fact]
    public async Task Update_Unknown_ReturnsNotFound()
    {
        var client = CreateClient();

        var response = await client.PutAsJsonAsync($"/tasks/{Guid.NewGuid()}",
            new UpdateTaskRequest("Ghost", Done: false));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Toggle_FlipsDone_ReturnsOk()
    {
        var client = CreateClient();

        var created = await (await client.PostAsJsonAsync("/tasks", new CreateTaskRequest("Toggle endpoint")))
            .Content.ReadFromJsonAsync<TaskResponse>();
        Assert.False(created!.Done);

        var first = await client.PatchAsync($"/tasks/{created.Id}/toggle", null);
        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        var afterFirst = await first.Content.ReadFromJsonAsync<TaskResponse>();
        Assert.Equal(created.Id, afterFirst!.Id);
        Assert.True(afterFirst.Done);

        var second = await client.PatchAsync($"/tasks/{created.Id}/toggle", null);
        Assert.Equal(HttpStatusCode.OK, second.StatusCode);
        var afterSecond = await second.Content.ReadFromJsonAsync<TaskResponse>();
        Assert.False(afterSecond!.Done);
    }

    [Fact]
    public async Task Toggle_Unknown_ReturnsNotFound()
    {
        var client = CreateClient();

        var response = await client.PatchAsync($"/tasks/{Guid.NewGuid()}/toggle", null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_ThenNotFound()
    {
        var client = CreateClient();

        var created = await (await client.PostAsJsonAsync("/tasks", new CreateTaskRequest("Delete me")))
            .Content.ReadFromJsonAsync<TaskResponse>();

        var deleteResponse = await client.DeleteAsync($"/tasks/{created!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var secondDelete = await client.DeleteAsync($"/tasks/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, secondDelete.StatusCode);

        var getResponse = await client.GetAsync($"/tasks/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}
