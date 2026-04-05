using System.ComponentModel.DataAnnotations;
using ToDoList.Api.Models.Enums;

namespace ToDoList.Api.DTOs.Todo;

public class CreateTodoDto
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    public Priority Priority { get; set; } = Priority.Medium;

    public DateTime? DueDate { get; set; }

    [MaxLength(500)]
    public string? Tags { get; set; }
}