using ToDoList.Api.Models;
using ToDoList.Api.Models.Enums;

namespace ToDoList.Api.Repositories.Interfaces;

public interface ITodoRepository
{
    Task<TodoItem?> GetByIdAsync(Guid id, Guid userId); 
    Task<IEnumerable<TodoItem>> GetAllAsync(
        Guid userId,
        bool? isCompleted,
        Priority? priority,
        string? search,
        string sortBy,
        bool ascending,
        int page,
        int pageSize);
    Task<int> CountAsync(Guid userId, bool? isCompleted, Priority? priority, string? search);

    Task AddAsync(TodoItem todo);
    Task UpdateAsync(TodoItem todo);
    Task DeleteAsync(TodoItem todo);
    Task SaveChangesAsync();
}