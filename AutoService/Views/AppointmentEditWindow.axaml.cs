using Avalonia.Controls;
using AutoService.Data;
using AutoService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace AutoService.Views;

public partial class AppointmentEditWindow : Window
{
    public AppointmentEditWindow()
    {
        InitializeComponent();

        using var db = new AppDbContext();
        ClientCombo.ItemsSource = db.Clients.ToList();
        CarCombo.ItemsSource = db.Cars.Include(c => c.Make).ToList();
        ServiceCombo.ItemsSource = db.Services.ToList();
        MechanicCombo.ItemsSource = db.Mechanics.Include(m => m.User).ToList();
        RepairBayCombo.ItemsSource = db.RepairBays.ToList();
        StatusCombo.ItemsSource = new[] { "Запланирована", "Подтверждена", "Отменена", "Выполнена" };

        if (AppointmentVariableData.SelectedAppointment != null)
        {
            var a = AppointmentVariableData.SelectedAppointment;
            ClientCombo.SelectedItem = a.Client;
            CarCombo.SelectedItem = a.Car;
            ServiceCombo.SelectedItem = a.Service;
            MechanicCombo.SelectedItem = a.Mechanic;
            RepairBayCombo.SelectedItem = a.RepairBay;
            ScheduledDatePicker.SelectedDate = new DateTimeOffset(a.ScheduledAt.Date);
            ScheduledTimePicker.SelectedTime = a.ScheduledAt.TimeOfDay;
            StatusCombo.SelectedItem = a.Status;
        }
    }

    private async void Save_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        using var db = new AppDbContext();

        var client = ClientCombo.SelectedItem as Client;
        var car = CarCombo.SelectedItem as Car;
        var service = ServiceCombo.SelectedItem as Service;
        var mechanic = MechanicCombo.SelectedItem as Mechanic;
        var repairBay = RepairBayCombo.SelectedItem as RepairBay;
        var status = StatusCombo.SelectedItem as string;
        if (client == null || car == null || service == null || mechanic == null || repairBay == null)
        {
            await InputDialog.ShowMessage(this, "Выберите клиента, автомобиль, услугу, механика и ремонтное место.");
            return;
        }

        if (ScheduledDatePicker.SelectedDate is not DateTimeOffset selectedDate || ScheduledTimePicker.SelectedTime is not TimeSpan selectedTime)
        {
            await InputDialog.ShowMessage(this, "Выберите дату и время обслуживания.");
            return;
        }

        var scheduledAt = DateTime.SpecifyKind(selectedDate.Date.Date + selectedTime, DateTimeKind.Utc);
        var appointmentId = AppointmentVariableData.SelectedAppointment?.Id ?? 0;
        var hasConflict = db.Appointments.Any(a => a.Id != appointmentId
            && a.Status != "Отменена"
            && (a.MechanicId == mechanic.Id || a.RepairBayId == repairBay.Id)
            && a.ScheduledAt == scheduledAt);
        if (hasConflict)
        {
            await InputDialog.ShowMessage(this, "На выбранные дату и время механик или ремонтное место уже заняты.");
            return;
        }

        if (AppointmentVariableData.SelectedAppointment != null)
        {
            var id = AppointmentVariableData.SelectedAppointment.Id;
            var thisAppointment = db.Appointments.FirstOrDefault(x => x.Id == id);
            if (thisAppointment != null)
            {
                thisAppointment.ClientId = client.Id;
                thisAppointment.CarId = car.Id;
                thisAppointment.ServiceId = service.Id;
                thisAppointment.MechanicId = mechanic.Id;
                thisAppointment.RepairBayId = repairBay.Id;
                thisAppointment.ScheduledAt = scheduledAt;
                thisAppointment.Status = status ?? "Запланирована";
            }
        }
        else
        {
            var newAppointment = new Appointment
            {
                ClientId = client.Id,
                CarId = car.Id,
                ServiceId = service.Id,
                MechanicId = mechanic.Id,
                RepairBayId = repairBay.Id,
                ScheduledAt = scheduledAt,
                Status = status ?? "Запланирована",
            };
            db.Appointments.Add(newAppointment);
        }

        db.SaveChanges();
        this.Close();
    }
}
