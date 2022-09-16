using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace Au.Loaders
{
    /// <summary>
    /// Build for Resource Manager
    /// </summary>
    public static class ResourceBuilder
    {
        /// <summary>
        /// Auto create bundle names in dir Assets/{path}
        /// Assume each sub-dir is a bundle
        /// </summary>
        /// <param name="path"></param>
        public static void AutoCreateBundleNames(string path)
        {
            var bundles = new DirectoryInfo(Path.Combine("Assets", path));
            var dirs = bundles.GetDirectories();
            foreach (var dir in dirs)
            {
                var importer = AssetImporter.GetAtPath(Path.Combine("Assets", path, dir.Name));
                importer.assetBundleName = dir.Name.ToLower();
            }
            AssetDatabase.RemoveUnusedAssetBundleNames();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Build bundles to output path dir
        /// </summary>
        /// <param name="outputPath"></param>
        /// <param name="target"></param>
        public static void BuildBundles(string outputPath, BuildTarget target)
        {
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.None, target);
        }

        /// <summary>
        /// Calc md5 for all bundle in outputPath dir
        /// </summary>
        /// <param name="outputPath"></param>
        /// <returns></returns>
        public static Dictionary<string, string> CalcBundleHash(string outputPath)
        {
            DirectoryInfo man = new DirectoryInfo(outputPath);
            Assert.IsTrue(man.Exists);
            string allbundlepath = Path.Combine(outputPath, man.Name);
            var allbundle = AssetBundle.LoadFromFile(allbundlepath);
            var manifest = allbundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            var bundles = manifest.GetAllAssetBundles();

            var ret = bundles.AsParallel().ToDictionary(i => i, i =>
            {
                var item = AssetBundle.LoadFromFile(Path.Combine(outputPath, i));
                string md5sum = "";
                string[] assets;
                if (item.isStreamedSceneAssetBundle)
                {
                    assets = item.GetAllScenePaths();
                }
                else
                {
                    assets = item.GetAllAssetNames();
                }
                md5sum = assets.AsParallel().Aggregate("", (last, value) =>
                {
                    var calc = CalcAssetMD5(value);
                    return last + calc;
                });
                return CalcMD5(md5sum);
            });
            AssetBundle.UnloadAllAssetBundles(true);
            return ret;
        }

        private static string CalcAssetMD5(string path)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            var m1 = CalcFileMD5(path);
            var m2 = CalcFileMD5(path + ".meta");
            return CalcMD5(m1 + m2);
        }

        private static string CalcFileMD5(string path)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var bytes = md5.ComputeHash(fs);
                var ret = BitConverter.ToString(bytes).ToLower().Replace("-", "");
                return ret;
            }
        }

        private static string CalcMD5(string text)
        {
            return CalcMD5(Encoding.UTF8.GetBytes(text));
        }

        private static string CalcMD5(byte[] bytes)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            var md5bytes = md5.ComputeHash(bytes);
            var ret = BitConverter.ToString(md5bytes).ToLower().Replace("-", "");
            return ret;
        }
    }
}
