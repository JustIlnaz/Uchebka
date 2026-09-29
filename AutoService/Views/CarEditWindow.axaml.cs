using Avalonia.Controls;
using AutoService.Data;
using AutoService.Models;
using System.Linq;

namespace AutoService.Views;

public partial class CarEditWindow : Window
{
    public CarEditWindow()
    {
        InitializeComponent();

        using var db = new AppDbContext();
        ClientCombo.ItemsSource = db.Clients.ToList();
        MakeCombo.ItemsSource = db.CarMakes.ToList();

        if (CarVariableData.SelectedCar != null)
        {
            var car = CarVariableData.SelectedCar;
            ClientCombo.SelectedItem = car.Client;
            MakeCombo.SelectedItem = car.Make;
            ModelText.Text = car.Model;
            RegText.Text = car.RegistrationNumber;
            MileageText.Text = car.Mileage?.ToString();
        }
    }

    private void Save_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        using var db = new AppDbContext();

        if (CarVariableData.SelectedCar != null)
        {
            var id = CarVariableData.SelectedCar.Id;
            var thisCar = db.Cars.FirstOrDefault(x => x.Id == id);
            if (thisCar != null)
            {
                var client = ClientCombo.SelectedItem as Client;
                var make = MakeCombo.SelectedItem as CarMake;
                thisCar.ClientId = client?.Id;
                thisCar.MakeId = make?.Id;
                thisCar.Model = ModelText.Text;
                thisCar.RegistrationNumber = RegText.Text;
                thisCar.Mileage = int.TryParse(MileageText.Text, out var m) ? m : (int?)null;
            }
        }
        else
        {
            var client = ClientCombo.SelectedItem as Client;
            var make = MakeCombo.SelectedItem as CarMake;
            var newCar = new Car
            {
                ClientId = client?.Id,
                MakeId = make?.Id,
                Model = ModelText.Text,
                RegistrationNumber = RegText.Text,
                Mileage = int.TryParse(MileageText.Text, out var m) ? m : (int?)null,
            };
            db.Cars.Add(newCar);
        }

        db.SaveChanges();
        this.Close();
    }
}
