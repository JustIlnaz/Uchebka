using Avalonia.Controls;
using AutoService.Data;
using AutoService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace AutoService.Views;

public partial class WorkOrderManagementWindow : Window
{
    private int _page = 0;
    private int _pageSize = 10;
    private int _totalCount = 0;

    public WorkOrderManagementWindow()
    {
        InitializeComponent();
        PageSizeCombo.SelectedIndex = 0;
        StatusCombo.ItemsSource = new[] { "Новый", "В работе", "Завершён", "Отменён" };
        LoadMechanics();
        LoadWorkOrders();
    }

    private void LoadMechanics()
    {
        try
        {
            using var db = new AppDbContext();
            MechanicCombo.ItemsSource = db.Mechanics.Include(m => m.User).ToList();
        }
        catch
        {
        }
    }

    private IQueryable<WorkOrder> BuildQuery(AppDbContext db)
    {
        IQueryable<WorkOrder> q = db.WorkOrders
            .Include(w => w.Appointment).ThenInclude(a => a.Car)
            .Include(w => w.Mechanic).ThenInclude(m => m.User);

        if (StatusCombo.SelectedItem is string status && !string.IsNullOrWhiteSpace(status))
            q = q.Where(w => w.Status == status);

        if (MechanicCombo.SelectedItem is Mechanic mechanic)
            q = q.Where(w => w.MechanicId == mechanic.Id);

        return q;
    }

    private void LoadWorkOrders()
    {
        try
        {
            using var db = new AppDbContext();
            var q = BuildQuery(db);
            _totalCount = q.Count();
            WorkOrdersListBox.ItemsSource = q
                .OrderByDescending(w => w.Id)
                .Skip(_page * _pageSize)
                .Take(_pageSize)
                .ToList();
            UpdatePageInfo();
        }
        catch
        {
        }
    }

    private void UpdatePageInfo()
    {
        var totalPages = Math.Max(1, (int)Math.Ceiling(_totalCount / (double)_pageSize));
        PageInfoText.Text = $"Стр. {_page + 1} из {totalPages} ({_totalCount} записей)";
    }

    private void Filter_SelectionChanged(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _page = 0;
        LoadWorkOrders();
    }

    private void ResetFilter_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        StatusCombo.SelectedItem = null;
        MechanicCombo.SelectedItem = null;
        _page = 0;
        LoadWorkOrders();
    }

    private void PrevPage_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (_page > 0) { _page--; LoadWorkOrders(); }
    }

    private void NextPage_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var totalPages = (int)Math.Ceiling(_totalCount / (double)_pageSize);
        if (_page < totalPages - 1) { _page++; LoadWorkOrders(); }
    }

    private void PageSize_Changed(object? sender, SelectionChangedEventArgs e)
    {
        if (PageSizeCombo.SelectedItem is ComboBoxItem item &&
            int.TryParse(item.Content?.ToString(), out var size))
        {
            _pageSize = size;
            _page = 0;
            LoadWorkOrders();
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

    private async void InvoiceButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var workOrder = WorkOrdersListBox.SelectedItem as WorkOrder;
        if (workOrder == null) return;

        var win = new InvoiceWindow();
        win.SetWorkOrder(workOrder);
        await win.ShowDialog(this);
        LoadWorkOrders();
    }

    private async void InspectionButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var workOrder = WorkOrdersListBox.SelectedItem as WorkOrder;
        if (workOrder == null) return;

        var win = new InspectionWindow();
        win.SetWorkOrder(workOrder);
        await win.ShowDialog(this);
        LoadWorkOrders();
    }
}
