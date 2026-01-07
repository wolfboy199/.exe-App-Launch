using System;
using System.Diagnostics;

namespace ExeLauncher.Services
{
    public static class UrlHandler
    {
        private static readonly string[] AllowedSchemes = new[] { "http", "https", "discord", "roblox" };
        public static void OpenUrl(string uri)
        {
            if (string.IsNullOrWhiteSpace(uri)) throw new ArgumentException("URI empty");
            if (!Uri.TryCreate(uri, UriKind.Absolute, out var u)) throw new ArgumentException("Invalid URI");
            if (Array.IndexOf(AllowedSchemes, u.Scheme.ToLowerInvariant()) < 0) throw new NotSupportedException("Scheme not allowed");

            Process.Start(new ProcessStartInfo { FileName = uri, UseShellExecute = true });
        }
    }
}
