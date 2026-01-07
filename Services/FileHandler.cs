using System;
using System.Diagnostics;
using System.IO;
using ExeLauncher.Models;

namespace ExeLauncher.Services
{
    public static class FileHandler
    {
        public static void Handle(string pathOrUri, SettingsModel settings)
        {
            if (string.IsNullOrWhiteSpace(pathOrUri)) throw new ArgumentException("Path empty");

            // If it looks like a URI with scheme
            if (Uri.TryCreate(pathOrUri, UriKind.Absolute, out var uri) && !string.IsNullOrEmpty(uri.Scheme))
            {
                UrlHandler.OpenUrl(pathOrUri);
                return;
            }

            if (!File.Exists(pathOrUri)) throw new FileNotFoundException("File not found", pathOrUri);

            var ext = Path.GetExtension(pathOrUri).ToLowerInvariant();
            if (ext == ".exe")
            {
                var psi = new ProcessStartInfo { FileName = pathOrUri, UseShellExecute = true };
                Process.Start(psi);
                return;
            }
            if (ext == ".zip")
            {
                ZipHandler.ExtractAndOpen(pathOrUri, settings.DefaultExtractionFolder);
                return;
            }

            // Common file types: open with default associated application
            if (ext == ".txt" || ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".mp3" || ext == ".mp4")
            {
                var psi = new ProcessStartInfo { FileName = pathOrUri, UseShellExecute = true };
                Process.Start(psi);
                return;
            }

            throw new NotSupportedException("Unknown or unsupported file type.");
        }
    }
}
