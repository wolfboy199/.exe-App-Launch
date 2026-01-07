using System.Collections.Generic;

namespace ExeLauncher.Models
{
    public class SettingsModel
    {
        public string DefaultExtractionFolder { get; set; } = "";
        public string WorkspaceFolder { get; set; } = "";
        public string Theme { get; set; } = "Light";
        public List<string> RecentFiles { get; set; } = new List<string>();
    }
}
