using MediatR;
using Microsoft.Extensions.Logging;
using Portfolio.Application.Common;
using Portfolio.Application.Common.Interfaces;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Enums;
using Portfolio.Domain.Interfaces;

namespace Portfolio.Application.Features.Admin.Commands;

// ── DTO ───────────────────────────────────────────────────────────────────────

public sealed record ChatMessageDto(string Role, string Content, DateTime Timestamp);

// ── Command ───────────────────────────────────────────────────────────────────

public sealed record SendChatMessageCommand(
    string SessionId,
    string UserMessage,
    string? VisitorIp = null) : IRequest<Result<ChatResponseDto>>;

public sealed record ChatResponseDto(
    string SessionId,
    string AssistantReply,
    List<ChatMessageDto> History);

// ── Handler ───────────────────────────────────────────────────────────────────

public sealed class SendChatMessageHandler(
    IChatSessionRepository chatRepo,
    IAiChatService         aiService,
    ILogger<SendChatMessageHandler> logger)
    : IRequestHandler<SendChatMessageCommand, Result<ChatResponseDto>>
{
    public async Task<Result<ChatResponseDto>> Handle(
        SendChatMessageCommand cmd, CancellationToken ct)
    {
        try
        {
            // 1. Get or create chat session
            var session = await chatRepo.GetBySessionIdAsync(cmd.SessionId, ct);
            if (session is null)
            {
                session = ChatSession.Create(cmd.SessionId, cmd.VisitorIp);
                await chatRepo.AddAsync(session, ct);
            }

            // 2. Build conversation history for context
            var history = session.Messages
                .Select(m => (m.Role.ToString().ToLower(), m.Content))
                .ToList();

            // 3. Add user message to session
            session.AddMessage(ChatMessageRole.User, cmd.UserMessage);

            // 4. Call AI service with full context
            var reply = await aiService.ChatAsync(cmd.UserMessage, history, ct);
            logger.LogInformation("AI reply generated for session {SessionId}", cmd.SessionId);

            // 5. Add assistant reply to session
            session.AddMessage(ChatMessageRole.Assistant, reply);

            // 6. Persist updated session
            await chatRepo.UpdateAsync(session, ct);

            var historyDtos = session.Messages
                .Select(m => new ChatMessageDto(m.Role.ToString(), m.Content, m.Timestamp))
                .ToList();

            return Result<ChatResponseDto>.Success(
                new ChatResponseDto(cmd.SessionId, reply, historyDtos));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing chat message for session {SessionId}", cmd.SessionId);
            return Result<ChatResponseDto>.Failure(
                "The AI assistant is temporarily unavailable.", "AI_ERROR");
        }
    }
}
