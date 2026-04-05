using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Api.DTOs.Common;

namespace ToDoList.Api.Controllers;

[ApiController]
public abstract class BaseController : ControllerBase
{
    protected Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    protected IActionResult OkResponse<T>(T data) => Ok(ApiResponse<T>.Ok(data));

    protected IActionResult CreatedResponse<T>(string actionName, object routeValues, T data) => CreatedAtAction(actionName, routeValues, ApiResponse<T>.Ok(data));

    protected IActionResult NoContentResponse() => NoContent();

    protected IActionResult NotFoundResponse(string message = "Resource not found.") => NotFound(ApiResponse<object>.Fail(ErrorCodes.NotFound, message));

    protected IActionResult ConflictResponse(string message) =>
        Conflict(ApiResponse<object>.Fail(ErrorCodes.Conflict, message));

    protected IActionResult UnauthorizedResponse(string message = "Invalid credentials.") =>
        Unauthorized(ApiResponse<object>.Fail(ErrorCodes.Unauthorized, message));

    protected IActionResult ValidationResponse(Dictionary<string, string[]> details) =>
        BadRequest(ApiResponse<object>.Fail(
            ErrorCodes.ValidationError,
            "One or more validation errors occurred.",
            details));
}