using AIResumeAnalyzerSystem.API.Extensions;
using AIResumeAnalyzerSystem.API.Middlewares;
using AIResumeAnalyzerSystem.Infrastructure.Data;
using AIResumeAnalyzerSystem.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ✅ Swagger with Bearer token support (for testing only)
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AI Resume Analyzer API",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your token}"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ✅ Register all application services (DB, Repos, Services)
builder.Services.AddApplicationServices(builder.Configuration);

// ✅ JWT Authentication - reads token from HttpOnly Cookie
builder.Services.AddJwtAuthentication(builder.Configuration);

// ✅ CORS - allows frontend with credentials (cookies)
builder.Services.AddCorsPolicy();

// ✅ Gemini AI HTTP client
builder.Services.AddHttpClient<GeminiService>();

var app = builder.Build();

// ✅ Global exception handling middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

// ✅ Swagger UI - available in all environments
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "AI Resume Analyzer API v1");
});

// ✅ Auto-apply database migrations on startup (runs on Railway too)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// ✅ CORS must be before Authentication
app.UseCors("AllowAll");

// ✅ Authentication reads JWT from HttpOnly Cookie
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();