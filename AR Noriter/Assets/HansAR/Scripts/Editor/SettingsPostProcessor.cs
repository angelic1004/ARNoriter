using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

#pragma warning disable 0414

public class SettingsPostProcessor
{
    // 인터넷 사용 여부
    public static bool useInternet = true;

    // 카메라 사용 여부
    public static bool useCamera = true;

    // 마이크 사용 여부
    public static bool useMicrophone = true;

    // 갤러리 (포토 라이브러리) 사용 여부
    public static bool useGallery = true;

    // 전화 사용 여부
    public static bool useTelephony = false;

    // 안드로이드 API 최소 레벨 (NatCorder 요구사항 : 18 이상)
    public static int androidMinVersion = 18;

    // 안드로이드 빌드에 사용할 API 레벨 (구글플레이 업로드 요구사항 : 26 이상)
    public static int androidTargetVersion = 26;

    // iOS 타겟 버전 (뷰포리아 7.5 요구사항 : 9.0 이상)
#if UNITY_5_5 || UNITY_5_6 || UNITY_2017 || UNITY_2018_1_OR_NEWER
    public static string iOSMinVersion = "9.0";
#else
    public static iOSTargetOSVersion iOSMinVersion = iOSTargetOSVersion.iOS_8_0;
#endif

    // 특정 플랫폼에 추가할 컴파일러 지시자
    private static Dictionary<BuildTarget, string> definesToAppend = new Dictionary<BuildTarget, string>
    {
        //{ BuildTarget.Android, "EVERYPLAY_ANDROID" },
        //{ BuildTarget.iOS, "EVERYPLAY_IPHONE" }
    };

    // 플랫폼에 관계없이 삭제할 컴파일러 지시자
    private static string[] definesToRemove =
    {
        //"EVERYPLAY_ANDROID",
        //"EVERYPLAY_IPHONE"
    };

    // 안드로이드 빌드 설정에 포함할 기능 (필수인 경우 true)
    private static Dictionary<string, bool> AndroidFeatures(bool fullFeatures)
    {
        Dictionary<string, bool> features = new Dictionary<string, bool>();
        if (fullFeatures || useInternet)
        {
            //features.Add("android.hardware.wifi", false);
        }
        if (fullFeatures || useCamera)
        {
            features.Add("android.hardware.camera", true);
            features.Add("android.hardware.camera.autofocus", false);
        }
        if (fullFeatures || useMicrophone)
        {
            features.Add("android.hardware.microphone", false);
        }
        if (fullFeatures || useGallery)
        {
            //features.Add("android.hardware.[name]", true);
        }
        if (fullFeatures || useTelephony)
        {
            features.Add("android.hardware.telephony", false);
        }
        return features;
    }

    // 안드로이드 빌드 설정에 포함할 권한 (필수인 경우 true)
    private static Dictionary<string, bool> AndroidPermissions(bool fullPermissions)
    {
        Dictionary<string, bool> permissions = new Dictionary<string, bool>();
        if (fullPermissions || useInternet)
        {
            permissions.Add("android.permission.INTERNET", true);
            //permissions.Add("android.permission.ACCESS_WIFI_STATE", false);
            permissions.Add("android.permission.ACCESS_NETWORK_STATE", true);
        }
        if (fullPermissions || useCamera)
        {
            permissions.Add("android.permission.CAMERA", true);
        }
        if (fullPermissions || useMicrophone)
        {
            permissions.Add("android.permission.RECORD_AUDIO", true);
        }
        if (fullPermissions || useGallery)
        {
            permissions.Add("android.permission.WRITE_EXTERNAL_STORAGE", true);
        }
        if (fullPermissions || useTelephony)
        {
            //permissions.Add("android.permission.READ_PHONE_STATE", false);
            permissions.Add("android.permission.CALL_PHONE", true);
        }
        return permissions;
    }

