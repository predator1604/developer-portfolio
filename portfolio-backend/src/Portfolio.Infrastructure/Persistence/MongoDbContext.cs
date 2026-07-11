using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Portfolio.Domain.Entities;

namespace Portfolio.Infrastructure.Persistence;

// ── Settings ─────────────────────────────────────────────────────────────────

public sealed class MongoDbSettings
{
    public string ConnectionString { get; init; } = string.Empty;

    public string DatabaseName { get; init; } = "portfolio_db";

    public string ContactMessages { get; init; } = "contact_messages";

    public string Projects { get; init; } = "projects";

    public string SkillGroups { get; init; } = "skill_groups";

    public string ChatSessions { get; init; } = "chat_sessions";
}

// ── Context ───────────────────────────────────────────────────────────────────

/// <summary>
/// Thin wrapper around IMongoDatabase.
/// Exposes typed IMongoCollection<T> for each entity.
/// </summary>
public sealed class MongoDbContext
{
    private readonly IMongoDatabase _db;

    public MongoDbContext(IMongoClient client, MongoDbSettings settings)
    {
        _db = client.GetDatabase(settings.DatabaseName);
        RegisterClassMaps();
        EnsureIndexes(settings);
    }

    public IMongoCollection<ContactMessage> ContactMessages => _db.GetCollection<ContactMessage>("contact_messages");

    public IMongoCollection<Project> Projects => _db.GetCollection<Project>("projects");

    public IMongoCollection<SkillGroup> SkillGroups => _db.GetCollection<SkillGroup>("skill_groups");

    public IMongoCollection<ChatSession> ChatSessions =>  _db.GetCollection<ChatSession>("chat_sessions");

    // ── BSON class maps ────────────────────────────────────────────────────
    private static void RegisterClassMaps()
    {
        if (BsonClassMap.IsClassMapRegistered(typeof(Domain.Common.BaseEntity)))
            return;

        // Map Id string to MongoDB _id
        BsonClassMap.RegisterClassMap<Domain.Common.BaseEntity>(cm =>
        {
            cm.AutoMap();
            cm.MapIdMember(c => c.Id)
              .SetSerializer(new StringSerializer(BsonType.ObjectId));
            cm.SetIgnoreExtraElements(true);
        });

        // Email value object serialised as plain string
        BsonClassMap.RegisterClassMap<Domain.ValueObjects.Email>(cm =>
        {
            cm.MapCreator(e => Domain.ValueObjects.Email.Create(e.Value));
            cm.MapMember(e => e.Value);
        });
    }

    // ── Indexes ────────────────────────────────────────────────────────────
    private void EnsureIndexes(MongoDbSettings settings)
    {
        // Projects: unique slug + sort order
        var projIdx = _db.GetCollection<Project>(settings.Projects);
        projIdx.Indexes.CreateMany([
            new CreateIndexModel<Project>(
                Builders<Project>.IndexKeys.Ascending(p => p.Slug),
                new CreateIndexOptions { Unique = true }),
            new CreateIndexModel<Project>(
                Builders<Project>.IndexKeys.Ascending(p => p.SortOrder)),
        ]);

        // ContactMessages: status + created
        var contactIdx = _db.GetCollection<ContactMessage>(settings.ContactMessages);
        contactIdx.Indexes.CreateMany([
            new CreateIndexModel<ContactMessage>(
                Builders<ContactMessage>.IndexKeys.Ascending(m => m.Status)),
            new CreateIndexModel<ContactMessage>(
                Builders<ContactMessage>.IndexKeys.Descending(m => m.CreatedAt)),
        ]);

        // ChatSessions: sessionId unique
        var chatIdx = _db.GetCollection<ChatSession>(settings.ChatSessions);
        chatIdx.Indexes.CreateOne(
            new CreateIndexModel<ChatSession>(
                Builders<ChatSession>.IndexKeys.Ascending(s => s.SessionId),
                new CreateIndexOptions { Unique = true }));
    }
}
