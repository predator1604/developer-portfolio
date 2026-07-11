using MediatR;
using Portfolio.Application.Common;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Interfaces;

namespace Portfolio.Application.Features.Skills.Queries;

// ── DTOs ─────────────────────────────────────────────────────────────────────

public sealed record SkillDto(string Name, int Level);

public sealed record SkillGroupDto(
    string Id,
    string GroupId,
    string Title,
    string Category,
    string AccentColor,
    string AccentBg,
    string IconPath,
    List<SkillDto> Skills,
    List<string>   Tools,
    int SortOrder);

// ── Get visible skill groups (public) ────────────────────────────────────────

public sealed record GetVisibleSkillsQuery : IRequest<Result<List<SkillGroupDto>>>;

public sealed class GetVisibleSkillsHandler(ISkillRepository repo) : IRequestHandler<GetVisibleSkillsQuery, Result<List<SkillGroupDto>>>
{
    public async Task<Result<List<SkillGroupDto>>> Handle(GetVisibleSkillsQuery _, CancellationToken ct)
    {
        var groups = await repo.GetAllVisibleAsync(ct);
        return groups.OrderBy(g => g.SortOrder).Select(ToDto).ToList();
    }

    internal static SkillGroupDto ToDto(SkillGroup g) => new(
        g.Id,
        g.GroupId,
        g.Title,
        g.Category,
        g.AccentColor,
        g.AccentBg,
        g.IconPath,
        g.Skills.Select(s => new SkillDto(s.Name, s.Level)).ToList(),
        g.Tools,
        g.SortOrder);
}

// ── Get all skill groups (admin) ──────────────────────────────────────────────

public sealed record GetAllSkillsQuery : IRequest<Result<List<SkillGroupDto>>>;

public sealed class GetAllSkillsHandler(ISkillRepository repo): IRequestHandler<GetAllSkillsQuery, Result<List<SkillGroupDto>>>
{
    public async Task<Result<List<SkillGroupDto>>> Handle(GetAllSkillsQuery _, CancellationToken ct)
    {
        var groups = await repo.GetAllAsync(ct);
        return groups.OrderBy(g => g.SortOrder)
                     .Select(GetVisibleSkillsHandler.ToDto)
                     .ToList();
    }
}
