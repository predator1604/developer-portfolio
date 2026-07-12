using MediatR;
using Microsoft.Extensions.Logging;
using Portfolio.Application.Common.Interfaces;
using Portfolio.Domain.Events;

namespace Portfolio.Application.Features.Contact.EventHandlers;

public sealed class MessageReceivedEventHandler(IEmailService emailService, ILogger<MessageReceivedEventHandler> logger) : INotificationHandler<MessageReceivedEvent>
{
    public async Task Handle(MessageReceivedEvent notification, CancellationToken ct)
    {
        try
        {
            await emailService.SendContactNotificationAsync(notification.SenderName, notification.SenderEmail, notification.Subject, notification.Message, ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to send owner notification for message {MessageId}.", notification.MessageId);
        }
    }
}