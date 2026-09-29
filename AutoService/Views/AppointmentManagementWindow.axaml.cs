using Avalonia.Controls;
using AutoService.Data;
using AutoService.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AutoService.Views;

public partial class AppointmentManagementWindow : Window
{
    public AppointmentManagementWindow()
    {
        InitializeComponent();
        LoadAppointments();
    }

    private void LoadAppointments()
    {
        try
        {
            using var db = new AppDbContext();
            var appointments = db.Appointments
                .Include(a => a.Client)
                .Include(a => a.Car)
                .Include(a => a.Service)
                .ToList();
            AppointmentsListBox.ItemsSource = appointments;
        }
        catch
        {
        }
    }

    private async void AddButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var win = new AppointmentEditWindow();
        await win.ShowDialog(this);
        LoadAppointments();
    }

    private async void ListBox_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        var appointment = AppointmentsListBox.SelectedItem as Appointment;
        if (appointment == null) return;

        AppointmentVariableData.SelectedAppointment = appointment;

        var win = new AppointmentEditWindow();
        await win.ShowDialog(this);

        AppointmentVariableData.SelectedAppointment = null;
        LoadAppointments();
    }
}
