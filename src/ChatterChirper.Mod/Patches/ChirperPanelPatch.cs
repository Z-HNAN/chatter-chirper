using System;
using ChatterChirper.Models;
using ChatterChirper.Utils;
using ICities;

namespace ChatterChirper.Patches
{
    public static class ChirperPanelPatch
    {
        // MVP: Simple fixed replacement
        private const string ReplacementText = "Chatter Chirper MVP: Hello World!";

        public static void Prefix(ref IChirperMessage message)
        {
            if (!ModConfig.Instance.Enabled) return;
            if (message == null) return;

            try
            {
                // Simple pass-through logging
                ModLogger.Info($"[Prefix] Intercepted message from {message.senderName}: {message.text}");

                // Replace content
                var proxy = new ChirperMessageProxy(message, ReplacementText);
                message = proxy;

                ModLogger.Info("[Prefix] Replaced with MVP Text.");
            }
            catch (Exception ex)
            {
                ModLogger.Error("Error in ChirpPanel Prefix", ex);
            }
        }
    }
}
