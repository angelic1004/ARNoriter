using UnityEngine;
using System.Collections;
using HedgehogTeam.EasyTouch;

public class RacingFourD : MonoBehaviour
{
    /// <summary>
    /// 터치 무브 시작점
    /// </summary>
    public Transform startPosition;

    /// <summary>
    /// 터치 무브 도착점
    /// </summary>
    public Transform alivePosition;

    /// <summary>
    /// 인식시 레이캐스트에 사용되는 Plane
    /// </summary>
    public GameObject recognizedPlane;

    /// <summary>
    /// 비인식시 레이캐스트에 사용되는 Plane
    /// </summary>
    public GameObject derecognizedPlane;

    /// <summary>
    /// 인식시 사용되는 Plane의 부모오브젝트
    /// </summary>
    [HideInInspector]
    public GameObject recognizedObj;

    /// <summary>
    /// 이동할 오브젝트
    /// </summary>
    private GameObject contentsObj;

    /// <summary>
    /// Audio 소스
    /// </summary>
    public AudioSource FourDAudio;

    /// <summary>
    /// 이동시 Audio 소스
    /// </summary>
    public AudioSource FourDMoveAudio;

    /// <summary>
    /// 사운드 파일을 가지고 있는 Prefab 이름
    /// </summary>
    public string soundDataName;

    // 사운드들
    [HideInInspector]
    public AudioClip idleSound;
    [HideInInspector]
    public AudioClip startSound;


    /// <summary>
    /// 처음 시작시 실행하는 애니가 실행 되었는지 체크
    /// </summary>
    private bool introAniIsPlaying = false;

    [HideInInspector]
    public bool moving = false;

    private bool uiBtnTouch = false;

