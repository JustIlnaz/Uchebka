using System.Collections.Generic;

namespace AutoService.Data;

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
