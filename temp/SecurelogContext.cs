using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SecureLog.temp;

public partial class SecurelogContext : DbContext
{
    public SecurelogContext()
    {
    }

    public SecurelogContext(DbContextOptions<SecurelogContext> options)
        : base(options)
    {
    }

    public virtual DbSet<VisitRequest> VisitRequests { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=dpg-d7v05e37uimc739bcic0-a.singapore-postgres.render.com;Database=securelog;Username=securelog_user;Password=C7uDlrhqLaStnVNDruZGdsgOZuKSKpYm;Port=5432;SSL Mode=Require;Trust Server Certificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<VisitRequest>(entity =>
        {
            entity.HasIndex(e => e.ClientUserId, "IX_VisitRequests_ClientUserId");

            entity.HasIndex(e => e.ConfirmationId, "IX_VisitRequests_ConfirmationId");

            entity.HasIndex(e => e.Status, "IX_VisitRequests_Status");

            entity.HasIndex(e => e.VisitDate, "IX_VisitRequests_VisitDate");

            entity.Property(e => e.Company).HasDefaultValueSql("''::text");
            entity.Property(e => e.PersonToMeet).HasDefaultValueSql("''::text");
            entity.Property(e => e.VisitTime).HasDefaultValueSql("'-infinity'::timestamp with time zone");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
