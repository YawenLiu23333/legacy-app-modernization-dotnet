using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using ModernizedApp.Data;
using ModernizedApp.Services;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

// Serilog configuration
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();
builder.Host.UseSerilog();

// EF Core (SQLite)
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("Default") ?? "Data Source=app.db"));

// Caching + HttpClient with resilience
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient<IExternalApiService, ExternalApiService>()
    .AddStandardResilienceHandler(); // .NET 8 built-in resilience handler

// Auth: JWT Bearer
var jwtKey = builder.Configuration["Jwt:Key"] ?? "dev-secret-change-me";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "ModernizedApp";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "ModernizedAppAudience";
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = key,
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Request logging
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

// Health
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

// Seed DB
await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.EnsureCreatedAsync();
    await DbInitializer.SeedAsync(db);
}

// Auth: demo login issues JWT
app.MapPost("/auth/login", (IConfiguration cfg, LoginDto login) =>
{
    var user = cfg["Auth:Username"] ?? "admin";
    var pass = cfg["Auth:Password"] ?? "admin123";

    if (login.Username != user || login.Password != pass)
        return Results.Unauthorized();

    var jwtKey = cfg["Jwt:Key"] ?? "dev-secret-change-me";
    var jwtIssuer = cfg["Jwt:Issuer"] ?? "ModernizedApp";
    var jwtAudience = cfg["Jwt:Audience"] ?? "ModernizedAppAudience";

    var token = new JwtSecurityToken(
        issuer: jwtIssuer,
        audience: jwtAudience,
        claims: new[] { new Claim(ClaimTypes.Name, login.Username) },
        expires: DateTime.UtcNow.AddHours(1),
        signingCredentials: new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)), SecurityAlgorithms.HmacSha256
        )
    );
    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
    return Results.Ok(new { token = tokenString });
})
.WithName("Login")
.Produces(StatusCodes.Status200OK);

app.MapGet("/api/products", async (AppDbContext db) =>
    Results.Ok(await db.Products.AsNoTracking().ToListAsync()))
    .WithName("GetProducts");

app.MapPost("/api/products", async (AppDbContext db, Product p) =>
{
    db.Products.Add(p);
    await db.SaveChangesAsync();
    return Results.Created($"/api/products/{p.Id}", p);
})
.RequireAuthorization()
.WithName("CreateProduct");

app.MapGet("/api/external/todo", async (IExternalApiService svc) =>
{
    var todo = await svc.GetTodoAsync();
    return Results.Ok(todo);
});

app.Run();

// DTOs
record LoginDto(string Username, string Password);

// Expose Program for tests
public partial class Program { }
