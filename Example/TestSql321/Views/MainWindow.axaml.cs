using Avalonia.Controls;
using System;
using TestSql321.Data;
using TestSql321.Models;
using TestSql321.ViewModels;

namespace TestSql321.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }

        private async void DataGrid_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
        {
            var selectedUser = MainDataGridUsers.SelectedItem as User;

            if (selectedUser == null) return;

            UserVariableData.seletedUserInMainWindow = selectedUser;

            var createAndChangeUserWindow = new CreateAndChangeUser();
            await createAndChangeUserWindow.ShowDialog(this);

            var viewModel = DataContext as MainWindowViewModel;
            viewModel.RefreshData();
        }

        private async void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            UserVariableData.seletedUserInMainWindow = null;

            var createAndChangeUserWindow = new CreateAndChangeUser();
            await createAndChangeUserWindow.ShowDialog(this);

            var viewModel = DataContext as MainWindowViewModel;
            viewModel.RefreshData();
        }
    }
}