using Portfolio.Domain.Common;
using Portfolio.Domain.Enums;

namespace Portfolio.Domain.Entities;

/// <summary>
/// Represents a visitor's AI chat session with the portfolio assistant.
/// Each session stores the full message history for context continuity.
/// </summary>
public sealed class ChatSession : BaseEntity
{
    public string           SessionId { get; private set; }
    public string?          VisitorIp { get; private set; }
    public List<ChatMessage> Messages  { get; private set; }
    public DateTime?        EndedAt   { get; private set; }

    private ChatSession()
    {
        SessionId = string.Empty;
        Messages  = [];
    }

    public static ChatSession Create(string sessionId, string? visitorIp = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sessionId);
        return new ChatSession
        {
            SessionId = sessionId,
            VisitorIp = visitorIp,
            Messages  = [],
        };
    }

    public void AddMessage(ChatMessageRole role, string content)
    {
        Messages.Add(new ChatMessage
        {
            Role      = role,
            Content   = content,
            Timestamp = DateTime.UtcNow,
        });
        Touch();
    }

    public void End() { EndedAt = DateTime.UtcNow; Touch(); }
}

/// <summary>Embedded chat message within a ChatSession document.</summary>
public sealed class ChatMessage
{
    public ChatMessageRole Role      { get; set; }
    public string          Content   { get; set; } = string.Empty;
    public DateTime        Timestamp { get; set; }
}
