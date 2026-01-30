using System;
using System.IO;
using UnityEngine;

namespace ChatterChirper.Utils
{
    public static class ModLogger
    {
        private const string ModName = "ChatterChirper";
        private static readonly object _lock = new object();
        private static string _logDir;
        private static string _logPath;
        private static bool _initialized;

        private static void EnsureInitialized()
        {
            if (_initialized) return;

            try
            {
                var platform = Environment.OSVersion.Platform;
                if (platform == PlatformID.Unix || platform == PlatformID.MacOSX)
                {
                    // macOS path (Steam default) — adjust if needed for Linux
                    var home = Environment.GetEnvironmentVariable("HOME") ?? "~";
                    var p1 = Path.Combine(home, "Library");
                    var p2 = Path.Combine(p1, "Application Support");
                    var p3 = Path.Combine(p2, "Colossal Order");
                    var p4 = Path.Combine(p3, "Cities_Skylines");
                    var p5 = Path.Combine(p4, "Addons");
                    var p6 = Path.Combine(p5, "Mods");
                    _logDir = Path.Combine(p6, ModName);
                }
                else
                {
                    // Windows
                    var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    var p1 = Path.Combine(localAppData, "Colossal Order");
                    var p2 = Path.Combine(p1, "Cities_Skylines");
                    var p3 = Path.Combine(p2, "Addons");
                    var p4 = Path.Combine(p3, "Mods");
                    _logDir = Path.Combine(p4, ModName);
                }

                if (!Directory.Exists(_logDir)) Directory.CreateDirectory(_logDir);

                _logPath = Path.Combine(_logDir, ModName + ".log");

                // Optional: simple rotation if file too large (>5MB)
                try
                {
                    if (File.Exists(_logPath))
                    {
                        var fi = new FileInfo(_logPath);
                        if (fi.Length > 5 * 1024 * 1024)
                        {
                            var ts = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                            File.Move(_logPath, Path.Combine(_logDir, ModName + "." + ts + ".log"));
                        }
                    }
                }
                catch { /* ignore rotation errors */ }

                _initialized = true;
            }
            catch (Exception ex)
            {
                // Fallback: still allow Unity console logging
                Debug.LogError("[" + ModName + "] Logger init failed: " + ex.Message);
                _initialized = true; // avoid repeated failures
            }
        }

        private static void Write(string level, string message)
        {
            if (!Constants.EnableLogging) return;

            EnsureInitialized();
            var line = "[" + ModName + "] [" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "] [" + level + "] " + message;

            // Unity console
            if (level == "ERROR") Debug.LogError(line);
            else if (level == "WARN") Debug.LogWarning(line);
            else Debug.Log(line);

            // File output
            try
            {
                lock (_lock)
                {
                    if (!string.IsNullOrEmpty(_logPath)) File.AppendAllText(_logPath, line + Environment.NewLine);
                }
            }
            catch { /* swallow file I/O issues to avoid crashing */ }
        }

        public static void Info(string message) => Write("INFO", message);
        public static void Warning(string message) => Write("WARN", message);
        public static void Error(string message) => Write("ERROR", message);

        // Optional helper for exceptions
        public static void Error(string message, Exception ex) => Write("ERROR", message + " | " + ex.GetType().Name + ": " + ex.Message);

        // Expose log path for diagnostics
        public static string LogFilePath
        {
            get { EnsureInitialized(); return _logPath; }
        }
    }
}