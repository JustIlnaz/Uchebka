namespace AutoService.Data;

public class WorkOrderPart
{
    public int WorkOrderId { get; set; }
    public int PartId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }

    public virtual WorkOrder? WorkOrder { get; set; }
    public virtual Part? Part { get; set; }
}
