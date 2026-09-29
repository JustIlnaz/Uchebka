using Avalonia.Controls;
using AutoService.Data;
using AutoService.Models;
using System.Linq;

namespace AutoService.Views;

public partial class ServiceEditWindow : Window
{
    public ServiceEditWindow()
    {
        InitializeComponent();

        if (ServiceVariableData.SelectedService != null)
        {
            NameText.Text = ServiceVariableData.SelectedService.Name;
            DescriptionText.Text = ServiceVariableData.SelectedService.Description;
            PriceText.Text = ServiceVariableData.SelectedService.BasePrice.ToString("F2");
            DurationText.Text = ServiceVariableData.SelectedService.DurationMinutes.ToString();
        }
    }

    private void Save_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        using var db = new AppDbContext();

        if (ServiceVariableData.SelectedService != null)
        {
            var id = ServiceVariableData.SelectedService.Id;
            var thisService = db.Services.FirstOrDefault(x => x.Id == id);
            if (thisService != null)
            {
                thisService.Name = NameText.Text;
                thisService.Description = DescriptionText.Text;
                thisService.BasePrice = decimal.TryParse(PriceText.Text, out var p) ? p : 0;
                thisService.DurationMinutes = int.TryParse(DurationText.Text, out var d) ? d : 0;
            }
        }
        else
        {
            var newService = new Service
            {
                Name = NameText.Text ?? string.Empty,
                Description = DescriptionText.Text,
                BasePrice = decimal.TryParse(PriceText.Text, out var p) ? p : 0,
                DurationMinutes = int.TryParse(DurationText.Text, out var d) ? d : 0,
            };
            db.Services.Add(newService);
        }

        db.SaveChanges();
        this.Close();
    }
}
