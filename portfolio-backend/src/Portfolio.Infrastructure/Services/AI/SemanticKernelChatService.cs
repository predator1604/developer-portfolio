using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Portfolio.Application.Common.Interfaces;

namespace Portfolio.Infrastructure.Services.AI;

// ── Settings ─────────────────────────────────────────────────────────────────

public sealed class AiSettings
{
    public string Provider { get; init; } = "OpenAI";   // "OpenAI" | "AzureOpenAI"

    public string ApiKey { get; init; } = string.Empty;

    public string ModelId { get; init; } = "gpt-4o-mini";

    public string? Endpoint { get; init; }                // Required for Azure OpenAI

    public string? DeploymentName { get; init; }           // Required for Azure OpenAI
}

// ── Service ───────────────────────────────────────────────────────────────────

/// <summary>
/// Portfolio AI chat assistant powered by Semantic Kernel.
/// The system prompt gives the LLM full context about the portfolio owner,
/// projects (NexusAgent, CognitiveFlow, OpsMind), and tech stack so it can
/// answer visitor questions intelligently.
/// </summary>
public sealed class SemanticKernelChatService(IOptions<AiSettings> options, ILogger<SemanticKernelChatService> logger) : IAiChatService
{
    private readonly AiSettings _settings = options.Value;

    private const string SystemPrompt = """
        You are an AI assistant embedded in the portfolio website of a Full-Stack AI Engineer
        who specializes in enterprise .NET and Angular applications supercharged by Agentic AI,
        RAG pipelines, and DevOps automation.

        ABOUT THE OWNER:
        - Expert in .NET Core (Web API, Clean Architecture, CQRS, MediatR)
        - Expert in Angular 19 (standalone components, signals, reactive forms)
        - Expert in MongoDB (aggregation pipelines, Atlas Vector Search)
        - Expert in Agentic AI, RAG pipelines, and Model Context Protocol (MCP)
        - Proficient in Docker, Kubernetes, Helm, GitHub Actions, Azure DevOps

        PORTFOLIO PROJECTS:
        1. NexusAgent — MCP-Driven Enterprise Orchestrator
           An intelligent internal developer platform using Agentic AI to automate
           environment provisioning and code reviews. Uses MCP server to let LLM agents
           inspect MongoDB schemas and trigger CI/CD via Helm charts.

        2. CognitiveFlow — Advanced RAG Knowledge Base
           Enterprise semantic search portal with hybrid RAG pipeline combining keyword
           and vector search in MongoDB Atlas, visualized in a real-time Angular signals dashboard.

        3. OpsMind — Self-Healing CI/CD Dashboard
           DevOps monitoring suite that predicts pipeline failures and auto-generates
           fix pull requests using Agentic AI.

        INSTRUCTIONS:
        - Answer questions about the owner's skills, projects, experience, and availability.
        - Be concise, professional, and friendly.
        - If asked about hiring or collaboration, encourage the visitor to use the contact form.
        - Do NOT make up information not provided in this prompt.
        - Keep responses under 150 words unless detail is specifically requested.
        """;

    public async Task<string> ChatAsync(string userMessage, IReadOnlyList<(string Role, string Content)> conversationHistory, CancellationToken ct = default)
    {
        try
        {
            var kernel = BuildKernel();
            var chatService = kernel.GetRequiredService<IChatCompletionService>();

            var chatHistory = new ChatHistory(SystemPrompt);

            // Rebuild history from stored messages
            foreach (var (role, content) in conversationHistory)
            {
                if (role.Equals("user", StringComparison.OrdinalIgnoreCase))
                    chatHistory.AddUserMessage(content);
                else if (role.Equals("assistant", StringComparison.OrdinalIgnoreCase))
                    chatHistory.AddAssistantMessage(content);
            }

            chatHistory.AddUserMessage(userMessage);

            var result = await chatService.GetChatMessageContentAsync(chatHistory, cancellationToken: ct);
            return result.Content ?? "I'm sorry, I couldn't generate a response. Please try again.";
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "AI chat service error");
            throw;
        }
    }

    private Kernel BuildKernel()
    {
        var builder = Kernel.CreateBuilder();

        if (_settings.Provider.Equals("AzureOpenAI", StringComparison.OrdinalIgnoreCase))
        {
            builder.AddAzureOpenAIChatCompletion(
                deploymentName: _settings.DeploymentName ?? _settings.ModelId,
                endpoint: _settings.Endpoint!,
                apiKey: _settings.ApiKey);
        }
        else
        {
            builder.AddOpenAIChatCompletion(
                modelId: _settings.ModelId,
                apiKey: _settings.ApiKey);
        }

        return builder.Build();
    }
}
