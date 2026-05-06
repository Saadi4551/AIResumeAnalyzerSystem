using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AIResumeAnalyzerSystem.Infrastructure.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        
        optionsBuilder.UseSqlServer(
            "Server=THINKPAD-T470S;Database=AIResumalyzerDB;Trusted_Connection=True;TrustServerCertificate=True",
            x => x.MigrationsAssembly("AIResumeAnalyzerSystem.Infrastructure"));

        return new AppDbContext(optionsBuilder.Options);
    }
}