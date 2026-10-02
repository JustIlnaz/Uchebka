using System;
using System.Collections.Generic;

namespace AutoService.Data;

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
