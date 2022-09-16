using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Au.Loaders
{
    public abstract class Loader
    {
        /// <summary>
        /// Enable or disable detail logs
        /// </summary>
        public static bool enableDetailLogs
        {
            get
            {
                return Logger.enableDetailLogs;
            }
            set
            {
                Logger.enableDetailLogs = value;
            }
        }

        /// <summary>
        /// Patch data path
        /// </summary>
        public static string patchDataPath { get; set; } = "patch_data";

        protected readonly Dictionary<string, PendingItem> pendingItems = new Dictionary<string, PendingItem>();

        /// <summary>
        /// Load a text from a file or url
        /// </summary>
        /// <param name="path"></param>
        /// <returns>text if succ, else return string.Empty</returns>
        public async Task<string> LoadText(string path)
        {
            byte[] bytes;
            if (Utils.IsWebFile(path))
            {
                var webrequest = UnityWebRequest.Get(path);
                webrequest.downloadHandler = new DownloadHandlerBuffer();
                var req = webrequest.SendWebRequest();
                await Utils.WaitAsyncOperation(req);
                if (webrequest.result != UnityWebRequest.Result.Success)
                {
                    Logger.Error($"load file {path} failed: {webrequest.error} ({webrequest.responseCode})");
                    return string.Empty;
                }
                bytes = webrequest.downloadHandler.data;
            }
            else
            {
                if (!File.Exists(path))
                {
                    Logger.Error($"{path} not found");
                    return string.Empty;
                }
                bytes = File.ReadAllBytes(path);
            }
            string str = Encoding.UTF8.GetString(bytes);
            return str;
        }

        /// <summary>
        /// Load a bundle by name
        /// </summary>
        /// <param name="name">Bundle name</param>
        /// <param name="progress">Loading progress</param>
        /// <returns>result: true or false</returns>
        public virtual async Task<bool> LoadBundle(string name, Action<float> progress)
        {
            return await PendingLock("bundle", name, async () =>
            {
                return await LoadingBundle(name, progress);
            });
        }

        /// <summary>
        /// Unload a bundle, all resources in bundle will be unload
        /// </summary>
        /// <param name="name">Bundle name</param>
        /// <returns></returns>
        public abstract bool UnloadBundle(string name);

        /// <summary>
        /// Load a scene 
        /// </summary>
        /// <param name="name">scene name</param>
        /// <param name="progress">Loading progress</param>
        /// <returns></returns>
        public virtual async Task<Scene> LoadScene(string name, bool additive, Action<float> progress)
        {
            return await PendingLock("scene", name, async () =>
            {
                var scene = await LoadingScene(name, additive, progress);
                return scene;
            });
        }

        /// <summary>
        /// Unload scene
        /// </summary>
        /// <param name="scene">Scene</param>
        /// <returns>true</returns>
        public virtual async Task<bool> UnloadScene(Scene scene)
        {
            var op = SceneManager.UnloadSceneAsync(scene);
            await Utils.WaitAsyncOperation(op);
            return true;
        }

        /// <summary>
        /// Load an asset
        /// </summary>
        /// <param name="path">Asset path like 'Assets/xxx/yyy.prefab'</param>
        /// <param name="type">Asset type</param>
        /// <returns>asset if succ, else return null</returns>
        public virtual async Task<UnityEngine.Object> LoadAsset(string path, Type type)
        {
            return await PendingLock("asset", path, async () =>
            {
                return await LoadingAsset(path, type);
            });
        }

        /// <summary>
        /// Load a scene
        /// </summary>
        /// <param name="name"></param>
        /// <param name="additive"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        protected abstract Task<Scene> LoadingScene(string name, bool additive, Action<float> progress);

        /// <summary>
        /// Load an asset
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        protected abstract Task<UnityEngine.Object> LoadingAsset(string path, Type type);

        /// <summary>
        /// Load a bundle
        /// </summary>
        /// <param name="name"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        protected abstract Task<bool> LoadingBundle(string name, Action<float> progress);

        protected async Task<T> PendingLock<T>(string type, string id, Func<Task<T>> func)
        {
            string key = $"{type}_{id}";
            if (pendingItems.TryGetValue(key, out var item))
            {
                await item.Wait();
                return item.GetData<T>();
            }

            item = new PendingItem();
            pendingItems.Add(key, item);
            var data = await func();
            item.SetData(data);
            pendingItems.Remove(key);
            return data;
        }
    }
}
