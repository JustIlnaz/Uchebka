using Avalonia.Controls;
using AutoService.Data;
using AutoService.Models;
using System.Linq;

namespace AutoService.Views;

public partial class UserEditWindow : Window
{
    public UserEditWindow()
    {
        InitializeComponent();

        using var db = new AppDbContext();
        RoleCombo.ItemsSource = db.Roles.ToList();

        if (UserVariableData.SelectedUser != null)
        {
            var u = UserVariableData.SelectedUser;
            UsernameText.Text = u.Username;
            FullNameText.Text = u.FullName;
            PasswordText.Text = u.Password;
            EmailText.Text = u.Email;
            PhoneText.Text = u.PhoneNumber;
            if (u.RoleId != null)
                RoleCombo.SelectedItem = db.Roles.FirstOrDefault(r => r.Id == u.RoleId);
        }
    }

    private void Save_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        using var db = new AppDbContext();
        var role = RoleCombo.SelectedItem as Role;

        if (UserVariableData.SelectedUser != null)
        {
            var id = UserVariableData.SelectedUser.Id;
            var u = db.Users.FirstOrDefault(x => x.Id == id);
            if (u != null)
            {
                u.Username = UsernameText.Text;
                u.FullName = FullNameText.Text;
                u.Password = PasswordText.Text;
                u.Email = EmailText.Text;
                u.PhoneNumber = PhoneText.Text;
                u.RoleId = role?.Id;
                u.RoleName = role?.Name;
            }
        }
        else
        {
            var u = new User
            {
                Username = UsernameText.Text,
                FullName = FullNameText.Text,
                Password = PasswordText.Text,
                Email = EmailText.Text,
                PhoneNumber = PhoneText.Text,
                RoleId = role?.Id,
                RoleName = role?.Name
            };
            db.Users.Add(u);
        }

        db.SaveChanges();
        this.Close();
    }
}
