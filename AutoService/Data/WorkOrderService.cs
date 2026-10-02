namespace AutoService.Data;

public class WorkOrderService
{
    public int WorkOrderId { get; set; }
    public int ServiceId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }

    public virtual WorkOrder? WorkOrder { get; set; }
    public virtual Service? Service { get; set; }
}
