using Microsoft.EntityFrameworkCore;
using ToDoList.Data;
using ToDoList.DTOs;
using ToDoList.Models;

namespace ToDoList.Services;

public interface ITodoService
{
    Task<TodoItemDto> CreateTodoAsync(int userId, CreateTodoItemDto createDto);
    Task<TodoItemsResponseDto> GetTodosAsync(int userId, int page = 1, int pageSize = 10, TodoStatus? status = null);
    Task<TodoItemDto?> GetTodoByIdAsync(int userId, int todoId);
    Task<TodoItemDto?> UpdateTodoAsync(int userId, int todoId, UpdateTodoItemDto updateDto);
    Task<bool> DeleteTodoAsync(int userId, int todoId);
    Task<TodoItemDto?> UpdateTodoStatusAsync(int userId, int todoId, TodoStatus status);
}

public class TodoService : ITodoService
{
    private readonly TodoDbContext _context;

    public TodoService(TodoDbContext context)
    {
        _context = context;
    }

    public async Task<TodoItemDto> CreateTodoAsync(int userId, CreateTodoItemDto createDto)
    {
        var todoItem = new TodoItem
        {
            Title = createDto.Title,
            Description = createDto.Description,
            DueDate = createDto.DueDate,
            UserId = userId,
            Status = TodoStatus.Pendente,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.TodoItems.Add(todoItem);
        await _context.SaveChangesAsync();

        return MapToDto(todoItem);
    }

    public async Task<TodoItemsResponseDto> GetTodosAsync(int userId, int page = 1, int pageSize = 10, TodoStatus? status = null)
    {
        var query = _context.TodoItems.Where(t => t.UserId == userId);

        if (status.HasValue)
        {
            query = query.Where(t => t.Status == status.Value);
        }

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        var items = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => MapToDto(t))
            .ToListAsync();

        return new TodoItemsResponseDto
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages
        };
    }

    public async Task<TodoItemDto?> GetTodoByIdAsync(int userId, int todoId)
    {
        var todoItem = await _context.TodoItems
            .FirstOrDefaultAsync(t => t.Id == todoId && t.UserId == userId);

        return todoItem != null ? MapToDto(todoItem) : null;
    }

    public async Task<TodoItemDto?> UpdateTodoAsync(int userId, int todoId, UpdateTodoItemDto updateDto)
    {
        var todoItem = await _context.TodoItems
            .FirstOrDefaultAsync(t => t.Id == todoId && t.UserId == userId);

        if (todoItem == null) return null;

        if (!string.IsNullOrEmpty(updateDto.Title))
            todoItem.Title = updateDto.Title;

        if (updateDto.Description != null)
            todoItem.Description = updateDto.Description;

        if (updateDto.Status.HasValue)
            todoItem.Status = updateDto.Status.Value;

        if (updateDto.DueDate.HasValue)
            todoItem.DueDate = updateDto.DueDate.Value;

        todoItem.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return MapToDto(todoItem);
    }

    public async Task<bool> DeleteTodoAsync(int userId, int todoId)
    {
        var todoItem = await _context.TodoItems
            .FirstOrDefaultAsync(t => t.Id == todoId && t.UserId == userId);

        if (todoItem == null) return false;

        _context.TodoItems.Remove(todoItem);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<TodoItemDto?> UpdateTodoStatusAsync(int userId, int todoId, TodoStatus status)
    {
        var todoItem = await _context.TodoItems
            .FirstOrDefaultAsync(t => t.Id == todoId && t.UserId == userId);

        if (todoItem == null) return null;

        todoItem.Status = status;
        todoItem.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return MapToDto(todoItem);
    }

    private static TodoItemDto MapToDto(TodoItem todoItem)
    {
        return new TodoItemDto
        {
            Id = todoItem.Id,
            Title = todoItem.Title,
            Description = todoItem.Description,
            Status = todoItem.Status,
            CreatedAt = todoItem.CreatedAt,
            UpdatedAt = todoItem.UpdatedAt,
            DueDate = todoItem.DueDate,
            UserId = todoItem.UserId
        };
    }
}
