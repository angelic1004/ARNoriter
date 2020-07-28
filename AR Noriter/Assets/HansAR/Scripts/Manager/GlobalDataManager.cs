using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

/// <summary>
/// Json 파일 내용인 에셋번들의 버전 정보를 갖는 클래스
/// </summary>
[Serializable]
public class JsonDataVersionInfo
{
    public string       fileTitle;                // 파일의 이름 (확장자 없는)
    public string       fileExt;                  // 파일의 확장자
    public string       fileType;                 // 파일의 타입을 적으면 됨 ex) 'video' or 'audio' or 'assetbundle' 등등
    public string       parentFolder;             // 파일이 존재 할 부모 폴더명
    public float        fileSize;                 // 파일의 크기 (Mbytes 표기 ex: 12.2)
    public int          fileVersion;              // 파일의 버전

    public JsonDataVersionInfo()
    {
        fileTitle       = string.Empty;
        fileExt         = string.Empty;
        fileType        = string.Empty;
        parentFolder    = string.Empty;

        fileSize        = 0f;
        fileVersion     = 0;
    }

    ~JsonDataVersionInfo() { }
}

/***
 * author   : N.C Park
 * date     : 2017.08
 * comment  : 전역으로 사용하는 변수, 함수를 갖는 클래스
***/
public class GlobalDataManager : MonoBehaviour {    
    /// <summary>
    /// 제품 타입의 enum 값
    /// </summary>
    /*
    public enum ProductType
    {
        None,
        Common,
        Dino,
        Animal,
        SeaAnimal,
        Insect,
        Vehicle,1
        Princess,
        PrincessDance,
        ThreeLetter,
        Alphabet,
        RacingCar,
        Soccer,
        WatchCar,
        NasCar        
    }
    */

    public enum ProductType
    {
        None,
        FolderMat
    }

    public enum CategoryType
    {
        None,
        Common,
        Alphabet,
        Animal,
        Bug,
        City,
        Dino,
        SeaAnimal,
        Princess        
    }

    /// <summary>
    /// request type enum 값
    /// </summary>
    public enum RequestUrlType
    {
        Server = 0,
        Client
    }

    /// <summary>
    /// 마그피아 퍼즐에서 사용 (Bus)
    /// </summary>
    public enum PuzzleProductType
    {
        None = -1,
        Bus
    }

    /// <summary>
    /// 씬에서 사용되어지는 종류의 enum 값
    /// enum 값에 따른 UI를 활성화 하게 됨
    /// </summary>
    public enum SceneState
    {
        NONE,
        MAP,
        STICKER,
        PUZZLE,        
        DRAG_AND_DROP,
        QUIZ_QUIZ,
        CRANE
    }

    /// <summary>
    /// request 상태를 나타내는 enum 값
    /// </summary>
    public enum RequestDownloadStatus
    {
        None = 0,
        VersionDownload,
        AssetBundleDownload
    }


    /// <summary>
    /// 에디터 경로의 File URL "file:///"
    /// </summary>
    public static string m_EditorFileURL
    {
        get { return string.Format("file:///"); }
    }

    /// <summary>
    /// 단말기 경로의 File URL "file://"
    /// </summary>
    public static string m_DeviceFileURL
    {
        get { return string.Format("file://"); }
    }

    /// <summary>
    /// AssetBundle 의 루트 폴더 'HansData'
    /// </summary>
    public static string m_HansDataFolderName
    {
        get { return string.Format("HansData"); }
    }

    /// <summary>
    /// 안드로이드 단말의 루트 폴더 'Android'
    /// </summary>
    public static string m_AndroidFolderName
    {
        get { return string.Format("Android"); }
    }

    /// <summary>
    /// iOS 단말의 루트 폴더 'iOS'
    /// </summary>
    public static string m_AppleFolderName
    {
        get { return string.Format("iOS"); }
    }

    /// <summary>
    /// 확장자 ini의 문자열
    /// </summary>
    public static string m_ExtIniFile
    {
        get { return string.Format("ini"); }
    }

