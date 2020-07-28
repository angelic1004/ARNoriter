using UnityEngine;

using System;
using System.Collections;
using System.IO;

using HansAR;

public class AssetBundleLoader : Singleton<AssetBundleLoader>
{
    // <summary>
    /// 로컬 버전 파일의 경로를 구성하여, json 데이터를 읽어와 JsonDataVersionInfo 클래스에 대입 합니다
    /// </summary>
    /// <param name="jsonData">버전 정보를 갖는 클래스</param>
    private JsonDataVersionInfo[] ReadAssetBundleJsonData()
    {
        string jsonDestPath             = string.Empty;
        string jsonFileName             = string.Empty;
        string readJsonData             = string.Empty;

        JsonDataVersionInfo[] jsonData  = null;


        jsonDestPath    = GlobalDataManager.GetRequestFilePath(null, GlobalDataManager.RequestUrlType.Client);
        jsonFileName    = GlobalDataManager.GetAssetBundleVersionFileName();

        jsonDestPath    = string.Format("{0}/{1}.{2}", jsonDestPath, jsonFileName, GlobalDataManager.m_ExtJsonFile);

        readJsonData    = File.ReadAllText(jsonDestPath);
        jsonData        = JsonHelper.FromJson<JsonDataVersionInfo>(readJsonData);

        return jsonData;
    }

    /// <summary>
    /// 로컬 버전 파일에서 에셋번들의 정보를 구한다    
    /// </summary>
    /// <param name="jsonData">버전 정보를 갖는 클래스</param>
    /// <param name="infoParam"></param>
    private void GetAssetBundleInformation(HttpRequestDataSet infoParam)
    {
        string filePath                 = string.Empty;
        JsonDataVersionInfo[] jsonData  = null;

        try
        {
            jsonData = ReadAssetBundleJsonData(); 
            filePath = GlobalDataManager.GetPlatformPathData();

            foreach (JsonDataVersionInfo info in jsonData)
            {
                //if (info.fileTitle.Equals(GlobalDataManager.m_SelectedAssetBundleName))
                if (info.fileTitle.Equals(infoParam.assetBundleName))
                {
                    infoParam.requestFileTitle      = info.fileTitle;
                    infoParam.requestFileExt        = info.fileExt;
                    infoParam.requestFilePath       = GlobalDataManager.GetRequestFilePath(null, GlobalDataManager.RequestUrlType.Client);
                    infoParam.requestFileVersion    = info.fileVersion;

                    infoParam.requestURL            = string.Format("{0}{1}/{2}.{3}", filePath, infoParam.destinationFilePath, infoParam.requestFileTitle, infoParam.requestFileExt);
                    break;
                }
            }
        }
        catch
        {
            throw;
        }
    }

    /// <summary>
    /// HttpRequestDataSet 의 기본적인 값을 할당 합니다.
    /// 에셋번들 이름, 버전파일 이름, 확장자, 저장 될 경로 등
    /// </summary>
    /// <param name="assetBundleDataSet">에셋번들 로드에 필요한 정로르 갖고 있는 클래스 개체</param>
    /// <param name="fileName">에셋번들 이름</param>
    private void CreateAssetBundleInfoData(HttpRequestDataSet assetBundleDataSet, string fileName)
    {
        assetBundleDataSet.assetBundleName          = fileName;

        assetBundleDataSet.requestFileTitle         = GlobalDataManager.GetAssetBundleVersionFileName();
        assetBundleDataSet.requestFileExt           = GlobalDataManager.m_ExtJsonFile;

        assetBundleDataSet.destinationFilePath      = GlobalDataManager.GetRequestFilePath(null, GlobalDataManager.RequestUrlType.Client);
        assetBundleDataSet.destinationFileFullPath  = GlobalDataManager.GetRequestFilePath(fileName, GlobalDataManager.RequestUrlType.Client);
    }

    /// <summary>
    /// 델리게이트 함수를 assetBundleDataSet 개체에 할당 합니다.
    /// </summary>
    /// <param name="assetBundleDataSet">에셋번들 로드에 필요한 정로르 갖고 있는 클래스 개체</param>
    /// <param name="onDrawProgressBar">에셋번들 로드 중 ProgresBar 표시를 하기위한 함수</param>
    /// <param name="onLoadComplete">에셋번들 로드를 완료 후 호출되는 함수</param>
    /// <param name="onAfterLoadCompete">onLoadComplete 함수 처리 후 호출되는 함수</param>
    /// <param name="onFinishWWW">www 개체가 Dispose() 된 후 호출되는 함수</param>
    /// <param name="onErrorWWW">www 개체로 에셋번들 로드 중 에러 발생 했을 때 호출되는 함수</param>
    /// <returns></returns>
    private int SetBundleDelegateFunction(HttpRequestDataSet assetBundleDataSet,
                                          EventHandlerDrawProgressBar onDrawProgressBar,
                                          EventHandlerRequestComplete onLoadComplete,
                                          EventHandlerAfterRequestComplete onAfterLoadCompete,
                                          EventHandlerFinishWWW onFinishWWW,
                                          EventHandlerErrorWWW onErrorWWW)
    {
        if (assetBundleDataSet == null)
        {
            return -1;
        }

        assetBundleDataSet.onDrawProgressBar        = onDrawProgressBar;
        assetBundleDataSet.onRequestComplete        = onLoadComplete;
        assetBundleDataSet.onAfterRequestComplete   = onAfterLoadCompete;
        assetBundleDataSet.onFinishWWW              = onFinishWWW;
        assetBundleDataSet.onErrorWWW               = onErrorWWW;

        return 0;
    }

