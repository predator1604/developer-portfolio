using MediatR;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Features.Projects.Commands;
using Portfolio.Application.Features.Projects.Queries;

namespace Portfolio.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ProjectsController(ISender sender) : ControllerBase
{
    /// <summary>Get all visible projects (public — consumed by Angular).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<ProjectDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await sender.Send(new GetVisibleProjectsQuery(), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    /// <summary>Get a project by slug (public).</summary>
    [HttpGet("{slug}")]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBySlug(string slug, CancellationToken ct)
    {
        var result = await sender.Send(new GetProjectBySlugQuery(slug), ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : result.ErrorCode == "NOT_FOUND" ? NotFound() : BadRequest(result.Error);
    }

    /// <summary>Create a new project (admin).</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateProjectCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetBySlug), new { slug = result.Value!.Slug }, result.Value)
            : BadRequest(new { error = result.Error, code = result.ErrorCode });
    }

    /// <summary>Update an existing project (admin).</summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateProjectCommand command, CancellationToken ct)
    {
        // Ensure route id matches body id
        var cmd = command with { Id = id };
        var result = await sender.Send(cmd, ct);
        return result.IsSuccess
            ? Ok(result.Value)
            : result.ErrorCode == "NOT_FOUND" ? NotFound() : BadRequest(result.Error);
    }

    /// <summary>Delete a project (admin).</summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id, CancellationToken ct)
    {
        var result = await sender.Send(new DeleteProjectCommand(id), ct);
        return result.IsSuccess
            ? NoContent()
            : result.ErrorCode == "NOT_FOUND" ? NotFound() : BadRequest(result.Error);
    }
}
