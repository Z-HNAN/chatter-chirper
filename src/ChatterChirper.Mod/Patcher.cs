using HarmonyLib;
using System.Reflection;
using ChatterChirper.Utils;
using System;
using System.Collections.Generic;

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

            // Dynamically patch ChirpPanel.AddMessage if available
            try
            {
                var chirpPanelType = AccessTools.TypeByName("ChirpPanel")
                                     ?? AccessTools.TypeByName("ColossalFramework.UI.ChirpPanel");
                if (chirpPanelType == null)
                {
                    ModLogger.Warning("ChirpPanel type not found. Skipping ChirpPanel patch.");
                }
                else
                {
                    var methods = AccessTools.GetDeclaredMethods(chirpPanelType);

                    var addMessageMethods = new List<MethodInfo>();
                    foreach (var m in methods)
                    {
                        if (m.Name == "AddMessage") addMessageMethods.Add(m);
                    }

                    if (addMessageMethods.Count == 0)
                    {
                        ModLogger.Warning("ChirpPanel.AddMessage not found. Skipping ChirpPanel patch.");
                    }
                    else
                    {
                        // Log all overloads for diagnostics
                        foreach (var m in addMessageMethods)
                        {
                            ModLogger.Info("Found AddMessage overload: " + DescribeMethod(m));
                        }

                        MethodInfo original = null;

                        // prefer single-parameter overload whose parameter type name contains "Chirp" or "Message"
                        foreach (var m in addMessageMethods)
                        {
                            var ps = m.GetParameters();
                            if (ps.Length == 1)
                            {
                                var pName = ps[0].ParameterType.Name ?? string.Empty;
                                if (pName.IndexOf("Chirp", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                    pName.IndexOf("Message", StringComparison.OrdinalIgnoreCase) >= 0)
                                {
                                    original = m;
                                    break;
                                }
                            }
                        }

                        if (original == null)
                        {
                            // fallback to first overload
                            original = addMessageMethods[0];
                        }

                        // Patch only the overload with the most parameters to avoid double-patching if one calls another
                        MethodInfo targetMethod = null;
                        int maxParams = -1;
                        
                        foreach (var m in addMessageMethods)
                        {
                            var pParams = m.GetParameters();
                            if (pParams.Length > maxParams)
                            {
                                maxParams = pParams.Length;
                                targetMethod = m;
                            }
                        }

                        if (targetMethod != null)
                        {
                            var prefix = new HarmonyMethod(typeof(Patches.ChirperPanelPatch).GetMethod("Prefix", BindingFlags.Public | BindingFlags.Static));
                            harmony.Patch(targetMethod, prefix: prefix);
                            ModLogger.Info("Patched ChirpPanel.AddMessage -> " + DescribeMethod(targetMethod));
                        }
                        else
                        {
                            ModLogger.Warning("No suitable AddMessage overload found to patch.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModLogger.Error("Failed to patch ChirpPanel: " + ex.Message, ex);
            }
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

        private static string DescribeMethod(MethodInfo m)
        {
            if (m == null) return "<null method>";
            var pars = m.GetParameters();
            var parts = new List<string>();
            foreach (var p in pars)
            {
                parts.Add(p.ParameterType != null ? p.ParameterType.FullName : "<null>");
            }
            var paramSig = string.Join(", ", parts.ToArray());
            return m.DeclaringType.FullName + "." + m.Name + "(" + paramSig + ")";
        }
    }
}