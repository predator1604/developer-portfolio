using Portfolio.Domain.Entities;
using Portfolio.Domain.Enums;

namespace Portfolio.Domain.Interfaces;

// ── Contact ───────────────────────────────────────────────────────────────────

public interface IContactRepository
{
    Task<ContactMessage> AddAsync(ContactMessage message, CancellationToken ct = default);
    Task<ContactMessage?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<IReadOnlyList<ContactMessage>> GetAllAsync(int page, int pageSize, CancellationToken ct = default);
    Task<IReadOnlyList<ContactMessage>> GetByStatusAsync(MessageStatus status, CancellationToken ct = default);
    Task UpdateAsync(ContactMessage message, CancellationToken ct = default);
    Task<long> CountAsync(CancellationToken ct = default);
}

// ── Projects ──────────────────────────────────────────────────────────────────

public interface IProjectRepository
{
    Task<IReadOnlyList<Project>> GetAllVisibleAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Project>> GetAllAsync(CancellationToken ct = default);
    Task<Project?> GetBySlugAsync(string slug, CancellationToken ct = default);
    Task<Project?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<Project> AddAsync(Project project, CancellationToken ct = default);
    Task UpdateAsync(Project project, CancellationToken ct = default);
    Task DeleteAsync(string id, CancellationToken ct = default);
}

// ── Skills ────────────────────────────────────────────────────────────────────

public interface ISkillRepository
{
    Task<IReadOnlyList<SkillGroup>> GetAllVisibleAsync(CancellationToken ct = default);
    Task<IReadOnlyList<SkillGroup>> GetAllAsync(CancellationToken ct = default);
    Task<SkillGroup?> GetByGroupIdAsync(string groupId, CancellationToken ct = default);
    Task<SkillGroup> AddAsync(SkillGroup group, CancellationToken ct = default);
    Task UpdateAsync(SkillGroup group, CancellationToken ct = default);
    Task DeleteAsync(string id, CancellationToken ct = default);
}

// ── Chat ──────────────────────────────────────────────────────────────────────

public interface IChatSessionRepository
{
    Task<ChatSession?> GetBySessionIdAsync(string sessionId, CancellationToken ct = default);
    Task<ChatSession> AddAsync(ChatSession session, CancellationToken ct = default);
    Task UpdateAsync(ChatSession session, CancellationToken ct = default);
}
