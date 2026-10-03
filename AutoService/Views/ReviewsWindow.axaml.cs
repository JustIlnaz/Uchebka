using Avalonia.Controls;
using AutoService.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AutoService.Views;

public partial class ReviewsWindow : Window
{
    public ReviewsWindow()
    {
        InitializeComponent();
        LoadReviews();
    }

    private void LoadReviews()
    {
        using var db = new AppDbContext();
        var reviews = db.Reviews
            .Include(r => r.Client)
            .Include(r => r.WorkOrder)
            .OrderByDescending(r => r.CreatedAt)
            .ToList();
        ReviewsListBox.ItemsSource = reviews;
    }

    private async void AddButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var win = new ReviewEditWindow();
        await win.ShowDialog(this);
        LoadReviews();
    }
}
