using System.Collections.Generic;

namespace AutoService.Data;

public class Supplier
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? ContactInfo { get; set; }

    public virtual ICollection<Part> Parts { get; set; } = new List<Part>();
}
