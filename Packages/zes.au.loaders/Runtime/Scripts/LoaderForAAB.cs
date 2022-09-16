// will move to zes.au.loaders.aab library
//#if USING_AAB
//using System;
//using System.IO;
//using System.Threading.Tasks;
//using UnityEngine;
//using Google.Play.AssetDelivery;


//namespace Au.Loaders
//{
//    internal class LoaderForAAB : LoaderForRuntime
//    {

//        protected override async Task<bool> LoadingBundle(string name, Action<float> progress)
//        {
//            if (bundles.TryGetValue(name, out var bundle))
//            {
//                return true;
//            }

//            Logger.Info($"AAB loader loading bundle ({name})");
//            name = name.ToLower();
//            string bundlePath = Path.Combine(Application.persistentDataPath, patchDataPath, name);
//            if (!File.Exists(bundlePath))
//            {
//                Logger.Info($"{bundlePath} not found, load from aab");
//                PlayAssetBundleRequest bundleReq = PlayAssetDelivery.RetrieveAssetBundleAsync(name);
//                await Utils.WaitUntil(() => !bundleReq.keepWaiting);
//                if (bundleReq.Status != AssetDeliveryStatus.Loaded)
//                {
//                    Logger.Error($"cannot load bundle ({name}) from aab");
//                    return false;
//                }
//                return bundleReq.AssetBundle;
//            }
//            else
//            {
//                var ret = await base.LoadBundle(name, progress);
//                return ret;
//            }
//        }
//    }
//}
//#endif
