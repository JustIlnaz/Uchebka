using System.Collections.Generic;

namespace AutoService.Data;

public class Role
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
