using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Models;

public partial class _ApplicationDbContext : DbContext
{
    public _ApplicationDbContext()
    {
    }

    public _ApplicationDbContext(DbContextOptions<_ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ExpenseDetail> ExpenseDetails { get; set; }

    public virtual DbSet<IncomeDateAndAmount> IncomeDateAndAmounts { get; set; }

    public virtual DbSet<Payable> Payables { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-IK7MGEI;Initial Catalog=DbExpenseTracker;Persist Security Info=False;User ID=sa;Password=Hello@123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ExpenseDetail>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<IncomeDateAndAmount>(entity =>
        {
            entity.ToTable("IncomeDateAndAmount");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.IncomeAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IncomeDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Payable>(entity =>
        {
            entity.ToTable("Payable");

            entity.Property(e => e.Payableid).HasColumnName("payableid");
            entity.Property(e => e.IsRecuring).HasDefaultValueSql("((0))");
            entity.Property(e => e.Payablename).HasColumnName("payablename");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IsComplete).HasDefaultValueSql("((0))");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
