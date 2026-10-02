using System.Collections.Generic;

namespace AutoService.Data;

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
