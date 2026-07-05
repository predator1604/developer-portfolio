using MongoDB.Driver;
using Portfolio.Domain.Entities;
using Portfolio.Infrastructure.Persistence;

namespace Portfolio.API.Extensions;

/// <summary>
/// Seeds MongoDB with initial portfolio data (projects + skills)
/// if the collections are empty. Safe to run on every startup.
/// </summary>
public class DatabaseSeeder
{
    public static async Task SeedAsync(MongoDbContext ctx, ILogger logger)
    {
        await SeedProjectsAsync(ctx, logger);
        await SeedSkillGroupsAsync(ctx, logger);
    }

    // ── Projects ─────────────────────────────────────────────────────────────

    private static async Task SeedProjectsAsync(MongoDbContext ctx, ILogger logger)
    {
        if (await ctx.Projects.CountDocumentsAsync(_ => true) > 0) return;

        var projects = new List<Project>
        {
            Project.Create(
                title:       "NexusAgent",
                slug:        "nexusagent",
                category:    "MCP · Agentic AI · Enterprise",
                description: "An intelligent internal developer platform using Agentic AI to automate environment provisioning and code reviews through natural language commands.",
                stack:       [".NET Web API", "Angular 19", "MongoDB", "MCP", "Kubernetes", "Helm"],
                accentColor: "#6366f1",
                accentBg:    "rgba(99,102,241,0.08)",
                status:      "live",
                statusLabel: "Live project",
                sortOrder:   1,
                feature:     "MCP server lets LLM agents securely inspect MongoDB schemas and trigger CI/CD pipelines via Helm charts from plain English requests.",
                liveUrl:     "https://github.com/yourname/nexusagent",
                gitHubUrl:   "https://github.com/yourname/nexusagent"),

            Project.Create(
                title:       "CognitiveFlow",
                slug:        "cognitiveflow",
                category:    "RAG · Vector Search · Real-time",
                description: "Enterprise-grade semantic search and document interaction portal for complex corporate knowledge bases with a real-time Angular signals dashboard.",
                stack:       [".NET Core", "Semantic Kernel", "Angular Signals", "MongoDB Atlas", "LangChain"],
                accentColor: "#a78bfa",
                accentBg:    "rgba(139,92,246,0.08)",
                status:      "live",
                statusLabel: "Live project",
                sortOrder:   2,
                feature:     "Hybrid RAG pipeline combining keyword and vector search stored in MongoDB Atlas, visualised through a live Angular signals dashboard.",
                liveUrl:     "https://github.com/yourname/cognitiveflow",
                gitHubUrl:   "https://github.com/yourname/cognitiveflow"),

            Project.Create(
                title:       "OpsMind",
                slug:        "opsmind",
                category:    "DevOps · GenAI · Self-Healing",
                description: "Self-healing CI/CD dashboard that predicts pipeline failures, diagnoses root causes, and auto-generates fix pull requests using Agentic AI.",
                stack:       [".NET Services", "Angular", "GH Actions", "GenAI", "MongoDB"],
                accentColor: "#22c55e",
                accentBg:    "rgba(34,197,94,0.08)",
                status:      "in-progress",
                statusLabel: "In progress",
                sortOrder:   3,
                gitHubUrl:   "https://github.com/yourname/opsmind"),
        };

        await ctx.Projects.InsertManyAsync(projects);
        logger.LogInformation("✅ Seeded {Count} projects.", projects.Count);
    }

    // ── Skill Groups ──────────────────────────────────────────────────────────

