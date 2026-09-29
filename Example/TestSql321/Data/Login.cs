using System;
using System.Collections.Generic;

namespace TestSql321.Data;

public partial class Login
{
    public int IdLogin { get; set; }

    public string? Login1 { get; set; }

    public string? Password { get; set; }

    public int? UserId { get; set; }

    public virtual User? User { get; set; }
}
