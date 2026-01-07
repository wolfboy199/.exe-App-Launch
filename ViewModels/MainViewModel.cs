using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using ExeLauncher.Models;
using ExeLauncher.Services;
using ExeLauncher.ViewModels.Helpers;

namespace ExeLauncher.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void Raise(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public ObservableCollection<string> RecentFiles { get; } = new ObservableCollection<string>();
        public ObservableCollection<Models.WorkspaceItem> WorkspaceItems { get; } = new ObservableCollection<Models.WorkspaceItem>();

        private string _selectedPath = string.Empty;
        public string SelectedPath
        {
            get => _selectedPath;
            set
            {
                _selectedPath = value;
                Raise(nameof(SelectedPath));
                (RunCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        private Models.WorkspaceItem? _selectedWorkspaceItem;
        public Models.WorkspaceItem? SelectedWorkspaceItem
        {
            get => _selectedWorkspaceItem;
            set
            {
                _selectedWorkspaceItem = value;
                if (value != null) SelectedPath = value.FullPath;
                Raise(nameof(SelectedWorkspaceItem));
                (RunCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public SettingsModel Settings { get; private set; }

        public RelayCommand BrowseCommand { get; }
        public RelayCommand RunCommand { get; }
        public RelayCommand SaveSettingsCommand { get; }

        private readonly SettingsManager _settingsManager;

        public MainViewModel()
        {
            _settingsManager = new SettingsManager();
            Settings = _settingsManager.Load();
            foreach (var f in Settings.RecentFiles) RecentFiles.Add(f);

            // Ensure workspace folder exists and load existing files into the simulated desktop
            try
            {
                if (string.IsNullOrWhiteSpace(Settings.WorkspaceFolder))
                {
                    var roaming = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
                    Settings.WorkspaceFolder = System.IO.Path.Combine(roaming, "ExeLauncher", "Workspace");
                }
                if (!System.IO.Directory.Exists(Settings.WorkspaceFolder)) System.IO.Directory.CreateDirectory(Settings.WorkspaceFolder);
                foreach (var file in System.IO.Directory.GetFiles(Settings.WorkspaceFolder))
                {
                    WorkspaceItems.Add(new Models.WorkspaceItem { FileName = System.IO.Path.GetFileName(file), FullPath = file, Extension = System.IO.Path.GetExtension(file) });
                }
            }
            catch { }

            BrowseCommand = new RelayCommand(_ => Browse());
            RunCommand = new RelayCommand(_ => RunSelected(), _ => !string.IsNullOrWhiteSpace(SelectedPath));
            SaveSettingsCommand = new RelayCommand(_ => SaveSettings());
        }

        private void Browse()
        {
            var dlg = new OpenFileDialog();
            dlg.Filter = "All files (*.*)|*.*";
            if (dlg.ShowDialog() == true)
            {
                SelectedPath = dlg.FileName;
                // Do not auto-add; user may click Run to open or add via drop
            }
        }

        public void AddFilesToWorkspace(string[] files)
        {
            var allowed = new[] { ".exe", ".zip", ".txt", ".png", ".jpg", ".jpeg", ".mp3", ".mp4" };
            foreach (var f in files)
            {
                try
                {
                    var ext = System.IO.Path.GetExtension(f).ToLowerInvariant();
                    if (Array.IndexOf(allowed, ext) < 0) continue;
                    var destName = System.IO.Path.GetFileName(f);
                    var destPath = System.IO.Path.Combine(Settings.WorkspaceFolder, destName);
                    // Avoid overwrite
                    if (System.IO.File.Exists(destPath))
                    {
                        var nameOnly = System.IO.Path.GetFileNameWithoutExtension(destName);
                        var uniq = nameOnly + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ext;
                        destPath = System.IO.Path.Combine(Settings.WorkspaceFolder, uniq);
                    }
                    System.IO.File.Copy(f, destPath);
                    var item = new Models.WorkspaceItem { FileName = System.IO.Path.GetFileName(destPath), FullPath = destPath, Extension = ext };
                    WorkspaceItems.Insert(0, item);
                }
                catch { }
            }
        }

        private void RunSelected()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SelectedPath)) return;
                var toRun = SelectedPath;
                if (SelectedWorkspaceItem != null) toRun = SelectedWorkspaceItem.FullPath;
                // Add to recent list and settings
                if (!RecentFiles.Contains(toRun))
                {
                    RecentFiles.Insert(0, toRun);
                    Settings.RecentFiles.Insert(0, toRun);
                    _settingsManager.Save(Settings);
                }

                FileHandler.Handle(toRun, Settings);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error running file: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveSettings()
        {
            _settingsManager.Save(Settings);
            MessageBox.Show("Settings saved.", "Settings", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
