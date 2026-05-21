using AzureTaskApi.Models;

namespace AzureTaskApi.Services;

public interface ITaskService
{
    Task<IEnumerable<TaskItem>> GetAllAsync();
    Task<TaskItem?> GetByIdAsync(int id);
    Task<TaskItem> CreateAsync(CreateTaskRequest request);
    Task<TaskItem?> UpdateAsync(int id, UpdateTaskRequest request);
    Task<bool> DeleteAsync(int id);
}
