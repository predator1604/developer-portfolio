using MediatR;
using Portfolio.Application.Common;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Enums;
using Portfolio.Domain.Interfaces;

namespace Portfolio.Application.Features.Contact.Queries;

// ── DTOs ──────────────────────────────────────────────────────────────────────

public sealed record ContactMessageDto(
    string        Id,
    string        Name,
    string        Email,
    string        Subject,
    string        Message,
    MessageStatus Status,
    string?       ReplyNote,
    DateTime?     RepliedAt,
    DateTime      CreatedAt);

public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int              Page,
    int              PageSize,
    long             TotalCount);

// ── Query: all messages (admin) ───────────────────────────────────────────────

public sealed record GetMessagesQuery(
    int Page     = 1,
    int PageSize = 20) : IRequest<Result<PagedResult<ContactMessageDto>>>;

public sealed class GetMessagesHandler(IContactRepository repo)
    : IRequestHandler<GetMessagesQuery, Result<PagedResult<ContactMessageDto>>>
{
    public async Task<Result<PagedResult<ContactMessageDto>>> Handle(
        GetMessagesQuery query, CancellationToken ct)
    {
        var messages = await repo.GetAllAsync(query.Page, query.PageSize, ct);
        var total    = await repo.CountAsync(ct);

        var dtos = messages.Select(ToDto).ToList();

        return Result<PagedResult<ContactMessageDto>>.Success(
            new PagedResult<ContactMessageDto>(dtos, query.Page, query.PageSize, total));
    }

    private static ContactMessageDto ToDto(ContactMessage m) => new(
        m.Id, m.Name, m.Email.Value, m.Subject, m.Message,
        m.Status, m.ReplyNote, m.RepliedAt, m.CreatedAt);
}

// ── Query: single message ─────────────────────────────────────────────────────

public sealed record GetMessageByIdQuery(string Id)
    : IRequest<Result<ContactMessageDto>>;

public sealed class GetMessageByIdHandler(IContactRepository repo)
    : IRequestHandler<GetMessageByIdQuery, Result<ContactMessageDto>>
{
    public async Task<Result<ContactMessageDto>> Handle(
        GetMessageByIdQuery query, CancellationToken ct)
    {
        var message = await repo.GetByIdAsync(query.Id, ct);
        if (message is null)
            return Result<ContactMessageDto>.Failure("Message not found.", "NOT_FOUND");

        return Result<ContactMessageDto>.Success(new ContactMessageDto(
            message.Id, message.Name, message.Email.Value, message.Subject,
            message.Message, message.Status, message.ReplyNote,
            message.RepliedAt, message.CreatedAt));
    }
}
