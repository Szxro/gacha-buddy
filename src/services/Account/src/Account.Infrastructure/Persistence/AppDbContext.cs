using Account.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Account.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<DateTime>().HaveColumnType("datetime2");
        
        base.ConfigureConventions(configurationBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<User> User => Set<User>();
    
    public DbSet<Credentials> Credentials => Set<Credentials>();
    
    public DbSet<Role> Role => Set<Role>();
    
    public DbSet<UserRole> UserRole => Set<UserRole>();
    
    public DbSet<Permission> Permission => Set<Permission>();
    
    public DbSet<RolePermission> RolePermission => Set<RolePermission>();
    
    public DbSet<AuditLog> AuditLog => Set<AuditLog>();

    public DbSet<RefreshToken> RefreshToken => Set<RefreshToken>();

    public DbSet<EmailCode> EmailCode => Set<EmailCode>();
}