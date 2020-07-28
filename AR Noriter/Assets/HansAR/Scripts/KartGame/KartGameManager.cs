using UnityEngine;
using System.Collections;
using HedgehogTeam.EasyTouch;
using System.Collections.Generic;
using HansAR;

public class KartGameManager : Singleton<KartGameManager>
{
    private static int ROAD_MOVE_REPEAT_SET = 500;                        // 길 움직임 반복 횟수 셋팅
    private static int SET_KART_SPEED = 40;                               // 카트 속도 셋팅
    private static float SET_BOOSTER_TIME = 3.5f;                         // 부스터 시간 설정

    public enum KartStage
    {
        STAGE1,
        STAGE2,
        STAGE3
    }

    /// <summary>
    /// 맞춰야할 과일 선택
    /// </summary>
    public enum FruitChoice
    {
        APPLE,
        PEACH,
        STRAWBERRY,
        WATERMELON
    }

    public GameObject kartGameUI;
    public GameObject gameRoot;

    public FruitChoice fruitChoice;
    public KartStage kartStage;
    private int stageNum;

    public GameObject carObj;
    public GameObject parkingParent;                                     // 한번 썼던 자동차 주차해두는 공간
    public Camera gameCam;
    public GameObject touchArea;                                         // 터치 영역

    private Vector2 carPos;

    public Transform[] carMovePoint;                                     // 자동차 이동 지점
    private int carMovePointNum = 1;                                     // 자동차 이동 지점 번호 : 0 = 왼쪽, 1 = 센터, 2 = 오른쪽

    public GameObject stageMapParent;
    private GameObject stageMapObj;
    public GameObject[] stageMaps;                                       // 스테이지 맵
    public GameObject[] roadLines;                                       // 차선
    public GameObject disableMapProp;
    private bool isRoadLinesMove;                                        // 차선이 움직이고 있는지 여부

    public float roadLineSpeed;                                          // 차선 움직임 속도
    public int roadMoveRepeat = 0;                                       // 차선 움직인 횟수 카운트

    private Vector3 gameCamOriginPos;
    private  bool fixedCarPos;                                           // 차 위치를 고정시킴
    public GameObject carLocation;                                       // 차 위치 표시
    private Vector3 carLocationPos;
    private Vector3 resetPos;

    public float totalFuel;                                              // 전체 기름양
    public float naviCarLocation = 0;                                    // 내비게이션 상 차 위치

    public Transform fuelGauges;
    private GameObject[] fuelGauge;
    private int fuelGaugeIndex;
    private int currentFuel;

    private Vector3 fuelIndicatorPos;
    private float consumeAmount;

    public GameObject finishPopUp;
    private bool clickedUI;
    public GameObject startNextStageBtn;
    public GameObject endGameBtnParent;

    public bool raceFinished;
    public GameObject moveUI;                                           // 이동 UI

    private bool restartGame = false;                                   // 게임을 재시작하는지 여부
    private int fruitNum;
    public float boostTime;
    private bool onBooster;

    private GameObject targetCar;

    public UILabel stageUI;
    private int countdown;
    public UILabel countdownUI;

    public GameObject boostTimeUI;
    public UILabel boostTimeLabel;

    public GameObject pauseBtn;
    public GameObject restartPopUp;
    public GameObject gameoverUI;

    public Texture2D[] coloredCarTex;

    private Coroutine moveRoadCoroutine;
    private Coroutine countdownCoroutine;

    private bool isModelBundleLoaded = false;
    private bool isMapBundleLoaded = false;


    void Awake()
    {
        kartStage = KartStage.STAGE1;
        InitKartGameManager();
        StartGame();
        //ResetKartGameData();
        //KartItemManager.getInstance.SetKartItemManager(0);
    }

    void OnEnable()
    {
        //StartCoroutine(SetAssetBundleContents());
        //LoadKartMap();

        TargetManager.DelTrackingReadyEvent = AfterBundleLoadEvent;
        EasyTouch.On_TouchUp += OnTouchUp;
    }

