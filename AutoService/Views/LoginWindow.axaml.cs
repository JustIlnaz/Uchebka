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

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            MessageText.Text = "Введите имя пользователя и пароль.";
            return;
        }

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



}
