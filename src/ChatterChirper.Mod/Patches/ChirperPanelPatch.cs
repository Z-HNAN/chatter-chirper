using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ChatterChirper.Models;
using ChatterChirper.Systems;
using ChatterChirper.Utils;
using HarmonyLib;
using ICities;

namespace ChatterChirper.Patches
{
    public static class ChirperPanelPatch
    {
        private static readonly object _initLock = new object();
        private static bool _initialized;
        private static List<MessageDefinition> _library = new List<MessageDefinition>();
        private static readonly TextSelector _selector = new TextSelector();
        private static readonly Random _random = new Random();
        private static string _libraryPath;

        public static void Prefix(ref IChirperMessage message)
        {
            if (!ModConfig.Instance.Enabled) return;
            if (message == null) return;

            try
            {
                EnsureLibrary();

                var originalText = message.text;
                var senderID = message.senderID;
                
                // Do not replace messages that are not from citizens (generic ID check, can be refined)
                // Actually, let's just replace everything for now if it matches criteria
                
                var ctx = CityStateReader.GetCurrentContext();
                ModLogger.Info("[Context] " + ctx.ToString());

                var selected = _selector.SelectMessage(ctx, _library);
                string newText = null;

                if (selected != null && selected.texts != null && selected.texts.Count > 0)
                {
                    var index = _random.Next(selected.texts.Count);
                    newText = selected.texts[index];
                }

                if (!string.IsNullOrEmpty(newText))
                {
                    // Create proxy to override text
                    var proxy = new ChirperMessageProxy(message, newText);
                    message = proxy;
                    
                    ModLogger.Info("Chirp replaced | Old=" + (originalText ?? "<null>") + " | New=" + newText + " | ID=" + selected?.id);
                }
                else
                {
                   ModLogger.Info("Chirp passthrough | Found=" + (selected != null) + " | Matches=" + (selected != null ? 1 : 0) + " | Lib=" + _library.Count);
                }
            }
            catch (Exception ex)
            {
                ModLogger.Error("Error in ChirpPanel Prefix: " + ex.Message, ex);
            }
        }

        private static void EnsureLibrary()
        {
            if (_initialized) return;
            lock (_initLock)
            {
                if (_initialized) return;
                try
                {
                    var dllDir = ResolveDllDirectory();
                    var modDir = ResolveModDirectory();

                    // Probe candidate paths; pick the first existing file, else default to dllDir.
                    var candidates = new List<string>();
                    candidates.Add(Path.Combine(dllDir, Constants.MessagesFileName));
                    if (!string.IsNullOrEmpty(modDir))
                        candidates.Add(Path.Combine(modDir, Constants.MessagesFileName));
                    candidates.Add(Path.Combine(Environment.CurrentDirectory, Constants.MessagesFileName));

                    string chosen = null;
                    foreach (var c in candidates)
                    {
                        if (File.Exists(c)) { chosen = c; break; }
                    }
                    if (chosen == null) chosen = candidates[0];

                    _libraryPath = chosen;
                    _library = TextProvider.LoadFromFile(_libraryPath);
                    ModLogger.Info("Library loaded from " + _libraryPath + " | Count=" + (_library != null ? _library.Count : 0));
                    if (_library == null) _library = new List<MessageDefinition>();
                }
                catch (Exception ex)
                {
                    ModLogger.Error("Failed to load library: " + ex.Message, ex);
                    _library = new List<MessageDefinition>();
                }
                _initialized = true;
            }
        }

        private static string ResolveDllDirectory()
        {
            try
            {
                var asm = Assembly.GetExecutingAssembly();
                // Prefer CodeBase to avoid 'file:' prefix issues
                var codeBase = asm.CodeBase;
                if (!string.IsNullOrEmpty(codeBase))
                {
                    var uri = new Uri(codeBase);
                    var path = uri.LocalPath; // unescapes and strips file:
                    var dir = Path.GetDirectoryName(path);
                    if (!string.IsNullOrEmpty(dir)) return dir;
                }

                var loc = asm.Location;
                if (!string.IsNullOrEmpty(loc))
                {
                    var dir = Path.GetDirectoryName(loc);
                    if (!string.IsNullOrEmpty(dir)) return dir;
                }
            }
            catch (Exception ex)
            {
                ModLogger.Warning("ResolveDllDirectory failed: " + ex.Message);
            }

            // Fallback to current directory
            return Environment.CurrentDirectory;
        }