    /// <summary>
    /// 확장자 json의 문자열
    /// </summary>
    public static string m_ExtJsonFile
    {
        get { return string.Format("json"); }
    }

    /// <summary>
    /// 에셋번들 hans의 문자열
    /// </summary>
    public static string m_ExtAssetBundle
    {
        get { return string.Format("hans"); }
    }

    /// <summary>
    /// 임시폴더명인 temporary 문자열
    /// </summary>
    public static string m_TemporaryFolderName
    {
        get { return string.Format("temporary"); }
    }    

    public static SceneState        m_SelectedSceneStateEnum    { get; set; }    
    public static CategoryType      m_SelectedCategoryEnum      { get; set; }
    public static CategoryType      m_ResourceFolderEnum        { get; set; }

    public static string            m_SelectedSceneName         { get; set; }    
    public static string            m_SelectedAssetBundleName   { get; set; }        
    public static string            m_AssetBundlePartName       { get; set; }


    public static int               m_MainMenuDepthValue        = -1;           // MainManuUI.cs 에서 사용됨(현재 선택된 메뉴의 depth)
    public static bool              m_ConfirmCertification      = false;
    public static float             m_MainMenuScrollValue       = 0f;

    /// <summary>
    /// AssetBundle File's URL 
    /// Backup URL
    /// </summary>

    //public static string            m_AssetBundleURL            = string.Format("https://ssl.hansapp.kr/AssetBundles/v5/ARNoriter");     // 회사 내부 서버        
    public static string            m_AssetBundleURL            = string.Format("http://kor.hansapp.kr/ARNoriter/1");                      // 국내 가속서버
    
    //public static string            m_AssetBundleURL            = string.Format("http://korea4d.ip-dynamic.com:88/DATA/ARNoriter");     // 키키 서버
    public static string            m_ServerCheckURL            = string.Format("http://ssl.hansapp.kr/CheckServer.aspx?appName=");

    public static readonly float   m_AssetBundleVersion         = 1f;        // 버전값을 사용하지 않으려면 0f로 설정

    /// <summary>
    /// 메인 카테고리의 Down 카테고리 번호 저장
    /// (뒤로가기 했을때 펼쳐주기 위함)
    /// </summary>
    public static int downCategoryNum { get; set; }

    /// <summary>
    /// BGM 재생 상태
    /// </summary>
    public static bool playingBGM { get; set; }


    // template variables
    //public static ProductType m_SelectedProductEnum { get; set; }


    /// <summary>
    /// 에셋번들 다운로드 URL을 반환 합니다.    
    /// </summary>
    /// <returns>다운로드 주소</returns>
    public static string GetAssetBundleURL()
    {
        string destURL          = string.Empty;       
        string versionString    = string.Empty;

        float versionValue      = 0f;
        int lastIndexValue      = -1;

        if (string.IsNullOrEmpty(m_AssetBundleURL))
        {
            return string.Empty;
        }

        // 현재 URL 에서 버전값이 포홤 되어 있는지 찾습니다.
        destURL                 = m_AssetBundleURL;

        lastIndexValue          = destURL.LastIndexOf('/');
        versionString           = destURL.Substring(lastIndexValue + 1, destURL.Length - (lastIndexValue + 1));

        float.TryParse(versionString, out versionValue);

        // versionString 변수값이 float 형으로 캐스팅 되지 않는다면(문자열이라면)
        if (versionValue <= 0)
        {
            // m_AssetBundleVersion 값이 있다면 버전값을 URL에 추가 합니다.
            if (m_AssetBundleVersion > 0f)
            {
                destURL = string.Format("{0}/{1}", destURL, m_AssetBundleVersion);
            }
        }
        else
        {
            // DB에 있는 백업 주소에 버전값이 있는 경우 APP 내부 버전보다 클 경우 APP 내부 버전으로 URL을 변경 하는 코드인데
            // 적용 여부를 결정하지 못하여, 주석 처리를 하였음.

            /*
            // url 에 버전값이 포함되어 있고, m_AssetBUndleVersion 변수값이 0f보다 크다면
            if (m_AssetBundleVersion > 0f)
            {
                // App 내 버전과 비교하여 크다면 App 내 버전으로 변경 한 URL을 구성 합니다.
                if (versionValue > m_AssetBundleVersion)
                {
                    destURL = string.Format("{0}/{1}", destURL.Substring(0, lastIndexValue), m_AssetBundleVersion);
                }
            }
            */
        }

        //Debug.LogWarningFormat("destURL = {0}", destURL);        
        return destURL;
    }    

