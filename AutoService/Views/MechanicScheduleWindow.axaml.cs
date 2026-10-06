using Avalonia.Controls;
using AutoService.Data;
using AutoService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.Linq;

namespace AutoService.Views;

public partial class MechanicScheduleWindow : Window
{
    private int _mechanicId;

    public MechanicScheduleWindow()
    {
        InitializeComponent();

        var m = MechanicVariableData.SelectedMechanic;
        if (m == null) { Close(); return; }
        _mechanicId = m.Id;

        using var db = new AppDbContext();
        var mech = db.Mechanics.Include(x => x.User).FirstOrDefault(x => x.Id == _mechanicId);
        MechanicNameText.Text = mech?.User?.FullName ?? $"Механик #{_mechanicId}";

        LoadSchedule();
    }

    private void LoadSchedule()
    {
        using var db = new AppDbContext();
        var schedule = db.MechanicSchedules
            .Where(s => s.MechanicId == _mechanicId)
            .OrderBy(s => s.Date)
            .ThenBy(s => s.StartTime)
            .ToList();
        ScheduleListBox.ItemsSource = schedule;
    }

    private void AddButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (!DateTime.TryParseExact(DateText.Text, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            if (!DateTime.TryParse(DateText.Text, out date)) return;
        }

        if (!TimeSpan.TryParse(StartText.Text, out var start)) return;
        if (!TimeSpan.TryParse(EndText.Text, out var end)) return;

        using var db = new AppDbContext();
        db.MechanicSchedules.Add(new MechanicSchedule
        {
            MechanicId = _mechanicId,
            Date = date,
            StartTime = start,
            EndTime = end
        });
        db.SaveChanges();

        DateText.Text = string.Empty;
        StartText.Text = string.Empty;
        EndText.Text = string.Empty;
        LoadSchedule();
    }

    private void DeleteButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var item = ScheduleListBox.SelectedItem as MechanicSchedule;
        if (item == null) return;

        using var db = new AppDbContext();
        var entity = db.MechanicSchedules.FirstOrDefault(s => s.Id == item.Id);
        if (entity != null)
        {
            db.MechanicSchedules.Remove(entity);
            db.SaveChanges();
        }
        LoadSchedule();
    }
}
