using MongoDB.Driver;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Enums;
using Portfolio.Domain.Interfaces;
using Portfolio.Infrastructure.Persistence;

namespace Portfolio.Infrastructure.Persistence.Repositories;

// ── Contact ───────────────────────────────────────────────────────────────────

public sealed class ContactRepository(MongoDbContext ctx) : IContactRepository
{
    private readonly IMongoCollection<ContactMessage> _col = ctx.ContactMessages;

    public async Task<ContactMessage> AddAsync(ContactMessage msg, CancellationToken ct = default)
    {
        await _col.InsertOneAsync(msg, cancellationToken: ct);
        return msg;
    }

    public async Task<ContactMessage?> GetByIdAsync(string id, CancellationToken ct = default) => await _col.Find(m => m.Id == id).FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<ContactMessage>> GetAllAsync(int page, int pageSize, CancellationToken ct = default)
    {
        return await _col.Find(_ => true)
            .SortByDescending(m => m.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<ContactMessage>> GetByStatusAsync(MessageStatus status, CancellationToken ct = default) => 
        await _col.Find(m => m.Status == status)
                  .SortByDescending(m => m.CreatedAt)
                  .ToListAsync(ct);

    public async Task UpdateAsync(ContactMessage msg, CancellationToken ct = default) => await _col.ReplaceOneAsync(m => m.Id == msg.Id, msg, cancellationToken: ct);

    public async Task<long> CountAsync(CancellationToken ct = default) => await _col.CountDocumentsAsync(_ => true, cancellationToken: ct);
}

// ── Project ───────────────────────────────────────────────────────────────────

public sealed class ProjectRepository(MongoDbContext ctx) : IProjectRepository
{
    private readonly IMongoCollection<Project> _col = ctx.Projects;

    public async Task<IReadOnlyList<Project>> GetAllVisibleAsync(CancellationToken ct = default) =>
        await _col.Find(p => p.IsVisible)
                  .SortBy(p => p.SortOrder)
                  .ToListAsync(ct);

    public async Task<IReadOnlyList<Project>> GetAllAsync(CancellationToken ct = default) =>
        await _col.Find(_ => true)
                  .SortBy(p => p.SortOrder)
                  .ToListAsync(ct);

    public async Task<Project?> GetBySlugAsync(string slug, CancellationToken ct = default) =>
        await _col.Find(p => p.Slug == slug.ToLowerInvariant())
                  .FirstOrDefaultAsync(ct);

    public async Task<Project?> GetByIdAsync(string id, CancellationToken ct = default) =>
        await _col.Find(p => p.Id == id).FirstOrDefaultAsync(ct);

    public async Task<Project> AddAsync(Project project, CancellationToken ct = default)
    {
        await _col.InsertOneAsync(project, cancellationToken: ct);
        return project;
    }

    public async Task UpdateAsync(Project project, CancellationToken ct = default) =>
        await _col.ReplaceOneAsync(p => p.Id == project.Id, project, cancellationToken: ct);

    public async Task DeleteAsync(string id, CancellationToken ct = default) =>
        await _col.DeleteOneAsync(p => p.Id == id, ct);
}

// ── Skill ─────────────────────────────────────────────────────────────────────

public sealed class SkillRepository(MongoDbContext ctx) : ISkillRepository
{
    private readonly IMongoCollection<SkillGroup> _col = ctx.SkillGroups;

    public async Task<IReadOnlyList<SkillGroup>> GetAllVisibleAsync(CancellationToken ct = default) =>
        await _col.Find(g => g.IsVisible)
                  .SortBy(g => g.SortOrder)
                  .ToListAsync(ct);

    public async Task<IReadOnlyList<SkillGroup>> GetAllAsync(CancellationToken ct = default) =>
        await _col.Find(_ => true)
                  .SortBy(g => g.SortOrder)
                  .ToListAsync(ct);

    public async Task<SkillGroup?> GetByGroupIdAsync(string groupId, CancellationToken ct = default) =>
        await _col.Find(g => g.GroupId == groupId.ToLowerInvariant())
                  .FirstOrDefaultAsync(ct);

    public async Task<SkillGroup> AddAsync(SkillGroup group, CancellationToken ct = default)
    {
        await _col.InsertOneAsync(group, cancellationToken: ct);
        return group;
    }

    public async Task UpdateAsync(SkillGroup group, CancellationToken ct = default) =>
        await _col.ReplaceOneAsync(g => g.Id == group.Id, group, cancellationToken: ct);

    public async Task DeleteAsync(string id, CancellationToken ct = default) =>
        await _col.DeleteOneAsync(g => g.Id == id, ct);
}

// ── Chat Session ──────────────────────────────────────────────────────────────

public sealed class ChatSessionRepository(MongoDbContext ctx) : IChatSessionRepository
{
    private readonly IMongoCollection<ChatSession> _col = ctx.ChatSessions;

    public async Task<ChatSession?> GetBySessionIdAsync(string sessionId, CancellationToken ct = default) =>
        await _col.Find(s => s.SessionId == sessionId).FirstOrDefaultAsync(ct);

    public async Task<ChatSession> AddAsync(ChatSession session, CancellationToken ct = default)
    {
        await _col.InsertOneAsync(session, cancellationToken: ct);
        return session;
    }

    public async Task UpdateAsync(ChatSession session, CancellationToken ct = default) =>
        await _col.ReplaceOneAsync(s => s.SessionId == session.SessionId, session, cancellationToken: ct);
}
