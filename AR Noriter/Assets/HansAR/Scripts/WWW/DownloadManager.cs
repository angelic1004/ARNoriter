using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using System.Reflection;

using HansAR;

/***
 * @author  : N.C Park
 * @date    : 2017.08.31 최초
 *            2017.09.13 수정
 * @comment : WWW클래스 환경에서 에셋번들 버전정보를 갖고 있는 json 데이터를 다운로드 하거나, 에셋번들을 다운로드 하는 클래스 입니다.
 * 
***/

public class DownloadManager : Singleton<DownloadManager> {
    private List<HttpRequestDataSet>            versionDataSetList;                         // 버전 파일에 관련된 RequestDataSet 개체를 갖는 List 변수
    private List<HttpRequestDataSet>            assetDataSetList;                           // 에셋번들 파일에 관련된 RequestDataSet 개체를 갖는 List 변수
    private List<GlobalDataManager.CategoryType> downloadedProductList;                      // 다운로드가 필요한 제품의 enum 갖는 List 변수

    private Queue<HttpRequestDataSet>           reqDataQueue;                               // 다운로드를 위한 큐
    private Coroutine                           queueCoroutine;                             // 무한 반복 상태에서 큐에 데이터가 들어오면 다운로드 코루틴을 호출 함

    private bool                                retryDownload;                              // 다운로드 요청에서 에러가 발생하여 재시도가 필요한지에 대한 값을 갖는 변수
    private string                              retryServerURI;                             // 서버에서 받아오는 값이므로 직접 입력하지 말것!

    public float                                downloadAssetTotalSize  { get; set; }       // 다운로드 해야하는 총 파일 크기를 갖는 변수     

    public HMSlideDropUIManager                 slideDropUIManager;
    public CircleMenuManager                    circleMenuManager;
    public HouseMenuManager                     houseMenuManager;
    public int                                  concurrentDownloadCount;                    // 동시에 다운로그 가능한 파일 개수
    public GlobalDataManager.CategoryType[]     addCategoryType;                             // 제품 외 추가로 다운로드가 필요한 제품 타입을 갖음

    protected static float                      receiveFileSize         = 0f;               // 하나의 파일에 대해 다운로드 한 크기를 갖는 변수
    protected static float                      downloadCompleteSize    = 0f;               // 현재까지 다운로드 완료된 파일 크기를 갖는 변수

    void Awake()
    {
        versionDataSetList      = new List<HttpRequestDataSet>();
        assetDataSetList        = new List<HttpRequestDataSet>();
        
        reqDataQueue            = new Queue<HttpRequestDataSet>();
        downloadedProductList   = new List<GlobalDataManager.CategoryType>();
    }

	// Use this for initialization
	void Start ()
    {
        retryDownload           = false;
        retryServerURI          = null;
    }
	
	// Update is called once per frame
	void Update ()
    {

    }
    
    void OnDisable()
    {
        StopDownloadCoroutine();
    }

    void UnsubscribeEvent()
    {
        OnDisable();
    }

    /// <summary>
    /// 무한루프 상태에서 Queue에 데이터(HttpRequestDataSet)가 들어오면 다운로드를 요청함
    /// </summary>
    /// <returns></returns>
    private IEnumerator HttpDownloaderLoop()
    {
        HttpRequestDataSet dataSet = null;

        while (true)
        {
            if (reqDataQueue.Count > 0)
            {
                if (WrapperWWW.count < concurrentDownloadCount)
                {
                    dataSet             = reqDataQueue.Dequeue();
                    dataSet.coroutine   = StartCoroutine(WrapperWWW.FileDownloadWWW(dataSet));
                }
            }

            yield return null;
        }
    }
    
    /// <summary>
    /// 다운로드 관련 리스트 변수, 파일 사이즈 변수 초기화
    /// </summary>
    private void ClearRequestDataSetList()
    {
        versionDataSetList.Clear();
        assetDataSetList.Clear();

        receiveFileSize         = 0f;
        downloadCompleteSize    = 0f;
        downloadAssetTotalSize  = 0f;
    }

    /// <summary>
    /// 다운로드 관련 제품 목록을 리스트로 만듭니다.
    /// </summary>
    private void ConfigureDownloadProductList()
    {
        downloadedProductList.Clear();

        foreach (GlobalDataManager.CategoryType addType in addCategoryType)
        {
            downloadedProductList.Add(addType);
        }

        downloadedProductList.Add(GlobalDataManager.m_SelectedCategoryEnum);
    }

