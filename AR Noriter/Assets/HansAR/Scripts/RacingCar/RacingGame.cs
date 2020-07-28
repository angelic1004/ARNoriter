using UnityEngine;
using System.Collections;

public class RacingGame : MonoBehaviour
{
    #region 변수 선언 부분

    /// <summary>
    /// 내차를 표시할 Label을 보여줄 카메라
    /// </summary>
    public Camera contentsCamera;

    /// <summary>
    /// RacingGame 스크립트를 담고, 자신을 껐다키기 위한 오브젝트
    /// </summary>
    public GameObject RacingGameUI;

    /// <summary>
    /// 컨텐츠를 복사한 오브젝트를 담을 부모오브젝트
    /// </summary>
    public GameObject racingObjParent;

    /// <summary>
    /// 복사한 오브젝트
    /// </summary>
    private GameObject[] racingCar;

    /// <summary>
    /// 터치할 스크린 영역
    /// </summary>
    public GameObject[] touchScreenObj;

    /// <summary>
    /// 게임 스타트 버튼
    /// </summary>
    public GameObject startBtn;

    /// <summary>
    /// 진행도 미니맵 배경
    /// </summary>
    public GameObject minimapBack;

    /// <summary>
    /// 진행도 미니맵 차
    /// </summary>
    //public GameObject minimapCar;

    /// <summary>
    /// 일시정지 팝업
    /// </summary>
    public GameObject pausePopup;

    /// <summary>
    /// 시작시 스타트 신호등
    /// </summary>
    public GameObject startLightLantern;

    /// <summary>
    /// 시작시 스타트 신호등 빛
    /// </summary>
    public GameObject[] startLight;

    /// <summary>
    /// 날라오는 Start 이미지
    /// </summary>
    public GameObject startSprite;

    /// <summary>
    /// 결과창 이미지
    /// </summary>
    public GameObject resultText;

    /// <summary>
    /// 일시정지 버튼
    /// </summary>
    public GameObject pauseBtn;

    /// <summary>
    /// 다음 레벨 버튼
    /// </summary>
    public GameObject nextLevelBtn;

    /// <summary>
    /// 게임 재시작 버튼
    /// </summary>
    public GameObject restartBtn;

    /// <summary>
    /// 차들의 위치값
    /// </summary>
    public Transform[] carPos;

    /// <summary>
    /// 1등시 움직일 좌표값
    /// </summary>
    public Transform victoryPos;

    /// <summary>
    /// 현재 레벨 Label
    /// </summary>
    public UILabel levelLabel;

    /// <summary>
    /// 내 차 Label
    /// </summary>
    public UILabel myCarLabel;

    /// <summary>
    /// 차의 회전값을 저장할 벡터값
    /// </summary>
    public Vector3 carEulerAngle;

    // 소리를 재생할 AudioSource
    public AudioSource bgmAudio;
    public AudioSource effectAudio;

    //각종 사운드
    [HideInInspector]
    public AudioClip[] engineSound;
    [HideInInspector]
    public AudioClip startSound;
    [HideInInspector]
    public AudioClip accelSound;
    [HideInInspector]
    public AudioClip decelSound;
    [HideInInspector]
    public AudioClip finishSound;
    [HideInInspector]
    public AudioClip victorySound;
    [HideInInspector]
    public AudioClip gameoverSound;

    /// <summary>
    /// 최대 차의 갯수(자신의 차 포함)
    /// </summary>
    public int carAmount;

    /// <summary>
    /// 차의 스피드(인스펙터에서 조정)
    /// </summary>
    public float carSpeed;

    /// <summary>
    /// 총 플레이 길이 또는 거리(인스펙터에서 조정)
    /// </summary>
    public float maxMeter;

    /// <summary>
    /// 터치 이벤트가 발생하는 타임워치(인스펙터에서 조정)
    /// </summary>
    public float eventTime;

    /// <summary>
    /// 현재 터치 이벤트 타임워치
    /// </summary>
    private float currentEventTime;

    /// <summary>
    /// 터치 이벤트 제한시간(인스펙터에서 조정)
    /// </summary>
    public float eventLimitTime;

    /// <summary>
    /// 현재 터치 이벤트 제한시간
    /// </summary>
    private float currentEventLimitTime;

