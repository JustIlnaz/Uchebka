using Avalonia.Controls;
using Avalonia.Interactivity;
using AutoService.Data;
using AutoService.Models;
using System.Linq;

namespace AutoService.Views;

public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();
    }

    private void Login_Click(object? sender, RoutedEventArgs e)
    {
        var username = UsernameText.Text?.Trim();
        var password = PasswordText.Text ?? string.Empty;

        using var db = new AppDbContext();
        var user = db.Users.FirstOrDefault(u => u.Username == username && u.PasswordHash == password);
        if (user == null)
        {
            // simple feedback - real app should show dialog
            return;
        }

        UserSession.CurrentUser = user;
        // open main window
        var main = new Views.MainWindow();
        main.Show();
        this.Close();
    }

    private void Register_Click(object? sender, RoutedEventArgs e)
    {
        var username = UsernameText.Text?.Trim();
        var password = PasswordText.Text ?? string.Empty;
        if (string.IsNullOrWhiteSpace(username)) return;

        using var db = new AppDbContext();
        if (db.Users.Any(u => u.Username == username)) return; // already exists

        var newUser = new User { Username = username, PasswordHash = password, FullName = username };
        db.Users.Add(newUser);
        db.SaveChanges();

        UserSession.CurrentUser = newUser;
        var main = new Views.MainWindow();
        main.Show();
        this.Close();
    }
}
