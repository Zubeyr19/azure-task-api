using Microsoft.AspNetCore.Mvc;
using AzureTaskApi.Models;
using AzureTaskApi.Services;

namespace AzureTaskApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _service;
    private readonly ILogger<TasksController> _logger;

    public TasksController(ITaskService service, ILogger<TasksController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var tasks = await _service.GetAllAsync();
        return Ok(tasks);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var task = await _service.GetByIdAsync(id);
        if (task is null)
        {
            _logger.LogWarning("Task {Id} not found", id);
            return NotFound();
        }
        return Ok(task);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskRequest request)
    {
        var task = await _service.CreateAsync(request);
        _logger.LogInformation("Created task {Id}: {Title}", task.Id, task.Title);
        return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTaskRequest request)
    {
        var task = await _service.UpdateAsync(id, request);
        if (task is null) return NotFound();
        return Ok(task);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
