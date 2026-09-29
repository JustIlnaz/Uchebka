using Avalonia.Controls;
using AutoService.Data;
using AutoService.Models;
using System.Linq;

namespace AutoService.Views;

public partial class ServiceManagementWindow : Window
{
    public ServiceManagementWindow()
    {
        InitializeComponent();
        LoadServices();
    }

    private void LoadServices()
    {
        try
        {
            using var db = new AppDbContext();
            var services = db.Services.ToList();
            ServicesListBox.ItemsSource = services;
        }
        catch
        {
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
