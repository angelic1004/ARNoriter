using System.IO;
using UnityEditor;
using UnityEngine;

public class CreateAssetBundles
{
    static string AssetBundleName = "HansData/";

    [MenuItem("한스앱/에셋번들 빌드")]
    static void BuildAllAssetBundles()
    {
        string 에셋번들경로 = Path.Combine(AssetBundleName, 
            GetPlatformForAssetBundles(EditorUserBuildSettings.activeBuildTarget));

        if (!Directory.Exists(에셋번들경로))
        {
            Directory.CreateDirectory(에셋번들경로);
        }

        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
        {
            BuildPipeline.BuildAssetBundles(에셋번들경로,
                BuildAssetBundleOptions.None, BuildTarget.Android); 
        }
        else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
        {
            BuildPipeline.BuildAssetBundles(에셋번들경로,
                BuildAssetBundleOptions.None, BuildTarget.iOS);
        }

        AssetDatabase.Refresh();
    }

#if UNITY_EDITOR
    private static string GetPlatformForAssetBundles(BuildTarget target)
    {
        switch (target)
        {
            case BuildTarget.Android:
                return "Android";

            case BuildTarget.iOS:
                return "iOS";

            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
                return "Windows";

            case BuildTarget.StandaloneOSXIntel:
            case BuildTarget.StandaloneOSXIntel64:
            case BuildTarget.StandaloneOSXUniversal:
                return "OSX";

            default:
                return null;
        }
    }
#endif
}