    void OnDisable()
    {
        Time.timeScale = 1;
        TargetManager.DelEventMarkerFound = null;
        TargetManager.DelEventMarkerLost = null;
        TargetManager.DelTrackingReadyEvent = null;
        UnsubscribeEvent();
    }

    void OnDestroy()
    {
        UnsubscribeEvent();
    }

    void UnsubscribeEvent()
    {
        EasyTouch.On_TouchUp -= OnTouchUp;
    }

    private int LoadKartMap()
    {
        string mapName = "KartMap";

        HttpRequestDataSet dataSet = null;
        string miniMapBundleName = string.Empty;

        dataSet = new HttpRequestDataSet();
        miniMapBundleName = string.Format("{0}_kartmap", GlobalDataManager.m_ResourceFolderEnum.ToString().ToLower());

        //switch (minimapPaintType)
        //{
        //    case MiniMapPaintType.Paint:
        //    case MiniMapPaintType.Color:
        //        miniMapBundleName = string.Format("{0}_{1}", miniMapBundleName, minimapPaintType.ToString().ToLower());
        //        break;
        //    default:
        //        break;
        //}

        Debug.Log("Bundle Name : " + miniMapBundleName);

        AssetBundleLoader.getInstance.SetAssetBundleLoadInfo(dataSet,
                                                             miniMapBundleName,
                                                             null,
                                                             AssetBundleLoader.getInstance.OnLoadCompleteOnceModeling,
                                                             AppendKartMapModeling,
                                                             null,
                                                             null);

        AssetBundleLoader.getInstance.SetStorageLoadObject(dataSet, mapName, stageMapParent);
        AssetBundleLoader.getInstance.StartLoadAssetBundle(dataSet);

        return 0;
    }

    private void AppendKartMapModeling(HttpRequestDataSet dataSet)
    {
        Debug.Log("Map Loaded");

        stageMapObj = dataSet.OnceModeling;
        stageMapObj.transform.localPosition = Vector3.zero;

        //stageMapObj.layer = LayerMask.NameToLayer("Game");
        stageMapObj.SetActive(true);

        SetStageMap(stageMapObj);

        ResetKartGameData();
        KartItemManager.getInstance.SetKartItemManager(0);

        isMapBundleLoaded = true;
    }

    /// <summary>
    /// 마커 인식 이벤트 셋팅
    /// </summary>
    private void SetMarkerFound(int targetNum)
    {
        if (isModelBundleLoaded && isMapBundleLoaded)
        {
            gameCam.gameObject.SetActive(true);
            gameRoot.SetActive(true);
            clickedUI = false;
            TargetManager.EnableTracking = false;
            AutoFocusMode.getInstance.OnOffAutoFucousMode(false);
            kartGameUI.SetActive(true);
            disableMapProp.SetActive(true);

            SetTargetCar();
            //StartCoroutine(StartCountdown());
            StartCountdownCoroutine();
            //TouchEventManager.터치.기준콜라이더 = TouchEventManager.터치.지형3D;
        }
    }

    private void StartGame()
    {
        //if (isModelBundleLoaded && isMapBundleLoaded)
        //{
            gameCam.gameObject.SetActive(true);
            gameRoot.SetActive(true);
            clickedUI = false;
            TargetManager.EnableTracking = false;
            AutoFocusMode.getInstance.OnOffAutoFucousMode(false);
            kartGameUI.SetActive(true);
            disableMapProp.SetActive(true);

            SetTargetCar();
            //StartCoroutine(StartCountdown());
            StartCountdownCoroutine();
            //TouchEventManager.터치.기준콜라이더 = TouchEventManager.터치.지형3D;
        //}
    }

    private void SetTargetCar()
    {
        targetCar = TargetManager.타깃메니저.GetCurrentCopyModel();
        targetCar.SetActive(false);
        targetCar.transform.parent = carObj.transform;
        targetCar.transform.localPosition = new Vector3(0, -0.5f, 0);
        targetCar.transform.localRotation = Quaternion.Euler(0, 180, 0);
        targetCar.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
    }