    /// <summary>
    /// 인자로 전달 된 제품의 version json 파일이 있는지 확인
    /// </summary>
    /// <param name="productType">제품 enum 값</param>
    /// <returns>true or false</returns>
    public bool CheckLocalDownFolderExist(GlobalDataManager.CategoryType categoryType)
    {
        string localVersionFileName     = string.Empty;
        string localVersionFilePath     = string.Empty;
        string[] identifierSplitData    = null;

        bool resultValue                = false;

        try
        {
            identifierSplitData         = GlobalDataManager.GetBundleIdArrayValue();        
            
            localVersionFileName        = string.Format("{0}_{1}_version.{2}", identifierSplitData[2], categoryType.ToString().ToLower(), GlobalDataManager.m_ExtJsonFile);
            localVersionFilePath        = GlobalDataManager.GetRequestFilePath(localVersionFileName, GlobalDataManager.RequestUrlType.Client, categoryType);

            resultValue                 = File.Exists(localVersionFilePath);
        }
        catch (Exception ex)
        {
            Debug.LogErrorFormat("Error Message : {0}", ex.Message);
        }
        
        return resultValue;
    }

    

    /// <summary>
    /// List 안에 들어 있는 리퀘스트를 다운로드 큐에 추가합니다.
    /// </summary>
    /// <param name="requestList">HttpRequestDataSet 개체를 갖는 List</param>
    private void PushDownloadRequest(List<HttpRequestDataSet> requestList)
    {
        foreach (HttpRequestDataSet dataSet in requestList)
        {
            reqDataQueue.Enqueue(dataSet);
        }
    }


    /// <summary>
    /// HttpRequestDataSet 개체를 생성하며, 기본적으로 사용하는 값을 세팅.
    /// </summary>
    /// <param name="fileName">다운로드 할 파일명 (확장자 포함)</param>
    /// <returns>HttpRequestDataSet 개체</returns>
    private HttpRequestDataSet CreateRequestData(string fileName)
    {
        HttpRequestDataSet requestDataSet       = null;
        requestDataSet                          = new HttpRequestDataSet();

        requestDataSet.requestURL               = GlobalDataManager.GetRequestFilePath(fileName, GlobalDataManager.RequestUrlType.Server);        

        requestDataSet.requestFileTitle         = GlobalDataManager.GetAssetBundleVersionFileName();
        requestDataSet.requestFileExt           = GlobalDataManager.m_ExtJsonFile;
        
        requestDataSet.destinationFilePath      = GlobalDataManager.GetRequestFilePath(null, GlobalDataManager.RequestUrlType.Client);
        requestDataSet.destinationFileFullPath  = GlobalDataManager.GetRequestFilePath(fileName, GlobalDataManager.RequestUrlType.Client);

        return requestDataSet;
    }

    /// <summary>
    /// 버전 파일에 대한 리퀘스트를 구성 합니다.
    /// </summary>
    private void RequestVersionJsonFile()
    {
        string requestFileName                      = string.Empty;
        HttpRequestDataSet requestDataSet           = null;

        ClearRequestDataSetList();        

        // 반복문을 적용하는 이유는 Common 또는 사용자가 추가한 폴더가 있을 때 함께 처리하기 위함
        foreach (GlobalDataManager.CategoryType type in downloadedProductList)
        {
            GlobalDataManager.m_ResourceFolderEnum  = type;
            
            requestFileName                         = string.Format("{0}.{1}", GlobalDataManager.GetAssetBundleVersionFileName(), GlobalDataManager.m_ExtJsonFile);
            requestDataSet                          = CreateRequestData(requestFileName);

            requestDataSet.categoryType             = type;
            requestDataSet.requestDownloadStatus    = GlobalDataManager.RequestDownloadStatus.VersionDownload;
                        
            requestDataSet.onRequestComplete        = OnCompleteDownloadVersionFile;
            requestDataSet.onErrorWWW               = OnErrorDownloadVersionFile;
                        
            versionDataSetList.Add(requestDataSet);
        }

        PushDownloadRequest(versionDataSetList);
    }

    /// <summary>
    /// 버전 파일에 대한 리퀘스트를 구성하며, 서버 주소는 백업주소로 변경 합니다.
    /// </summary>
    private void RequestRetryConfirmUri()
    {
        HttpRequestDataSet requestDataSet           = null;
        requestDataSet                              = CreateRequestData(null);

        ClearRequestDataSetList();

        // 반복문을 적용하는 이유는 Common 또는 사용자가 추가한 폴더가 있을 때 함께 처리하기 위함
        foreach (GlobalDataManager.CategoryType type in downloadedProductList)
        {
            GlobalDataManager.m_ResourceFolderEnum  = type;

            requestDataSet.categoryType             = type;
            requestDataSet.requestDownloadStatus    = GlobalDataManager.RequestDownloadStatus.VersionDownload;

            requestDataSet.requestURL               = string.Format("{0}{1}", GlobalDataManager.m_ServerCheckURL, Application.bundleIdentifier);
            requestDataSet.onRequestComplete        = OnConfirmServerURI;
            requestDataSet.onErrorWWW               = OnErrorDownloadVersionFile;
                        
            versionDataSetList.Add(requestDataSet);
        }

        PushDownloadRequest(versionDataSetList);
    }