        private static string ResolveModDirectory()
        {
            try
            {
                var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var p1 = Path.Combine(localAppData, "Colossal Order");
                var p2 = Path.Combine(p1, "Cities_Skylines");
                var p3 = Path.Combine(p2, "Addons");
                var p4 = Path.Combine(p3, "Mods");
                var p5 = Path.Combine(p4, "ChatterChirper");
                return p5;
            }
            catch (Exception ex)
            {
                ModLogger.Warning("ResolveModDirectory failed: " + ex.Message);
                return null;
            }
        }

        private static string ExtractText(object message)
        {
            if (message == null) return null;

            // common property/field names seen in chirp messages
            string[] candidates = { "m_Text", "m_text", "text", "Text", "message", "Message" };
            var t = message.GetType();

            foreach (var name in candidates)
            {
                var prop = t.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (prop != null && prop.CanRead)
                {
                    var val = prop.GetValue(message, null);
                    if (val != null) return val.ToString();
                }

                var field = t.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (field != null)
                {
                    var val = field.GetValue(message);
                    if (val != null) return val.ToString();
                }
            }

            return null;
        }

        private static bool TrySetText(object message, string newText)
        {
            if (message == null) return false;
            var t = message.GetType();
            
            // Priority 1: Direct property setter (text, m_Text, Message)
            string[] propCandidates = { "text", "Text", "m_Text", "m_text", "message", "Message" };
            foreach (var name in propCandidates)
            {
                var prop = t.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (prop != null && prop.CanWrite)
                {
                    try
                    {
                        prop.SetValue(message, newText, null);
                        return true;
                    }
                    catch { }
                }
            }

            // Priority 2: Direct fields (m_text, etc.)
            string[] fieldCandidates = { "m_text", "m_Text", "text", "Text", "_text", "message", "Message" };
            foreach (var name in fieldCandidates)
            {
                var field = t.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (field != null && !field.IsInitOnly)
                {
                    try
                    {
                        field.SetValue(message, newText);
                        return true;
                    }
                    catch { }
                }
            }

            // Priority 3: Backing fields for properties (e.g. <text>k__BackingField)
            foreach (var name in propCandidates)
            {
                var backingName = string.Format("<{0}>k__BackingField", name);
                var field = t.GetField(backingName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (field != null && !field.IsInitOnly)
                {
                    try
                    {
                        field.SetValue(message, newText);
                        return true;
                    }
                    catch { }
                }
            }
            
            ModLogger.Warning("TrySetText failed to find writable member on " + t.FullName);
            return false;
        }

        private static string ExtractMembers(object message, int maxItems)
        {
            if (message == null) return null;
            var t = message.GetType();
            var items = new List<string>();

            foreach (var p in t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (!p.CanRead) continue;
                try
                {
                    var val = p.GetValue(message, null);
                    items.Add("prop " + p.Name + "=" + SafeToString(val));
                }
                catch { }
                if (items.Count >= maxItems) break;
            }

            if (items.Count < maxItems)
            {
                foreach (var f in t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    try
                    {
                        var val = f.GetValue(message);
                        items.Add("field " + f.Name + "=" + SafeToString(val));
                    }
                    catch { }
                    if (items.Count >= maxItems) break;
                }
            }

            if (items.Count == 0) return null;
            return string.Join("; ", items.ToArray());
        }

        private static string SafeToString(object obj)
        {
            try
            {
                return obj != null ? obj.ToString() : "<null>";
            }
            catch (Exception ex)
            {
                return "<ToString error: " + ex.GetType().Name + ">";
            }
        }
    }
}