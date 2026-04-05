using ToDoList.Api.DTOs.Todo;
using ToDoList.Api.Models.Enums;

namespace ToDoList.Api.Services.Interfaces;

public interface ITodoService
{
    Task<PagedResponseDto<TodoResponseDto>> GetAllAsync(
        Guid userId,
        bool? isCompleted,
        Priority? priority,
        string? search,
        string sortBy,
        bool ascending,
        int page,
        int pageSize);

    Task<TodoResponseDto?> GetByIdAsync(Guid id, Guid userId);
    Task<TodoResponseDto> CreateAsync(Guid userId, CreateTodoDto dto);
    Task<TodoResponseDto?> UpdateAsync(Guid id, Guid userId, UpdateTodoDto dto);
    Task<TodoResponseDto?> ToggleCompleteAsync(Guid id, Guid userId);
    Task<bool> DeleteAsync(Guid id, Guid userId);
}