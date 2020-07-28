using UnityEngine;
using System.Collections;
using HedgehogTeam.EasyTouch;

public class TouchEventManager : MonoBehaviour
{
    public static TouchEventManager 터치;

    public ObSelectManager obSelectManager;


    private Quaternion 캐릭터각도저장;

    private Vector3 이동좌표저장;

    public GameObject EasyTouchObj;

    /// <summary>
    /// 3D 지형을 사용합니다.
    /// </summary>
    public GameObject 지형3D;

    /// <summary>
    /// 2D 지형을 사용합니다.
    /// </summary>
    public GameObject 지형2D;

    /// <summary>
    /// 
    /// </summary>
    public GameObject 컨텐츠오브젝트;

    // 클릭 후 이동 관련 변수
    public Camera 카메라;
    public Transform 깊이고정;

    /// <summary>
    /// 모델 사이즈의 최소 값을 지정 합니다.
    /// </summary>
    public float 인식후_최소크기 = 0.1f;

    /// <summary>
    /// 모델 사이즈의 최대 값을 지정 합니다.
    /// </summary>
    public float 인식후_최대크기 = 0.2f;

    /// <summary>
    /// 모델 사이즈의 최소 값을 지정 합니다.
    /// </summary>
    public float 비인식후_최소크기 = 0.5f;

    /// <summary>
    /// 모델 사이즈의 최대 값을 지정 합니다.
    /// </summary>
    public float 비인식후_최대크기 = 2.0f;

    /// <summary>
    /// 확대 축소 감도
    /// </summary>
    public float 확대축소감도 = 10.0f;

    public float 회전감도 = 1.0f;

    /// <summary>
    /// true 이면 2D 좌표로 움직 입니다.
    /// false 이면 3D 좌표로 움직 입니다.
    /// </summary>
    public static bool 이동좌표계 = true;

    /// <summary>
    /// 3D 콘텐츠 이동속도
    /// </summary>
    private float 클릭후이동속도 = 60.0f;

    /// <summary>
    /// 더블클릭한 좌표를 저장합니다.
    /// </summary>
    private Vector3 클릭좌표;

    /// <summary>
    /// 더블클릭을 했는지 안했는지를 판단하기 위한 변수입니다.
    /// </summary>
    bool 더블클릭여부 = false;

    /// <summary>
    /// 마커 인식시 초기의 위치값을 저장합니다.
    /// </summary>
    private Vector3 오브젝트위치초기값;

    /// <summary>
    /// 더블클릭 했을때 플레이할 애니 번호
    /// </summary>
    public int DoubleTabAniNumber = 1;

    /// <summary>
    /// 더블탭 이벤트를 사용할지 여부를 결정합니다.
    /// </summary>
    public bool 더블탭사용 = false;

    /// <summary>
    /// 오브젝트 터치 이벤트를 사용할지 여부를 결정합니다.
    /// </summary>
    public bool 오브젝트터치사용 = false;


    //[HideInInspector]
    public GameObject 기준콜라이더;
    private float 현재거리;

    void Awake()
    {
        오브젝트위치초기값 = 컨텐츠오브젝트.transform.position;

        터치 = this;
    }

    void Update()
    {

    }

    void FixedUpdate()
    {
        if (이동좌표계)
        {
            지형3D.SetActive(false);
            지형2D.SetActive(true);

            if (더블클릭여부)
            {
                클릭좌표 = 오브젝트위치초기값;
                Mathf.Abs(1);
            }

            //스케치씬일경우 지형2D는 사용하지 않습니다.
            if (TargetManager.타깃메니저.스케치씬사용)
            {
                지형2D.GetComponent<BoxCollider>().enabled = false;
            }
        }
        else
        {
            지형2D.SetActive(false);
            지형3D.SetActive(true);

            if (더블클릭여부)
            {
                더블클릭이동();
            }
        }
    }

