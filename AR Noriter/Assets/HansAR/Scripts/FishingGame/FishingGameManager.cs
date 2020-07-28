using UnityEngine;
using System.Collections;
using HedgehogTeam.EasyTouch;
using HansAR;

public class FishingGameManager : MonoBehaviour
{
    /// <summary>
    /// 스테이지
    /// </summary>
    public enum FishingStage
    {
        one,
        two,
        three,
        four,
        five,
        none
    }

    public enum map
    {
        normal,
        ice,
        typhoon,
        none
    }

    /// <summary>
    /// 게임 진행 상태
    /// </summary>
    public enum GameStatus
    {
        STANDBY,        // 대기중
        PLAYING,        // 게임중
        SUCCESS,        // 성공
        FAILED          // 실패
    }

    public enum FishingFloatStatus
    {
        IDLE,
        BITE,
        END
    }

    public enum TamiAniStatus
    {
        IDLE,
        CAST,
        ACTION,
        SUCCESS,
        FISHING,
        FAIL
    }

    private float FISHING_WAITING_TIME = 1.0f;                   // 낚시 대기 시간 (낚시대 던지고 물고기 잡히는데 걸리는 시간)
    private int emblemBgHeightSaved = 0;                         // 엠블램 배경 높이 저장


    public bool testMode = false;                                // 개발자 테스팅 모드
    public bool pause = false;                                   // 일시정지 여부

    public GameObject fishingGameObjRoot;                        // 에셋번들 상위 오브젝트
    public GameObject fishingGameObj;                            // 3D모델 Obj
    public GameObject weatherLight;                              // 날씨효과를 주기위한 조명

    private GameObject fishingShipObj;                           // 배 obj
    private GameObject fishingSeaObj;                            // 바다 obj
    private GameObject fishingFloatFish;                         // 잡힐 물고기 obj
    private GameObject fishingResultPanel;                       // 결과판(3d)  
    public GameObject fishingResultTami;                         // 결과판 타미
    public GameObject fishingResultFish;                         // 결과판 잡은 물고기
    public GameObject fishingResultTrap;                         // 결과판 잡은 함점(쓰레기)
    public GameObject collectTami;                               // 수집 리스트 타미

    public static readonly int startFishingPoint = 500;          // 게임 시작시 낚시 포인트 (ex : 500 설정시 게이지 중간에서 시작함)

    public int fishingPoint;                                     // 유저의 낚시 포인트
    public int fishStrength = 5;                                 // 물고기 힘 (포인트 감소)
    public int upPoint = 100;                                    // 터치할때 게이지 UP 포인트

    public float fishLength = 0;                                 // 물고기 길이

    public bool fishingSet = false;                              // 낚시 게이지 터치 시작
    private bool finishedGame = false;                           // 전체 게임 끝났는지 여부
    private bool stageClear = false;                             // 스테이지 클리어 여부
    private bool trapSet = false;                                // 물고기가 아닌경우 체크
    public bool resultSet = false;                               // 물고기 결과창 활성화 여부

    [HideInInspector]
    public FishingStage fishingStage;                            // 현재 스테이지

    [HideInInspector]
    public map fishingMap;                                       // 현재 맵

    //[HideInInspector]
    public FishSpeciesInfo.Fish[] fish;                          // 낚시할 물고기들 (유니티 Inspector 창에서 배열 셋팅)

    //[HideInInspector]
    public FishSpeciesInfo.Trap[] trapList;                      // 물고기가 아닌 다른 것

    [HideInInspector]
    public int fishIdx = 0;                                      // 낚시 물고기 인덱스

    [HideInInspector]
    public int trapIdx = 0;                                      // 함정 인덱스

    public GameStatus gameStatus;                                // 게임 진행 상태
    public FishingFloatStatus floatStatus;                       // 낚시 찌 상태

    public GameObject fishingFloat;                              // 낚시 찌
    public GameObject fishingFloatCursor;                        // 낚시 찌 위치 포인트 obj
    private GameObject fishingSucPar;                            // 낚시 완료 파티클(물튀는 효과 파티클)
    private Vector3 fishingFloatFirstPos;                        // 낚시 찌 초기화 position

    public int remnantFishNum = 0;                               // 남은 물고기 수
    public GameObject[] mapList;                                 // 맵object

    private Coroutine loadCor;                                   // 번들 로드 코루틴
    private Coroutine fishingFloatMoveCor;                       // 낚시찌 움직임 코루틴
    private Coroutine fishingStartCor;                           // 낚시 시작 코루틴
    private Coroutine eventTweenCor;                             // 낚시 이벤트 코루틴
    private Coroutine DecreaseFishingPointCor;                   // 낚시 게이지 감소 코루틴
    private Coroutine emblemCor;                                 // 낚시 시작 앰블럼 코루틴
    private Coroutine fishingResultCor;                          // 낚시 한마리 성공/실패시 결과 코루틴
    