    /// <summary>
    /// 에셋번들의 다운로드 리퀘스트를 구성 합니다.
    /// </summary>
    /// <param name="fileName">다운로드 요청 파일명</param>
    /// <param name="fileExt">다운로드 요청 파일 확장자</param>
    /// <param name="fileSize">다운로드 요청 파일 크기</param>
    private void RequestAssetBundleFile(string fileName, string fileExt, float fileSize)
    {
        string requestFileName                  = string.Empty;
        HttpRequestDataSet requestDataSet       = null;

        requestFileName                         = string.Format("{0}.{1}", fileName, fileExt);
        requestDataSet                          = CreateRequestData(requestFileName);

        requestDataSet.categoryType             = GlobalDataManager.m_ResourceFolderEnum;
        requestDataSet.requestDownloadStatus    = GlobalDataManager.RequestDownloadStatus.AssetBundleDownload;

        requestDataSet.requestFileTitle         = fileName;
        requestDataSet.requestFileExt           = fileExt;

        requestDataSet.requestFileSize          = fileSize;        

        requestDataSet.onDrawProgressBar        = OnDrawProgressValue;
        requestDataSet.onRequestComplete        = OnCompleteDownloadAssetBundleFile;
        requestDataSet.onFinishWWW              = OnFinishDownloadAssetBundleFile;
        requestDataSet.onErrorWWW               = OnErrorDownloadAssetBundleFile;

        assetDataSetList.Add(requestDataSet);
    }

    /// <summary>
    /// 임시폴더에 있는 Resouce 정보 파일을 메인 폴더로 복사 후 임시 파일은 삭제
    /// </summary>
    private void FileCopyToDestiny()
    {
        string sourcePath   = string.Empty;
        string sourceFile   = string.Empty;
        string destFile     = string.Empty;

        foreach (HttpRequestDataSet data in versionDataSetList)
        {
            sourcePath      = string.Format("{0}/{1}", data.destinationFilePath, GlobalDataManager.m_TemporaryFolderName);
            sourceFile      = string.Format("{0}/{1}.{2}", sourcePath, data.requestFileTitle, data.requestFileExt);
            destFile        = string.Format("{0}/{1}.{2}", data.destinationFilePath, data.requestFileTitle, data.requestFileExt);

            if (!File.Exists(sourceFile))
            {
                return;
            }

            if (File.Exists(destFile))
            {
                File.Delete(destFile);
            }

            File.Move(sourceFile, destFile);

            if (Directory.Exists(sourcePath))
            {
                Directory.Delete(sourcePath);
            }
        }
    }

    /// <summary>
    /// 다운로드 완료 후 메뉴 슬라이드로 이동
    /// </summary>
    private void MoveSlideDropCategory()
    {
        StopDownloadCoroutine();
       
        if(houseMenuManager != null)
        {
            houseMenuManager.SubMenuOpenCoroutineStart();
        }
        else if (circleMenuManager != null)
        {
            circleMenuManager.SubMenuOpenCoroutineStart();
        }
        else
        {
            slideDropUIManager.SetCategoryOnOff();
            slideDropUIManager.SlideCategory();
        }
    }

    /// <summary>
    /// 다운로드 한 JSON 데이터를 임시폴더에 파일로 쓰고, 에셋번들 다운로드 리스트를 구성한다.
    /// </summary>
    /// <param name="requestDataSet">다운로드 요청 정보 개체</param>
    /// <param name="www">WWW object</param>
    private void OnCompleteDownloadVersionFile(HttpRequestDataSet requestDataSet, WWW www)
    {
        try
        {
            GlobalDataManager.m_ResourceFolderEnum = requestDataSet.categoryType;

            if (www != null)
            {
                WriteVersionFile(requestDataSet, www.bytes);
                GetDownloadSizeFromJson(www.bytes);

                // 에셋번들 다운로드 리스트 구성 후 큐에 넣음
                CreateDownloadList(www.bytes);
            }

            if (reqDataQueue.Count == 0 && requestDataSet.requestDownloadStatus == GlobalDataManager.RequestDownloadStatus.VersionDownload)
            {
                if (assetDataSetList.Count > 0)
                {
                    // 3G, LTE 사용하는지                
                    if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
                    {
                        if (circleMenuManager != null)
                        {
                            CircleMenuUI.getInstance.m_DataPopupText.text = string.Format(LocalizeText.Value["MobileDatacall_F"], downloadAssetTotalSize.ToString());
                            CircleMenuUI.getInstance.OpenDataPopup();
                        }
                        else if (houseMenuManager != null)
                        {
                            HouseMenuUI.getInstance.m_DataPopupText.text = string.Format(LocalizeText.Value["MobileDatacall_F"], downloadAssetTotalSize.ToString());
                            HouseMenuUI.getInstance.OpenDataPopup();
                        }
                        else
                        {
                            // 데이터 안내 팝업 오픈
                            HMSlideDropDownloaderUI.getInstance.m_DataPopupText.text = string.Format(LocalizeText.Value["MobileDatacall_F"], downloadAssetTotalSize.ToString());
                            HMSlideDropDownloaderUI.getInstance.OpenDataPopup();
                        }
                    }
                    else
                    {
                        PushDownloadRequest(assetDataSetList);
                    }
                }
                else
                {
                    MoveSlideDropCategory();
                }
            }
        }
        catch
        {
            if (www != null)
            {
                www.Dispose();
            }

            if (circleMenuManager != null)
            {
                CircleMenuUI.getInstance.OepnErrorPopup(LocalizeText.Value["Net_Retry"]);
            }
            else if(houseMenuManager != null)
            {
                HouseMenuUI.getInstance.OepnErrorPopup(LocalizeText.Value["Net_Retry"]);
            }
            else
            {
                HMSlideDropDownloaderUI.getInstance.OepnErrorPopup(LocalizeText.Value["Net_Retry"]);
            }
            //throw;
        }
        
    }

