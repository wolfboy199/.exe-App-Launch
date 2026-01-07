using System;
using System.IO;
using System.Text.Json;
using ExeLauncher.Models;

namespace ExeLauncher.Services
{
    public class SettingsManager
    {
        private readonly string _path;
        public SettingsManager()
        {
            var roaming = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var dir = Path.Combine(roaming, "ExeLauncher");
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            _path = Path.Combine(dir, "settings.json");
        }

        public SettingsModel Load()
        {
            try
            {
                if (!File.Exists(_path))
                {
                    var settingsDir = Path.GetDirectoryName(_path) ?? Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    var workspace = Path.Combine(settingsDir, "Workspace");
                    var def = new SettingsModel { DefaultExtractionFolder = Path.GetTempPath(), WorkspaceFolder = workspace, Theme = "Light" };
                    if (!Directory.Exists(def.WorkspaceFolder)) Directory.CreateDirectory(def.WorkspaceFolder);
                    Save(def);
                    return def;
                }
                var txt = File.ReadAllText(_path);
                return JsonSerializer.Deserialize<SettingsModel>(txt) ?? new SettingsModel();
            }
            catch
            {
                return new SettingsModel();
            }
        }

        public void Save(SettingsModel model)
        {
            try
            {
                var txt = JsonSerializer.Serialize(model, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_path, txt);
            }
            catch { }
        }
    }
}
