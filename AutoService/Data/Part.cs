using System.Collections.Generic;

namespace AutoService.Data;

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
