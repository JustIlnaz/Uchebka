using Avalonia.Controls;
using AutoService.Data;
using AutoService.Models;
using System.Linq;

namespace AutoService.Views;

public partial class ClientEditWindow : Window
{
    public ClientEditWindow()
    {
        InitializeComponent();

        if (ClientVariableData.SelectedClient != null)
        {
            FullNameText.Text = ClientVariableData.SelectedClient.FullName;
            PhoneText.Text = ClientVariableData.SelectedClient.Phone;
            EmailText.Text = ClientVariableData.SelectedClient.Email;
        }
    }

    private void Save_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        using var db = new AppDbContext();

        if (ClientVariableData.SelectedClient != null)
        {
            var id = ClientVariableData.SelectedClient.Id;
            var thisClient = db.Clients.FirstOrDefault(x => x.Id == id);
            if (thisClient != null)
            {
                thisClient.FullName = FullNameText.Text;
                thisClient.Phone = PhoneText.Text;
                thisClient.Email = EmailText.Text;
            }
        }
        else
        {
            var newClient = new Client
            {
                FullName = FullNameText.Text ?? string.Empty,
                Phone = PhoneText.Text,
                Email = EmailText.Text,
            };
            db.Clients.Add(newClient);
        }

        db.SaveChanges();
        this.Close();
    }
}
