using System.Collections.Generic;

namespace AutoService.Data;

public class Car
{
    public int Id { get; set; }
    public int? ClientId { get; set; }
    public int? MakeId { get; set; }
    public int? CategoryId { get; set; }

    public string? Model { get; set; }
    public int? Year { get; set; }
    public string? Vin { get; set; }
    public string? RegistrationNumber { get; set; }
    public int? Mileage { get; set; }

    public virtual Client? Client { get; set; }
    public virtual CarMake? Make { get; set; }
    public virtual CarCategory? Category { get; set; }
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
