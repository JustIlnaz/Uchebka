using Avalonia.Controls;
using AutoService.Data;
using AutoService.Models;
using System.Linq;

namespace AutoService.Views;

public partial class PartManagementWindow : Window
{
    public PartManagementWindow()
    {
        InitializeComponent();
        LoadParts();
    }

    private void LoadParts()
    {
        try
        {
            using var db = new AppDbContext();
            var parts = db.Parts.ToList();
            PartsListBox.ItemsSource = parts;
        }
        catch
        {
        }
    }

    private async void AddButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var win = new PartEditWindow();
        await win.ShowDialog(this);
        LoadParts();
    }

    private async void ListBox_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        var part = PartsListBox.SelectedItem as Part;
        if (part == null) return;

        PartVariableData.SelectedPart = part;

        var win = new PartEditWindow();
        await win.ShowDialog(this);

        PartVariableData.SelectedPart = null;
        LoadParts();
    }
}
