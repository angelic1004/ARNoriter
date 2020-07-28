using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using HedgehogTeam.EasyTouch;

public class HMSlideDropUIManager : BaseMainUI
{
    public static HMSlideDropUIManager Instance;

    public GameObject main2DUI;
    public Camera nguiCam;                                                // NGUI 카메라

    // write by N.C Park
    public DownloadManager downloadManager;

    public GameObject categoryOne;                                        // 첫번째 카테고리
    public GameObject categoryTwo;                                        // 두번째 카테고리

    [HideInInspector]
    public List<GameObject> categoryTwoSub;                               // 카테고리2 하위 오브젝트

    public GameObject loadingPopup;

    public GameObject clickPrevention;                                    // 클릭 방지용 오브젝트 (다운로드 중일때 적용)
    public GameObject leftMenuBar;                                        // 왼쪽 메뉴 바

    public GameObject downCategory4D;                                     // 4D 체험하기 다운카테고리
    private GameObject[] downCate4DchildObj;                              // 4D 체험하기 다운카테고리 자식 오브젝트

    public QRCodeInfo m_QRCodeInfo;
    public PassTargetInfo m_PassTargetInfo;

    public CategoryMenuListDepthInformation[] m_CategoryMenuDepthInfo;

    private int clickObjectIndex;                                         // 클릭한 오브젝트 인덱스

    private int currentDownCategoryNum;                                   // 현재 다운 카테고리 인덱스

    private UISprite categoryOneSprite;
    public GameObject blackPanel;

    // 스크롤 표시 UI 오브젝트들
    public GameObject scrollUpBtn;                                        // 스크롤 UP 버튼
    public Transform scrollTopPos;                                        // 스크롤 Top영역 위치
    public Transform scrollCheckUp;                                       // 스크롤 Up 체크

    public GameObject scrollDownBtn;                                      // 스크롤 Down 버튼
    public Transform scrollBottomPos;                                     // 스크롤 Bottom영역 위치
    public Transform scrollCheckDown;                                     // 스크롤 Down 체크

    private bool isDoubleClickBackButton;                                 // 장치의 Back버튼을 더블클릭했는지 체크 (앱종료 기능 사용시)

    void Awake()
    {
        Instance = this;

        blackPanel.SetActive(true);
        ScreenSet();

        ResetCategoryOneAnchors();

        Invoke("InitHMSlideDropUIManager", 0.1f);
        Invoke("RecallMenuStatus", 0.3f);

        StartCoroutine("DeactiveBlackPanel", 0.5f);
    }

    void Start()
    {
        
    }
        
    public override void OnClickPreDepthButton()
    {
        throw new NotImplementedException();
    }

    void Update()
    {
        // 물리 Back버튼 더블클릭으로 종료 기능 (안드로이드 디바이스)
        if (Application.platform != RuntimePlatform.IPhonePlayer)
        {
            if(Input.GetKeyUp(KeyCode.Escape))
            {
                // 한번 클릭 체크
                if(isDoubleClickBackButton == false)
                {
                    isDoubleClickBackButton = true;
                    StartCoroutine(ResetDoubleClickBackButton());
                }
                else if(!m_ClosePopupObj.activeSelf)
                {
                    // 두번 클릭시 종료팝업
                    ExpirePopupOpen();
                }
            }
        }
    }

    void OnEnable()
    {
        EasyTouch.On_DragStart += DragStartEvent;
        EasyTouch.On_DragEnd += DragEndEvent;        
    }

    void OnDisable()
    {
        UnsubscribeEvent();
    }

    void OnDestroy()
    {
        UnsubscribeEvent();
    }

    void UnsubscribeEvent()
    {
        EasyTouch.On_DragStart -= DragStartEvent;
        EasyTouch.On_DragEnd -= DragEndEvent;
    }

