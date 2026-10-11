using Avalonia.Controls;
using AutoService.Data;
using AutoService.Models;
using Microsoft.EntityFrameworkCore;
using System;
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

    private async void AddButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DatePicker.SelectedDate is not DateTimeOffset selectedDate)
        {
            await InputDialog.ShowMessage(this, "Выберите дату расписания.");
            return;
        }

        if (StartTimePicker.SelectedTime is not TimeSpan start)
        {
            await InputDialog.ShowMessage(this, "Выберите время начала.");
            return;
        }
        if (EndTimePicker.SelectedTime is not TimeSpan end)
        {
            await InputDialog.ShowMessage(this, "Выберите время окончания.");
            return;
        }
        if (end <= start)
        {
            await InputDialog.ShowMessage(this, "Время окончания должно быть позже времени начала.");
            return;
        }

        using var db = new AppDbContext();
        var date = selectedDate.Date.Date;
        if (db.MechanicSchedules.Any(s => s.MechanicId == _mechanicId
            && s.Date.Date == date && s.StartTime == start && s.EndTime == end))
        {
            await InputDialog.ShowMessage(this, "Такое расписание уже добавлено.");
            return;
        }

        db.MechanicSchedules.Add(new MechanicSchedule
        {
            MechanicId = _mechanicId,
            Date = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc),
            StartTime = start,
            EndTime = end
        });
        db.SaveChanges();

        DatePicker.SelectedDate = null;
        StartTimePicker.SelectedTime = null;
        EndTimePicker.SelectedTime = null;
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
