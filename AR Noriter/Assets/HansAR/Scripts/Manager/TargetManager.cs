using UnityEngine;
using System.Collections;
using Vuforia;
using System;
using System.IO;
using System.Collections.Generic;
using HedgehogTeam.EasyTouch;

using HansAR;

public class TargetManager : MonoBehaviour
{
    public delegate void TrackingMarkerFound(int markerIndex);
    public delegate void TrackingMarkerLost(int markerIndex);
    public delegate void TrackingReadyEvent();

    public enum CommonFunctionEnum
    {
        None,
        ChildrenSong,
        SimpleConversation
    }

    public static TrackingMarkerFound DelEventMarkerFound  = null;
    public static TrackingMarkerLost DelEventMarkerLost    = null;
    public static TrackingReadyEvent DelTrackingReadyEvent = null;

    public bool 첫인식상태 = false;    
    public GlobalDataManager.SceneState SceneMode;

    private CommonFunctionEnum commonFunctionMode;

    /// <summary>
    /// 증강된 상태인지 아닌지를 체크 합니다.
    /// </summary>
    public static bool trackableStatus = false;
    
    /// <summary>
    /// 발견된 마커 이름 입니다.
    /// </summary>
    public static string trackingFoundName = string.Empty;
    //public static string 비인식된마커이름 = string.Empty;

    /// <summary>
    /// 에셋번들을 읽어와 프리펩을 복제 합니다.
    /// </summary>
    public GameObject[] 에셋번들복제컨텐츠 = null;

    /// <summary>
    /// AR 카메라를 넣어 줍니다.
    /// </summary>
    public GameObject AR카메라 = null;

    /// <summary>
    /// 증강된 모델을 넣어 줍니다.
    /// </summary>
    public GameObject 컨텐츠최상위오브젝트 = null;

    /// <summary>
    /// 모델링 컨텐츠들을 담을 오브젝트
    /// </summary>
    public GameObject 모델링오브젝트 = null;

    /// <summary>
    /// 에셋번들 이름 xxx.hans
    /// </summary>
    public string 에센번들컨텐츠이름 = string.Empty;

    /// <summary>
    /// 에셋번들에 들어 있는 모델링 이름들..
    /// </summary>
    public string[] 컨텐츠모델링이름 = null;

    /// <summary>
    /// 마커를 잃을경우 표시될 좌표 오브젝트를 넣어 줍니다.
    /// </summary>
    public GameObject 비인식후_좌표오브젝트 = null;

    /// <summary>
    /// 마커 인식후 모델의 포지션을 변경 합니다.
    /// </summary>
    public Vector3 인식후_좌표값;

    /// <summary>
    /// 마커가 인식될 경우 회전될 오브젝트의 값을 넣어 줍니다. X, Y, Z
    /// </summary>
    public Vector3 인식후_회전값 = new Vector3(0, 0, 0);

    /// <summary>
    /// 마커를 인식 하였을때 모델 사이즈 입니다. (마커 사이즈가 100, 100, 100 일경우 1.0f입니다.)
    /// </summary>
    public Vector3 인식후_사이즈값 = new Vector3(1, 1, 1);

    /// <summary>
    /// 마커를 잃은후 모델의 포지션을 변경 합니다.
    /// </summary>
    public Vector3 비인식후_좌표값;

    /// <summary>
    /// 마커를 잃을 경우 회전될 오브젝트의 값을 넣어 줍니다. X, Y, Z
    /// </summary>
    public Vector3 비인식후_회전값 = new Vector3(0, 0, 0);

    /// <summary>
    /// 마커가 잃었을때 모델 사이즈 입니다.
    /// </summary>
    public Vector3 비인식후_사이즈값 = new Vector3(1, 1, 1);
    

    public enum MenuTitleList
    {
        None = 0,
        Observation,
        PaintPaper,
        PaintColor,
        GamePuzzle,
        GameMaze,
        GameMatching,
        GameWord,
        GameDriving,
        GameFishing,
        PostCardDeco,
        PostCardEvent
    }

    public MenuTitleList currentMenuTitle;

    public bool 공부하기사용 = false;
    public bool 질문답변사용 = false;
    public bool 듣고익히기사용 = false;
    public bool 슬라이드모드사용 = false;
    public bool 슬라이드초기화사용 = false;

    //public bool 동요사운드사용 = false;
    //public bool 지원공주미니맵사용 = false;

    public bool 오버레이사용 = false;

    public bool 스케치씬사용 = false;

    public bool UsedMiniMap = false;

    public bool usedMazeGame = false;

    public bool usedWordGame = false;

    public bool usedSelfiMode = false;

    public bool UsedMatchingGame = false;

    public bool UsedDriveEdu = false;

    public bool usedCrayonPaint = false;

    public bool usedKartGame = false;

    public bool usedPuzzle = false;

    public bool usedFishing = false;

    /// <summary>
    /// 슬라이드 버튼 숨김 여부
    /// </summary>
    public bool 슬라이드사용 = true;

    //public bool 공통간단회화사용 = false;


    public List<int> 모델컨텐츠저장 = new List<int>();

    /// <summary>
    /// 성대혁 : 슬라이드나 탐색 사용시 출력할 배열의 번호를 가져오기 위한 값입니다.
    /// </summary>
    public int 하위인덱스배열번호 = 0;

    public int 타깃정보인덱스 = -1;
    public int 복제모델링인덱스 = 0;

    [HideInInspector]
    public bool postcardEnd = false;

    private float 딜레이 = 2.0f;
    private bool 딜레이멈춤체크 = false;

    private int preModelingIndex = -1;
    private bool m_DragSettingCheck = false;

    [Serializable]
    public class 타깃설정
    {
        /// <summary>
        /// 이미지 마커를 넣어 줍니다. (Unity3d Editor)
        /// </summary>
        public GameObject 마커타깃오브젝트 = null;

        /// <summary>
        /// 가지고있는 원본텍스쳐를 넣어 줍니다.
        /// </summary>
        [HideInInspector]
        public Texture2D 원본텍스쳐 = null;
        
