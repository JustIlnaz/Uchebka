using System;
using System.Collections.Generic;

namespace TestSql321.Data;

public partial class User
{
    public int IdUser { get; set; }

    public string? FullName { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Login> Logins { get; set; } = new List<Login>();
}
