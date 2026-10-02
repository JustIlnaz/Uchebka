using System;
using System.Collections.Generic;

namespace AutoService.Data;

public class Client
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Email { get; set; }

    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
