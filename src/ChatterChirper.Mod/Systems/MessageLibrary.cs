using System;
using System.Collections.Generic;
using System.IO;
using ChatterChirper.Utils;

namespace ChatterChirper.Systems
{
    public static class MessageLibrary
    {
        private static Dictionary<string, List<string>> _messages = new Dictionary<string, List<string>>();
        private static readonly Random _random = new Random();
        private static bool _loaded = false;

        public static void Load()
        {
            if (_loaded) return;
            
            try
            {
                string foundPath = null;
                var candidates = GetCandidatePaths();

                foreach(var p in candidates)
                {
                    if(File.Exists(p)) 
                    {
                        foundPath = p;
                        break;
                    }
                    ModLogger.Info($"Checked: {p} (Not Found)");
                }

                if (foundPath == null)
                {
                    ModLogger.Warning("messages.json not found in any candidate path.");
                    return;
                }

                ModLogger.Info($"Loading messages from: {foundPath}");
                string json = File.ReadAllText(foundPath);
                
                if (string.IsNullOrEmpty(json)) {
                     ModLogger.Error("JSON file is empty.");
                     return;
                }

                // Trim to avoid MiniJSON failure on leading whitespace/BOM
                json = json.Trim();

                // Debug JSON content
                if (json.Length > 50) ModLogger.Info($"JSON Preview: {json.Substring(0, 50)}...");
                
                var dict = MiniJSON.Deserialize(json) as Dictionary<string, object>;
                
                if (dict != null)
                {
                    foreach (var kvp in dict)
                    {
                        var list = kvp.Value as List<object>;
                        if (list != null)
                        {
                            var texts = new List<string>();
                            foreach(var item in list) texts.Add(item.ToString());
                            
                            // Log loaded keys for debugging
                            ModLogger.Info($"[MessageLibrary] Loaded key: {kvp.Key} with {texts.Count} messages");
                            
                            _messages[kvp.Key] = texts;
                        }
                    }
                    ModLogger.Info($"MessageLibrary Loaded {_messages.Count} categories.");
                }
                else
                {
                    ModLogger.Error("MiniJSON returned null (Failed to parse JSON).");
                }
            }
            catch (Exception ex)
            {
                ModLogger.Error("Failed to load MessageLibrary", ex);
            }
            _loaded = true;
        }

        private static List<string> GetCandidatePaths()
        {
            var list = new List<string>();
            string fileName = Constants.MessagesFileName;
            
            // 1. Assembly CodeBase
            try {
                string codebase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                if (!string.IsNullOrEmpty(codebase)) {
                    var uri = new Uri(codebase);
                    string path = Path.Combine(Path.GetDirectoryName(uri.LocalPath), fileName);
                    if (!list.Contains(path)) list.Add(path);
                }
            } catch {}

            // 2. Assembly Location
            try {
                 string loc = System.Reflection.Assembly.GetExecutingAssembly().Location;
                 if (!string.IsNullOrEmpty(loc)) {
                    string path = Path.Combine(Path.GetDirectoryName(loc), fileName);
                    if (!list.Contains(path)) list.Add(path);
                 }
            } catch {}

            // 3. Local App Data (Explicit fallback for local dev)
            try {
                var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var subPath = Path.Combine("Colossal Order", Path.Combine("Cities_Skylines", Path.Combine("Addons", Path.Combine("Mods", "ChatterChirper"))));
                var p = Path.Combine(localAppData, Path.Combine(subPath, fileName));
                if (!list.Contains(p)) list.Add(p);
            } catch {}
            
            return list;
        }

        public static string GetRandomText(string messageID)
        {
            if (string.IsNullOrEmpty(messageID)) return null;
            
            // Allow matching "LocaleID" constants or approximation
            // The game IDs are often like "CHIRP_..."
            // We just do direct lookup.
            
            if (_messages.TryGetValue(messageID, out var texts) && texts.Count > 0)
            {
                return texts[_random.Next(texts.Count)];
            }
            
            return null;
        }
    }
}
