using Avalonia.Controls;
using AutoService.Data;
using AutoService.Models;
using System.Linq;

namespace AutoService.Views;

public partial class UserManagementWindow : Window
{
    public UserManagementWindow()
    {
        InitializeComponent();
        LoadUsers();
    }

    private void LoadUsers()
    {
        using var db = new AppDbContext();
        UsersListBox.ItemsSource = db.Users.ToList();
    }

    private async void AddButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        UserVariableData.SelectedUser = null;
        var win = new UserEditWindow();
        await win.ShowDialog(this);
        LoadUsers();
    }

    private async void ListBox_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        var user = UsersListBox.SelectedItem as User;
        if (user == null) return;

        UserVariableData.SelectedUser = user;
        var win = new UserEditWindow();
        await win.ShowDialog(this);
        UserVariableData.SelectedUser = null;
        LoadUsers();
    }

    private void DeleteButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var user = UsersListBox.SelectedItem as User;
        if (user == null) return;

        using var db = new AppDbContext();
        var u = db.Users.FirstOrDefault(x => x.Id == user.Id);
        if (u != null)
        {
            db.Users.Remove(u);
            db.SaveChanges();
        }
        LoadUsers();
    }
}
