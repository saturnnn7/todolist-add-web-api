using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Api.DTOs.Auth;
using ToDoList.Api.Services.Interfaces;

using ToDoList.Api.DTOs.Common;

namespace ToDoList.Api.Controllers;

[ApiController]
[Route("api/auth")]
[AllowAnonymous]
[Produces("application/json")]
public class AuthController : BaseController
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService) => _authService = authService;

    // REGISTRATION
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        try
        {
            var result = await _authService.RegisterAsync(dto);
            return CreatedResponse(nameof(Register), null!, result);
        }
        catch (InvalidOperationException ex)
        {
            return ConflictResponse(ex.Message);
        }
    }

    // LOGIN
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        try
        {
            var result = await _authService.LoginAsync(dto);
            return OkResponse(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return UnauthorizedResponse(ex.Message);
        }
    }
}