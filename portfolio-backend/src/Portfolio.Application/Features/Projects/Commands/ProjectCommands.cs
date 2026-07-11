using FluentValidation;
using MediatR;
using Portfolio.Application.Common;
using Portfolio.Application.Features.Projects.Queries;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Interfaces;

namespace Portfolio.Application.Features.Projects.Commands;

// ── Create ────────────────────────────────────────────────────────────────────

public sealed record CreateProjectCommand(
    string Title,

    string Slug,

    string Category,

    string Description,

    List<string> Stack,

    string AccentColor,

    string AccentBg,

    string Status,

    string StatusLabel,

    int SortOrder,

    string? Feature = null,

    string? LiveUrl = null,

    string? GitHubUrl = null) : IRequest<Result<ProjectDto>>;

public sealed class CreateProjectValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Slug).NotEmpty().Matches("^[a-z0-9-]+$").WithMessage("Slug must be lowercase with hyphens only.");
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.Stack).NotEmpty().WithMessage("At least one tech is required.");
        RuleFor(x => x.AccentColor).NotEmpty();
        RuleFor(x => x.Status).NotEmpty();
    }
}

public sealed class CreateProjectHandler(IProjectRepository repo) : IRequestHandler<CreateProjectCommand, Result<ProjectDto>>
{
    public async Task<Result<ProjectDto>> Handle(CreateProjectCommand cmd, CancellationToken ct)
    {
        var existing = await repo.GetBySlugAsync(cmd.Slug, ct);
        if (existing is not null)
            return Result<ProjectDto>.Failure($"A project with slug '{cmd.Slug}' already exists.", "DUPLICATE_SLUG");

        var project = Project.Create(
            cmd.Title, cmd.Slug, cmd.Category, cmd.Description,
            cmd.Stack, cmd.AccentColor, cmd.AccentBg,
            cmd.Status, cmd.StatusLabel, cmd.SortOrder,
            cmd.Feature, cmd.LiveUrl, cmd.GitHubUrl);

        await repo.AddAsync(project, ct);
        return GetVisibleProjectsHandler.ToDto(project);
    }
}

// ── Update ────────────────────────────────────────────────────────────────────

public sealed record UpdateProjectCommand(
    string Id,
    string Title,
    string Category,
    string Description,
    List<string> Stack,
    string Status,
    string StatusLabel,
    bool IsVisible,
    string? Feature = null,
    string? LiveUrl = null,
    string? GitHubUrl = null) : IRequest<Result<ProjectDto>>;

public sealed class UpdateProjectHandler(IProjectRepository repo) : IRequestHandler<UpdateProjectCommand, Result<ProjectDto>>
{
    public async Task<Result<ProjectDto>> Handle(UpdateProjectCommand cmd, CancellationToken ct)
    {
        var project = await repo.GetByIdAsync(cmd.Id, ct);
        if (project is null)
            return Result<ProjectDto>.Failure("Project not found.", "NOT_FOUND");

        project.Update(cmd.Title, cmd.Category, cmd.Description,
            cmd.Stack, cmd.Status, cmd.StatusLabel,
            cmd.IsVisible, cmd.Feature, cmd.LiveUrl, cmd.GitHubUrl);

        await repo.UpdateAsync(project, ct);
        return GetVisibleProjectsHandler.ToDto(project);
    }
}

// ── Delete ────────────────────────────────────────────────────────────────────

public sealed record DeleteProjectCommand(string Id) : IRequest<Result>;

public sealed class DeleteProjectHandler(IProjectRepository repo) : IRequestHandler<DeleteProjectCommand, Result>
{
    public async Task<Result> Handle(DeleteProjectCommand cmd, CancellationToken ct)
    {
        var project = await repo.GetByIdAsync(cmd.Id, ct);
        if (project is null)
            return Result.Failure("Project not found.", "NOT_FOUND");

        await repo.DeleteAsync(cmd.Id, ct);
        return Result.Success();
    }
}
