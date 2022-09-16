using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Au.Loaders
{
    internal static class Utils
    {
        readonly static string[] webprefix = new string[] { "jar:", "http:", "https:" };

        public static bool IsWebFile(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }

            path = path.ToLower();

            foreach (var item in webprefix)
            {
                if (path.StartsWith(item))
                {
                    return true;
                }
            }

            return false;
        }

        public static async Task WaitAsyncOperation(AsyncOperation op, Action<float> progress = null)
        {
            while (!op.isDone)
            {
                progress?.Invoke(op.progress);
                await Task.Yield();
            }
            progress?.Invoke(1);
        }

        public static async Task WaitUntil(Func<bool> condition)
        {
            while (!condition())
            {
                await Task.Yield();
            }
        }
    }
}
