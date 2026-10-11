using Avalonia.Controls;
using AutoService.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoService.Views;

public partial class AnalyticsWindow : Window
{
    public AnalyticsWindow()
    {
        InitializeComponent();
    }

    private async void SummaryButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        await LoadReportAsync(db =>
        {
            var totalOrders = db.WorkOrders.Count();
            var completedOrders = db.WorkOrders.Count(w => w.Status == "Завершён");
            var totalRevenue = SuccessfulPayments(db).Sum(p => (decimal?)p.Amount) ?? 0m;
            var avgCheck = db.WorkOrders
                .Where(w => w.Status == "Завершён")
                .Average(w => (decimal?)w.TotalCost) ?? 0m;

            return new List<SummaryStat>
            {
                new() { Metric = "Всего заказов", Value = totalOrders.ToString() },
                new() { Metric = "Завершено заказов", Value = completedOrders.ToString() },
                new() { Metric = "Выручка по завершённым платежам", Value = totalRevenue.ToString("C") },
                new() { Metric = "Средний чек", Value = avgCheck.ToString("C") }
            };
        });
    }

    private async void RevenueButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        await LoadReportAsync(db => SuccessfulPayments(db)
            .GroupBy(p => new { p.Date.Year, p.Date.Month })
            .Select(g => new RevenueStat
            {
                Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                Total = g.Sum(p => p.Amount)
            })
            .OrderBy(x => x.Month)
            .ToList());
    }

    private async void TopServicesButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        await LoadReportAsync(db => db.WorkOrderServices
            .Where(ws => ws.WorkOrder != null && ws.WorkOrder.Status == "Завершён")
            .GroupBy(ws => ws.Service != null ? ws.Service.Name : "Удалённая услуга")
            .Select(g => new TopServiceStat
            {
                Service = g.Key,
                Count = g.Sum(x => x.Quantity),
                Revenue = g.Sum(x => x.Price * x.Quantity)
            })
            .OrderByDescending(x => x.Count)
            .Take(10)
            .ToList());
    }

    private async void MechanicLoadButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        await LoadReportAsync(db => db.WorkOrders
            .Where(w => w.Status != "Отменён" && w.Status != "Отменен")
            .GroupBy(w => w.Mechanic != null && w.Mechanic.User != null
                ? w.Mechanic.User.FullName ?? "Без имени"
                : "Не назначен")
            .Select(g => new MechanicLoadStat
            {
                Mechanic = g.Key,
                Orders = g.Count(),
                TotalOrderValue = g.Sum(x => x.TotalCost)
            })
            .OrderByDescending(x => x.Orders)
            .ToList());
    }

    private async void LowStockButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        await LoadReportAsync(db => db.Parts
            .Where(p => p.QuantityInStock < p.MinQuantity)
            .Select(p => new LowStockStat
            {
                Sku = p.Sku,
                InStock = p.QuantityInStock,
                MinQuantity = p.MinQuantity
            })
            .ToList());
    }

    private async void AboveAvgServicesButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        await LoadReportAsync(db =>
        {
            var avg = db.Services.Average(s => (decimal?)s.BasePrice) ?? 0m;
            return db.Services
                .Where(s => s.BasePrice > avg)
                .OrderByDescending(s => s.BasePrice)
                .Select(s => new AboveAvgServiceStat { Service = s.Name, Price = s.BasePrice, AvgPrice = avg })
                .ToList();
        });
    }

    private async void RepeatClientsButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        await LoadReportAsync(db => db.Clients
            .Where(c => db.Appointments.Count(a => a.ClientId == c.Id
                && a.Status != "Отменена" && a.Status != "Отменен") > 1)
            .Select(c => new RepeatClientStat
            {
                Client = c.FullName,
                Visits = db.Appointments.Count(a => a.ClientId == c.Id
                    && a.Status != "Отменена" && a.Status != "Отменен")
            })
            .OrderByDescending(x => x.Visits)
            .ToList());
    }

    private async void AboveAvgMechanicsButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        await LoadReportAsync(db =>
        {
            var counts = db.Mechanics
                .Select(m => db.WorkOrders.Count(w => w.MechanicId == m.Id && w.Status == "Завершён"))
                .ToList();
            var avgOrders = counts.Count == 0 ? 0 : counts.Average();
            return db.Mechanics
                .Include(m => m.User)
                .Select(m => new AboveAvgMechanicStat
                {
                    Mechanic = m.User != null ? m.User.FullName ?? "Без имени" : "Не назначен",
                    Orders = db.WorkOrders.Count(w => w.MechanicId == m.Id && w.Status == "Завершён")
                })
                .ToList()
                .Where(x => x.Orders > avgOrders)
                .OrderByDescending(x => x.Orders)
                .ToList();
        });
    }

    private static IQueryable<Payment> SuccessfulPayments(AppDbContext db) =>
        db.Payments.Where(p => p.Status == "Завершён" || p.Status == "Успешно");

    private async Task LoadReportAsync<T>(Func<AppDbContext, List<T>> query)
    {
        ReportStatusText.Text = "Формирование отчёта…";
        try
        {
            using var db = new AppDbContext();
            var results = query(db);
            ResultsGrid.ItemsSource = results;
            ReportStatusText.Text = results.Count == 0
                ? "По выбранному отчёту данных нет."
                : $"Записей: {results.Count}";
        }
        catch (Exception ex)
        {
            ResultsGrid.ItemsSource = null;
            ReportStatusText.Text = $"Не удалось сформировать отчёт: {ex.GetBaseException().Message}";
            await InputDialog.ShowMessage(this, ReportStatusText.Text);
        }
    }

    private async void RollupButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        await LoadReportAsync(db => db.Database.SqlQuery<RollupRow>($"""
            WITH "PaymentPeriods" AS (
                SELECT EXTRACT(YEAR FROM "Date")::int AS "Year",
                       EXTRACT(MONTH FROM "Date")::int AS "Month",
                       "Amount"
                FROM "Payments"
                WHERE "Status" IN ('Завершён', 'Успешно')
            )
            SELECT CASE
                       WHEN GROUPING("Year") = 1 THEN 'Общий итог'
                       WHEN GROUPING("Month") = 1 THEN 'Итого за год'
                       ELSE 'По месяцам'
                   END AS "Period",
                   "Year",
                   "Month",
                   COALESCE(SUM("Amount"), 0)::numeric AS "Total"
            FROM "PaymentPeriods"
            GROUP BY ROLLUP ("Year", "Month")
            ORDER BY GROUPING("Year"), "Year", GROUPING("Month"), "Month"
            """).ToList());
    }

    private async void CubeButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        await LoadReportAsync(db => db.Database.SqlQuery<CubeRow>($"""
            WITH "WorkOrderRows" AS (
                SELECT w."Id", w."Status", w."MechanicId",
                       COALESCE(u."FullName", 'Без имени') AS "Mechanic",
                       w."TotalCost"
                FROM "WorkOrders" w
                LEFT JOIN "Mechanics" m ON w."MechanicId" = m."Id"
                LEFT JOIN "Users" u ON m."UserId" = u."Id"
            )
            SELECT CASE
                       WHEN GROUPING(w."Status") = 1 AND GROUPING(w."MechanicId") = 1 THEN 'Общий итог'
                       WHEN GROUPING(w."Status") = 1 THEN 'Итог по механику'
                       WHEN GROUPING(w."MechanicId") = 1 THEN 'Итог по статусу'
                       ELSE 'Детализация'
                   END AS "Level",
                   CASE
                       WHEN GROUPING(w."Status") = 1 THEN 'Все статусы'
                       ELSE COALESCE(w."Status", 'Статус не указан')
                   END AS "Status",
                   CASE
                       WHEN GROUPING(w."MechanicId") = 1 THEN 'Все механики'
                       WHEN w."MechanicId" IS NULL THEN 'Не назначен'
                       ELSE MAX(w."Mechanic")
                   END AS "Mechanic",
                   COUNT(w."Id")::bigint AS "Orders",
                   COALESCE(SUM(w."TotalCost"), 0)::numeric AS "TotalCost"
            FROM "WorkOrderRows" w
            GROUP BY CUBE (w."Status", w."MechanicId")
            ORDER BY GROUPING(w."Status"), w."Status", GROUPING(w."MechanicId"), "Mechanic"
            """).ToList());
    }

    public class SummaryStat { public string Metric { get; set; } = ""; public string Value { get; set; } = ""; }
    public class RevenueStat { public string Month { get; set; } = ""; public decimal Total { get; set; } }
    public class TopServiceStat { public string Service { get; set; } = ""; public int Count { get; set; } public decimal Revenue { get; set; } }
    public class MechanicLoadStat { public string Mechanic { get; set; } = ""; public int Orders { get; set; } public decimal TotalOrderValue { get; set; } }
    public class LowStockStat { public string? Sku { get; set; } public int InStock { get; set; } public int MinQuantity { get; set; } }
    public class AboveAvgServiceStat { public string Service { get; set; } = ""; public decimal Price { get; set; } public decimal AvgPrice { get; set; } }
    public class RepeatClientStat { public string Client { get; set; } = ""; public int Visits { get; set; } }
    public class AboveAvgMechanicStat { public string Mechanic { get; set; } = ""; public int Orders { get; set; } }
    public class RollupRow { public string Period { get; set; } = ""; public int? Year { get; set; } public int? Month { get; set; } public decimal Total { get; set; } }
    public class CubeRow { public string Level { get; set; } = ""; public string Status { get; set; } = ""; public string Mechanic { get; set; } = ""; public long Orders { get; set; } public decimal TotalCost { get; set; } }
}