    /// <summary>
    /// 백업 URL로 다운로드 재시도. 재시도 후에도 에러 발생시 에러 팝업 활성화
    /// </summary>
    /// <param name="requestDataSet">다운로드 요청 정보 개체</param>
    /// <param name="www">WWW object</param>
    private void OnConfirmServerURI(HttpRequestDataSet requestDataSet, WWW www)
    {
        // null 이면 서버에서 주소 체크를 하지 않은 상태
        if (retryServerURI == null)
        {
            retryServerURI = www.text;
        }

        // 서버에서 받은 주소를 사용해서 다운로드 재시도
        if (!string.IsNullOrEmpty(retryServerURI) && retryServerURI.Contains("://"))
        {
            GlobalDataManager.m_AssetBundleURL = retryServerURI;
            retryDownload = true;
                        
            RequestVersionJsonFile();
        }
        else
        {
            // retryServerURI 값이 정상적인 주소가 아닌 null 또는 "no server url" 등 일때 
            if (CheckLocalDownFolderExist(requestDataSet.categoryType))     // categoryType 에 해당하는 xml(version file) 이 존재 할 경우
            {
                // 하위 메뉴를 화면에 활성화 합니다.
                MoveSlideDropCategory();                
            }
            else
            {
                // 에러 팝업을 화면에 활성화 합니다.
                if (circleMenuManager != null)
                {
                    CircleMenuUI.getInstance.OepnErrorPopup(LocalizeText.Value["Net_Retry"]);
                }
                else if (houseMenuManager != null)
                {
                    HouseMenuUI.getInstance.OepnErrorPopup(LocalizeText.Value["Net_Retry"]);
                }
                else
                {
                    HMSlideDropDownloaderUI.getInstance.OepnErrorPopup(LocalizeText.Value["Net_Retry"]);
                }
            }         
        }
    }      

    /// <summary>
    /// 버전파일 다운로드 중 에러 발생시 호출 되는 함수
    /// </summary>
    /// <param name="msg">에러 내용</param>
    private void OnErrorDownloadVersionFile(HttpRequestDataSet dataSet, string msg)
    {
        // 큐를 모두 비운다. (json 요청이 하나 이상 일 수 있어서... 테스트 필요함)
        reqDataQueue.Clear();

        if (!retryDownload)
        {
            RequestRetryConfirmUri();           
        }

        // 서버에 등록되어 있는 재접속 URL로 연결 시도 하였으나 에러가 발생 하였을 경우
        if (!string.IsNullOrEmpty(retryServerURI) && retryDownload)
        {
            // categoryType 에 해당하는 xml(version file) 이 존재 할 경우
            if (CheckLocalDownFolderExist(dataSet.categoryType))     
            {
                // 하위 메뉴를 화면에 활성화 합니다.
                MoveSlideDropCategory();
            }
            else
            {
                // 에러 팝업을 화면에 활성화 합니다.
                if (circleMenuManager != null)
                {
                    CircleMenuUI.getInstance.OepnErrorPopup(LocalizeText.Value["Net_Retry"]);
                }
                else if (houseMenuManager != null)
                {
                    HouseMenuUI.getInstance.OepnErrorPopup(LocalizeText.Value["Net_Retry"]);
                }
                else
                {
                    HMSlideDropDownloaderUI.getInstance.OepnErrorPopup(LocalizeText.Value["Net_Retry"]);
                }
            }
        }                        
    }