    private static async Task SeedSkillGroupsAsync(MongoDbContext ctx, ILogger logger)
    {
        if (await ctx.SkillGroups.CountDocumentsAsync(_ => true) > 0) return;

        var groups = new List<SkillGroup>
        {
            SkillGroup.Create(
                groupId:     "backend",
                title:       "Backend",
                category:    "backend",
                accentColor: "#6366f1",
                accentBg:    "rgba(99,102,241,0.12)",
                iconPath:    "<rect x='1' y='1' width='6' height='6' rx='1' stroke='#6366f1' stroke-width='1.3'/><rect x='9' y='1' width='6' height='6' rx='1' stroke='#6366f1' stroke-width='1.3'/><rect x='1' y='9' width='6' height='6' rx='1' stroke='#6366f1' stroke-width='1.3'/><rect x='9' y='9' width='6' height='6' rx='1' stroke='#6366f1' stroke-width='1.3'/>",
                skills:
                [
                    new Skill { Name = ".NET Core / C#",        Level = 92 },
                    new Skill { Name = "Web API / Clean Arch",  Level = 88 },
                    new Skill { Name = "SignalR / gRPC",         Level = 75 },
                    new Skill { Name = "Entity Framework Core", Level = 80 },
                ],
                tools:       ["CQRS", "MediatR", "FluentValidation", "Serilog"],
                sortOrder:   1),

            SkillGroup.Create(
                groupId:     "frontend",
                title:       "Frontend",
                category:    "frontend",
                accentColor: "#ec4899",
                accentBg:    "rgba(236,72,153,0.10)",
                iconPath:    "<polyline points='4,6 1,8 4,10' stroke='#ec4899' stroke-width='1.3' stroke-linecap='round' stroke-linejoin='round'/><polyline points='12,6 15,8 12,10' stroke='#ec4899' stroke-width='1.3' stroke-linecap='round' stroke-linejoin='round'/><line x1='10' y1='3' x2='6' y2='13' stroke='#ec4899' stroke-width='1.3' stroke-linecap='round'/>",
                skills:
                [
                    new Skill { Name = "Angular 19",           Level = 90 },
                    new Skill { Name = "TypeScript / Signals",  Level = 88 },
                    new Skill { Name = "RxJS / NgRx",           Level = 78 },
                    new Skill { Name = "SCSS / Tailwind",       Level = 85 },
                ],
                tools:       ["Angular Material", "Storybook", "Jest", "Cypress"],
                sortOrder:   2),

            SkillGroup.Create(
                groupId:     "ai",
                title:       "AI / ML",
                category:    "ai",
                accentColor: "#a78bfa",
                accentBg:    "rgba(139,92,246,0.12)",
                iconPath:    "<circle cx='8' cy='8' r='3' stroke='#a78bfa' stroke-width='1.3'/><path d='M8 1v2M8 13v2M1 8h2M13 8h2M3.22 3.22l1.42 1.42M11.36 11.36l1.42 1.42M3.22 12.78l1.42-1.42M11.36 4.64l1.42-1.42' stroke='#a78bfa' stroke-width='1.3' stroke-linecap='round'/>",
                skills:
                [
                    new Skill { Name = "Agentic AI / LLM Orch.", Level = 85 },
                    new Skill { Name = "RAG Pipelines",           Level = 82 },
                    new Skill { Name = "Model Context Protocol",  Level = 78 },
                    new Skill { Name = "Semantic Kernel",         Level = 80 },
                ],
                tools:       ["LangChain", "Vector Search", "Prompt Engineering", "OpenAI SDK"],
                sortOrder:   3),

            SkillGroup.Create(
                groupId:     "devops",
                title:       "DevOps",
                category:    "devops",
                accentColor: "#22c55e",
                accentBg:    "rgba(34,197,94,0.10)",
                iconPath:    "<circle cx='8' cy='8' r='7' stroke='#22c55e' stroke-width='1.3'/><path d='M5 8l2 2 4-4' stroke='#22c55e' stroke-width='1.3' stroke-linecap='round' stroke-linejoin='round'/>",
                skills:
                [
                    new Skill { Name = "Docker / Kubernetes", Level = 83 },
                    new Skill { Name = "CI/CD (GH Actions)",  Level = 80 },
                    new Skill { Name = "Helm Charts",          Level = 72 },
                    new Skill { Name = "Azure DevOps",         Level = 76 },
                ],
                tools:       ["Azure", "Terraform", "ArgoCD", "Prometheus"],
                sortOrder:   4),

            SkillGroup.Create(
                groupId:     "database",
                title:       "Database",
                category:    "database",
                accentColor: "#f59e0b",
                accentBg:    "rgba(245,158,11,0.10)",
                iconPath:    "<ellipse cx='8' cy='4' rx='7' ry='2.5' stroke='#f59e0b' stroke-width='1.3'/><path d='M1 4v4c0 1.38 3.13 2.5 7 2.5S15 9.38 15 8V4' stroke='#f59e0b' stroke-width='1.3'/><path d='M1 8v4c0 1.38 3.13 2.5 7 2.5S15 13.38 15 12V8' stroke='#f59e0b' stroke-width='1.3'/>",
                skills:
                [
                    new Skill { Name = "MongoDB / Atlas",       Level = 87 },
                    new Skill { Name = "Aggregation Pipelines", Level = 83 },
                    new Skill { Name = "Vector Search",         Level = 78 },
                    new Skill { Name = "Redis / Caching",       Level = 74 },
                ],
                tools:       ["PostgreSQL", "BSON Mapping", "Change Streams", "Atlas Search"],
                sortOrder:   5),
        };

        await ctx.SkillGroups.InsertManyAsync(groups);
        logger.LogInformation("✅ Seeded {Count} skill groups.", groups.Count);
    }
}
