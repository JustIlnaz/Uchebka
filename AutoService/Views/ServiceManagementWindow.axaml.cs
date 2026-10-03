using Avalonia.Controls;
using AutoService.Data;
using AutoService.Models;
using System;
using System.Linq;

namespace AutoService.Views;

public partial class ServiceManagementWindow : Window
{
    private int _page = 0;
    private int _pageSize = 10;
    private int _totalCount = 0;
    private bool _sortByPrice;

    public ServiceManagementWindow()
    {
        InitializeComponent();
        PageSizeCombo.SelectedIndex = 0;
        LoadServices();
    }

    private void LoadServices()
    {
        _page = 0;
        _sortByPrice = false;
        ApplyFilter();
    }

    private void Filter_KeyUp(object? sender, Avalonia.Input.KeyEventArgs e)
    {
        _page = 0;
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        try
        {
            using var db = new AppDbContext();
            IQueryable<Service> q = db.Services;

            if (decimal.TryParse(MinPriceText.Text, out var minP))
                q = q.Where(s => s.BasePrice >= minP);
            if (decimal.TryParse(MaxPriceText.Text, out var maxP))
                q = q.Where(s => s.BasePrice <= maxP);
            if (int.TryParse(MinDurationText.Text, out var minD))
                q = q.Where(s => s.DurationMinutes >= minD);

            q = _sortByPrice
                ? q.OrderByDescending(s => s.BasePrice)
                : q.OrderBy(s => s.Id);

            _totalCount = q.Count();
            ServicesListBox.ItemsSource = q
                .Skip(_page * _pageSize)
                .Take(_pageSize)
                .ToList();
            UpdatePageInfo();
        }
        catch
        {
        }
    }

    private void SortByPrice_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _sortByPrice = true;
        _page = 0;
        ApplyFilter();
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

    private async void AddButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var win = new ServiceEditWindow();
        await win.ShowDialog(this);
        LoadServices();
    }

    private async void ListBox_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        var service = ServicesListBox.SelectedItem as Service;
        if (service == null) return;

        ServiceVariableData.SelectedService = service;

        var win = new ServiceEditWindow();
        await win.ShowDialog(this);

        ServiceVariableData.SelectedService = null;
        LoadServices();
    }
}
