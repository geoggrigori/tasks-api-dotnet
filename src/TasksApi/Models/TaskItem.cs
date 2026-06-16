namespace TasksApi.Models;

/// <summary>
/// Represents a single task (todo) item.
/// </summary>
public class TaskItem
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public bool Done { get; set; }

    public DateTime CreatedAt { get; set; }
}
