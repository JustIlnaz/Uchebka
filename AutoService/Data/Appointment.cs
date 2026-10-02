using System;

namespace AutoService.Data;

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
