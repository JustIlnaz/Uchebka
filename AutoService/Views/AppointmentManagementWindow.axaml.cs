using Avalonia.Controls;
using AutoService.Data;
using AutoService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace AutoService.Views;

public partial class AppointmentManagementWindow : Window
{
    private int _page = 0;
    private int _pageSize = 10;
    private int _totalCount = 0;

    public AppointmentManagementWindow()
    {
        InitializeComponent();
        StatusCombo.ItemsSource = new[] { "Запланирована", "Подтверждена", "Отменена", "Выполнена" };
        PageSizeCombo.SelectedIndex = 0;
        LoadAppointments();
    }

    private void LoadAppointments()
    {
        _page = 0;
        ApplyFilter();
    }

    private void Filter_SelectionChanged(object? sender, Avalonia.Interactivity.RoutedEventArgs e) { _page = 0; ApplyFilter(); }
    private void Filter_KeyUp(object? sender, Avalonia.Input.KeyEventArgs e) { _page = 0; ApplyFilter(); }

    private void ApplyFilter()
    {
        try
        {
            using var db = new AppDbContext();
            IQueryable<Appointment> q = db.Appointments
                .Include(a => a.Client)
                .Include(a => a.Car)
                .Include(a => a.Service);

            if (StatusCombo.SelectedItem is string status && !string.IsNullOrWhiteSpace(status))
                q = q.Where(a => a.Status == status);

            if (DateTime.TryParse(DateText.Text, out var date))
                q = q.Where(a => a.ScheduledAt.Date == date.Date);

            _totalCount = q.Count();
            AppointmentsListBox.ItemsSource = q
                .OrderByDescending(a => a.ScheduledAt)
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

    private void PrevPage_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (_page > 0) { _page--; ApplyFilter(); }
    }

    private void NextPage_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var totalPages = (int)Math.Ceiling(_totalCount / (double)_pageSize);
        if (_page < totalPages - 1) { _page++; ApplyFilter(); }
    }

    private void PageSize_Changed(object? sender, SelectionChangedEventArgs e)
    {
        if (PageSizeCombo.SelectedItem is ComboBoxItem item &&
            int.TryParse(item.Content?.ToString(), out var size))
        {
            _pageSize = size;
            _page = 0;
            ApplyFilter();
        }
    }

    private void ResetFilter_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        StatusCombo.SelectedItem = null;
        DateText.Text = string.Empty;
        LoadAppointments();
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
