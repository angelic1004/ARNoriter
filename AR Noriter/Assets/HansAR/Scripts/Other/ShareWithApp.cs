using UnityEngine;
using System.IO;
using System.Runtime.InteropServices;

public class ShareWithApp
{
#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern void iOS_ShareWithApp(string text, string path);
#endif

#if UNITY_ANDROID
    private static void Android_ShareWithApp(string title, string fileType, string filePath)
    {
        AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
        AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
        intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
        intentObject.Call<AndroidJavaObject>("setType", fileType);
        //intentObject.Call<AndroidJavaObject>("setPackage", "com.kakao.talk");// 카카오톡으로 보내기
        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), title);
        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TITLE"), title);
        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), Application.productName);
        AndroidJavaObject fileObject = new AndroidJavaObject("java.io.File", filePath);
        bool fileExist = fileObject.Call<bool>("exists");
        if (fileExist)
        {
            AndroidJavaClass strictModeClass = new AndroidJavaClass("android.os.StrictMode");
            AndroidJavaObject builderObject = new AndroidJavaObject("android.os.StrictMode$VmPolicy$Builder");
            strictModeClass.CallStatic("setVmPolicy", builderObject.Call<AndroidJavaObject>("build"));
            AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
            AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("fromFile", fileObject);
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);
            //currentActivity.Call("startActivity", intentObject);
            currentActivity.Call("startActivity", intentObject.CallStatic<AndroidJavaObject>("createChooser", new object[] { intentObject, null }));
        }
    }
#endif

    public static void ShareImage(string filePath)
    {
        string fileName = Path.GetFileName(filePath);
#if UNITY_IOS
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            iOS_ShareWithApp(fileName, filePath);
        }
#elif UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            Android_ShareWithApp(fileName, "image/*", filePath);
        }
#endif
    }

    public static void ShareVideo(string filePath)
    {
        string fileName = Path.GetFileName(filePath);
#if UNITY_IOS
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            iOS_ShareWithApp(fileName, filePath);
        }
#elif UNITY_ANDROID
        if (Application.platform == RuntimePlatform.Android)
        {
            Android_ShareWithApp(fileName, "video/*", filePath);
        }
#endif
    }
}
