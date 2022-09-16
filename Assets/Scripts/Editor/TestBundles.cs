using Au.Loaders;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class TestBundles
{
    [MenuItem("TEST/Add Bundles")]
    public static void AddBundles()
    {
        ResourceBuilder.AutoCreateBundleNames("Bundles");
    }

    [MenuItem("TEST/Build Bundles")]
    public static void BuildBundles()
    {
        ResourceBuilder.BuildBundles("AssetBundles", EditorUserBuildSettings.activeBuildTarget);
    }

    [MenuItem("TEST/Calc MD5")]
    public static void CalcMD5()
    {
        var dict = ResourceBuilder.CalcBundleHash("AssetBundles");
        dict.ToList().ForEach(i =>
        {
            Debug.Log($"{i.Key} => {i.Value}");
        });
    }
}