    /// <summary>
    /// QR 씬에서 'Skip' 버튼 또는 인증을 완료 한 후 HansMainScene 이 실행 될 때 QR 씬 이동이 필요 없는 경우 
    /// OnClickCategoryButton() 함수를 호출 하여 '에셋번들 다운로드' 또는 '카테고리 TWO' 를 동작 하도록 합니다.
    /// </summary>
    private void ApplyQRCodeResult()
    {
        if (m_QRCodeInfo.usedQRScene)
        {
            if (QRCodeUI.needScanning == false)
            {
                if (string.IsNullOrEmpty(QRCodeUI.backupSceneName) == false)
                {
                    foreach (CategoryMenuListDepthInformation info in m_CategoryMenuDepthInfo)
                    {
                        if (string.Compare(info.categoryType.ToString().ToLower(), QRCodeUI.backupSceneName) == 0)
                        {
                            OnClickCategoryButton(info.curDepthObject);

                            QRCodeUI.needScanning       = true;
                            QRCodeUI.backupSceneName    = string.Empty;
                            
                            break;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 드래그 시작 이벤트
    /// </summary>
    private void DragStartEvent(Gesture ges)
    {
        Invoke("ResetScrollUI", 0.2f);
    }

    /// <summary>
    /// 드래그 종료 이벤트
    /// </summary>
    private void DragEndEvent(Gesture ges)
    {
        Invoke("ResetScrollUI", 0.5f);
    }

    #region 유틸리티

    /// <summary>
    /// 게임오브젝트를 지정한 시간동안 켰다 끕니다.
    /// </summary>
    IEnumerator OnAndOffGameObject(GameObject obj, float second)
    {
        obj.SetActive(true);

        yield return new WaitForSeconds(second);

        obj.SetActive(false);
    }

    #endregion


    #region 초기화 함수들

    private void InitSecondMenuIcon()
    {
        //HMCategoryInfoManager.Instance.categoryOneInfo
    }
    

    /// <summary>
    /// 초기화 함수
    /// </summary>
    private void InitHMSlideDropUIManager()
    {
        SetCategoryOnOff();
        SetCategoryPosition();
        CamTweenPositionSetting();

        clickObjectIndex = -1;
        clickPrevention.SetActive(false);

        currentDownCategoryNum = -1;
        InitLoadingPopup(false);
        //loadingPopup.SetActive(false);

        ResetCategoryTwoButtonAnchor();

        ResetScrollUI();
    }

    private void InitLoadingPopup(bool state)
    {
        GlobalDataManager.CategoryType prod = GlobalDataManager.m_SelectedCategoryEnum;
        UIAtlas at = loadingPopup.transform.FindChild("Loading Image").GetComponent<UISprite>().atlas;
        UISprite sp = loadingPopup.transform.FindChild("Loading Image").GetComponent<UISprite>();
        UISpriteAnimation spAni = loadingPopup.transform.FindChild("Loading Image").GetComponent<UISpriteAnimation>();

        at.name = "MainMenu";

        switch (prod)
        {
            /*
            case GlobalDataManager.CategoryType.Princess:
                sp.spriteName = "princess_loading_1";
                spAni.namePrefix = "princess_loading_";
                spAni.framesPerSecond = 8;
                break;
            case GlobalDataManager.ProductType.Soccer:
                sp.spriteName = "soccer_loading_1";
                spAni.namePrefix = "soccer_loading_";
                spAni.framesPerSecond = 12;
                break;

            case GlobalDataManager.ProductType.RacingCar:
                sp.spriteName = "racingcar_loading_1";
                spAni.namePrefix = "racingcar_loading_";
                spAni.framesPerSecond = 15;
                break;

            case GlobalDataManager.ProductType.WatchCar:
                sp.spriteName = "watchcar_loading_1";
                spAni.namePrefix = "watchcar_loading_";
                spAni.framesPerSecond = 11;
                break;

            case GlobalDataManager.ProductType.NasCar:
                sp.spriteName = "nascar_loading_1";
                spAni.namePrefix = "nascar_loading_";
                spAni.framesPerSecond = 19;
                break;

            case GlobalDataManager.ProductType.PrincessDance:
                sp.spriteName = "princess_loading_1";
                spAni.namePrefix = "princess_loading_";
                spAni.framesPerSecond = 8;
                break;
                */
        }

        loadingPopup.SetActive(state);
    }

    /// <summary>
    /// 지정된 시간동안 대기후에 BlackPanel을 끕니다.
    /// </summary>
    IEnumerator DeactiveBlackPanel(float sec)
    {
        yield return new WaitForSeconds(sec);

        blackPanel.SetActive(false);
        ApplyQRCodeResult();
    }

    /// <summary>
    /// 스크린 회전방향 셋팅
    /// </summary>
    private void ScreenSet()
    {
        // 자동 회전 적용
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = false;
    }

    /// <summary>
    /// CAM의 트윈포지션 셋팅
    /// </summary>
    public void CamTweenPositionSetting()
    {
        // 카테고리1 -> 카테고리2로 이동위치 설정
        nguiCam.gameObject.GetComponent<TweenPosition>().from = new Vector3(categoryOne.transform.localPosition.x, 0, 0);
        nguiCam.gameObject.GetComponent<TweenPosition>().to = new Vector3(categoryTwo.transform.localPosition.x, 0, 0);
    }

    /// <summary>
    /// AR씬에서 돌아왔을 경우 마지막 메뉴 상태를 불러옵니다.
    /// </summary>
    public void RecallMenuStatus()
    {
        if (GlobalDataManager.m_SelectedCategoryEnum != GlobalDataManager.CategoryType.None)
        {
            ResetCategoryTwoButtonAnchor();

            GlobalDataManager.CategoryType prod = GlobalDataManager.m_SelectedCategoryEnum;

            // 클릭한 오브젝트 번호 찾기
            for (int i = 0; i < m_CategoryMenuDepthInfo.Length; i++)
            {
                if (prod == m_CategoryMenuDepthInfo[i].categoryType)
                {
                    clickObjectIndex = i;
                    break;
                }
            }

            // 카테고리2 Y위치 설정
            categoryTwo.transform.localPosition = new Vector2(categoryTwo.transform.localPosition.x, categoryOne.transform.localPosition.y);

            ResetCategoryTwoButtonAnchor();

            // Tween으로 슬라이드 오른쪽 이동
            nguiCam.GetComponent<TweenPosition>().PlayForward();

            HMCategoryInfoManager.Instance.ChangeCategoryValues(clickObjectIndex);

            // 업데이트 텍스트 메시지 숨기기
            HMSlideDropDownloaderUI.Instance.m_UpdateText.gameObject.SetActive(false);

            ResetCategoryTwoButtonAnchor();

            categoryTwo.GetComponent<UIScrollView>().ResetPosition();

            ScrollDownCategory();
        }

        if (GlobalDataManager.m_ResourceFolderEnum != GlobalDataManager.CategoryType.None && GlobalDataManager.m_ConfirmCertification)
        {
            GameObject clickObj                 = null;
            GlobalDataManager.CategoryType prod  = GlobalDataManager.m_ResourceFolderEnum;

            // 클릭한 오브젝트 번호 찾기
            for (int i = 0; i < m_CategoryMenuDepthInfo.Length; i++)
            {
                if (prod == m_CategoryMenuDepthInfo[i].categoryType)
                {
                    clickObj = m_CategoryMenuDepthInfo[i].curDepthObject;
                    break;
                }
            }

            if (clickObj != null)
            {
                categoryOne.transform.localPosition = new Vector3(categoryOne.transform.localPosition.x, GlobalDataManager.m_MainMenuScrollValue, categoryOne.transform.localPosition.z);
                OnClickCategoryButton(clickObj);
            }
        }
    }

    /// <summary>
    /// 카테고리1의 앵커를 리셋 합니다.
    /// </summary>
    private void ResetCategoryOneAnchors()
    {
        // 카테고리 앵커 리셋
        categoryOneSprite = m_CategoryMenuDepthInfo[0].curDepthObject.GetComponent<UISprite>();
        categoryOneSprite.UpdateAnchors();
    }

    /// <summary>
    /// 백버튼을 더블클릭하지 않았을시 초기화합니다.
    /// </summary>
    IEnumerator ResetDoubleClickBackButton()
    {
        yield return new WaitForSeconds(2.0f);

        isDoubleClickBackButton = false;
    }


    #endregion


    #region 씬로드 부분 : 추후 다운로더 부분으로 이동시켜야 될듯

    /// <summary>
    /// 씬을 로드합니다.
    /// </summary>
    public void LoadScene(GameObject obj)
    {
        ButtonInfo btnInfo = obj.GetComponent<ButtonInfo>();

        if (btnInfo != null)
        {
            GlobalDataManager.m_SelectedSceneStateEnum = btnInfo.sceneState;
            GlobalDataManager.m_SelectedSceneName = string.Format("{0}_{1}", GlobalDataManager.m_SelectedCategoryEnum.ToString().ToLower(), btnInfo.loadSceneName);
            GlobalDataManager.m_AssetBundlePartName = btnInfo.assetBundleName;

            Debug.Log("GlobalDataManager.m_AssetBundlePartName: " + GlobalDataManager.m_AssetBundlePartName);
        }

        //SceneManager.LoadScene(GlobalDataManager.m_SelectedSceneName);
        StartCoroutine(OpenLoadScene());

        //loadingPopup.SetActive(true);
        InitLoadingPopup(true);
        ResetLoadingScreenOrientation(btnInfo.loadSceneName);
    }

    public IEnumerator OpenLoadScene()
    {
        SceneManager.LoadSceneAsync(GlobalDataManager.m_SelectedSceneName);
        yield return null;
    }

    ///////// LoadSceneAsync를 Additive 모드로 사용하는 소스 ////////
    //public IEnumerator OpenLoadScene()
    //{
    //    yield return SceneManager.LoadSceneAsync(GlobalDataManager.m_SelectedSceneName, LoadSceneMode.Additive);

    //    SceneManager.SetActiveScene(SceneManager.GetSceneByName(GlobalDataManager.m_SelectedSceneName));
    //    MainUI.메인.gameObject.SetActive(false); // 2D UI를 끈다.
    //}

    //public static void CloseLoadScene()
    //{
    //    if (!string.IsNullOrEmpty(GlobalDataManager.m_SelectedSceneName))
    //    {
    //        Instance.main2DUI.SetActive(false); // 2D UI를 끈다.
    //        SceneManager.UnloadScene("01. HansMain");

    //        MainUI.메인.gameObject.SetActive(true); // 2D UI를 다시 켜준다.
    //        GlobalDataManager.m_SelectedSceneName = null;
    //    }
    //}
    ////////////////////////////////////////////////////////////////

    /// <summary>
    /// 로딩 스크린 회전 리셋
    /// </summary>
    private void ResetLoadingScreenOrientation(string loadSceneInfo)
    {
        bool setPortrait = false;

        switch (loadSceneInfo)
        {
            //case "observe":
            //case "language":
            case "conversation":

                setPortrait = true;             // 세로모드
                break;

            default:
                setPortrait = false;            // 가로모드
                break;
        }

        if (!setPortrait)
        {
            // 가로모드 적용
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        }
    }

    #endregion


    #region 카테고리1 관련 함수들

    /// <summary>
    /// 선택한 버튼 위로 업데이트 텍스트를 이동시킵니다.
    /// </summary>
    private void MoveUpdateTextObject(GameObject selectedBtn)
    {
        GameObject updateTextObj = HMSlideDropDownloaderUI.Instance.m_UpdateText.gameObject;
        updateTextObj.SetActive(true);
        updateTextObj.transform.localPosition = new Vector2(0, selectedBtn.transform.localPosition.y - 190);
    }

    private void ApplyNextMotion(int clickIndex)
    {
        try
        {
            if (clickIndex == -1)
            {
                throw new Exception(string.Format("clickIndex value is -1"));
            }
            
            GlobalDataManager.m_SelectedCategoryEnum    = m_CategoryMenuDepthInfo[clickIndex].categoryType;
            GlobalDataManager.m_ResourceFolderEnum      = m_CategoryMenuDepthInfo[clickIndex].categoryType;
                        
            if (downloadManager.ConfrimNetworkConnection())
            {
                if (m_PassTargetInfo.usedPassScene && GlobalDataManager.m_ConfirmCertification == false)
                {
                    // 타겟으로 인증을 하면 에셋번들을 다운로드 할 수 있도록 인증 씬으로 이동 함
                    // 로컬에 선택한 카테고리의 버전 파일이 존재하면 인증 씬으로 이동하지 않고 다음 스텝을 실행
                    if (GlobalDataManager.GetResultFindVersionFile() == 0)
                    {
                        GlobalDataManager.m_MainMenuScrollValue = categoryOne.transform.localPosition.y;
                        GlobalDataManager.m_SelectedSceneName   = m_PassTargetInfo.namePassScene;

                        GlobalDataManager.GlobalLoadScene();
                        return;
                    }
                }

                HMSlideDropDownloaderUI.Instance.ChangeDownloadStatusIcon(m_CategoryMenuDepthInfo[clickIndex].curDepthObject);
                HMSlideDropDownloaderUI.Instance.SetProgressbarTarget(m_CategoryMenuDepthInfo[clickIndex].curDepthObject);

                downloadManager.RequestAssetbundleDownload();
            }
            else
            {
                if (downloadManager.ConfirmHasVersionFile())
                {
                    downloadManager.StopDownloadCoroutine();
                    HMSlideDropDownloaderUI.getInstance.m_UpdateText.text = LocalizeText.Value["Net_UpdateFinish"];

                    SetCategoryOnOff();
                    SlideCategory();
                }
                else
                {
                    HMSlideDropDownloaderUI.getInstance.OpenPopup();
                }                
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(string.Format("exception messgae : {0}", ex.Message));
            throw;
        }
    }

    /// <summary>
    /// 슬라이드로 카테고리 이동
    /// </summary>
    public void SlideCategory()
    {
        GameObject clickCatObj = m_CategoryMenuDepthInfo[clickObjectIndex].curDepthObject;
        HMSlideDropDownloaderUI.Instance.ChangeDownloadStatusIcon(clickCatObj);

        // Tween으로 슬라이드 오른쪽 이동
        nguiCam.GetComponent<TweenPosition>().PlayForward();

        HMCategoryInfoManager.Instance.ChangeCategoryValues(clickObjectIndex);

        // 업데이트 텍스트 메시지 숨기기
        HMSlideDropDownloaderUI.Instance.m_UpdateText.gameObject.SetActive(false);

        clickPrevention.SetActive(false);
        categoryTwo.GetComponent<UIScrollView>().ResetPosition();
        ChangeScrollCheckAnchor(false);

        ScrollDownCategory();
    }

    /// <summary>
    /// 다운로드 완료된 카테고리UI는 켜줌
    /// </summary>
    public void SetCategoryOnOff()
    {
        bool downFolderExist;       // 다운로드 폴더가 존재하는지 여부
        GameObject blackSlide;
        GameObject downloadButton;

        for (int i = 0; i < m_CategoryMenuDepthInfo.Length; i++)
        {
            downFolderExist = downloadManager.CheckLocalDownFolderExist(m_CategoryMenuDepthInfo[i].categoryType);

            if (downFolderExist)
            {
                blackSlide = m_CategoryMenuDepthInfo[i].curDepthObject.transform.FindChild("Black Slide").gameObject;
                blackSlide.SetActive(false);

                downloadButton = m_CategoryMenuDepthInfo[i].curDepthObject.transform.FindChild("Download Button").gameObject;
                downloadButton.GetComponent<UISprite>().spriteName = "icon_downloaded";
            }
            else
            {
                blackSlide = m_CategoryMenuDepthInfo[i].curDepthObject.transform.FindChild("Black Slide").gameObject;
                blackSlide.SetActive(true);
            }
        }
    }

    public override void OnClickCategoryButton(GameObject obj)
    { 
        try
        {
            if (obj == null)
            {
                throw new Exception(string.Format("GameObject value is null"));
            }

            clickPrevention.SetActive(true);
            clickObjectIndex = -1;

            // 클릭한 오브젝트 인덱스 구하기
            for (int idx = 0; idx < m_CategoryMenuDepthInfo.Length; idx++)
            {
                if (obj.name.Equals(m_CategoryMenuDepthInfo[idx].curDepthObject.name))
                {
                    clickObjectIndex = idx;
                    break;
                }
            }

            // 이곳에서 QRScene 으로 이동 시킬 것이지 체크
            if (m_QRCodeInfo.usedQRScene)
            {
                if (QRCodeUI.needScanning)
                {
                    if (QRCodeUI.CheckCertResult(clickObjectIndex) == false)
                    {
                        QRCodeUI.backupSceneName = m_CategoryMenuDepthInfo[clickObjectIndex].categoryType.ToString().ToLower();
                        QRCodeUI.메뉴인덱스1단계 = clickObjectIndex;

                        SceneManager.LoadScene(m_QRCodeInfo.nameQRScene);

                        return;
                    }
                }
                else
                {
                    // backupSceneName 값이 없으면 '뒤로가기' 버튼을 눌러 넘어온 것으로 판단하여 다음 스텝을 진행 하지 않음 
                    if (string.IsNullOrEmpty(QRCodeUI.backupSceneName))
                    {
                        return;
                    }                 
                }                                
            }

            MoveUpdateTextObject(obj);
            ApplyNextMotion(clickObjectIndex);
        }
        catch (Exception ex)
        {
            Debug.LogError(string.Format("exception message : {0}", ex.Message));
            throw;
        }
    }

    

    #endregion


    #region 카테고리2 관련 함수들

    /// <summary>
    /// 카테고리2 버튼 앵커를 리셋합니다.
    /// </summary>
    public void ResetCategoryTwoButtonAnchor()
    {
        categoryTwoSub[0].GetComponent<UISprite>().UpdateAnchors();
    }

    /// <summary>
    /// 카테고리 시작위치 설정
    /// </summary>
    private void SetCategoryPosition()
    {
        Vector4 clipRegion = categoryOne.GetComponent<UIPanel>().baseClipRegion;

        float sizeZ, sizeW;

        // AR씬에서 뒤로가기로 왔을경우 스크롤뷰 Clip Region 가로/세로 체크
        if (clipRegion.z < clipRegion.w)
        {
            sizeZ = clipRegion.z;
            sizeW = clipRegion.w;
        }
        else
        {
            sizeZ = clipRegion.w;
            sizeW = clipRegion.z;
        }

        // 카테고리2 스크롤뷰에 사이즈 적용
        categoryTwo.GetComponent<UIPanel>().baseClipRegion = new Vector4(clipRegion.x, clipRegion.y, sizeZ, sizeW);
        categoryTwo.transform.localPosition = new Vector3(sizeZ, categoryOne.transform.localPosition.y, categoryOne.transform.localPosition.z);

        foreach (Transform child in categoryTwo.transform)
        {
            if (child.name.Contains("BTN"))
            {
                categoryTwoSub.Add(child.gameObject);
            }
        }
    }

    public void ResetCategoryPosition()
    {
        categoryTwo.GetComponent<UIScrollView>().ResetPosition();
        categoryTwo.transform.localPosition = new Vector3(categoryTwo.transform.localPosition.x, categoryTwo.transform.localPosition.y, categoryTwo.transform.localPosition.z);
    }

    /// <summary>
    /// 뒤로가기 버튼 클릭시 (카테고리2 -> 카테고리1)
    /// </summary>
    public void ClickBackButton()
    {
        ResetCategoryPosition();

        for (int i = 0; i < categoryTwoSub.Count; i++)
        {
            if (categoryTwoSub[i].GetComponent<UIPlayTween>() != null)
            {
                // 다운 카테고리 오브젝트
                GameObject downCate = categoryTwoSub[i].transform.FindChild("Down Category").gameObject;

                if (categoryTwoSub[i].GetComponent<UIPlayTween>().tweenTarget == null)
                {
                    categoryTwoSub[i].GetComponent<UIPlayTween>().tweenTarget = downCate;
                }

                // 다운 카테고리가 펼쳐져 있으면
                if (downCate.transform.localPosition.y != downCate.GetComponent<TweenPosition>().from.y)
                {
                    // 닫기 트윈 실행
                    categoryTwoSub[i].GetComponent<UIPlayTween>().Play(false);
                }
            }
        }

        StartCoroutine(HMCategoryInfoManager.Instance.OffDownCategoryObj());
        HMCategoryInfoManager.Instance.ResetEmptyDownCategory();

        // 현재 다운 카테고리 번호 초기화
        currentDownCategoryNum                      = -1;
        GlobalDataManager.downCategoryNum           = -1;
        GlobalDataManager.m_ConfirmCertification    = false;

        ChangeScrollCheckAnchor(true);
    }

    /// <summary>
    /// Category Two의 포지션을 리셋합니다.
    /// </summary>
    private void ResetCategoryTwoPosition(GameObject obj)
    {
        Vector3 pos = nguiCam.WorldToViewportPoint(obj.transform.position);

        if (pos.y > 1)
        {
            categoryTwo.GetComponent<UIScrollView>().ResetPosition();
        }
    }

    /// <summary>
    /// 제품을 클릭한 인덱스를 리턴합니다.
    /// </summary>
    /// <returns>int</returns>
    public int GetClickObjectIndex()
    {
        return clickObjectIndex;
    }


    #endregion


    #region 다운 카테고리 관련 함수들

    /// <summary>
    /// 다운카테고리 슬라이드 이벤트
    /// </summary>
    /// <param name="obj">Self 오브젝트</param>
    public void DownCategorySlideEvent(GameObject obj)
    {
        bool isDirectLoad = HMCategoryInfoManager.Instance.CheckDirectLoading(obj);

        if (isDirectLoad)
        {
            LoadScene(obj);
        }
        else
        {
            Invoke("ResetScrollUI", 0.5f);

            // 기존의 다운카테고리가 펼쳐져 있는경우 닫기
            if (currentDownCategoryNum != -1)
            {
                categoryTwoSub[currentDownCategoryNum].GetComponent<UIPlayTween>().Play(false);
            }

            if (obj != null)
            {
                // 현재 다운카테고리 번호를 저장
                for (int i = 0; i < categoryTwoSub.Count; i++)
                {
                    if (obj == categoryTwoSub[i])
                    {
                        if (currentDownCategoryNum != i)
                        {
                            currentDownCategoryNum = i;
                            GameObject clickPrevention2 = categoryTwo.transform.FindChild("Click Prevention").gameObject;
                            StartCoroutine(OnAndOffGameObject(clickPrevention2, 0.5f));

                            GlobalDataManager.downCategoryNum = currentDownCategoryNum;

                            return;
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
            else
            {
                Debug.LogError("On Click Method가 비어 있습니다. Hierarchy를 통해 확인하세요.");
            }
        }
    }

    private void ScrollDownCategory()
    {
        int downCategoryNum = GlobalDataManager.downCategoryNum;
        bool showDownCategory = HMCategoryInfoManager.Instance.categoryOneInfo[clickObjectIndex].showDefaultDownCategory;

        if (showDownCategory)
        {
            if(downCategoryNum == -1)
            {
                downCategoryNum = 0;
            }

            categoryTwoSub[downCategoryNum].GetComponent<UIPlayTween>().Play(true);
            currentDownCategoryNum = downCategoryNum;
        } 
    }

    #endregion


    #region 스크롤 관련 함수들


    /// <summary>
    /// 스크롤 UI를 리셋합니다.
    /// </summary>
    private void ResetScrollUI()
    {
        // 스크롤 기준 포지션과 체크 포지션을 비교함 (Top)
        if (scrollTopPos.localPosition.y < scrollCheckUp.localPosition.y - 300)         // 숫자는 Sprite가 보여질 거리를 조정함
        {
            scrollUpBtn.SetActive(true);
        }
        else
        {
            scrollUpBtn.SetActive(false);
            scrollUpBtn.GetComponent<UISpriteAnimation>().ResetToBeginning();
        }

        // 스크롤 기준 포지션과 체크 포지션을 비교함 (Down)
        if (scrollBottomPos.localPosition.y > scrollCheckDown.localPosition.y + 300)    // 숫자는 Sprite가 보여질 거리를 조정함
        {
            scrollDownBtn.SetActive(true);
        }
        else
        {
            scrollDownBtn.SetActive(false);
            scrollDownBtn.GetComponent<UISpriteAnimation>().ResetToBeginning();
        }
    }

    /// <summary>
    /// 스크롤 체크 앵커를 변경합니다.
    /// </summary>
    /// <param name="isCategoryOne">카테고리1로 설정할 것인지</param>
    private void ChangeScrollCheckAnchor(bool setCategoryOne)
    {
        // 스크롤 버튼 리셋
        scrollDownBtn.SetActive(false);
        scrollUpBtn.SetActive(false);
        scrollUpBtn.GetComponent<UISpriteAnimation>().ResetToBeginning();
        scrollDownBtn.GetComponent<UISpriteAnimation>().ResetToBeginning();

        if (setCategoryOne)
        {
            // 스크롤 체크 앵커 카테고리1로 재설정
            scrollCheckUp.GetComponent<UIWidget>().SetAnchor(m_CategoryMenuDepthInfo[0].curDepthObject);
            scrollCheckDown.GetComponent<UIWidget>().SetAnchor(m_CategoryMenuDepthInfo[m_CategoryMenuDepthInfo.Length - 1].curDepthObject);
        }
        else
        {
            GameObject[] categoryTwoObj = HMCategoryInfoManager.Instance.categoryTwoObj;

            // 카테고리2 길이
            int categoryTwolength = HMCategoryInfoManager.Instance.categoryOneInfo[clickObjectIndex].categoryTwoInfo.Length;

            // 스크롤 체크 앵커 카테고리2로 재설정
            scrollCheckUp.GetComponent<UIWidget>().SetAnchor(categoryTwoObj[0]);
            scrollCheckDown.GetComponent<UIWidget>().SetAnchor(categoryTwoObj[categoryTwolength - 1]);
        }

        Invoke("ResetScrollUI", 0.5f);
    }


    #endregion
}