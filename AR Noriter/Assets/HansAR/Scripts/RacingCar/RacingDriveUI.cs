using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RacingDriveUI : MonoBehaviour
{
    #region Public Variable

    /// <summary>
    /// 미니맵을 비추고 있는 카메라
    /// </summary>
    public GameObject minimapCamera;

    /// <summary>
    /// 게임에 필요한 UI
    /// </summary>
    public GameObject gameUI;

    [Space(20)]
    /// <summary>
    /// 조이스틱 UI
    /// </summary>
    public GameObject joystickUI;

    /// <summary>
    /// 조이스틱 배경(움직임 범위)
    /// </summary>
    public GameObject joystickBack;

    /// <summary>
    /// 조이스틱의 스틱
    /// </summary>
    public GameObject stick;

    /// <summary>
    /// 엑셀레이터 버튼
    /// </summary>
    public GameObject accel;

    /// <summary>
    /// 시작 버튼
    /// </summary>
    public GameObject startBtn;

    /// <summary>
    /// 일시정지 버튼
    /// </summary>
    public GameObject pauseBtn;

    [Space(20)]
    /// <summary>
    /// 스타트 스프라이트
    /// </summary>
    public GameObject startSprite;

    /// <summary>
    /// 시작 신호등
    /// </summary>
    public GameObject startLightLantern;

    /// <summary>
    /// 시작 신호등 불빛
    /// </summary>
    public GameObject[] startLight;

    /// <summary>
    /// 역주행 경고 스프라이트
    /// </summary>
    public GameObject warningSprite;

    [Space(20)]
    /// <summary>
    /// 랭킹 UI
    /// </summary>
    public GameObject rankingUI;

    /// <summary>
    /// 랭킹에 대한 Raptime 표시 라벨 배열순서대로 1위,2위 ....
    /// </summary>
    public GameObject[] rapTimeRankingLabel;

    [Space(20)]
    /// <summary>
    /// 내 차의 미니맵 위치를 표시할 오브젝트
    /// </summary>
    public GameObject myCarMinimapPos;

    /// <summary>
    /// 고스트 차량의 미니맵 위치를 표시할 오브젝트
    /// </summary>
    public GameObject ghostCarMinimapPos;

    /// <summary>
    /// 미니맵의 화면에 표시할 위치값 오브젝트
    /// </summary>
    public GameObject minimapPos;

    /// <summary>
    /// 미니맵 캔버스
    /// </summary>
    public GameObject minimapCanvas;

    public UnityEngine.UI.RawImage minimapRawImage;

    /// <summary>
    /// 랜더 택스쳐
    /// </summary>
    public RenderTexture renderTexture;

    /// <summary>
    /// 현재 돈 바퀴수 Label
    /// </summary>
    [Space(20)]
    public UILabel rapLabel;

    /// <summary>
    /// Rap Time Label
    /// </summary>
    public UILabel rapTimeLabel;

    /// <summary>
    /// 역주행 Label(사용안하는 중)
    /// </summary>
    public UILabel backwardLabel;

    /// <summary>
    /// 속도 표시 Label
    /// </summary>
    public UILabel velocityLabel;


    #endregion

    /// <summary>
    /// 고스트 차량 미니맵 오브젝트
    /// </summary>
    private GameObject[] ghostCarPosObj;
    /// <summary>
    /// 조이스틱을 놓았을시 조이스틱을 원래 위치로 돌릴 Vector값
    /// </summary>
    private Vector2 stickZeroPosition;

    /// <summary>
    /// Rap Time 시간
    /// </summary>
    private float rapTime;

    /// <summary>
    /// 돈 바퀴 수
    /// </summary>
    private int rap;

    /// <summary>
    /// 랩타임을 분단위로 저장할 변수
    /// </summary>
    private int min;

    [HideInInspector]
    public List<string> ranking;

    private RacingDrive driveData;

    public static RacingDriveUI instance;

    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        driveData = RacingDrive.instance;

        ranking = new List<string>();

        //조이스틱을 켜고, 처음위치를 저장
        joystickUI.SetActive(true);

        //조이스틱UI의 움직일 범위를 지정
        if (joystickBack != null)
        {
            joystickBack.GetComponent<CircleCollider2D>().radius = joystickBack.GetComponent<UISprite>().localSize.x / 2;
        }

        //UI들 비활성화
        pauseBtn.SetActive(false);
        rankingUI.SetActive(false);
        startBtn.SetActive(false);
        gameUI.SetActive(false);
    }

    #region UI 초기화 관련

    /// <summary>
    /// 레이싱 운전하기 UI 상태 변경 메서드
    /// </summary>
    /// <param name="value">마커 인식 상태</param>
    public void RacingDriveUISetActive()
    {
        //if (value)
        //{
        //    //미니맵 카메라를 원래위치로 되돌림
        //    minimapCamera.transform.parent = driveData.driveRoot.transform;
        //    minimapCamera.SetActive(false);

        //    //메인 UI를 다시 켜줌
        //    MainUI.메인.UIOpen(MainUI.메인.uiEventLinkManager);

        //    if (ghostCarPosObj != null)
        //    {
        //        for (int i = 0; i < ghostCarPosObj.Length; i++)
        //        {
        //            Destroy(ghostCarPosObj[i]);
        //        }

        //        ghostCarPosObj = null;
        //    }

        //    //UI들을 꺼줌
        //    pauseBtn.SetActive(false);
        //    rankingUI.SetActive(false);
        //    startBtn.SetActive(false);
        //    gameUI.SetActive(false);

        //}
        //else
        //{
        //UI 상태 변경
        gameUI.SetActive(true);
        startBtn.SetActive(true);
        MainUI.메인.UI_상태변경(MainUI.메인.인식글자UI, false);
        MainUI.메인.UIClose(MainUI.메인.uiEventLinkManager);

        //엑셀버튼과 조이스틱을 아직 사용하지 못하게끔 변경
        accel.GetComponent<BoxCollider2D>().enabled = false;
        joystickBack.GetComponent<CircleCollider2D>().enabled = false;

        //조이스틱의 위치 조정
        stick.GetComponent<UISprite>().ResetAndUpdateAnchors();
        stickZeroPosition = stick.transform.localPosition;

        //시작 신호등을 비활성상태로 변경
        for (int i = 0; i < startLight.Length; i++)
        {
            startLight[i].SetActive(false);
        }

        //랩타임, 랩수 초기화
        rapTime = 0.0f;
        rap = 0;
        rapLabel.text = string.Format(rap + " / " + driveData.maxRap);
        rapTimeLabel.text = "00 : 00.00";

        //랭킹 초기화
        for (int i = 0; i < rapTimeRankingLabel.Length; i++)
        {
            rapTimeRankingLabel[i].transform.GetChild(1).GetComponent<UILabel>().text = "00 : 00 : 00";
            rapTimeRankingLabel[i].transform.GetChild(1).GetComponent<UILabel>().color = new Color(255, 255, 255);
        }

        //랭킹 리스트 초기화
        ranking.Clear();

        //신호등 원래위치로 이동
        startSprite.GetComponent<TweenPosition>().from = startSprite.transform.localPosition;
        startSprite.GetComponent<TweenPosition>().from.x = -(startSprite.GetComponent<UISprite>().localSize.x + (Screen.width / 2));
        startSprite.GetComponent<TweenPosition>().to = startSprite.transform.localPosition;
        startSprite.GetComponent<TweenPosition>().to.x = 0;
        startSprite.GetComponent<TweenPosition>().ResetToBeginning();

        startLightLantern.GetComponent<TweenPosition>().ResetToBeginning();

        //트윈 꺼줌
        startSprite.GetComponent<TweenPosition>().enabled = false;
        startLightLantern.GetComponent<TweenPosition>().enabled = false;
        //}

        //역주행시 경고 스프라이트
        warningSprite.SetActive(false);
    }

    /// <summary>
    /// 미니맵 관련 초기화
    /// </summary>
    public void MinimapInit()
    {
        //미니맵의 크기 지정
        Vector2 sizeSave = new Vector2(minimapPos.GetComponent<UISprite>().localSize.x, minimapPos.GetComponent<UISprite>().localSize.y);
        minimapCanvas.GetComponent<RectTransform>().sizeDelta = sizeSave;

        //미니맵을 로컬좌표 zero포지션으로
        minimapCanvas.GetComponent<RectTransform>().localPosition = Vector3.zero;

        minimapRawImage.uvRect = driveData.track.GetComponent<RacingTrackInfo>().rawImgRect;

        //미니맵 카메라의 위치,각도 조정
        minimapCamera.SetActive(true);
        minimapCamera.GetComponent<Camera>().targetTexture = renderTexture;
        minimapCamera.transform.parent = driveData.track.transform;
        minimapCamera.transform.localPosition = Vector3.zero;
        minimapCamera.transform.localPosition = new Vector3(0, 45, 0);
        minimapCamera.transform.LookAt(driveData.track.transform);
        minimapCamera.transform.localEulerAngles += (Vector3.up * -90);

        //내차 미니맵 오브젝트 위치,회전,크기 조정
        myCarMinimapPos.transform.localPosition = Vector3.zero;
        myCarMinimapPos.transform.localEulerAngles = Vector3.zero;

        myCarMinimapPos.transform.localPosition = RacingDrive.instance.myCarObj.transform.localPosition;
        myCarMinimapPos.transform.localPosition += (Vector3.up * 0.3f);
        myCarMinimapPos.transform.Rotate(new Vector3(90, -180, 0));
        myCarMinimapPos.transform.localScale = new Vector3(15, 15, 1);

        //배틀모드일 경우
        if (RacingDrive.battleMode)
        {
            ghostCarPosObj = new GameObject[driveData.usingGhostCarCount];

            for (int i = 0; i < ghostCarPosObj.Length; i++)
            {
                ghostCarPosObj[i] = Object.Instantiate(ghostCarMinimapPos);
                ghostCarPosObj[i].transform.parent = driveData.driveRoot.transform;

                //고스트 차량 미니맵 오브젝트 위치,회전,크기 조정
                ghostCarPosObj[i].SetActive(true);
                ghostCarPosObj[i].transform.localPosition = Vector3.zero;
                ghostCarPosObj[i].transform.localEulerAngles = Vector3.zero;

                ghostCarPosObj[i].transform.localPosition = driveData.usingGhostCarObj[i].transform.localPosition;
                ghostCarPosObj[i].transform.localPosition += (Vector3.up * 0.2f);
                ghostCarPosObj[i].transform.Rotate(new Vector3(90, 180, 0));
                ghostCarPosObj[i].transform.localScale = new Vector3(15, 15, 15);
            }
        }
        else
        {
            //고스트차량 미니맵 오브젝트를 꺼줌
            ghostCarMinimapPos.SetActive(false);
        }
    }

    #endregion

    #region 게임 시작 관련

    /// <summary>
    /// 게임 시작 UI제어 메서드
    /// </summary>
    public void GameStart()
    {
        pauseBtn.SetActive(true);

        //Start Text를 tween으로 움직여 애니메이션처럼 이동
        startSprite.GetComponent<TweenPosition>().PlayForward();
        startLightLantern.GetComponent<TweenPosition>().PlayForward();
        accel.GetComponent<BoxCollider2D>().enabled = true;
        joystickBack.GetComponent<CircleCollider2D>().enabled = true;

        //조이스틱을 원위치로
        stick.transform.localPosition = stickZeroPosition;

        //분단위 시간을 저장할 int
        min = 0;

    }

    /// <summary>
    /// Start 할때 start가 날라오는 부분을 두번으로 나눔(가운데서 한번 멈춰야 하므로)
    /// </summary>
    public void StartSpriteNextFoward()
    {
        StartCoroutine("StartSpriteMove");
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

    #endregion

    #region UI 제어 관련

    /// <summary>
    /// 게임 선택 UI 켜줌
    /// </summary>
    public void GameSelectUIOpen()
    {
        //UI 상태변경
        RotateUI.회전.회전UI_숨기기();
        MainUI.메인.UIClose(MainUI.메인.uiEventLinkManager);
        MainUI.메인.인식글자UI.SetActive(false);

        //modeSelectUI.SetActive(true);
    }

    /// <summary>
    /// 경고 스프라이트 부름
    /// </summary>
    public void WarningUIOpen()
    {
        if (warningSprite != null)
        {
            TweenManager.tween_Manager.TweenAllDestroy(warningSprite);
            TweenManager.tween_Manager.AddTweenAlpha(warningSprite, 0.25f, 1, 0.5f, UITweener.Style.PingPong);
            TweenManager.tween_Manager.TweenAlpha(warningSprite);
        }
    }

    /// <summary>
    /// 경고 스프라이트 닫음
    /// </summary>
    public void WarningUIClose()
    {
        if (warningSprite != null)
        {
            TweenManager.tween_Manager.TweenAllDestroy(warningSprite);
            TweenManager.tween_Manager.AddTweenAlpha(warningSprite, warningSprite.GetComponent<UIWidget>().alpha, 0, 0.2f, UITweener.Style.Once);
            TweenManager.tween_Manager.TweenAlpha(warningSprite);
        }
    }

    public void TrackSelectUIOpen()
    {

    }

    /// <summary>
    /// 게임중 업데이트 되는 UI 컨트롤 코루틴
    /// </summary>
    /// <returns></returns>
    public void GamePlayUIController()
    {

        //60초를 1분으로 치환
        if (rapTime >= 60)
        {
            rapTime -= 60;
            min++;
        }

        //RapTime 표시
        RacingDriveLabelSet(rapTimeLabel, string.Format("{0:d2} : {1:00.00}", min, rapTime));

        rapTime += Time.deltaTime;

        //내 차의 위치를 표시할 오브젝트를 차를 따라가게끔 계속 이동
        myCarMinimapPos.transform.localPosition = RacingDrive.instance.myCarObj.transform.localPosition;
        myCarMinimapPos.transform.localPosition +=
            (Vector3.up * 0.3f);

        //고스트차량의 위치를 표시할 오브젝트를 차를 따라가게끔 계속 이동
        if (RacingDrive.battleMode)
        {
            for (int i = 0; i < ghostCarPosObj.Length; i++)
            {
                ghostCarPosObj[i].transform.localPosition = driveData.usingGhostCarObj[i].transform.localPosition;
                ghostCarPosObj[i].transform.localPosition += (Vector3.up * 0.2f);
            }
        }

        //차의 속력에 따른 계기판의 속도를 업데이트
        RacingDriveLabelSet(velocityLabel,
                string.Format("{0:f0} KM", Mathf.Abs(Vector3.Dot(RacingDrive.instance.myCarObj.GetComponent<Rigidbody>().velocity, driveData.rigidVel) * 10)));

    }

    /// <summary>
    /// UI Label text 수정 메서드
    /// </summary>
    /// <param name="label"></param>
    /// <param name="format"></param>
    public void RacingDriveLabelSet(UILabel label, string format)
    {
        label.text = format;
    }

    /// <summary>
    /// 게임 종료 세팅
    /// </summary>
    public void GameEndUISet()
    {
        //UI 업데이트 종료
        StopCoroutine("GamePlayUIController");

        //랭킹 UI 활성화
        rankingUI.SetActive(true);

        //싱글모드일 경우
        if (RacingDrive.battleMode == false)
        {
            //내 차 랭킹을 보여줌
            rapTimeRankingLabel[0].SetActive(true);
            rapTimeRankingLabel[0].transform.GetChild(1).GetComponent<UILabel>().text = ranking[0];

            //다른 랭킹 Label은 비활성화
            for (int i = 1; i < rapTimeRankingLabel.Length; i++)
            {
                rapTimeRankingLabel[i].SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < rapTimeRankingLabel.Length; i++)
            {
                //랭킹을 순위 순서대로 보여줌
                if (i < ranking.Count)
                {
                    rapTimeRankingLabel[i].transform.GetChild(1).GetComponent<UILabel>().text = ranking[i];
                }
                else
                {
                    //랭킹이 적히지 않은 Label은 비활성화
                    rapTimeRankingLabel[i].SetActive(false);
                }
            }
        }
    }
    #endregion

    #region 버튼 클릭 관련

    /// <summary>
    /// 재시작 버튼 클릭 메서드
    /// </summary>
    public void RetryBtnUIControl()
    {
        //UI를 비활성화
        rankingUI.SetActive(false);
        gameUI.SetActive(false);
        //modeSelectUI.SetActive(false);

        //카메라 오브젝트를 원래 위치로 이동
        minimapCamera.transform.parent = driveData.driveRoot.transform;
        minimapCamera.SetActive(false);

        //AR에 필요한 UI 켜줌
        MainUI.메인.인식글자UI.SetActive(true);
        MainUI.메인.UIOpen(MainUI.메인.uiEventLinkManager);
        MainUI.메인.애니동작UI.SetActive(false);
        RotateUI.회전.회전UI_숨기기();

        TargetManager.타깃메니저.첫인식상태 = false;

        //일시정지 팝업 끄기
        MainUI.메인.PausePopupClose();
    }

    /// <summary>
    /// 싱글모드 버튼 클릭
    /// </summary>
    //public void SingleModeBtnClick()
    //{
    //    if (driveData != null)
    //    {
    //        //싱글모드로 설정후 초기화 시작
    //        RacingDrive.battleMode = false;
    //        driveData.DriveInit();
    //    }
    //}

    /// <summary>
    /// 배틀모드 버튼 클릭
    /// </summary>
    //public void BattleModeBtnClick()
    //{
    //    if (driveData != null)
    //    {
    //        //배틀모드로 설정후 초기화 시작
    //        RacingDrive.battleMode = true;
    //        driveData.DriveInit();
    //    }
    //}

    #endregion

    #region 조이스틱 관련
    /// <summary>
    /// 조이스틱 드래그했을때, 터치한 위치로 조이스틱을 이동하는 메서드
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public float JoyStickDragControl(Transform pos)
    {

        //터치포지션과 현재 포지션 사이의 각도 계산
        float radian = Mathf.Atan2(pos.localPosition.y - stickZeroPosition.y, pos.localPosition.x - stickZeroPosition.x);

        //터치한 점과 원점사이의 거리가 스틱의 반지름보다 작은경우
        if (Vector2.Distance(stickZeroPosition, pos.localPosition) < (joystickBack.GetComponent<UISprite>().localSize.x / 2) - (stick.GetComponent<UISprite>().localSize.x / 2))
        {
            //포지션값이 그대로 마우스 포지션
            stick.transform.position = pos.position;
        }
        //그보다 큰경우, 배경 밖으로 나가는경우가 있어 위치를 제한
        else
        {
            Vector2 calcPos = calcCirclePos(radian, (joystickBack.GetComponent<UISprite>().localSize.x / 2.0f) - (stick.GetComponent<UISprite>().localSize.x / 2.0f));

            //Vector2 calcPos = calcCirclePos(radian, (Vector2.Distance(stickZeroPosition, pos.localPosition)));

            stick.transform.localPosition = calcPos;
        }

        return radian;
    }

    /// <summary>
    /// 조이스틱을 뗐을때 발생하는 코루틴 호출 메서드
    /// </summary>
    public void JoyStickReverse()
    {
        StartCoroutine("JoyStickReverseControl");
    }

    /// <summary>
    /// 조이스틱에서 터치 똄 코루틴
    /// </summary>
    /// <returns></returns>
    public IEnumerator JoyStickReverseControl()
    {
        //현재 스틱 위치
        Vector2 startCurrentPosition = stick.transform.localPosition;

        for (float i = 0.0f; i <= 1.0f; i += 0.1f)
        {
            //처음위치로 Lerp
            stick.transform.localPosition = Vector2.Lerp(startCurrentPosition, stickZeroPosition, i);
            yield return new WaitForEndOfFrame();
        }

        //마지막엔 정확히 중앙으로 돌려줌
        stick.transform.localPosition = stickZeroPosition;

        if (driveData.currentMaxVelocity == driveData.maxVelocity)
        {
            driveData.currentMaxVelocity = driveData.currentMaxVelocity + (driveData.calcRotate.transform.eulerAngles.y / 50);
        }
        else if(driveData.currentMaxVelocity > driveData.maxVelocity)
        {
            driveData.currentMaxVelocity = driveData.maxVelocity;
        }
    }

    /// <summary>
    /// 원의 선상의 좌표 구하기
    /// </summary>
    /// <param name="angle">두점 사이의 각도</param>
    /// <param name="radius">반지름</param>
    /// <returns></returns>
    private Vector2 calcCirclePos(float angle, float radius)
    {
        Vector2 pos = Vector2.zero;

        pos.x = stickZeroPosition.x + (Mathf.Cos(angle) * radius);
        pos.y = stickZeroPosition.y + (Mathf.Sin(angle) * radius);

        return pos;
    }
    #endregion


}
