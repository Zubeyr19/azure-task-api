using Microsoft.AspNetCore.Mvc;
using Moq;
using AzureTaskApi.Controllers;
using AzureTaskApi.Models;
using AzureTaskApi.Services;
using Microsoft.Extensions.Logging;
using Xunit;

namespace AzureTaskApi.Tests;

public class TasksControllerTests
{
    private readonly Mock<ITaskService> _serviceMock;
    private readonly Mock<ILogger<TasksController>> _loggerMock;
    private readonly TasksController _controller;

    public TasksControllerTests()
    {
        _serviceMock = new Mock<ITaskService>();
        _loggerMock = new Mock<ILogger<TasksController>>();
        _controller = new TasksController(_serviceMock.Object, _loggerMock.Object);
    }

    private static TaskItem MakeTask(int id = 1, string title = "Test task") =>
        new() { Id = id, Title = title, Status = TaskStatus.Todo, CreatedAt = DateTime.UtcNow };

    [Fact]
    public async Task GetAll_Returns200WithList()
    {
        _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new[] { MakeTask() });
        var result = await _controller.GetAll();
        var ok = Assert.IsType<OkObjectResult>(result);
        var items = Assert.IsAssignableFrom<IEnumerable<TaskItem>>(ok.Value);
        Assert.Single(items);
    }

    [Fact]
    public async Task GetById_Returns200WhenFound()
    {
        _serviceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(MakeTask(1));
        var result = await _controller.GetById(1);
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetById_Returns404WhenNotFound()
    {
        _serviceMock.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((TaskItem?)null);
        var result = await _controller.GetById(99);
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Create_Returns201WithLocation()
    {
        var request = new CreateTaskRequest { Title = "New task" };
        var created = MakeTask(5, "New task");
        _serviceMock.Setup(s => s.CreateAsync(request)).ReturnsAsync(created);
        var result = await _controller.Create(request);
        var createdAt = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(TasksController.GetById), createdAt.ActionName);
        Assert.Equal(5, ((TaskItem)createdAt.Value!).Id);
    }

    [Fact]
    public async Task Update_Returns200OnSuccess()
    {
        var req = new UpdateTaskRequest { Title = "Updated" };
        _serviceMock.Setup(s => s.UpdateAsync(1, req)).ReturnsAsync(MakeTask(1, "Updated"));
        var result = await _controller.Update(1, req);
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Update_Returns404WhenNotFound()
    {
        var req = new UpdateTaskRequest { Title = "Ghost" };
        _serviceMock.Setup(s => s.UpdateAsync(99, req)).ReturnsAsync((TaskItem?)null);
        var result = await _controller.Update(99, req);
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_Returns204OnSuccess()
    {
        _serviceMock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);
        var result = await _controller.Delete(1);
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_Returns404WhenNotFound()
    {
        _serviceMock.Setup(s => s.DeleteAsync(99)).ReturnsAsync(false);
        var result = await _controller.Delete(99);
        Assert.IsType<NotFoundResult>(result);
    }
}
