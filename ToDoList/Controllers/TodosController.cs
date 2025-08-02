using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoList.DTOs;
using ToDoList.Models;
using ToDoList.Services;

namespace ToDoList.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TodosController : ControllerBase
{
    private readonly ITodoService _todoService;

    public TodosController(ITodoService todoService)
    {
        _todoService = todoService;
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim ?? throw new UnauthorizedAccessException());
    }

    /// <summary>
    /// Create a new todo item
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TodoItemDto>> CreateTodo([FromBody] CreateTodoItemDto createDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userId = GetUserId();
        var todoItem = await _todoService.CreateTodoAsync(userId, createDto);

        return CreatedAtAction(nameof(GetTodoById), new { id = todoItem.Id }, todoItem);
    }

    /// <summary>
    /// Get all user's todo items with pagination and optional status filter
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<TodoItemsResponseDto>> GetTodos(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] TodoStatus? status = null)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        var userId = GetUserId();
        var response = await _todoService.GetTodosAsync(userId, page, pageSize, status);

        return Ok(response);
    }

    /// <summary>
    /// Get a specific todo item by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<TodoItemDto>> GetTodoById(int id)
    {
        var userId = GetUserId();
        var todoItem = await _todoService.GetTodoByIdAsync(userId, id);

        if (todoItem == null)
        {
            return NotFound(new { message = "Todo item not found" });
        }

        return Ok(todoItem);
    }

    /// <summary>
    /// Update a todo item
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<TodoItemDto>> UpdateTodo(int id, [FromBody] UpdateTodoItemDto updateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userId = GetUserId();
        var todoItem = await _todoService.UpdateTodoAsync(userId, id, updateDto);

        if (todoItem == null)
        {
            return NotFound(new { message = "Todo item not found" });
        }

        return Ok(todoItem);
    }

    /// <summary>
    /// Update only the status of a todo item
    /// </summary>
    [HttpPatch("{id}/status")]
    public async Task<ActionResult<TodoItemDto>> UpdateTodoStatus(int id, [FromBody] TodoStatus status)
    {
        var userId = GetUserId();
        var todoItem = await _todoService.UpdateTodoStatusAsync(userId, id, status);

        if (todoItem == null)
        {
            return NotFound(new { message = "Todo item not found" });
        }

        return Ok(todoItem);
    }

    /// <summary>
    /// Delete a todo item
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTodo(int id)
    {
        var userId = GetUserId();
        var success = await _todoService.DeleteTodoAsync(userId, id);

        if (!success)
        {
            return NotFound(new { message = "Todo item not found" });
        }

        return NoContent();
    }

    /// <summary>
    /// Get user's todo statistics
    /// </summary>
    [HttpGet("stats")]
    public async Task<ActionResult> GetStats()
    {
        var userId = GetUserId();

        var pendentes = await _todoService.GetTodosAsync(userId, 1, int.MaxValue, TodoStatus.Pendente);
        var fazendo = await _todoService.GetTodosAsync(userId, 1, int.MaxValue, TodoStatus.Fazendo);
        var concluidas = await _todoService.GetTodosAsync(userId, 1, int.MaxValue, TodoStatus.Concluido);

        return Ok(new
        {
            total = pendentes.TotalCount + fazendo.TotalCount + concluidas.TotalCount,
            pendentes = pendentes.TotalCount,
            fazendo = fazendo.TotalCount,
            concluidas = concluidas.TotalCount
        });
    }
}