    /// <summary>
    /// 파일 다운로드 중 progress bar 적용
    /// </summary>
    /// <param name="requestDataSet">다운로드 요청 정보 개체</param>
    /// <param name="progressValue">progress value</param>
    //private void OnDrawProgressValue(HttpRequestDataSet requestDataSet, float progressValue)
    public virtual void OnDrawProgressValue(HttpRequestDataSet requestDataSet, float progressValue)
    {
        if(circleMenuManager !=null)
        {
            if (CircleMenuUI.circleInstance.m_ProgressbarObj != null)
            {
                if (CircleMenuUI.circleInstance.m_ProgressbarObj.gameObject.activeSelf == false)
                {
                    CircleMenuUI.circleInstance.m_ProgressbarObj.gameObject.SetActive(true);
                }
                // receiveFileSize 값은 downloadCompleteSize(현재까지 받은 크기) 에 현재 파일의 받은 크기를 더한 값
                receiveFileSize = downloadCompleteSize + (progressValue * requestDataSet.requestFileSize);

                //receiveFileSize = progressValue * requestDataSet.requestFileSize;

                CircleMenuUI.circleInstance.m_UpdateText.text = string.Format(LocalizeText.Value["DownloadProgress2_F"], receiveFileSize.ToString("N2"), downloadAssetTotalSize.ToString("N2"));
                // UI 부분에서 좌 -> 우 로 UI를 변경 하기 위해 (1 - 다운로드 비율) 을 적용 하게 됨
                CircleMenuUI.circleInstance.m_ProgressbarObj.value = receiveFileSize / downloadAssetTotalSize;

            }
        }
        else if (houseMenuManager != null)
        {
            if (HouseMenuUI.houseInstance.m_ProgressbarObj != null)
            {
                if (HouseMenuUI.houseInstance.m_ProgressbarObj.gameObject.activeSelf == false)
                {
                    HouseMenuUI.houseInstance.m_ProgressbarObj.gameObject.SetActive(true);
                }
                // receiveFileSize 값은 downloadCompleteSize(현재까지 받은 크기) 에 현재 파일의 받은 크기를 더한 값
                receiveFileSize = downloadCompleteSize + (progressValue * requestDataSet.requestFileSize);

                //receiveFileSize = progressValue * requestDataSet.requestFileSize;

                HouseMenuUI.houseInstance.m_UpdateText.text = string.Format(LocalizeText.Value["DownloadProgress2_F"], receiveFileSize.ToString("N2"), downloadAssetTotalSize.ToString("N2"));
                // UI 부분에서 좌 -> 우 로 UI를 변경 하기 위해 (1 - 다운로드 비율) 을 적용 하게 됨
                HouseMenuUI.houseInstance.m_ProgressbarObj.value = receiveFileSize / downloadAssetTotalSize;

            }
        }
        else if (HMSlideDropDownloaderUI.Instance.GetUseProgressBarValue())
        {
            if (HMSlideDropDownloaderUI.Instance.m_ProgressbarObj.gameObject.activeSelf == false)
            { 
                
                HMSlideDropDownloaderUI.Instance.m_ProgressbarObj.gameObject.SetActive(true);
            }

            // receiveFileSize 값은 downloadCompleteSize(현재까지 받은 크기) 에 현재 파일의 받은 크기를 더한 값
            receiveFileSize = downloadCompleteSize + (progressValue * requestDataSet.requestFileSize);           

            //receiveFileSize = progressValue * requestDataSet.requestFileSize;

            HMSlideDropDownloaderUI.Instance.m_UpdateText.text = string.Format(LocalizeText.Value["DownloadProgress_F"], receiveFileSize.ToString("N2"), downloadAssetTotalSize.ToString("N2"));
            // UI 부분에서 좌 -> 우 로 UI를 변경 하기 위해 (1 - 다운로드 비율) 을 적용 하게 됨
            HMSlideDropDownloaderUI.Instance.m_ProgressbarObj.value = 1 - (receiveFileSize / downloadAssetTotalSize);
        }
    }

    /// <summary>
    /// 에셋번들 파일을 다운로드 한 후 동작을 정의 하는 함수
    /// </summary>
    /// <param name="requestDataSet">다운로드 요청 정보 개체</param>
    /// <param name="www">WWW object</param>
    private void OnCompleteDownloadAssetBundleFile(HttpRequestDataSet requestDataSet, WWW www)
    {
        try
        {
            if (www == null)
            {
                throw new Exception("www object value is null");
            }

            // 다운로드 한 파일의 용량을 더한다. (progressbar 를 한번에 그리기 위해) 
            downloadCompleteSize += requestDataSet.requestFileSize;

            if (!Directory.Exists(requestDataSet.destinationFilePath))
            {
                Directory.CreateDirectory(requestDataSet.destinationFilePath);
            }

            File.WriteAllBytes(requestDataSet.destinationFileFullPath, www.bytes);
        }
        catch (Exception ex) 
        {
            Debug.LogError(string.Format("Function:{0}, Message:{1}", MethodBase.GetCurrentMethod().Name, ex.Message));
            throw;
        }        
    }

