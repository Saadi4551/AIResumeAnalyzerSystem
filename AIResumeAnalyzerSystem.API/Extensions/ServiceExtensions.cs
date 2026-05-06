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
        // Database
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                x => x.MigrationsAssembly("AIResumeAnalyzerSystem.Infrastructure")));

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IResumeRepository, ResumeRepository>();

        // Services
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
                policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
        return services;
    }
}