using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TestSql321.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Login> Logins { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, move it out of source code. The connection string is read from environment variable AUTOSERVICE_CONNECTION. Example: "Host=localhost;Port=5432;Database=AutoServiceDb;Username=postgres;Password=secret"
        => optionsBuilder.UseNpgsql(Environment.GetEnvironmentVariable("AUTOSERVICE_CONNECTION") ?? "Host=localhost;Port=5432;Database=TestDbAvalonia2;Username=postgres;Password=postgres;TrustServerCertificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Login>(entity =>
        {
            entity.HasKey(e => e.IdLogin);

            entity.Property(e => e.IdLogin).HasColumnName("id_Login");
            entity.Property(e => e.Login1)
                .HasColumnType("text")
                .HasColumnName("Login");
            entity.Property(e => e.Password).HasColumnType("text");
            entity.Property(e => e.UserId).HasColumnName("User_id");

            entity.HasOne(d => d.User).WithMany(p => p.Logins)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Logins_Users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUser);

            entity.Property(e => e.IdUser).HasColumnName("id_User");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.FullName).HasColumnType("text");
            entity.Property(e => e.PhoneNumber).HasColumnType("text");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
