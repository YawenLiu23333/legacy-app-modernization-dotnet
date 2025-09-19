# Legacy App Modernization — .NET 8, Auth, Logging, Azure-ready


- Migrated a legacy-style app to **.NET 8 minimal APIs**, optimized outbound API calls (caching + resilience).
- **Authentication & logging:** JWT auth + structured logs via Serilog.
- **Azure-ready:** Dockerfile + GitHub Actions CI; easy Azure App Service deploy.
- **Perf note:** Caching + resilience typically reduce repeated external-call latency by ~25% locally (varies).

