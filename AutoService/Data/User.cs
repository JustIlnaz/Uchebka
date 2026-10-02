using System.Collections.Generic;

namespace AutoService.Data;

public class User
{
    public int Id { get; set; }
    public string? FullName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }

    // Keep RoleId for backwards compatibility with DB relations,
    // but also expose a simple string role field as requested.
    public int? RoleId { get; set; }
    public string? RoleName { get; set; }

    // Simple authentication fields (no hashing as requested)
    public string? Username { get; set; }
    public string? Password { get; set; }

    public virtual Role? Role { get; set; }
    public virtual ICollection<Login> Logins { get; set; } = new List<Login>();
}
