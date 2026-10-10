using System;
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

        try
        {
            using var db = new AppDbContext();
            var user = db.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user == null)
            {
                MessageText.Text = "Неверное имя пользователя или пароль.";
                return;
            }

            UserSession.CurrentUser = user;
            var main = new Views.MainWindow();
            main.Show();
            this.Close();
        }
        catch (Exception ex)
        {
            MessageText.Text = "Ошибка подключения к БД: " + ex.Message;
        }
    }

    private void Register_Click(object? sender, RoutedEventArgs e)
    {
        var username = UsernameText.Text?.Trim();
        var password = PasswordText.Text ?? string.Empty;

        if (string.IsNullOrWhiteSpace(username))
        {
            MessageText.Text = "Введите имя пользователя.";
            return;
        }
        if (string.IsNullOrWhiteSpace(password))
        {
            MessageText.Text = "Введите пароль.";
            return;
        }

        try
        {
            using var db = new AppDbContext();
            if (db.Users.Any(u => u.Username == username))
            {
                MessageText.Text = "Пользователь с таким именем уже существует.";
                return;
            }

            var clientRole = db.Roles.FirstOrDefault(r => r.Name == "Клиент");
            var newUser = new User
            {
                Username = username,
                Password = password,
                FullName = username,
                RoleId = clientRole?.Id,
                RoleName = clientRole?.Name ?? "Клиент"
            };
            db.Users.Add(newUser);
            db.SaveChanges();

            db.Set<Login>().Add(new Login
            {
                LoginName = username,
                Password = password,
                UserId = newUser.Id
            });
            db.SaveChanges();

            UserSession.CurrentUser = newUser;
            var main = new Views.MainWindow();
            main.Show();
            this.Close();
        }
        catch (Exception ex)
        {
            MessageText.Text = "Ошибка регистрации: " + ex.Message;
        }
    }
}
