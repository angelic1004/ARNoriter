using UnityEditor;

public class UninstallNatCorder
{
    private const string UNINSTALL_SCRIPT_ASSET = "Assets/HansAR/Scripts/Editor/UninstallNatCorder.cs";
    private const string UNINSTALL_MENUITEM = "Assets/Uninstall Package/NatCorder";
    private const string UNINSTALL_TITLE = "NatCorder";

    private static string[] assetPathNames =
    {
        "Assets/NatCorder",
        "Assets/HansAR/Scripts/UI/NatCorderRecordScreenUI.cs"
    };

    [MenuItem(UNINSTALL_MENUITEM, true)]
    private static bool PackageHasInstalled()
    {
        return UnityPackageUtility.HasInstalled(assetPathNames);
    }

    [MenuItem(UNINSTALL_MENUITEM, false, 24)]
    private static void UninstallPackage()
    {
        if (UnityPackageUtility.Uninstall(UNINSTALL_TITLE, assetPathNames))
        {
            AssetDatabase.MoveAssetToTrash(UNINSTALL_SCRIPT_ASSET);
        }
    }
}
