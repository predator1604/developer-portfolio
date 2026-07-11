using MediatR;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Features.Admin.Commands;
using Portfolio.Application.Features.Skills.Queries;

namespace Portfolio.API.Controllers;

// ── Skills ────────────────────────────────────────────────────────────────────

[ApiController]
[Route("api/[controller]")]
public sealed class SkillsController(ISender sender) : ControllerBase
{
    /// <summary>Get all visible skill groups (public — consumed by Angular).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<SkillGroupDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await sender.Send(new GetVisibleSkillsQuery(), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}

// ── AI Chat ───────────────────────────────────────────────────────────────────

[ApiController]
[Route("api/[controller]")]
public sealed class ChatController(ISender sender) : ControllerBase
{
    /// <summary>
    /// Send a message to the AI portfolio assistant.
    /// The session is identified by sessionId — generate a UUID on the Angular side
    /// and persist it in sessionStorage to maintain context across messages.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ChatResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Chat([FromBody] SendChatMessageCommand command, CancellationToken ct)
    {
        // Attach visitor IP for session tracking
        var cmd = command with
        {
            VisitorIp = HttpContext.Connection.RemoteIpAddress?.ToString()
        };

        var result = await sender.Send(cmd, ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : StatusCode(StatusCodes.Status503ServiceUnavailable,
                new { error = result.Error, code = result.ErrorCode });
    }
}