    /// <summary>
    /// 에셋번들 파일 다운로드 중 에러 발생시 호출 되는 함수
    /// </summary>
    /// <param name="msg">에러 내용</param>
    private void OnErrorDownloadAssetBundleFile(HttpRequestDataSet dataSet, string msg)
    {
        FileDownloaderUI.getInstance.OepnErrorPopup(msg);        
    }

    /// <summary>
    /// 에셋번들 파일의 다운로가 완료 될 때 들어오는 함수
    /// </summary>
    /// <param name="requestDataSet">다운로드 요청 정보 개체</param>
    private void OnFinishDownloadAssetBundleFile(HttpRequestDataSet requestDataSet)
    {
        // 에셋번들의 다운로드가 완료되면 임시폴더에 있는 json version 파일을 주 폴더로 이동
        if (reqDataQueue.Count == 0 && requestDataSet.requestDownloadStatus == GlobalDataManager.RequestDownloadStatus.AssetBundleDownload)
        {
            // 버전 파일 옮김
            FileCopyToDestiny();

            if (circleMenuManager == null)
            {
                MoveSlideDropCategory();
            }
            else
            {
                Invoke("MoveSlideDropCategory", 0.5f);
            }         
        }
    }

    /// <summary>
    /// 서버에서 받은 버전정보와 로컬의 버전정보를 리스트로 구성한다.
    /// </summary>
    /// <param name="data">서버에서 받은 JSON 데이터</param>
    /// <param name="revInfoJson">서버에서 받은 버전 정보를 저장할 리스트</param>
    /// <param name="localInfoJson">로컬 JSON 파일의 버전 정보를 저장할 리스트</param>
    /// <param name="isSearchLocalVersion">로컬 JSON 파일이 존재 하는지 확인하는 변수</param>
    private void ConfigureJsonVersionData(byte[] data, ref JsonDataVersionInfo[] revInfoJson, ref JsonDataVersionInfo[] localInfoJson, ref bool isSearchLocalVersion)
    {
        string receiveData                  = string.Empty;
        string versionFileName              = string.Empty;
        string localVersionFilePath         = string.Empty;
        string requestAssetBundleFileName   = string.Empty;        

        receiveData                         = Encoding.UTF8.GetString(data).TrimEnd('\0');
        versionFileName                     = string.Format("{0}.{1}", GlobalDataManager.GetAssetBundleVersionFileName(), GlobalDataManager.m_ExtJsonFile);
        localVersionFilePath                = GlobalDataManager.GetRequestFilePath(versionFileName, GlobalDataManager.RequestUrlType.Client);

        revInfoJson                         = JsonHelper.FromJson<JsonDataVersionInfo>(receiveData);

        if (File.Exists(localVersionFilePath))
        {
            localInfoJson                   = JsonHelper.FromJson<JsonDataVersionInfo>(File.ReadAllText(localVersionFilePath));
            isSearchLocalVersion            = true;
        }
        else
        {
            isSearchLocalVersion            = false;
        }
    }

    /// <summary>
    /// 서버에서 받은 JSON 데이터를 파일로 만듭니다.
    /// </summary>
    /// <param name="requestDataSet">다운로드 요청 정보 개체</param>
    /// <param name="data">버전 정보 JSON 데이터</param>
    private void WriteVersionFile(HttpRequestDataSet requestDataSet, byte[] data)
    {
        try
        {
            // 임시폴더에 버전 파일을 쓴다.
            StringBuilder destFilePathBuilder = new StringBuilder();

            destFilePathBuilder.AppendFormat("{0}/{1}", requestDataSet.destinationFilePath, GlobalDataManager.m_TemporaryFolderName);

            if (!Directory.Exists(destFilePathBuilder.ToString()))
            {
                Directory.CreateDirectory(destFilePathBuilder.ToString());
            }

            destFilePathBuilder.AppendFormat("/{0}.{1}", requestDataSet.requestFileTitle, requestDataSet.requestFileExt);
            File.WriteAllBytes(destFilePathBuilder.ToString(), data);
        }
        catch (Exception ex)
        {
            Debug.LogError(string.Format("Function:{0}, Message:{1}", MethodBase.GetCurrentMethod().Name, ex.Message));
            throw;
        }
    }

