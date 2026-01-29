using System;
using ChatterChirper.Models;
using ChatterChirper.Systems;
using ChatterChirper.Utils;
using ICities;

namespace ChatterChirper.Patches
{
    public static class ChirperPanelPatch
    {
        private static bool _libraryInitialized = false;

        public static void Prefix(ref IChirperMessage message)
        {
            if (!ModConfig.Instance.Enabled) return;
            if (message == null) return;

            try
            {
                ModLogger.Info($"[Prefix] Entry. Sender: {message.senderName} (ID:{message.senderID}), Text: {message.text}");

                if (!_libraryInitialized)
                {
                    MessageLibrary.Load();
                    _libraryInitialized = true;
                }

                // 1. Get the internal message ID (e.g. "CHIRP_NO_WATER")
                string msgID = ReflectionHelper.GetMessageID(message);
                
                if (string.IsNullOrEmpty(msgID))
                {
                    ModLogger.Info($"[SKIPPED] No ID found. DUMPING OBJECT DETAILS:");
                    ReflectionHelper.LogObjectDetails(message);
                    return;
                }

                ModLogger.Info($"[ID CHECK] Found ID: {msgID}");

                // 2. Check if we have a replacement for this ID
                string replacement = MessageLibrary.GetRandomText(msgID);
                
                if (!string.IsNullOrEmpty(replacement))
                {
                    // 3. Replace
                    string originalText = message.text;
                    var proxy = new ChirperMessageProxy(message, replacement);
                    message = proxy;
                    ModLogger.Info($"[REPLACED] Old: {originalText} with ID [{msgID}] -> New: {replacement}");
                }
                else
                {
                    ModLogger.Info($"[NO_MATCH] ID: {msgID} has no custom message");
                }
            }
            catch (Exception ex)
            {
                ModLogger.Error("Error in ChirpPanel Prefix", ex);
            }
        }
    }
}
