using Avalonia.Controls;
using Avalonia.Input;
using Avalonia;
using System;
using System.Linq;
using AutoService.Data;
using AutoService.Views;
using AutoService.Models;

namespace AutoService.Views;

public partial class MainWindow : Window
{
    private int _page = 0;
    private int _pageSize = 10;
    private int _totalCount = 0;

    public MainWindow()
    {
        InitializeComponent();
        PageSizeCombo.SelectedIndex = 0;
        LoadClients();
        ApplyRoleVisibility();
    }

    private void ApplyRoleVisibility()
    {
        var user = UserSession.CurrentUser;
        if (user == null) return;

        void SetMechanic()
        {
            AddClientButton.IsVisible = false;
            ManageCarsButton.IsVisible = false;
            ManageAppointmentsButton.IsVisible = false;
            ManageServicesButton.IsVisible = false;
            ManagePartsButton.IsVisible = false;
            ManageUsersButton.IsVisible = false;
            ManageRolesButton.IsVisible = false;
            ManageMechanicsButton.IsVisible = false;
            ReviewsButton.IsVisible = false;
            AnalyticsButton.IsVisible = false;
        }

        void SetManager()
        {
            AddClientButton.IsVisible = false;
            ManageCarsButton.IsVisible = false;
            ManageAppointmentsButton.IsVisible = false;
            ManageServicesButton.IsVisible = false;
            ManagePartsButton.IsVisible = false;
            ManageUsersButton.IsVisible = false;
            ManageRolesButton.IsVisible = false;
            ManageMechanicsButton.IsVisible = false;
            // Руководителю доступны: Отзывы, Аналитика
        }

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
                    SetMechanic();
                    break;
                case "manager":
                case "lead":
                    SetManager();
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
            case 2: // Mechanic
                SetMechanic();
                break;
            case 3: // Manager
                SetManager();
                break;
        }
    }

    private void LoadClients()
    {
        try
        {
            using var db = new AppDbContext();
            _totalCount = db.Clients.Count();
            var clients = db.Clients
                .OrderBy(c => c.FullName)
                .Skip(_page * _pageSize)
                .Take(_pageSize)
                .ToList();
            ClientsListBox.ItemsSource = clients;
            UpdatePageInfo();
        }
        catch
        {
            // In example keep silent; real app should log or show message
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

    private void ApplySearch()
    {
        var query = SearchText.Text?.Trim();
        try
        {
            using var db = new AppDbContext();
            IQueryable<Client> q = db.Clients;
            if (!string.IsNullOrWhiteSpace(query))
            {
                var s = query.ToLower();
                q = q.Where(c => (c.FullName != null && c.FullName.ToLower().Contains(s))
                              || (c.Phone != null && c.Phone.ToLower().Contains(s))
                              || (c.Email != null && c.Email.ToLower().Contains(s)));
            }
            _totalCount = q.Count();
            ClientsListBox.ItemsSource = q
                .OrderBy(c => c.FullName)
                .Skip(_page * _pageSize)
                .Take(_pageSize)
                .ToList();
            UpdatePageInfo();
        }
        catch
        {
        }
    }

    // New search handler referenced from XAML; filters clients by name, phone or email
    private void SearchText_KeyUp(object? sender, KeyEventArgs e)
    {
        _page = 0;
        ApplySearch();
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

    private async void ManageMechanics_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var win = new MechanicManagementWindow();
        await win.ShowDialog(this);
    }

    private async void Reviews_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var win = new ReviewsWindow();
        await win.ShowDialog(this);
    }

    private async void Analytics_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var win = new AnalyticsWindow();
        await win.ShowDialog(this);
    }
}