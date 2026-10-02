namespace AutoService.Data;

public class InspectionResult
{
    public int Id { get; set; }
    public int? WorkOrderId { get; set; }
    public string? EngineState { get; set; }
    public string? BrakesState { get; set; }
    public string? SuspensionState { get; set; }
    public string? ElectricalState { get; set; }
    public string? Recommendations { get; set; }

    public virtual WorkOrder? WorkOrder { get; set; }
}
