using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using ToDoList.Api.Data;
using ToDoList.Api.Models;
using ToDoList.Api.Models.Enums;
using ToDoList.Api.Repositories.Interfaces;

namespace ToDoList.Api.Repositories;

public class TodoRepository : ITodoRepository
{
    private readonly AppDbContext _context;
    public TodoRepository(AppDbContext context) => _context = context;


    public async Task<TodoItem?> GetByIdAsync(Guid id, Guid userId)
    => await _context.TodoItems
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

    public async Task<IEnumerable<TodoItem>> GetAllAsync(
        Guid userId,
        bool? isCompleted,
        Priority? priority,
        string? search,
        string sortBy,
        bool ascending,
        int page,
        int pageSize)
    {
        // FILTERS
        var query = _context.TodoItems
                .Where(t => t.UserId == userId);
        
        if (isCompleted.HasValue)
                query = query.Where(t => t.IsCompleted == isCompleted);
        
        if (priority.HasValue)
                query = query.Where(t => t.Priority == priority.Value);
        
        if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(t =>
                t.Title.ToLower().Contains(search.ToLower()) ||
                (t.Description != null && t.Description.ToLower().Contains(search.ToLower()))); 
        
        // SORTING
        query = sortBy.ToLower() switch
        {
            "title"     => ascending ? query.OrderBy(t => t.Title)
                                     : query.OrderByDescending(t => t.Title),
            "duedate"   => ascending ? query.OrderBy(t => t.DueDate)
                                     : query.OrderByDescending(t => t.DueDate),
            "priority"  => ascending ? query.OrderBy(t => t.Priority)
                                     : query.OrderByDescending(t => t.Priority),
            "updatedat" => ascending ? query.OrderBy(t => t.UpdatetAt)
                                     : query.OrderByDescending(t => t.UpdatetAt),
            _           => ascending ? query.OrderBy(t => t.CreatedAt)
                                     : query.OrderByDescending(t => t.CreatedAt)
        };

        return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
    }

    public async Task<int> CountAsync(
        Guid userId, 
        bool? isCompleted, 
        Priority? priority, 
        string? search)
    {
        var query = _context.TodoItems
                .Where(t => t.UserId == userId);
        
        if (isCompleted.HasValue)
            query = query.Where(t => t.IsCompleted == isCompleted.Value);
        
        if (priority.HasValue)
            query = query.Where(t => t.Priority == priority.Value);
        
        if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(t =>
                t.Title.ToLower().Contains(search.ToLower()) ||
                (t.Description != null && t.Description.ToLower().Contains(search.ToLower())));
        
        return await query.CountAsync();
    }

    public async Task AddAsync(TodoItem todo)
    => await _context.TodoItems.AddAsync(todo);

    public async Task UpdateAsync(TodoItem todo)
    => _context.TodoItems.Update(todo);

    public async Task DeleteAsync(TodoItem todo)
    => _context.TodoItems.Remove(todo);

    public async Task SaveChangesAsync()
    => await _context.SaveChangesAsync();
}