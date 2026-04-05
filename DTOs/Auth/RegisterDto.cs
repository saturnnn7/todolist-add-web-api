using System.ComponentModel.DataAnnotations;

namespace ToDoList.Api.DTOs.Auth;

public class RegisterDto
{
    [Required]
    [MaxLength(50)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    [MaxLength(50)]
    public string Password { get; set; } = string.Empty;
}