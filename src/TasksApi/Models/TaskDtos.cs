namespace TasksApi.Models;

/// <summary>
/// Payload for creating a new task.
/// </summary>
/// <param name="Title">The task title. Must be non-empty.</param>
/// <param name="Done">Whether the task is already completed. Defaults to false.</param>
public record CreateTaskRequest(string? Title, bool Done = false);

/// <summary>
/// Payload for replacing an existing task.
/// </summary>
/// <param name="Title">The task title. Must be non-empty.</param>
/// <param name="Done">Completion state of the task.</param>
public record UpdateTaskRequest(string? Title, bool Done);

/// <summary>
/// Response shape returned to clients for a task.
/// </summary>
public record TaskResponse(Guid Id, string Title, bool Done, DateTime CreatedAt)
{
    public static TaskResponse FromEntity(TaskItem item) =>
        new(item.Id, item.Title, item.Done, item.CreatedAt);
}
