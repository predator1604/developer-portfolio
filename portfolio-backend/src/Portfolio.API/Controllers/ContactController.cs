using MediatR;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Features.Contact.Commands;
using Portfolio.Application.Features.Contact.Queries;

namespace Portfolio.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ContactController(ISender sender) : ControllerBase
{
    /// <summary>Submit a contact form message (public).</summary>
    [HttpPost]
    [ProducesResponseType(typeof(SendMessageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Send([FromBody] SendMessageCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { error = result.Error, code = result.ErrorCode });
    }

    /// <summary>Get paginated list of all messages (admin).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ContactMessageDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var result = await sender.Send(new GetMessagesQuery(page, pageSize), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    /// <summary>Get a single message by ID (admin).</summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ContactMessageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(string id, CancellationToken ct)
    {
        var result = await sender.Send(new GetMessageByIdQuery(id), ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : result.ErrorCode == "NOT_FOUND"
                ? NotFound(result.Error)
                : BadRequest(result.Error);
    }
}
