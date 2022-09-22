using Au.Loaders;
using UnityEditor;

public static class TestBundles
{
    [MenuItem("TEST/Auto Add Bundles")]
    public static void AddBundles()
    {
        ResourceBuilder.AutoCreateBundleNames("Assets/Bundles");
    }

    [MenuItem("TEST/Build Bundles")]
    public static void BuildBundles()
    {
        ResourceBuilder.BuildBundles("AssetBundles", EditorUserBuildSettings.activeBuildTarget);
    }
}
