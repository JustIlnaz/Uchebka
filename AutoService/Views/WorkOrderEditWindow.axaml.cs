using Avalonia.Controls;
using Avalonia.Interactivity;
using AutoService.Data;
using AutoService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;

namespace AutoService.Views;

public partial class WorkOrderEditWindow : Window
{
    private readonly List<WorkOrderService> _tempServices = new();
    private readonly List<WorkOrderPart> _tempParts = new();
    private readonly List<string> _tempServicesDisplay = new();
    private readonly List<string> _tempPartsDisplay = new();
    private readonly Dictionary<int, int> _originalPartQuantities = new();
    private readonly Dictionary<int, string> _serviceNames = new();
    private bool IsMechanic => UserSession.CurrentUser?.RoleName?.Trim().ToLowerInvariant() is "mechanic" or "механик"
        || UserSession.CurrentUser?.RoleId == 2;

    public WorkOrderEditWindow()
    {
        InitializeComponent();

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
        StatusCombo.ItemsSource = IsMechanic
            ? new[] { "Создан", "В работе", "Завершён" }
            : new[] { "Создан", "В работе", "Завершён", "Отменён" };

        var services = db.Services.ToList();
        ServiceCombo.ItemsSource = services;
        _serviceNames = services.ToDictionary(x => x.Id, x => x.Name);
        PartCombo.ItemsSource = db.Parts.ToList();

        if (WorkOrderVariableData.SelectedWorkOrder != null)
        {
            var w = WorkOrderVariableData.SelectedWorkOrder;
            AppointmentCombo.SelectedItem = w.Appointment;
            MechanicCombo.SelectedItem = w.Mechanic;
            StatusCombo.SelectedItem = w.Status;
            TotalCostText.Text = w.TotalCost.ToString("F2");

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
                _originalPartQuantities[p.PartId] = p.Quantity;
                _tempPartsDisplay.Add($"{p.Part?.Sku ?? p.PartId.ToString()} x{p.Quantity}");
            }

            ServicesListBox.ItemsSource = _tempServicesDisplay.ToList();
            PartsListBox.ItemsSource = _tempPartsDisplay.ToList();
        }