    void OnEnable()
    {
        // 터치 이벤트 등록
        EasyTouch.On_PinchIn += EasyTouch_On_PinchIn;
        EasyTouch.On_PinchOut += EasyTouch_On_PinchOut;

        if (RacingFourD.instance == null)
        {
            EasyTouch.On_SimpleTap += EasyTouch_On_SimpleTap;
        }

        if (더블탭사용)
        {
            EasyTouch.On_DoubleTap += EasyTouch_On_DoubleTap;
        }

        EasyTouch.On_TouchStart += EasyTouch_On_TouchStart;
        //EasyTouch.On_TouchDown += EasyTouch_On_TouchDown;
        EasyTouch.On_Drag += EasyTouch_On_Drag;
    }

    void UnsubscribeEvent()
    {
        // 터치 이벤트 해제
        EasyTouch.On_PinchIn -= EasyTouch_On_PinchIn;
        EasyTouch.On_PinchOut -= EasyTouch_On_PinchOut;

        if (RacingFourD.instance == null)
        {
            EasyTouch.On_SimpleTap -= EasyTouch_On_SimpleTap;
        }

        if (더블탭사용)
        {
            EasyTouch.On_DoubleTap -= EasyTouch_On_DoubleTap;
        }

        EasyTouch.On_TouchStart -= EasyTouch_On_TouchStart;
        //EasyTouch.On_TouchDown -= EasyTouch_On_TouchDown;
        EasyTouch.On_Drag -= EasyTouch_On_Drag;
    }

    void OnDisable()
    {
        UnsubscribeEvent();
    }

    void OnDestroy()
    {
        UnsubscribeEvent();
    }

    public void 더블클릭이동()
    {
        // 클릭 후 이동
        컨텐츠오브젝트.transform.position = Vector3.MoveTowards(컨텐츠오브젝트.transform.position, 클릭좌표, 클릭후이동속도 * Time.deltaTime);

        // 현재거리 측정
        현재거리 = Vector3.Distance(컨텐츠오브젝트.transform.position, 클릭좌표);

        // 이동이 완료되면 정지
        if (Mathf.Abs(현재거리) < 1)
        {
            AnimationManager.애니메이션.애니메이션01_재생();
            더블클릭여부 = false;
        }
    }

    /// <summary>
    /// 더블 터치한 좌표에 컨텐츠를 3D 좌표로 이동 합니다.
    /// </summary>
    private void EasyTouch_On_DoubleTap(Gesture 제스처)
    {
        if (!이동좌표계 && 더블탭사용)
        {
            // 모델에 저장된 애니메이션 중 걷는 모션을 재생한다.
            if (DoubleTabAniNumber == 1)
            {
                AnimationManager.애니메이션.애니메이션01_재생();
            }
            else if (DoubleTabAniNumber == 2)
            {
                AnimationManager.애니메이션.애니메이션02_재생();
            }

            float pos = Mathf.Abs(카메라.transform.position.z);        // 카메라 깊이값 설정
            클릭좌표 = 제스처.GetTouchToWorldPoint(pos);               // 화면 클릭 좌표를 월드좌표로 변환

            // 컨텐츠오브젝트의 깊이값 유지하며 방향전환
            깊이고정.transform.position =
                new Vector3(클릭좌표.x, 클릭좌표.y, 컨텐츠오브젝트.transform.position.z);
            컨텐츠오브젝트.transform.LookAt(깊이고정.transform);

            // 로테이션 위치 조정
            if (컨텐츠오브젝트.transform.position.x < 클릭좌표.x)
            {
                //더블클릭의 x값이 더 크다면 밑의 회전을 적용
                if (TargetManager.타깃메니저.스케치씬사용)
                {
                    //스케치씬일 경우
                    컨텐츠오브젝트.transform.Rotate(new Vector3(0, 180, 90));
                }
                else
                {
                    //그 외의 씬
                    컨텐츠오브젝트.transform.Rotate(new Vector3(0, 90, 0));
                }
            }
            else
            {
                //오브젝트의 x값이 더 크다면 아래의 회전을 적용
                if (TargetManager.타깃메니저.스케치씬사용)
                {
                    //스케치씬일 경우
                    컨텐츠오브젝트.transform.Rotate(new Vector3(180, 0, 90));
                }
                else
                {
                    //그 외의 씬
                    컨텐츠오브젝트.transform.Rotate(new Vector3(180, 90, 0));
                }
            }

            더블클릭여부 = true;
        }
    }

