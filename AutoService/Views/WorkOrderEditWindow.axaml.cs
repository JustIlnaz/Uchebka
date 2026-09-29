using Avalonia.Controls;
using AutoService.Data;
using AutoService.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AutoService.Views;

public partial class WorkOrderEditWindow : Window
{
    public WorkOrderEditWindow()
    {
        InitializeComponent();

        using var db = new AppDbContext();
        AppointmentCombo.ItemsSource = db.Appointments
            .Include(a => a.Car)
            .Include(a => a.Client)
            .ToList();
        MechanicCombo.ItemsSource = db.Mechanics.Include(m => m.User).ToList();
        StatusCombo.ItemsSource = new[] { "Создан", "В работе", "Завершён", "Отменён" };

        if (WorkOrderVariableData.SelectedWorkOrder != null)
        {
            var w = WorkOrderVariableData.SelectedWorkOrder;
            AppointmentCombo.SelectedItem = w.Appointment;
            MechanicCombo.SelectedItem = w.Mechanic;
            StatusCombo.SelectedItem = w.Status;
            TotalCostText.Text = w.TotalCost.ToString("F2");
        }
    }

    private void Save_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        using var db = new AppDbContext();

        var appointment = AppointmentCombo.SelectedItem as Appointment;
        var mechanic = MechanicCombo.SelectedItem as Mechanic;
        var status = StatusCombo.SelectedItem as string;
        decimal.TryParse(TotalCostText.Text, out var totalCost);

        if (WorkOrderVariableData.SelectedWorkOrder != null)
        {
            var id = WorkOrderVariableData.SelectedWorkOrder.Id;
            var thisWorkOrder = db.WorkOrders.FirstOrDefault(x => x.Id == id);
            if (thisWorkOrder != null)
            {
                thisWorkOrder.AppointmentId = appointment?.Id;
                thisWorkOrder.MechanicId = mechanic?.Id;
                thisWorkOrder.Status = status;
                thisWorkOrder.TotalCost = totalCost;
            }
        }
        else
        {
            var newWorkOrder = new WorkOrder
            {
                AppointmentId = appointment?.Id,
                MechanicId = mechanic?.Id,
                Status = status ?? "Создан",
                TotalCost = totalCost,
            };
            db.WorkOrders.Add(newWorkOrder);
        }

        db.SaveChanges();
        this.Close();
    }
}
