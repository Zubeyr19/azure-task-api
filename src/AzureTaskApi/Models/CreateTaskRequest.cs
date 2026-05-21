using System.ComponentModel.DataAnnotations;

namespace AzureTaskApi.Models;

public class CreateTaskRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }
}

public class UpdateTaskRequest
{
    [MaxLength(200)]
    public string? Title { get; set; }

    [MaxLength(2000)]
    public string? Description { get; set; }

    public TaskStatus? Status { get; set; }
}
