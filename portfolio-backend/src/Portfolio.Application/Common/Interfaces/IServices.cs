namespace Portfolio.Application.Common.Interfaces;

// ── Email ─────────────────────────────────────────────────────────────────────

public interface IEmailService
{
    /// <summary>Sends an email notification to the portfolio owner.</summary>
    Task SendContactNotificationAsync(
        string senderName,
        string senderEmail,
        string subject,
        string message,
        CancellationToken ct = default);

    /// <summary>Sends an auto-reply confirmation to the visitor.</summary>
    Task SendContactConfirmationAsync(
        string toEmail,
        string toName,
        CancellationToken ct = default);
}

// ── AI Chat ───────────────────────────────────────────────────────────────────

public interface IAiChatService
{
    /// <summary>
    /// Sends a user message to the AI assistant and returns the reply.
    /// Context is maintained across the session via conversationHistory.
    /// </summary>
    Task<string> ChatAsync(
        string userMessage,
        IReadOnlyList<(string Role, string Content)> conversationHistory,
        CancellationToken ct = default);
}
