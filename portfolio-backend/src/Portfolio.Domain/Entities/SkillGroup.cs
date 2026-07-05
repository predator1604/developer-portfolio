using Portfolio.Domain.Common;

namespace Portfolio.Domain.Entities;

/// <summary>
/// Represents a skill group (e.g. Backend, Frontend, AI/ML)
/// served from MongoDB to the Angular Skills section.
/// </summary>
public sealed class SkillGroup : BaseEntity
{
    public string        GroupId     { get; private set; }  // e.g. "backend"
    public string        Title       { get; private set; }
    public string        Category    { get; private set; }  // matches frontend SkillCategory
    public string        AccentColor { get; private set; }
    public string        AccentBg    { get; private set; }
    public string        IconPath    { get; private set; }  // SVG inner markup
    public List<Skill>   Skills      { get; private set; }
    public List<string>  Tools       { get; private set; }  // "Also used" chips
    public int           SortOrder   { get; private set; }
    public bool          IsVisible   { get; private set; }

    private SkillGroup()
    {
        GroupId = Title = Category =
        AccentColor = AccentBg = IconPath = string.Empty;
        Skills = [];
        Tools  = [];
    }

    public static SkillGroup Create(
        string groupId, string title, string category,
        string accentColor, string accentBg, string iconPath,
        List<Skill> skills, List<string> tools, int sortOrder)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(groupId);
        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        return new SkillGroup
        {
            GroupId     = groupId.ToLowerInvariant(),
            Title       = title,
            Category    = category,
            AccentColor = accentColor,
            AccentBg    = accentBg,
            IconPath    = iconPath,
            Skills      = skills,
            Tools       = tools,
            SortOrder   = sortOrder,
            IsVisible   = true,
        };
    }

    public void UpdateSkills(List<Skill> skills) { Skills = skills; Touch(); }
    public void SetVisibility(bool visible)       { IsVisible = visible; Touch(); }
}

/// <summary>Owned type — embedded inside SkillGroup document.</summary>
public sealed class Skill
{
    public string Name  { get; set; } = string.Empty;
    public int    Level { get; set; } // 0–100
}
