using System;

namespace AutoService.Data;

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
