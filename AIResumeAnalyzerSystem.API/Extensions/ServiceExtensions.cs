using AIResumeAnalyzerSystem.Core.Interfaces.Repositories;
using AIResumeAnalyzerSystem.Core.Interfaces.Services;
using AIResumeAnalyzerSystem.Infrastructure.Data;
using AIResumeAnalyzerSystem.Infrastructure.Repositories;
using AIResumeAnalyzerSystem.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace AIResumeAnalyzerSystem.API.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ✅ PostgreSQL Database (Neon in production, local in dev)
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                x => x.MigrationsAssembly("AIResumeAnalyzerSystem.Infrastructure")));

        // ✅ Repository registrations
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IResumeRepository, ResumeRepository>();

        // ✅ Service registrations
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IResumeService, ResumeService>();

        return services;
    }

    public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy
                    .WithOrigins(
                        // ✅ Production frontend on Render
                        "https://ai-resume-plcw.onrender.com",
                        // ✅ Local development (React default ports)
                        "http://localhost:3000",
                        "http://localhost:5173",
                        "https://localhost:3000",
                        "https://localhost:5173"
                    )
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    // ✅ CRITICAL: Required for cookies to work cross-origin
                    // Without this, browser will not send/receive cookies
                    .AllowCredentials();
            });
        });
        return services;
    }
}