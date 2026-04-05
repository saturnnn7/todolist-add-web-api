using System.ComponentModel.DataAnnotations;
using ToDoList.Api.Models.Enums;

namespace ToDoList.Api.Models;
public class TodoItem
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }
    public bool IsCompleted { get; set; } = false;
    public Priority Priority { get; set; }
    public DateTime? DueDate { get; set; }

    // Хранится как строка через запятую: "work,personal,urgent"
    public string? Tags { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatetAt { get; set; } = DateTime.UtcNow;

    // FK
    public Guid UserId { get; set; }

    // Navigation property
    public User User { get; set; } = null!;
}