    /// <summary>
    /// Template Function.
    /// </summary>
    public void DelegateEventDrawProgressBar()
    {

    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="assetBundleDataSet"></param>
    /// <param name="obj"></param>
    public void OnLoadCompleteModeling(HttpRequestDataSet assetBundleDataSet, object obj)
    {
        WWW www             = null;
        AssetBundle bundle  = null;
        GameObject prefab   = null;

        try
        {
            www = obj as WWW;
            bundle = www.assetBundle;

            for (int i = 0; i < assetBundleDataSet.assetBundleCopyObjects.Length; i++)
            {
                //Debug.Log("assetBundleDataSet.contentsModelingNames[i] : " + assetBundleDataSet.contentsModelingNames[i]);
                prefab = (Instantiate(bundle.LoadAsset(assetBundleDataSet.contentsModelingNames[i])) as GameObject);

                if (prefab != null)
                {
                    prefab.name = GlobalDataManager.GetReplaceCloneName(prefab.name);    // (Clone) 이름 삭제
                    prefab.transform.parent = assetBundleDataSet.rootObject.transform;

                    assetBundleDataSet.assetBundleCopyObjects[i] = prefab;

                    prefab.SetActive(false);
                    GlobalDataManager.ShaderRefresh(prefab);
                }
            }

            bundle.Unload(false);
            www.Dispose();

            if (assetBundleDataSet.onAfterRequestComplete != null)
            {
                assetBundleDataSet.onAfterRequestComplete(assetBundleDataSet);
            }
        }
        catch
        {
            throw;
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="assetBundleDataSet"></param>
    /// <param name="obj"></param>
    public void OnLoadCompleteOnceModeling(HttpRequestDataSet assetBundleDataSet, object obj)
    {
        WWW www             = null;
        AssetBundle bundle  = null;
        GameObject prefab   = null;

        try
        {
            www     = obj as WWW;
            bundle  = www.assetBundle;
            
            prefab = (Instantiate(bundle.LoadAsset(assetBundleDataSet.OnceModelingName)) as GameObject);

            if (prefab != null)
            {
                prefab.name                     = GlobalDataManager.GetReplaceCloneName(prefab.name);    // (Clone) 이름 삭제
                prefab.transform.parent         = assetBundleDataSet.rootObject.transform;

                assetBundleDataSet.OnceModeling = prefab;

                prefab.SetActive(false);
                GlobalDataManager.ShaderRefresh(prefab);
            }

            bundle.Unload(false);
            www.Dispose();

            if (assetBundleDataSet.onAfterRequestComplete != null)
            {
                assetBundleDataSet.onAfterRequestComplete(assetBundleDataSet);
            }
        }
        catch
        {
            throw;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="assetBundleDataSet"></param>
    /// <param name="contentModelingName"></param>
    /// <param name="rootObject"></param>
    /// <returns></returns>
    public int SetStorageLoadObject(HttpRequestDataSet assetBundleDataSet, string contentModelingName, GameObject rootObject)
    {
        if (assetBundleDataSet == null)
        {
            return -1;
        }

        assetBundleDataSet.OnceModelingName = contentModelingName;
        assetBundleDataSet.rootObject       = rootObject;

        return 0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="assetBundleDataSet"></param>
    /// <param name="contentsModelingNames"></param>
    /// <param name="contentsModeling"></param>
    /// <param name="rootObject"></param>
    /// <param name="arCameraObj"></param>
    /// <returns></returns>
    public int SetStorageLoadObject(HttpRequestDataSet assetBundleDataSet, string[] contentsModelingNames, GameObject[] contentsModeling, GameObject rootObject, GameObject arCameraObj)
    {
        if (assetBundleDataSet == null)
        {
            return -1;
        }

        assetBundleDataSet.contentsModelingNames    = contentsModelingNames;
        assetBundleDataSet.assetBundleCopyObjects   = contentsModeling;
        assetBundleDataSet.rootObject               = rootObject;
        assetBundleDataSet.arCameraObj              = arCameraObj;

        return 0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="assetBundleDataSet"></param>
    /// <param name="assetBundleName"></param>
    /// <param name="onDrawProgressBar"></param>
    /// <param name="onLoadComplete"></param>
    /// <param name="onAfterLoadComplete"></param>
    /// <param name="onFinishWWW"></param>
    /// <param name="onErrorWWW"></param>
    /// <returns></returns>
    public int SetAssetBundleLoadInfo(HttpRequestDataSet assetBundleDataSet,
                                     string assetBundleName,
                                     EventHandlerDrawProgressBar onDrawProgressBar,
                                     EventHandlerRequestComplete onLoadComplete,
                                     EventHandlerAfterRequestComplete onAfterLoadComplete,
                                     EventHandlerFinishWWW onFinishWWW,
                                     EventHandlerErrorWWW onErrorWWW)
    {
        if (assetBundleDataSet == null)
        {
            return -1;
        }

        if (string.IsNullOrEmpty(assetBundleName))
        {
            return -2;
        }

        CreateAssetBundleInfoData(assetBundleDataSet, assetBundleName);
        SetBundleDelegateFunction(assetBundleDataSet, onDrawProgressBar, onLoadComplete, onAfterLoadComplete, onFinishWWW, onErrorWWW);
        GetAssetBundleInformation(assetBundleDataSet);

        return 0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="assetBundleDataSet"></param>
    public void StartLoadAssetBundle(HttpRequestDataSet assetBundleDataSet)
    {
        StartCoroutine(WrapperWWW.FileDownloadWWW(assetBundleDataSet));
    }    
}
