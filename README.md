# Legacy App Modernization — .NET 8, Auth, Logging, Azure-ready

**Highlights for recruiters:**
- Migrated a legacy-style app to **.NET 8 minimal APIs**, optimized outbound API calls (caching + resilience).
- **Authentication & logging:** JWT auth + structured logs via Serilog.
- **Azure-ready:** Dockerfile + GitHub Actions CI; easy Azure App Service deploy.
- **Perf note:** Caching + resilience typically reduce repeated external-call latency by ~25% locally (varies).

## Project Structure
```
legacy-app-modernization-dotnet/
├─ src/ModernizedApp/
│  ├─ Program.cs
│  ├─ ModernizedApp.csproj
│  ├─ appsettings.json
│  ├─ appsettings.Development.json
│  ├─ Data/
│  │  ├─ AppDbContext.cs
│  │  ├─ Product.cs
│  │  └─ DbInitializer.cs
│  ├─ Services/
│  │  ├─ ExternalApiService.cs
│  │  └─ IExternalApiService.cs
│  └─ Properties/launchSettings.json
├─ tests/ModernizedApp.Tests/
│  ├─ ModernizedApp.Tests.csproj
│  └─ HealthTests.cs
├─ Dockerfile
├─ .github/workflows/dotnet.yml
└─ deployment/azure/azure-webapp-deploy.md
```

## Run locally

Prereqs: **.NET 8 SDK**

```bash
dotnet build src/ModernizedApp/ModernizedApp.csproj -c Release
dotnet run --project src/ModernizedApp/ModernizedApp.csproj
# Swagger UI:
open http://localhost:5000/swagger   # Windows: start http://localhost:5000/swagger
# Health:
curl http://localhost:5000/health
```

### Auth (JWT demo)
```bash
# default dev creds: admin / admin123
TOKEN=$(curl -s -X POST http://localhost:5000/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"admin123"}' | jq -r .token)

curl -H "Authorization: Bearer $TOKEN" http://localhost:5000/api/products
```

## Push to GitHub (recruiter-ready)

```bash
git init
git add .
git commit -m "feat: .NET 8 modernization demo with auth, logging, caching, CI"
git branch -M main
git remote add origin https://github.com/<your-username>/legacy-app-modernization-dotnet.git
git push -u origin main
```

### Optional polish
- Repo → Settings → **Public**
- Description + Topics: `dotnet`, `csharp`, `azure`, `modernization`, `serilog`, `polly`
- Pin on profile; verify CI badge is green.

## Azure App Service deploy

See `deployment/azure/azure-webapp-deploy.md` for step-by-step GitHub Actions deployment (Publish Profile or OIDC).