    private void EasyTouch_On_TouchStart(Gesture 제스처)
    {
        if (이동좌표계)
        {
            if (기준콜라이더 != null && 제스처.pickedObject == 기준콜라이더 && 제스처.touchCount == 1)
            {
                Vector3 터치좌표 = 제스처.GetTouchToWorldPoint(제스처.pickedObject.transform.position);

                이동좌표저장 = (터치좌표 - 컨텐츠오브젝트.transform.position);
            }
            else if (RacingFourD.instance != null && 제스처.pickedObject != null)
            {
                if (제스처.pickedObject.name == "body" && 제스처.touchCount == 1)
                {
                    Vector3 터치좌표 = 제스처.GetTouchToWorldPoint(제스처.pickedObject.transform.position);
                    이동좌표저장 = (터치좌표 - 컨텐츠오브젝트.transform.position);
                }
            }
        }
    }

    /*
    /// <summary>
    /// 오브젝트 2D 좌표계 이동
    /// </summary>
    private void EasyTouch_On_TouchDown(Gesture 제스처)
    {
        //레이싱 4D에 사용되는 오브젝트 터치다운 이벤트
        if (이동좌표계 && RacingFourD.instance != null && 제스처.pickedObject != null)
        {
            if (제스처.pickedObject.name == "body" && 제스처.touchCount == 1 &&
                RacingFourD.instance.moving == false)
            {
                Vector3 터치좌표 = 제스처.GetTouchToWorldPoint(제스처.pickedObject.transform.position);

                // 마커가 인식 중이라면..
                if (TargetManager.trackableStatus)
                {
                    터치좌표 = new Vector3(터치좌표.x, 터치좌표.y, 0);
                    이동좌표저장 = new Vector3(이동좌표저장.x, 이동좌표저장.y, 0);

                    Vector3 임시좌표 = (터치좌표 - 이동좌표저장);
                    컨텐츠오브젝트.transform.position = new Vector3(임시좌표.x, 임시좌표.y, 임시좌표.z);
                }
                else
                {
                    컨텐츠오브젝트.transform.position = (터치좌표 - 이동좌표저장);
                }
            }

            return;
        }
    }
    */

    private void EasyTouch_On_Drag(Gesture ges)
    {
        if (이동좌표계 && 기준콜라이더 != null)
        {
            if (ges.pickedObject == 기준콜라이더 && ges.touchCount == 1)
            {
                Vector3 터치좌표;

                터치좌표 = ges.GetTouchToWorldPoint(ges.pickedObject.transform.position);

                // 마커가 인식 중이라면..
                if (TargetManager.trackableStatus)
                {
                    터치좌표 = new Vector3(터치좌표.x, 터치좌표.y, 0);
                    이동좌표저장 = new Vector3(이동좌표저장.x, 이동좌표저장.y, 0);

                    Vector3 임시좌표 = (터치좌표 - 이동좌표저장);
                    컨텐츠오브젝트.transform.position = new Vector3(임시좌표.x, 임시좌표.y, 임시좌표.z);
                }
                else
                {
                    컨텐츠오브젝트.transform.position = (터치좌표 - 이동좌표저장);
                }
            }

            /*
            if (제스처.pickedObject == null)
            {
                Debug.LogWarning(string.Format("pickedObject value is null"));
            }
            else
            {
                Debug.LogWarning(string.Format("이동좌표계={0}, pickedObject.name={1}, 기준콜라이더.name={2}, 마커인식상태={3}", 이동좌표계.ToString(),
                                                                                                                          제스처.pickedObject.name,
                                                                                                                          기준콜라이더.name,
                                                                                                                          ImageMarkerEvent.마커인식상태.ToString()));
            }        
            */
        }
    }

