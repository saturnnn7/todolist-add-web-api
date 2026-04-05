using ToDoList.Api.DTOs.Todo;
using ToDoList.Api.Models;
using ToDoList.Api.Models.Enums;
using ToDoList.Api.Repositories.Interfaces;

using ToDoList.Api.Services.Interfaces;

namespace ToDoList.Api.Services;

public class TodoService : ITodoService
{
    private readonly ITodoRepository _todoRepository;
    public TodoService(ITodoRepository todoRepository) => _todoRepository = todoRepository;

    public async Task<PagedResponseDto<TodoResponseDto>> GetAllAsync(
        Guid userId,
        bool? isCompleted,
        Priority? priority,
        string? search,
        string sortBy,
        bool ascending,
        int page,
        int pageSize)
    {
        // LIMITING PAGE SIZE
        pageSize    = Math.Clamp(pageSize, 1, 50);
        page        = Math.Max(page, 1);

        var items = await _todoRepository.GetAllAsync(userId, isCompleted, priority, search, sortBy, ascending, page, pageSize);

        var total = await _todoRepository.CountAsync(userId, isCompleted, priority, search);
        
        return new PagedResponseDto<TodoResponseDto>
        {
            Items      = items.Select(MapToDto),
            TotalCount = total,
            Page       = page,
            PageSize   = pageSize
        };

    }

    public async Task<TodoResponseDto?> GetByIdAsync(Guid id, Guid userId)
    {
        var todo = await _todoRepository.GetByIdAsync(id, userId);
        return todo is null ? null : MapToDto(todo);
    }

    public async Task<TodoResponseDto> CreateAsync(Guid userId, CreateTodoDto dto)
    {
        var todo = new TodoItem
        {
            Title = dto.Title,
            Description = dto.Description,
            Priority = dto.Priority,
            DueDate = dto.DueDate,
            Tags = dto.Tags,
            UserId = userId
        };

        await _todoRepository.AddAsync(todo);
        await _todoRepository.SaveChangesAsync();

        return MapToDto(todo);
    }

    public async Task<TodoResponseDto?> UpdateAsync(Guid id, Guid userId, UpdateTodoDto dto)
    {
        var todo = await _todoRepository.GetByIdAsync(id, userId);
        if (todo is null) return null;

        todo.Title = dto.Title;
        todo.Description = dto.Description;
        todo.IsCompleted = dto.isCompleted;
        todo.Priority = dto.Priority;
        todo.DueDate = dto.DueDate;
        todo.Tags = dto.Tags;
        todo.UpdatetAt = DateTime.UtcNow;

        await _todoRepository.UpdateAsync(todo);
        await _todoRepository.SaveChangesAsync();

        return MapToDto(todo);
    }

    public async Task<TodoResponseDto?> ToggleCompleteAsync(Guid id, Guid userId)
    {
        var todo = await _todoRepository.GetByIdAsync(id, userId);
        if (todo is null) return null;

        todo.IsCompleted = !todo.IsCompleted;
        todo.UpdatetAt = DateTime.UtcNow;

        await _todoRepository.UpdateAsync(todo);
        await _todoRepository.SaveChangesAsync();

        return MapToDto(todo);
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId)
    {
        var todo = await _todoRepository.GetByIdAsync(id, userId);
        if (todo is null) return false;

        await _todoRepository.DeleteAsync(todo);
        await _todoRepository.SaveChangesAsync();

        return true;
    }

    // ----------------------------------

    private static TodoResponseDto MapToDto(TodoItem todo) => new()
    {
        Id          = todo.Id,
        Title       = todo.Title,
        Description = todo.Description,
        isCompleted = todo.IsCompleted,
        Priority    = todo.Priority,
        DueDate     = todo.DueDate,
        Tags        = todo.Tags,
        CreatedAt   = todo.CreatedAt,
        UpdatedAt   = todo.UpdatetAt
    };
}