using Portfolio.Domain.Common;
using Portfolio.Domain.Enums;
using Portfolio.Domain.Events;
using Portfolio.Domain.ValueObjects;

namespace Portfolio.Domain.Entities;

/// <summary>
/// Represents a contact message submitted through the portfolio contact form.
/// Encapsulates business rules around message state transitions.
/// </summary>
public sealed class ContactMessage : BaseEntity
{
    public string Name { get; private set; }

    public Email Email { get; private set; }

    public string Subject { get; private set; }

    public string Message { get; private set; }

    public MessageStatus Status { get; private set; }

    public string? ReplyNote { get; private set; }

    public DateTime? RepliedAt { get; private set; }


    private readonly List<IDomainEvent> _domainEvents = [];

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    // Required for MongoDB deserialization
    private ContactMessage() 
    {
        Name = string.Empty;
        Subject = string.Empty;
        Message = string.Empty;
        Email = Email.Create("placeholder@placeholder.com");
    }

    private ContactMessage(string name, Email email, string subject, string message)
    {
        Name = name;
        Email = email;
        Subject = subject;
        Message = message;
        Status = MessageStatus.New;
    }

    /// <summary>Factory method — enforces creation invariants.</summary>
    public static ContactMessage Create(string name, string email, string subject, string message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(subject);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        var entity = new ContactMessage(name, Email.Create(email), subject, message);
        entity._domainEvents.Add(new MessageReceivedEvent(entity.Id, name, email));
        return entity;
    }

    /// <summary>Mark the message as read without replying.</summary>
    public void MarkAsRead()
    {
        if (Status == MessageStatus.New)
        {
            Status = MessageStatus.Read;
            Touch();
        }
    }

    /// <summary>Attach an admin reply note and mark as replied.</summary>
    public void MarkAsReplied(string replyNote)
    {
        ArgumentException.ThrowINullOrWhiteSpace(replyNote);
        Status = MessageStatus.Replied;
        ReplyNote = replyNote;
        RepliedAt = DateTime.UtcNow;
        Touch();
    }

    public void ClearDomainEvents() => _domainEvents.Clear();
}