    /// <summary>
    /// 서버에서 받은 버전정보와 로컬의 버전정보를 비교하여
    /// 다운로드 해야 하는 에셋번들의 다운로드 용량을 구합니다.
    /// </summary>
    /// <param name="data">서버에서 받은 버전 JSON 데이터</param>
    private void GetDownloadSizeFromJson(byte[] data)
    {
        JsonDataVersionInfo[] revInfoJson   = null;
        JsonDataVersionInfo[] localInfoJson = null;

        bool isSearchLocalVersion           = false;
        bool isFindAssetBundleVersion       = false;

        try
        {
            ConfigureJsonVersionData(data, ref revInfoJson, ref localInfoJson, ref isSearchLocalVersion);

            if (revInfoJson == null) { return; }

            // 로컬에 버전 파일이 있는 경우
            if (isSearchLocalVersion)
            {
                if (localInfoJson == null) { return; }

                // 서버에서 받은 에셋번들 버전 정보와 로컬 에셋번들 버전 정보를 비교
                foreach (JsonDataVersionInfo receiveVersion in revInfoJson)
                {
                    foreach (JsonDataVersionInfo localVersion in localInfoJson)
                    {
                        if (receiveVersion.fileTitle.Equals(localVersion.fileTitle))
                        {
                            if (receiveVersion.fileVersion > localVersion.fileVersion)
                            {
                                downloadAssetTotalSize += receiveVersion.fileSize;
                            }

                            isFindAssetBundleVersion = true;
                            break;
                        }
                    }

                    // 위에서 찾았을 경우 continue 함 (하위에 있는 '신규로 추가 ...' 코드를 실행 하지 않기 위함)
                    if (isFindAssetBundleVersion)
                    {
                        isFindAssetBundleVersion = false;
                        continue;
                    }

                    // 다운로드 크기를 추가 합니다.
                    downloadAssetTotalSize += receiveVersion.fileSize;
                }
            }
            else
            {
                // 로컬에 버전 파일이 없는 경우 서버 버전 정보대로 다운로드 요청
                foreach (JsonDataVersionInfo versionInfo in revInfoJson)
                {
                    downloadAssetTotalSize += versionInfo.fileSize;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(string.Format("Function:{0}, Message:{1}", MethodBase.GetCurrentMethod().Name, ex.Message));
            if (circleMenuManager != null)
            {
                CircleMenuUI.getInstance.OepnErrorPopup(LocalizeText.Value["Net_Retry"]);
            }
            else if(houseMenuManager !=null)
            {
                HouseMenuUI.getInstance.OepnErrorPopup(LocalizeText.Value["Net_Retry"]);
            }
            else
            {
                HMSlideDropDownloaderUI.getInstance.OepnErrorPopup(LocalizeText.Value["Net_Retry"]);
            }
            throw;
        }        
    }

    /// <summary>
    /// 서버에서 받은 버전정보와 로컬의 버전정보를 비교하여
    /// 다운로드 해야 하는 에셋번들의 다운로드 리스트를 구성합니다.
    /// </summary>
    /// <param name="data">서버에서 받은 버전 JSON 데이터</param>
    private void CreateDownloadList(byte[] data)
    {
        JsonDataVersionInfo[] revInfoJson   = null;
        JsonDataVersionInfo[] localInfoJson = null;

        bool isSearchLocalVersion           = false;
        bool isFindAssetBundleVersion       = false;

        try
        {
            ConfigureJsonVersionData(data, ref revInfoJson, ref localInfoJson, ref isSearchLocalVersion);

            if (revInfoJson == null) { return; }

            // 로컬에 버전 파일이 있는 경우
            if (isSearchLocalVersion)
            {
                if (localInfoJson == null) { return; }

                // 서버에서 받은 에셋번들 버전 정보와 로컬 에셋번들 버전 정보를 비교
                foreach (JsonDataVersionInfo receiveVersion in revInfoJson)
                {
                    foreach (JsonDataVersionInfo localVersion in localInfoJson)
                    {
                        if (receiveVersion.fileTitle.Equals(localVersion.fileTitle))
                        {
                            if (receiveVersion.fileVersion > localVersion.fileVersion)
                            {
                                // 에셋번들 다운로드 요청
                                RequestAssetBundleFile(receiveVersion.fileTitle, receiveVersion.fileExt, receiveVersion.fileSize);
                            }

                            isFindAssetBundleVersion = true;
                            break;
                        }
                    }

                    // 위에서 찾았을 경우 continue 함 (하위에 있는 '신규로 추가 ...' 코드를 실행 하지 않기 위함)
                    if (isFindAssetBundleVersion)
                    {
                        isFindAssetBundleVersion = false;
                        continue;
                    }

                    // 신규로 추가 되는 에셋번들
                    RequestAssetBundleFile(receiveVersion.fileTitle, receiveVersion.fileExt, receiveVersion.fileSize);
                }
            }
            else
            {
                // 로컬에 버전 파일이 없는 경우 서버 버전 정보대로 다운로드 요청
                foreach (JsonDataVersionInfo versionInfo in revInfoJson)
                {
                    RequestAssetBundleFile(versionInfo.fileTitle, versionInfo.fileExt, versionInfo.fileSize);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(string.Format("Function:{0}, Message:{1}", MethodBase.GetCurrentMethod().Name, ex.Message));
            throw;
        }        
    }

    /// <summary>    
    /// 팝업 윈도우에서 다운로드 버튼 클릭 이벤트 처리
    /// </summary>
    public void OnClickDownloadAssetBundle()
    {
        HMSlideDropDownloaderUI.getInstance.ClosePopupWindow();
        PushDownloadRequest(assetDataSetList);

        if (HMSlideDropUIManager.Instance.clickPrevention.activeSelf == false)
        {
            HMSlideDropUIManager.Instance.clickPrevention.SetActive(true);
        }
    }

    /// <summary>
    /// 팝업 윈도우에서 닫기 버튼 클릭 이벤트 처리
    /// </summary>
    public void OnClickCloseDialog()
    {
        HMSlideDropDownloaderUI.getInstance.ClosePopupWindow();
    }

    /// <summary>
    /// 무한반복 하는 다운로드 코루틴을 멈춥니다.
    /// </summary>
    public void StopDownloadCoroutine()
    {
        if (queueCoroutine != null)
        {
            StopCoroutine(queueCoroutine);
            queueCoroutine = null;
        }
    }

    /// <summary>
    /// 인터넷 (3G, LTE, Wi-Fi) 연결이 되어 있는지 확인
    /// </summary>
    /// <returns>true or false</returns>            
    public bool ConfrimNetworkConnection()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 로컬 경로에 해당 파일이 있는지 확인
    /// </summary>
    /// <returns>true or false</returns>
    public bool ConfirmHasVersionFile()
    {
        string versionFile = string.Empty;
        versionFile = string.Format("{0}/{1}.{2}", GlobalDataManager.GetRequestFilePath(null, GlobalDataManager.RequestUrlType.Client), GlobalDataManager.GetAssetBundleVersionFileName(), GlobalDataManager.m_ExtJsonFile);
        Debug.Log("versionFile : " + versionFile);
        return (File.Exists(versionFile));
    }

    public void RequestAssetbundleDownload()
    {
        // 다운로더 루프 실행
        queueCoroutine = StartCoroutine(HttpDownloaderLoop());

        // add product value (버전 파일 다운로드 할 때 제품 리스트로 반복문을 구성 함)
        ConfigureDownloadProductList();

        // 버전 json 파일 다운로드 요청            
        RequestVersionJsonFile();
    }

    /// <summary>
    /// 다운로드 리스트를 구성하고 다운로드 합니다. (사용하지 않음)
    /// </summary>
    public void RunDownloader()
    {
        if (ConfrimNetworkConnection())
        {
            RequestAssetbundleDownload();
        }
        else
        {
            // 버전 파일 유무 확인 후 안내 팝업 또는 씬 이동
            if (ConfirmHasVersionFile())
            {
                // "콘텐츠 업데이트가 완료 되었습니다."                
                HMSlideDropDownloaderUI.getInstance.m_UpdateText.text = LocalizeText.Value["Net_UpdateFinish"];
                //MoveSlideDropCategory();                
            }
            else
            {
               // int clickIndex      = -1;
               // GameObject clickObj = null;

               // clickIndex          = HMSlideDropUIManager.Instance.GetClickObjectIndex();
               // clickObj            = HMSlideDropUIManager.Instance.m_CategoryMenuDepthInfo[clickIndex].curDepthObject;

                //HMSlideDropDownloaderUI.Instance.ChangeDownloadStatusIcon(clickObj);
                HMSlideDropDownloaderUI.getInstance.OpenPopup();
            }
        }
    }

    /// <summary>
    /// 제품에 상관없이 다운로드 가능한 에셋번들을 모두 다운로드 합니다.
    /// </summary>
    public void DownloadAllAssetBundle()
    {
        if (ConfrimNetworkConnection())
        {
            // 다운로더 루프 실행
            queueCoroutine = StartCoroutine(HttpDownloaderLoop());            
            downloadedProductList.Clear();

            foreach (GlobalDataManager.CategoryType addType in addCategoryType)
            {
                downloadedProductList.Add(addType);
            }

            for (int idx = 0; idx < slideDropUIManager.m_CategoryMenuDepthInfo.Length; idx++)
            {
                downloadedProductList.Add(slideDropUIManager.m_CategoryMenuDepthInfo[idx].categoryType);
            }

            // 버전 json 파일 다운로드 요청            
            RequestVersionJsonFile();            
        }
    }
}
 