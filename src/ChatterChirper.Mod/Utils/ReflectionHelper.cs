using System;
using System.Reflection;
using ChatterChirper.Utils;

namespace ChatterChirper.Utils
{
    public static class ReflectionHelper
    {
        public static string GetMessageID(object message)
        {
            if (message == null) return null;
            var type = message.GetType();

            // 1. Try public/private field "m_messageID" (Common in CitizenMessage)
            var field = type.GetField("m_messageID", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (field != null)
            {
                var val = field.GetValue(message);
                return val?.ToString();
            }

            // 2. Try property "messageID"
            var prop = type.GetProperty("messageID", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (prop != null)
            {
                var val = prop.GetValue(message, null);
                return val?.ToString();
            }

            // 3. Fallback: Log member names to help debug new discovery
            if (type.Name == "CitizenMessage") 
            {
                 ModLogger.Error($"[ReflectionHelper] Warning: Object is CitizenMessage but ID extraction failed.");
            }
            return null;
        }

        public static void LogObjectDetails(object obj)
        {
            if (obj == null) return;
            var t = obj.GetType();
            ModLogger.Info($"[Dump] Object Type: {t.FullName}");

            foreach (var p in t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                try {
                    var val = p.GetValue(obj, null);
                    ModLogger.Info($"[Dump] Prop: {p.Name} = {val}");
                } catch (Exception ex) {
                    ModLogger.Info($"[Dump] Prop: {p.Name} = <Error: {ex.Message}>");
                }
            }

            foreach (var f in t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                try {
                    var val = f.GetValue(obj);
                    ModLogger.Info($"[Dump] Field: {f.Name} = {val}");
                } catch (Exception ex) {
                    ModLogger.Info($"[Dump] Field: {f.Name} = <Error: {ex.Message}>");
                }
            }
        }
    }
}
