using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Portfolio.Application.Common;
using Portfolio.Application.Common.Interfaces;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Interfaces;

namespace Portfolio.Application.Features.Contact.Commands;

// ── Command ───────────────────────────────────────────────────────────────────

public sealed record SendMessageCommand(string Name, string Email, string Subject, string Message) : IRequest<Result<SendMessageResponse>>;

public sealed record SendMessageResponse(string MessageId, string ConfirmationText);

// ── Validator ─────────────────────────────────────────────────────────────────

public sealed class SendMessageValidator : AbstractValidator<SendMessageCommand>
{
    public SendMessageValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MinimumLength(2).WithMessage("Name must be at least 2 characters.")
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.");

        RuleFor(x => x.Subject)
            .NotEmpty().WithMessage("Subject is required.")
            .MinimumLength(3).WithMessage("Subject must be at least 3 characters.")
            .MaximumLength(200);

        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Message is required.")
            .MinimumLength(10).WithMessage("Message must be at least 10 characters.")
            .MaximumLength(500).WithMessage("Message cannot exceed 500 characters.");
    }
}

// ── Handler ───────────────────────────────────────────────────────────────────

public sealed class SendMessageHandler(IContactRepository  contactRepo, IEmailService emailService, IPublisher publisher, ILogger<SendMessageHandler> logger) : IRequestHandler<SendMessageCommand, Result<SendMessageResponse>>
{
    public async Task<Result<SendMessageResponse>> Handle(SendMessageCommand cmd, CancellationToken ct)
    {
        try
        {
            // 1. Create domain entity (raises MessageReceivedEvent internally)
            var message = ContactMessage.Create(cmd.Name, cmd.Email, cmd.Subject, cmd.Message);

            // 2. Persist to MongoDB
            await contactRepo.AddAsync(message, ct);
            logger.LogInformation("Contact message {Id} saved.", message.Id);

            // 3. Publish domain events (triggers email notification via handler)
            foreach (var domainEvent in message.DomainEvents)
                await publisher.Publish(domainEvent, ct);

            message.ClearDomainEvents();

            // 4. Send auto-reply confirmation to visitor
            await emailService.SendContactConfirmationAsync(cmd.Email, cmd.Name, ct);

            return Result<SendMessageResponse>.Success(
                new SendMessageResponse(
                    message.Id,
                    "Thank you for your message! I'll get back to you within 24 hours."));
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning("Invalid contact message data: {Error}", ex.Message);
            return Result<SendMessageResponse>.Failure(ex.Message, "INVALID_DATA");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error saving contact message.");
            return Result<SendMessageResponse>.Failure(
                "An unexpected error occurred. Please try again.", "INTERNAL_ERROR");
        }
    }
}