        if (IsMechanic)
        {
            AppointmentCombo.IsEnabled = false;
            MechanicCombo.IsEnabled = false;
            TotalCostText.IsReadOnly = true;
        }
    }

    public async void AddService_Click(object? sender, RoutedEventArgs e)
    {
        var service = ServiceCombo.SelectedItem as Service;
        if (service == null || !int.TryParse(ServiceQtyText.Text, out var qty) || qty <= 0)
        {
            await InputDialog.ShowMessage(this, "Выберите услугу и укажите количество больше нуля.");
            return;
        }

        var existing = _tempServices.FirstOrDefault(x => x.ServiceId == service.Id);
        if (existing == null)
            _tempServices.Add(new WorkOrderService { ServiceId = service.Id, Quantity = qty, Price = service.BasePrice });
        else
            existing.Quantity += qty;

        _tempServicesDisplay.Clear();
        _tempServicesDisplay.AddRange(_tempServices.Select(x =>
            $"{_serviceNames.GetValueOrDefault(x.ServiceId, x.ServiceId.ToString())} x{x.Quantity}"));
        ServicesListBox.ItemsSource = _tempServicesDisplay.ToList();
    }

    public async void AddPart_Click(object? sender, RoutedEventArgs e)
    {
        var part = PartCombo.SelectedItem as Part;
        if (part == null || !int.TryParse(PartQtyText.Text, out var qty) || qty <= 0)
        {
            await InputDialog.ShowMessage(this, "Выберите запчасть и укажите количество больше нуля.");
            return;
        }

        using var db = new AppDbContext();
        var currentPart = db.Parts.FirstOrDefault(x => x.Id == part.Id);
        if (currentPart == null)
        {
            await InputDialog.ShowMessage(this, "Запчасть не найдена на складе.");
            return;
        }

        var existing = _tempParts.FirstOrDefault(x => x.PartId == part.Id);
        var requestedQuantity = (existing?.Quantity ?? 0) + qty;
        var availableQuantity = currentPart.QuantityInStock + _originalPartQuantities.GetValueOrDefault(part.Id);
        if (requestedQuantity > availableQuantity)
        {
            await InputDialog.ShowMessage(this, $"Недостаточно запчасти на складе. Доступно: {availableQuantity} шт.");
            return;
        }

        if (existing == null)
            _tempParts.Add(new WorkOrderPart { PartId = part.Id, Quantity = qty, Price = currentPart.SalePrice });
        else
            existing.Quantity += qty;

        _tempPartsDisplay.Clear();
        _tempPartsDisplay.AddRange(_tempParts.Select(x =>
            $"{db.Parts.FirstOrDefault(p => p.Id == x.PartId)?.Sku ?? x.PartId.ToString()} x{x.Quantity}"));
        PartsListBox.ItemsSource = _tempPartsDisplay.ToList();
    }

    public async void Save_Click(object? sender, RoutedEventArgs e)
    {
        var appointment = AppointmentCombo.SelectedItem as Appointment;
        var mechanic = MechanicCombo.SelectedItem as Mechanic;
        var status = StatusCombo.SelectedItem as string;

        if (IsMechanic && WorkOrderVariableData.SelectedWorkOrder == null)
        {
            await InputDialog.ShowMessage(this, "Механик может изменять только назначенный ему заказ-наряд.");
            return;
        }

        if (appointment == null || mechanic == null || string.IsNullOrWhiteSpace(status))
        {
            await InputDialog.ShowMessage(this, "Выберите запись, механика и статус заказа.");
            return;
        }

        var isNew = WorkOrderVariableData.SelectedWorkOrder == null;
        if (isNew && !_tempServices.Any() && !_tempParts.Any())
        {
            await InputDialog.ShowMessage(this, "Добавьте в заказ хотя бы одну услугу или запчасть.");
            return;
        }

        using var db = new AppDbContext();
        using var transaction = db.Database.BeginTransaction();
        try
        {
            WorkOrder? workOrder;
            List<WorkOrderPart> oldParts;
            List<WorkOrderService> oldServices;
            if (!isNew)
            {
                var id = WorkOrderVariableData.SelectedWorkOrder!.Id;
                workOrder = db.WorkOrders.Include(x => x.Mechanic).FirstOrDefault(x => x.Id == id);
                if (workOrder == null)
                {
                    await InputDialog.ShowMessage(this, "Заказ-наряд не найден.");
                    return;
                }

                if (IsMechanic && workOrder.Mechanic?.UserId != UserSession.CurrentUser?.Id)
                {
                    await InputDialog.ShowMessage(this, "Нет доступа к этому заказ-наряду.");
                    return;
                }

                oldParts = db.WorkOrderParts.Where(x => x.WorkOrderId == id).ToList();
                oldServices = db.WorkOrderServices.Where(x => x.WorkOrderId == id).ToList();
            }
            else
            {
                workOrder = new WorkOrder();
                oldParts = new List<WorkOrderPart>();
                oldServices = new List<WorkOrderService>();
            }

            if (db.WorkOrders.Any(x => x.AppointmentId == appointment.Id && x.Id != workOrder.Id))
            {
                await InputDialog.ShowMessage(this, "Для этой записи уже создан заказ-наряд.");
                return;
            }

            var inventoryIds = oldParts.Select(x => x.PartId).Concat(_tempParts.Select(x => x.PartId)).Distinct().ToList();
            var inventory = db.Parts.Where(x => inventoryIds.Contains(x.Id)).ToDictionary(x => x.Id);
            foreach (var oldPart in oldParts)
            {
                if (inventory.TryGetValue(oldPart.PartId, out var oldInventoryPart))
                    oldInventoryPart.QuantityInStock += oldPart.Quantity;
            }

            foreach (var requestedPart in _tempParts)
            {
                if (!inventory.TryGetValue(requestedPart.PartId, out var inventoryPart)
                    || requestedPart.Quantity <= 0 || inventoryPart.QuantityInStock < requestedPart.Quantity)
                {
                    var available = inventoryPart?.QuantityInStock ?? 0;
                    await InputDialog.ShowMessage(this, $"Недостаточно запчасти на складе. Доступно: {available} шт.");
                    return;
                }
            }

            if (!IsMechanic)
            {
                workOrder.AppointmentId = appointment.Id;
                workOrder.MechanicId = mechanic.Id;
            }
            workOrder.Status = status;
            if (_tempServices.Any() || _tempParts.Any())
            {
                workOrder.TotalCost = _tempServices.Sum(s => s.Price * s.Quantity)
                    + _tempParts.Sum(p => p.Price * p.Quantity);
            }
            if (isNew) db.WorkOrders.Add(workOrder);

            db.WorkOrderServices.RemoveRange(oldServices);
            db.WorkOrderParts.RemoveRange(oldParts);
            db.SaveChanges();

            foreach (var serviceLine in _tempServices)
            {
                db.WorkOrderServices.Add(new WorkOrderService
                {
                    WorkOrderId = workOrder.Id,
                    ServiceId = serviceLine.ServiceId,
                    Quantity = serviceLine.Quantity,
                    Price = serviceLine.Price
                });
            }

            foreach (var partLine in _tempParts)
            {
                inventory[partLine.PartId].QuantityInStock -= partLine.Quantity;
                db.WorkOrderParts.Add(new WorkOrderPart
                {
                    WorkOrderId = workOrder.Id,
                    PartId = partLine.PartId,
                    Quantity = partLine.Quantity,
                    Price = partLine.Price
                });
            }

            db.SaveChanges();
            transaction.Commit();
            Close();
        }
        catch (Exception)
        {
            await InputDialog.ShowMessage(this, "Не удалось сохранить заказ-наряд. Проверьте данные и наличие запчастей на складе.");
        }
    }
}