    [HideInInspector]
    public Animator tamiAnimator;                               // 타미 animator
    private FishingMapInfo fishingMapInfo;                       // map, tami info
    public static FishingGameManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        LoadCorStart();

    }

    private void OnEnable()
    {
        EasyTouch.On_TouchUp += OnTouchUp;
        TargetManager.DelEventMarkerFound = Cognitive;
        TargetManager.DelEventMarkerLost = Noncognitive;
        TargetManager.DelTrackingReadyEvent = AfterLoadBundleEvent;
    }

    private void OnDisable()
    {
        UnsubscribeEvent();
    }

    private void OnDestroy()
    {
        UnsubscribeEvent();
    }

    private void UnsubscribeEvent()
    {
        EasyTouch.On_TouchUp -= OnTouchUp;
        TargetManager.DelEventMarkerFound = null;
        TargetManager.DelEventMarkerLost = null;
        TargetManager.DelTrackingReadyEvent = null;

        FishingGameAllCorStop();
    }

    /// <summary>
    /// fishinggame 에서 사용되는 모든 코루틴 정지
    /// </summary>
    private void FishingGameAllCorStop()
    {
        StartGameCorStop();
        EventUiTweenCorStop();
        FishingFloatMoveStop();
        DecreaseFishingPointStop();
        EmblemCorStop();
        FishingResultCorStop();
    }

    private void LoadCorStart()
    {
        LoadCorStop();
        loadCor = StartCoroutine(TrackerManagerCheck());
    }

    private void LoadCorStop()
    {
        if (loadCor != null)
        {
            StopCoroutine(loadCor);
            loadCor = null;
        }
    }

    private IEnumerator TrackerManagerCheck()
    {
        while (true)
        {
          //  yield return new WaitForEndOfFrame();

         //   if (Vuforia.TrackerManager.Instance.GetTracker<Vuforia.ObjectTracker>() != null)
         //   {
                BundleObj();
                yield break;
          //  }
        }
    }

    private void BundleObj()
    {
        HttpRequestDataSet allDataSet = null;

        allDataSet = new HttpRequestDataSet();

        //GlobalDataManager.m_ResourceFolderEnum = GlobalDataManager.m_ResourceFolderEnum;
        GlobalDataManager.m_ResourceFolderEnum = GlobalDataManager.CategoryType.SeaAnimal;
        GlobalDataManager.m_AssetBundlePartName = "fishinggame";

        GlobalDataManager.m_SelectedAssetBundleName = string.Format("{0}_{1}", GlobalDataManager.m_ResourceFolderEnum.ToString().ToLower(),
                                                                               GlobalDataManager.m_AssetBundlePartName.ToLower());

        Debug.Log("AssetBundleName : " + GlobalDataManager.m_SelectedAssetBundleName);

        TargetManager.타깃메니저.에셋번들복제컨텐츠 = new GameObject[TargetManager.타깃메니저.컨텐츠모델링이름.Length];

        AssetBundleLoader.getInstance.SetAssetBundleLoadInfo(allDataSet,
                                                           GlobalDataManager.m_SelectedAssetBundleName,
                                                           null,
                                                           AssetBundleLoader.getInstance.OnLoadCompleteModeling,
                                                           AfterCompletSet,
                                                           null,
                                                           null);


        AssetBundleLoader.getInstance.SetStorageLoadObject(allDataSet,
                                                          TargetManager.타깃메니저.컨텐츠모델링이름,
                                                          TargetManager.타깃메니저.에셋번들복제컨텐츠,
                                                          fishingGameObjRoot,
                                                          TargetManager.타깃메니저.AR카메라);


        AssetBundleLoader.getInstance.StartLoadAssetBundle(allDataSet);

        Debug.Log("번들로드");
    }

    public void AfterCompletSet(HttpRequestDataSet dataSet)
    {
        TargetManager.타깃메니저.StartVuforia();

        fishingGameObj.SetActive(false);

        gameStatus = GameStatus.STANDBY;
        fishingFloatFirstPos = fishingFloat.transform.localPosition;

        BundleLoadInitSet();
    }

    private void AfterLoadBundleEvent()
    {
        MainUI.메인.딜레이팝업UI.SetActive(false);
    }


    /// <summary>
    /// 번들 로드 후 오브젝트 셋팅
    /// </summary>
    private void BundleLoadInitSet()
    {
        fishingGameObj = TargetManager.타깃메니저.에셋번들복제컨텐츠[0];
        fishingMapInfo = TargetManager.타깃메니저.에셋번들복제컨텐츠[0].GetComponent<FishingMapInfo>();

        fishingFloatCursor = fishingMapInfo.fishingFloatCursor;
        fishingShipObj = fishingMapInfo.shipObj;
        fishingSeaObj = fishingMapInfo.seaObj;
        fishingFloatFish = fishingMapInfo.fishingFloatFish;

        fishingFloatCursor.transform.localScale = Vector3.zero;
        fishingFloatFish.transform.localScale = Vector3.zero;

        mapList = new GameObject[fishingMapInfo.mapLevelObj.Length];

        for(int i=0; i<mapList.Length; i++)
        {
            mapList[i] = fishingMapInfo.mapLevelObj[i];
            mapList[i].SetActive(false);
        }

        fishingFloatCursor.SetActive(false);
        fishingFloat.SetActive(false);
        fishingGameObj.SetActive(false);
        CollectionGet();

    }

    /// <summary>
    /// 인식시
    /// </summary>
    /// <param name="index"></param>
    public void Cognitive(int index)
    {
        MainUI.메인.인식글자UI.SetActive(false);
        StageSet(FishingStage.one);

        TargetManager.EnableTracking = false;
        AutoFocusMode.getInstance.useAutoFocusMode = false;
        
    }

    /// <summary>
    /// 비인식시
    /// </summary>
    /// <param name="index"></param>
    public void Noncognitive(int index)
    {

    }

    /// <summary>
    /// 낚시 딜레이 coroutine start
    /// </summary>
    private void StartGameCorStart()
    {
        StartGameCorStop();
        fishingStartCor = StartCoroutine(WaitingFishEvent());
    }

    /// <summary>
    /// 낚시 딜레이 coroutine stop
    /// </summary>
    private void StartGameCorStop()
    {
        if (fishingStartCor != null)
        {
            StopCoroutine(fishingStartCor);
            fishingStartCor = null;
        }
    }

    /// <summary>
    /// 낚시 시작
    /// </summary>
    private void StartClickGame()
    {
        //FishingGameUI.getInstance.rodUi.rodBtn.SetActive(false);
        fishingGauageUiActiveSet(true);

        fishingPoint = startFishingPoint;

        FishingFloatBite();

        if (trapIdx < trapList.Length)
        {
            if(Random.Range(0, 2) == 0)
            {
                //함정
                Debug.Log("함정");
                trapSet = true;
                ResetTrapInfo();
            }
            else
            {
                //물고기
                Debug.Log("물고기");
                trapSet = false;
                ResetFishInfo();
            }

        }
        else
        {
            trapSet = false;
            ResetFishInfo();
        }

        DecreaseFishingPointStart();
    }

    /// <summary>
    /// 게이지 감소 코루틴 start
    /// </summary>
    private void DecreaseFishingPointStart()
    {
        DecreaseFishingPointStop();

        DecreaseFishingPointCor = StartCoroutine(DecreaseFishingPoint());
    }

    /// <summary>
    /// 게이지 감소 코루틴 stop
    /// </summary>
    private void DecreaseFishingPointStop()
    {
        if(DecreaseFishingPointCor !=null)
        {
            StopCoroutine(DecreaseFishingPointCor);
            DecreaseFishingPointCor = null;
        }
    }

    /// <summary>
    /// 남은 물고기 수 확인 및 fishRemnantLabel 에 남은 물고기 수 출력
    /// </summary>
    private void RemnantFishCheck()
    {
        remnantFishNum = fish.Length - fishIdx;
        FishingGameUI.getInstance.fishRemnantLabel.text = remnantFishNum.ToString();
    }

    /// <summary>
    /// 다음 스테이지 버튼 클릭
    /// </summary>
    public void NextStage()
    {
        StageSet((FishingStage)((int)fishingStage + 1));
        AlphaPopupClose(FishingGameUI.getInstance.result.resultUI, 0.3f);
    }

    /// <summary>
    /// 이전 스테이지 버튼 클릭
    /// </summary>
    public void PrevStage()
    {
        StageSet((FishingStage)((int)fishingStage - 1));
        AlphaPopupClose(FishingGameUI.getInstance.result.resultUI, 0.3f);
    }

    /// <summary>
    /// 현재 스테이지 재시작 버튼 클릭
    /// </summary>
    public void RestartStage()
    {
        StageSet(fishingStage);
        AlphaPopupClose(FishingGameUI.getInstance.result.resultUI, 0.3f);
    }


    /// <summary>
    /// 일시정지 상태에서 현재 스테이지 재시작 버튼 클릭
    /// </summary>
    public void PauseRestartStage()
    {
        StageSet(fishingStage);
        PauseUiClose();
    }

    /// <summary>
    /// 게임 종료 
    /// </summary>
    public void EndGame()
    {
        gameStatus = GameStatus.STANDBY;
        TargetManager.EnableTracking = true;
        AutoFocusMode.getInstance.useAutoFocusMode = true;
        FishingGameUI.getInstance.initUi();

        FishingGameAllCorStop();

        fishingGameObj.SetActive(false);
        fishingFloat.SetActive(false);

        FishingFloatCursorInit();

        MainUI.메인.인식글자UI.SetActive(true);
    }

    /// <summary>
    /// 낚시 찌 위치 표시 커서 초기화 부분
    /// </summary>
    private void FishingFloatCursorInit()
    {
        if (fishingFloatCursor != null)
        {
            TweenManager.tween_Manager.TweenAllDestroy(fishingFloatCursor);
            fishingFloatCursor.transform.localScale = Vector3.zero;
            fishingFloatCursor.SetActive(false);
        }

        if (fishingSucPar != null)
        {
            Destroy(fishingSucPar);
        }
    }

    /// <summary>
    /// 터치up 이벤트로 게이지 확인(낚시 성공부분 확인)
    /// </summary>
    /// <param name="ges"></param>
    private void OnTouchUp(Gesture ges)
    {
        if (ges.pickedObject != null && gameStatus == GameStatus.PLAYING)
        {
            if (ges.pickedObject == FishingGameUI.getInstance.touchArea)
            {
                // 낚시 포인트 900점 기준으로 성공 여부 판단
                if (fishingPoint <= 900)
                {
                    // 900 포인트 이하일시 터치 할때마다 포인트 증가
                    fishingPoint += upPoint;
                    FishingCameraMove.instance.ShakeCamStart();
                }
                else
                {
                    // 900점 이상일시 낚시 성공
                    FishingResultCorStart();

                    fishingGauageUiActiveSet(false);
                }
            }
        }
    }

    /// <summary>
    /// 낚시 성공(물고기, 함정)시에 물이 튀는 파티클 호출
    /// </summary>
    private void FishingSuccessPar()
    {
        GameObject prefab;
        prefab = Instantiate(fishingMapInfo.fishingPar) as GameObject;
        GlobalDataManager.ShaderRefresh(prefab);

        prefab.transform.parent = fishingFloatCursor.transform;
        prefab.transform.localPosition = Vector3.zero;
        prefab.transform.localScale = Vector3.one;
        prefab.SetActive(true);

        fishingSucPar = prefab;
    }

    private void ResultObjDestroy()
    {
        if(fishingResultFish != null)
        {
            Destroy(fishingResultFish);
        }

        if(fishingResultPanel != null)
        {
            Destroy(fishingResultPanel);
        }

        if(fishingResultTrap != null)
        {
            Destroy(fishingResultTrap);
        }

        if (fishingResultTami != null)
        {
            Destroy(fishingResultTami);
        }
    }


    private void CopyTamiAndFishingEndPanelSet(bool state)
    {
        if (state)
        {
            FishingResultPanelSet();
            CopyTamiSet();

            if (gameStatus == GameStatus.SUCCESS)
            {
                CopyFishSet();
            }
            else
            {
                CopyTrapSet();
            }
        }
        else
        {
            if (fishingResultPanel != null)
            {
                fishingResultPanel.SetActive(false);
            }

            if (fishingResultTami != null)
            {
                fishingResultTami.SetActive(false);
            }

            if (fishingResultFish != null)
            {
                fishingResultFish.SetActive(false);
            }

            if(fishingResultTrap !=null)
            {
                fishingResultTrap.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 물고기 잡거나 쓰레기를 낚았을 경우 결과창에 보여지는 결과판
    /// </summary>
    private void FishingResultPanelSet()
    {
        if (fishingResultPanel == null)
        {
            fishingResultPanel = Instantiate(fishingMapInfo.resultPanelInfo.resultPanel) as GameObject;
            ChangeLayerName(fishingResultPanel, "2D UI");
            GlobalDataManager.ShaderRefresh(fishingResultPanel);

            fishingResultPanel.transform.parent = FishingGameUI.getInstance.rootUI.resultPanelPosObj.transform;
            fishingResultPanel.transform.localPosition = new Vector3(0, 0, 0);
            fishingResultPanel.transform.localEulerAngles = new Vector3(-90, 180, 0);
            fishingResultPanel.transform.localScale = new Vector3(2000, 2000, 2000);
        }

        if (gameStatus == GameStatus.SUCCESS)
        {
            if (LocalizeText.CurrentLanguage == SystemLanguage.Korean)
            {
                fishingResultPanel.GetComponent<Renderer>().material.mainTexture = fishingMapInfo.resultPanelInfo.catchKorTex;
            }
            else
            {
                fishingResultPanel.GetComponent<Renderer>().material.mainTexture = fishingMapInfo.resultPanelInfo.catchEnTex;
            }
        }
        else
        {
            if (LocalizeText.CurrentLanguage == SystemLanguage.Korean)
            {
                fishingResultPanel.GetComponent<Renderer>().material.mainTexture = fishingMapInfo.resultPanelInfo.failKorTex;
            }
            else
            {
                fishingResultPanel.GetComponent<Renderer>().material.mainTexture = fishingMapInfo.resultPanelInfo.failEnTex;
            }
        }
        
        fishingResultPanel.SetActive(true);
    }

    /// <summary>
    /// 물고기 잡거나 쓰레기를 낚았을 경우 결과창에 보여지는 타미
    /// </summary>
    private void CopyTamiSet()
    {
        if (fishingResultTami != null)
        {
            Destroy(fishingResultTami);
        }

        fishingResultTami = Instantiate(tamiAnimator.gameObject) as GameObject;
        ChangeLayerName(fishingResultTami, "2D UI");
        GlobalDataManager.ShaderRefresh(fishingResultTami);

        fishingResultTami.transform.parent = FishingGameUI.getInstance.rootUI.CopyTamiPosObj.transform;
        fishingResultTami.transform.localPosition = new Vector3(0, 0, -600);
        fishingResultTami.transform.localEulerAngles = new Vector3(0, -180, 0);
        fishingResultTami.transform.localScale = new Vector3(2000, 2000, 2000);

        fishingResultTami.SetActive(true);
        resultSet = true;
    }

    /// <summary>
    /// 물고기 잡기 성공시 결과창에 보여지는 물고기
    /// </summary>
    private void CopyFishSet()
    {
        bool isSame = false;
        bool already = false;

        fishingResultFish = null;

        for (int i = 0; i < FishingGameUI.getInstance.rootUI.CopyFishPosObj.transform.childCount; i++)
        {
            isSame = FishingGameUI.getInstance.rootUI.CopyFishPosObj.transform.GetChild(i).name.Contains(fishingMapInfo.fishPrefabs[(int)fish[fishIdx].species].gameObject.name);
            FishingGameUI.getInstance.rootUI.CopyFishPosObj.transform.GetChild(i).gameObject.SetActive(false);

            if (isSame)
            {
                fishingResultFish = FishingGameUI.getInstance.rootUI.CopyFishPosObj.transform.GetChild(i).gameObject;
                fishingResultFish.SetActive(true);
                already = true;
            }
        }

        if(already)
        {
            return;
        }

        fishingResultFish = Instantiate(fishingMapInfo.fishPrefabs[(int)fish[fishIdx].species].gameObject) as GameObject;
        ChangeLayerName(fishingResultFish, "2D UI");
        GlobalDataManager.ShaderRefresh(fishingResultFish);

        fishingResultFish.transform.parent = FishingGameUI.getInstance.rootUI.CopyFishPosObj.transform;
        fishingResultFish.transform.localPosition = new Vector3(0, 0, -200);
        fishingResultFish.transform.localEulerAngles = new Vector3(0, 45, 0);
        fishingResultFish.transform.localScale = new Vector3(1000, 1000, 1000);

        fishingResultFish.SetActive(true);
        resultSet = true;
    }

    /// <summary>
    /// 잡기 실패 결과창에 보여지는 함정(쓰레기)
    /// </summary>
    private void CopyTrapSet()
    {
        bool isSame = false;
        bool already = false;

        fishingResultTrap = null;

        for (int i = 0; i < FishingGameUI.getInstance.rootUI.CopyTrapPosObj.transform.childCount; i++)
        {
            isSame = FishingGameUI.getInstance.rootUI.CopyTrapPosObj.transform.GetChild(i).name.Contains(fishingMapInfo.trapPrefans[(int)trapList[trapIdx].species].gameObject.name);
            FishingGameUI.getInstance.rootUI.CopyTrapPosObj.transform.GetChild(i).gameObject.SetActive(false);

            if (isSame)
            {
                fishingResultTrap = FishingGameUI.getInstance.rootUI.CopyTrapPosObj.transform.GetChild(i).gameObject;
                fishingResultTrap.SetActive(true);
                already = true;
            }
        }

        if (already)
        {
            return;
        }

        fishingResultTrap = Instantiate(fishingMapInfo.trapPrefans[(int)trapList[trapIdx].species].gameObject) as GameObject;
        ChangeLayerName(fishingResultTrap, "2D UI");
        GlobalDataManager.ShaderRefresh(fishingResultTrap);

        fishingResultTrap.transform.parent = FishingGameUI.getInstance.rootUI.CopyTrapPosObj.transform;
        fishingResultTrap.transform.localPosition = new Vector3(0, 0, -200);
        fishingResultTrap.transform.localEulerAngles = new Vector3(-90, 45, 0);
        fishingResultTrap.transform.localScale = new Vector3(1000, 1000, 1000);

        fishingResultTrap.SetActive(true);
        resultSet = true;
    }


    public void ChangeLayerName(GameObject obj, string layerName)
    {
        Transform[] tran = obj.GetComponentsInChildren<Transform>(true);

        // 하위 레이어까지 전부 변경
        foreach (Transform child in tran)
        {
            child.gameObject.layer = LayerMask.NameToLayer(layerName);
        }
    }

    private void fishingGauageUiActiveSet(bool state)
    {
        FishingGameUI.getInstance.fishingGauage.fishingGaugeUI.SetActive(state);
        FishingGameUI.getInstance.fishingGauage.touchImgUI.SetActive(state);
        FishingGameUI.getInstance.touchArea.SetActive(state);
    }

    /// <summary>
    /// 낚시대 버튼 클릭 이벤트 (클릭시 게임 시작)
    /// </summary>
    public void OnClickFishingRodBtn()
    {
        AlphaPopupClose(FishingGameUI.getInstance.rodUi.rodUI, 0.3f);
        TamiAnimationSet(TamiAniStatus.CAST);
        //FishingGameUI.getInstance.rodUi.rodBtn.SetActive(false);
        StartGameCorStart();
    }

    /// <summary>
    /// 재시작 버튼 클릭 이벤트
    /// </summary>
    public void OnClickRestartFishingBtn()
    {
        FishingGameUI.getInstance.failFishingEndUi.endFishingUI.SetActive(false);
        StartGameCorStart();
    }

    /// <summary>
    /// 한마리씩 낚을때 또는 실패할시 확인창 닫기
    /// </summary>
    public void OnClickCloseFishingUI()
    {
        FishingGameUI.getInstance.sucFishingEndUi.endFishingUI.SetActive(false);
        FishingGameUI.getInstance.failFishingEndUi.endFishingUI.SetActive(false);
        FishingGameUI.getInstance.trapFishingEndUi.endFishingUI.SetActive(false);
        CopyTamiAndFishingEndPanelSet(false);
        UpdateFishIndex();

        AlphaPopupOpen(FishingGameUI.getInstance.rodUi.rodUI, 0.3f);
        TamiAnimationSet(TamiAniStatus.IDLE);
        resultSet = false;
    }
    
    /// <summary>
    /// 낚시 포인트를 감소시킴
    /// </summary>
    private IEnumerator DecreaseFishingPoint()
    {
        float fishingFloatYposValue = FishingGameUI.getInstance.fishingGauage.fishingGaugeBar.GetComponent<UISlider>().value - 0.5f;
        float savedfishingPoint = fishingPoint;

        GameObject wheelBar = FishingGameUI.getInstance.fishingGauage.fishingWheelBar;

        TweenManager.tween_Manager.TweenAllDestroy(wheelBar);

        wheelBar.transform.localEulerAngles = Vector3.zero;

        while (gameStatus == GameStatus.PLAYING && fishingPoint > 0)
        {
            if (!pause)
            {
                // 물고기 힘만큼 포인트가 줄게 됨
                fishingPoint -= fishStrength;
                FishingGameUI.getInstance.fishingGauage.fishingGaugeBar.GetComponent<UISlider>().value = (fishingPoint / 1000f);
                
                fishingFloatYposValue = FishingGameUI.getInstance.fishingGauage.fishingGaugeBar.GetComponent<UISlider>().value - 0.5f;
                fishingFloat.transform.localPosition = new Vector3(fishingFloatFirstPos.x, fishingFloatYposValue, fishingFloatFirstPos.z);

            }

            TweenManager.tween_Manager.AddTweenRotation(wheelBar,
                                                       wheelBar.transform.localEulerAngles,
                                                       new Vector3(wheelBar.transform.localEulerAngles.x,
                                                                   wheelBar.transform.localEulerAngles.y,
                                                                   wheelBar.transform.localEulerAngles.z + ((fishingPoint - savedfishingPoint) * 1.08f)),
                                                       0.1f);

            TweenManager.tween_Manager.TweenRotation(FishingGameUI.getInstance.fishingGauage.fishingWheelBar);

            savedfishingPoint = fishingPoint;

            yield return new WaitForSeconds(0.1f);
        }

        if (gameStatus == GameStatus.PLAYING)
        {
            gameStatus = GameStatus.FAILED;
            fishingGauageUiActiveSet(false);
            FishingResultCorStart();
        }

        yield break;
    }

    /// <summary>
    /// 낚시 실패 이벤트
    /// </summary>
    private void FailedFishing()
    {
        gameStatus = GameStatus.FAILED;
        TamiAnimationSet(TamiAniStatus.FAIL);
        FishingGameUI.getInstance.failFishingEndUi.endFishingUI.SetActive(true);

        FishingFloatEnd();
        
        Debug.Log("낚시 실패 이벤트");
    }

    /// <summary>
    /// 낚시 성공 이벤트
    /// </summary>
    private void SuccessFishing()
    {
        gameStatus = GameStatus.SUCCESS;
        TamiAnimationSet(TamiAniStatus.SUCCESS);
        CopyTamiAndFishingEndPanelSet(true);

        FishingGameUI.getInstance.sucFishingEndUi.endFishingUI.SetActive(true);

        FishingGameUI.getInstance.catchFish[fishIdx].SetActive(true);
        FishingFloatEnd();

        fishIdx++;
        RemnantFishCheck();

        if(fishingResultTami != null)
        {
            fishingResultTami.GetComponent<Animator>().SetTrigger(fishingMapInfo.tamiInfo.successTriggerName);
        }
        Debug.Log("물고기 인덱스 : " + fishIdx);
        Debug.Log("낚시 성공 이벤트");

        // TODO : 물고기 낚는 애니메이션 실행
    }

    /// <summary>
    /// 함정 성공 이벤트
    /// </summary>
    private void SuccessTrap()
    {
        gameStatus = GameStatus.FAILED;
        TamiAnimationSet(TamiAniStatus.FAIL);
        CopyTamiAndFishingEndPanelSet(true);
        FishingGameUI.getInstance.trapFishingEndUi.endFishingUI.SetActive(true);

        FishingFloatEnd();

        trapIdx++;

        if (fishingResultTami != null)
        {
            fishingResultTami.GetComponent<Animator>().SetTrigger(fishingMapInfo.tamiInfo.failTriggerName);
        }

        Debug.Log("함정 인덱스 : " + trapIdx);
        Debug.Log("함정 성공 이벤트");

        // TODO : 물고기 낚는 애니메이션 실행
    }

    /// <summary>
    /// 스테이지 설정에 따른 맵 설정
    /// </summary>
    /// <param name="stage"></param>
    private void StageSet(FishingStage stage)
    {
        pause = false;
        resultSet = false;
        stageClear = false;

        fishIdx = 0;
        trapIdx = 0;

        FishingGameAllCorStop();
        ResultObjDestroy();

        fishingGameObj.SetActive(true);
        FishingGameUI.getInstance.InitFishingGameUI();
        FishingGameUI.getInstance.StartUiSet();
     
        RemnantFishCheck();
        EmblemCorStart();

        if (fishingFloatFish != null)
        {
            fishingFloat.SetActive(false);
            fishingFloatFish.transform.localScale = Vector3.zero;
        }

        FishingFloatCursorInit();

        fishingStage = stage;

        switch (stage)
        {
            case FishingStage.one:
                fishingMap = map.normal;
                break;

            case FishingStage.two:
                fishingMap = map.ice;
                break;

            case FishingStage.three:
                fishingMap = map.typhoon;
                break;

            case FishingStage.four:
                fishingMap = map.typhoon;
                break;

            case FishingStage.five:
                fishingMap = map.typhoon;
                break;

            default:
                Debug.Log("잘못된 스테이지 설정입니다.");
                break;
        }

        MapSetting();
        ResetFishList();
    }

    /// <summary>
    /// stage에 따라 설정된 맵 셋팅 적용부분 
    /// </summary>
    private void MapSetting()
    {
        string mapLocalizeTextName = string.Empty;
        string mapSpriteName = string.Empty;
        UISprite eventImg = FishingGameUI.getInstance.eventUi.leftTextObj.GetComponent<UISprite>();

        for (int i = 0; i < mapList.Length; i++)
        {
            mapList[i].SetActive(false);
        }

        switch (fishingMap)
        {
            case map.normal:
                mapList[0].SetActive(true);
                mapLocalizeTextName = "normalMap";
                mapSpriteName = "normal_Map";
                eventImg.spriteName = "Catch_bg_00";

                ShipMoveTween(2, 2);
                SeaMoveSet(0.01f, -0.05f);
                weatherLight.transform.localEulerAngles = new Vector3(4, 0, 0);
                weatherLight.GetComponent<Light>().color = new Color32(255, 255, 255, 255);
                FishingGameUI.getInstance.weatherScreen.alpha = 0;
                FishingGameUI.getInstance.weatherScreen.gameObject.SetActive(false);
                break;

            case map.ice:
                mapList[1].SetActive(true);
                mapLocalizeTextName = "iceMap";
                mapSpriteName = "ice_Map";
                eventImg.spriteName = "Catch_bg_01";

                ShipMoveTween(2, 2);
                SeaMoveSet(0.01f, -0.05f);
                weatherLight.transform.localEulerAngles = new Vector3(7.5f, 0, 0);
                weatherLight.GetComponent<Light>().color = new Color32(200, 200, 200, 255);
                FishingGameUI.getInstance.weatherScreen.alpha = 0;
                FishingGameUI.getInstance.weatherScreen.gameObject.SetActive(false);
                break;

            case map.typhoon:
                mapList[2].SetActive(true);
                mapLocalizeTextName = "typhoonMap";
                mapSpriteName = "typhoon_Map";
                eventImg.spriteName = "Catch_bg_02";

                ShipMoveTween(1.5f, 4);
                SeaMoveSet(0.05f, -0.2f);
                weatherLight.transform.localEulerAngles = new Vector3(-90, 0, 0);
                weatherLight.GetComponent<Light>().color = new Color32(255, 255, 255, 255);
                FishingGameUI.getInstance.weatherScreen.alpha = 0.3f;
                FishingGameUI.getInstance.weatherScreen.gameObject.SetActive(true);
                break;

            default:
                Debug.Log("맵설정을 확인하세요");
                break;
        }

        FishingGameUI.getInstance.emblemUi.emblemLabel.GetComponent<LocalizeText>().ValueName = "mapLocalizeTextName";
        FishingGameUI.getInstance.emblemUi.emblemLabel.text = LocalizeText.Value[mapLocalizeTextName];
        FishingGameUI.getInstance.emblemUi.emblemSpr.spriteName = mapSpriteName;

        TamiModelSet();
    }

    private void ShipMoveTween(float tweenTime, float rotSensitivity)
    {
        TweenManager.tween_Manager.TweenAllDestroy(fishingShipObj);
        TweenManager.tween_Manager.AddTweenRotation(fishingShipObj,
                                                    new Vector3(-rotSensitivity, 0, 0),
                                                    new Vector3(rotSensitivity, 0, 0),
                                                    tweenTime,
                                                    UITweener.Style.PingPong,
                                                    TweenManager.tween_Manager.animationCurveList[1]);

        TweenManager.tween_Manager.TweenRotation(fishingShipObj);
        
        
    }

    /// <summary>
    /// 바다 움직임(쉐이더 변수값 변경)
    /// </summary>
    /// <param name="waveHeight"></param>
    /// <param name="foamSpeed"></param>
    private void SeaMoveSet(float waveHeight, float foamSpeed)
    {
        fishingSeaObj.transform.GetChild(0).GetComponent<MeshRenderer>().material.SetFloat("_Height", waveHeight);
        fishingSeaObj.transform.GetChild(0).GetComponent<MeshRenderer>().material.SetFloat("_WSpeed", foamSpeed);
    }

    /// <summary>
    /// 물고기(중복안됨), 함정 설정(중복) 
    /// </summary>
    private void ResetFishList()
    {
        int stageFishCount = fish.Length;
        //bool isSame = false;

        for (int i = 0; i < trapList.Length; i++)
        {
            trapList[i].species = (FishSpeciesInfo.TrapSpecies)Random.Range(0, (int)FishSpeciesInfo.TrapSpecies.none);
        }

        for (int i = 0; i < fish.Length; ++i)
        {
            fish[i].species = (FishSpeciesInfo.FishSpecies)Random.Range(StageFishStartNumSet(), StageFishEndNumSet());
        }
        /*
        for (int i = 0; i < fish.Length; ++i)
        {
            while (true)
            {
                fish[i].species = (FishSpeciesInfo.FishSpecies)Random.Range((int)fishingStage * stageFishCount, ((int)fishingStage + 1) * stageFishCount);

                isSame = false;

                for (int j = 0; j < i; ++j)
                {
                    if (fish[i00].species == fish[j].species)
                    {
                        isSame = true;
                        break;
                    }
                }

                if (!isSame)
                {
                    break;
                }
            }
        }
        */
    }

    /// <summary>
    /// 물고기 - 스테이지에 따른 시작 enum 값
    /// </summary>
    /// <returns></returns>
    private int StageFishStartNumSet()
    { 
        int startNum = 0;

        switch(fishingStage)
        {
            case FishingStage.one:
                startNum = 0;
                break;

            case FishingStage.two:
                startNum = 5;
                break;

            case FishingStage.three:
                startNum = 9;
                break;

            default:
                Debug.Log("스테이지 설정 이상부분 확인");
                break;
        }

        return startNum;
    }

    /// <summary>
    ///  물고기 - 스테이지에 따른 끝 enum 값
    /// </summary>
    /// <returns></returns>
    private int StageFishEndNumSet()
    {
        int endNum = 0;

        switch (fishingStage)
        {
            case FishingStage.one:
                endNum = 5;
                break;

            case FishingStage.two:
                endNum = 9;
                break;

            case FishingStage.three:
                endNum = 12;
                break;

            default:
                Debug.Log("스테이지 설정 이상부분 확인");
                break;
        }

        return endNum;
    }



    /// <summary>
    /// 낚시 정보 리셋 (물고기 정보 리셋)
    /// </summary>
    private void ResetFishInfo()
    {
        FishSpeciesInfo.SetFishInfo(fish[fishIdx]);
        fishStrength = fish[fishIdx].strength;
        fishLength = fish[fishIdx].length;

        FishNameSet(fish[fishIdx].textValueName);
        FishLengthSet(fish[fishIdx].length);
        if (testMode)
        {
            upPoint = 200;
        }
        else
        {
            upPoint = fish[fishIdx].upPoint;
        }

        gameStatus = GameStatus.PLAYING;
    }


    /// <summary>
    /// 함정 정보 리셋 (물고기 정보 리셋)
    /// </summary>
    private void ResetTrapInfo()
    {
        FishSpeciesInfo.SetTrapInfo(trapList[trapIdx]);
        fishStrength = trapList[trapIdx].strength;
        fishLength = trapList[trapIdx].length;

        FishNameSet(trapList[trapIdx].textValueName);
        FishLengthSet(trapList[trapIdx].length);
        if (testMode)
        {
            upPoint = 200;
        }
        else
        {
            upPoint = trapList[trapIdx].upPoint;
        }

        gameStatus = GameStatus.PLAYING;
    }

    /// <summary>
    /// 한마리 낚을때 또는 놓쳤을때 해당 물고기 이름 결과창에 띄워줌
    /// </summary>
    /// <param name="localizeValueName"></param>
    private void FishNameSet(string localizeValueName)
    {
        FishingGameUI.getInstance.sucFishingEndUi.endFishLabel.text = LocalizeText.Value[localizeValueName];
        FishingGameUI.getInstance.sucFishingEndUi.endFishLabel.GetComponent<LocalizeText>().ValueName = localizeValueName;

        FishingGameUI.getInstance.failFishingEndUi.endFishLabel.text = LocalizeText.Value[localizeValueName];
        FishingGameUI.getInstance.failFishingEndUi.endFishLabel.GetComponent<LocalizeText>().ValueName = localizeValueName;

        FishingGameUI.getInstance.trapFishingEndUi.endFishLabel.text = LocalizeText.Value[localizeValueName];
        FishingGameUI.getInstance.trapFishingEndUi.endFishLabel.GetComponent<LocalizeText>().ValueName = localizeValueName;
    }

    /// <summary>
    /// 물고기 길이 text에 출력
    /// </summary>
    /// <param name="fishLength"></param>
    private void FishLengthSet(float fishLength)
    {
        FishingGameUI.getInstance.sucFishingEndUi.lengthLabel.text = string.Format("{0}m", fishLength.ToString("N2")); 

        FishingGameUI.getInstance.failFishingEndUi.lengthLabel.text = string.Format("{0}m", fishLength.ToString("N2"));

        FishingGameUI.getInstance.trapFishingEndUi.lengthLabel.text = string.Format("{0}m", fishLength.ToString("N2"));
    }

    /// <summary>
    /// 물고기 인덱스 업데이트
    /// </summary>
    private void UpdateFishIndex()
    {
        // 낚시 성공 했을시
        if (gameStatus == GameStatus.SUCCESS)
        {
            if (!trapSet)
            {
                // 스테이지에 더 잡을 물고기가 남았는지 체크함
                if (fishIdx > fish.Length - 1)
                {
                    fishIdx = 0;
                    stageClear = true;

                    // 스테이지의 모든 물고기를 잡았을시
                    // 스테이지 클리어 이벤트 실행
                    StageClearEvent();

                  //  // 잡을 물고기가 더 있으면 인덱스 증가
                  //  Debug.Log("물고기 인덱스 : " + fishIdx);
                }
                /*
                else
                {
                    fishIdx = 0;
                    stageClear = true;

                    // 스테이지의 모든 물고기를 잡았을시
                    // 스테이지 클리어 이벤트 실행
                    StageClearEvent();
                }
                */
            }
        }
    }

    /// <summary>
    /// 맵에 따른 타미 설정
    /// </summary>
    private void TamiModelSet()
    {
        fishingMapInfo.tamiInfo.normalTami.SetActive(false);
        fishingMapInfo.tamiInfo.iceTami.SetActive(false);
        fishingMapInfo.tamiInfo.typhoonTami.SetActive(false);

        switch (fishingMap)
        {
            case map.normal:
                fishingMapInfo.tamiInfo.normalTami.SetActive(true);
                tamiAnimator = fishingMapInfo.tamiInfo.normalTami.GetComponent<Animator>();
                break;

            case map.ice:
                fishingMapInfo.tamiInfo.iceTami.SetActive(true);
                tamiAnimator = fishingMapInfo.tamiInfo.iceTami.GetComponent<Animator>();
                break;

            case map.typhoon:
                fishingMapInfo.tamiInfo.typhoonTami.SetActive(true);
                tamiAnimator = fishingMapInfo.tamiInfo.typhoonTami.GetComponent<Animator>();
                break;

            default:
                Debug.Log("현재 맵 설정 확인");
                break;
        }
        GlobalDataManager.ShaderRefresh(tamiAnimator.gameObject);
        TamiAnimationSet(TamiAniStatus.IDLE);

        CollectTamiSet();

        FishingCameraMove.instance.StartCamMove();
        
    }

    /// <summary>
    /// 타미 애니메이션 호출
    /// </summary>
    /// <param name="tamiAni"></param>
    private void TamiAnimationSet(TamiAniStatus tamiAni)
    {
      switch(tamiAni)
        {
            case TamiAniStatus.IDLE:
                tamiAnimator.SetTrigger(fishingMapInfo.tamiInfo.idleTriggerName);
                break;

            case TamiAniStatus.CAST:
                tamiAnimator.SetTrigger(fishingMapInfo.tamiInfo.castTriggerName);
                break;

            case TamiAniStatus.ACTION:
                tamiAnimator.SetTrigger(fishingMapInfo.tamiInfo.actionTriggerName);
                break;

            case TamiAniStatus.FISHING:
                tamiAnimator.SetTrigger(fishingMapInfo.tamiInfo.fishingTriggerName);
                break;

            case TamiAniStatus.SUCCESS:
                tamiAnimator.SetTrigger(fishingMapInfo.tamiInfo.successTriggerName);
                break;

            case TamiAniStatus.FAIL:
                tamiAnimator.SetTrigger(fishingMapInfo.tamiInfo.failTriggerName);
                break;

            default:
                Debug.Log("없는 애니메이션 동작입니다.");
                break;
        }
    }


    /// <summary>
    /// 물고기 기다림 이벤트
    /// </summary>
    private IEnumerator WaitingFishEvent()
    {
        fishingFloat.transform.localPosition = fishingFloatFirstPos;

        FishingFloatMove();

        FISHING_WAITING_TIME = Random.Range(1.0f, 4.0f);

        while (true)
        {
            yield return new WaitForEndOfFrame();

            if (fishingSet)
            {
                yield return new WaitForSeconds(FISHING_WAITING_TIME);
                TamiAnimationSet(TamiAniStatus.ACTION);

                EventUiTweenCorStart();
                yield break;
            }
        }
        //yield return new WaitForSeconds(FISHING_WAITING_TIME);

        //StartClickGame();
    }

    /// <summary>
    /// 스테이지 클리어 이벤트
    /// </summary>
    private void StageClearEvent()
    {
        FishingGameUI.getInstance.InitFishingGameUI();
        CopyTamiAndFishingEndPanelSet(false);

        ResultBtnSet();
        AlphaPopupOpen(FishingGameUI.getInstance.result.resultUI, 0.3f);
    }

    /// <summary>
    /// 마지막 결과창(5마리 다 잡고) 버튼 비/활성화 부분 
    /// </summary>
    private void ResultBtnSet()
    {
        FishingGameUI.getInstance.result.prevBtn.SetActive(false);
        FishingGameUI.getInstance.result.nextBtn.SetActive(false);

        switch (fishingStage)
        {
            case FishingStage.one:
                FishingGameUI.getInstance.result.nextBtn.SetActive(true);
                break;

            case FishingStage.two:
                FishingGameUI.getInstance.result.prevBtn.SetActive(true);
                FishingGameUI.getInstance.result.nextBtn.SetActive(true);
                break;

            case FishingStage.three:
                FishingGameUI.getInstance.result.prevBtn.SetActive(true);
                break;

            default:
                Debug.Log("스테이지 설정 확인 필요");
                break;
        }
    }

    /// <summary>
    /// 일시정지 창 open
    /// </summary>
    public void PauseUiOpen()
    {
        SideMenuOpenPause();

        AlphaPopupOpen(FishingGameUI.getInstance.pauseUi.pausePopUpUI, 0.3f);
    }

    /// <summary>
    /// 일시정지 창 close
    /// </summary>
    public void PauseUiClose()
    {
        SideMenuClosePause();

        AlphaPopupClose(FishingGameUI.getInstance.pauseUi.pausePopUpUI, 0.3f);
    }

    /// <summary>
    /// 사이드 메뉴 클릭시 일시정지
    /// </summary>
    public void SideMenuOpenPause()
    {
        pause = true;

        FishingGameUI.getInstance.rootUI.objRootUI.SetActive(false);
    }

    /// <summary>
    /// 사이드 메뉴 클릭시 일시정지 해제
    /// </summary>
    public void SideMenuClosePause()
    {
        pause = false;
        FishingGameUI.getInstance.rootUI.objRootUI.SetActive(true);

        if (fishingResultTami != null)
        {
            if(gameStatus == GameStatus.SUCCESS)
            {
                fishingResultTami.GetComponent<Animator>().SetTrigger(fishingMapInfo.tamiInfo.successTriggerName);
            }
            else if(gameStatus == GameStatus.FAILED)
            {
                fishingResultTami.GetComponent<Animator>().SetTrigger(fishingMapInfo.tamiInfo.failTriggerName);
            }
        }
    }


    /// <summary>
    /// 팝업창 열기
    /// </summary>
    /// <param name="popupObj"></param>
    /// <param name="tweenTime"></param>
    private void AlphaPopupOpen(GameObject popupObj, float tweenTime)
    {
        Transform[] tran = popupObj.GetComponentsInChildren<Transform>(true);

        foreach (Transform child in tran)
        {
            if(child.GetComponent<BoxCollider2D>() != null)
            {
                child.gameObject.GetComponent<BoxCollider2D>().enabled = true;
            }
        }

        if(!popupObj.activeSelf)
        {
            popupObj.SetActive(true);
        }

        if (popupObj.GetComponent<UIPanel>() != null)
        {
            TweenManager.tween_Manager.TweenAllDestroy(popupObj);
            TweenManager.tween_Manager.AddTweenAlpha(popupObj, 0, 1, tweenTime);
            TweenManager.tween_Manager.TweenAlpha(popupObj);
        }
        else
        {
            popupObj.SetActive(true);
        }
    }

    /// <summary>
    /// 팝업창 닫기
    /// </summary>
    /// <param name="popupObj"></param>
    /// <param name="tweenTime"></param>
    private void AlphaPopupClose(GameObject popupObj, float tweenTime)
    {
        Transform[] tran = popupObj.GetComponentsInChildren<Transform>(true);

        foreach (Transform child in tran)
        {
            if (child.GetComponent<BoxCollider2D>() != null)
            {
                child.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            }
        }

        if (!popupObj.activeSelf)
        {
            popupObj.SetActive(true);
        }

        if (popupObj.GetComponent<UIPanel>() != null)
        {
            TweenManager.tween_Manager.TweenAllDestroy(popupObj);
            TweenManager.tween_Manager.AddTweenAlpha(popupObj, 1, 0, tweenTime);
            TweenManager.tween_Manager.TweenAlpha(popupObj);
        }
        else
        {
            popupObj.SetActive(false);
        }
    }


    private void EmblemCorStart()
    {
        EmblemCorStop();
        emblemCor = StartCoroutine(EmblemCor());
    }

    private void EmblemCorStop()
    {
        if(emblemCor != null)
        {
            StopCoroutine(emblemCor);
            emblemCor = null;
        }
    }

    private IEnumerator EmblemCor()
    {
        float tweenTime = 1.5f;
        float savedTime = 0;
        emblemBgHeightSaved = FishingGameUI.getInstance.emblemUi.emblemOutsideBg.height;

        AlphaPopupOpen(FishingGameUI.getInstance.emblemUi.emblemUI, tweenTime);
        //TweenManager.tween_Manager.AddTweenScale(FishingGameUI.getInstance.emblemUi.emblemUI,
        //                                         Vector3.one, new Vector3(1.2f, 1.2f, 1.2f), tweenTime);

        //TweenManager.tween_Manager.TweenScale(FishingGameUI.getInstance.emblemUi.emblemUI);

        yield return new WaitForSeconds(tweenTime + 1.0f);

        AlphaPopupClose(FishingGameUI.getInstance.emblemUi.emblemUI, tweenTime);

        while(true)
        {
            yield return new WaitForEndOfFrame();
            savedTime = Time.deltaTime * (emblemBgHeightSaved / tweenTime);// 786.66f;
            FishingGameUI.getInstance.emblemUi.emblemOutsideBg.height -= (int)savedTime;

            if (FishingGameUI.getInstance.emblemUi.emblemUI.GetComponent<UIPanel>().alpha <=0)
            {
                FishingGameUI.getInstance.emblemUi.emblemOutsideBg.height = emblemBgHeightSaved;
                AlphaPopupOpen(FishingGameUI.getInstance.rodUi.rodUI, 0.3f);
                FishingGameUI.getInstance.rodUi.rodBtn.SetActive(true);
                yield break;
            }
        }
    }


    /// <summary>
    /// 이벤트 발생 스프라이트 출력 코루틴 start
    /// </summary>
    private void EventUiTweenCorStart()
    {
       // FishingFloatBite();
        EventUiTweenCorStop();
        eventTweenCor = StartCoroutine(EventTweenCor());
    }

    /// <summary>
    /// 이벤트 발생 스프라이트 출력 코루틴 stop
    /// </summary>
    private void EventUiTweenCorStop()
    {
        if(eventTweenCor != null)
        {
            StopCoroutine(eventTweenCor);
            eventTweenCor = null;
        }
    }

    /// <summary>
    /// leftImg position 변경부분
    /// </summary>
    private void LeftImgMoveStart()
    {
        GameObject leftImgObj = FishingGameUI.getInstance.eventUi.leftTextObj;// watcarBattleUI.m_battleSettingUI.m_playerPortrait;
        Vector3 leftImgFirstPos = FishingGameUI.getInstance.eventUi.leftTextObjFirstPos;
        Vector3 leftImgTargetPos = FishingGameUI.getInstance.eventUi.leftMoveTargetObj.transform.localPosition;

        TweenManager.tween_Manager.TweenAllDestroy(leftImgObj);

        TweenManager.tween_Manager.AddTweenPosition(leftImgObj, leftImgFirstPos,
                                                  new Vector3(leftImgTargetPos.x,//leftImgFirstPos.x + (leftImgObj.GetComponent<UIWidget>().width - 5),
                                                              leftImgFirstPos.y,
                                                              leftImgFirstPos.z),
                                                              0.2f);

        TweenManager.tween_Manager.AddTweenAlpha(leftImgObj, 0, 1, 0.1f);

        TweenManager.tween_Manager.TweenPosition(leftImgObj);
        TweenManager.tween_Manager.TweenAlpha(leftImgObj);
    }

    /// <summary>
    /// RightImg position 변경부분
    /// </summary>
    private void RightImgMoveStart()
    {
        GameObject rightImgObj = FishingGameUI.getInstance.eventUi.rightTextObj;
        Vector3 rightImgFirstPos = FishingGameUI.getInstance.eventUi.rightTextObjFirstPos;
        Vector3 rightImgTargetPos = FishingGameUI.getInstance.eventUi.rightMoveTargetObj.transform.localPosition;

        TweenManager.tween_Manager.TweenAllDestroy(rightImgObj);

        TweenManager.tween_Manager.AddTweenPosition(rightImgObj, rightImgFirstPos,
                                                new Vector3(rightImgTargetPos.x,//rightImgFirstPos.x - (rightImgObj.GetComponent<UIWidget>().width - 5),
                                                            rightImgFirstPos.y,
                                                            rightImgFirstPos.z),
                                                            0.2f);

        TweenManager.tween_Manager.AddTweenAlpha(rightImgObj, 0, 1, 0.1f);

        TweenManager.tween_Manager.TweenPosition(rightImgObj);
        TweenManager.tween_Manager.TweenAlpha(rightImgObj);

    }

    private void RightImgShakeMove()
    {
        GameObject rightImgObj = FishingGameUI.getInstance.eventUi.rightTextObj;
        Vector3 rightImgPos = rightImgObj.transform.localPosition;

        float shakeSensitivity = 10;

        TweenManager.tween_Manager.TweenAllDestroy(rightImgObj);

        TweenManager.tween_Manager.AddTweenPosition(rightImgObj, rightImgPos,
                                                new Vector3(Random.Range(rightImgPos.x, rightImgPos.x + shakeSensitivity),//rightImgFirstPos.x - (rightImgObj.GetComponent<UIWidget>().width - 5),
                                                            Random.Range(rightImgPos.y, rightImgPos.y + shakeSensitivity),
                                                            Random.Range(rightImgPos.z, rightImgPos.z + shakeSensitivity)),
                                                            0.05f, UITweener.Style.PingPong);

        TweenManager.tween_Manager.TweenPosition(rightImgObj);
    }

    /// <summary>
    /// left, right imgobj 숨김
    /// </summary>
    private void MoveImgObjHide()
    {
        GameObject leftImgObj = FishingGameUI.getInstance.eventUi.leftTextObj;
        Vector3 leftImgFirstPos = FishingGameUI.getInstance.eventUi.leftTextObjFirstPos;

        GameObject rightImgObj = FishingGameUI.getInstance.eventUi.rightTextObj;
        Vector3 rightImgFirstPos = FishingGameUI.getInstance.eventUi.rightTextObjFirstPos;


        TweenManager.tween_Manager.TweenAllDestroy(leftImgObj);
        TweenManager.tween_Manager.TweenAllDestroy(rightImgObj);

        TweenManager.tween_Manager.AddTweenPosition(leftImgObj,
                                                    leftImgObj.transform.localPosition,
                                                    leftImgFirstPos,
                                                    0.3f);

        TweenManager.tween_Manager.AddTweenPosition(rightImgObj,
                                                    rightImgObj.transform.localPosition,
                                                    rightImgFirstPos,
                                                    0.3f);


        TweenManager.tween_Manager.AddTweenAlpha(leftImgObj, 1, 0, 0.3f);
        TweenManager.tween_Manager.AddTweenAlpha(rightImgObj, 1, 0, 0.3f);

        TweenManager.tween_Manager.TweenPosition(leftImgObj);
        TweenManager.tween_Manager.TweenPosition(rightImgObj);

        TweenManager.tween_Manager.TweenAlpha(leftImgObj);
        TweenManager.tween_Manager.TweenAlpha(rightImgObj);
    }

    /// <summary>
    /// 이벤트 발생 스프라이트 출력 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator EventTweenCor()
    {
        float delayTime = 0.4f;

        LeftImgMoveStart();

        yield return new WaitForSeconds(delayTime);

        RightImgMoveStart();

        yield return new WaitForSeconds(delayTime/2);

        RightImgShakeMove();

        yield return new WaitForSeconds(delayTime * 1.5f);

        MoveImgObjHide();

        yield return new WaitForSeconds(delayTime * 2);

        StartClickGame();
        yield break;

    }

    /// <summary>
    /// 낚시찌 idle 상태
    /// </summary>
    private void FishingFloatMove()
    {
        floatStatus = FishingFloatStatus.IDLE;
        FishingFloatMoveStart();
    }
  
    /// <summary>
    /// 낚시찌 물고기가 문 상태
    /// </summary>
    private void FishingFloatBite()
    {
        floatStatus = FishingFloatStatus.BITE;
    }
    
    /// <summary>
    /// 낚시찌 낚시 종료 상태
    /// </summary>
    private void FishingFloatEnd()
    {
        floatStatus = FishingFloatStatus.END;
    }

    /// <summary>
    /// 낚시찌 위아래 움직이는 코루틴 start
    /// </summary>
    private void FishingFloatMoveStart()
    {
        FishingFloatMoveStop();
        fishingFloatMoveCor = StartCoroutine(FishingFloatMoveCall());
    }

    /// <summary>
    ///  낚시찌 위아래 움직이는 코루틴 stop
    /// </summary>
    private void FishingFloatMoveStop()
    {
        if(fishingFloatMoveCor != null)
        {
            StopCoroutine(fishingFloatMoveCor);
            fishingFloatMoveCor = null;
        }
    }

    /// <summary>
    /// 낚시 찌 위아래 움직임
    /// </summary>
    /// <returns></returns>
    private IEnumerator FishingFloatMoveCall()
    {
        float fishingFloatMoveRange = 0.25f;

        bool fishingFloatMoveDir = true;

        fishingFloatCursor.transform.position = fishingFloat.transform.position;

        
        fishingFloatCursor.SetActive(true);

        yield return new WaitForSeconds(0.2f);

        fishingFloat.SetActive(true);

        TweenManager.tween_Manager.TweenAllDestroy(fishingFloatCursor);
        TweenManager.tween_Manager.AddTweenScale(fishingFloatCursor,
                                                 Vector3.zero,
                                                 new Vector3(5, 5, 5),
                                                 0.3f,
                                                 UITweener.Style.Once,
                                                 TweenManager.tween_Manager.subTweenScaleCurve
                                                 );
        TweenManager.tween_Manager.TweenScale(fishingFloatCursor);

        while (true)
        {
            yield return new WaitForEndOfFrame();

            if (floatStatus == FishingFloatStatus.IDLE)
            {
                if (fishingFloatMoveDir)
                {
                    fishingFloat.transform.localPosition = new Vector3(fishingFloat.transform.localPosition.x,
                                                   fishingFloat.transform.localPosition.y + (Time.deltaTime * 0.15f),
                                                   fishingFloat.transform.localPosition.z);

                    if (fishingFloat.transform.localPosition.y >= (fishingFloatFirstPos.y + fishingFloatMoveRange))
                    {
                        fishingFloatMoveDir = false;
                    }
                }
                else
                {
                    fishingFloat.transform.localPosition = new Vector3(fishingFloat.transform.localPosition.x,
                                                   fishingFloat.transform.localPosition.y - (Time.deltaTime * 0.15f),
                                                   fishingFloat.transform.localPosition.z);

                    if (fishingFloat.transform.localPosition.y <= (fishingFloatFirstPos.y - fishingFloatMoveRange))
                    {
                        fishingFloatMoveDir = true;
                    }
                }
            }
            else if (floatStatus == FishingFloatStatus.BITE)
            {
                if (fishingFloatMoveDir)
                {
                    fishingFloat.transform.localPosition = new Vector3(fishingFloat.transform.localPosition.x,
                                                   fishingFloat.transform.localPosition.y + (Time.deltaTime + Random.Range(0.01f, 0.02f)),
                                                   fishingFloat.transform.localPosition.z);

                    fishingFloatMoveDir = false;
                }
                else
                {
                    fishingFloat.transform.localPosition = new Vector3(fishingFloat.transform.localPosition.x,
                                                   fishingFloat.transform.localPosition.y - (Time.deltaTime + Random.Range(0.01f, 0.02f)),
                                                   fishingFloat.transform.localPosition.z);

                    fishingFloatMoveDir = true;
                }
            }
            else
            {
                fishingFloat.SetActive(false);
                //fishingFloatCursor.SetActive(false);

                TweenManager.tween_Manager.TweenAllDestroy(fishingFloatCursor);
                TweenManager.tween_Manager.AddTweenScale(fishingFloatCursor,
                                                         fishingFloatCursor.transform.localScale,
                                                         Vector3.zero,
                                                         0.3f,
                                                         UITweener.Style.Once,
                                                         TweenManager.tween_Manager.normalAnimationCurve
                                                         );
                TweenManager.tween_Manager.TweenScale(fishingFloatCursor);
                yield break;
            }
        }
    }

    private void FishingResultCorStart()
    {
        FishingResultCorStop();
        fishingResultCor = StartCoroutine(FishingResultCorCall());
    }

    private void FishingResultCorStop()
    {
        if(fishingResultCor != null)
        {
            StopCoroutine(fishingResultCor);
            fishingResultCor = null;
        }
    }

    private IEnumerator FishingResultCorCall()
    {
        TamiAnimationSet(TamiAniStatus.FISHING);

      //  yield return new WaitForSeconds(2.0f);

        if (gameStatus == GameStatus.FAILED)
        {
            FailedFishing();
            yield break;
        }
        /*
        else
        {
            if (!trapSet)
            {
                FishingSucFishSave((int)fish[fishIdx].species);
                SuccessFishing();
            }
            else
            {
                SuccessTrap();
            }
        }
        */
        yield return new WaitForSeconds(1.0f);

        FishingSuccessPar();

        if (!trapSet)
        {
            float tweenTime = 0.3f;
            GameObject tweenColorTargetObj = fishingFloatFish.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).gameObject;
            TweenManager.tween_Manager.TweenAllDestroy(fishingFloatFish);
            TweenManager.tween_Manager.TweenAllDestroy(tweenColorTargetObj);

            fishingFloatFish.transform.localScale = Vector3.zero;
            fishingFloatFish.transform.position = fishingFloatCursor.transform.position;
            tweenColorTargetObj.GetComponent<SkinnedMeshRenderer>().material.color = Color.white;

            TweenManager.tween_Manager.AddTweenScale(fishingFloatFish, Vector3.zero, new Vector3(5, 5, 5), tweenTime);
            TweenManager.tween_Manager.AddTweenPosition(fishingFloatFish,
                                                        new Vector3(fishingFloatFish.transform.localPosition.x, -0.25f, fishingFloatFish.transform.localPosition.z),
                                                        new Vector3(fishingFloatFish.transform.localPosition.x, 2.5f, fishingFloatFish.transform.localPosition.z),
                                                        tweenTime);

            TweenManager.tween_Manager.TweenScale(fishingFloatFish);
            TweenManager.tween_Manager.TweenPosition(fishingFloatFish);

            yield return new WaitForSeconds(tweenTime + 0.5f);

            TweenManager.tween_Manager.AddTweenColor(tweenColorTargetObj, Color.white, new Color(255, 255, 255, 0), tweenTime);
            TweenManager.tween_Manager.TweenColor(tweenColorTargetObj);
        }
        yield return new WaitForSeconds(1.0f);

        if (!trapSet)
        {
            FishingSucFishSave((int)fish[fishIdx].species);
            SuccessFishing();
        }
        else
        {
            SuccessTrap();
        }

        yield break;

    }


    public void CollectionUiOpen()
    {
        SideMenuOpenPause();
        CollectNewFishStop();
        FishingGameUI.getInstance.collectionUI.collectUI.SetActive(true);

        if (collectTami != null)
        {
            collectTami.GetComponent<Animator>().SetTrigger(fishingMapInfo.tamiInfo.collectTriggerName);
        }
    }


    public void CollectionUiClose()
    {
        SideMenuClosePause();
        CollectNewFishLabelInit();
        FishingGameUI.getInstance.collectionUI.collectUI.SetActive(false);
    }



    /// <summary>
    /// 물고기 한마리만 수집 저장
    /// </summary>
    /// <param name="index"></param>
    private void FishingSucFishSave(int index)
    {
        //0 = 비활성화, 1 = 활성화
        int setActiveInt = 1;
        GameObject getFish = null;

        if(FishingGameUI.getInstance.collectionUI.collectListRoot.transform.GetChild(index).childCount > 0)
        {
            FishingGameUI.getInstance.collectionUI.collectShadowRoot.transform.GetChild(index).gameObject.SetActive(false);
            FishingGameUI.getInstance.collectionUI.collectListRoot.transform.GetChild(index).gameObject.SetActive(true);
            Debug.Log("이미 잡은 물고기 수집됨");
            return;
        }

        float objSize = FishingGameUI.getInstance.collectionUI.collectListRoot.transform.GetChild(index).transform.GetComponent<UIWidget>().width * 2.5f;
        Debug.Log("objSize : " + objSize);

        getFish = Instantiate(fishingMapInfo.fishPrefabs[index].gameObject) as GameObject;
        ChangeLayerName(getFish, "2D UI");
        GlobalDataManager.ShaderRefresh(getFish);

        getFish.transform.parent = FishingGameUI.getInstance.collectionUI.collectListRoot.transform.GetChild(index).transform;
        getFish.transform.localPosition = new Vector3(0, 0, -200);
        getFish.transform.localEulerAngles = new Vector3(0, 90, 0);
        getFish.transform.localScale = new Vector3(objSize, objSize, objSize);

        getFish.SetActive(true);
        FishingGameUI.getInstance.collectionUI.collectShadowRoot.transform.GetChild(index).gameObject.SetActive(false);
        FishingGameUI.getInstance.collectionUI.collectListRoot.transform.GetChild(index).gameObject.SetActive(true);
        
        Debug.Log("저장 물고기 : " + FishingGameUI.getInstance.collectionUI.collectListRoot.transform.GetChild(index).name);
        PlayerPrefs.SetInt(FishingGameUI.getInstance.collectionUI.collectListRoot.transform.GetChild(index).name, setActiveInt);
        PlayerPrefs.Save();

        CollectNewFishStart();
        CollectNewFishLabelCheck(index);
    }

    /// <summary>
    /// 수집한 물고기 불러오기
    /// </summary>
    private void CollectionGet()
    {
        //0 = 비활성화, 1 = 활성화
        int setActiveInt = -1;
        bool already = false;

        CollectNewFishLabelInit();

        for (int i = 0; i < FishingGameUI.getInstance.collectionUI.collectListRoot.transform.childCount; i++)
        {
            already = false;
            setActiveInt = PlayerPrefs.GetInt(FishingGameUI.getInstance.collectionUI.collectListRoot.transform.GetChild(i).name);

            if (setActiveInt > 0)
            {
                GameObject getFish = null;

                if (FishingGameUI.getInstance.collectionUI.collectListRoot.transform.GetChild(i).childCount > 0)
                {
                    Debug.Log("이미 잡은 물고기 수집됨 : " + FishingGameUI.getInstance.collectionUI.collectListRoot.transform.GetChild(i).gameObject.name);
                    already = true;
                }

                if (!already)
                {
                    float objSize = FishingGameUI.getInstance.collectionUI.collectListRoot.transform.GetChild(i).transform.GetComponent<UIWidget>().width * 2.5f;
                    Debug.Log("objSize : " + objSize);
                    getFish = Instantiate(fishingMapInfo.fishPrefabs[i].gameObject) as GameObject;
                    ChangeLayerName(getFish, "2D UI");
                    GlobalDataManager.ShaderRefresh(getFish);

                    getFish.transform.parent = FishingGameUI.getInstance.collectionUI.collectListRoot.transform.GetChild(i).transform;
                    getFish.transform.localPosition = new Vector3(0, 0, -200);
                    getFish.transform.localEulerAngles = new Vector3(0, 90, 0);
                    getFish.transform.localScale = new Vector3(objSize, objSize, objSize);

                    getFish.SetActive(true);
                }
                Debug.Log("있음 : " + FishingGameUI.getInstance.collectionUI.collectListRoot.transform.GetChild(i).gameObject.name);
                FishingGameUI.getInstance.collectionUI.collectShadowRoot.transform.GetChild(i).gameObject.SetActive(false);
                FishingGameUI.getInstance.collectionUI.collectListRoot.transform.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                Debug.Log("없음 : " + FishingGameUI.getInstance.collectionUI.collectListRoot.transform.GetChild(i).gameObject.name);
                FishingGameUI.getInstance.collectionUI.collectShadowRoot.transform.GetChild(i).gameObject.SetActive(true);
                FishingGameUI.getInstance.collectionUI.collectListRoot.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 수집 리스트 옆에 타미 
    /// </summary>
    private void CollectTamiSet()
    {
        if(collectTami!=null)
        {
            Destroy(collectTami);
        }

        float objSize = FishingGameUI.getInstance.collectionUI.collectTamiRoot.GetComponent<UIWidget>().width * 20;
        Debug.Log("objSize : " + objSize);
        collectTami = Instantiate(tamiAnimator.gameObject) as GameObject;
        ChangeLayerName(collectTami, "2D UI");
        GlobalDataManager.ShaderRefresh(collectTami);

        collectTami.transform.parent = FishingGameUI.getInstance.collectionUI.collectTamiRoot.transform;
        collectTami.transform.localPosition = new Vector3(0, 0, -600);
        collectTami.transform.localEulerAngles = new Vector3(0, -180, 0);
        collectTami.transform.localScale = new Vector3(objSize, objSize, objSize);

        collectTami.SetActive(true);
        collectTami.GetComponent<Animator>().SetTrigger(fishingMapInfo.tamiInfo.collectTriggerName);
    }


    private void CollectNewFishStart()
    {
        Vector3 startPos = FishingGameUI.getInstance.collectionUI.collectNewfishObj.transform.localPosition;

        TweenManager.tween_Manager.TweenAllDestroy(FishingGameUI.getInstance.collectionUI.collectNewfishObj);
        TweenManager.tween_Manager.AddTweenAlpha(FishingGameUI.getInstance.collectionUI.collectNewfishObj,
                                                 0.5f,
                                                 1,
                                                 0.4f,
                                                 UITweener.Style.PingPong);

        TweenManager.tween_Manager.AddTweenPosition(FishingGameUI.getInstance.collectionUI.collectNewfishObj,
                                                    startPos,
                                                    new Vector3(startPos.x, startPos.y + 3, startPos.z),
                                                    0.4f,
                                                    UITweener.Style.PingPong);


        TweenManager.tween_Manager.TweenAlpha(FishingGameUI.getInstance.collectionUI.collectNewfishObj);
        TweenManager.tween_Manager.TweenPosition(FishingGameUI.getInstance.collectionUI.collectNewfishObj);
    }


    private void CollectNewFishStop()
    {
        if(FishingGameUI.getInstance.collectionUI.collectNewfishObj.GetComponent<TweenPosition>() == null)
        {
            return;
        }

        Vector3 endPos = FishingGameUI.getInstance.collectionUI.collectNewfishObj.GetComponent<TweenPosition>().from;

        TweenManager.tween_Manager.TweenAllDestroy(FishingGameUI.getInstance.collectionUI.collectNewfishObj);

        TweenManager.tween_Manager.AddTweenAlpha(FishingGameUI.getInstance.collectionUI.collectNewfishObj,
                                                 FishingGameUI.getInstance.collectionUI.collectNewfishObj.GetComponent<UIWidget>().alpha,
                                                 0,
                                                 0.4f);

        TweenManager.tween_Manager.AddTweenPosition(FishingGameUI.getInstance.collectionUI.collectNewfishObj,
                                                    FishingGameUI.getInstance.collectionUI.collectNewfishObj.transform.localPosition,
                                                    endPos,
                                                    0.4f);


        TweenManager.tween_Manager.TweenAlpha(FishingGameUI.getInstance.collectionUI.collectNewfishObj);
        TweenManager.tween_Manager.TweenPosition(FishingGameUI.getInstance.collectionUI.collectNewfishObj);

    }

    private void CollectNewFishLabelInit()
    {
        for(int i=0; i< FishingGameUI.getInstance.collectionUI.collectNewfishLabelRoot.transform.childCount; i++)
        {
            FishingGameUI.getInstance.collectionUI.collectNewfishLabelRoot.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    private void CollectNewFishLabelCheck(int idx)
    {
        FishingGameUI.getInstance.collectionUI.collectNewfishLabelRoot.transform.GetChild(idx).gameObject.SetActive(true);
    }
}