    // iOS 빌드 설정에 포함할 프레임워크 (필수인 경우 true)
    private static Dictionary<string, bool> iOSFrameworks(bool fullFrameworks)
    {
        Dictionary<string, bool> frameworks = new Dictionary<string, bool>();
        if (fullFrameworks || useInternet)
        {
            //frameworks.Add("[name].framework", true);
        }
        if (fullFrameworks || useCamera)
        {
            //frameworks.Add("[name].framework", true);
        }
        if (fullFrameworks || useMicrophone)
        {
            //frameworks.Add("[name].framework", true);
        }
        if (fullFrameworks || useGallery)
        {
            frameworks.Add("Photos.framework", false);
            frameworks.Add("MobileCoreServices.framework", false);
        }
        if (fullFrameworks || useTelephony)
        {
            //frameworks.Add("[name].framework", true);
        }
        return frameworks;
    }

    // iOS 빌드 설정에 포함할 퍼미션 (사용 설명)
    private static Dictionary<string, string> iOSPermissions(bool fullPermissions)
    {
        Dictionary<string, string> permissions = new Dictionary<string, string>();
        if (fullPermissions || useInternet)
        {
            //permissions.Add("[name]", "[description]");
        }
        if (fullPermissions || useCamera)
        {
            permissions.Add("NSCameraUsageDescription", "Required to experience augmented reality and used for snapshot photo and video recording.");
        }
        if (fullPermissions || useMicrophone)
        {
            permissions.Add("NSMicrophoneUsageDescription", "Used to record voice from microphone on video recording.");
        }
        if (fullPermissions || useGallery)
        {
            permissions.Add("NSPhotoLibraryUsageDescription", "Used to load an image or video from photo library.");
            // iOS 11+
            permissions.Add("NSPhotoLibraryAddUsageDescription", "Used to save captured image or recorded video.");
        }
        if (fullPermissions || useTelephony)
        {
            //permissions.Add("[name]", "[description]");
        }
        return permissions;
    }

#if UNITY_IOS
    #if UNITY_5_5 || UNITY_5_6 || UNITY_2017 || UNITY_2018_1_OR_NEWER
    private struct VersionNumber
    {
        public int Major, Minor, Patch;
        public VersionNumber(string version)
        {
            Major = Minor = Patch = 0;
            if (!string.IsNullOrEmpty(version))
            {
                string[] numbers = version.Split('.');
                if (numbers.Length > 0)
                {
                    int.TryParse(numbers[0],out Major);
                    if (numbers.Length > 1)
                    {
                        int.TryParse(numbers[1], out Minor);
                        if (numbers.Length > 2)
                        {
                            int.TryParse(numbers[2], out Patch);
                        }
                    }
                }
            }
        }

        public override string ToString()
        {
            if (Patch > 0)
            {
                return string.Format("{0}.{1}.{2}", Major, Minor, Patch);
            }
            return string.Format("{0}.{1}", Major, Minor);
        }
    }
    #endif
#endif

