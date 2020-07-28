using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System.IO;

public class UnityPackageUtility
{
    private static string projectPath = null;

    public static string GetProjectPath()
    {
        if (projectPath == null)
        {
            projectPath = Directory.GetParent(Application.dataPath).FullName.Replace("\\", "/");
        }
        return projectPath;
    }

    private static string GetPackagePath(string packageName)
    {
        if (string.IsNullOrEmpty(packageName))
        {
            return string.Format("{0}/(none).unitypackage", GetProjectPath());
        }
        if (packageName.ToLower().EndsWith(".unitypackage"))
        {
            return string.Format("{0}/{1}", GetProjectPath(), packageName);
        }
        return string.Format("{0}/{1}.unitypackage", GetProjectPath(), packageName);
    }

    private static string[] GetPackageList(string listFileName)
    {
        if (string.IsNullOrEmpty(listFileName))
        {
            return null;
        }
        string listFilePath = string.Format("{0}/{1}.filelist", GetProjectPath(), listFileName);
        if (!File.Exists(listFilePath))
        {
            return null;
        }
        List<string> sortedList = new List<string>();
        string[] list = File.ReadAllLines(listFilePath);
        foreach (string path in list)
        {
            if (!string.IsNullOrEmpty(path) && path.IndexOfAny(Path.GetInvalidPathChars()) == -1)
            {
                sortedList.Add(path);
            }
        }
        if (sortedList.Count > 0)
        {
            return sortedList.ToArray();
        }
        return null;
    }

    private static string GetAssetPathName(string assetFileName)
    {
        if (string.IsNullOrEmpty(assetFileName))
        {
            return string.Empty;
        }
        assetFileName = assetFileName.Replace("\\", "/");
        string projectPath = GetProjectPath().ToLower() + "/";
        if (assetFileName.ToLower().StartsWith(projectPath))
        {
            return assetFileName.Remove(0, projectPath.Length);
        }
        return assetFileName;
    }

    public static bool Exists(string packageName)
    {
        return File.Exists(GetPackagePath(packageName));
    }

    public static bool HasInstalled(string[] assetPathNames)
    {
        if (assetPathNames == null)
        {
            return false;
        }
        for (int i = 0; i < assetPathNames.Length; i++)
        {
            string assetPath = string.Format("{0}/{1}", GetProjectPath(), assetPathNames[i]);
            if (Directory.Exists(assetPath))
            {
                return true;
            }
            else if (File.Exists(assetPath))
            {
                return true;
            }
        }
        return false;
    }

    public static bool Import(string packageName, bool interactive)
    {
        if (EditorApplication.isPlaying)
        {
            Debug.LogError("Failed to import package with error: Cannot import package in play mode.");
            return false;
        }
        string packagePath = GetPackagePath(packageName);
        if (File.Exists(packagePath))
        {
            AssetDatabase.ImportPackage(packagePath, interactive);
        }
        else if (interactive)
        {
            packagePath = EditorUtility.OpenFilePanel("Import package ...", GetProjectPath(), "unitypackage");
            if (!string.IsNullOrEmpty(packagePath))
            {
                AssetDatabase.ImportPackage(packagePath, interactive);
                return true;
            }
        }
        return false;
    }

    public static bool Import(string packageName)
    {
        return Import(packageName, true);
    }

