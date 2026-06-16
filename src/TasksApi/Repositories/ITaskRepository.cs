using TasksApi.Models;

namespace TasksApi.Repositories;

/// <summary>
/// Abstraction over task persistence.
/// </summary>
public interface ITaskRepository
{
    Task<IReadOnlyList<TaskItem>> ListAsync(bool? done, CancellationToken cancellationToken = default);

    Task<TaskItem?> GetAsync(Guid id, CancellationToken cancellationToken = default);

    Task<TaskItem> AddAsync(TaskItem item, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(TaskItem item, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
