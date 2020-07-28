using UnityEngine;
using System.Collections;

public class DriveEduUI : MonoBehaviour
{
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
    /// 일시정지 버튼
    /// </summary>
    public GameObject pauseBtn;

    /// <summary>
    /// 조이스틱을 놓았을시 조이스틱을 원래 위치로 돌릴 Vector값
    /// </summary>
    private Vector2 stickZeroPosition;

    public static DriveEduUI instance;

    void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start()
    {
        //조이스틱을 켜고, 처음위치를 저장
        joystickUI.SetActive(true);

        //조이스틱UI의 움직일 범위를 지정
        if (joystickBack != null)
        {
            joystickBack.GetComponent<CircleCollider2D>().radius = joystickBack.GetComponent<UISprite>().localSize.x / 2;
        }

        pauseBtn.SetActive(false);
        gameUI.SetActive(false);
    }

    public void RacingDriveUISetActive(bool value)
    {
        if(value)
        {
            //메인 UI를 다시 켜줌
            MainUI.메인.UIOpen(MainUI.메인.uiEventLinkManager);

            //UI들을 꺼줌
            pauseBtn.SetActive(false);
            gameUI.SetActive(false);
        }
        else
        {
            //메인 UI를 다시 켜줌
            MainUI.메인.UIClose(MainUI.메인.uiEventLinkManager);

            gameUI.SetActive(true);

            MainUI.메인.애니동작UI.SetActive(false);
            
            //조이스틱의 위치 조정
            stick.GetComponent<UISprite>().ResetAndUpdateAnchors();
            stickZeroPosition = stick.transform.localPosition;
            
        }
    }

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
