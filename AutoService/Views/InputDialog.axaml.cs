using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AutoService.Views;

public partial class InputDialog : Window
{
    public InputDialog() { InitializeComponent(); }
    public InputDialog(string prompt) { InitializeComponent(); Title = prompt; }
    public InputDialog(string prompt, string initial) { InitializeComponent(); Title = prompt; InputText.Text = initial; }

    private void Ok_Click(object? sender, RoutedEventArgs e)
    {
        Close(InputText.Text);
    }

    private void Cancel_Click(object? sender, RoutedEventArgs e)
    {
        Close(null);
    }
}
