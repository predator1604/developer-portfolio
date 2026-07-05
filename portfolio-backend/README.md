# Portfolio Backend API

> Clean Architecture .NET 9 backend — Contact form, Projects & Skills API, AI Chat assistant.

## Tech Stack

| Layer          | Technology                              |
|----------------|-----------------------------------------|
| API            | ASP.NET Core 9, Minimal + MVC           |
| CQRS           | MediatR 12, FluentValidation            |
| Database       | MongoDB 7 (Driver 3.x)                  |
| Email          | MailKit / SMTP                          |
| AI             | Semantic Kernel + OpenAI / Azure OpenAI |
| Logging        | Serilog (Console + File)                |
| Containerisation | Docker, Docker Compose                |
| CI/CD          | GitHub Actions → VPS via SSH            |

---

## Project Structure

```
portfolio-backend/
├── src/
│   ├── Portfolio.Domain/          # Entities, value objects, interfaces, events
│   ├── Portfolio.Application/     # CQRS commands/queries, MediatR, validators
│   ├── Portfolio.Infrastructure/  # MongoDB repos, SMTP email, Semantic Kernel AI
│   └── Portfolio.API/             # Controllers, middleware, Program.cs, seeder
├── docker-compose.yml
├── Dockerfile
├── Portfolio.sln
└── .github/workflows/ci-cd.yml
```

---

## Quick Start (Local)

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

### Option A — Docker Compose (recommended)

```bash
git clone https://github.com/yourname/portfolio-backend
cd portfolio-backend

# Start MongoDB + MailHog + API
docker compose up -d

# View logs
docker compose logs -f portfolio-api
```

| Service       | URL                         |
|---------------|-----------------------------|
| API           | http://localhost:8080        |
| Swagger UI    | http://localhost:8080/swagger|
| MailHog UI    | http://localhost:8025        |
| Mongo Express | http://localhost:8081        |

### Option B — Run API locally against Docker MongoDB

```bash
# Start only MongoDB + MailHog
docker compose up -d mongodb mailhog

# Run API
cd src/Portfolio.API
dotnet run
```

API will be at `https://localhost:5001` / `http://localhost:5000`.

---

## Configuration

Copy and edit `appsettings.Development.json`:

```jsonc
{
  "MongoDB": {
    "ConnectionString": "mongodb://localhost:27017"
  },
  "Smtp": {
    "Host": "localhost",
    "Port": 1025            // MailHog local
  },
  "AI": {
    "Provider": "OpenAI",
    "ApiKey": "sk-...",     // Replace with your key
    "ModelId": "gpt-4o-mini"
  }
}
```

---

## API Endpoints

### Contact
| Method | Route                 | Auth   | Description                  |
|--------|-----------------------|--------|------------------------------|
| POST   | `/api/contact`        | Public | Submit contact form          |
| GET    | `/api/contact`        | Admin  | List all messages (paged)    |
| GET    | `/api/contact/{id}`   | Admin  | Get message by ID            |

### Projects
| Method | Route                   | Auth   | Description              |
|--------|-------------------------|--------|--------------------------|
| GET    | `/api/projects`         | Public | All visible projects     |
| GET    | `/api/projects/{slug}`  | Public | Project by slug          |
| POST   | `/api/projects`         | Admin  | Create project           |
| PUT    | `/api/projects/{id}`    | Admin  | Update project           |
| DELETE | `/api/projects/{id}`    | Admin  | Delete project           |

### Skills
| Method | Route          | Auth   | Description              |
|--------|----------------|--------|--------------------------|
| GET    | `/api/skills`  | Public | All visible skill groups |

### AI Chat
| Method | Route       | Auth   | Description                        |
|--------|-------------|--------|------------------------------------|
| POST   | `/api/chat` | Public | Send message to AI portfolio assistant |

---

## Deployment (VPS with Docker)

1. Add GitHub Secrets: `VPS_HOST`, `VPS_USER`, `VPS_SSH_KEY`
2. Push to `main` → GitHub Actions builds, pushes image, and deploys via SSH
3. On the VPS, copy `docker-compose.yml` to `/opt/portfolio/` and set production env vars

---

## Health Check

```bash
curl http://localhost:8080/health
# → Healthy
```
