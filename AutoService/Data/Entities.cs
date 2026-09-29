using System;
using System.Collections.Generic;

namespace AutoService.Data;

// Простые сущности предметной области автосервиса

public class Role
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}

public class User
{
    public int Id { get; set; }
    public string? FullName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public int? RoleId { get; set; }

    // Simple authentication fields
    public string? Username { get; set; }
    public string? PasswordHash { get; set; }

    public virtual Role? Role { get; set; }
    public virtual ICollection<Login> Logins { get; set; } = new List<Login>();
}

public class Login
{
    public int Id { get; set; }
    public string? LoginName { get; set; }
    public string? Password { get; set; }
    public int? UserId { get; set; }

    public virtual User? User { get; set; }
}

public class Department
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public virtual ICollection<Mechanic> Mechanics { get; set; } = new List<Mechanic>();
}

public class Specialization
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public virtual ICollection<Mechanic> Mechanics { get; set; } = new List<Mechanic>();
}

public class Mechanic
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public int? DepartmentId { get; set; }
    public int? SpecializationId { get; set; }

    public virtual User? User { get; set; }
    public virtual Department? Department { get; set; }
    public virtual Specialization? Specialization { get; set; }
    public virtual ICollection<MechanicService> MechanicServices { get; set; } = new List<MechanicService>();
    public virtual ICollection<WorkOrder> WorkOrders { get; set; } = new List<WorkOrder>();
}

public class Client
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Email { get; set; }

    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}

public class CarMake
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();
}

public class CarCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();
}

public class Car
{
    public int Id { get; set; }
    public int? ClientId { get; set; }
    public int? MakeId { get; set; }
    public int? CategoryId { get; set; }

    public string? Model { get; set; }
    public int? Year { get; set; }
    public string? Vin { get; set; }
    public string? RegistrationNumber { get; set; }
    public int? Mileage { get; set; }

    public virtual Client? Client { get; set; }
    public virtual CarMake? Make { get; set; }
    public virtual CarCategory? Category { get; set; }
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}

public class Service
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal BasePrice { get; set; }
    public int DurationMinutes { get; set; }

    public virtual ICollection<MechanicService> MechanicServices { get; set; } = new List<MechanicService>();
    public virtual ICollection<WorkOrderService> WorkOrderServices { get; set; } = new List<WorkOrderService>();
}

public class MechanicService
{
    public int MechanicId { get; set; }
    public int ServiceId { get; set; }

    public virtual Mechanic? Mechanic { get; set; }
    public virtual Service? Service { get; set; }
}

public class RepairBay
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}

public class MechanicSchedule
{
    public int Id { get; set; }
    public int MechanicId { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }

    public virtual Mechanic? Mechanic { get; set; }
}

public class Appointment
{
    public int Id { get; set; }
    public int? ClientId { get; set; }
    public int? CarId { get; set; }
    public int? ServiceId { get; set; }
    public DateTime ScheduledAt { get; set; }
    public int? MechanicId { get; set; }
    public int? RepairBayId { get; set; }
    public string? Status { get; set; }

    public virtual Client? Client { get; set; }
    public virtual Car? Car { get; set; }
    public virtual Service? Service { get; set; }
    public virtual Mechanic? Mechanic { get; set; }
    public virtual RepairBay? RepairBay { get; set; }
    public virtual WorkOrder? WorkOrder { get; set; }
}

public class WorkOrder
{
    public int Id { get; set; }
    public int? AppointmentId { get; set; }
    public int? MechanicId { get; set; }
    public string? Status { get; set; }
    public decimal TotalCost { get; set; }

    public virtual Appointment? Appointment { get; set; }
    public virtual Mechanic? Mechanic { get; set; }
    public virtual ICollection<WorkOrderService> WorkOrderServices { get; set; } = new List<WorkOrderService>();
    public virtual ICollection<WorkOrderPart> WorkOrderParts { get; set; } = new List<WorkOrderPart>();
    public virtual Invoice? Invoice { get; set; }
}

public class WorkOrderService
{
    public int WorkOrderId { get; set; }
    public int ServiceId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }

    public virtual WorkOrder? WorkOrder { get; set; }
    public virtual Service? Service { get; set; }
}

public class Supplier
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? ContactInfo { get; set; }

    public virtual ICollection<Part> Parts { get; set; } = new List<Part>();
}

public class Part
{
    public int Id { get; set; }
    public int? SupplierId { get; set; }
    public string? Sku { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal SalePrice { get; set; }
    public int QuantityInStock { get; set; }
    public int MinQuantity { get; set; }

    public virtual Supplier? Supplier { get; set; }
    public virtual ICollection<WorkOrderPart> WorkOrderParts { get; set; } = new List<WorkOrderPart>();
}

public class WorkOrderPart
{
    public int WorkOrderId { get; set; }
    public int PartId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }

    public virtual WorkOrder? WorkOrder { get; set; }
    public virtual Part? Part { get; set; }
}

public class Invoice
{
    public int Id { get; set; }
    public int? WorkOrderId { get; set; }
    public DateTime CreatedAt { get; set; }
    public decimal AmountBeforeDiscount { get; set; }
    public decimal Discount { get; set; }
    public decimal Tax { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Status { get; set; }

    public virtual WorkOrder? WorkOrder { get; set; }
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}

public class Payment
{
    public int Id { get; set; }
    public int? InvoiceId { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string? Method { get; set; }
    public string? Status { get; set; }
    public string? TransactionNumber { get; set; }

    public virtual Invoice? Invoice { get; set; }
}

public class InspectionResult
{
    public int Id { get; set; }
    public int? WorkOrderId { get; set; }
    public string? EngineState { get; set; }
    public string? BrakesState { get; set; }
    public string? SuspensionState { get; set; }
    public string? ElectricalState { get; set; }
    public string? Recommendations { get; set; }

    public virtual WorkOrder? WorkOrder { get; set; }
}

public class Review
{
    public int Id { get; set; }
    public int? ClientId { get; set; }
    public int? WorkOrderId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }

    public virtual Client? Client { get; set; }
    public virtual WorkOrder? WorkOrder { get; set; }
}