    [InitializeOnLoadMethod]
    private static void ChangeSettings()
    {
        if (definesToAppend.Count > 0 || definesToRemove.Length > 0)
        {
            BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
            BuildTargetGroup targetGroup = BuildPipeline.GetBuildTargetGroup(target);
            string backupKey = string.Format("ScriptingDefineSymbols_{0}", target);
            string previous = EditorPrefs.GetString(backupKey);
            string current = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
            if (string.Compare(previous, current) == 0)
            {
                EditorPrefs.DeleteKey(backupKey);
            }
            else
            {
                List<string> symbols = new List<string>(current.Split(';'));
                bool modified = false;
                foreach (KeyValuePair<BuildTarget, string> define in definesToAppend)
                {
                    if (define.Key == target && !symbols.Contains(define.Value))
                    {
                        symbols.Add(define.Value);
                        Debug.Log(string.Format("Settings for {0} : Adding \"{1}\" to Scripting Define Symbols.", target, define.Value));
                        modified = true;
                    }
                    else if (define.Key != target && symbols.Contains(define.Value))
                    {
                        symbols.Remove(define.Value);
                        Debug.Log(string.Format("Settings for {0} : Removing \"{1}\" from Scripting Define Symbols.", target, define.Value));
                        modified = true;
                    }
                }
                foreach (string defineValue in definesToRemove)
                {
                    if (symbols.Contains(defineValue))
                    {
                        symbols.Remove(defineValue);
                        Debug.Log(string.Format("Settings for {0} : Removing \"{1}\" from Scripting Define Symbols.", target, defineValue));
                        modified = true;
                    }
                }
                if (modified)
                {
                    EditorPrefs.SetString(backupKey, current);
                    current = string.Empty;
                    foreach (string symbol in symbols)
                    {
                        if (!string.IsNullOrEmpty(symbol))
                        {
                            if (!string.IsNullOrEmpty(current))
                            {
                                current += ";";
                            }
                            current += symbol;
                        }
                    }
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, current);
                }
            }
        }
#if UNITY_ANDROID
        if (PlayerSettings.Android.minSdkVersion < (AndroidSdkVersions)androidMinVersion)
        {
            PlayerSettings.Android.minSdkVersion = (AndroidSdkVersions)androidMinVersion;
            Debug.Log(string.Format("Settings for Android : Minimum API Level is set to {0}.", androidMinVersion));
        }
    #if UNITY_5_5 || UNITY_5_6 || UNITY_2017 || UNITY_2018_1_OR_NEWER
        if (PlayerSettings.Android.targetSdkVersion != AndroidSdkVersions.AndroidApiLevelAuto && PlayerSettings.Android.targetSdkVersion != (AndroidSdkVersions)androidTargetVersion)
        {
            PlayerSettings.Android.targetSdkVersion = (AndroidSdkVersions)androidTargetVersion;
            Debug.Log(string.Format("Settings for Android : Target API Level is set to {0}.", androidTargetVersion));
        }
    #endif
        if (useInternet)
        {
            PlayerSettings.Android.forceInternetPermission = true;
        }
        if (useGallery)
        {
            PlayerSettings.Android.forceSDCardPermission = true;
        }
        PlayerSettings.strippingLevel = StrippingLevel.Disabled;
#endif
#if UNITY_IOS
    #if UNITY_5_5 || UNITY_5_6 || UNITY_2017 || UNITY_2018_1_OR_NEWER
        VersionNumber targetVersion = new VersionNumber(PlayerSettings.iOS.targetOSVersionString);
        VersionNumber minVersion = new VersionNumber(iOSMinVersion);
        if (targetVersion.Major < minVersion.Major || (targetVersion.Major == minVersion.Major && targetVersion.Minor < minVersion.Minor))
        {
            PlayerSettings.iOS.targetOSVersionString = iOSMinVersion;
            Debug.Log(string.Format("Settings for iOS : Target OS Version is set to {0}.", iOSMinVersion));
        }
    #else
        if (PlayerSettings.iOS.targetOSVersion < iOSMinVersion)
        {
            PlayerSettings.iOS.targetOSVersion = iOSMinVersion;
            Debug.Log(string.Format("Settings for iOS : Target OS Version is set to {0}.", iOSMinVersion));
        }
    #endif
        if (useInternet)
        {
            PlayerSettings.iOS.allowHTTPDownload = true;
            PlayerSettings.iOS.requiresPersistentWiFi = true;
        }
        PlayerSettings.stripEngineCode = false;
#endif
    }

