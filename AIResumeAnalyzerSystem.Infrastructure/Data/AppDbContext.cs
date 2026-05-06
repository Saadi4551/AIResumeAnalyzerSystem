using AIResumeAnalyzerSystem.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace AIResumeAnalyzerSystem.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Resume> Resumes { get; set; }
    public DbSet<ResumeAnalysis> ResumeAnalyses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PasswordHash).IsRequired();
        });

        modelBuilder.Entity<Resume>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User)
                .WithMany(u => u.Resumes)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ResumeAnalysis>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Resume)
                .WithOne(r => r.Analysis)
                .HasForeignKey<ResumeAnalysis>(e => e.ResumeId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}