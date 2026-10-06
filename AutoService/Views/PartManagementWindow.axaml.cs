using Avalonia.Controls;
using AutoService.Data;
using AutoService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace AutoService.Views;

public partial class PartManagementWindow : Window
{
    private int _page = 0;
    private int _pageSize = 10;
    private int _totalCount = 0;

    public PartManagementWindow()
    {
        InitializeComponent();
        PageSizeCombo.SelectedIndex = 0;
        LoadSuppliers();
        LoadParts();
    }

    private void LoadSuppliers()
    {
        try
        {
            using var db = new AppDbContext();
            SupplierCombo.ItemsSource = db.Suppliers.OrderBy(s => s.Name).ToList();
        }
        catch
        {
        }
    }

    private void LoadParts()
    {
        _page = 0;
        ApplyFilter();
    }

    private void Filter_SelectionChanged(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _page = 0;
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        try
        {
            using var db = new AppDbContext();
            IQueryable<Part> q = db.Parts.Include(p => p.Supplier);

            if (SupplierCombo.SelectedItem is Supplier supplier)
                q = q.Where(p => p.SupplierId == supplier.Id);

            if (LowStockCheck.IsChecked == true)
                q = q.Where(p => p.QuantityInStock < p.MinQuantity);

            _totalCount = q.Count();
            PartsListBox.ItemsSource = q
                .OrderBy(p => p.Id)
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

    private async void AddButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var win = new PartEditWindow();
        await win.ShowDialog(this);
        LoadParts();
    }

    private async void ListBox_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        var part = PartsListBox.SelectedItem as Part;
        if (part == null) return;

        PartVariableData.SelectedPart = part;

        var win = new PartEditWindow();
        await win.ShowDialog(this);

        PartVariableData.SelectedPart = null;
        LoadParts();
    }
}
