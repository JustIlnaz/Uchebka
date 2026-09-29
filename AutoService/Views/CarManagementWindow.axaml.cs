using Avalonia.Controls;
using AutoService.Data;
using AutoService.Models;
using System.Linq;

namespace AutoService.Views;

public partial class CarManagementWindow : Window
{
    public CarManagementWindow()
    {
        InitializeComponent();
        LoadCars();
    }

    private void LoadCars()
    {
        try
        {
            using var db = new AppDbContext();
            var cars = db.Cars.ToList();
            CarsListBox.ItemsSource = cars;
        }
        catch
        {
        }
    }

    private async void AddButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var win = new CarEditWindow();
        await win.ShowDialog(this);
        LoadCars();
    }

    private async void ListBox_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        var car = CarsListBox.SelectedItem as Car;
        if (car == null) return;

        CarVariableData.SelectedCar = car;

        var win = new CarEditWindow();
        await win.ShowDialog(this);

        CarVariableData.SelectedCar = null;
        LoadCars();
    }
}