    void Awake()
    {
        downCategoryNum = 0;        
    }

	// Use this for initialization
	void Start () {
        m_SelectedSceneStateEnum    = SceneState.NONE;
        m_SelectedCategoryEnum       = CategoryType.None;
    }	

	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// 앱의 bundle id를 얻어 '.' 기준으로 나누다.
    /// </summary>
    /// <returns>string 배열을 리턴</returns>
    public static string[] GetBundleIdArrayValue()
    {
        string bundleId         = string.Empty;
        string[] splitData      = null;
        
        bundleId                = Application.bundleIdentifier;
        splitData               = bundleId.Split('.');

        return splitData;
    }

    /// <summary>
    /// 파라메터로 전달 된 오브젝트의 쉐이더를 다시 적용 함
    /// </summary>
    /// <param name="go">3d model</param>
    public static void ShaderRefresh(GameObject go)
    {
        Renderer[] renderers = go.GetComponentsInChildren<Renderer>(true);
        Material[] materials;
        string[] shaders;

        renderers = go.GetComponentsInChildren<Renderer>();

        foreach (var rend in renderers)
        {
            materials = rend.sharedMaterials;
            shaders = new string[materials.Length];

            for (int i = 0; i < materials.Length; i++)
            {
                shaders[i] = materials[i].shader.name;
            }

            for (int i = 0; i < materials.Length; i++)
            {
                materials[i].shader = Shader.Find(shaders[i]);
            }
        }
    }
     
    /// <summary>
    /// 스크롤의 위치를 리셋 합니다
    /// </summary>
    /// <param name="obj">스크롤 오브젝트의 부모</param>
    public static void RefreshScrollView(GameObject obj)
    {
        obj.SetActive(true);
        if (obj.GetComponent<UIScrollView>() != null)
        {
            obj.GetComponent<UIPanel>().ResetAndUpdateAnchors();
            obj.GetComponent<UIScrollView>().ResetPosition();
        }
    }    

    public static string GetPlatformFolderName()
    {
        string folderName = string.Empty;

        switch (Application.platform)
        {
            case RuntimePlatform.OSXEditor:
                folderName = m_AppleFolderName;
                break;
            case RuntimePlatform.WindowsEditor:
                folderName = m_AndroidFolderName;
                break;
            case RuntimePlatform.IPhonePlayer:
                folderName = m_AppleFolderName;
                break;
            case RuntimePlatform.Android:
                folderName = m_AndroidFolderName;
                break;
            default:
                folderName = m_AndroidFolderName;
                break;
        }

        return folderName;
    }

    public static string GetPlatformPathData()
    {
        string fileURL = string.Empty;

        switch (Application.platform)
        {
            case RuntimePlatform.OSXEditor:
                fileURL = m_EditorFileURL;
                break;
            case RuntimePlatform.WindowsEditor:
                fileURL = m_EditorFileURL;
                break;
            case RuntimePlatform.IPhonePlayer:
                fileURL = m_DeviceFileURL;
                break;
            case RuntimePlatform.Android:
                fileURL = m_DeviceFileURL;
                break;
            default:
                fileURL = m_EditorFileURL;
                break;
        }

        return fileURL;
    }

