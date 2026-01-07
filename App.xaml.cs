using System;
using System.IO;
using System.Linq;
using System.Windows;
using ExeLauncher.ViewModels;

namespace ExeLauncher
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var main = new MainWindow();
            main.Show();

            try
            {
                // If startup argument provided, import workspace files (folder or single file)
                if (e.Args != null && e.Args.Length > 0)
                {
                    var arg = e.Args[0];
                    string[] files = Array.Empty<string>();
                    if (Directory.Exists(arg))
                    {
                        files = Directory.GetFiles(arg);
                    }
                    else if (File.Exists(arg))
                    {
                        files = new[] { arg };
                    }

                    if (files.Length > 0 && main.DataContext is MainViewModel vm)
                    {
                        vm.AddFilesToWorkspace(files);
                        MessageBox.Show($"Imported {files.Length} file(s) into workspace.", "Workspace Imported", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch { }
        }
    }
}
