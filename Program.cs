using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ExeLauncher
{
    // Legacy console helper class. Main() was removed to allow WPF App.xaml startup.
    public static class LegacyConsoleLauncher
    {
        public static void LaunchApp(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) return;
            if (!File.Exists(filePath)) return;
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                };
                Process.Start(startInfo);
            }
            catch { }
        }
        public static void LaunchUri(string uri)
        {
            if (string.IsNullOrWhiteSpace(uri)) return;
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = uri,
                    UseShellExecute = true
                };
                Process.Start(startInfo);
            }
            catch { }
        }
    }
}
