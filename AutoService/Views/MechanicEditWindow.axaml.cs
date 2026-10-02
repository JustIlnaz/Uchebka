using Avalonia.Controls;
using AutoService.Data;
using AutoService.Models;
using System.Linq;

namespace AutoService.Views;

public partial class MechanicEditWindow : Window
{
    public MechanicEditWindow()
    {
        InitializeComponent();

        using var db = new AppDbContext();
        UserCombo.ItemsSource = db.Users.ToList();
        DepartmentCombo.ItemsSource = db.Departments.ToList();
        SpecializationCombo.ItemsSource = db.Specializations.ToList();

        if (MechanicVariableData.SelectedMechanic != null)
        {
            var m = MechanicVariableData.SelectedMechanic;
            UserCombo.SelectedItem = db.Users.FirstOrDefault(u => u.Id == m.UserId);
            DepartmentCombo.SelectedItem = db.Departments.FirstOrDefault(d => d.Id == m.DepartmentId);
            SpecializationCombo.SelectedItem = db.Specializations.FirstOrDefault(s => s.Id == m.SpecializationId);
        }
    }

    private void Save_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        using var db = new AppDbContext();
        var user = UserCombo.SelectedItem as User;
        var dept = DepartmentCombo.SelectedItem as Department;
        var spec = SpecializationCombo.SelectedItem as Specialization;

        if (MechanicVariableData.SelectedMechanic != null)
        {
            var id = MechanicVariableData.SelectedMechanic.Id;
            var m = db.Mechanics.FirstOrDefault(x => x.Id == id);
            if (m != null)
            {
                m.UserId = user?.Id;
                m.DepartmentId = dept?.Id;
                m.SpecializationId = spec?.Id;
            }
        }
        else
        {
            var m = new Mechanic
            {
                UserId = user?.Id,
                DepartmentId = dept?.Id,
                SpecializationId = spec?.Id
            };
            db.Mechanics.Add(m);
        }

        db.SaveChanges();
        this.Close();
    }
}
