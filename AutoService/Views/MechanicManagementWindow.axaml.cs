using Avalonia.Controls;
using AutoService.Data;
using AutoService.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AutoService.Views;

public partial class MechanicManagementWindow : Window
{
    public MechanicManagementWindow()
    {
        InitializeComponent();
        LoadMechanics();
    }

    private void LoadMechanics()
    {
        using var db = new AppDbContext();
        MechanicsListBox.ItemsSource = db.Mechanics
            .Include(m => m.User)
            .Include(m => m.Department)
            .Include(m => m.Specialization)
            .ToList();
    }

    private async void AddButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        MechanicVariableData.SelectedMechanic = null;
        var win = new MechanicEditWindow();
        await win.ShowDialog(this);
        LoadMechanics();
    }

    private async void ListBox_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        var m = MechanicsListBox.SelectedItem as Mechanic;
        if (m == null) return;

        MechanicVariableData.SelectedMechanic = m;
        var win = new MechanicEditWindow();
        await win.ShowDialog(this);
        MechanicVariableData.SelectedMechanic = null;
        LoadMechanics();
    }

    private async void ServicesButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var m = MechanicsListBox.SelectedItem as Mechanic;
        if (m == null) return;

        MechanicVariableData.SelectedMechanic = m;
        var win = new MechanicServicesWindow();
        await win.ShowDialog(this);
        MechanicVariableData.SelectedMechanic = null;
    }

    private async void ScheduleButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var m = MechanicsListBox.SelectedItem as Mechanic;
        if (m == null) return;

        MechanicVariableData.SelectedMechanic = m;
        var win = new MechanicScheduleWindow();
        await win.ShowDialog(this);
        MechanicVariableData.SelectedMechanic = null;
    }

    private void DeleteButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var m = MechanicsListBox.SelectedItem as Mechanic;
        if (m == null) return;

        using var db = new AppDbContext();
        var mech = db.Mechanics.FirstOrDefault(x => x.Id == m.Id);
        if (mech != null)
        {
            db.Mechanics.Remove(mech);
            db.SaveChanges();
        }
        LoadMechanics();
    }
}
