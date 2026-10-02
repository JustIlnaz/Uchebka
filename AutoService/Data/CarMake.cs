using System.Collections.Generic;

namespace AutoService.Data;

public class CarMake
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();
}
