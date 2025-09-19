# Deploy to Azure App Service (GitHub Actions)

## Option A — Publish Profile Secret (fastest)

1. Create an **Azure Web App** (Linux, .NET 8).
2. In the Web App → **Get publish profile**, download the XML.
3. On GitHub repo: **Settings → Secrets and variables → Actions → New repository secret**
   - Name: `AZURE_WEBAPP_PUBLISH_PROFILE`
   - Value: paste the XML
4. Add this step to `.github/workflows/dotnet.yml` (after tests):

```yaml
      - name: Deploy to Azure Web App
        uses: azure/webapps-deploy@v2
        with:
          app-name: <YOUR_WEBAPP_NAME>
          publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
          package: ./src/ModernizedApp/bin/Release/net8.0/publish
```

Push to `main`; deployment will run automatically.

## Option B — OIDC (no publish profile)

Use `azure/login@v2` with Federated Credentials and `azure/webapps-deploy@v2`. See Azure docs for setting up the Federated Credential.

## App Settings

Configure these in Azure → **Configuration**:
- `Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience`
- `Auth:Username`, `Auth:Password`
- `ConnectionStrings:Default` (e.g., `Data Source=/home/site/wwwroot/app.db` or Azure SQL)

## Test

Open `https://<YOUR_WEBAPP_NAME>.azurewebsites.net/swagger`.