    /// <summary>
    /// 이벤트 성공시 한번에 얼마나 움직일지를 정하는 값(인스펙터에서 조정)
    /// </summary>
    public float successMoveDistance;

    /// <summary>
    /// 이벤트 실패시 한번에 얼마나 움직일지를 정하는 값(인스펙터에서 조정)
    /// </summary>
    public float failMoveDistance;

    /// <summary>
    /// 터치이벤트시 움직여줄 차의 인덱스
    /// </summary>
    private int moveIndex = -1;

    /// <summary>
    /// 터치이벤트를 실패했을때 쓸 bool값
    /// </summary>
    private bool eventFail = false;

    /// <summary>
    /// 게임 현재 레벨
    /// </summary>
    private int currentLevel = 1;

    /// <summary>
    /// 게임 최대 레벨
    /// </summary>
    public int maxLevel;

    /// <summary>
    /// 일시정지 중인지
    /// </summary>
    private bool pausedGame = false;

    [HideInInspector]
    /// <summary>
    /// 게임중인지
    /// </summary>
    public bool isGaming = false;

    /// <summary>
    /// 이겼는지 졌는지
    /// </summary>
    private bool isWin = false;

    /// <summary>
    /// true = 다음난이도 // false = 재시작
    /// </summary>
    private bool gameSetting = false;

    public static RacingGame instance;

    /// <summary>
    /// 외부에서 init을 호출하기 위한 델리게이트
    /// </summary>
    public delegate void InitDel();

    /// <summary>
    /// 델리게이트 선언
    /// </summary>
    public InitDel init;
    #endregion

    void Awake()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = false;

