using UnityEngine;

namespace ChatterChirper.Utils
{
    public static class ModLogger
    {
        private const string Prefix = "[ChatterChirper] ";

        public static void Info(string message)
        {
            Debug.Log(Prefix + message);
        }

        public static void Error(string message)
        {
            Debug.LogError(Prefix + message);
        }
        
        public static void Warning(string message)
        {
            Debug.LogWarning(Prefix + message);
        }
    }
}