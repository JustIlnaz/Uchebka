using Avalonia.Controls;
using AutoService.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace AutoService.Views;

public partial class ReviewEditWindow : Window
{
    public ReviewEditWindow()
    {
        InitializeComponent();

        using var db = new AppDbContext();
        ClientCombo.ItemsSource = db.Clients.OrderBy(c => c.FullName).ToList();
        WorkOrderCombo.ItemsSource = db.WorkOrders
            .Include(w => w.Appointment)
            .ThenInclude(a => a.Car)
            .OrderByDescending(w => w.Id)
            .ToList();
    }

    private void Save_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var client = ClientCombo.SelectedItem as Client;
        var workOrder = WorkOrderCombo.SelectedItem as WorkOrder;
        if (client == null || workOrder == null) return;

        using var db = new AppDbContext();
        db.Reviews.Add(new Review
        {
            ClientId = client.Id,
            WorkOrderId = workOrder.Id,
            Rating = (int)RatingInput.Value,
            Comment = CommentText.Text,
            CreatedAt = DateTime.Now
        });
        db.SaveChanges();
        Close();
    }
}
