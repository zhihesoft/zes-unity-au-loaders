using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Au.Loaders
{
    public class LoaderForRuntime : Loader
    {
        protected Dictionary<string, AssetBundle> bundles = new Dictionary<string, AssetBundle>();
        protected Dictionary<string, string> assets2bundle = new Dictionary<string, string>();

        public override bool UnloadBundle(string name)
        {
            if (bundles.TryGetValue(name, out var bundle))
            {
                bundles.Remove(name);
                assets2bundle = assets2bundle.Where(i => i.Value != name).ToDictionary(i => i.Key, i => i.Value);
                bundle.Unload(true);
            }
            return true;
        }

        protected override async Task<UnityEngine.Object> LoadingAsset(string path, Type type)
        {
            if (!assets2bundle.TryGetValue(path, out var item))
            {
                Logger.Error($"Cannot find bundle for {path}");
                return null;
            }

            if (!bundles.TryGetValue(item, out var bundle))
            {
                Logger.Error($"Bundle {item} not loaded");
                return null;
            }

            Debug.Assert(bundle != null, $"bundle ({item}) cannot be null");
            var op = bundle.LoadAssetAsync(path, type);
            await Utils.WaitAsyncOperation(op);
            return op.asset;
        }

        protected override async Task<bool> LoadingBundle(string name, Action<float> progress)
        {
            if (bundles.TryGetValue(name, out var bundle))
            {
                return true;
            }

            name = name.ToLower();
            string bundlePath = Path.Combine(Application.persistentDataPath, patchDataPath, name);

            if (!File.Exists(bundlePath))
            {
                bundlePath = Path.Combine(Application.streamingAssetsPath, name);
            }

            Debug.Assert(File.Exists(bundlePath), $"cannot find bundle in path: ({bundlePath})");

            var bundlereq = AssetBundle.LoadFromFileAsync(bundlePath);
            await Utils.WaitAsyncOperation(bundlereq, progress);
            bundle = bundlereq.assetBundle;

            Debug.Assert(bundle != null, $"load bundle {name} failed, result is null");
            Logger.Info($"bundle {name} loaded");
            bundles.Add(name, bundle);
            if (!bundle.isStreamedSceneAssetBundle)
            {
                bundle.GetAllAssetNames()
                    .ToList()
                    .ForEach(i => assets2bundle.Add(i, name));
            }

            return true;
        }

        protected override async Task<Scene> LoadingScene(string name, bool additive, Action<float> progress)
        {
            Scene loadedScene = default(Scene);
            UnityAction<Scene, LoadSceneMode> loadCallback = (scene, mode) =>
            {
                loadedScene = scene;
            };
            SceneManager.sceneLoaded += loadCallback;
            var op = SceneManager.LoadSceneAsync(name, additive ? LoadSceneMode.Additive : LoadSceneMode.Single);
            await Utils.WaitAsyncOperation(op, progress);
            SceneManager.sceneLoaded -= loadCallback;
            return loadedScene;
        }
    }
}
