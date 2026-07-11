using FluentValidation;
using MediatR;
using Portfolio.Application.Common;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Interfaces;

namespace Portfolio.Application.Features.Projects.Queries;

// ── DTOs ─────────────────────────────────────────────────────────────────────

public sealed record ProjectDto(
    string Id,
    string Title,
    string Slug,
    string Category,
    string Description,
    string? Feature,
    List<string>  Stack,
    string AccentColor,
    string AccentBg,
    string Status,
    string StatusLabel,
    string? LiveUrl,
    string? GitHubUrl,
    int SortOrder);

// ── Get all visible projects (public endpoint) ────────────────────────────────

public sealed record GetVisibleProjectsQuery : IRequest<Result<List<ProjectDto>>>;

public sealed class GetVisibleProjectsHandler(IProjectRepository repo) : IRequestHandler<GetVisibleProjectsQuery, Result<List<ProjectDto>>>
{
    public async Task<Result<List<ProjectDto>>> Handle(GetVisibleProjectsQuery _, CancellationToken ct)
    {
        var projects = await repo.GetAllVisibleAsync(ct);
        return projects.Select(ToDto).ToList();
    }

    internal static ProjectDto ToDto(Project p) => new(
        p.Id, p.Title, p.Slug, p.Category, p.Description,
        p.Feature, p.Stack, p.AccentColor, p.AccentBg,
        p.Status, p.StatusLabel, p.LiveUrl, p.GitHubUrl, p.SortOrder);
}

// ── Get project by slug ───────────────────────────────────────────────────────

public sealed record GetProjectBySlugQuery(string Slug) : IRequest<Result<ProjectDto>>;

public sealed class GetProjectBySlugHandler(IProjectRepository repo) : IRequestHandler<GetProjectBySlugQuery, Result<ProjectDto>>
{
    public async Task<Result<ProjectDto>> Handle(GetProjectBySlugQuery query, CancellationToken ct)
    {
        var project = await repo.GetBySlugAsync(query.Slug, ct);
        if (project is null)
            return Result<ProjectDto>.Failure("Project not found.", "NOT_FOUND");

        return GetVisibleProjectsHandler.ToDto(project);
    }
}
