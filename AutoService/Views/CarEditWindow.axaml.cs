using Avalonia.Controls;
using AutoService.Data;
using AutoService.Models;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace AutoService.Views;

public partial class CarEditWindow : Window
{
    public CarEditWindow()
    {
        InitializeComponent();

        using var db = new AppDbContext();
        ClientCombo.ItemsSource = db.Clients.ToList();
        MakeCombo.ItemsSource = db.CarMakes.ToList();
        CategoryCombo.ItemsSource = db.CarCategories.OrderBy(c => c.Name).ToList();

        if (CarVariableData.SelectedCar != null)
        {
            var car = CarVariableData.SelectedCar;
            ClientCombo.SelectedItem = car.Client;
            MakeCombo.SelectedItem = car.Make;
            CategoryCombo.SelectedItem = car.Category;
            ModelText.Text = car.Model;
            RegText.Text = car.RegistrationNumber;
            VinText.Text = car.Vin;
            YearInput.Value = car.Year;
            MileageInput.Value = car.Mileage;
            Title = "Редактирование автомобиля";
        }
    }

    private async void Save_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        using var db = new AppDbContext();
        var client = ClientCombo.SelectedItem as Client;
        var make = MakeCombo.SelectedItem as CarMake;
        var category = CategoryCombo.SelectedItem as CarCategory;
        if (client == null || make == null || string.IsNullOrWhiteSpace(ModelText.Text) || string.IsNullOrWhiteSpace(RegText.Text))
        {
            await InputDialog.ShowMessage(this, "Заполните клиента, марку, модель и госномер.");
            return;
        }
        var registrationNumber = RegText.Text.Trim().ToUpperInvariant();
        var vin = VinText.Text?.Trim().ToUpperInvariant();
        if (!string.IsNullOrWhiteSpace(vin) && !Regex.IsMatch(vin, "^[A-HJ-NPR-Z0-9]{17}$"))
        {
            await InputDialog.ShowMessage(this, "VIN должен содержать ровно 17 латинских букв и цифр без I, O и Q.");
            return;
        }
        var carId = CarVariableData.SelectedCar?.Id ?? 0;
        if (db.Cars.Any(c => c.Id != carId && c.RegistrationNumber == registrationNumber))
        {
            await InputDialog.ShowMessage(this, "Автомобиль с таким госномером уже зарегистрирован.");
            return;
        }
        if (!string.IsNullOrWhiteSpace(vin) && db.Cars.Any(c => c.Id != carId && c.Vin == vin))
        {
            await InputDialog.ShowMessage(this, "Автомобиль с таким VIN уже зарегистрирован.");
            return;
        }
        var year = YearInput.Value is decimal yearValue ? (int?)yearValue : null;
        var mileage = MileageInput.Value is decimal mileageValue ? (int?)mileageValue : null;

        if (CarVariableData.SelectedCar != null)
        {
            var id = CarVariableData.SelectedCar.Id;
            var thisCar = db.Cars.FirstOrDefault(x => x.Id == id);
            if (thisCar != null)
            {
                thisCar.ClientId = client.Id;
                thisCar.MakeId = make.Id;
                thisCar.CategoryId = category?.Id;
                thisCar.Model = ModelText.Text;
                thisCar.RegistrationNumber = registrationNumber;
                thisCar.Vin = vin;
                thisCar.Year = year;
                thisCar.Mileage = mileage;
            }
        }
        else
        {
            var newCar = new Car
            {
                ClientId = client.Id,
                MakeId = make.Id,
                CategoryId = category?.Id,
                Model = ModelText.Text,
                RegistrationNumber = registrationNumber,
                Vin = vin,
                Year = year,
                Mileage = mileage,
            };
            db.Cars.Add(newCar);
        }

        db.SaveChanges();
        this.Close();
    }

    private void Cancel_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close();
    }
}
