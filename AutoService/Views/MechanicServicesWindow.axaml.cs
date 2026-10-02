using Avalonia.Controls;
using AutoService.Data;
using AutoService.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AutoService.Views;

public partial class MechanicServicesWindow : Window
{
    private int _mechanicId;

    public MechanicServicesWindow()
    {
        InitializeComponent();

        var m = MechanicVariableData.SelectedMechanic;
        if (m == null) { Close(); return; }
        _mechanicId = m.Id;

        using var db = new AppDbContext();
        var mech = db.Mechanics.Include(x => x.User).FirstOrDefault(x => x.Id == _mechanicId);
        MechanicNameText.Text = mech?.User?.FullName ?? $"Механик #{_mechanicId}";

        ServiceCombo.ItemsSource = db.Services.ToList();
        LoadAssigned();
    }

    private void LoadAssigned()
    {
        using var db = new AppDbContext();
        var assigned = db.MechanicServices
            .Where(ms => ms.MechanicId == _mechanicId)
            .Include(ms => ms.Service)
            .Select(ms => ms.Service!.Name)
            .ToList();
        AssignedListBox.ItemsSource = assigned;
    }

    private void AddButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var service = ServiceCombo.SelectedItem as Service;
        if (service == null) return;

        using var db = new AppDbContext();
        bool exists = db.MechanicServices.Any(ms => ms.MechanicId == _mechanicId && ms.ServiceId == service.Id);
        if (!exists)
        {
            db.MechanicServices.Add(new MechanicService { MechanicId = _mechanicId, ServiceId = service.Id });
            db.SaveChanges();
        }
        LoadAssigned();
    }

    private void RemoveButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var name = AssignedListBox.SelectedItem as string;
        if (name == null) return;

        using var db = new AppDbContext();
        var service = db.Services.FirstOrDefault(s => s.Name == name);
        if (service == null) return;

        var link = db.MechanicServices.FirstOrDefault(ms => ms.MechanicId == _mechanicId && ms.ServiceId == service.Id);
        if (link != null)
        {
            db.MechanicServices.Remove(link);
            db.SaveChanges();
        }
        LoadAssigned();
    }
}