    /// <summary>
    /// 오브젝트 확대
    /// </summary>
    private void EasyTouch_On_PinchOut(Gesture 제스처)
    {
        if (TargetManager.타깃메니저.usedSelfiMode && LetterManager.Instance.isPreview)
        {
            return;
        }

        float 최대줌인 = 0;

        // 마커가 인식 중이라면..
        if (TargetManager.trackableStatus)
        {
            최대줌인 = 인식후_최대크기;
        }
        else
        {
            최대줌인 = 비인식후_최대크기;
        }

        if (제스처.touchCount == 2)
        {
            float 최대줌인크기 = (Time.deltaTime * (제스처.deltaPinch / 확대축소감도));
            Vector3 오브젝트크기 = 컨텐츠오브젝트.transform.localScale;

            if (obSelectManager != null)
            {
                오브젝트크기 = TargetManager.타깃메니저.모델링오브젝트.transform.localScale;
            }

            // 안드로이드 전면 카메라로 전환했을 때 : 오브젝트크기.x 값이 음수로 바뀜
            if (오브젝트크기.x > 0)
            {
                // 후면 카메라에서 적용
                if (오브젝트크기.x + 최대줌인크기 < 최대줌인)
                {
                    if (obSelectManager != null)
                    {
                        TargetManager.타깃메니저.모델링오브젝트.transform.localScale = new Vector3(
                       (오브젝트크기.x + 최대줌인크기), (오브젝트크기.y + 최대줌인크기), (오브젝트크기.z + 최대줌인크기));
                    }
                    else
                    {
                        컨텐츠오브젝트.transform.localScale = new Vector3(
                       (오브젝트크기.x + 최대줌인크기), (오브젝트크기.y + 최대줌인크기), (오브젝트크기.z + 최대줌인크기));
                    }
                }
            }
            else
            {
                // 전면 카메라에서 적용
                if (Mathf.Abs(오브젝트크기.x - 최대줌인크기) < 최대줌인)
                {
                    if (obSelectManager != null)
                    {
                        TargetManager.타깃메니저.모델링오브젝트.transform.localScale = new Vector3(
                        (오브젝트크기.x - 최대줌인크기), (오브젝트크기.y + 최대줌인크기), (오브젝트크기.z + 최대줌인크기));
                    }
                    else
                    {
                        컨텐츠오브젝트.transform.localScale = new Vector3(
                        (오브젝트크기.x - 최대줌인크기), (오브젝트크기.y + 최대줌인크기), (오브젝트크기.z + 최대줌인크기));
                    }
                }
            }

            if (LetterManager.Instance != null)
            {
                // 마커가 인식 중이라면..
                if (TargetManager.trackableStatus)
                {
                    LetterManager.Instance.SetLetterTextScale(컨텐츠오브젝트, TargetManager.타깃메니저.인식후_사이즈값);
                }
                else
                {
                    LetterManager.Instance.SetLetterTextScale(컨텐츠오브젝트, TargetManager.타깃메니저.비인식후_사이즈값);
                }
            }
        }
    }

