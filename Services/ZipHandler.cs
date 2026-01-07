using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

namespace ExeLauncher.Services
{
    public static class ZipHandler
    {
        public static void ExtractAndOpen(string zipPath, string? extractionRoot)
        {
            if (string.IsNullOrWhiteSpace(zipPath) || !File.Exists(zipPath)) throw new FileNotFoundException("Zip not found", zipPath);
            var root = string.IsNullOrWhiteSpace(extractionRoot) ? Path.GetTempPath() : extractionRoot;
            var folder = Path.Combine(root, "ExeLauncher_" + Path.GetFileNameWithoutExtension(zipPath) + "_" + DateTime.Now.ToString("yyyyMMddHHmmss"));
            Directory.CreateDirectory(folder);
            try
            {
                ZipFile.ExtractToDirectory(zipPath, folder);
                // Open the extracted folder in explorer
                Process.Start(new ProcessStartInfo { FileName = folder, UseShellExecute = true });
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to extract zip: " + ex.Message, ex);
            }
        }
    }
}
