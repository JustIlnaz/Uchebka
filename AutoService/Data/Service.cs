using System.Collections.Generic;

namespace AutoService.Data;

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
