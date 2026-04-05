using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Tree;
using ToDoList.Api.DTOs.Todo;
using ToDoList.Api.Models.Enums;
using ToDoList.Api.Services.Interfaces;

namespace ToDoList.Api.Controllers;

[ApiController]
[Route("api/todos")]
[Authorize]
public class TodoController : ControllerBase
{
    private readonly ITodoService _todoService;
    public TodoController(ITodoService todoService) => _todoService = todoService;

    // ----------------------------------------------
    // UserId from JWT
    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // ----------------------------------------------

    // Get Todos List with filters, sorting, pagination
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponseDto<TodoResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] bool?        IsCompleted = null,
        [FromQuery] Priority?    priority    = null,
        [FromQuery] string?      search      = null,
        [FromQuery] string       sortBy      = "createdat",
        [FromQuery] bool         ascending   = true,
        [FromQuery] int          page        = 1,
        [FromQuery] int          pageSize    = 10)
    {
        var result = await _todoService.GetAllAsync(
            GetUserId(),
            IsCompleted,
            priority,
            search,
            sortBy,
            ascending,
            page,
            pageSize);
        
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PagedResponseDto<TodoResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var result = _todoService.GetByIdAsync(id, GetUserId());

        return result is null
            ? NotFound(new { message = "Todo not found." })
            : Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TodoResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateTodoDto dto)
    {
        var result = await _todoService.CreateAsync(GetUserId(), dto);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(TodoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateTodoDto dto)
    {
        var result = await _todoService.UpdateAsync(id, GetUserId(), dto);

        return result is null
            ? NotFound(new { message = "Todo not found."})
            : Ok(result);
    }

    [HttpPatch("{id:guid}/toggle")]
    [ProducesResponseType(typeof(TodoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Toggle([FromRoute] Guid id)
    {
        var deleted = await _todoService.DeleteAsync(id, GetUserId());

        return deleted
            ? NoContent()
            : NotFound(new { message = "Todo not found." });
    }
}

