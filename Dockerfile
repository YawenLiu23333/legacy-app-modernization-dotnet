# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY src/ModernizedApp/ModernizedApp.csproj src/ModernizedApp/
RUN dotnet restore src/ModernizedApp/ModernizedApp.csproj
COPY . .
RUN dotnet publish src/ModernizedApp/ModernizedApp.csproj -c Release -o /app/publish

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "ModernizedApp.dll"]
