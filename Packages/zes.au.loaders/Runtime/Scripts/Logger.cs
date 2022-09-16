using UnityEngine;

namespace Au.Loaders
{
    internal static class Logger
    {
        private static readonly string tag = "RESMAN";

        public static bool enableDetailLogs = false;

        public static void Info(string message)
        {
            if (enableDetailLogs)
            {
                Debug.Log($"[{tag}] {message}");
            }
        }

        public static void Warn(string message)
        {
            Debug.LogWarning($"[{tag}] {message}");
        }

        public static void Error(string message)
        {
            Debug.LogError($"[{tag}] {message}");
        }

    }
}
