using Microsoft.EntityFrameworkCore;
using CMDProject.Domain.Entities;

namespace CMDProject.Infrastructure.Persistence.DBContext;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<AuthUsers> AuthUsers => Set<AuthUsers>();
    public DbSet<Users> Users => Set<Users>();
    public DbSet<UserToken> UserTokens => Set<UserToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
