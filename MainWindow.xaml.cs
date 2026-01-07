using System;
using System.IO;
using System.Windows;
using ExeLauncher.ViewModels;

namespace ExeLauncher
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void Window_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files != null && files.Length > 0 && Path.GetExtension(files[0]).Equals(".exe", StringComparison.OrdinalIgnoreCase))
                {
                    e.Effects = DragDropEffects.Copy;
                }
                else
                {
                    e.Effects = DragDropEffects.None;
                }
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            try
            {
                if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files == null || files.Length == 0) return;
                var first = files[0];
                // Accept multiple file types for the workspace; ViewModel will filter allowed ones.
                if (DataContext is ViewModels.MainViewModel vm)
                {
                    vm.AddFilesToWorkspace(files);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error handling drop: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void WorkspaceItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is System.Windows.Controls.Button b && b.DataContext is Models.WorkspaceItem wi && DataContext is ViewModels.MainViewModel vm)
                {
                    vm.SelectedWorkspaceItem = wi;
                }
            }
            catch { }
        }
    }
}