    public static RacingFourD instance;


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
        contentsObj = TargetManager.타깃메니저.컨텐츠최상위오브젝트;
    }

    void OnEnable()
    {
        EasyTouch.On_SimpleTap += RacingFourD_SimpleTap;
    }

    void OnDisable()
    {
        UnsubscribeEvent();
    }

    void UnsubscribeEvent()
    {
        EasyTouch.On_SimpleTap -= RacingFourD_SimpleTap;
    }
    
    void OnDestroy()
    {
        UnsubscribeEvent();
    }

    /// <summary>
    /// 레이싱 관찰하기 초기화
    /// </summary>
    public void RacingFourDInit()
    {
        if (TargetManager.trackableStatus)
        {
            introAniIsPlaying = false;
            IntroPlay();
        }
        else
        {
            if (introAniIsPlaying)
            {
                //시작 애니가 이미 끝난상태로 비인식을 왔다면 idle 애니를 실행
                TargetManager target = TargetManager.타깃메니저;
                target.에셋번들복제컨텐츠[target.복제모델링인덱스].GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animation>().clip =
                    target.에셋번들복제컨텐츠[target.복제모델링인덱스].GetComponent<ModelInfo>().애니메이션정보.애니클립[1];
                target.에셋번들복제컨텐츠[target.복제모델링인덱스].GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animation>().wrapMode = WrapMode.Loop;
                target.에셋번들복제컨텐츠[target.복제모델링인덱스].GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animation>().Play();
            }
            else
            {
                IntroPlay();
            }
        }
    }

    /// <summary>
    /// 레이캐스트를 사용한 터치이벤트 발생
    /// </summary>
    public void RacingFourD_SimpleTap(Gesture 제스처)
    {
        Debug.Log("들어옴");

        if (TargetManager.타깃메니저.첫인식상태)
        {
            if (제스처.pickedObject != null && 제스처.pickedObject.name != "body")
            {
                StartCoroutine("CalcMovePos");
            }
        }
    }

    /// <summary>
    /// 터치한 포지션을 계산하기 위한 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator CalcMovePos()
    {
        if(SideMenuUI.Instance.sideMenuStatus != SideMenuUI.SideMenuStatus.None)
        {
            yield break;
        }

        //첫 애니가 실행된 상태라면
        if (introAniIsPlaying)
        {
            RaycastHit hit = new RaycastHit();
            Ray ray = TouchEventManager.터치.카메라.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray.origin, ray.direction, out hit))
            {
                if (TargetManager.trackableStatus)
                {
                    //제대로 레이캐스트 히트 했다면 이리로
                    if (hit.collider.name == "Recorgnized Plane")
                    {
                        alivePosition.position = hit.point;
                    }
                    else
                    {
                        Debug.LogWarning("Touch Collider is not RayPlane");
                    }
                }
                else
                {
                    //제대로 레이캐스트 히트 했다면 이리로
                    if (hit.collider.name == "Derecorgnized Plane")
                    {
                        alivePosition.position = hit.point;
                    }
                    else
                    {
                        Debug.LogWarning("Touch Collider is not RayPlane");
                    }
                }
            }

            yield return new WaitForSeconds(0.1f);

            if (uiBtnTouch)
            {
                uiBtnTouch = false;
                yield break;
            }

            //만일 이미 터치무브를 하고 있다면 Stop
            if (moving)
            {
                StopCoroutine("LerpPositionMoving");
            }

            StartCoroutine("LerpPositionMoving");
        }
    }

    /// <summary>
    /// 실제로 오브젝트를 움직이는 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator LerpPositionMoving()
    {
        //이동 사운드 재생
        FourDAudio.Pause();
        FourDMoveAudio.Play();

        moving = true;

        Vector3 endPos = alivePosition.position;

        TargetManager target = TargetManager.타깃메니저;

        target.에셋번들복제컨텐츠[target.복제모델링인덱스].GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animation>().clip =
            target.에셋번들복제컨텐츠[target.복제모델링인덱스].GetComponent<ModelInfo>().애니메이션정보.애니클립[1];
        target.에셋번들복제컨텐츠[target.복제모델링인덱스].GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animation>().Play();

        if (TargetManager.trackableStatus)
        {
            //시작되는 위치값을 저장
            startPosition.position = contentsObj.transform.position;

            //도착지점과의 회전각을 계산
            Quaternion clickedpostion = Quaternion.LookRotation(endPos - contentsObj.transform.position, Vector3.back);

            //인식상태에서의 회전각을 돌려줌
            clickedpostion.eulerAngles = new Vector3(clickedpostion.eulerAngles.x + 180, clickedpostion.eulerAngles.y, clickedpostion.eulerAngles.z);

            //시작점과 도착점 사이의 거리를 계산
            float distance = Vector3.Distance(startPosition.position, endPos);

            //속도 지정
            float velocity = 2.0f;

            //걸리는 시간을 계산
            float time = distance / velocity;

            //걸리는 시간이 1초보다 적다면 움직이지 않습니다.(너무 가까운곳에서 움직이면 오브젝트가 휘어버림)
            if (time <= 1)
            {
                yield break;
            }

            //오브젝트 회전
            for (float i = 0f; i < 1; i += 0.1f)
            {
                contentsObj.transform.rotation = Quaternion.Lerp(contentsObj.transform.rotation, clickedpostion, i);

                yield return new WaitForFixedUpdate();
            }

            //오브젝트 이동
            for (float i = 0f; i <= time; i += velocity)
            {
                contentsObj.transform.position = Vector3.Lerp(startPosition.position, endPos, (velocity * i) / distance);

                yield return new WaitForFixedUpdate();
            }
        }
        else
        {
            //위와 기능은 동일
            startPosition.position = contentsObj.transform.position;

            Quaternion clickedpostion = Quaternion.LookRotation(endPos - contentsObj.transform.position, Vector3.back);

            clickedpostion.eulerAngles = new Vector3(clickedpostion.eulerAngles.x + 180, clickedpostion.eulerAngles.y, clickedpostion.eulerAngles.z);

            float distance = Vector3.Distance(startPosition.position, endPos);
            float velocity = 0.5f;
            float time = distance / velocity;

            if (time <= 1)
            {
                yield break;
            }

            for (float i = 0f; i < 1; i += 0.1f)
            {

                contentsObj.transform.rotation = Quaternion.Lerp(contentsObj.transform.rotation, clickedpostion, i);


                yield return new WaitForFixedUpdate();
            }

            for (float i = 0f; i <= time; i += velocity)
            {
                contentsObj.transform.position = Vector3.Lerp(startPosition.position, endPos, (velocity * i) / distance);

                yield return new WaitForFixedUpdate();
            }
        }

        //움직임이 끝났다면 다시 idle 사운드로 변경
        FourDMoveAudio.Stop();
        FourDAudio.Pause();
        FourDAudio.Play();

        moving = false;

        yield return null;

    }

    /// <summary>
    /// 레이캐스트에 사용되는 패널을 변경해줌
    /// </summary>
    public void RayPenelSetting()
    {
        if (TargetManager.trackableStatus)
        {
            //인식패널로 변경
            recognizedPlane.SetActive(true);
            derecognizedPlane.SetActive(false);

            //인식된 타겟으로 위치를 변경
            recognizedPlane.transform.parent = recognizedObj.transform;
            recognizedPlane.transform.localScale = new Vector3(Screen.width, Screen.height, 1);

            recognizedPlane.transform.localEulerAngles = new Vector3(90, 0, 0);

        }
        else
        {
            //비인식 패널로 변경
            recognizedPlane.SetActive(false);
            derecognizedPlane.SetActive(true);

            //위치값 초기화
            derecognizedPlane.transform.localScale = new Vector3(Screen.width, Screen.height, 1);
            derecognizedPlane.transform.position = Vector3.zero;
            derecognizedPlane.transform.position = new Vector3(0, 0, contentsObj.transform.position.z);
            derecognizedPlane.transform.eulerAngles = Vector3.zero;
        }
    }

    /// <summary>
    /// 인트로 애니메이션,사운드를 실행
    /// </summary>
    public void IntroPlay()
    {
        TargetManager target = TargetManager.타깃메니저;
        target.에셋번들복제컨텐츠[target.복제모델링인덱스].GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animation>().clip =
            target.에셋번들복제컨텐츠[target.복제모델링인덱스].GetComponent<ModelInfo>().애니메이션정보.애니클립[0];
        target.에셋번들복제컨텐츠[target.복제모델링인덱스].GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animation>().wrapMode = WrapMode.Once;
        target.에셋번들복제컨텐츠[target.복제모델링인덱스].GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animation>().Play();

        MainUI.메인.애니메이션동작_목록보이기(target.에셋번들복제컨텐츠[target.복제모델링인덱스].GetComponent<ModelInfo>().애니메이션정보.애니클립.Length);

        FourDAudio.clip = startSound;
        FourDAudio.loop = false;
        FourDAudio.Play();

        MainUI.메인.애니메이션동작_UI숨기기();

        StartCoroutine("IntroIsPlay");
    }

    /// <summary>
    /// 인트로가 끝나면 터치무브를 사용할수 있게끔 체크할 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator IntroIsPlay()
    {
        while (true)
        {
            if (!TargetManager.타깃메니저.에셋번들복제컨텐츠[TargetManager.타깃메니저.복제모델링인덱스].GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animation>().isPlaying)
            {
                introAniIsPlaying = true;
                FourDAudio.clip = idleSound;
                FourDAudio.loop = true;
                FourDAudio.Play();
                MainUI.메인.애니메이션동작_UI보이기();
                yield break;
            }

            yield return new WaitForFixedUpdate();
        }
    }

    /// <summary>
    /// 레이가 아닌 UI버튼을 눌렀다면 코루틴 중지
    /// </summary>
    public void UIButtonClicked()
    {
        uiBtnTouch = true;
    }

    /// <summary>
    /// 외부에서 자동차의 움직임을 멈춰주기 위한 메서드
    /// </summary>
    public void StopCarMove()
    {
        StopCoroutine("LerpPositionMoving");
    }
}
