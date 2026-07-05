# ── Stage 1: Restore ──────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS restore
WORKDIR /src

# Copy only project files first (layer caching — rebuilds only when deps change)
COPY src/Portfolio.Domain/Portfolio.Domain.csproj             src/Portfolio.Domain/
COPY src/Portfolio.Application/Portfolio.Application.csproj   src/Portfolio.Application/
COPY src/Portfolio.Infrastructure/Portfolio.Infrastructure.csproj src/Portfolio.Infrastructure/
COPY src/Portfolio.API/Portfolio.API.csproj                   src/Portfolio.API/

RUN dotnet restore src/Portfolio.API/Portfolio.API.csproj

# ── Stage 2: Build & Publish ───────────────────────────────────────────────────
FROM restore AS publish
WORKDIR /src

# Copy all source code
COPY src/ src/

RUN dotnet publish src/Portfolio.API/Portfolio.API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ── Stage 3: Runtime ───────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Non-root user for security
RUN addgroup --system --gid 1001 appgroup && \
    adduser  --system --uid 1001 --ingroup appgroup appuser

COPY --from=publish --chown=appuser:appgroup /app/publish .

USER appuser

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Portfolio.API.dll"]