        instance = this;
    }

    void Start()
    {
        //델리게이트를 초기화
        init = new InitDel(InitRacingGame);

        //UI를 꺼줍니다.
        if (RacingGameUI != null)
        {
            RacingGameUI.SetActive(false);
        }

        bgmAudio.Stop();
        //사용할 차의 갯수 만큼 담을 오브젝트의 배열 크기를 지정
        racingCar = new GameObject[carAmount];

        startLight = new GameObject[startLightLantern.transform.childCount];
        for (int i = 0; i < startLightLantern.transform.childCount; i++)
        {
            startLight[i] = startLightLantern.transform.GetChild(i).gameObject;
            startLight[i].SetActive(false);
        }
    }

    #region 게임 초기화 부분

    /// <summary>
    /// 레이싱 게임 초기화 부분
    /// </summary>
    private void InitRacingGame()
    {
        //일시정지 팝업이 뜬상태라면 무시
        if (pausedGame == false)
        {
            //인식시
            if (TargetManager.trackableStatus)
            {
                contentsCamera.gameObject.SetActive(false);

                //모든 코루틴을 꺼줌
                StopAllCoroutines();

                isGaming = false;

                //터치이벤트를 켜줌
                TouchEventManager.터치.enabled = true;

                startSprite.GetComponent<TweenPosition>().enabled = false;
                startLightLantern.GetComponent<TweenPosition>().enabled = false;

                MainUI.메인.UIOpen(MainUI.메인.uiEventLinkManager);

                for (int i = 0; i < racingCar.Length; i++)
                {
                    if (racingCar[i] != null)
                    {
                        //복사한 오브젝트를 삭제시켜주고, 오브젝트를 담았던 배열도 초기화,터치 콜라이더도 비활성화로 변경
                        Destroy(racingCar[i]);
                        racingCar[i] = null;
                        touchScreenObj[i].SetActive(false);
                    }
                }

                for (int i = 0; i < startLight.Length; i++)
                {
                    startLight[i].SetActive(false);
                }

                nextLevelBtn.SetActive(false);
                restartBtn.SetActive(false);
                pauseBtn.SetActive(false);
                resultText.SetActive(false);
                RacingGameUI.SetActive(false);
            }
            else
            {
                if (TargetManager.타깃메니저.첫인식상태)
                {
                    contentsCamera.gameObject.SetActive(true);

                    //터치는 사용하지 않으므로 비활성화 시켜줍니다.
                    TouchEventManager.터치.enabled = false;

                    RacingGameUI.SetActive(true);

                    //불필요한 UI는 모두 꺼줍니다.
                    RotateUI.회전.회전UI_숨기기();
                    MainUI.메인.UIClose(MainUI.메인.애니동작UI);
                    MainUI.메인.UIClose(MainUI.메인.uiEventLinkManager);
                    MainUI.메인.UIClose(MainUI.메인.오버레이UI);

                    //시작버튼 활성화
                    startBtn.SetActive(true);

                    currentLevel = 1;
                    levelLabel.text = string.Format("Level " + currentLevel);

                    currentEventTime = eventTime;
                    currentEventLimitTime = eventLimitTime;

                    //적 차의 인덱스 번호를 랜덤으로 받을 배열
                    int[] enemyIndex = new int[carAmount - 1];

                    for (int i = 0; i < enemyIndex.Length; i++)
                    {
                        //인덱스 세팅
                        if (i == 0)
                        {
                            enemyIndex[i] = SelectEnemyCar();
                        }
                        else
                        {
                            //차의 수가 더 늘어날경우 수정 필요한 부분
                            enemyIndex[i] = SelectEnemyCar(enemyIndex[0]);
                        }
                    }

                    //받아온 인덱스의 차를 복사
                    for (int i = 0; i < racingCar.Length; i++)
                    {
                        if (i == 0)
                        {
                            racingCar[i] = Object.Instantiate(TargetManager.타깃메니저.에셋번들복제컨텐츠[TargetManager.타깃메니저.복제모델링인덱스]);
                        }
                        else
                        {
                            racingCar[i] = Object.Instantiate(TargetManager.타깃메니저.에셋번들복제컨텐츠[enemyIndex[i - 1]]);
                        }

                        //지정한 위치로 차를 옮겨,활성화 후 transform 지정
                        racingCar[i].transform.parent = racingObjParent.transform;
                        racingCar[i].SetActive(true);

                        racingCar[i].layer = 15;
                        racingCar[i].transform.SetChildLayer(15);

                        CarTransformSetting(racingCar[i], carPos[i]);
                        racingCar[i].GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animation>().Stop();
                    }

                    startSprite.GetComponent<TweenPosition>().from = startSprite.transform.localPosition;
                    startSprite.GetComponent<TweenPosition>().from.x = -(startSprite.GetComponent<UISprite>().localSize.x + (Screen.width / 2));
                    startSprite.GetComponent<TweenPosition>().to = startSprite.transform.localPosition;
                    startSprite.GetComponent<TweenPosition>().to.x = 0;
                    startSprite.GetComponent<TweenPosition>().ResetToBeginning();

                    startLightLantern.GetComponent<TweenPosition>().ResetToBeginning();
                    //미니맵 차의 위치 지정
                    //minimapCar.transform.position = minimapStartPos.transform.position;

                    myCarLabel.text = "My Car";

                    Vector3 pos = UICamera.mainCamera.ViewportToWorldPoint(contentsCamera.WorldToViewportPoint(racingCar[0].transform.position));

                    pos.y += racingCar[0].transform.localScale.y / 4;

                    myCarLabel.transform.position = pos;
                }
            }
        }
    }

    /// <summary>
    /// 1번 적차 세팅
    /// </summary>
    /// <returns>배열 번호 반환</returns>
    private int SelectEnemyCar()
    {
        int num;

        do
        {
            //랜덤 값을 저장
            num = Random.Range(0, TargetManager.타깃메니저.컨텐츠모델링이름.Length - 1);
        }
        //자신의 차(타겟으로 비춘 차량)와 인덱스 번호가 같지않다면 빠져나옴
        while (num == TargetManager.타깃메니저.복제모델링인덱스);

        return num;
    }

    /// <summary>
    /// 2번 적차 세팅
    /// </summary>
    /// <param name="exceptNum">1번 적차의 인덱스 번호</param>
    /// <returns>배열 번호 반환</returns>
    private int SelectEnemyCar(int exceptNum)
    {
        int num;

        do
        {
            //랜덤 값을 저장
            num = Random.Range(0, TargetManager.타깃메니저.컨텐츠모델링이름.Length);

        }
        //자신의 차(타겟으로 비춘 차량) 또는  1번 적차와 인덱스 번호가 같지않다면 빠져나옴
        while (num == TargetManager.타깃메니저.복제모델링인덱스 || exceptNum == num);

        return num;
    }

    /// <summary>
    /// 복제된 차의 Transform을 이동시켜줌
    /// </summary>
    /// <param name="settingCar">세팅할 복제 차량</param>
    /// <param name="transform">이동시킬 Transform</param>
    private void CarTransformSetting(GameObject settingCar, Transform transform)
    {
        settingCar.transform.position = transform.position;
        settingCar.transform.localScale = transform.localScale;

        //회전값만 지정한 회전을 사용
        settingCar.transform.eulerAngles = carEulerAngle;
    }
    #endregion

    #region 게임 기능 관련 부분
    /// <summary>
    /// 스타트버튼을 눌렀을때 들어오는 메서드
    /// </summary>
    public void GameStartEvent()
    {
        //스타트버튼은 끄고, 실질적인 이벤트가 있는 코루틴을 실행
        startBtn.SetActive(false);
        StartCoroutine("RacingStart");
    }



    /// <summary>
    /// 스크린 터치이벤트가 정상적으로 터치하였을때 발생하는 메서드
    /// </summary>
    public void ScreenTouch()
    {
        //터치했을때 실질적으로 이벤트를 이행하는 코루틴
        StartCoroutine("ScreenTouchEvent");

        //제한시간을 체크하고있을 코루틴을 종료
        StopCoroutine("ScreenTouchStart");
    }


    /// <summary>
    /// 게임 일시정지
    /// </summary>
    public void GamePause()
    {
        //컨텐츠를 안보이게끔
        TargetManager.타깃메니저.HideAllModelingContents();

        //일시정지 팝업 오픈
        MainUI.메인.PausePopupOpen();

        //일시정지중..
        pausedGame = true;
    }

    /// <summary>
    /// Retry 버튼 클릭
    /// </summary>
    public void RetryBtnClick()
    {
        //코루틴 정지
        StopAllCoroutines();

        EffectSoundManager.이펙트.이펙트사운드_중지();

        //일시정지상태를 풀어주고,게임중이었기떄문에 게임중 bool값을 false로 변경
        pausedGame = false;
        isGaming = false;

        //일시정지 팝업창이 뜬상태로 인식상태 그대로 Retry를 했을경우, 
        //타겟에서 카메라를 떼면 바로 비인식으로 넘어가는 문제가 있어 첫인식상태로 변환
        TargetManager.타깃메니저.첫인식상태 = false;

        //일시정지 팝업을 꺼줌
        pausePopup.SetActive(false);
        pauseBtn.SetActive(false);

        //다시 인식해야하므로 인식글자를 띄워줌
        MainUI.메인.인식글자UI.SetActive(true);

        //레이싱게임 ui를 닫아줌
        RacingGameUI.SetActive(false);

        //인식후 팝업이 뜨기떄문에 UI가 켜지므로 UI 초기상태로 돌립니다.
        MainUI.메인.UIClose(MainUI.메인.애니동작UI);
        MainUI.메인.UIClose(MainUI.메인.uiEventLinkManager);
        MainUI.메인.UIClose(MainUI.메인.오버레이UI);
        RotateUI.회전.회전UI_숨기기();

        //게임중이었기 때문에 레이싱게임 관련부분을 초기화
        for (int i = 0; i < racingCar.Length; i++)
        {
            if (racingCar[i] != null)
            {
                //복사한 오브젝트를 삭제시켜주고, 오브젝트를 담았던 배열도 초기화,터치 콜라이더도 비활성화로 변경
                Destroy(racingCar[i]);
                racingCar[i] = null;
                touchScreenObj[i].SetActive(false);
            }
        }

        MainUI.메인.PausePopupClose();
    }

    /// <summary>
    /// 일시정지 취소 버튼 클릭
    /// </summary>
    public void CancleBtnClick()
    {
        TargetManager.trackableStatus = false;

        MainUI.메인.PausePopupClose();

        pausedGame = false;
    }

    /// <summary>
    /// Start 할때 start가 날라오는 부분을 두번으로 나눔(가운데서 한번 멈춰야 하므로)
    /// </summary>
    public void StartSpriteNextFoward()
    {
        StartCoroutine("StartSpriteMove");
    }

    public void NextLevelBtnClick()
    {
        StopAllCoroutines();
        gameSetting = true;
        StartCoroutine("GameReset");
    }

    public void RestartBtnClick()
    {
        StopAllCoroutines();
        gameSetting = false;
        StartCoroutine("GameReset");
    }

    #endregion

    #region 코루틴 함수
    /// <summary>
    /// 스타트버튼을 눌렀을때 시작되는 코루틴(GameStartEvent에서 넘어옴)
    /// </summary>
    private IEnumerator RacingStart()
    {
        //준비 신호등 이벤트
        effectAudio.clip = startSound;
        effectAudio.Play();
        yield return new WaitForSeconds(1.0f);
        startLight[0].SetActive(true);
        yield return new WaitForSeconds(1.0f);
        startLight[1].SetActive(true);
        yield return new WaitForSeconds(1.0f);
        startLight[2].SetActive(true);
        yield return new WaitForSeconds(1.0f);

        isGaming = true;

        bgmAudio.Play();

        pauseBtn.SetActive(true);
        //Start Text를 tween으로 움직여 애니메이션처럼 이동
        startSprite.GetComponent<TweenPosition>().PlayForward();
        startLightLantern.GetComponent<TweenPosition>().PlayForward();
        yield return new WaitForSeconds(1.0f);

        for (int index = 0; index < racingCar.Length; index++)
        {
            ModelInfo info = racingCar[index].GetComponent<ModelInfo>();
            info.애니메이션타겟.GetComponent<Animation>().clip = info.애니메이션정보.애니클립[0];
            info.애니메이션타겟.GetComponent<Animation>().wrapMode = WrapMode.Loop;
            info.애니메이션타겟.GetComponent<Animation>().Play();
        }

        racingCar[0].GetComponent<ModelInfo>().이펙트클립 = engineSound[Random.Range(0, engineSound.Length)];
        EffectSoundManager.이펙트.이펙트사운드_재생(racingCar[0]);

        //차의 스피드를 저장
        float i = carSpeed;

        //고정된 시간마다 실행할 스크린터치이벤트를 위한 임시시계
        float time = 0;

        int minimapPoint = 1;

        //Lerp가 1.0f까지 갈때까지
        while (i / maxMeter <= 1.0f)
        {
            try
            {
                if (pausedGame == false)
                {
                    if ((maxMeter / 25) * minimapPoint <= i)
                    {
                        minimapBack.GetComponent<UISprite>().spriteName =
                            string.Format("RacingGameMap" + minimapPoint);
                        minimapPoint++;
                    }

                    //임시시계가 지정한 시간이 될때마다 이벤트 발생
                    if (time >= currentEventTime)
                    {
                        StartCoroutine("ScreenTouchStart");
                        time = 0;
                    }

                    //i값은 계속 커져 미니맵 차가 이동하는것처럼 보임
                    i += carSpeed;

                    //임시 시계가 움직임
                    time += Time.deltaTime;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex);
                throw;
            }

            yield return new WaitForFixedUpdate();
        }

        //함수를 빠져나왔다면, 종료이벤트가 발생
        StartCoroutine("GameEndEvent");
    }

    /// <summary>
    /// 지정한 시간마다 발생하는 터치이벤트 (RacingStart 에서 넘어옴)
    /// </summary>
    private IEnumerator ScreenTouchStart()
    {
        //활성화할 오브젝트를 지정, 활성화
        moveIndex = Random.Range(0, touchScreenObj.Length);
        touchScreenObj[moveIndex].SetActive(true);
        touchScreenObj[moveIndex].transform.parent.GetComponent<UISprite>().spriteName = "4dgame_touchBtn_active";

        //제한시간 5초
        yield return new WaitForSeconds(currentEventLimitTime);

        //5초가 지났다면 미션실패 bool값을 true로 변경 이동 코루틴을 발생
        eventFail = true;
        StartCoroutine("ScreenTouchEvent");
    }

    /// <summary>
    /// 터치 이벤트를 터치했을때 실질적으로 움직이는 코루틴 (ScreenTouch에서 넘어옴)
    /// </summary>
    private IEnumerator ScreenTouchEvent()
    {
        int movedcar = moveIndex;

        //활성된 터치스크린을 꺼줌
        touchScreenObj[moveIndex].SetActive(false);
        touchScreenObj[moveIndex].transform.parent.GetComponent<UISprite>().spriteName = "4dgame_touchBtn_deactive";

        //움직일 대상이 제대로 지정이 안됐다면 코루틴을 빠져나감
        if (movedcar == -1)
        {
            yield break;
        }

        //움직일 좌표값을 저장
        float moving = 0.0f;

        //이벤트에 실패해서 코루틴을 들어온거라면 음수로 변경
        if (eventFail)
        {
            moving = failMoveDistance;
            moving = -moving;
        }
        else
        {
            moving = successMoveDistance;
        }

        //움직일 차가 적차라면 음수로 변경
        if (movedcar != 0)
        {
            moving = -moving;
        }

        if (moving > 0)
        {
            effectAudio.clip = accelSound;
        }
        else
        {
            effectAudio.clip = decelSound;
        }

        //갈 거리 지정
        Vector3 carPos = racingCar[movedcar].transform.localPosition;
        Vector3 distance = new Vector3(racingCar[movedcar].transform.localPosition.x, racingCar[movedcar].transform.localPosition.y + moving, racingCar[movedcar].transform.localPosition.z) - carPos;

        effectAudio.Play();

        for (int i = 0; i < 50; i++)
        {
            //10프레임동안 지정한만큼 이동
            racingCar[movedcar].transform.localPosition = Vector3.Lerp(carPos, carPos + distance, i * 0.02f);

            //차 이동에 따른 내 차 이름 이동
            if (movedcar == 0)
            {
                Vector3 pos = UICamera.mainCamera.ViewportToWorldPoint(contentsCamera.WorldToViewportPoint(racingCar[0].transform.position));

                pos.y += racingCar[0].transform.localScale.y / 4;

                myCarLabel.transform.position = pos;
            }

            yield return new WaitForFixedUpdate();
        }

        //실패인지를 따지는 bool값을 다시 false로 변경
        eventFail = false;
    }

    /// <summary>
    /// Start가 멈춘뒤 2초뒤에 날라갑니다.
    /// </summary>
    private IEnumerator StartSpriteMove()
    {
        yield return new WaitForSeconds(2.0f);

        startSprite.GetComponent<TweenPosition>().from = startSprite.transform.localPosition;
        startSprite.GetComponent<TweenPosition>().to.x = startSprite.GetComponent<UISprite>().localSize.x + (Screen.width / 2);
        startSprite.GetComponent<TweenPosition>().ResetToBeginning();
        startSprite.GetComponent<TweenPosition>().PlayForward();
    }

    /// <summary>
    /// 게임 종료 이벤트
    /// </summary>
    private IEnumerator GameEndEvent()
    {
        //터치 이벤트 종료
        StopCoroutine("ScreenTouchStart");

        //터치 이벤트 버튼 비활성화 상태로 변경
        if (moveIndex != -1 && touchScreenObj[moveIndex] != null)
        {
            touchScreenObj[moveIndex].transform.parent.GetComponent<UISprite>().spriteName = "4dgame_touchBtn_deactive";
            touchScreenObj[moveIndex].SetActive(false);
        }

        //사운드 중지
        bgmAudio.Stop();
        EffectSoundManager.이펙트.이펙트사운드_중지();
        effectAudio.clip = finishSound;
        effectAudio.Play();

        //내 차 라벨 숨기기
        myCarLabel.gameObject.SetActive(false);

        isWin = true;

        //적차와 내 차 위치 비교하여, 승패 여부 확인
        for (int i = 0; i < racingCar.Length; i++)
        {
            //위치 비교하여, 내 차가 뒤쳐져있다면 패배로 변경
            if (i != 0 && racingCar[0].transform.position.y < racingCar[i].transform.position.y)
            {
                isWin = false;
            }

            //종료 애니메이션 실행
            racingCar[i].GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animation>().clip =
                racingCar[i].GetComponent<ModelInfo>().애니메이션정보.애니클립[1];
            racingCar[i].GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animation>().wrapMode = WrapMode.Once;
            racingCar[i].GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animation>().Play();
        }

        //이펙트 사운드가 다 끝날떄까지 대기
        while (effectAudio.isPlaying)
        {
            yield return new WaitForFixedUpdate();
        }
        
        //내 차 애니메이션에 따른 위치,회전 변경
        racingCar[0].transform.localPosition = victoryPos.transform.localPosition;
        racingCar[0].transform.eulerAngles = victoryPos.transform.eulerAngles;

        //승패 택스트 출력
        resultText.SetActive(true);
        resultText.GetComponent<TweenAlpha>().PlayForward();

        //적차 숨기기
        for (int i = 1; i < racingCar.Length; i++)
        {
            racingCar[i].GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animation>().Stop();
            racingCar[i].SetActive(false);
        }

        //승리 이벤트
        if (isWin)
        {
            //승리 사운드,이미지 출력
            effectAudio.clip = victorySound;
            effectAudio.Play();
            resultText.GetComponent<UISprite>().spriteName = "4dgame_victory";

            yield return new WaitForSeconds(2.0f);

            //다음난이도 버튼 띄우기
            if (currentLevel < maxLevel)
            {
                nextLevelBtn.SetActive(true);
            }
        }
        else
        {
            //패배 사운드,이미지 출력
            effectAudio.clip = gameoverSound;
            effectAudio.Play();
            resultText.GetComponent<UISprite>().spriteName = "4dgame_gmaeover";

            yield return new WaitForSeconds(2.0f);

            //재시작 버튼 띄우기
            if (currentLevel < maxLevel)
            {
                restartBtn.SetActive(true);
            }
        }
    }

    /// <summary>
    /// 얻어온 gameSetting 값에 따라 다음난이도,재시작 실행
    /// </summary>
    private IEnumerator GameReset()
    {
        MainUI.메인.딜레이팝업UI.SetActive(true);

        //레이싱카,다음 난이도 버튼 숨기기
        racingCar[0].SetActive(false);

        if (isWin)
        {
            nextLevelBtn.SetActive(false);

            //난이도 상승
            currentLevel += 1;

            //전보다 20퍼센트 더 빠르게 이벤트가 돌게끔 변경
            currentEventTime *= 0.8f;
            currentEventLimitTime *= 0.8f;

            //난이도 라벨 변경
            if (currentLevel == maxLevel)
            {
                levelLabel.text = string.Format("Level Max");
            }
            else
            {
                levelLabel.text = string.Format("Level " + currentLevel);
            }
        }
        else
        {
            restartBtn.SetActive(false);
        }

        //스타트 Sprite 비활성
        startSprite.GetComponent<TweenPosition>().enabled = false;
        startLightLantern.GetComponent<TweenPosition>().enabled = false;


        //레이싱카 활성 및 애니메이션 정지
        for (int i = 0; i < racingCar.Length; i++)
        {
            racingCar[i].SetActive(true);
            racingCar[i].GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animation>().clip = racingCar[i].GetComponent<ModelInfo>().애니메이션정보.애니클립[0];
            CarTransformSetting(racingCar[i], carPos[i]);

            racingCar[i].GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animation>().Play();

            yield return new WaitForFixedUpdate();

            racingCar[i].GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animation>().Stop();

            yield return new WaitForSeconds(0.1f);
        }

        //스타트 Sprtie 원래 위치로 이동
        startSprite.GetComponent<TweenPosition>().from = startSprite.transform.localPosition;
        startSprite.GetComponent<TweenPosition>().from.x = -(startSprite.GetComponent<UISprite>().localSize.x + (Screen.width / 2));
        startSprite.GetComponent<TweenPosition>().to = startSprite.transform.localPosition;
        startSprite.GetComponent<TweenPosition>().to.x = 0;
        startSprite.GetComponent<TweenPosition>().ResetToBeginning();

        startLightLantern.GetComponent<TweenPosition>().ResetToBeginning();

        //내 차 이름 위치 조정
        myCarLabel.gameObject.SetActive(true);

        Vector3 pos = UICamera.mainCamera.ViewportToWorldPoint(contentsCamera.WorldToViewportPoint(racingCar[0].transform.position));

        pos.y += racingCar[0].transform.localScale.y / 4;

        myCarLabel.transform.position = pos;

        //신호등 꺼주기
        for (int i = 0; i < startLight.Length; i++)
        {
            startLight[i].SetActive(false);
        }

        //기타 UI 비활성화
        pauseBtn.SetActive(false);
        resultText.SetActive(false);
        startBtn.SetActive(true);

        MainUI.메인.딜레이팝업UI.SetActive(false);
    }
    #endregion   
}