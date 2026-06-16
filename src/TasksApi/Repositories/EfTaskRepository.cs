using Microsoft.EntityFrameworkCore;
using TasksApi.Data;
using TasksApi.Models;

namespace TasksApi.Repositories;

/// <summary>
/// EF Core backed implementation of <see cref="ITaskRepository"/>.
/// </summary>
public class EfTaskRepository : ITaskRepository
{
    private readonly TasksDbContext _db;

    public EfTaskRepository(TasksDbContext db) => _db = db;

    public async Task<IReadOnlyList<TaskItem>> ListAsync(bool? done, CancellationToken cancellationToken = default)
    {
        IQueryable<TaskItem> query = _db.Tasks.AsNoTracking();

        if (done is not null)
        {
            query = query.Where(t => t.Done == done);
        }

        return await query.OrderBy(t => t.CreatedAt).ToListAsync(cancellationToken);
    }

    public async Task<TaskItem?> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _db.Tasks.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task<TaskItem> AddAsync(TaskItem item, CancellationToken cancellationToken = default)
    {
        _db.Tasks.Add(item);
        await _db.SaveChangesAsync(cancellationToken);
        return item;
    }

    public async Task<bool> UpdateAsync(TaskItem item, CancellationToken cancellationToken = default)
    {
        var exists = await _db.Tasks.AnyAsync(t => t.Id == item.Id, cancellationToken);
        if (!exists)
        {
            return false;
        }

        _db.Tasks.Update(item);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        _db.Tasks.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
