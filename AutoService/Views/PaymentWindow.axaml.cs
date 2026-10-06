using Avalonia.Controls;
using Avalonia.Interactivity;
using AutoService.Data;
using System;
using System.Linq;

namespace AutoService.Views;

public partial class PaymentWindow : Window
{
    private Invoice? _invoice;

    public PaymentWindow()
    {
        InitializeComponent();
        MethodCombo.ItemsSource = new[] { "Наличные", "Карта", "Перевод" };
        StatusCombo.ItemsSource = new[] { "Ожидает", "Завершён", "Отменён" };
        StatusCombo.SelectedIndex = 0;
    }

    public void SetInvoice(Invoice invoice)
    {
        _invoice = invoice;
    }

    private void Save_Click(object? sender, RoutedEventArgs e)
    {
        if (_invoice == null) return;
        if (!decimal.TryParse(AmountText.Text, out var amount)) return;

        using var db = new AppDbContext();

        var payment = new Payment
        {
            InvoiceId = _invoice.Id,
            Date = DateTime.Now,
            Amount = amount,
            Method = MethodCombo.SelectedItem as string,
            Status = StatusCombo.SelectedItem as string,
            TransactionNumber = TransactionText.Text
        };

        db.Payments.Add(payment);

        // Update invoice status if fully paid
        var totalPaid = db.Payments.Where(p => p.InvoiceId == _invoice.Id).Sum(p => p.Amount) + amount;
        var invoice = db.Invoices.FirstOrDefault(i => i.Id == _invoice.Id);
        if (invoice != null && totalPaid >= invoice.TotalAmount)
        {
            invoice.Status = "Оплачен";
        }

        db.SaveChanges();
        this.Close();
    }
}