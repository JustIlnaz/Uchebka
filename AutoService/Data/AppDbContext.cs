using System;
using Microsoft.EntityFrameworkCore;

namespace AutoService.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // DbSets for domain entities
    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Department> Departments { get; set; }
    public virtual DbSet<Specialization> Specializations { get; set; }
    public virtual DbSet<Mechanic> Mechanics { get; set; }
    public virtual DbSet<Client> Clients { get; set; }
    public virtual DbSet<CarMake> CarMakes { get; set; }
    public virtual DbSet<CarCategory> CarCategories { get; set; }
    public virtual DbSet<Car> Cars { get; set; }
    public virtual DbSet<Service> Services { get; set; }
    public virtual DbSet<MechanicService> MechanicServices { get; set; }
    public virtual DbSet<RepairBay> RepairBays { get; set; }
    public virtual DbSet<MechanicSchedule> MechanicSchedules { get; set; }
    public virtual DbSet<Appointment> Appointments { get; set; }
    public virtual DbSet<WorkOrder> WorkOrders { get; set; }
    public virtual DbSet<WorkOrderService> WorkOrderServices { get; set; }
    public virtual DbSet<Supplier> Suppliers { get; set; }
    public virtual DbSet<Part> Parts { get; set; }
    public virtual DbSet<WorkOrderPart> WorkOrderParts { get; set; }
    public virtual DbSet<Invoice> Invoices { get; set; }
    public virtual DbSet<Payment> Payments { get; set; }
    public virtual DbSet<InspectionResult> InspectionResults { get; set; }
    public virtual DbSet<Review> Reviews { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning Connection string is read from environment variable AUTOSERVICE_CONNECTION. Example: "Host=localhost;Port=5432;Database=AutoServiceDb;Username=postgres;Password=secret"
        => optionsBuilder.UseNpgsql(Environment.GetEnvironmentVariable("AUTOSERVICE_CONNECTION") ?? "Host=localhost;Port=5432;Database=AutoServiceDb;Username=postgres;Password=123;TrustServerCertificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure keys and relationships where necessary
        modelBuilder.Entity<Role>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).IsRequired();
        });

        modelBuilder.Entity<User>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasOne(x => x.Role).WithMany(r => r.Users).HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.SetNull);
            b.Property(x => x.FullName).HasColumnType("text");
            b.Property(x => x.PhoneNumber).HasColumnType("text");
            b.Property(x => x.Email).HasColumnType("text");
        });

        modelBuilder.Entity<Department>(b => { b.HasKey(x => x.Id); b.Property(x => x.Name).IsRequired(); });
        modelBuilder.Entity<Specialization>(b => { b.HasKey(x => x.Id); b.Property(x => x.Name).IsRequired(); });

        modelBuilder.Entity<Mechanic>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.SetNull);
            b.HasOne(x => x.Department).WithMany(d => d.Mechanics).HasForeignKey(x => x.DepartmentId).OnDelete(DeleteBehavior.SetNull);
            b.HasOne(x => x.Specialization).WithMany(s => s.Mechanics).HasForeignKey(x => x.SpecializationId).OnDelete(DeleteBehavior.SetNull);
            b.HasMany(x => x.MechanicServices).WithOne(ms => ms.Mechanic).HasForeignKey(ms => ms.MechanicId);
        });

        modelBuilder.Entity<Client>(b => { b.HasKey(x => x.Id); b.Property(x => x.FullName).IsRequired(); });
        modelBuilder.Entity<CarMake>(b => { b.HasKey(x => x.Id); b.Property(x => x.Name).IsRequired(); });
        modelBuilder.Entity<CarCategory>(b => { b.HasKey(x => x.Id); b.Property(x => x.Name).IsRequired(); });

        modelBuilder.Entity<Car>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasOne(x => x.Client).WithMany(c => c.Cars).HasForeignKey(x => x.ClientId).OnDelete(DeleteBehavior.Cascade);
            b.HasOne(x => x.Make).WithMany(m => m.Cars).HasForeignKey(x => x.MakeId).OnDelete(DeleteBehavior.SetNull);
            b.HasOne(x => x.Category).WithMany(c => c.Cars).HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Service>(b => { b.HasKey(x => x.Id); b.Property(x => x.Name).IsRequired(); });

        // Many-to-many: Mechanic <-> Service via MechanicService (explicit)
        modelBuilder.Entity<MechanicService>(b =>
        {
            b.HasKey(ms => new { ms.MechanicId, ms.ServiceId });
            b.HasOne(ms => ms.Mechanic).WithMany(m => m.MechanicServices).HasForeignKey(ms => ms.MechanicId);
            b.HasOne(ms => ms.Service).WithMany(s => s.MechanicServices).HasForeignKey(ms => ms.ServiceId);
        });

        modelBuilder.Entity<RepairBay>(b => { b.HasKey(x => x.Id); b.Property(x => x.Name).IsRequired(); });

        modelBuilder.Entity<MechanicSchedule>(b => { b.HasKey(x => x.Id); b.HasOne(x => x.Mechanic).WithMany().HasForeignKey(x => x.MechanicId); });

        modelBuilder.Entity<Appointment>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasOne(x => x.Client).WithMany().HasForeignKey(x => x.ClientId).OnDelete(DeleteBehavior.SetNull);
            b.HasOne(x => x.Car).WithMany(c => c.Appointments).HasForeignKey(x => x.CarId).OnDelete(DeleteBehavior.SetNull);
            b.HasOne(x => x.Service).WithMany().HasForeignKey(x => x.ServiceId).OnDelete(DeleteBehavior.SetNull);
            b.HasOne(x => x.Mechanic).WithMany().HasForeignKey(x => x.MechanicId).OnDelete(DeleteBehavior.SetNull);
            b.HasOne(x => x.RepairBay).WithMany().HasForeignKey(x => x.RepairBayId).OnDelete(DeleteBehavior.SetNull);
            b.HasOne(x => x.WorkOrder).WithOne(w => w.Appointment).HasForeignKey<WorkOrder>(w => w.AppointmentId).IsRequired(false);
        });

        modelBuilder.Entity<WorkOrder>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasOne(x => x.Mechanic).WithMany(m => m.WorkOrders).HasForeignKey(x => x.MechanicId).OnDelete(DeleteBehavior.SetNull);
            b.HasMany(x => x.WorkOrderServices).WithOne(ws => ws.WorkOrder).HasForeignKey(ws => ws.WorkOrderId);
            b.HasMany(x => x.WorkOrderParts).WithOne(wp => wp.WorkOrder).HasForeignKey(wp => wp.WorkOrderId);
            b.HasOne(x => x.Invoice).WithOne(i => i.WorkOrder).HasForeignKey<Invoice>(i => i.WorkOrderId).IsRequired(false);
        });

        modelBuilder.Entity<WorkOrderService>(b =>
        {
            b.HasKey(x => new { x.WorkOrderId, x.ServiceId });
            b.HasOne(x => x.Service).WithMany(s => s.WorkOrderServices).HasForeignKey(x => x.ServiceId);
        });

        modelBuilder.Entity<Supplier>(b => { b.HasKey(x => x.Id); b.Property(x => x.Name).IsRequired(); });
        modelBuilder.Entity<Part>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasOne(x => x.Supplier).WithMany(s => s.Parts).HasForeignKey(x => x.SupplierId).OnDelete(DeleteBehavior.SetNull);
            b.HasMany(x => x.WorkOrderParts).WithOne(wp => wp.Part).HasForeignKey(wp => wp.PartId);
        });

        modelBuilder.Entity<WorkOrderPart>(b => { b.HasKey(x => new { x.WorkOrderId, x.PartId }); });

        modelBuilder.Entity<Invoice>(b => { b.HasKey(x => x.Id); b.Property(x => x.CreatedAt).HasDefaultValueSql("now()"); });
        modelBuilder.Entity<Payment>(b => { b.HasKey(x => x.Id); b.HasOne(x => x.Invoice).WithMany(i => i.Payments).HasForeignKey(x => x.InvoiceId); });
        modelBuilder.Entity<InspectionResult>(b => { b.HasKey(x => x.Id); b.HasOne(x => x.WorkOrder).WithMany().HasForeignKey(x => x.WorkOrderId); });
        modelBuilder.Entity<Review>(b => { b.HasKey(x => x.Id); b.HasOne(x => x.Client).WithMany(c => c.Reviews).HasForeignKey(x => x.ClientId).OnDelete(DeleteBehavior.SetNull); b.HasOne(x => x.WorkOrder).WithMany().HasForeignKey(x => x.WorkOrderId).OnDelete(DeleteBehavior.SetNull); });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