#if UNITY_ANDROID
    [InitializeOnLoadMethod]
    private static void ChangeManfiest()
    {
        string manifestPath = string.Format("{0}/Plugins/Android/AndroidManifest.xml", Application.dataPath.Replace("\\", "/"));
        Dictionary<string, bool> fullFeatures = AndroidFeatures(true);
        Dictionary<string, bool> usesFeatures = AndroidFeatures(false);
        Dictionary<string, bool> fullPermissions = AndroidPermissions(true);
        Dictionary<string, bool> usesPermissions = AndroidPermissions(false);
        XmlDocument doc = new XmlDocument();
        try
        {
            bool modified = false;
            if (File.Exists(manifestPath))
            {
                doc.Load(manifestPath);
            }
            else
            {
                #region Create AndroidManifest.xml
                doc.InnerXml =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<manifest xmlns:android=""http://schemas.android.com/apk/res/android"" package=""com.unity3d.player"" android:installLocation=""preferExternal"" android:theme=""@android:style/Theme.NoTitleBar"" android:versionCode=""1"" android:versionName=""1.0"">
  <uses-sdk android:minSdkVersion=""18"" android:targetSdkVersion=""26"" />
  <supports-screens android:smallScreens=""true"" android:normalScreens=""true"" android:largeScreens=""true"" android:xlargeScreens=""true"" android:anyDensity=""true"" />
  <application android:icon=""@drawable/app_icon"" android:label=""@string/app_name"" android:theme=""@android:style/Theme.NoTitleBar.Fullscreen"" android:debuggable=""false"">
    <activity android:name=""com.unity3d.player.UnityPlayerNativeActivity"" android:label=""@string/app_name"">
      <intent-filter>
        <action android:name=""android.intent.action.MAIN"" />
        <category android:name=""android.intent.category.LAUNCHER"" />
      </intent-filter>
      <meta-data android:name=""unityplayer.UnityActivity"" android:value=""true"" />
      <meta-data android:name=""unityplayer.ForwardNativeEventsToDalvik"" android:value=""false"" />
    </activity>
    <activity android:name=""com.unity3d.player.VideoPlayer"" android:label=""@string/app_name"" android:screenOrientation=""portrait"" android:configChanges=""fontScale|keyboard|keyboardHidden|locale|mnc|mcc|navigation|orientation|screenLayout|screenSize|smallestScreenSize|uiMode|touchscreen"">
    </activity>
    <!--
      To support devices using the TI S3D library for stereo mode we must 
      add the following library.
      Devices that require this are: ODG X6 
    -->
    <uses-library android:name=""com.ti.s3d"" android:required=""false"" />
    <!--
      To support the ODG R7 in stereo mode we must add the following library.
    -->
    <uses-library android:name=""com.osterhoutgroup.api.ext"" android:required=""false"" />
  </application>
</manifest>";
                #endregion

                Debug.Log("Settings for Android : Creating AndroidManifest.xml.");
                modified = true;
            }
            XmlNodeList manifestNodes = doc.GetElementsByTagName("manifest");
            XmlElement manifestNode;
            string namespaceURI;
            if (manifestNodes.Count > 0)
            {
                manifestNode = manifestNodes[0] as XmlElement;
                namespaceURI = manifestNode.GetNamespaceOfPrefix("android");
            }
            else
            {
                namespaceURI = "http://schemas.android.com/apk/res/android";
                manifestNode = doc.CreateElement("manifest");
                manifestNode.SetAttribute("xmlns:android", namespaceURI);
                manifestNode.SetAttribute("package", "com.unity3d.player");
                manifestNode.SetAttribute("android:installLocation", namespaceURI, "preferExternal");
                manifestNode.SetAttribute("android:theme", namespaceURI, "@android:style/Theme.NoTitleBar");
                manifestNode.SetAttribute("android:versionCode", namespaceURI, "1");
                manifestNode.SetAttribute("android:versionName", namespaceURI, "1.0");
                doc.AppendChild(manifestNode);
                Debug.Log("Settings for Android : Creating manifest to AndroidManifest.xml.");
                modified = true;
            }
            XmlElement usesSdkNode = GetElementByTagName(manifestNode, "uses-sdk");
            if (usesSdkNode == null)
            {
                usesSdkNode = doc.CreateElement("uses-sdk");
                usesSdkNode.SetAttribute("android:minSdkVersion", namespaceURI, string.Format("{0}", (int)androidMinVersion));
                if (androidTargetVersion > 0)
                {
                    usesSdkNode.SetAttribute("android:targetSdkVersion", namespaceURI, string.Format("{0}", (int)androidTargetVersion));
                }
                manifestNode.PrependChild(usesSdkNode);
                Debug.Log("Settings for Android : Adding SDK version to AndroidManifest.xml.");
                modified = true;
            }
            else
            {
                int minVersion, targetVersion;
                string minSdkVersionValue = usesSdkNode.GetAttribute("android:minSdkVersion");
                string targetSdkVersionValue = usesSdkNode.GetAttribute("android:targetSdkVersion");
                if (int.TryParse(minSdkVersionValue, out minVersion))
                {
                    if (minVersion < (int)androidMinVersion)
                    {
                        usesSdkNode.SetAttribute("android:minSdkVersion", namespaceURI, string.Format("{0}", (int)androidMinVersion));
                        Debug.Log(string.Format("Settings for Android : Minimum API Level is set to {0} in AndroidManifest.xml.", (int)androidMinVersion));
                        modified = true;
                    }
                    if (int.TryParse(targetSdkVersionValue, out targetVersion) && targetVersion != (int)androidTargetVersion)
                    {
                        if (androidTargetVersion == 0)
                        {
                            usesSdkNode.RemoveAttribute("android:targetSdkVersion", namespaceURI);
                        }
                        else
                        {
                            usesSdkNode.SetAttribute("android:targetSdkVersion", namespaceURI, string.Format("{0}", (int)androidTargetVersion));
                        }
                        Debug.Log(string.Format("Settings for Android : Target API Level is set to {0} in AndroidManifest.xml.", (int)androidTargetVersion));
                        modified = true;
                    }
                }
            }
            foreach (KeyValuePair<string, bool> feature in fullFeatures)
            {
                XmlElement usesFeatureNode = GetElementByAttribute(manifestNode, "uses-feature", "android:name", feature.Key);
                if (usesFeatures.ContainsKey(feature.Key))
                {
                    if (usesFeatureNode == null)
                    {
                        XmlElement appendNode = doc.CreateElement("uses-feature");
                        appendNode.SetAttribute("android:name", namespaceURI, feature.Key);
                        if (!feature.Value)
                        {
                            appendNode.SetAttribute("android:required", namespaceURI, "false");
                        }
                        XmlElement lastNode = GetElementByTagName(manifestNode, "uses-feature");
                        if (lastNode == null)
                        {
                            lastNode = usesSdkNode;
                        }
                        manifestNode.InsertAfter(appendNode, lastNode);
                        if (feature.Value)
                        {
                            Debug.Log(string.Format("Settings for Android : Adding feature \"{0}\" to AndroidManifest.xml.", feature.Key));
                        }
                        else
                        {
                            Debug.Log(string.Format("Settings for Android : Adding feature \"{0}\" as reference to AndroidManifest.xml.", feature.Key));
                        }
                        modified = true;
                    }
                    else
                    {
                        string requiredValue = usesFeatureNode.GetAttribute("android:required");
                        bool required = (string.IsNullOrEmpty(requiredValue) || string.Compare(requiredValue, "true", true) == 0);
                        if (required != feature.Value)
                        {
                            if (feature.Value)
                            {
                                usesFeatureNode.RemoveAttribute("android:required");
                                Debug.Log(string.Format("Settings for Android : Feature \"{0}\" is set to required in AndroidManifest.xml.", feature.Key));
                            }
                            else
                            {
                                usesFeatureNode.SetAttribute("android:required", "false");
                                Debug.Log(string.Format("Settings for Android : Feature \"{0}\" is set as reference in AndroidManifest.xml.", feature.Key));
                            }
                            modified = true;
                        }
                    }
                }
                else
                {
                    if (usesFeatureNode != null)
                    {
                        manifestNode.RemoveChild(usesFeatureNode);
                        Debug.Log(string.Format("Settings for Android : Removing feature \"{0}\" from AndroidManifest.xml.", feature.Key));
                        modified = true;
                    }
                }
            }
            foreach (KeyValuePair<string, bool> permission in fullPermissions)
            {
                XmlElement usesPermissionNode = GetElementByAttribute(manifestNode, "uses-permission", "android:name", permission.Key);
                if (usesPermissions.ContainsKey(permission.Key))
                {
                    if (usesPermissionNode == null)
                    {
                        XmlElement appendNode = doc.CreateElement("uses-permission");
                        appendNode.SetAttribute("android:name", namespaceURI, permission.Key);
                        if (!permission.Value)
                        {
                            appendNode.SetAttribute("android:required", namespaceURI, "false");
                        }
                        XmlElement lastNode = GetElementByTagName(manifestNode, "uses-permission");
                        if (lastNode == null)
                        {
                            lastNode = GetElementByTagName(manifestNode, "uses-feature");
                            if (lastNode == null)
                            {
                                lastNode = usesSdkNode;
                            }
                        }
                        manifestNode.InsertAfter(appendNode, lastNode);
                        if (permission.Value)
                        {
                            Debug.Log(string.Format("Settings for Android : Adding permission \"{0}\" to AndroidManifest.xml.", permission.Key));
                        }
                        else
                        {
                            Debug.Log(string.Format("Settings for Android : Adding permission \"{0}\" as reference to AndroidManifest.xml.", permission.Key));
                        }
                        modified = true;
                    }
                    else
                    {
                        string requiredValue = usesPermissionNode.GetAttribute("android:required");
                        bool required = (string.IsNullOrEmpty(requiredValue) || string.Compare(requiredValue, "true", true) == 0);
                        if (required != permission.Value)
                        {
                            if (permission.Value)
                            {
                                usesPermissionNode.RemoveAttribute("android:required");
                                Debug.Log(string.Format("Settings for Android : Permission \"{0}\" is set to required in AndroidManifest.xml.", permission.Key));
                            }
                            else
                            {
                                usesPermissionNode.SetAttribute("android:required", "false");
                                Debug.Log(string.Format("Settings for Android : Permission \"{0}\" is set as reference in AndroidManifest.xml.", permission.Key));
                            }
                            modified = true;
                        }
                    }
                }
                else
                {
                    if (usesPermissionNode != null)
                    {
                        manifestNode.RemoveChild(usesPermissionNode);
                        Debug.Log(string.Format("Settings for Android : Removing permission \"{0}\" from AndroidManifest.xml.", permission.Key));
                        modified = true;
                    }
                }
            }
#if UNITY_5_5 || UNITY_5_6 || UNITY_2017 || UNITY_2018_1_OR_NEWER
#else
            // 유니티에서 제공하는 권한 요청이 안드로이드 7.0 이상에서 동작하지 않으므로 권한 요청 기능을 직접 구현해야 함
            if (androidTargetVersion >= 24)
            {
                XmlElement applicationNode = GetElementByTagName(manifestNode, "application");
                if (applicationNode != null)
                {
                    XmlElement activityNode = GetElementByAttribute(applicationNode, "activity", "android:name", "com.unity3d.player.UnityPlayerNativeActivity");
                    if (activityNode != null)
                    {
                        XmlElement metaDataNode = GetElementByAttribute(activityNode, "meta-data", "android:name", "unityplayer.SkipPermissionsDialog");
                        if (metaDataNode == null)
                        {
                            XmlElement appendNode = doc.CreateElement("meta-data");
                            appendNode.SetAttribute("android:name", namespaceURI, "unityplayer.SkipPermissionsDialog");
                            appendNode.SetAttribute("android:value", namespaceURI, "true");
                            activityNode.AppendChild(appendNode);
                            Debug.Log("Settings for Android : Adding meta data for skip permissions dialog.");
                            Debug.LogWarning("For publish to Google Play, you must add a script that requests permissions.");//
                            modified = true;
                        }
                    }
                }
            }
#endif
            if (modified)
            {
                string manifestDir = Path.GetDirectoryName(manifestPath);
                if (!Directory.Exists(manifestDir))
                {
                    Directory.CreateDirectory(manifestDir);
                }
                doc.Save(manifestPath);
                EditorApplication.delayCall += delegate ()
                {
                    AssetDatabase.Refresh();
                };
            }
        }
        catch (XmlException ex)
        {
            Debug.LogError(ex);
        }
    }

    private static XmlElement GetElementByTagName(XmlNode node, string tagName)
    {
        if (node != null && node.HasChildNodes)
        {
            XmlElement lastElement = null;
            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (childNode.NodeType == XmlNodeType.Element)
                {
                    XmlElement element = (XmlElement)childNode;
                    if (string.Compare(childNode.Name, tagName) == 0)
                    {
                        lastElement = element;
                    }
                }
            }
            return lastElement;
        }
        return null;
    }

    private static XmlElement GetElementByAttribute(XmlNode node, string tagName, string attributeName, string attributeValue)
    {
        if (node != null && node.HasChildNodes)
        {
            XmlElement lastElement = null;
            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (childNode.NodeType == XmlNodeType.Element)
                {
                    XmlElement element = (XmlElement)childNode;
                    if (string.Compare(element.Name, tagName) == 0)
                    {
                        if (string.Compare(element.GetAttribute(attributeName), attributeValue) == 0)
                        {
                            lastElement = element;
                        }
                    }
                }
            }
            return lastElement;
        }
        return null;
    }
