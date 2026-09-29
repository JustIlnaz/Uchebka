using Avalonia.Controls;
using AutoService.Data;
using AutoService.Models;
using System.Linq;

namespace AutoService.Views;

public partial class PartEditWindow : Window
{
    public PartEditWindow()
    {
        InitializeComponent();

        using var db = new AppDbContext();
        SupplierCombo.ItemsSource = db.Suppliers.ToList();

        if (PartVariableData.SelectedPart != null)
        {
            var p = PartVariableData.SelectedPart;
            SupplierCombo.SelectedItem = p.Supplier;
            SkuText.Text = p.Sku;
            SalePriceText.Text = p.SalePrice.ToString("F2");
            QuantityText.Text = p.QuantityInStock.ToString();
        }
    }

    private void Save_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        using var db = new AppDbContext();

        if (PartVariableData.SelectedPart != null)
        {
            var id = PartVariableData.SelectedPart.Id;
            var thisPart = db.Parts.FirstOrDefault(x => x.Id == id);
            if (thisPart != null)
            {
                var supplier = SupplierCombo.SelectedItem as Supplier;
                thisPart.SupplierId = supplier?.Id;
                thisPart.Sku = SkuText.Text;
                thisPart.SalePrice = decimal.TryParse(SalePriceText.Text, out var sp) ? sp : 0;
                thisPart.QuantityInStock = int.TryParse(QuantityText.Text, out var q) ? q : 0;
            }
        }
        else
        {
            var supplier = SupplierCombo.SelectedItem as Supplier;
            var newPart = new Part
            {
                SupplierId = supplier?.Id,
                Sku = SkuText.Text,
                SalePrice = decimal.TryParse(SalePriceText.Text, out var sp) ? sp : 0,
                QuantityInStock = int.TryParse(QuantityText.Text, out var q) ? q : 0,
            };
            db.Parts.Add(newPart);
        }

        db.SaveChanges();
        this.Close();
    }
}
