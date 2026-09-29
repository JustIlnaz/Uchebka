using Avalonia.Controls;
using AutoService.Data;
using AutoService.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AutoService.Views;

public partial class WorkOrderManagementWindow : Window
{
    public WorkOrderManagementWindow()
    {
        InitializeComponent();
        LoadWorkOrders();
    }

    private void LoadWorkOrders()
    {
        try
        {
            using var db = new AppDbContext();
            var workOrders = db.WorkOrders
                .Include(w => w.Appointment).ThenInclude(a => a.Car)
                .Include(w => w.Mechanic).ThenInclude(m => m.User)
                .ToList();
            WorkOrdersListBox.ItemsSource = workOrders;
        }
        catch
        {
        }
    }

    private async void AddButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var win = new WorkOrderEditWindow();
        await win.ShowDialog(this);
        LoadWorkOrders();
    }

    private async void ListBox_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        var workOrder = WorkOrdersListBox.SelectedItem as WorkOrder;
        if (workOrder == null) return;

        WorkOrderVariableData.SelectedWorkOrder = workOrder;

        var win = new WorkOrderEditWindow();
        await win.ShowDialog(this);

        WorkOrderVariableData.SelectedWorkOrder = null;
        LoadWorkOrders();
    }
}