    public static bool Export(string packageName, string[] assetPathNames, bool interactive)
    {
        if (assetPathNames == null)
        {
            return false;
        }
        List<string> targetPathNames = new List<string>();
        for (int i = 0; i < assetPathNames.Length; i++)
        {
            string assetPath = string.Format("{0}/{1}", GetProjectPath(), assetPathNames[i]);
            if (Directory.Exists(assetPath))
            {
                targetPathNames.Add(assetPathNames[i]);
                string[] assetFiles = Directory.GetFiles(assetPath, "*.*", SearchOption.AllDirectories);
                foreach (string assetFile in assetFiles)
                {
                    targetPathNames.Add(GetAssetPathName(assetFile));
                }
            }
            else if (File.Exists(assetPath))
            {
                targetPathNames.Add(assetPathNames[i]);
            }
            else
            {
                Debug.Log("Export package : Asset path \"" + assetPathNames[i] + "\" doesn't exists ");
            }
        }
        if (targetPathNames.Count > 0)
        {
            string packagePath = GetPackagePath(packageName);
            if (string.IsNullOrEmpty(packageName) || interactive)
            {
                packagePath = EditorUtility.SaveFilePanel("Export package ...", GetProjectPath(), packageName, "unitypackage");
            }
            if (!string.IsNullOrEmpty(packagePath))
            {
                ExportPackageOptions options = ExportPackageOptions.Default;
                if (interactive)
                {
                    options |= ExportPackageOptions.Interactive;
                }
                AssetDatabase.ExportPackage(targetPathNames.ToArray(), packagePath, options);
                return true;
            }
        }
        return false;
    }

    public static bool Export(string packageName, string[] assetPathNames)
    {
        return Export(packageName, assetPathNames, true);
    }

    public static bool Export(string packageName, string listFileName, bool interactive)
    {
        return Export(packageName, GetPackageList(listFileName), interactive);
    }

    public static bool Export(string packageName, string listFileName)
    {
        return Export(packageName, GetPackageList(listFileName), true);
    }

