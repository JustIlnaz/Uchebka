using Avalonia.Controls;
using AutoService.Data;
using AutoService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace AutoService.Views;

public partial class CarManagementWindow : Window
{
    private int _page = 0;
    private int _pageSize = 10;
    private int _totalCount = 0;
    private bool _sortByMileage;

    public CarManagementWindow()
    {
        InitializeComponent();
        PageSizeCombo.SelectedIndex = 0;
        LoadCars();
    }

    private void LoadCars()
    {
        _page = 0;
        _sortByMileage = false;
        ApplySearch();
    }

    private void ApplySearch()
    {
        var query = SearchText.Text?.Trim();
        try
        {
            using var db = new AppDbContext();
            IQueryable<Car> q = db.Cars.Include(c => c.Make).Include(c => c.Category);
            if (!string.IsNullOrWhiteSpace(query))
            {
                var s = query.ToLower();
                q = q.Where(c => (c.Vin != null && c.Vin.ToLower().Contains(s))
                              || (c.Model != null && c.Model.ToLower().Contains(s))
                              || (c.RegistrationNumber != null && c.RegistrationNumber.ToLower().Contains(s))
                              || (c.Make != null && c.Make.Name.ToLower().Contains(s)));
            }
            q = _sortByMileage
                ? q.OrderByDescending(c => c.Mileage)
                : q.OrderBy(c => c.Id);
            _totalCount = q.Count();
            CarsListBox.ItemsSource = q
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
        if (_page > 0) { _page--; ApplySearch(); }
    }

    private void NextPage_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var totalPages = (int)Math.Ceiling(_totalCount / (double)_pageSize);
        if (_page < totalPages - 1) { _page++; ApplySearch(); }
    }

    private void PageSize_Changed(object? sender, SelectionChangedEventArgs e)
    {
        if (PageSizeCombo.SelectedItem is ComboBoxItem item &&
            int.TryParse(item.Content?.ToString(), out var size))
        {
            _pageSize = size;
            _page = 0;
            ApplySearch();
        }
    }

    private void SearchText_KeyUp(object? sender, Avalonia.Input.KeyEventArgs e)
    {
        _page = 0;
        ApplySearch();
    }

    private void SortByMileage_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _sortByMileage = true;
        _page = 0;
        ApplySearch();
    }

    private async void AddButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var win = new CarEditWindow();
        await win.ShowDialog(this);
        LoadCars();
    }

    private async void ListBox_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        var car = CarsListBox.SelectedItem as Car;
        if (car == null) return;

        CarVariableData.SelectedCar = car;

        var win = new CarEditWindow();
        await win.ShowDialog(this);

        CarVariableData.SelectedCar = null;
        LoadCars();
    }
}
