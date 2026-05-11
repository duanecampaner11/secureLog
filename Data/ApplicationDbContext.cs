using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SecureLog.Models;

namespace SecureLog.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options) { }

    public DbSet<VisitRequest> VisitRequests { get; set; }
    public DbSet<GuestEntry> GuestEntries { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.Entity<VisitRequest>()
            .HasOne(v => v.ClientUser)
            .WithMany(u => u.VisitRequests)
            .HasForeignKey(v => v.ClientUserId);
            
        builder.Entity<VisitRequest>()
            .HasIndex(v => v.Status);
            
        builder.Entity<VisitRequest>()
            .HasIndex(v => v.VisitDate);
            
        builder.Entity<VisitRequest>()
            .HasIndex(v => v.ConfirmationId);
            
        builder.Entity<ApplicationUser>()
            .HasIndex(u => u.IsApproved);
            
        builder.Entity<GuestEntry>().HasIndex(g => g.TimeIn);
        builder.Entity<GuestEntry>().HasIndex(g => g.Name);
    }
}