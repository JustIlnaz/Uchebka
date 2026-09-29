using Avalonia.Controls;
using Avalonia.Interactivity;
using AutoService.Data;
using AutoService.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;

namespace AutoService.Views;

public partial class WorkOrderEditWindow : Window
{
    // Temporary lists to hold services and parts before saving
    private readonly List<WorkOrderService> _tempServices = new();
    private readonly List<WorkOrderPart> _tempParts = new();
    private readonly List<string> _tempServicesDisplay = new();
    private readonly List<string> _tempPartsDisplay = new();

    public WorkOrderEditWindow()
    {
        InitializeComponent();

        // create buttons in code to avoid XAML Click parsing problem
        var addServiceBtn = new Button { Content = "Добавить услугу" };
        addServiceBtn.Click += AddService_Click;
        ServicePanel.Children.Add(addServiceBtn);

        var addPartBtn = new Button { Content = "Добавить запчасть" };
        addPartBtn.Click += AddPart_Click;
        PartPanel.Children.Add(addPartBtn);

        using var db = new AppDbContext();
        AppointmentCombo.ItemsSource = db.Appointments
            .Include(a => a.Car)
            .Include(a => a.Client)
            .ToList();
        MechanicCombo.ItemsSource = db.Mechanics.Include(m => m.User).ToList();
        StatusCombo.ItemsSource = new[] { "Создан", "В работе", "Завершён", "Отменён" };

        // Populate services and parts lists for selection
        ServiceCombo.ItemsSource = db.Services.ToList();
        PartCombo.ItemsSource = db.Parts.ToList();

        if (WorkOrderVariableData.SelectedWorkOrder != null)
        {
            var w = WorkOrderVariableData.SelectedWorkOrder;
            AppointmentCombo.SelectedItem = w.Appointment;
            MechanicCombo.SelectedItem = w.Mechanic;
            StatusCombo.SelectedItem = w.Status;
            TotalCostText.Text = w.TotalCost.ToString("F2");

            // Load existing services and parts into temp lists for editing
            var existingServices = db.WorkOrderServices.Where(x => x.WorkOrderId == w.Id).Include(x => x.Service).ToList();
            foreach (var s in existingServices)
            {
                _tempServices.Add(new WorkOrderService { ServiceId = s.ServiceId, Quantity = s.Quantity, Price = s.Price });
                _tempServicesDisplay.Add($"{s.Service?.Name ?? s.ServiceId.ToString()} x{s.Quantity}");
            }

            var existingParts = db.WorkOrderParts.Where(x => x.WorkOrderId == w.Id).Include(x => x.Part).ToList();
            foreach (var p in existingParts)
            {
                _tempParts.Add(new WorkOrderPart { PartId = p.PartId, Quantity = p.Quantity, Price = p.Price });
                _tempPartsDisplay.Add($"{p.Part?.Sku ?? p.PartId.ToString()} x{p.Quantity}");
            }

            ServicesListBox.ItemsSource = _tempServicesDisplay.ToList();
            PartsListBox.ItemsSource = _tempPartsDisplay.ToList();
        }
    }

    public void AddService_Click(object sender, RoutedEventArgs e)
    {
        var service = ServiceCombo.SelectedItem as Service;
        if (service == null) return;
        var qty = int.TryParse(ServiceQtyText.Text, out var q) ? q : 1;

        _tempServices.Add(new WorkOrderService { ServiceId = service.Id, Quantity = qty, Price = service.BasePrice });
        _tempServicesDisplay.Add($"{service.Name} x{qty}");
        ServicesListBox.ItemsSource = _tempServicesDisplay.ToList();
    }

    public void AddPart_Click(object sender, RoutedEventArgs e)
    {
        var part = PartCombo.SelectedItem as Part;
        if (part == null) return;
        var qty = int.TryParse(PartQtyText.Text, out var q) ? q : 1;

        _tempParts.Add(new WorkOrderPart { PartId = part.Id, Quantity = qty, Price = part.SalePrice });
        _tempPartsDisplay.Add($"{part.Sku ?? part.Id.ToString()} x{qty}");
        PartsListBox.ItemsSource = _tempPartsDisplay.ToList();
    }

    public void Save_Click(object sender, RoutedEventArgs e)
    {
        using var db = new AppDbContext();

        var appointment = AppointmentCombo.SelectedItem as Appointment;
        var mechanic = MechanicCombo.SelectedItem as Mechanic;
        var status = StatusCombo.SelectedItem as string;

        // Compute total cost from selected services and parts
        var computedTotal = _tempServices.Sum(s => s.Price * s.Quantity) + _tempParts.Sum(p => p.Price * p.Quantity);
        decimal.TryParse(TotalCostText.Text, out var enteredTotal);
        var totalCost = computedTotal > 0 ? computedTotal : enteredTotal;

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

                // Replace existing services and parts
                var oldServices = db.WorkOrderServices.Where(x => x.WorkOrderId == id).ToList();
                if (oldServices.Any()) db.WorkOrderServices.RemoveRange(oldServices);

                var oldParts = db.WorkOrderParts.Where(x => x.WorkOrderId == id).ToList();
                if (oldParts.Any()) db.WorkOrderParts.RemoveRange(oldParts);

                // Add new
                foreach (var s in _tempServices)
                {
                    s.WorkOrderId = id;
                    db.WorkOrderServices.Add(s);
                }
                foreach (var p in _tempParts)
                {
                    p.WorkOrderId = id;
                    db.WorkOrderParts.Add(p);
                }
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
            db.SaveChanges(); // Save to get Id

            // Add services and parts with new WorkOrderId
            foreach (var s in _tempServices)
            {
                s.WorkOrderId = newWorkOrder.Id;
                db.WorkOrderServices.Add(s);
            }
            foreach (var p in _tempParts)
            {
                p.WorkOrderId = newWorkOrder.Id;
                db.WorkOrderParts.Add(p);
            }
        }

        db.SaveChanges();
        this.Close();
    }
}