    private IEnumerator StartCountdown()
    {
        countdownUI.text = countdown.ToString();
        countdownUI.gameObject.SetActive(true);

        while (countdown > 0)
        {
            yield return new WaitForSeconds(1.0f);
            countdown--;
            countdownUI.text = countdown.ToString();
        }

        countdownUI.text = "START";
        yield return new WaitForSeconds(1.0f);

        countdownUI.gameObject.SetActive(false);
        StartMoveRoadCoroutine();
        StartCoroutine(ActiveMoveUI());
        KartItemManager.getInstance.StartKartItemCoroutine();
        KartItemManager.getInstance.scoreLabel.gameObject.SetActive(true);

        fixedCarPos = false;
        pauseBtn.SetActive(true);
        targetCar.SetActive(true);
        touchArea.SetActive(true);

        AnimationManager.애니메이션.애니메이션02_재생();

    }

    private void StartMoveRoadCoroutine()
    {
        if (moveRoadCoroutine != null)
        {
            StopCoroutine(moveRoadCoroutine);
        }

        moveRoadCoroutine = StartCoroutine(MoveRoad());
    }

    private void SetMarkerLost(int targetNum)
    {
        //TouchEventManager.터치.기준콜라이더 = TouchEventManager.터치.지형2D;
    }

    private void InitKartGameManager()
    {
        gameRoot.SetActive(false);
        gameCam.gameObject.SetActive(false);
        kartGameUI.SetActive(false);
        countdown = 3;
        //KartItemManager.getInstance.scoreLabel.gameObject.SetActive(false);
        pauseBtn.SetActive(false);
        restartPopUp.SetActive(false);
        //stageMaps[0].SetActive(true);

        countdownUI.gameObject.transform.parent.gameObject.SetActive(true);
        touchArea.SetActive(false);
        moveRoadCoroutine = null;
        countdownCoroutine = null;

        disableMapProp.SetActive(false);
    }

    private void ResetKartGameData()
    {
        ChoiceFruit();

        fixedCarPos = true;
        gameCamOriginPos = gameCam.transform.localPosition;
        carLocationPos = carLocation.transform.localPosition;

        roadMoveRepeat = 0;
        totalFuel = 13000;
        currentFuel = (int)totalFuel;

        resetPos = new Vector3(0, 0.1f, 160);
        consumeAmount = 5;
        clickedUI = false;
        raceFinished = false;

        SetGameSpeed();
        SetFuelGaugeUI();

        moveUI.SetActive(false);
        finishPopUp.SetActive(false);
        gameoverUI.SetActive(false);
        boostTimeUI.SetActive(false);

        Time.timeScale = 1;
    }

    private IEnumerator SetAssetBundleContents()
    {
        while (TargetManager.타깃메니저 == null)
        {
            yield return new WaitForSeconds(0.1f);
        }

        HttpRequestDataSet allDataSet = null;

        allDataSet = new HttpRequestDataSet();

        GlobalDataManager.m_ResourceFolderEnum = GlobalDataManager.CategoryType.City;
        GlobalDataManager.m_AssetBundlePartName = "sketch";

        GlobalDataManager.m_SelectedAssetBundleName = string.Format("{0}_{1}", GlobalDataManager.m_ResourceFolderEnum.ToString().ToLower(),
                                                                               GlobalDataManager.m_AssetBundlePartName.ToLower());


        TargetManager.타깃메니저.에셋번들복제컨텐츠 = new GameObject[TargetManager.타깃메니저.컨텐츠모델링이름.Length];

        AssetBundleLoader.getInstance.SetAssetBundleLoadInfo(allDataSet,
                                                           GlobalDataManager.m_SelectedAssetBundleName,
                                                           null,
                                                           AssetBundleLoader.getInstance.OnLoadCompleteModeling,
                                                           AfterModelLoadComplete,
                                                           null,
                                                           null);


        AssetBundleLoader.getInstance.SetStorageLoadObject(allDataSet,
                                                          TargetManager.타깃메니저.컨텐츠모델링이름,
                                                          TargetManager.타깃메니저.에셋번들복제컨텐츠,
                                                          TargetManager.타깃메니저.모델링오브젝트,
                                                          TargetManager.타깃메니저.AR카메라);


        AssetBundleLoader.getInstance.StartLoadAssetBundle(allDataSet);


        
        
        Debug.Log("Bundle Load : Kartgame");
    }