        /// 탐색모드를 사용할지 여부
        /// </summary>
        public bool 탐색여부 = false;

        /// <summary>
        /// 슬라이드를 숨길지 사용할지 여부
        /// </summary>
        public bool 슬라이드숨김여부 = false;

        /// <summary>
        /// 증강된 모델을 넣어 줍니다. (Unity3d Editor)
        /// </summary>
        public int[] 증강될컨텐츠번호 = null;

        /// <summary>
        /// 증강될 컨텐츠 이름으로 오브젝트를 찾아 넣어줍니다.
        /// </summary>
        //public GameObject[] 컨텐츠모델링 = null;

        /// <summary>
        /// 동요, 동화 증강현실을 사용할지 여부
        /// </summary>
        public bool 동요동화_사용여부 = false;

        /// <summary>
        /// 동요, 동화 사이트 주소 링크
        /// </summary>
        public string 동요동화_주소링크 = "";
    }

    [SerializeField]
    public 타깃설정[] 타깃정보;

    public ObserveManager observeManager;

    private bool shaderTweenCheck = false;

    public static TargetManager 타깃메니저;

    private Coroutine SetARCamCoroutine = null;

    void Awake()
    {
        Screen.orientation                      = ScreenOrientation.LandscapeLeft;
        Screen.autorotateToPortrait             = false;        
        Screen.autorotateToLandscapeLeft        = true;
        Screen.autorotateToLandscapeRight       = false;
        Screen.autorotateToPortraitUpsideDown   = false;

        타깃메니저                               = this;


        마커이벤트설정초기화();
    }
   
    private void RequestLoadAssetBunle()
    {
        HttpRequestDataSet assetBundleDataSet       = null;

        assetBundleDataSet                          = new HttpRequestDataSet();
        에셋번들복제컨텐츠                           = new GameObject[컨텐츠모델링이름.Length];

        GlobalDataManager.m_SelectedAssetBundleName = string.Format("{0}_{1}", GlobalDataManager.m_ResourceFolderEnum.ToString().ToLower(),
                                                                               GlobalDataManager.m_AssetBundlePartName.ToLower());

        AssetBundleLoader.getInstance.SetAssetBundleLoadInfo(assetBundleDataSet,
                                                             에센번들컨텐츠이름,
                                                             null,
                                                             AssetBundleLoader.getInstance.OnLoadCompleteModeling,
                                                             AfterLoadComplete,
                                                             ApplySceneUI,
                                                             null);

        AssetBundleLoader.getInstance.SetStorageLoadObject(assetBundleDataSet,
                                                           컨텐츠모델링이름,
                                                           에셋번들복제컨텐츠,
                                                           모델링오브젝트,
                                                           AR카메라);

        AssetBundleLoader.getInstance.StartLoadAssetBundle(assetBundleDataSet);
    }

    public void AfterLoadComplete(HttpRequestDataSet assetBundleDataSet)
    {
        Debug.LogWarningFormat("AfterLoadComplete Function !!! === > File Title : {0}", assetBundleDataSet.requestFileTitle);
    }

    public void ApplySceneUI(HttpRequestDataSet requestDataSet)
    {
        StartCoroutine(MainUI.메인.CloseLoadingPopUp());      
    }

    void Start()
    {
        SceneMode = GlobalDataManager.m_SelectedSceneStateEnum;
        // asset bundle load function
        //RequestLoadAssetBunle();        
        CheckOffVuforiaMode();
    }

    void FixedUpdate()
    {
        if (string.IsNullOrEmpty(에센번들컨텐츠이름) && 딜레이멈춤체크 == false)
        {
            딜레이 -= Time.fixedDeltaTime;
            if (딜레이 <= 0)
            {
                수동실행();
                Debug.Log("수동");
            }
        }
    }

    public static void 마커이벤트설정초기화()
    {
        // 씬로딩시 이미지 마커 이벤트 설정 초기화
        trackableStatus     = false;
        trackingFoundName   = string.Empty;       
    }

    private void 수동실행()
    {
        MainUI.메인.딜레이팝업UI.SetActive(false);

        MainUI.메인.인식글자UI.SetActive(true);
        AR카메라.GetComponent<VuforiaBehaviour>().enabled = true;
        딜레이멈춤체크 = true;
    }

    #region 에셋번들 로드 함수

    private void ConfigureAssetBundleName(string assetBundleName)
    {
        // Common 번들에 대한 ResourceFolderEnum, AssetBundlePartName 변수 값 구성 부분     
        if (commonFunctionMode == CommonFunctionEnum.ChildrenSong)
        {
            GlobalDataManager.m_ResourceFolderEnum = GlobalDataManager.CategoryType.Common;

            if (GlobalDataManager.m_SelectedCategoryEnum == GlobalDataManager.CategoryType.None)
            {
                GlobalDataManager.m_AssetBundlePartName = "dongyo";
            }
        }
        else if (commonFunctionMode == CommonFunctionEnum.SimpleConversation)
        {
            GlobalDataManager.m_ResourceFolderEnum = GlobalDataManager.CategoryType.Common;

            if (GlobalDataManager.m_SelectedCategoryEnum == GlobalDataManager.CategoryType.None)
            {
                GlobalDataManager.m_AssetBundlePartName = "conversation";
            }
        }
        else
        {
            if (GlobalDataManager.m_SelectedCategoryEnum == GlobalDataManager.CategoryType.None)
            {
                GlobalDataManager.SetProductAndSceneName(에센번들컨텐츠이름, ".hans");
            }

            GlobalDataManager.m_ResourceFolderEnum = GlobalDataManager.m_SelectedCategoryEnum;
        }
    }

