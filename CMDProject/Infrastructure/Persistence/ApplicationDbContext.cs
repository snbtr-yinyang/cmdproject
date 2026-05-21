using Microsoft.EntityFrameworkCore;
using CMDProject.Domain.Entities;

namespace CMDProject.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<MsAuthUsers> AuthUsers => Set<MsAuthUsers>();
    public DbSet<MsUsers> Users => Set<MsUsers>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
