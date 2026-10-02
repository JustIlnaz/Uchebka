using System.Collections.Generic;

namespace AutoService.Data;

public class Department
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public virtual ICollection<Mechanic> Mechanics { get; set; } = new List<Mechanic>();
}
