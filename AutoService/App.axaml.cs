using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using AutoService.Data;
using AutoService.ViewModels;
using AutoService.Views;

namespace AutoService;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Применяем миграции и заполняем БД начальными данными
            using (var db = new AppDbContext())
            {
                db.Database.Migrate();
                DbSeeder.Seed(db);
            }

            desktop.MainWindow = new LoginWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
}