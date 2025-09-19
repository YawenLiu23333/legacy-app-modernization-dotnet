# Legacy App Modernization — .NET 8, Auth, Logging, Azure-ready


- Migrated a legacy-style app to **.NET 8 minimal APIs**, optimized outbound API calls (caching + resilience).
- **Authentication & logging:** JWT auth + structured logs via Serilog.
- **Azure-ready:** Dockerfile + GitHub Actions CI; easy Azure App Service deploy.
- **Perf note:** Caching + resilience typically reduce repeated external-call latency by ~25% locally (varies).

## Demo (Screenshots)

> A quick walkthrough of the modernized .NET API. All shots captured from GitHub Codespaces.

**1) Swagger Home**  
![Swagger endpoints](assets/demo/01-swagger.png)  
Modernized minimal APIs with health, auth, products, and an external call.

**2) Health OK**  
![Health OK](assets/demo/02-health.png)  
Simple liveness probe: `GET /health` → `{"status":"ok"}`.

**3) Login (request)**  
![Login request](assets/demo/03-login-request.png)  
POST `/auth/login` with demo creds to obtain a JWT.

**4) Login (response)**  
![Login response (token truncated)](assets/demo/04-login-response.png)  
JWT returned on success (**token redacted** in screenshot).

**5) Authorize (Swagger)** *(optional)*  
![Authorize modal](assets/demo/05-authorize.png)  
Paste `Bearer <token>` to test protected endpoints in the UI.

