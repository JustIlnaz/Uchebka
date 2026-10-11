using System.Collections.Generic;

namespace AutoService.Data;

public class User
{
    public int Id { get; set; }
    public string? FullName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }

    public int? RoleId { get; set; }
    public string? RoleName { get; set; }

    public string? Username { get; set; }
    public string? Password { get; set; }

    public virtual Role? Role { get; set; }
    public virtual ICollection<Login> Logins { get; set; } = new List<Login>();

    public override string ToString() => FullName ?? Username ?? $"Пользователь #{Id}";
}
