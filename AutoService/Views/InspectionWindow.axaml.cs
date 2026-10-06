using Avalonia.Controls;
using AutoService.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AutoService.Views;

public partial class InspectionWindow : Window
{
    private int _workOrderId;
    private InspectionResult? _existing;

    public InspectionWindow()
    {
        InitializeComponent();
    }

    public void SetWorkOrder(WorkOrder workOrder)
    {
        _workOrderId = workOrder.Id;
        WorkOrderText.Text = $"Заказ-наряд №{_workOrderId}";

        using var db = new AppDbContext();
        _existing = db.InspectionResults.FirstOrDefault(x => x.WorkOrderId == _workOrderId);
        if (_existing != null)
        {
            EngineText.Text = _existing.EngineState;
            BrakesText.Text = _existing.BrakesState;
            SuspensionText.Text = _existing.SuspensionState;
            ElectricalText.Text = _existing.ElectricalState;
            RecommendationsText.Text = _existing.Recommendations;
        }
    }

    private void Save_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        using var db = new AppDbContext();
        var entity = db.InspectionResults.FirstOrDefault(x => x.WorkOrderId == _workOrderId);
        if (entity == null)
        {
            entity = new InspectionResult { WorkOrderId = _workOrderId };
            db.InspectionResults.Add(entity);
        }

        entity.EngineState = EngineText.Text;
        entity.BrakesState = BrakesText.Text;
        entity.SuspensionState = SuspensionText.Text;
        entity.ElectricalState = ElectricalText.Text;
        entity.Recommendations = RecommendationsText.Text;

        db.SaveChanges();
        Close();
    }
}
