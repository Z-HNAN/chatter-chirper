using System;
using System.Collections.Generic;
using System.IO;
using ChatterChirper.Models;
using ChatterChirper.Utils;

namespace ChatterChirper.Systems
{
    public static class TextProvider
    {
        public static List<MessageDefinition> LoadFromFile(string path)
        {
            var list = new List<MessageDefinition>();
            if (string.IsNullOrEmpty(path)) return list;

            try
            {
                if (!File.Exists(path))
                {
                    ModLogger.Warning("TextProvider: messages file not found at " + path);
                    return list;
                }

                var json = File.ReadAllText(path);
                list = Parse(json);
                ModLogger.Info("TextProvider loaded messages: " + list.Count);
            }
            catch (Exception ex)
            {
                ModLogger.Error("TextProvider failed to load: " + ex.Message, ex);
            }

            return list;
        }

        public static List<MessageDefinition> Parse(string json)
        {
            var list = new List<MessageDefinition>();
            if (string.IsNullOrEmpty(json)) return list;

            try
            {
                // Strip UTF-8 BOM if present
                if (json.Length > 0 && json[0] == '\ufeff')
                {
                    json = json.Substring(1);
                }
                json = json.Trim();

                var rootObj = MiniJSON.Deserialize(json) as Dictionary<string, object>;
                if (rootObj == null || !rootObj.ContainsKey("messages"))
                {
                    ModLogger.Warning("TextProvider parse: root null or missing 'messages'");
                    return list;
                }

                var msgs = rootObj["messages"] as List<object>;
                if (msgs == null)
                {
                    ModLogger.Warning("TextProvider parse: 'messages' not an array");
                    return list;
                }

                foreach (var m in msgs)
                {
                    var dict = m as Dictionary<string, object>;
                    if (dict == null) continue;

                    var msg = new MessageDefinition();

                    msg.id = GetString(dict, "id");
                    msg.category = GetString(dict, "category");
                    msg.severity = GetInt(dict, "severity", 0);
                    msg.weight = GetFloat(dict, "weight", 1.0f);
                    msg.cooldown = GetInt(dict, "cooldown", 600);

                    // conditions
                    msg.conditions = new Dictionary<string, string>();
                    if (dict.ContainsKey("conditions"))
                    {
                        var cond = dict["conditions"] as Dictionary<string, object>;
                        if (cond != null)
                        {
                            foreach (var kv in cond)
                            {
                                msg.conditions[kv.Key] = kv.Value != null ? kv.Value.ToString() : string.Empty;
                            }
                        }
                    }

                    // texts
                    msg.texts = new List<string>();
                    if (dict.ContainsKey("texts"))
                    {
                        var arr = dict["texts"] as List<object>;
                        if (arr != null)
                        {
                            foreach (var t in arr)
                            {
                                if (t != null) msg.texts.Add(t.ToString());
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(msg.id) && msg.texts.Count > 0)
                    {
                        list.Add(msg);
                    }
                }
            }
            catch (Exception ex)
            {
                ModLogger.Error("TextProvider parse error: " + ex.Message, ex);
            }

            return list;
        }

        private static string GetString(Dictionary<string, object> dict, string key)
        {
            object val;
            if (dict.TryGetValue(key, out val) && val != null) return val.ToString();
            return null;
        }

        private static int GetInt(Dictionary<string, object> dict, string key, int defaultValue)
        {
            object val;
            if (dict.TryGetValue(key, out val) && val != null)
            {
                int parsed;
                if (int.TryParse(val.ToString(), out parsed)) return parsed;
            }
            return defaultValue;
        }

        private static float GetFloat(Dictionary<string, object> dict, string key, float defaultValue)
        {
            object val;
            if (dict.TryGetValue(key, out val) && val != null)
            {
                float parsed;
                if (float.TryParse(val.ToString(), out parsed)) return parsed;
            }
            return defaultValue;
        }
    }
}