    private void LoadAssetBundleFromFile()
    {
        if (string.IsNullOrEmpty(에센번들컨텐츠이름))
        {
            Debug.LogWarning("에셋번들 파일 이름이 없습니다.");
            return;
        }

        // Assetbundle 파일명을 Common, Product 로 구성하며, Product 값이 없는 경우 Inspector에 등록 된 '에셋번들컨텐츠이름' 변수 값을 사용하여 에셋번들 파일명을 완성한다.
        ConfigureAssetBundleName(에센번들컨텐츠이름);
        
        GlobalDataManager.m_SelectedAssetBundleName = string.Format("{0}_{1}", GlobalDataManager.m_ResourceFolderEnum.ToString().ToLower(),
                                                                               GlobalDataManager.m_AssetBundlePartName.ToLower());

        에셋번들복제컨텐츠 = new GameObject[컨텐츠모델링이름.Length];
    }

    /// <summary>
    /// 트레커 제어용 프로퍼티
    /// </summary>
    public static bool EnableTracking
    {
        get
        {
            return TrackerManager.Instance.GetTracker<ObjectTracker>().IsActive;
        }
        set
        {
            if (value)
            {
                TrackerManager.Instance.GetTracker<ObjectTracker>().Start();
                AutoFocusMode.getInstance.OnOffAutoFucousMode(true);
            }
            else
            {
                TrackerManager.Instance.GetTracker<ObjectTracker>().Stop();
                AutoFocusMode.getInstance.OnOffAutoFucousMode(false);
            }
        }
    }


    #endregion


    #region 마커 인식 이벤트
    /*
    private void ApplyRecognitionUI()
    {
        switch (currentMenuTitle)
        {
            case MenuTitleList.Observation:
                break;
            case MenuTitleList.PaintPaper:
                break;
            case MenuTitleList.PaintColor:
                break;
            case MenuTitleList.GamePuzzle:
                break;
            case MenuTitleList.GameMaze:
                break;
            case MenuTitleList.GameMatching:
                break;
            case MenuTitleList.GameWord:
                break;
            case MenuTitleList.GameDriving:
                break;
            case MenuTitleList.GameFishing:
                break;
            case MenuTitleList.PostCardDeco:
                break;
            case MenuTitleList.PostCardEvent:
                break;
            default:                
                break;
        }        
    }
    */

    /// <summary>
    /// 타겟이 인식 되었을 때 전처리가 필요 한 경우 작성 합니다.
    /// </summary>
    /// <returns>인식 처리를 진행 하는 경우 true, 그러지 않는 경우 false 를 반환</returns>
    public bool PreMarkerFound(string trackableTargetName)
    {
        첫인식상태 = true;

        if (!QuizQuizSceneModeCheck(true))
        {
            return false;
        }

        if (usedPuzzle)
        {
            string markerNameOfList = string.Empty;

            for (int i = 0; i < 타깃정보.Length; i++)
            {
                markerNameOfList = 타깃정보[i].마커타깃오브젝트.GetComponent<Vuforia.ImageTargetBehaviour>().TrackableName;

                // 뷰포리아에 등록된 마커 이름이 같다면..
                if (string.Compare(trackableTargetName, markerNameOfList, true) == 0)
                {
                    NrPuzzleManager.instance.Cognitive(타깃정보[i].증강될컨텐츠번호[0]);
                }
            }
            return false;
        }

        if(usedFishing)
        {
            FishingGameManager.instance.Cognitive(0);
            return false;
        }

        if (usedSelfiMode)
        {
            if(LetterManager.Instance.pikMode)
            {
                return false;
            }

            LetterManager.Instance.OnOffOpenedLetterUI(false);

            if(!스케치씬사용)
            {
                for (int i = 0; i < 타깃정보.Length; i++)
                {

                   string trackableName = 타깃정보[i].마커타깃오브젝트.GetComponent<ImageTargetBehaviour>().TrackableName;
                    
                    if (string.Compare(trackableTargetName, trackableName, true) == 0)
                    {
                        trackingFoundName = trackableTargetName;

                        컨텐츠최상위오브젝트.transform.parent = 타깃정보[i].마커타깃오브젝트.transform;
                        LetterNomodelManager.instance.MarkerFound(타깃정보[i].증강될컨텐츠번호[0]);
                        break;
                    }
                }
                return false; 
            }
        }

        if (UsedMatchingGame)
        {
            MatchingManager.instance.Cognitive(0);
            return false;
        }

        if(usedMazeGame)
        {
            MazeSelectUI.instance.MazeSelectStart(trackableTargetName);
            return false;
        }

        if (usedWordGame)
        {
            WordUI.instance.WordGameStart(trackableTargetName);
            return false;
        }

        trackingFoundName = trackableTargetName;
        return true;

    }

    public void MarkerEventFound()
    {
        //RotateUI.회전.컨텐츠_회전_중지();
        //if (!QuizQuizSceneModeCheck(true))
        //{
        //    return;
        //}

        HideAllModelingContents();
        MainUI.메인.인식글자UI.SetActive(false);

        // 회전UI 상태를 변경 하는 구간 시작       
        //RotateUI.회전.회전UI_보이기();        
        //-- 회전UI 상태를 변경 하는 구간 끝

        int searchFindIndex     = -1;
        string markerNameOfList = string.Empty;        

        for (int i = 0; i < 타깃정보.Length; i++)
        {
            markerNameOfList = 타깃정보[i].마커타깃오브젝트.GetComponent<ImageTargetBehaviour>().TrackableName;

            // 뷰포리아에 등록된 마커 이름이 같다면..
            if (string.Compare(trackingFoundName, markerNameOfList, true) == 0)
            {
                searchFindIndex = i;
                break;              
            }            
        }
        
        if (searchFindIndex != -1)
        {
            타깃정보인덱스 = searchFindIndex;

            if (DelEventMarkerFound != null)
            {
                SetTrackingFoundModeling(searchFindIndex);
                DelEventMarkerFound(searchFindIndex);
            }
        }     
    }