    /// <summary>
    /// request type 에 따라 경로를 구성 합니다    
    /// </summary>
    /// <param name="fileName">요청 하는 파일의 이름</param>
    /// <param name="reqType">request type</param>
    /// <returns></returns>
    public static string GetRequestFilePath(string fileName, RequestUrlType reqType)
    {
        string urlPath      = string.Empty;        
        string deviceType   = string.Empty;

        switch (reqType)
        {
            case RequestUrlType.Server:                
                urlPath = string.Format("{0}", GetAssetBundleURL());
                break;
            case RequestUrlType.Client:
                urlPath = string.Format("{0}/{1}", Application.persistentDataPath, m_HansDataFolderName);
                break;
            default:
                break;
        }

        deviceType  = GetPlatformFolderName();        
        urlPath     = string.Format("{0}/{1}/{2}", urlPath, deviceType, m_ResourceFolderEnum.ToString());

        if (!string.IsNullOrEmpty(fileName))
        {
            urlPath = string.Format("{0}/{1}", urlPath, fileName);
        }

        return urlPath;
    }

    /// <summary>
    /// request type 에 따라 경로를 구성 합니다    
    /// 제품 타입을 적용한 경로를 구성 합니다
    /// </summary>
    /// <param name="fileName">요청 하는 파일의 이름</param>
    /// <param name="reqType">request type</param>
    /// <param name="folderEnum">product type</param>
    /// <returns></returns>
    public static string GetRequestFilePath(string fileName, RequestUrlType reqType, CategoryType folderEnum)
    {
        string urlPath      = string.Empty;        
        string deviceType   = string.Empty;

        switch (reqType)
        {
            case RequestUrlType.Server:                
                urlPath = string.Format("{0}", GetAssetBundleURL());
                break;
            case RequestUrlType.Client:
                urlPath = string.Format("{0}/{1}", Application.persistentDataPath, m_HansDataFolderName);
                break;
            default:
                break;
        }

        deviceType  = GetPlatformFolderName();        
        urlPath     = string.Format("{0}/{1}/{2}", urlPath, deviceType, folderEnum.ToString());

        if (!string.IsNullOrEmpty(fileName))
        {
            urlPath = string.Format("{0}/{1}", urlPath, fileName);
        }

        return urlPath;
    }
    
    /// <summary>
    /// identifier, category, version 문자열이 합쳐진 파일 이름을 구성 합니다
    /// </summary>
    /// <returns>버전 파일 이름 문자열</returns>
    public static string GetAssetBundleVersionFileName()
    {
        string versionFileName  = string.Empty;
        string[] splitData      = null;

        splitData               = GetBundleIdArrayValue();
        versionFileName         = string.Format("{0}_{1}_version", splitData[splitData.Length - 1].ToLower(), m_ResourceFolderEnum.ToString().ToLower());

        return versionFileName;
    }

    /// <summary>
    /// 씬 로드
    /// </summary>
    public static void GlobalLoadScene()
    {
        SceneManager.LoadScene(m_SelectedSceneName);
    }  

    /*
    /// <summary>
    /// 로컬 버전 파일의 경로를 구성하여, json 데이터를 읽어와 JsonDataVersionInfo 클래스에 대입 합니다
    /// </summary>
    /// <param name="jsonData">버전 정보를 갖는 클래스</param>
    public static JsonDataVersionInfo[] ReadAssetBundleJsonData(ref JsonDataVersionInfo[] jsonData)
    {
        string jsonDestPath = string.Empty;
        string jsonFileName = string.Empty;
        string readJsonData = string.Empty;

        try
        {
            jsonDestPath    = GetRequestFilePath(null, RequestUrlType.Client);
            jsonFileName    = GetAssetBundleVersionFileName();

            jsonDestPath    = string.Format("{0}/{1}.{2}", jsonDestPath, jsonFileName, m_ExtJsonFile);

            readJsonData    = File.ReadAllText(jsonDestPath);
            jsonData        = JsonHelper.FromJson<JsonDataVersionInfo>(readJsonData);            
        }
        catch
        {
            throw;
        }
    }
    */

