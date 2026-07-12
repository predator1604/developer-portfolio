using Portfolio.Domain.Common;

namespace Portfolio.Domain.Events;

/// <summary>
/// Raised when a new contact message is received.
/// Handled by Infrastructure to send confirmation email.
/// </summary>
public sealed record MessageReceivedEvent(string MessageId, string SenderName, string SenderEmail, string Subject, string Message) : IDomainEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