    private void SetTrackingFoundModeling(int targetIndex)
    {
        첫인식상태 = true;
        컨텐츠최상위오브젝트.transform.parent = 타깃정보[targetIndex].마커타깃오브젝트.transform;

        // 모델 위치 변경
        ChangeModelingPosition(인식후_좌표값, 인식후_사이즈값, 인식후_회전값);

        // 탐색 및 슬라이드 모드일시 무조건 0번만 출력 되게 설정되어있습니다.
        ShowCopyContents();

        // 모델의 위치, 회전, 크기 값을 '인식후' 값으로 초기화 합니다.
        RotateUI.회전.컨텐츠_회전_초기화();

        if (슬라이드사용)
        {
            슬라이드모델링저장();            
            MainUI.메인.OnOffOverlayUI(true);           
        }        
    }
 
    #endregion


    #region 마커 비인식 이벤트
    public bool PreMarkerLost()
    {
        if (SceneMode != GlobalDataManager.SceneState.NONE)
        {
            return false;
        }

        return true;
    }

    public void MarkerEventLost()
    {
        int searchFindIndex     = -1;
        string markerNameOfList = string.Empty;
        Debug.Log("trackingFoundName : " + trackingFoundName);

        if(postcardEnd)
        {
            return;
        }

        for (int i = 0; i < 타깃정보.Length; i++)
        {
            markerNameOfList = 타깃정보[i].마커타깃오브젝트.GetComponent<ImageTargetBehaviour>().TrackableName;
            //Debug.Log("markerNameOfList : " + markerNameOfList);
            // 마커 인식이 0번 보다 크고, 뷰포리아에 등록된 마커 이름이 같다면..
            if (string.Compare(trackingFoundName, markerNameOfList, true) == 0)
            {
                HideAllModelingContents();
                //MainUI.메인.탐색UI.SetActive(false);

                searchFindIndex = i;
                break;
            }
        }

        if (searchFindIndex != -1)
        {
            타깃정보인덱스 = searchFindIndex;

            if (DelEventMarkerLost != null)
            {
                if(usedSelfiMode)
                {
                    if(LetterManager.Instance.pikMode)
                    {
                        return;
                    }
                }
                SetTrackingLostModeling(searchFindIndex);
                DelEventMarkerLost(searchFindIndex);
            }                    
        }    
    }

    private void SetTrackingLostModeling(int targetIndex)
    {
        컨텐츠최상위오브젝트.transform.parent   = 비인식후_좌표오브젝트.transform;
        AR카메라.transform.eulerAngles         = new Vector3(0, 0, 0);

        ChangeModelingPosition(비인식후_좌표값, 비인식후_사이즈값, 비인식후_회전값);

        //ChaSpawnTween(에셋번들복제컨텐츠[복제모델링인덱스]);
        에셋번들복제컨텐츠[복제모델링인덱스].SetActive(true);

        // 마커를 잃었을때 회전방향에 맞게 초기화
        RotateUI.회전.컨텐츠_회전_초기화();
    }


    public void 마커비인식호출(int index)
    {
        int trackingFoundIndex = 타깃메니저.복제모델링인덱스;

        if (스케치씬사용)
        {
            ColoringManager.컬러링매니저.savedTargetIndex = index;
            TouchEventManager.터치.기준콜라이더             = 에셋번들복제컨텐츠[trackingFoundIndex].GetComponent<ColoringInfo>().색칠하기속성.콜라이더;
        }

        SetTrackingLostModeling(index);
    }


    #endregion


    #region 슬라이드 관련 함수


    /// <summary>
    /// 슬라이드시 모델링 인덱스 값을 저장 합니다.
    /// </summary>
    public void 슬라이드모델링저장()
    {
        if (모델컨텐츠저장.IndexOf(복제모델링인덱스) == -1)
        {
            모델컨텐츠저장.Add(복제모델링인덱스);
        }
    }


    #endregion


    #region 복제컨텐츠 관련 함수들


    /// <summary>
    /// 모든 모델링을 숨깁니다.
    /// </summary>
    public void HideAllModelingContents()
    {
        #region MyRegion             
        for (int i = 0; i < 에셋번들복제컨텐츠.Length; i++)
        {
            // null 체크
            if (에셋번들복제컨텐츠[i] != null)
            {
                에셋번들복제컨텐츠[i].SetActive(false);
            }
        }


        #endregion
    }

    private void ShowCopyContents()
    {        
        복제모델링인덱스 = 타깃정보[타깃정보인덱스].증강될컨텐츠번호[0]; 

        if (!스케치씬사용)
        {
            //ChaSpawnTween(에셋번들복제컨텐츠[복제모델링인덱스]);
            에셋번들복제컨텐츠[복제모델링인덱스].SetActive(true);
        }
    }

    public void ShowModelingObj()
    {
        에셋번들복제컨텐츠[복제모델링인덱스].SetActive(true);
    }

    /// <summary>
    /// 탐색에서 참조하여 사용함
    /// </summary>
    /// <param name="하위인덱스"></param>
    public void 복제컨텐츠_바로보이기(int 하위인덱스)
    {
        복제모델링인덱스 = 하위인덱스;
        에셋번들복제컨텐츠[복제모델링인덱스].SetActive(true);

    }

    /// <summary>
    /// 탐색에서 참조하여 사용함
    /// </summary>
    /// <param name="하위인덱스"></param>
    public void ShowCopyContents(int 하위인덱스)
    {
        복제모델링인덱스 = 타깃정보[타깃정보인덱스].증강될컨텐츠번호[하위인덱스];
        에셋번들복제컨텐츠[복제모델링인덱스].SetActive(true);    

    }

    /// <summary>
    /// 탐색에서 참조하여 사용함
    /// </summary>
    /// <param name="슬라이드인덱스"></param>
    public void 복제컨텐츠인덱스_저장(int 슬라이드인덱스)
    {
        하위인덱스배열번호 = 슬라이드인덱스;
        복제모델링인덱스 = 타깃정보[타깃정보인덱스].증강될컨텐츠번호[슬라이드인덱스];

        

    }

    public void 슬라이드컨텐츠_보이기(int 인덱스)
    {
        복제모델링인덱스 = 인덱스;
        에셋번들복제컨텐츠[복제모델링인덱스].SetActive(true);        
    }

