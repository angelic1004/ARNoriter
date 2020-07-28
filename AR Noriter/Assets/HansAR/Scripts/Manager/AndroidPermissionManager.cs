using UnityEngine;
using System;
using System.Collections.Generic;

public class AndroidPermissionManager : MonoBehaviour
{
    public bool RequireCamera = true;
    public bool RequireReadPhoneState = true;
    public bool RequireCallPhone = false;
    public bool RequireExternalStorage = true;
    public bool RequireMicrophone = true;

    private const string ANDROID_PERMISSION_CLASS_NAME = "com.hansapp.androidpermission.AndroidPermission";
    private static readonly string PERMISSION_CAMERA = "android.permission.CAMERA";
    private static readonly string PERMISSION_READ_PHONE_STATE = "android.permission.READ_PHONE_STATE";
    private static readonly string PERMISSION_CALL_PHONE = "android.permission.CALL_PHONE";
    private static readonly string PERMISSION_WRITE_EXTERNAL_STORAGE = "android.permission.WRITE_EXTERNAL_STORAGE";
    private static readonly string PERMISSION_RECORD_AUDIO = "android.permission.RECORD_AUDIO";

    void Awake()
    {
        if (IsRuntimePermissionDevice())
        {
            List<string> permissions = new List<string>();
            if (RequireCamera && !CheckSelfPermission(PERMISSION_CAMERA))
            {
                permissions.Add(PERMISSION_CAMERA);
            }
            if (RequireReadPhoneState && !CheckSelfPermission(PERMISSION_READ_PHONE_STATE))
            {
                permissions.Add(PERMISSION_READ_PHONE_STATE);
            }
            if (RequireCallPhone && !CheckSelfPermission(PERMISSION_CALL_PHONE))
            {
                permissions.Add(PERMISSION_CALL_PHONE);
            }
            if (RequireExternalStorage && !CheckSelfPermission(PERMISSION_WRITE_EXTERNAL_STORAGE))
            {
                permissions.Add(PERMISSION_WRITE_EXTERNAL_STORAGE);
            }
            if (RequireMicrophone && !CheckSelfPermission(PERMISSION_RECORD_AUDIO))
            {
                permissions.Add(PERMISSION_RECORD_AUDIO);
            }
            if (permissions.Count > 0)
            {
                RequestPermissions(permissions);
                permissions.Clear();
            }
        }
    }

    void Start()
    {
        if (IsRuntimePermissionDevice())
        {
            if (RequireCamera && ShouldShowRequestPermissionRationale(PERMISSION_CAMERA))
            {
                Debug.Log("Camera permission is not granted.");
                //Application.Quit();
            }
        }
    }

    public static bool IsRuntimePermissionDevice()
    {
        bool answer = false;
#if UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            using (AndroidJavaClass plugin = new AndroidJavaClass(ANDROID_PERMISSION_CLASS_NAME))
            {
                answer = plugin.CallStatic<bool>("isRuntimePermissionDevice");
            }
        }
#endif
        return answer;
    }

    public static void RequestPermissions(IEnumerable<string> _permissions)
    {
        if (!IsRuntimePermissionDevice())
            return;

        string permissionValue = string.Empty;
        foreach (string permission in _permissions)
        {
            if (!string.IsNullOrEmpty(permission))
            {
                permissionValue += ",";
            }
            permissionValue += permission;
        }

        using (AndroidJavaClass plugin = new AndroidJavaClass(ANDROID_PERMISSION_CLASS_NAME))
        {
            plugin.CallStatic("requestPermissions", permissionValue);
        }
    }

    public static bool CheckSelfPermission(string permission)
    {
        bool answer = true;
#if UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            using (AndroidJavaClass plugin = new AndroidJavaClass(ANDROID_PERMISSION_CLASS_NAME))
            {
                var valueFromAndroidMethod = plugin.CallStatic<int>("checkSelfPermission", permission);
                if (valueFromAndroidMethod != 0)
                {
                    answer = false;
                }
            }
        }
#endif
        return answer;
    }

    public static bool ShouldShowRequestPermissionRationale(string permission)
    {
        bool answer = false;
#if UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            using (AndroidJavaClass plugin = new AndroidJavaClass(ANDROID_PERMISSION_CLASS_NAME))
            {
                answer = plugin.CallStatic<bool>("shouldShowRequestPermissionRationale", permission);
            }
        }
#endif
        return answer;
    }
}
