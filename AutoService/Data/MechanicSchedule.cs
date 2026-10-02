using System;

namespace AutoService.Data;

public class MechanicSchedule
{
    public int Id { get; set; }
    public int MechanicId { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }

    public virtual Mechanic? Mechanic { get; set; }
}
