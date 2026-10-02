using Avalonia.Controls;
using AutoService.Data;
using System.Linq;

namespace AutoService.Views;

public partial class RoleManagementWindow : Window
{
    public RoleManagementWindow()
    {
        InitializeComponent();
        LoadRoles();
    }

    private void LoadRoles()
    {
        using var db = new AppDbContext();
        RolesListBox.ItemsSource = db.Roles.ToList();
    }

    private async void AddButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var input = new InputDialog("Role name:");
        var result = await input.ShowDialog<string?>(this);
        if (string.IsNullOrWhiteSpace(result)) return;

        using var db = new AppDbContext();
        db.Roles.Add(new Role { Name = result });
        db.SaveChanges();
        LoadRoles();
    }

    private async void EditButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var role = RolesListBox.SelectedItem as Role;
        if (role == null) return;

        var input = new InputDialog("Role name:", role.Name);
        var result = await input.ShowDialog<string?>(this);
        if (string.IsNullOrWhiteSpace(result)) return;

        using var db = new AppDbContext();
        var r = db.Roles.FirstOrDefault(x => x.Id == role.Id);
        if (r != null) { r.Name = result; db.SaveChanges(); }
        LoadRoles();
    }

    private void DeleteButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var role = RolesListBox.SelectedItem as Role;
        if (role == null) return;

        using var db = new AppDbContext();
        var r = db.Roles.FirstOrDefault(x => x.Id == role.Id);
        if (r != null) { db.Roles.Remove(r); db.SaveChanges(); }
        LoadRoles();
    }
}
