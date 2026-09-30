using Avalonia.Controls;
using Avalonia.Interactivity;
using AutoService.Data;
using AutoService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

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
            // Pre-fill with work order total
            AmountBeforeDiscountText.Text = workOrder.TotalCost.ToString("F2");
            DiscountText.Text = "0";
            TaxText.Text = "0";
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

    private void Save_Click(object? sender, RoutedEventArgs e)
    {
        if (_workOrder == null) return;

        using var db = new AppDbContext();

        if (!decimal.TryParse(AmountBeforeDiscountText.Text, out var amount)) return;
        if (!decimal.TryParse(DiscountText.Text, out var discount)) discount = 0;
        if (!decimal.TryParse(TaxText.Text, out var tax)) tax = 0;

        var total = amount - discount + tax;
        var status = StatusCombo.SelectedItem as string ?? "Создан";

        if (_invoice != null)
        {
            var existing = db.Invoices.FirstOrDefault(i => i.Id == _invoice.Id);
            if (existing != null)
            {
                existing.AmountBeforeDiscount = amount;
                existing.Discount = discount;
                existing.Tax = tax;
                existing.TotalAmount = total;
                existing.Status = status;
            }
        }
        else
        {
            var newInvoice = new Invoice
            {
                WorkOrderId = _workOrder.Id,
                CreatedAt = DateTime.Now,
                AmountBeforeDiscount = amount,
                Discount = discount,
                Tax = tax,
                TotalAmount = total,
                Status = status
            };
            db.Invoices.Add(newInvoice);
        }

        db.SaveChanges();
        this.Close();
    }

    private void AddPayment_Click(object? sender, RoutedEventArgs e)
    {
        if (_invoice == null)
        {
            // Save invoice first
            Save_Click(sender, e);
            return;
        }

        var paymentWindow = new PaymentWindow();
        paymentWindow.SetInvoice(_invoice);
        paymentWindow.ShowDialog(this);
    }
}