    /*
    /// <summary>
    /// 로컬 버전 파일에서 에셋번들의 정보를 구한다
    /// 에셋번들 정보를 갖는 클래스가 현재는 DownloadInfoParam 인데 HttpRequestDataSet 로 변경 되어야 함
    /// </summary>
    /// <param name="jsonData">버전 정보를 갖는 클래스</param>
    /// <param name="infoParam"></param>
    public static void GetAssetBundleInformation(ref JsonDataVersionInfo[] jsonData, ref HttpRequestDataSet infoParam)
    {
        string filePath = string.Empty;
        string deviceType = string.Empty;

        try
        {
            ReadAssetBundleJsonData(ref jsonData);
            GetPlatformPathData(out filePath, out deviceType);

            foreach (JsonDataVersionInfo info in jsonData)
            {
                if (info.fileTitle.Equals(m_SelectedAssetBundleName))
                {
                    infoParam.requestFileTitle = info.fileTitle;
                    infoParam.requestFileExt = info.fileExt;
                    infoParam.requestFilePath = GetRequestFilePath(null, RequestUrlType.Client);
                    infoParam.requestFileVersion = info.fileVersion;

                    infoParam.requestURL = string.Format("{0}{1}/{2}.{3}", filePath, infoParam.destinationFilePath, infoParam.requestFileTitle, infoParam.requestFileExt);
                    break;
                }
            }
        }
        catch
        {
            throw;
        }
    }
    */


    /// <summary>
    /// 제품명과 에셋번들 부분 이름값을 구성 합니다
    /// </summary>
    /// <param name="bundleName">에셋번들 이름</param>
    /// <param name="replaceValue">변경 하려는 문자열</param>
    public static void SetProductAndSceneName(string bundleName, string replaceValue)
    {
        string[] splitData  = null;
        splitData           = bundleName.Split('_');

        foreach (CategoryType type in Enum.GetValues(typeof(CategoryType)))
        {
            if (splitData[0].ToLower().Equals(type.ToString().ToLower()))
            {
                m_SelectedCategoryEnum = type;
                m_AssetBundlePartName = splitData[1].Replace(replaceValue, string.Empty);

                break;
            }
        }        
    }

    /// <summary>
    /// 미니맵 오브젝트를 MiniMapManager 멤버변수에 대입 합니다
    /// </summary>
    /// <param name="obj">미니맵 오브젝트</param>
    public static void SetMiniMapModel(GameObject obj)
    {
        MiniMapManager.instance.SetMiniMapModeling(obj);
    }

    public static void ApplyTweenAlphaValue(GameObject obj, bool viewing)
    {
        int fromValue = 0;
        int toValue = 0;

        if (viewing)
        {
            fromValue = 0;
            toValue = 1;
        }
        else
        {
            if (obj.GetComponent<TweenAlpha>().from == 1)
            {
                fromValue = 1;
            }

            toValue = 0;
        }

        if (obj.activeSelf)
        {
            obj.GetComponent<TweenAlpha>().from = fromValue;
            obj.GetComponent<TweenAlpha>().to = toValue;

            TweenManager.tween_Manager.TweenAlpha(obj);
        }
    }

    public static GameObject GetFindChildrenObject(Transform transformObj, string finder)
    {
        GameObject childrenObj      = null;
        Transform[] childrenTrans   = null;

        childrenTrans               = transformObj.GetComponentsInChildren<Transform>();

        foreach (Transform trans in childrenTrans)
        {
            if (string.Compare(trans.name, finder) == 0)
            {
                childrenObj = trans.gameObject;
                break;
            }
        }

        return childrenObj;
    }

    public static int GetResultFindVersionFile()
    {
        string checkFileName    = string.Empty;
        string checkPath        = string.Empty;
        string[] identityValues = null;

        if (m_ResourceFolderEnum == CategoryType.None)
        {
            return -1;
        }

        identityValues  = GetBundleIdArrayValue();
        checkFileName   = string.Format("{0}_{1}_version.json", identityValues[identityValues.Length - 1], m_ResourceFolderEnum.ToString().ToLower());
        checkPath       = GetRequestFilePath(checkFileName, RequestUrlType.Client, m_ResourceFolderEnum);

        //Debug.LogWarningFormat("checkFileName = {0}", checkFileName);
        //Debug.LogWarningFormat("checkPath = {0}", checkPath);

        if (File.Exists(checkPath))
        {
            return 1;
        }

        return 0;
    }

    public static string GetReplaceCloneName(string cloneName)
    {
        return cloneName.Replace(string.Format("(Clone)"), string.Empty);
    }
}