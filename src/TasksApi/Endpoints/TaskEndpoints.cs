using TasksApi.Models;
using TasksApi.Repositories;

namespace TasksApi.Endpoints;

/// <summary>
/// Maps the CRUD endpoints for tasks.
/// </summary>
public static class TaskEndpoints
{
    public static IEndpointRouteBuilder MapTaskEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/tasks").WithTags("Tasks");

        group.MapGet("/", async (bool? done, ITaskRepository repo, CancellationToken ct) =>
        {
            var tasks = await repo.ListAsync(done, ct);
            return Results.Ok(tasks.Select(TaskResponse.FromEntity));
        })
        .WithName("ListTasks")
        .WithSummary("List tasks, optionally filtered by completion state.");

        group.MapGet("/{id:guid}", async (Guid id, ITaskRepository repo, CancellationToken ct) =>
        {
            var task = await repo.GetAsync(id, ct);
            return task is null
                ? Results.NotFound()
                : Results.Ok(TaskResponse.FromEntity(task));
        })
        .WithName("GetTask")
        .WithSummary("Get a single task by id.");

        group.MapPost("/", async (CreateTaskRequest request, ITaskRepository repo, CancellationToken ct) =>
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    ["title"] = ["Title is required and cannot be empty."]
                });
            }

            var item = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = request.Title.Trim(),
                Done = request.Done,
                CreatedAt = DateTime.UtcNow
            };

            await repo.AddAsync(item, ct);
            var response = TaskResponse.FromEntity(item);
            return Results.Created($"/tasks/{item.Id}", response);
        })
        .WithName("CreateTask")
        .WithSummary("Create a new task.");

        group.MapPut("/{id:guid}", async (Guid id, UpdateTaskRequest request, ITaskRepository repo, CancellationToken ct) =>
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    ["title"] = ["Title is required and cannot be empty."]
                });
            }

            var existing = await repo.GetAsync(id, ct);
            if (existing is null)
            {
                return Results.NotFound();
            }

            existing.Title = request.Title.Trim();
            existing.Done = request.Done;

            await repo.UpdateAsync(existing, ct);
            return Results.Ok(TaskResponse.FromEntity(existing));
        })
        .WithName("UpdateTask")
        .WithSummary("Replace an existing task.");

        group.MapPatch("/{id:guid}/toggle", async (Guid id, ITaskRepository repo, CancellationToken ct) =>
        {
            var existing = await repo.GetAsync(id, ct);
            if (existing is null)
            {
                return Results.NotFound();
            }

            existing.Done = !existing.Done;

            await repo.UpdateAsync(existing, ct);
            return Results.Ok(TaskResponse.FromEntity(existing));
        })
        .WithName("ToggleTask")
        .WithSummary("Flip the completion state of a task.");

        group.MapDelete("/{id:guid}", async (Guid id, ITaskRepository repo, CancellationToken ct) =>
        {
            var deleted = await repo.DeleteAsync(id, ct);
            return deleted ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteTask")
        .WithSummary("Delete a task by id.");

        return app;
    }
}
