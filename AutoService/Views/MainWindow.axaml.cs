using Avalonia.Controls;
using Avalonia.Input;
using System.Linq;
using AutoService.Data;
using AutoService.Views;
using AutoService.Models;

namespace AutoService.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        LoadClients();
    }

    private void LoadClients()
    {
        try
        {
            using var db = new AppDbContext();
            var clients = db.Clients.ToList();
            ClientsListBox.ItemsSource = clients;
        }
        catch
        {
            // In example keep silent; real app should log or show message
        }
    }

    private async void AddButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var win = new ClientEditWindow();
        await win.ShowDialog(this);
        LoadClients();
    }

    private async void ListBox_DoubleTapped(object? sender, TappedEventArgs e)
    {
        var client = ClientsListBox.SelectedItem as Client;
        if (client == null) return;

        ClientVariableData.SelectedClient = client;

        var win = new ClientEditWindow();
        await win.ShowDialog(this);

        ClientVariableData.SelectedClient = null;
        LoadClients();
    }

    private async void ManageCars_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var win = new CarManagementWindow();
        await win.ShowDialog(this);
    }

    private async void ManageAppointments_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var win = new AppointmentManagementWindow();
        await win.ShowDialog(this);
    }

    private async void ManageWorkOrders_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var win = new WorkOrderManagementWindow();
        await win.ShowDialog(this);
    }

    private async void ManageServices_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var win = new ServiceManagementWindow();
        await win.ShowDialog(this);
    }

    private async void ManageParts_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var win = new PartManagementWindow();
        await win.ShowDialog(this);
    }
}