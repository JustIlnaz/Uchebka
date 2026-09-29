using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Linq;
using TestSql321.Data;
using TestSql321.Models;

namespace TestSql321;

public partial class CreateAndChangeUser : Window
{
    public CreateAndChangeUser()
    {
        InitializeComponent();

        if (UserVariableData.seletedUserInMainWindow == null) return;
        FullNameText.Text = UserVariableData.seletedUserInMainWindow.FullName;
        DescriptionText.Text = UserVariableData.seletedUserInMainWindow.Description;
        PhoneNumberText.Text = UserVariableData.seletedUserInMainWindow.PhoneNumber;
    }

    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if(UserVariableData.seletedUserInMainWindow != null)
        {
            var idUser = UserVariableData.seletedUserInMainWindow.IdUser;
            var thisUser = App.DbContext.Users.FirstOrDefault(x => x.IdUser == idUser);

            if (thisUser == null) return;

            thisUser.PhoneNumber = PhoneNumberText.Text;
            thisUser.Description = DescriptionText.Text;
            thisUser.FullName = FullNameText.Text;
        }
        else
        {
            var newUser = new User() { 
                FullName = FullNameText.Text,
                Description = DescriptionText.Text,
                PhoneNumber = PhoneNumberText.Text,
            };
            App.DbContext.Users.Add(newUser);
        }
        App.DbContext.SaveChanges();
        this.Close();
    }
}