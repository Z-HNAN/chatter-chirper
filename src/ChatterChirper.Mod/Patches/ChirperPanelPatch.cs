using System;
using System.Collections.Generic;
using System.Reflection;
using ChatterChirper.Systems;
using ChatterChirper.Utils;
using HarmonyLib;
using ICities;

namespace ChatterChirper.Patches
{
    public static class ChirperPanelPatch
    {
        public static void Prefix(object message)
        {
            if (!ModConfig.Instance.Enabled) return;

            try
            {
                var msgType = message?.GetType();
                var text = ExtractText(message);
                var extras = ExtractMembers(message, 6);
                ModLogger.Info("Chirp received | Type=" + (msgType?.FullName ?? "<null>") + " | Text=" + (text ?? "<null>") + " | ToString=" + SafeToString(message) + (extras != null ? " | Members=" + extras : string.Empty));
            }
            catch (Exception ex)
            {
                ModLogger.Error("Error in ChirpPanel Prefix: " + ex.Message, ex);
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