    /// <summary>
    /// 현재 복제모델링 반환
    /// </summary>
    public GameObject GetCurrentCopyModel()
    {
        return 에셋번들복제컨텐츠[복제모델링인덱스];
    }

    #endregion


    #region 컨텐츠오브젝트 위치 관련 함수들


    /// <summary>
    /// 모델링 위치를 변경합니다. (좌표값, 사이즈값, 회전값 입력)
    /// </summary>
    private void ChangeModelingPosition(Vector3 positionValue, Vector3 sizeValue, Vector3 rotateValue)
    {
        // write by N.C.Park
        컨텐츠최상위오브젝트.transform.localPosition = new Vector3(positionValue.x, positionValue.y, positionValue.z);        

        // 모델 크기 변경
        컨텐츠최상위오브젝트.transform.localScale = new Vector3(sizeValue.x, sizeValue.y, sizeValue.z);

        // 모델 회전 변경
        컨텐츠최상위오브젝트.transform.localEulerAngles = new Vector3(rotateValue.x, rotateValue.y, rotateValue.z);
    }

    public void 컨텐츠오브젝트_위치(bool awareness)
    {
        if (awareness)
        {
            컨텐츠오브젝트_인식위치();
        }
        else
        {
            컨텐츠오브젝트_비인식위치();
        }
    }

    private void 컨텐츠오브젝트_인식위치()
    {
        // 모델 위치 변경        
        컨텐츠최상위오브젝트.transform.localPosition = new Vector3(0, 0, 0);

        // 모델 크기 변경
        컨텐츠최상위오브젝트.transform.localScale = new Vector3(인식후_사이즈값.x, 인식후_사이즈값.y, 인식후_사이즈값.z);

        // 모델 회전 변경
        컨텐츠최상위오브젝트.transform.localEulerAngles = new Vector3(인식후_회전값.x, 인식후_회전값.y, 인식후_회전값.z);

        if (LetterManager.Instance != null)
        {
            LetterManager.Instance.SetLetterTextScale(컨텐츠최상위오브젝트, TargetManager.타깃메니저.인식후_사이즈값);
        }
    }

    private void 컨텐츠오브젝트_비인식위치()
    {
        컨텐츠최상위오브젝트.transform.parent = 비인식후_좌표오브젝트.transform;
        AR카메라.transform.eulerAngles = new Vector3(0, 0, 0);

        // 모델 회전 변경
        컨텐츠최상위오브젝트.transform.localEulerAngles = new Vector3(비인식후_회전값.x, 비인식후_회전값.y, 비인식후_회전값.z);

        // 모델 위치 변경
        컨텐츠최상위오브젝트.transform.localPosition = new Vector3(비인식후_좌표값.x, 비인식후_좌표값.y, 비인식후_좌표값.z);

        // 모델 크기 변경
        컨텐츠최상위오브젝트.transform.localScale = new Vector3(비인식후_사이즈값.x, 비인식후_사이즈값.y, 비인식후_사이즈값.z);

        if (LetterManager.Instance != null)
        {
            LetterManager.Instance.SetLetterTextScale(컨텐츠최상위오브젝트, TargetManager.타깃메니저.인식후_사이즈값);
        }
    }


    #endregion



    #region QUIZ 관련 함수들

