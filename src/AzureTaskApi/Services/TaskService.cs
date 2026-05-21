using Microsoft.EntityFrameworkCore;
using AzureTaskApi.Data;
using AzureTaskApi.Models;

namespace AzureTaskApi.Services;

public class TaskService : ITaskService
{
    private readonly AppDbContext _db;

    public TaskService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<TaskItem>> GetAllAsync()
    {
        return await _db.Tasks.OrderByDescending(t => t.CreatedAt).ToListAsync();
    }

    public async Task<TaskItem?> GetByIdAsync(int id)
    {
        return await _db.Tasks.FindAsync(id);
    }

    public async Task<TaskItem> CreateAsync(CreateTaskRequest request)
    {
        var task = new TaskItem
        {
            Title = request.Title,
            Description = request.Description,
        };
        _db.Tasks.Add(task);
        await _db.SaveChangesAsync();
        return task;
    }

    public async Task<TaskItem?> UpdateAsync(int id, UpdateTaskRequest request)
    {
        var task = await _db.Tasks.FindAsync(id);
        if (task is null) return null;

        if (request.Title is not null) task.Title = request.Title;
        if (request.Description is not null) task.Description = request.Description;
        if (request.Status is not null) task.Status = request.Status.Value;
        task.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return task;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var task = await _db.Tasks.FindAsync(id);
        if (task is null) return false;

        _db.Tasks.Remove(task);
        await _db.SaveChangesAsync();
        return true;
    }
}
