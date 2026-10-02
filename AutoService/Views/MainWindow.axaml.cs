using Avalonia.Controls;
using Avalonia.Input;
using Avalonia;
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
        ApplyRoleVisibility();
    }

    private void ApplyRoleVisibility()
    {
        var user = UserSession.CurrentUser;
        if (user == null) return;

        // Prefer string RoleName if present (e.g. "admin", "mechanic", "manager")
        if (!string.IsNullOrWhiteSpace(user.RoleName))
        {
            var rn = user.RoleName.Trim().ToLower();
            switch (rn)
            {
                case "admin":
                case "administrator":
                    // full access
                    break;
                case "mechanic":
                    AddClientButton.IsVisible = false;
                    ManageCarsButton.IsVisible = false;
                    ManageAppointmentsButton.IsVisible = false;
                    ManageServicesButton.IsVisible = false;
                    ManagePartsButton.IsVisible = false;
                    break;
                case "manager":
                case "lead":
                    AddClientButton.IsVisible = false;
                    ManageCarsButton.IsVisible = false;
                    ManageAppointmentsButton.IsVisible = false;
                    ManageServicesButton.IsVisible = false;
                    ManagePartsButton.IsVisible = false;
                    break;
            }

            return;
        }

        // Fallback to numeric RoleId mapping (1=Admin, 2=Mechanic, 3=Manager)
        if (user.RoleId == null) return;

        switch (user.RoleId)
        {
            case 1: // Admin - full access
                break;
            case 2: // Mechanic - limited access
                AddClientButton.IsVisible = false;
                ManageCarsButton.IsVisible = false;
                ManageAppointmentsButton.IsVisible = false;
                ManageServicesButton.IsVisible = false;
                ManagePartsButton.IsVisible = false;
                break;
            case 3: // Manager - read-only mostly
                AddClientButton.IsVisible = false;
                ManageCarsButton.IsVisible = false;
                ManageAppointmentsButton.IsVisible = false;
                ManageServicesButton.IsVisible = false;
                ManagePartsButton.IsVisible = false;
                break;
        }
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

    // New search handler referenced from XAML; filters clients by name, phone or email
    private void SearchText_KeyUp(object? sender, KeyEventArgs e)
    {
        var tb = sender as TextBox;
        var query = tb?.Text?.Trim();

        try
        {
            using var db = new AppDbContext();
            if (string.IsNullOrWhiteSpace(query))
            {
                ClientsListBox.ItemsSource = db.Clients.ToList();
            }
            else
            {
                var q = query.ToLower();
                var results = db.Clients
                    .Where(c => (c.FullName != null && c.FullName.ToLower().Contains(q))
                             || (c.Phone != null && c.Phone.ToLower().Contains(q))
                             || (c.Email != null && c.Email.ToLower().Contains(q)))
                    .ToList();
                ClientsListBox.ItemsSource = results;
            }
        }
        catch
        {
            // ignore errors for now
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

    private async void ManageRoles_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var win = new RoleManagementWindow();
        await win.ShowDialog(this);
    }

    private async void ManageUsers_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var win = new UserManagementWindow();
        await win.ShowDialog(this);
    }
}