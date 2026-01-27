using HarmonyLib;
using System.Reflection;
using ChatterChirper.Utils;

namespace ChatterChirper
{
    public static class Patcher
    {
        private static bool patched = false;

        public static void PatchAll()
        {
            if (patched) return;

            var harmony = new Harmony(Constants.HarmonyId);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            patched = true;
            ModLogger.Info("Harmony Patches Applied");
        }

        public static void UnpatchAll()
        {
            if (!patched) return;

            var harmony = new Harmony(Constants.HarmonyId);
            harmony.UnpatchAll(Constants.HarmonyId);
            patched = false;
            ModLogger.Info("Harmony Patches Removed");
        }
    }
}