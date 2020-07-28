using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DeletePlayerCaches : EditorWindow
{
    private string appDataPath;
    private string appCachePath;
    private string appPrefsPath;
    private bool _deleteAppDataPath = false;
    private bool _deletePlayerCache = false;
    private bool _deletePlayerPrefs = false;

    [MenuItem("한스앱/플레이어 캐시 삭제", false, 1)]
    public static void ShowDialog()
    {
        DeletePlayerCaches window = GetWindow<DeletePlayerCaches>(true, "플레이어 캐시 삭제");
        window.minSize = new Vector2(620, 240);
        window.maxSize = window.minSize;
        window.Init();
        window.Show();
    }

    void Init()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            appDataPath = Application.persistentDataPath.Replace('/', '\\');
            DirectoryInfo dirInfo = Directory.GetParent(appDataPath).Parent;
            appCachePath = string.Format("{0}\\Unity\\WebPlayer\\Cache\\{1}_{2}", dirInfo.FullName, Application.companyName, Application.productName);
            appPrefsPath = string.Format("HKEY_CURRENT_USER\\SOFTWARE\\{0}\\{1}", Application.companyName, Application.productName);
        }
        else
        {
            appDataPath = Application.persistentDataPath;
            DirectoryInfo dirInfo = Directory.GetParent(appDataPath).Parent;
            appCachePath = string.Format("{0}/Unity/WebPlayer/Cache/{1}_{2}", dirInfo.FullName, Application.companyName, Application.productName);
            appPrefsPath = "PlayerPrefs";
        }
    }

    void OnProjectChange()
    {
        Close();
    }

    void OnGUI()
    {
        int top = 20;

        GUI.Label(new Rect(10, top + 2, 200, 20), "삭제할 항목을 선택하세요.");
        top += 30;

        EditorGUI.BeginDisabledGroup(!Directory.Exists(appDataPath));
        _deleteAppDataPath = GUI.Toggle(new Rect(20, top + 2, 200, 20), _deleteAppDataPath, "앱 데이터 폴더 삭제");
        top += 20;

        GUI.Label(new Rect(20, top + 2, position.width - 30, 20), appDataPath);
        EditorGUI.EndDisabledGroup();
        top += 30;

        EditorGUI.BeginDisabledGroup(!Directory.Exists(appCachePath));
        _deletePlayerCache = GUI.Toggle(new Rect(20, top + 2, 200, 20), _deletePlayerCache, "플레이어 캐시 삭제");
        top += 20;

        GUI.Label(new Rect(20, top + 2, position.width - 30, 20), appCachePath);
        EditorGUI.EndDisabledGroup();
        top += 30;

        _deletePlayerPrefs = GUI.Toggle(new Rect(20, top + 2, 200, 20), _deletePlayerPrefs, "플레이어 설정값 초기화");
        top += 20;

        GUI.Label(new Rect(20, top + 2, position.width - 30, 20), appPrefsPath);
        top += 30;

        EditorGUI.BeginDisabledGroup(!(_deleteAppDataPath || _deletePlayerCache || _deletePlayerPrefs));
        if (GUI.Button(new Rect(150, top, 100, 20), "삭제"))
        {
            Close();
            DeleteSelectedPlayerCaches();
        }
        EditorGUI.EndDisabledGroup();
        if (GUI.Button(new Rect(position.width - 250, top, 100, 20), "취소"))
        {
            Close();
        }
    }

    private void DeleteSelectedPlayerCaches()
    {
        string result = string.Empty;
        if (_deleteAppDataPath)
        {
            if (Directory.Exists(appDataPath))
            {
                Directory.Delete(appDataPath, true);
            }
            result += "앱 데이터 폴더 삭제\n";
        }
        if (_deletePlayerCache)
        {
            if (Directory.Exists(appCachePath))
            {
                Directory.Delete(appCachePath, true);
            }
            result += "플레이어 캐시 삭제\n";
        }
        if (_deletePlayerPrefs)
        {
            PlayerPrefs.DeleteAll();
            result += "플레이어 설정값 초기화\n";
        }
        EditorUtility.DisplayDialog("플레이어 캐시 삭제", result + "\n선택한 항목의 삭제가 완료되었습니다.", "확인");
    }
}