using Avalonia.Controls;
using Avalonia.Interactivity;
using AutoService.Data;
using AutoService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AutoService.Views;

public partial class InvoiceWindow : Window
{
    private WorkOrder? _workOrder;
    private Invoice? _invoice;

    public InvoiceWindow()
    {
        InitializeComponent();
        StatusCombo.ItemsSource = new[] { "Создан", "Оплачен", "Частично оплачен", "Отменён" };
    }

    public void SetWorkOrder(WorkOrder workOrder)
    {
        _workOrder = workOrder;
        WorkOrderText.Text = $"Заказ #{workOrder.Id}";

        using var db = new AppDbContext();
        _invoice = db.Invoices.FirstOrDefault(i => i.WorkOrderId == workOrder.Id);

        if (_invoice != null)
        {
            AmountBeforeDiscountText.Text = _invoice.AmountBeforeDiscount.ToString("F2");
            DiscountText.Text = _invoice.Discount.ToString("F2");
            TaxText.Text = _invoice.Tax.ToString("F2");
            TotalAmountText.Text = _invoice.TotalAmount.ToString("F2");
            StatusCombo.SelectedItem = _invoice.Status;
        }
        else
        {
            AmountBeforeDiscountText.Text = workOrder.TotalCost.ToString("F2");
            DiscountText.Text = "0";
            TaxText.Text = "0";
            StatusCombo.SelectedItem = "Создан";
            CalculateTotal();
        }
    }

    private void CalculateTotal()
    {
        if (decimal.TryParse(AmountBeforeDiscountText.Text, out var amount) &&
            decimal.TryParse(DiscountText.Text, out var discount) &&
            decimal.TryParse(TaxText.Text, out var tax))
        {
            var total = amount - discount + tax;
            TotalAmountText.Text = total.ToString("F2");
        }
    }

    private async void Save_Click(object? sender, RoutedEventArgs e)
    {
        await SaveInvoiceAsync(closeAfterSave: true);
    }

    private async Task<bool> SaveInvoiceAsync(bool closeAfterSave)
    {
        if (_workOrder == null) return false;

        using var db = new AppDbContext();

        if (!decimal.TryParse(AmountBeforeDiscountText.Text, out var amount) || amount < 0
            || !decimal.TryParse(DiscountText.Text, out var discount) || discount < 0 || discount > amount
            || !decimal.TryParse(TaxText.Text, out var tax) || tax < 0)
        {
            await InputDialog.ShowMessage(this, "Проверьте сумму, скидку и налог: значения должны быть корректными и неотрицательными, а скидка — не больше суммы.");
            return false;
        }

        var total = amount - discount + tax;
        Invoice invoice;

        if (_invoice != null)
        {
            var existing = db.Invoices.FirstOrDefault(i => i.Id == _invoice.Id);
            if (existing == null)
            {
                await InputDialog.ShowMessage(this, "Счёт не найден.");
                return false;
            }
            invoice = existing;
        }
        else
        {
            invoice = new Invoice { WorkOrderId = _workOrder.Id, CreatedAt = DateTime.Now };
            db.Invoices.Add(invoice);
        }

        if (invoice.Id != 0)
        {
            var reservedPayments = db.Payments
                .Where(p => p.InvoiceId == invoice.Id && p.Status != "Отменён" && p.Status != "Отменен")
                .Sum(p => p.Amount);
            if (total < reservedPayments)
            {
                await InputDialog.ShowMessage(this, "Итог счёта нельзя уменьшить ниже уже внесённых платежей.");
                return false;
            }
        }

        invoice.AmountBeforeDiscount = amount;
        invoice.Discount = discount;
        invoice.Tax = tax;
        invoice.TotalAmount = total;
        db.SaveChanges();
        var paid = db.Payments
            .Where(p => p.InvoiceId == invoice.Id && (p.Status == "Завершён" || p.Status == "Успешно"))
            .Sum(p => p.Amount);
        invoice.Status = paid >= total
            ? "Оплачен"
            : paid > 0 ? "Частично оплачен" : "Создан";
        db.SaveChanges();

        _invoice = invoice;
        StatusCombo.SelectedItem = invoice.Status;
        TotalAmountText.Text = total.ToString("F2");
        if (closeAfterSave) Close();
        return true;
    }

    private async void AddPayment_Click(object? sender, RoutedEventArgs e)
    {
        if (_invoice == null && !await SaveInvoiceAsync(closeAfterSave: false))
        {
            return;
        }

        if (_invoice == null) return;
        var paymentWindow = new PaymentWindow();
        paymentWindow.SetInvoice(_invoice);
        await paymentWindow.ShowDialog(this);

        using var db = new AppDbContext();
        _invoice = db.Invoices.FirstOrDefault(i => i.Id == _invoice.Id);
        if (_invoice != null) StatusCombo.SelectedItem = _invoice.Status;
    }
}