    /// <summary>
    /// 오브젝트 축소
    /// </summary>
    private void EasyTouch_On_PinchIn(Gesture 제스처)
    {
        if (TargetManager.타깃메니저.usedSelfiMode && LetterManager.Instance.isPreview)
        {
            return;
        }

        float 최소줌인 = 0;

        // 마커가 인식 중이라면..
        if (TargetManager.trackableStatus)
        {
            최소줌인 = 인식후_최소크기;
        }
        else
        {
            최소줌인 = 비인식후_최소크기;
        }


        if (제스처.touchCount == 2)
        {
            float 최소줌인크기 = (Time.deltaTime * (제스처.deltaPinch / 확대축소감도));
            Vector3 오브젝트크기 = 컨텐츠오브젝트.transform.localScale;

            if (obSelectManager != null)
            {
                오브젝트크기 = TargetManager.타깃메니저.모델링오브젝트.transform.localScale;
            }

            // 안드로이드 전면 카메라로 전환했을 때 : 오브젝트크기.x 값이 음수로 바뀜
            if (오브젝트크기.x > 0)
            {
                // 후면 카메라에서 적용
                if ((오브젝트크기.x - 최소줌인크기) > 최소줌인)
                {
                    if (obSelectManager != null)
                    {
                        TargetManager.타깃메니저.모델링오브젝트.transform.localScale = new Vector3(
                      (오브젝트크기.x - 최소줌인크기), (오브젝트크기.y - 최소줌인크기), (오브젝트크기.z - 최소줌인크기));
                    }
                    else
                    {
                        컨텐츠오브젝트.transform.localScale = new Vector3(
                      (오브젝트크기.x - 최소줌인크기), (오브젝트크기.y - 최소줌인크기), (오브젝트크기.z - 최소줌인크기));
                    }
                }
            }
            else
            {
                // 전면 카메라에서 적용
                if (Mathf.Abs(오브젝트크기.x + 최소줌인크기) > 최소줌인)
                {
                    if (obSelectManager != null)
                    {
                        TargetManager.타깃메니저.모델링오브젝트.transform.localScale = new Vector3(
                            (오브젝트크기.x + 최소줌인크기), (오브젝트크기.y - 최소줌인크기), (오브젝트크기.z - 최소줌인크기));
                    }
                    else
                    {
                        컨텐츠오브젝트.transform.localScale = new Vector3(
                            (오브젝트크기.x + 최소줌인크기), (오브젝트크기.y - 최소줌인크기), (오브젝트크기.z - 최소줌인크기));
                    }
                }
            }

            if (LetterManager.Instance != null)
            {
                // 마커가 인식 중이라면..
                if (TargetManager.trackableStatus)
                {
                    LetterManager.Instance.SetLetterTextScale(컨텐츠오브젝트, TargetManager.타깃메니저.인식후_사이즈값);
                }
                else
                {
                    LetterManager.Instance.SetLetterTextScale(컨텐츠오브젝트, TargetManager.타깃메니저.비인식후_사이즈값);
                }
            }
        }
    }

    /// <summary>
    /// 오브젝트 터치 시 지정된 애니메이션과 이펙트 사운드 발생
    /// </summary>
    /// <param name="제스처"></param>
    private void EasyTouch_On_SimpleTap(Gesture 제스처)
    {

        if (제스처.pickedObject != null && 기준콜라이더 != null && 제스처.pickedObject.tag == "modelLabel")
        {
            if (TargetManager.타깃메니저.observeManager != null && TargetManager.타깃메니저.observeManager.modelLabelSet)
            {
                TargetManager.타깃메니저.observeManager.ModelLabelClickEvent(제스처.pickedObject);
            }
        }

        if (오브젝트터치사용 && 기준콜라이더 != null && 제스처.pickedObject == 기준콜라이더)
        {
            Debug.Log("오브젝트 터치이벤트 발생");

            //증강된 오브젝트를 저장
            int 인덱스 = TargetManager.타깃메니저.복제모델링인덱스;
            GameObject 오브젝트 = TargetManager.타깃메니저.에셋번들복제컨텐츠[인덱스];

            //지정된 애니메이션을 실행
            AnimationManager.애니메이션.지정애니메이션_재생(오브젝트.GetComponent<ModelInfo>().애니메이션정보.애니클립.Length - 1);

            //이펙트 반복재생을 잠시 꺼둠
            EffectSoundManager.이펙트.루프여부 = false;
        }
    }

    public void 비활성화_호출()
    {
        StartCoroutine(터치콜라이더_비활성화());
    }

    /// <summary>
    /// 버튼과 컨텐츠가 겹쳤을 경우에 컨텐츠의 콜라이더를 비활성화 시켜, 둘다 동작하는것을 막습니다.
    /// </summary>
    /// <returns></returns>
    IEnumerator 터치콜라이더_비활성화()
    {
        if (기준콜라이더 != null)
        {
            기준콜라이더.GetComponent<BoxCollider>().enabled = false;
            yield return new WaitForSeconds(0.01f);
            기준콜라이더.GetComponent<BoxCollider>().enabled = true;
        }

        if (RacingFourD.instance != null)
        {
            RacingFourD.instance.UIButtonClicked();
        }

        if (TargetManager.타깃메니저.usedSelfiMode)
        {
            TargetManager.EnableTracking = true;
        }
    }

    /// <summary>
    /// 마커 오브젝트 기준콜라이더를 설정합니다.
    /// </summary>
    public void SetMarkerCollider(GameObject colliderObj)
    {
        기준콜라이더 = colliderObj;
    }
}