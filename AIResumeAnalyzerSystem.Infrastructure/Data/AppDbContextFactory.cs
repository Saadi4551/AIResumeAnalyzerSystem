using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AIResumeAnalyzerSystem.Infrastructure.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        
        optionsBuilder.UseNpgsql(
            "Host=ep-wispy-voice-aqkkwatw-pooler.c-8.us-east-1.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=npg_4ySclxhM3XYs;SSL Mode=Require;Trust Server Certificate=true",
            x => x.MigrationsAssembly("AIResumeAnalyzerSystem.Infrastructure"));

        return new AppDbContext(optionsBuilder.Options);
    }
}