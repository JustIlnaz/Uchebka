namespace AutoService.Data;

public class MechanicService
{
    public int MechanicId { get; set; }
    public int ServiceId { get; set; }

    public virtual Mechanic? Mechanic { get; set; }
    public virtual Service? Service { get; set; }
}
