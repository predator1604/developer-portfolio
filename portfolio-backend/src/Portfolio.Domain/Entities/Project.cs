using Portfolio.Domain.Common;

namespace Portfolio.Domain.Entities;

/// <summary>
/// Represents a portfolio project stored in MongoDB and served
/// to the Angular frontend via the Projects API endpoint.
/// </summary>
public sealed class Project : BaseEntity
{
    public string Title       { get; private set; }
    public string Slug        { get; private set; }   // URL-friendly key e.g. "nexusagent"
    public string Category    { get; private set; }   // e.g. "MCP · Agentic AI"
    public string Description { get; private set; }
    public string? Feature    { get; private set; }   // Key feature callout text
    public List<string> Stack { get; private set; }   // Tech chips
    public string AccentColor { get; private set; }   // e.g. "#6366f1"
    public string AccentBg    { get; private set; }   // e.g. "rgba(99,102,241,0.08)"
    public string Status      { get; private set; }   // "live" | "in-progress" | "archived"
    public string StatusLabel { get; private set; }
    public string? LiveUrl    { get; private set; }
    public string? GitHubUrl  { get; private set; }
    public int    SortOrder   { get; private set; }
    public bool   IsVisible   { get; private set; }

    private Project()
    {
        Title = Category = Description = AccentColor =
        AccentBg = Status = StatusLabel = Slug = string.Empty;
        Stack = [];
    }

    public static Project Create(
        string title,
        string slug,
        string category,
        string description,
        List<string> stack,
        string accentColor,
        string accentBg,
        string status,
        string statusLabel,
        int sortOrder,
        string? feature    = null,
        string? liveUrl    = null,
        string? gitHubUrl  = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(slug);

        return new Project
        {
            Title       = title,
            Slug        = slug.ToLowerInvariant(),
            Category    = category,
            Description = description,
            Feature     = feature,
            Stack       = stack,
            AccentColor = accentColor,
            AccentBg    = accentBg,
            Status      = status,
            StatusLabel = statusLabel,
            SortOrder   = sortOrder,
            IsVisible   = true,
            LiveUrl     = liveUrl,
            GitHubUrl   = gitHubUrl,
        };
    }

    public void Update(
        string title, string category, string description,
        List<string> stack, string status, string statusLabel,
        bool isVisible, string? feature = null,
        string? liveUrl = null, string? gitHubUrl = null)
    {
        Title       = title;
        Category    = category;
        Description = description;
        Feature     = feature;
        Stack       = stack;
        Status      = status;
        StatusLabel = statusLabel;
        IsVisible   = isVisible;
        LiveUrl     = liveUrl;
        GitHubUrl   = gitHubUrl;
        Touch();
    }

    public void SetVisibility(bool visible) { IsVisible = visible; Touch(); }
    public void SetSortOrder(int order)     { SortOrder = order;   Touch(); }
}
