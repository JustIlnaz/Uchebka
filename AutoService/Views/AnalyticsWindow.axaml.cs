using Avalonia.Controls;
using AutoService.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoService.Views;

public partial class AnalyticsWindow : Window
{
    public AnalyticsWindow()
    {
        InitializeComponent();
    }

    private void SummaryButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        using var db = new AppDbContext();
        var totalOrders = db.WorkOrders.Count();
        var completedOrders = db.WorkOrders.Count(w => w.Status == "Завершён");
        var totalRevenue = db.Payments.Sum(p => p.Amount);
        var avgCheck = db.WorkOrders.Average(w => (decimal?)w.TotalCost) ?? 0;

        var stats = new List<SummaryStat>
        {
            new() { Metric = "Всего заказов", Value = totalOrders.ToString() },
            new() { Metric = "Завершено заказов", Value = completedOrders.ToString() },
            new() { Metric = "Общая выручка", Value = totalRevenue.ToString("C") },
            new() { Metric = "Средний чек", Value = avgCheck.ToString("C") }
        };
        ResultsGrid.ItemsSource = stats;
    }

    private void RevenueButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        using var db = new AppDbContext();
        var revenue = db.Payments
            .GroupBy(p => new { p.Date.Year, p.Date.Month })
            .Select(g => new RevenueStat
            {
                Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                Total = g.Sum(p => p.Amount)
            })
            .OrderBy(x => x.Month)
            .ToList();
        ResultsGrid.ItemsSource = revenue;
    }

    private void TopServicesButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        using var db = new AppDbContext();
        var top = db.WorkOrderServices
            .GroupBy(ws => ws.Service.Name)
            .Select(g => new TopServiceStat
            {
                Service = g.Key,
                Count = g.Sum(x => x.Quantity),
                Revenue = g.Sum(x => x.Price * x.Quantity)
            })
            .OrderByDescending(x => x.Count)
            .Take(10)
            .ToList();
        ResultsGrid.ItemsSource = top;
    }

    private void MechanicLoadButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        using var db = new AppDbContext();
        var load = db.WorkOrders
            .GroupBy(w => w.Mechanic.User.FullName)
            .Select(g => new MechanicLoadStat
            {
                Mechanic = g.Key,
                Orders = g.Count(),
                TotalRevenue = g.Sum(x => x.TotalCost)
            })
            .OrderByDescending(x => x.Orders)
            .ToList();
        ResultsGrid.ItemsSource = load;
    }

    private void LowStockButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        using var db = new AppDbContext();
        var low = db.Parts
            .Where(p => p.QuantityInStock < p.MinQuantity)
            .Select(p => new LowStockStat
            {
                Sku = p.Sku,
                InStock = p.QuantityInStock,
                MinQuantity = p.MinQuantity
            })
            .ToList();
        ResultsGrid.ItemsSource = low;
    }

    // Вложенный запрос: услуги, стоимость которых выше средней
    private void AboveAvgServicesButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        using var db = new AppDbContext();
        var avg = db.Services.Average(s => s.BasePrice);
        var list = db.Services
            .Where(s => s.BasePrice > avg)
            .OrderByDescending(s => s.BasePrice)
            .Select(s => new AboveAvgServiceStat { Service = s.Name, Price = s.BasePrice, AvgPrice = avg })
            .ToList();
        ResultsGrid.ItemsSource = list;
    }

    // Вложенный запрос: клиенты с несколькими обращениями
    private void RepeatClientsButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        using var db = new AppDbContext();
        var list = db.Clients
            .Where(c => db.Appointments.Count(a => a.ClientId == c.Id) > 1)
            .Select(c => new RepeatClientStat
            {
                Client = c.FullName,
                Visits = db.Appointments.Count(a => a.ClientId == c.Id)
            })
            .OrderByDescending(x => x.Visits)
            .ToList();
        ResultsGrid.ItemsSource = list;
    }

    // Вложенный запрос: механики, выполнившие больше среднего количества заказов
    private void AboveAvgMechanicsButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        using var db = new AppDbContext();
        var avgOrders = db.Mechanics
            .Select(m => db.WorkOrders.Count(w => w.MechanicId == m.Id))
            .DefaultIfEmpty(0)
            .Average();

        var list = db.Mechanics
            .Include(m => m.User)
            .Select(m => new AboveAvgMechanicStat
            {
                Mechanic = m.User != null ? m.User.FullName ?? "?" : "?",
                Orders = db.WorkOrders.Count(w => w.MechanicId == m.Id)
            })
            .ToList()
            .Where(x => x.Orders > avgOrders)
            .OrderByDescending(x => x.Orders)
            .ToList();
        ResultsGrid.ItemsSource = list;
    }

    // ROLLUP: выручка по годам и месяцам с промежуточными итогами
    private void RollupButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        using var db = new AppDbContext();
        var list = db.Database.SqlQuery<RollupRow>($"""
            SELECT EXTRACT(YEAR FROM "Date")::int AS "Year",
                   EXTRACT(MONTH FROM "Date")::int AS "Month",
                   SUM("Amount") AS "Total"
            FROM "Payments"
            GROUP BY ROLLUP (EXTRACT(YEAR FROM "Date"), EXTRACT(MONTH FROM "Date"))
            ORDER BY "Year" NULLS LAST, "Month" NULLS LAST
            """).ToList();
        ResultsGrid.ItemsSource = list;
    }

    // CUBE: количество заказов и выручка по статусу заказа и механику
    private void CubeButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        using var db = new AppDbContext();
        var list = db.Database.SqlQuery<CubeRow>($"""
            SELECT COALESCE(w."Status", 'ИТОГО') AS "Status",
                   COALESCE(u."FullName", 'Все механики') AS "Mechanic",
                   COUNT(*) AS "Orders",
                   COALESCE(SUM(w."TotalCost"), 0) AS "Total"
            FROM "WorkOrders" w
            LEFT JOIN "Mechanics" m ON w."MechanicId" = m."Id"
            LEFT JOIN "Users" u ON m."UserId" = u."Id"
            GROUP BY CUBE (w."Status", u."FullName")
            ORDER BY "Status", "Mechanic"
            """).ToList();
        ResultsGrid.ItemsSource = list;
    }

    private class SummaryStat { public string Metric { get; set; } = ""; public string Value { get; set; } = ""; }
    private class RevenueStat { public string Month { get; set; } = ""; public decimal Total { get; set; } }
    private class TopServiceStat { public string Service { get; set; } = ""; public int Count { get; set; } public decimal Revenue { get; set; } }
    private class MechanicLoadStat { public string Mechanic { get; set; } = ""; public int Orders { get; set; } public decimal TotalRevenue { get; set; } }
    private class LowStockStat { public string Sku { get; set; } = ""; public int InStock { get; set; } public int MinQuantity { get; set; } }
    private class AboveAvgServiceStat { public string Service { get; set; } = ""; public decimal Price { get; set; } public decimal AvgPrice { get; set; } }
    private class RepeatClientStat { public string Client { get; set; } = ""; public int Visits { get; set; } }
    private class AboveAvgMechanicStat { public string Mechanic { get; set; } = ""; public int Orders { get; set; } }
    public class RollupRow { public int? Year { get; set; } public int? Month { get; set; } public decimal Total { get; set; } }
    public class CubeRow { public string Status { get; set; } = ""; public string Mechanic { get; set; } = ""; public int Orders { get; set; } public decimal Total { get; set; } }
}