    private void AfterModelLoadComplete(HttpRequestDataSet dataSet)
    {
        //Invoke("SetColoredCarTexture", 2.0f);
        
        isModelBundleLoaded = true;
        StartCoroutine(SetSceneUI());
    }

    private IEnumerator SetSceneUI()
    {
        while (!isModelBundleLoaded || !isMapBundleLoaded)
        {
            yield return new WaitForSeconds(0.5f);
        }

        TargetManager.타깃메니저.StartVuforia();
    }

    private void AfterBundleLoadEvent()
    {
        SetColoredCarTexture();
        MainUI.메인.딜레이팝업UI.SetActive(false);
        ShowRecognizeUI();

        TargetManager.DelEventMarkerFound = SetMarkerFound;
        TargetManager.DelEventMarkerLost = SetMarkerLost;
    }

    private void ShowRecognizeUI()
    {
        MainUI.메인.인식글자UI.SetActive(true);
    }

    /// <summary>
    /// 색칠한 텍스쳐 입힘 ( TODO : 텍스쳐 FBX로 옮기도록 수정할 것 )
    /// </summary>
    private void SetColoredCarTexture()
    {
        GameObject[] bundleCopyContents = TargetManager.타깃메니저.에셋번들복제컨텐츠;

        for (int i = 0; i < bundleCopyContents.Length; i++)
        {
            for (int j = 0; j < coloredCarTex.Length; j++)
            {
                if (bundleCopyContents[i].name.ToLower() == coloredCarTex[j].name.ToLower())
                {
                    bundleCopyContents[i].GetComponent<ColoringInfo>().색칠하기속성.색칠머테리얼.mainTexture = coloredCarTex[j];
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 기름 게이지 UI 셋팅
    /// </summary>
    private void SetFuelGaugeUI()
    {
        fuelGauge = new GameObject[12];

        int i = 0;
        foreach (Transform child in fuelGauges)
        {
            fuelGauge[i] = child.gameObject;
            child.gameObject.SetActive(true);
            i++;
        }

        fuelGaugeIndex = fuelGauge.Length;
        fuelGauges.parent.gameObject.SetActive(true);
    }

    private void ChoiceFruit()
    {
        if (restartGame)
        {
            int previousNum = fruitNum;

            // 재시작시 이전 과일과 다른 과일 고르도록 함
            while (previousNum == fruitNum)
            {
                fruitNum = Random.Range(0, System.Enum.GetValues(typeof(FruitChoice)).Length);
            }
        }
        else
        {
            fruitNum = Random.Range(0, System.Enum.GetValues(typeof(FruitChoice)).Length);
            restartGame = true;
        }

        fruitChoice = (FruitChoice)fruitNum;
    }

    private void SetGameSpeed()
    {
        stageNum = (int)kartStage;

        if (stageNum > 1)
        {
            SET_KART_SPEED = 45;
        }
        else
        {
            SET_KART_SPEED = 40;
        }
    }

    public void OnClickStartGameBtn()
    {
        clickedUI = true;
        TargetManager.EnableTracking = false;
        AutoFocusMode.getInstance.OnOffAutoFucousMode(false);
        fixedCarPos = false;
        StartMoveRoadCoroutine();
        StartCoroutine(ActiveMoveUI());
        KartItemManager.getInstance.StartKartItemCoroutine();


    }

    /// <summary>
    /// 다음 스테이지 시작 버튼
    /// </summary>
    public void OnClickStartNextStage()
    {
        StopAllCoroutines();

        stageNum++;
        kartStage = (KartStage)stageNum;
        stageUI.text = kartStage.ToString();
        //restartGame = true;
        ResetKartGameData();
        KartItemManager.getInstance.StartKartItemManager(stageNum);
        ResetStageMap(stageNum);

        clickedUI = true;
        TargetManager.EnableTracking = false;
        AutoFocusMode.getInstance.OnOffAutoFucousMode(false);
        fixedCarPos = false;
        boostTime = 0;
        onBooster = false;
        StartMoveRoadCoroutine();
        StartCoroutine(ActiveMoveUI());
    }

    public void OnClickRegame()
    {
        StopAllCoroutines();

        stageNum = 0;
        kartStage = 0;
        stageUI.text = kartStage.ToString();
        KartItemManager.instance.score = 0;
        ResetKartGameData();
        KartItemManager.getInstance.StartKartItemManager(stageNum);
        ResetStageMap(stageNum);


        clickedUI = true;
        boostTime = 0;
        onBooster = false;
        TargetManager.EnableTracking = false;
        AutoFocusMode.getInstance.OnOffAutoFucousMode(false);
        fixedCarPos = false;
        StartMoveRoadCoroutine();
        StartCoroutine(ActiveMoveUI());
    }

    public void OnClickEndGame()
    {
        StopAllCoroutines();
        stageNum = 0;
        kartStage = 0;
        stageUI.text = kartStage.ToString();
        KartItemManager.instance.score = 0;
        KartItemManager.getInstance.DeactiveItems();
        InitKartGameManager();
        ResetKartGameData();

        KartItemManager.getInstance.StartKartItemManager(0);
        ResetStageMap(0);
        KartItemManager.instance.StopAllCoroutines();
        StopAllCoroutines();

        clickedUI = true;
        boostTime = 0;
        onBooster = false;
        fixedCarPos = true;

        targetCar.SetActive(false);
        finishPopUp.SetActive(false);
        
        TargetManager.EnableTracking = true;
        AutoFocusMode.getInstance.OnOffAutoFucousMode(true);
        ShowRecognizeUI();

        // 사용한 자동차 주차공간에 이동시켜놓음
        targetCar.transform.parent = parkingParent.transform;
        targetCar.transform.localPosition = Vector3.zero;

        MoveCarToCenter();
        //gameRoot.SetActive(false);
    }

    private void OnTouchUp(Gesture ges)
    {
        if (SideMenuUI.Instance.sideMenuStatus == SideMenuUI.SideMenuStatus.None)
        {
            if (!clickedUI && ges.pickedObject != null && !fixedCarPos) //&& UICamera.hoveredObject.name == "UI Root")
            {
                carPos = gameCam.WorldToScreenPoint(carObj.transform.position);

                if (ges.position.x < carPos.x)
                {
                    //Debug.Log("자동차 왼쪽 이동");
                    MoveToPoint(true);
                }
                else
                {
                    //Debug.Log("자동차 오른쪽 이동");
                    MoveToPoint(false);
                }
            }

            if (clickedUI)
            {
                clickedUI = false;
            }
        }
        else
        {
            Time.timeScale = 0;
        }
    }

    /// <summary>
    /// 자동차를 가운데로 이동시킴 (리셋용)
    /// </summary>
    private void MoveCarToCenter()
    {
        if (carMovePointNum == 0)
        {
            MoveToPoint(false);
        }
        else if (carMovePointNum == 2)
        {
            MoveToPoint(true);
        }
    }

    /// <summary>
    /// 맵포인트로 이동
    /// </summary>
    /// <param name="moveLeft">왼쪽 이동 여부</param>
    private void MoveToPoint(bool moveLeft)
    {
        if (moveLeft)
        {
            if (carMovePointNum > 0)
            {
                carMovePointNum--;
            }
            else
            {
                return;
            }
        }
        else
        {
            if (carMovePointNum < carMovePoint.Length - 1)
            {
                carMovePointNum++;
            }
            else
            {
                return;
            }
        }

        KartPlayerManager.getInstance.SwitchCar(carMovePoint[carMovePointNum]);
    }

    #region Map 관련 함수

    /// <summary>
    /// 도로를 움직입니다.
    /// </summary>
    private IEnumerator MoveRoad()
    {
        roadLineSpeed = 0;

        while (true)
        {
            // 시작할때 속도 서서히 올라가는 효과
            if (roadLineSpeed < SET_KART_SPEED)
            {
                roadLineSpeed += 0.3f;
            }

            // 차선 움직임
            for (int i = 0; i < roadLines.Length; i++)
            {
                roadLines[i].transform.Translate(Vector3.back * roadLineSpeed * Time.deltaTime);

                if (roadLines[i].transform.position.z < -20)
                {
                    roadLines[i].transform.localPosition = resetPos;
                    roadMoveRepeat++;
                }

                if (!raceFinished)
                {
                    KartItemManager.getInstance.DisposeItem(roadLines[i], roadMoveRepeat);
                }
            }

            //SetNaviLocation();
            ConsumeFuel();

            yield return new WaitForFixedUpdate();
        }
    }

    public void CrashCar()
    {
        fixedCarPos = true;
        StopAllCoroutines();
        StartCoroutine(ShakeCam());
        boostTime = 0;
        StartCoroutine(CarBooster());

        Invoke("StartMoveRoad", 1.0f);
    }

    public void StartMoveRoad()
    {
        roadLineSpeed = 0;
        StartMoveRoadCoroutine();
        fixedCarPos = false;
    }

    /// <summary>
    /// 카메라를 흔듭니다.
    /// </summary>
    private IEnumerator ShakeCam()
    {
        float timer = 0;
        float amount = 0.5f;                // 흔드는 정도
        float duration = 0.5f;              // 지속 시간

        while (timer <= duration)
        {
            gameCam.transform.localPosition = (Vector3)Random.insideUnitCircle * amount + gameCamOriginPos;

            timer += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        gameCam.transform.localPosition = gameCamOriginPos;
    }

    public void OnClickBoosterBtn()
    {
        clickedUI = true;

        PlayBooster();
    }

    public void PlayBooster()
    {
        if (onBooster)
        {
            boostTime += SET_BOOSTER_TIME;
        }
        else
        {
            boostTime += SET_BOOSTER_TIME;
            StartCoroutine(CarBooster());
        }
    }

    public IEnumerator CarBooster()
    {
        onBooster = true;
        boostTimeUI.SetActive(true);

        if (stageNum > 1)
        {
            roadLineSpeed = 80f;
        }
        else
        {
            roadLineSpeed = 100f;
        }

        while (boostTime > 0)
        {
            boostTimeLabel.text = boostTime.ToString("N2");
            boostTime -= Time.deltaTime;

            yield return new WaitForFixedUpdate();
        }

        roadLineSpeed = SET_KART_SPEED;
        onBooster = false;
        boostTimeUI.SetActive(false);
    }

    /// <summary>
    /// 기름 소모
    /// </summary>
    private void ConsumeFuel()
    {
        if (!raceFinished)
        {
            if (totalFuel > 0)
            {
                consumeAmount = roadLineSpeed / 10;
                totalFuel -= consumeAmount;
                ApplyFuelGauges();
            }
            else
            {
                gameoverUI.SetActive(true);
                Time.timeScale = 0;
                raceFinished = true;
                //Debug.Log("===GAME OVER=== 기름이 떨어졌습니다");
            }
        }
    }

    /// <summary>
    /// 기름 아이템 획득
    /// </summary>
    public void GetFuelItem()
    {
        totalFuel += 4000;

        if (totalFuel > 10000)
        {
            totalFuel = 13000;
        }
    }

    private void ApplyFuelGauges()
    {
        currentFuel = (int)(totalFuel / 1000);

        if (fuelGaugeIndex > currentFuel)
        {
            fuelGauge[12 - fuelGaugeIndex].SetActive(false);
            fuelGaugeIndex = currentFuel;
        }
        else if (fuelGaugeIndex < currentFuel)
        {
            for (int i = 0; i < fuelGauge.Length; i++)
            {
                if (11 - i < currentFuel)
                {
                    fuelGauge[i].SetActive(true);
                }
                else
                {
                    fuelGauge[i].SetActive(false);
                }
            }

            fuelGaugeIndex = currentFuel;
        }
    }

    /// <summary>
    /// 네비게이션의 차 위치를 설정함
    /// </summary>
    private void SetNaviLocation()
    {
        naviCarLocation = (float)roadMoveRepeat * 4 - 200;
        carLocationPos.x = naviCarLocation;
        carLocation.transform.localPosition = carLocationPos;
    }

    /// <summary>
    /// 경주 완료 이벤트
    /// </summary>
    public void RaceFinishEvent()
    {
        ShowFinishPopup();
        fuelGauges.parent.gameObject.SetActive(false);
        fixedCarPos = true;
        raceFinished = true;
        KartItemManager.getInstance.DeactiveItems();
        MoveCarToCenter();
        roadLineSpeed = SET_KART_SPEED;
    }

    public void ShowFinishPopup()
    {
        if (stageNum < 2)
        {
            startNextStageBtn.SetActive(true);
            endGameBtnParent.SetActive(false);
        }
        else
        {
            startNextStageBtn.SetActive(false);
            endGameBtnParent.SetActive(true);
        }

        finishPopUp.SetActive(true);
    }

    /// <summary>
    /// 해당 시간동안 3D 터치를 막아둠
    /// </summary>
    private IEnumerator Prevent3dTouch(float time)
    {
        fixedCarPos = true;

        yield return new WaitForSeconds(time);

        fixedCarPos = false;
    }

    public void OnClickPauseBtn()
    {
        Time.timeScale = 0;
        restartPopUp.SetActive(true);
        clickedUI = true;
    }

    public void OnClickCancelBtn()
    {
        CloseRestartPopUp();
        clickedUI = true;
    }

    public void OnClickRestartBtn()
    {
        CloseRestartPopUp();
        OnClickEndGame();
        clickedUI = true;

        KartItemManager.getInstance.scoreLabel.gameObject.SetActive(false);
    }

    private void CloseRestartPopUp()
    {
        Time.timeScale = 1;
        restartPopUp.SetActive(false);
        clickedUI = true;
    }

    public int GetRoadLineSpeedValue()
    {
        int value = (int)roadLineSpeed;
        return value;
    }

    /// <summary>
    /// 이동키 UI를 활성화 시킴 (지정 시간 후 Fade Out 적용)
    /// </summary>
    /// <returns></returns>
    private IEnumerator ActiveMoveUI()
    {
        moveUI.SetActive(true);

        yield return new WaitForSeconds(15f);

        float alpha = moveUI.GetComponent<UISprite>().color.a;
        Color col;
        while (alpha > 0)
        {
            alpha -= (1 * Time.deltaTime);
            col = new Color(0, 0, 0, alpha);
            moveUI.GetComponent<UISprite>().color = col;
            yield return new WaitForFixedUpdate();
        }

        moveUI.SetActive(false);
    }

    public int GetFruitChoiceNum()
    {
        int fruitChoiceNum = (int)fruitChoice;
        return fruitChoiceNum;
    }

    public void OnClickPreventPreventBG()
    {
        clickedUI = false;
    }

    /// <summary>
    /// 자동차 움직임 방지
    /// </summary>
    public void PreventKartMove()
    {
        clickedUI = true;

        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
    }

    public void OnClickRestartPopUpCol()
    {
        CloseRestartPopUp();
    }

    public void OnClickRestartPopUp()
    {
        clickedUI = true;
    }

    private void SetStageMap(GameObject stageObj)
    {
        KartGameMapInfo info = stageObj.GetComponent<KartGameMapInfo>();

        stageMaps = info.stageMaps;
        roadLines = info.RoadLines;
        KartItemManager.getInstance.SetKartItem(info);
        coloredCarTex = info.coloredTex;

        ResetStageMap(0);
    }

    public void ResetStageMap(int stageNum)
    {
        for (int i = 0; i < stageMaps.Length; i++)
        {
            if (i == stageNum)
            {
                stageMaps[i].SetActive(true);
            }
            else
            {
                stageMaps[i].SetActive(false);
            }
        }
    }

    private void StartCountdownCoroutine()
    {
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
        }

        countdownCoroutine = StartCoroutine(StartCountdown());
    }

    #endregion
}