    /// <summary>
    /// quiz 종류의 씬일 경우(awareness : 인식(true) 비인식(false))
    /// </summary>
    /// <param name="awareness"></param>
    /// <returns></returns>
    public bool QuizQuizSceneModeCheck(bool awareness)
    {
        if (SceneMode != GlobalDataManager.SceneState.NONE)
        {
            Debug.Log("드오니");

            if (RotateUI.회전.회전UI.activeSelf)
            {
                if (awareness)
                {
                    Debug.Log("인식");
                    컨텐츠최상위오브젝트.transform.parent = 타깃정보[타깃정보인덱스].마커타깃오브젝트.transform;
                }
                else
                {
                    Debug.Log("비인식");
                    컨텐츠최상위오브젝트.transform.parent = 비인식후_좌표오브젝트.transform;
                }

                RotateUI.회전.컨텐츠_회전_초기화();
            }

            // 인식 글자가 없으면 Map, Sticker, Puzzle, QuizQuiz UI 를 화면에 띄우지 못하도록 함
            if (!MainUI.메인.인식글자UI.activeSelf)
            {
                return false;
            }

            if (SceneMode != GlobalDataManager.SceneState.MAP)
            {
                // 마커를 확인하여 Quiz or SPuzzle, Sticker 이면 해당 씬 상태 값으로 변경 합니다.
                string trackableName = string.Empty;
                string targetObjectName = string.Empty;

                // 인식된 타겟 이름으로 타깃정보에서 타겟 이름을 찾아 비교 합니다.
                for (int i = 0; i < 타깃정보.Length; i++)
                {
                    trackableName = 타깃정보[i].마커타깃오브젝트.GetComponent<ImageTargetBehaviour>().TrackableName;

                    if (string.Compare(trackingFoundName, trackableName, true) == 0)
                    {
                        targetObjectName = 타깃정보[i].마커타깃오브젝트.name;
                        break;
                    }
                }

                // 찾은 타겟 이름으로 컨텐츠 종류를 구분 하기 위해 SceneMode 값을 대입 합니다.
                if (!string.IsNullOrEmpty(targetObjectName))
                {
                    if (targetObjectName.ToLower().Contains("quiz"))
                    {
                        SceneMode = GlobalDataManager.SceneState.QUIZ_QUIZ;
                    }
                    else if (targetObjectName.ToLower().Contains("puzzle"))
                    {
                        SceneMode = GlobalDataManager.SceneState.PUZZLE;
                    }
                    else if (targetObjectName.ToLower().Contains("sticker"))
                    {
                        SceneMode = GlobalDataManager.SceneState.STICKER;
                    }
                }
            }

            switch (SceneMode)
            {
                case GlobalDataManager.SceneState.DRAG_AND_DROP:
                    Debug.Log("DRAG_AND_DROP 인식, 비인식 실행 부분");
                    break;

                case GlobalDataManager.SceneState.MAP:
                    MapModeNumberSetting();
                    Debug.Log("MAP 인식, 비인식 실행 부분");
                    break;

                case GlobalDataManager.SceneState.PUZZLE:
                    SketchPuzzleModeNumberSetting();
                    Debug.Log("PUZZLE 인식, 비인식 실행 부분");
                    break;

                case GlobalDataManager.SceneState.QUIZ_QUIZ:
                    Debug.Log("QUIZ_QUIZ 인식, 비인식 실행 부분");
                    /*
                    if (GlobalDataManager.CategoryType.Princess == GlobalDataManager.m_SelectedCategoryEnum)
                    {
                        PrincessManager.instance.QuestionInit();
                        return true;
                    }
                    else
                    {
                        QuizQuizMode(awareness);
                    }
                    */
                    QuizQuizMode(awareness);
                    break;

                case GlobalDataManager.SceneState.STICKER:
                    StickerModeNumberSetting();
                    Debug.Log("STICKER 인식, 비인식 실행 부분");
                    break;
            }

            if (m_DragSettingCheck)
            {
                SketchBookUI.getInstance.Ui_On(SceneMode);
                MainUI.메인.인식글자UI.SetActive(false);
            }

            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// 인식시 quiz 상태 변경 
    /// </summary>
    /// <param name="awareness"></param>
    public void QuizQuizMode(bool awareness)
    {
        QuizNumberSetting();

        if (QuizQuizUI.getInstance.m_QuizMode != QuizQuizUI.QuizMode.NONE &&
            QuizQuizManager.getInstance.m_QuizNum != SketchBookTargetInfo.QuizNum.NONE)
        {
            if (QuizQuizManager.getInstance.m_QuizState != QuizQuizManager.QuizState.END)
            {
                if (QuizQuizManager.getInstance.m_3Dset)
                {
                    QuizTextInitSetting(awareness);
                }

                if (awareness)
                {
                    HideAllModelingContents();
                    QuizQuizManager.getInstance.QuizTextTween();
                    QuizQuizManager.getInstance.m_QuizState = QuizQuizManager.QuizState.PLAYING;
                }
            }
            else
            {
                if (QuizQuizManager.getInstance.m_3Dset)
                {
                    컨텐츠오브젝트_위치(awareness);
                }

                if (!awareness)
                {
                    QuizQuizManager.getInstance.m_QuizState = QuizQuizManager.QuizState.READY;
                }
            }
        }
    }

    /// <summary>
    /// quiz 모드에서 인식시 
    /// </summary>
    private void QuizNumberSetting()
    {
        for (int i = 0; i < 타깃정보.Length; i++)
        {
            string 등록된마커이름 = 타깃정보[i].마커타깃오브젝트.GetComponent<ImageTargetBehaviour>().TrackableName;

            if (string.Compare(trackingFoundName, 등록된마커이름, true) == 0)
            {
                if (타깃정보[i].마커타깃오브젝트.GetComponent<SketchBookTargetInfo>().m_QuizMarkerState == SketchBookTargetInfo.QuizNum.NONE)
                {
                    m_DragSettingCheck = false;
                    return;
                }

                m_DragSettingCheck = true;
                QuizQuizManager.getInstance.m_QuizNum = 타깃정보[i].마커타깃오브젝트.GetComponent<SketchBookTargetInfo>().m_QuizMarkerState;
                컨텐츠최상위오브젝트.transform.parent = 타깃정보[i].마커타깃오브젝트.transform;
                return;
            }
            else
            {
                QuizQuizManager.getInstance.m_QuizNum = SketchBookTargetInfo.QuizNum.NONE;
            }
        }
    }

    #endregion


    #region 스티커 모드 함수


    private void StickerModeNumberSetting()
    {
        for (int i = 0; i < 타깃정보.Length; i++)
        {
            string 등록된마커이름 = 타깃정보[i].마커타깃오브젝트.GetComponent<ImageTargetBehaviour>().TrackableName;

            if (string.Compare(trackingFoundName, 등록된마커이름, true) == 0)
            {
                if (타깃정보[i].마커타깃오브젝트.GetComponent<SketchBookTargetInfo>().m_StickerMarkerState == SketchBookTargetInfo.QuizNum.NONE)
                {
                    m_DragSettingCheck = false;
                    return;
                }
                m_DragSettingCheck = true;
                SketchBookUI.getInstance.DragManager.GetComponent<StickerManager>().m_QuestionNum = 타깃정보[i].마커타깃오브젝트.GetComponent<SketchBookTargetInfo>().m_StickerMarkerState;
                컨텐츠최상위오브젝트.transform.parent = 타깃정보[i].마커타깃오브젝트.transform;
                return;
            }
            else
            {
                SketchBookUI.getInstance.DragManager.GetComponent<StickerManager>().m_QuestionNum = SketchBookTargetInfo.QuizNum.NONE;
            }
        }
    }


    #endregion


    #region MAP 모드 함수


    private void MapModeNumberSetting()
    {
        try
        {
            for (int i = 0; i < 타깃정보.Length; i++)
            {
                string 등록된마커이름 = 타깃정보[i].마커타깃오브젝트.GetComponent<ImageTargetBehaviour>().TrackableName;

                if (string.Compare(trackingFoundName, 등록된마커이름, true) == 0)
                {
                    if (타깃정보[i].마커타깃오브젝트.GetComponent<SketchBookTargetInfo>().m_MapMarkerState == SketchBookTargetInfo.QuizNum.NONE)
                    {
                        m_DragSettingCheck = false;
                        return;
                    }
                    m_DragSettingCheck = true;
                    SketchBookUI.getInstance.DragManager.GetComponent<StickerManager>().m_QuestionNum = 타깃정보[i].마커타깃오브젝트.GetComponent<SketchBookTargetInfo>().m_MapMarkerState;
                    컨텐츠최상위오브젝트.transform.parent = 타깃정보[i].마커타깃오브젝트.transform;
                    return;
                }
                else
                {
                    SketchBookUI.getInstance.DragManager.GetComponent<StickerManager>().m_QuestionNum = SketchBookTargetInfo.QuizNum.NONE;
                }
            }
        }
        catch
        {

        }
    }


    #endregion


    #region 퍼즐 모드 함수


    private void SketchPuzzleModeNumberSetting()
    {
        for (int i = 0; i < 타깃정보.Length; i++)
        {
            string 등록된마커이름 = 타깃정보[i].마커타깃오브젝트.GetComponent<ImageTargetBehaviour>().TrackableName;

            if (string.Compare(trackingFoundName, 등록된마커이름, true) == 0)
            {

                if (타깃정보[i].마커타깃오브젝트.GetComponent<SketchBookTargetInfo>().m_SketchPuzzleMarkerState == SketchBookTargetInfo.QuizNum.NONE)
                {
                    m_DragSettingCheck = false;
                    return;
                }
                m_DragSettingCheck = true;
                SketchBookUI.getInstance.DragManager.GetComponent<StickerManager>().m_QuestionNum = 타깃정보[i].마커타깃오브젝트.GetComponent<SketchBookTargetInfo>().m_SketchPuzzleMarkerState;
                컨텐츠최상위오브젝트.transform.parent = 타깃정보[i].마커타깃오브젝트.transform;
                return;
            }
            else
            {
                SketchBookUI.getInstance.DragManager.GetComponent<StickerManager>().m_QuestionNum = SketchBookTargetInfo.QuizNum.NONE;
            }
        }
    }


    #endregion


    #region QUIZ 모드 함수


    /// <summary>
    /// 인식 비인식 컨텐츠오브젝트 설정 변경
    /// </summary>
    /// <param name="awareness"></param>
    private void QuizTextInitSetting(bool awareness)
    {
        if (awareness)
        {
            SketchBookUI.getInstance.quizUI.transform.parent = 컨텐츠최상위오브젝트.transform;

            SketchBookUI.getInstance.quizUI.transform.localPosition = new Vector3(0, 0, 0);
            SketchBookUI.getInstance.quizUI.transform.localEulerAngles = new Vector3(0, 0, 0);
            SketchBookUI.getInstance.quizUI.transform.localScale = new Vector3(1f, 1f, 1f);

            컨텐츠최상위오브젝트.transform.localPosition = new Vector3(0, 0, 0);
            컨텐츠최상위오브젝트.transform.localEulerAngles = new Vector3(90, 0, 0);
            컨텐츠최상위오브젝트.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);

        }
        else
        {
            컨텐츠최상위오브젝트.transform.parent = 비인식후_좌표오브젝트.transform;

            SketchBookUI.getInstance.quizUI.transform.localPosition = new Vector3(0, 0, 200);
            SketchBookUI.getInstance.quizUI.transform.localEulerAngles = new Vector3(0, 0, 0);
            SketchBookUI.getInstance.quizUI.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

            컨텐츠최상위오브젝트.transform.localPosition = new Vector3(0, 0, 0);
            컨텐츠최상위오브젝트.transform.localEulerAngles = new Vector3(0, 0, 0);
            컨텐츠최상위오브젝트.transform.localScale = new Vector3(1, 1, 1);
        }
    }


    #endregion


    #region 이펙트, 더빙 관련 함수


    public void OnPlayPreviewClick()
    {
        if (EffectSoundManager.이펙트.이펙트사운드.GetComponent<AudioSource>().clip != null)
        {
            EffectSoundManager.이펙트.이펙트사운드.GetComponent<AudioSource>().mute = true;
        }

        if (DubbingManager.더빙.더빙사운드.GetComponent<AudioSource>().clip != null)
        {
            DubbingManager.더빙.더빙사운드.GetComponent<AudioSource>().mute = true;
        }
    }

    public void OnClosePlayPreviewClick()
    {
        try
        {
            if (EffectSoundManager.이펙트.이펙트사운드.GetComponent<AudioSource>().clip != null)
            {
                EffectSoundManager.이펙트.이펙트사운드.GetComponent<AudioSource>().mute = false;
            }

            if (DubbingManager.더빙.더빙사운드.GetComponent<AudioSource>().clip != null)
            {
                DubbingManager.더빙.더빙사운드.GetComponent<AudioSource>().mute = false;
            }
        }
        catch (Exception ex)
        {
            Debug.Log(string.Format("Error Message : {0}, Function Name : OnClosePlayPreviewClick", ex.Message));
        }
    }


    #endregion

    public void ChaSpawnTween(GameObject obj)
    {
        string shaderName = "CustomShader/MaskShader";

        Transform[] objList = obj.transform.GetComponentsInChildren<Transform>();
        Material eMat;
        foreach (Transform child in objList)
        {
            if (child.GetComponent<Renderer>() != null)
            {
                if (child.GetComponent<Renderer>().materials.Length > 1)
                {
                    for (int i = 0; i < child.GetComponent<Renderer>().materials.Length; i++)
                    {
                        eMat = child.GetComponent<Renderer>().materials[i];

                        if (eMat.shader.name != shaderName)
                        {
                            eMat.shader = Shader.Find(shaderName);
                        }

                        eMat.color = Color.white;
                        StartCoroutine(ShaderTween(eMat, 0, 1));
                    }
                }
                else
                {
                    eMat = child.GetComponent<Renderer>().material;

                    if (eMat.shader.name != shaderName)
                    {
                        eMat.shader = Shader.Find(shaderName);
                    }

                    eMat.color = Color.white;
                    StartCoroutine(ShaderTween(eMat, 0, 1));
                }
                // eMat.SetFloat("_Mode", 2);
                // eMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                // eMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                // eMat.SetInt("_ZWrite", 0);
                // eMat.DisableKeyword("_ALPHATEST_ON");
                // eMat.EnableKeyword("_ALPHABLEND_ON");
                // eMat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                // eMat.renderQueue = 3000;

                

                //TweenManager.tween_Manager.TweenAllDestroy(child.gameObject);
                //TweenManager.tween_Manager.AddTweenAlpha(child.gameObject, 0, 1, 1.0f);

                //  Debug.Log("ed.methodName : " + ed.methodName);
                //  Debug.Log("param : " + param);

                //  TweenAlpha ta = child.gameObject.GetComponent<TweenAlpha>();
                // ta.RemoveOnFinished(ed);

                //param.value = child.gameObject;
                //ed.parameters[0] = param;

                // ta.onFinished.Add(ed);

                // TweenManager.tween_Manager.TweenAlpha(child.gameObject);
            }
        }
    }


    public void ChaDespawnTween(GameObject obj)
    {
        string shaderName = "CustomShader/MaskShader";

        Transform[] objList = obj.transform.GetComponentsInChildren<Transform>();
        Material eMat;
        foreach (Transform child in objList)
        {

            if (child.GetComponent<Renderer>() != null)
            {
                for (int i = 0; i < child.GetComponent<Renderer>().materials.Length; i++)
                {
                    eMat = child.GetComponent<Renderer>().materials[i];

                    if (eMat.shader.name != shaderName)
                    {
                        eMat.shader = Shader.Find(shaderName);
                    }

                    eMat.color = Color.white;
                }
                    // eMat.SetFloat("_Mode", 2);
                    // eMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    // eMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    // eMat.SetInt("_ZWrite", 0);
                    // eMat.DisableKeyword("_ALPHATEST_ON");
                    // eMat.EnableKeyword("_ALPHABLEND_ON");
                    // eMat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    // eMat.renderQueue = 3000;

                    TweenManager.tween_Manager.TweenAllDestroy(child.gameObject);
                    TweenManager.tween_Manager.AddTweenAlpha(child.gameObject, 1, 0, 1.0f);

                    //  Debug.Log("ed.methodName : " + ed.methodName);
                    //  Debug.Log("param : " + param);

                    //  TweenAlpha ta = child.gameObject.GetComponent<TweenAlpha>();
                    // ta.RemoveOnFinished(ed);

                    //param.value = child.gameObject;
                    //ed.parameters[0] = param;

                    // ta.onFinished.Add(ed);

                    TweenManager.tween_Manager.TweenAlpha(child.gameObject);
                
            }
        }
    }

    private IEnumerator ShaderTween(Material mat, float startAlpha, float endAlpha)
    {
        float speed = 1.0f;

        shaderTweenCheck = (startAlpha <= 0);

        mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, startAlpha);

        while (true)
        {
            yield return new WaitForEndOfFrame();

            if (shaderTweenCheck)
            {
                mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, mat.color.a + Time.deltaTime * speed);

                if (mat.color.a >= endAlpha)
                {
                    yield break;
                }
            }
            else
            {
                mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, mat.color.a - Time.deltaTime * speed);

                if (mat.color.a <= endAlpha)
                {
                    yield break;
                }
            }
        }
    }


    private IEnumerator FollowHead(Transform tr, GameObject obj)
    {
        float setTime = 1.0f;

        yield return new WaitForSeconds(setTime * 0.5f);

        obj.GetComponent<UIWidget>().color = new Color(1, 1, 1, 0);
        TweenManager.tween_Manager.TweenAllDestroy(obj);
        TweenManager.tween_Manager.AddTweenAlpha(obj, 0, 1, setTime * 0.5f);
        TweenManager.tween_Manager.TweenAlpha(obj);

        obj.SetActive(true);

        while (true)
        {
            obj.transform.position = new Vector3(tr.position.x, tr.position.y + 1.5f, tr.position.z);
            yield return new WaitForEndOfFrame();
        }
    }

    public void StartVuforia()
    {
        if (SetARCamCoroutine != null)
        {
            StopCoroutine(SetARCamCoroutine);
            SetARCamCoroutine = null;
        }

        SetARCamCoroutine = StartCoroutine(SetArCam());
    }

    private void SetRendererShadowsOnly()
    {
        Renderer mesh;

        for (int i = 0; i < 타깃정보.Length; i++)
        {
            if (타깃정보[i].마커타깃오브젝트.GetComponent<Renderer>() != null)
            {
                mesh = 타깃정보[i].마커타깃오브젝트.GetComponent<Renderer>();
                mesh.enabled = true;
                mesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                mesh.enabled = false;
            }
        }
    }

    private IEnumerator SetArCam()
    {
        Camera cam = null;

        for (int i =0; i< AR카메라.transform.childCount; i++)
        {
            if (AR카메라.transform.GetChild(i).name.Equals("Camera"))
            {
                cam = AR카메라.transform.GetChild(i).GetComponent<Camera>();
                break;
            }
        }

        if(cam == null)
        {
            Debug.Log("메인카메라가 없습니다.");
        }
        else
        {
            EasyTouch.RemoveCamera(cam);
            EasyTouch.AddCamera(cam);

            AR카메라.transform.Find("Camera").GetComponent<Camera>().depth = 1;

            AR카메라.SetActive(true);
        }

        TurnOffBehaviour.Instance.TurnOnVuforiaBehaviour();
        SetRendererShadowsOnly();

        yield return new WaitForSeconds(1f);
        if (DelTrackingReadyEvent != null)
        {
            DelTrackingReadyEvent();
        }
        Debug.Log("AR Camera [ OFF ] MODE");
    }

    /// <summary>
    /// 뷰포리아 끄기 모드 사용하는지 체크
    /// </summary>
    private void CheckOffVuforiaMode()
    {
        if (AR카메라 != null && AR카메라.activeSelf)
        {
            Debug.Log("AR Camera [ ON ] MODE");
            TurnOffBehaviour.Instance.TurnOnVuforiaBehaviour();
            SetRendererShadowsOnly();
        }
    }
}
