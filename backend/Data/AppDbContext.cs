using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Coordinator> Coordinators => Set<Coordinator>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<Participant> Participants => Set<Participant>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Store the SessionSlot enum as a string in SQLite for readability
        modelBuilder.Entity<Session>()
            .Property(s => s.Slot)
            .HasConversion<string>();

        // Don't map the computed Assigned property to the DB
        modelBuilder.Entity<Participant>()
            .Ignore(p => p.Assigned);
    }
}