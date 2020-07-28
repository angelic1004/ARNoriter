using UnityEngine;
using System.Collections;
using Vuforia;

public class RotateUI : MonoBehaviour
{
    private enum RotationButtonKey
    {
        LEFT = 0,
        RIGHT,
        UP,
        DOWN
    }


    // 모델링 인식-비인식 방향 설정 (어디를 바라보고 있는지) 
    public enum ModelDirection
    {
        DEFAULT,
        LEFT_LEFT,          // 인식-왼쪽, 비인식-왼쪽 (실사 동물)
        FRONT_LEFT,         // 인식-왼쪽, 비인식-왼쪽 (스케치 동물)
        FRONT_FRONT         // 인식-정면, 비인식-정면 (스케치 사람)
    }

    public ModelDirection modelDirection;

    public GameObject 회전UI;

    /// <summary>
    /// 증강된 모델 오브젝트를 넣어 줍니다. (Unity3d Editor) ImageTarget Contents
    /// </summary>
    public GameObject 컨텐츠오브젝트 = null;

    public float 회전감도 = 1.0f;

    private bool[] 버튼눌림상태 = new bool[4];

    public static RotateUI 회전;

    public bool 월드좌표사용 = false;
    public bool 상하좌우반전 { set; get; }

    private Vector2 runwayMinimapPos;


    void Awake()
    {
        회전 = this;
    }

    void Start()
    {
        CallByStartEvent();
    }

    public void CallByStartEvent()
    {
        //회전UI.SetActive(false);
        //SetDefaultModelDirection();

        StartCoroutine(디바이스방향체크());
        StartCoroutine(ApplyDirectionClickEvent());
    }

    /// <summary>
    /// 회전을 동작 시키는 함수
    /// </summary>
    /// <param name="directionKey">방향</param>
    private void ApplyTransformRotate(RotationButtonKey directionKey)
    {
        Vector3 applyVectorValue = Vector3.zero;

        switch (directionKey)
        {
            case RotationButtonKey.LEFT:
                if (월드좌표사용)
                {
                    if (상하좌우반전)
                    {
                        applyVectorValue = Vector3.down;
                    }
                    else
                    {
                        applyVectorValue = Vector3.up;
                    }
                }
                else
                {
                   
                    applyVectorValue = Vector3.forward;
             
                }
                break;
            case RotationButtonKey.RIGHT:
                if (월드좌표사용)
                {
                    if (상하좌우반전)
                    {
                        applyVectorValue = Vector3.up;
                    }
                    else
                    {
                        applyVectorValue = Vector3.down;
                    }
                }
                else
                {
                    
                     applyVectorValue = Vector3.back;
                  
                }
                break;
            case RotationButtonKey.UP:
                if (월드좌표사용)
                {
                    if (상하좌우반전)
                    {
                        applyVectorValue = Vector3.left;
                    }
                    else
                    {
                        applyVectorValue = Vector3.right;
                    }
                }
                else
                {
                    
                    applyVectorValue = Vector3.right;
                    
                }
                break;
            case RotationButtonKey.DOWN:
                if (월드좌표사용)
                {
                    if (상하좌우반전)
                    {
                        applyVectorValue = Vector3.right;
                    }
                    else
                    {
                        applyVectorValue = Vector3.left;
                    }
                }
                else
                {
                    
                     applyVectorValue = Vector3.left;
                    
                }
                break;
            default:
                break;
        }

        if (월드좌표사용)
        {
            컨텐츠오브젝트.transform.Rotate(applyVectorValue, 회전감도, Space.World);
        }
        else
        {
            컨텐츠오브젝트.transform.Rotate(applyVectorValue, 회전감도);
        }
    }

    /// <summary>
    /// 버튼눌림상태에 따른 회전을 동작하는 함수 호출
    /// </summary>
    /// <returns></returns>
    private IEnumerator ApplyDirectionClickEvent()
    {
        while (true)
        {
            if (버튼눌림상태[0])                          // left
            {
                ApplyTransformRotate(RotationButtonKey.LEFT);
            }
            else if (버튼눌림상태[1])                     // right
            {
                ApplyTransformRotate(RotationButtonKey.RIGHT);
            }
            else if (버튼눌림상태[2])                     // up
            {
                ApplyTransformRotate(RotationButtonKey.UP);
            }
            else if (버튼눌림상태[3])                     // down
            {
                ApplyTransformRotate(RotationButtonKey.DOWN);
            }

            yield return new WaitForFixedUpdate();
        }
    }

    public void 회전UI_보이기()
    {
        회전UI.SetActive(true);
    }

    public void 회전UI_숨기기()
    {
        회전UI.SetActive(false);
    }

    public void 컨텐츠_회전_초기화()
    {
        if (RacingFourD.instance != null && RacingFourD.instance.moving)
        {
            return;
        }

        컨텐츠_회전_중지();

        StartCoroutine(컨텐츠_회전_코루틴());
    }

