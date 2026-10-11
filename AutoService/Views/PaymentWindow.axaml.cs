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

    private async void Save_Click(object? sender, RoutedEventArgs e)
    {
        if (_invoice == null) return;
        if (!decimal.TryParse(AmountText.Text, out var amount) || amount <= 0)
        {
            await InputDialog.ShowMessage(this, "Сумма платежа должна быть больше нуля.");
            return;
        }
        if (MethodCombo.SelectedItem is not string method || string.IsNullOrWhiteSpace(method))
        {
            await InputDialog.ShowMessage(this, "Выберите способ оплаты.");
            return;
        }

        using var db = new AppDbContext();
        var invoice = db.Invoices.FirstOrDefault(i => i.Id == _invoice.Id);
        if (invoice == null)
        {
            await InputDialog.ShowMessage(this, "Счёт не найден.");
            return;
        }
        var status = StatusCombo.SelectedItem as string;
        if (status is not ("Ожидает" or "Завершён" or "Отменён"))
        {
            await InputDialog.ShowMessage(this, "Выберите корректный статус платежа.");
            return;
        }

        var reservedPayments = db.Payments
            .Where(p => p.InvoiceId == _invoice.Id && p.Status != "Отменён" && p.Status != "Отменен")
            .Sum(p => p.Amount);
        if (reservedPayments + amount > invoice.TotalAmount)
        {
            await InputDialog.ShowMessage(this, "Платёж не может превышать остаток по счёту.");
            return;
        }

        var payment = new Payment
        {
            InvoiceId = _invoice.Id,
            Date = DateTime.Now,
            Amount = amount,
            Method = method,
            Status = status,
            TransactionNumber = TransactionText.Text
        };

        db.Payments.Add(payment);

        var totalPaid = db.Payments
            .Where(p => p.InvoiceId == _invoice.Id && (p.Status == "Завершён" || p.Status == "Успешно"))
            .Sum(p => p.Amount)
            + (status is "Завершён" or "Успешно" ? amount : 0m);
        invoice.Status = totalPaid >= invoice.TotalAmount
            ? "Оплачен"
            : totalPaid > 0 ? "Частично оплачен" : "Создан";

        db.SaveChanges();
        this.Close();
    }
}