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

    private bool IsClient => UserSession.CurrentUser?.RoleName?.Trim().ToLowerInvariant() is "client" or "клиент"
        || UserSession.CurrentUser?.RoleId == 4;

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

        var buttons = new[]
        {
            AddClientButton, ManageCarsButton, ManageAppointmentsButton,
            ManageWorkOrdersButton, ManageServicesButton, ManagePartsButton,
            ManageUsersButton, ManageRolesButton, ManageMechanicsButton,
            ReviewsButton, AnalyticsButton
        };
        foreach (var button in buttons)
            button.IsVisible = false;

        var role = user.RoleName?.Trim().ToLowerInvariant();
        var roleId = role switch
        {
            "admin" or "administrator" or "администратор" => 1,
            "mechanic" or "механик" => 2,
            "manager" or "lead" or "руководитель" => 3,
            "client" or "клиент" => 4,
            _ => user.RoleId ?? 0
        };

        switch (roleId)
        {
            case 1:
                foreach (var button in buttons)
                    button.IsVisible = true;
                break;
            case 2:
                ManageWorkOrdersButton.IsVisible = true;
                break;
            case 3:
                ReviewsButton.IsVisible = true;
                AnalyticsButton.IsVisible = true;
                break;
            case 4:
                ManageCarsButton.IsVisible = true;
                ManageAppointmentsButton.IsVisible = true;
                ManageWorkOrdersButton.IsVisible = true;
                AddClientButton.IsVisible = false;
                SearchText.Watermark = "Ваши данные";
                break;
        }
    }

    private void LoadClients()
    {
        try
        {
            using var db = new AppDbContext();
            if (IsClient)
            {
                ApplySearch();
                return;
            }

            _totalCount = db.Clients.Count();
            ClientsListBox.ItemsSource = db.Clients
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
            if (IsClient)
            {
                var email = UserSession.CurrentUser?.Email?.Trim().ToLowerInvariant();
                q = q.Where(c => email != null && c.Email != null && c.Email.Trim().ToLower() == email);
            }
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
        if (IsClient) return;

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