    public static bool RemoveComponents(string title, string[] assetPathNames, bool interactive)
    {
        if (assetPathNames == null)
        {
            return false;
        }
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            return false;
        }
        float progress = 0f;
        if (interactive)
        {
            EditorUtility.DisplayProgressBar(title, "Gathering files...", progress);
        }
        List<string> scriptNames = new List<string>();
        for (int i = 0; i < assetPathNames.Length; i++)
        {
            string assetPath = string.Format("{0}/{1}", GetProjectPath(), assetPathNames[i]);
            if (Directory.Exists(assetPath))
            {
                string[] assetFiles = Directory.GetFiles(assetPath, "*.*", SearchOption.AllDirectories);
                foreach (string assetFile in assetFiles)
                {
                    MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(GetAssetPathName(assetFile));
                    if (script != null)
                    {
                        scriptNames.Add(script.name);
                    }
                }
            }
            else if (File.Exists(assetPath))
            {
                MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPathNames[i]);
                if (script != null)
                {
                    scriptNames.Add(script.name);
                }
            }
        }
        if (interactive)
        {
            EditorUtility.ClearProgressBar();
        }
        if (scriptNames.Count > 0)
        {
            if (interactive)
            {
                EditorUtility.DisplayProgressBar(title, "Removing components...", progress);
            }
            SceneSetup[] sceneSetup = EditorSceneManager.GetSceneManagerSetup();
            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                Scene scene = EditorSceneManager.OpenScene(EditorBuildSettings.scenes[i].path);
                MonoBehaviour[] components = GameObject.FindObjectsOfType<MonoBehaviour>();
                foreach (string scriptName in scriptNames)
                {
                    if (interactive)
                    {
                        progress = (i + 1) / (float)EditorBuildSettings.scenes.Length;
                        EditorUtility.DisplayProgressBar(title, "Removing components...", progress);
                    }
                    foreach (MonoBehaviour component in components)
                    {
                        if (string.Compare(scriptName, component.GetType().ToString()) == 0)
                        {
                            Transform t = component.transform;
                            Object.DestroyImmediate(component);
                            Component[] slibings = t.GetComponents<Component>();
                            if (t.childCount == 0 && slibings.Length == 1 && t == slibings[0])
                            {
                                Object.DestroyImmediate(t.gameObject);
                            }
                            EditorSceneManager.MarkSceneDirty(scene);
                        }
                    }
                }
                if (scene.isDirty)
                {
                    EditorSceneManager.SaveScene(scene);
                }
            }
            if (sceneSetup == null || sceneSetup.Length == 0)
            {
                EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
            }
            else
            {
                bool hasActive = false;
                for (int i = 0; i < sceneSetup.Length; i++)
                {
                    if (sceneSetup[i].isActive)
                    {
                        hasActive = true;
                        break;
                    }
                }
                if (!hasActive)
                {
                    sceneSetup[0].isActive = true;
                }
                EditorSceneManager.RestoreSceneManagerSetup(sceneSetup);
            }
            if (interactive)
            {
                EditorUtility.ClearProgressBar();
            }
        }
        return true;
    }

    public static bool RemoveComponents(string title, string[] assetPathNames)
    {
        return RemoveComponents(title, assetPathNames);
    }

    public static bool RemoveComponents(string title, string listFileName, bool interactive)
    {
        return RemoveComponents(title, GetPackageList(listFileName), interactive);
    }

    public static bool RemoveComponents(string title, string listFileName)
    {
        return RemoveComponents(title, GetPackageList(listFileName), true);
    }

    public static bool DeleteAssets(string title, string[] assetPathNames, bool interactive)
    {
        if (assetPathNames == null)
        {
            return false;
        }
        float progress = 0f;
        int deleteCount = 0;
        if (interactive)
        {
            EditorUtility.DisplayProgressBar(title, "Deleting asset files...", progress);
        }
        for (int i = 0; i < assetPathNames.Length; i++)
        {
            string assetPath = string.Format("{0}/{1}", GetProjectPath(), assetPathNames[i]);
            if (interactive)
            {
                progress = (i + 1) / (float)assetPathNames.Length;
                EditorUtility.DisplayProgressBar(title, "Deleting asset files...", progress);
            }
            if (Directory.Exists(assetPath))
            {
                AssetDatabase.DeleteAsset(assetPathNames[i]);
                deleteCount++;
            }
            else if (File.Exists(assetPath))
            {
                AssetDatabase.DeleteAsset(assetPathNames[i]);
                deleteCount++;
                string parentPath = Path.GetDirectoryName(assetPath).Replace("\\", "/");
                string[] parentFiles = Directory.GetFiles(parentPath, "*.*", SearchOption.AllDirectories);
                if (parentFiles.Length == 0)
                {
                    AssetDatabase.DeleteAsset(GetAssetPathName(parentPath));
                    deleteCount++;
                }
            }
        }
        if (interactive)
        {
            EditorUtility.ClearProgressBar();
        }
        EditorApplication.delayCall += delegate ()
        {
            AssetDatabase.Refresh();
        };
        return (deleteCount > 0);
    }

    public static bool DeleteAssets(string title, string[] assetPathNames)
    {
        return DeleteAssets(title, assetPathNames);
    }

    public static bool DeleteAssets(string title, string listFileName, bool interactive)
    {
        return DeleteAssets(title, GetPackageList(listFileName), interactive);
    }

    public static bool DeleteAssets(string title, string listFileName)
    {
        return DeleteAssets(title, GetPackageList(listFileName), true);
    }

    public static bool Uninstall(string packageName, string[] assetPathNames, bool interactive)
    {
        if (EditorApplication.isPlaying)
        {
            Debug.LogError("Failed to uninstall package with error: Cannot uninstall package in play mode.");
            return false;
        }
        string title = string.Format("Uninstall package - {0}", packageName);
        int choice = 0;
        if (interactive)
        {
            choice = EditorUtility.DisplayDialogComplex(title, "Are you sure you want to uninstall package?\n\n\"Forced uninstall\" will remove attached components from all scenes.", "Forced uninstall", "Uninstall", "Cancel");
        }
        if (choice == 0)
        {
            return RemoveComponents(title, assetPathNames, interactive) && DeleteAssets(title, assetPathNames, interactive);
        }
        else if (choice == 1)
        {
            return DeleteAssets(title, assetPathNames, interactive);
        }
        return false;
    }

    public static bool Uninstall(string packageName, string[] assetPathNames)
    {
        return Uninstall(packageName, assetPathNames, true);
    }

    public static bool Uninstall(string packageName, string listFileName, bool interactive)
    {
        return Uninstall(packageName, GetPackageList(listFileName), interactive);
    }

    public static bool Uninstall(string packageName, string listFileName)
    {
        return Uninstall(packageName, GetPackageList(listFileName), true);
    }
}