    private IEnumerator 컨텐츠_회전_코루틴()
    {
        yield return new WaitForEndOfFrame();

        상하좌우반전 = false;

        if (TargetManager.trackableStatus)
        {
            컨텐츠오브젝트.transform.localPosition = TargetManager.타깃메니저.인식후_좌표값;
            컨텐츠오브젝트.transform.localEulerAngles = TargetManager.타깃메니저.인식후_회전값;
            컨텐츠오브젝트.transform.localScale = TargetManager.타깃메니저.인식후_사이즈값;

            /*
            if (VuforiaBehaviour.Instance.VideoBackGroundMirrored != VuforiaRenderer.VideoBackgroundReflection.ON)
            {
                컨텐츠오브젝트.transform.localScale = new Vector3(-TargetManager.타깃메니저.인식후_사이즈값.x, TargetManager.타깃메니저.인식후_사이즈값.y, TargetManager.타깃메니저.인식후_사이즈값.z);
            }
            else
            {
                컨텐츠오브젝트.transform.localScale = TargetManager.타깃메니저.인식후_사이즈값;
            }
            */

            if (LetterManager.Instance != null)
            {
                LetterManager.Instance.SetLetterTextScale(컨텐츠오브젝트, TargetManager.타깃메니저.인식후_사이즈값);
            }
        }
        else
        {
            Vector3 비인식후_회전값 = TargetManager.타깃메니저.비인식후_회전값;
            Camera camera = TargetManager.타깃메니저.AR카메라.GetComponentInChildren<Camera>();
            Matrix4x4 m = camera.projectionMatrix;

            // ipad, lenovo 패드에서 전면카메라 가로일 경우 m.m11 값이 0보다 작다
            // gpad, asus 는 전면카메라 세로일 경우 m.m11 값이 0보다 작다
            // 이때는 상/하, 좌/우 값에 180f 을 더하여 반전 시켜 준다.
            if (m.m11 < 0f)
            {
                비인식후_회전값.x += 180f;
                비인식후_회전값.y += 180f;

                상하좌우반전 = true;
            }

            컨텐츠오브젝트.transform.localPosition = TargetManager.타깃메니저.비인식후_좌표값; ;
            컨텐츠오브젝트.transform.localEulerAngles = 비인식후_회전값;
            컨텐츠오브젝트.transform.localScale = TargetManager.타깃메니저.비인식후_사이즈값;

            /*
            if (VuforiaBehaviour.Instance.VideoBackGroundMirrored != VuforiaRenderer.VideoBackgroundReflection.ON)
            {
                컨텐츠오브젝트.transform.localScale = TargetManager.타깃메니저.비인식후_사이즈값;
            }
            else
            {
                컨텐츠오브젝트.transform.localScale = new Vector3(-TargetManager.타깃메니저.비인식후_사이즈값.x, TargetManager.타깃메니저.비인식후_사이즈값.y, TargetManager.타깃메니저.비인식후_사이즈값.z);
            }
            */

            if (LetterManager.Instance != null)
            {
                LetterManager.Instance.SetLetterTextScale(컨텐츠오브젝트, TargetManager.타깃메니저.비인식후_사이즈값);
            }
        }

        콜라이더_활성화();
    }

    public void 컨텐츠_회전_왼쪽()
    {
        if (VuforiaBehaviour.Instance.VideoBackGroundMirrored != VuforiaRenderer.VideoBackgroundReflection.ON)
        {
            버튼상태변경(0);
        }
        else
        {
            버튼상태변경(1);
        }
    }

    public void 컨텐츠_회전_오른쪽()
    {
        if (VuforiaBehaviour.Instance.VideoBackGroundMirrored != VuforiaRenderer.VideoBackgroundReflection.ON)
        {
            버튼상태변경(1);
        }
        else
        {
            버튼상태변경(0);
        }
    }

    public void 컨텐츠_회전_위()
    {
        버튼상태변경(2);
    }

    public void 컨텐츠_회전_아래()
    {
        버튼상태변경(3);
    }

    public void 컨텐츠_회전_중지()
    {
        버튼상태중지();
    }

    private void 버튼상태변경(int 인덱스)
    {
        if(RacingFourD.instance != null && RacingFourD.instance.moving)
        {
            return;
        }

        for (int i = 0; i < 버튼눌림상태.Length; i++)
        {
            if (i == 인덱스)
            {
                버튼눌림상태[i] = true;

                콜라이더_비활성화();
            }
            else
            {
                버튼눌림상태[i] = false;
            }
        }
    }

    private void 버튼상태중지()
    {
        for (int i = 0; i < 버튼눌림상태.Length; i++)
        {
            버튼눌림상태[i] = false;

            콜라이더_활성화();
        }
    }

    public void 콜라이더_비활성화()
    {
        // 눌렀을때 이지터치 이벤트가 안되도록 콜라이더 비활성화
        //TouchEventManager.터치.기준콜라이더.GetComponent<BoxCollider>().enabled = false;
        TouchEventManager.터치.기준콜라이더.GetComponent<BoxCollider>().enabled = false;
    }

    public void 콜라이더_활성화()
    {
        // 눌렀을때 이지터치 이벤트가 안되도록 콜라이더 활성화
        //TouchEventManager.터치.비활성화_호출();
        TouchEventManager.터치.비활성화_호출();
    }

    private IEnumerator 디바이스방향체크()
    {
        // 셀카모드에서 디바이스 방향 변경시 방향상태를 초기화하여 적용
        while (true)
        {
            if (MainUI.방향상태 == 2)
            {
                if (Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeRight)
                {
                    MainUI.방향상태 = 1;
                    컨텐츠_회전_초기화();
                }
            }
            else if (MainUI.방향상태 == 1)
            {
                if (Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown)
                {
                    MainUI.방향상태 = 2;
                    컨텐츠_회전_초기화();
                }
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    /// <summary>
    /// modelDirection의 기본값을 설정합니다.
    /// </summary>
    public void SetDefaultModelDirection()
    {
        // DEFAULT일 경우에 기본값을 인식-왼쪽, 비인식-왼쪽 (스케치 동물)로 설정합니다.
        if (modelDirection == ModelDirection.DEFAULT)
        {
            modelDirection = ModelDirection.FRONT_LEFT;
        }
    }
}