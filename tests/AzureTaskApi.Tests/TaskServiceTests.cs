using Microsoft.EntityFrameworkCore;
using AzureTaskApi.Data;
using AzureTaskApi.Models;
using AzureTaskApi.Services;
using Xunit;

namespace AzureTaskApi.Tests;

public class TaskServiceTests : IDisposable
{
    private readonly AppDbContext _db;
    private readonly TaskService _service;

    public TaskServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _db = new AppDbContext(options);
        _service = new TaskService(_db);
    }

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task CreateAsync_ReturnsTaskWithId()
    {
        var request = new CreateTaskRequest { Title = "Write tests", Description = "Important" };
        var task = await _service.CreateAsync(request);
        Assert.True(task.Id > 0);
        Assert.Equal("Write tests", task.Title);
        Assert.Equal(TaskStatus.Todo, task.Status);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllTasks()
    {
        await _service.CreateAsync(new CreateTaskRequest { Title = "Task A" });
        await _service.CreateAsync(new CreateTaskRequest { Title = "Task B" });
        var tasks = await _service.GetAllAsync();
        Assert.Equal(2, tasks.Count());
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsCorrectTask()
    {
        var created = await _service.CreateAsync(new CreateTaskRequest { Title = "Find me" });
        var found = await _service.GetByIdAsync(created.Id);
        Assert.NotNull(found);
        Assert.Equal("Find me", found.Title);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNullForMissingId()
    {
        var result = await _service.GetByIdAsync(9999);
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_ChangesTitle()
    {
        var task = await _service.CreateAsync(new CreateTaskRequest { Title = "Old title" });
        var updated = await _service.UpdateAsync(task.Id, new UpdateTaskRequest { Title = "New title" });
        Assert.NotNull(updated);
        Assert.Equal("New title", updated.Title);
        Assert.NotNull(updated.UpdatedAt);
    }

    [Fact]
    public async Task UpdateAsync_ChangesStatus()
    {
        var task = await _service.CreateAsync(new CreateTaskRequest { Title = "Pending task" });
        var updated = await _service.UpdateAsync(task.Id, new UpdateTaskRequest { Status = TaskStatus.Done });
        Assert.NotNull(updated);
        Assert.Equal(TaskStatus.Done, updated.Status);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNullForMissingId()
    {
        var result = await _service.UpdateAsync(9999, new UpdateTaskRequest { Title = "Ghost" });
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_RemovesTask()
    {
        var task = await _service.CreateAsync(new CreateTaskRequest { Title = "Delete me" });
        var deleted = await _service.DeleteAsync(task.Id);
        Assert.True(deleted);
        Assert.Null(await _service.GetByIdAsync(task.Id));
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalseForMissingId()
    {
        var result = await _service.DeleteAsync(9999);
        Assert.False(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsNewestFirst()
    {
        var first = await _service.CreateAsync(new CreateTaskRequest { Title = "First" });
        await Task.Delay(5);
        var second = await _service.CreateAsync(new CreateTaskRequest { Title = "Second" });
        var tasks = (await _service.GetAllAsync()).ToList();
        Assert.Equal(second.Id, tasks[0].Id);
    }
}
