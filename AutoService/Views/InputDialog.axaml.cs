using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AutoService.Views;

public partial class InputDialog : Window
{
    public InputDialog() { InitializeComponent(); }
    public InputDialog(string prompt) { InitializeComponent(); Title = prompt; }
    public InputDialog(string prompt, string initial) { InitializeComponent(); Title = prompt; InputText.Text = initial; }

    public static async System.Threading.Tasks.Task ShowMessage(Window owner, string message)
    {
        var d = new InputDialog();
        d.Title = message;
        d.Width = 360;
        d.Height = 100;
        d.InputText.IsVisible = false;
        d.CancelButton.IsVisible = false;
        await d.ShowDialog<string?>(owner);
    }

    private void Ok_Click(object? sender, RoutedEventArgs e)
    {
        Close(InputText.Text);
    }

    private void Cancel_Click(object? sender, RoutedEventArgs e)
    {
        Close(null);
    }
}