#endif

#if UNITY_IOS
    [PostProcessBuild]
    private static void LinkFrameworks(BuildTarget buildTarget, string path)
    {
        if (buildTarget != BuildTarget.iOS) return;
        string projPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";
        bool modified = false;
        PBXProject proj = new PBXProject();
        proj.ReadFromString(File.ReadAllText(projPath));
        string target = proj.TargetGuidByName("Unity-iPhone");
        foreach (KeyValuePair<string, bool> framework in iOSFrameworks(false))
        {
    #if UNITY_5
            if (!proj.HasFramework(framework.Key))
    #else
            if (!proj.ContainsFramework(target, framework.Key))
    #endif
            {
                proj.AddFrameworkToProject(target, framework.Key, !framework.Value);
                if (framework.Value)
                {
                    Debug.Log(string.Format("Settings for iOS : Adding framework \"{0}\" to Xcode project.", framework.Key));
                }
                else
                {
                    Debug.Log(string.Format("Settings for iOS : Adding framework \"{0}\" as reference to Xcode project.", framework.Key));
                }
                modified = true;
            }
        }
        if (modified)
        {
            File.WriteAllText(projPath, proj.WriteToString());
        }
    }

    [PostProcessBuild]
    private static void SetPermissions(BuildTarget buildTarget, string path)
    {
        if (buildTarget != BuildTarget.iOS) return;
        string plistPath = path + "/Info.plist";
        bool modified = false;
        PlistDocument plist = new PlistDocument();
        plist.ReadFromString(File.ReadAllText(plistPath));
        PlistElementDict rootDict = plist.root;
        foreach (KeyValuePair<string, string> permission in iOSPermissions(false))
        {
            if (!rootDict.values.ContainsKey(permission.Key))
            {
                rootDict.SetString(permission.Key, permission.Value);
                Debug.Log(string.Format("Settings for iOS : Adding usage description \"{0}\" to info.plist.", permission.Key));
                modified = true;
            }
        }
        if (modified)
        {
            File.WriteAllText(plistPath, plist.WriteToString());
        }
    }
#endif
}

#pragma warning restore 0414
