using UnityEngine;
using System.Collections;

public class DriveEduManager : MonoBehaviour
{
    /// <summary>
    /// 돌발이벤트 셋팅
    /// </summary>
    [System.Serializable]
    public class EventSetting
    {
        //저속
        public bool reduce_speed;
        public string[] comment;
    }

    /// <summary>
    /// 애니메이션 
    /// </summary>
    [System.Serializable]
    public class MapAnimationSetting
    {
        public AnimationClip[] animation;
    }

    /// <summary>
    /// ar 카메라
    /// </summary>
    public GameObject ARCamera;

    /// <summary>
    /// 운전하는 차를 따라갈 카메라
    /// </summary>
    public Camera driveCamera;

    /// <summary>
    /// 터치한 위치를 저장할 Transform
    /// </summary>
    public Transform touchPos;

    /// <summary>
    /// 차의 회전값을 저장할 Transform
    /// </summary>
    public Transform calcRotate;

    public GameObject myCarObj;

    /// <summary>
    /// 번들에서 불러오는 컨텐츠의 위치를 지정해줄 부모오브젝트
    /// </summary>
    public GameObject contentsRoot;

    /// <summary>
    /// 돌발이벤트 텍스트
    /// </summary>
    public UILabel event_Text;

    public UILabel test_Text;

    /// <summary>
    /// 차의 최대 속력
    /// </summary>
    public float maxVelocity;

    /// <summary>
    /// 가감속 1회당 차의 속력 증감값
    /// </summary>
    public float minVelocity;

    public float speed;

    /// <summary>
    /// 내 차의 회전에 다른 정면 속도값
    /// </summary>
    [HideInInspector]
    public Vector3 rigidVel;

    private GameObject driveMap;

    private string aniState = "front";

    public int speed_Check = 0;

    [HideInInspector]
    /// <summary>
    /// 차의 현재 속력
    /// </summary>
    public float velocity;

    [HideInInspector]
    /// <summary>
    /// 더해줄 속력 값
    /// </summary>
    public float velocityAmount;

    [HideInInspector]
    public string tagName;
    private GameObject target_Meter;
    public GameObject reset_Obj;


    [SerializeField]
    public EventSetting[] event_Number;

    public MapAnimationSetting[] map_event_Number;

    /// <summary>
    /// 돌발 이벤트
    /// </summary>
    public GameObject sudden_Event;
    /// <summary>
    /// 돌발이벤트 종류
    /// </summary>
    public int event_num = 0;

    /// <summary>
    /// 조이스틱이 움직이고 있는지
    /// </summary>
    private bool moveStart = false;

    /// <summary>
    /// 충돌상태 체크
    /// </summary>
    private bool isCrash = false;

    /// <summary>
    /// 가속상태
    /// </summary>
    private bool velocityState = false;

    /// <summary>
    /// 일시정지 여부
    /// </summary>
    private bool pausedGame = false;

    private bool mission_ing = false;

    private bool collision = false;

    /// <summary>
    /// 맵 오브젝트의 이름
    /// </summary>
    public string mapName;

    public string soundDataName;

    [HideInInspector]
    public string[] assetBundleData;

    public static DriveEduManager instance;

    private DriveEduUI uiController;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        uiController = DriveEduUI.instance;
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.W))
        {
            VelocityUp();
        }

        if (Input.GetKeyUp(KeyCode.W))
        {
            VelocityDown();
        }

        if (Input.GetKey(KeyCode.A))
        {
           myCarObj.transform.Rotate(Vector3.down * 1);
        }

        if (Input.GetKey(KeyCode.D))
        {
            myCarObj.transform.Rotate(Vector3.up * 1);
        }
        
        #region 터치 이벤트
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                //레이캐스트
                Vector2 wp = UICamera.mainCamera.ScreenToWorldPoint(new Vector3(Input.GetTouch(i).position.x, Input.GetTouch(i).position.y, 0));
                Ray2D ray = new Ray2D(wp, Vector2.zero);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

                if (hit.collider != null)
                {
                    //엑셀레이터 터치
                    if (hit.collider.tag == "accel")
                    {
                        if (Input.GetTouch(i).phase == TouchPhase.Began)
                        {
                            VelocityUp();
                        }
                        else if (Input.GetTouch(i).phase == TouchPhase.Ended)
                        {
                            VelocityDown();
                        }
                    }
                    //조이스틱 터치
                    else if (hit.collider.tag == "joystick")
                    {
                        touchPos.position = wp;

                        if (Input.GetTouch(i).phase == TouchPhase.Began)
                        {
                            OnDragJoyStickStart();
                        }
                        else if (Input.GetTouch(i).phase == TouchPhase.Moved)
                        {
                            OnDragJoyStickOver();
                        }
                        else if (Input.GetTouch(i).phase == TouchPhase.Stationary)
                        {
                            OnDragJoyStickOver();
                        }
                        else if (Input.GetTouch(i).phase == TouchPhase.Canceled)
                        {
                            OnDragJoyStickEnd();
                        }
                        else if (Input.GetTouch(i).phase == TouchPhase.Ended)
                        {
                            OnDragJoyStickEnd();
                        }
                    }
                }
                else
                {
                    //아무것도 터치를 하지않았지만, 조이스틱이 눌림상태인 경우
                    touchPos.position = wp;

                    if (moveStart)
                    {
                        if (Input.GetTouch(i).phase == TouchPhase.Moved)
                        {
                            OnDragJoyStickOver();
                        }
                        else if (Input.GetTouch(i).phase == TouchPhase.Stationary)
                        {
                            OnDragJoyStickOver();
                        }
                        else if (Input.GetTouch(i).phase == TouchPhase.Canceled)
                        {
                            OnDragJoyStickEnd();
                        }
                        else if (Input.GetTouch(i).phase == TouchPhase.Ended)
                        {
                            OnDragJoyStickEnd();
                        }
                    }
                }
            }
        }
        #endregion
    }

    #region 초기화 관련
    public void driveEduAssetLoad(string loadAssetData)
    {
        //AssetBundleLoader.getInstance.ReadAssetBundleDriveEdu(GlobalDataManager.m_SelectedAssetBundleName, loadAssetData, contentsRoot);
    }

    public void MapSetting(GameObject obj)
    {
        driveMap = obj;
        driveMap.transform.parent = contentsRoot.transform;
        driveMap.transform.position = Vector3.zero;

        driveMap.SetActive(false);

        map_event_Number[0].animation = driveMap.transform.GetChild(0).GetComponent<MapInfo>().trafficLight1;
        map_event_Number[1].animation = driveMap.transform.GetChild(0).GetComponent<MapInfo>().trafficLight2;

        driveEduAssetLoad(soundDataName);
    }

    public void SoundSetting(GameObject obj)
    {
        obj.AddComponent<DriveEduSoundManager>();

        DriveSoundInfo soundInfo = obj.GetComponent<DriveSoundInfo>();
        DriveEduSoundManager soundManager = obj.GetComponent<DriveEduSoundManager>();

        soundManager.bg_audio_Source = soundInfo.bg_audio_Source;
        soundManager.car_audio_Source = soundInfo.car_audio_Source;
        soundManager.brake_audio_Source = soundInfo.brake_audio_Source;
        soundManager.mission_audio_Source = soundInfo.mission_audio_Source;
        soundManager.collision_audio_Source = soundInfo.collision_audio_Source;
        soundManager.safetyBelt_audio_Source = soundInfo.safetyBelt_audio_Source;

        soundManager.bg_Clip = soundInfo.bg_Clip;
        soundManager.ready_Clip = soundInfo.ready_Clip;
        soundManager.mission_Clip = soundInfo.mission_Clip;
        soundManager.success_Clip = soundInfo.success_Clip;
        soundManager.fail_Clip = soundInfo.fail_Clip;
        soundManager.accel_Clip = soundInfo.accel_Clip;
        soundManager.brake_Clip = soundInfo.brake_Clip;
        soundManager.collision_Clip = soundInfo.collision_Clip;
        soundManager.safetyBelt_Clip = soundInfo.safetyBelt_Clip;
    }

    public void DriveEduInit()
    {
        //일시정지가 떠있다면 무시
        if (pausedGame == false)
        {
            if (TargetManager.trackableStatus)
            {
                recogInit();
            }
            else
            {
                derecogInit();
            }
        }
    }

    /// <summary>
    /// 인식시 초기화
    /// </summary>
    public void recogInit()
    {
        //비인식후 다시 인식시 모든 코루틴을 꺼주기 위해 실행
        StopAllCoroutines();

        ARCamera.GetComponent<Camera>().enabled = true;
        TouchEventManager.터치.EasyTouchObj.SetActive(true);

        //UI를 꺼줌
        uiController.RacingDriveUISetActive(true);
        
        //차 밑으로 내려갔던 카메라를 다시 원래위치로 되돌림
        driveCamera.transform.parent = contentsRoot.transform;
        driveCamera.enabled = false;

        if (myCarObj != null)
        {
            Destroy(myCarObj);
        }

        if (driveMap != null)
        {
            driveMap.SetActive(false);
        }
    }

    public void derecogInit()
    {
        if (TargetManager.타깃메니저.첫인식상태)
        {
            ARCamera.GetComponent<Camera>().enabled = false;
            TouchEventManager.터치.EasyTouchObj.SetActive(false);

            sudden_Event.SetActive(false);

            //차를 복사, 켜줌
            myCarObj = Object.Instantiate(TargetManager.타깃메니저.에셋번들복제컨텐츠[TargetManager.타깃메니저.복제모델링인덱스]);
            myCarObj.SetActive(true);

            //차와 차의 하위 오브젝트의 레이어를 RacingDrive로 변경
            myCarObj.layer = 15;
            myCarObj.transform.SetChildLayer(15);

            //차를 지정한 위치로 이동시킨 후, 강체와 Col메서드를 붙여줌
            myCarObj.transform.parent = contentsRoot.transform;
            myCarObj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            myCarObj.transform.GetChild(0).GetComponent<Animation>().Stop();
            myCarObj.transform.GetChild(0).Rotate(new Vector3(0, 180, 0));
            Destroy(myCarObj.transform.GetChild(0).GetComponent<Animation>());
            myCarObj.transform.GetChild(0).GetComponent<Animator>().enabled = true;
            
            myCarObj.AddComponent<Rigidbody>();
            myCarObj.AddComponent<DriveEduCol>();

            //차의 애니메이션을 재생중지
            myCarObj.GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animator>().StartPlayback();

            driveMap.SetActive(true);

            myCarObj.transform.position = driveMap.transform.GetChild(0).GetComponent<MapInfo>().startPoint.transform.position;
            myCarObj.transform.localPosition = new Vector3(3.65f, 4.4f,myCarObj.transform.localPosition.z);
            velocity = 0;
            velocityAmount = 0;

            //각종 bool값 false;로 변경
            velocityState = false;
            moveStart = false;
            isCrash = false;
            pausedGame = false;

            //Follow Camera 실행
            //driveCamera.transform.parent = myCarObj.transform;
            //driveCamera.transform.position = myCarObj.transform.position + new Vector3(0, 2.0f, -5f);
            //driveCamera.transform.LookAt(myCarObj.transform.position);
            //driveCamera.transform.RotateAround(myCarObj.transform.position, Vector3.up, myCarObj.transform.rotation.z);

            driveCamera.enabled = true;
            driveCamera.GetComponent<FollowCam>().target = myCarObj.transform;

            TargetManager.타깃메니저.HideAllModelingContents();

            uiController.RacingDriveUISetActive(false);

            //차의 강체 고정
            myCarObj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;

            //StartCoroutine("VeclocityControl");
            StartCoroutine("MoveCar");
        }
    }
    #endregion

    #region 조이스틱 이벤트
    /// <summary>
    /// 조이스틱 드래그 시작
    /// </summary>
    public void OnDragJoyStickStart()
    {
        //움직이기 시작
        moveStart = true;
    }

    /// <summary>
    /// 조이스틱 드래그 중
    /// </summary>
    public void OnDragJoyStickOver()
    {
        //버튼이 눌린상태에서만 움직이게끔 함
        if (moveStart)
        {
            //강체중 Y회전만 프리즈 풀어줌
            myCarObj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            OnDragJoyStickEvent();
        }
    }

    /// <summary>
    /// 드래그 종료
    /// </summary>
    public void OnDragJoyStickEnd()
    {
        //강체 프리즈 원상복구
        myCarObj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;

        JoyStickReversePosition();

        //움직이기 종료
        moveStart = false;
    }

    /// <summary>
    /// 조이스틱 드래그 이벤트 코루틴(자동차 회전도 포함)
    /// </summary>
    /// <returns></returns>
    private void OnDragJoyStickEvent()
    {
        //시작시 차의 회전값
        Vector3 startRotate = myCarObj.transform.eulerAngles;

        try
        {
            float radian = 0.0f;

            radian = uiController.JoyStickDragControl(touchPos);

            //차 회전 변경
            //라디안 값을 degree값으로 변경
            calcRotate.transform.eulerAngles = new Vector3(myCarObj.transform.eulerAngles.x, (-(radian * 180 / Mathf.PI - 90)), 0);

            //각 각도별로 지정해주는 앵글값
            if (calcRotate.transform.eulerAngles.y == 0 || calcRotate.transform.eulerAngles.y == 180)
            {
                //0도 or 180도일때
                myCarObj.transform.rotation = myCarObj.transform.rotation;
            }
            else if (calcRotate.transform.eulerAngles.y > 0 && calcRotate.transform.eulerAngles.y < 90)
            {
                //0~90도
                myCarObj.transform.eulerAngles = myCarObj.transform.eulerAngles + (Vector3.up * (calcRotate.transform.eulerAngles.y / 50));

                if (aniState != "right")
                {
                    myCarObj.GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animator>().SetTrigger("right");
                    aniState = "right";
                    Debug.Log("aniState : " + aniState);
                }
            }
            else if (calcRotate.transform.eulerAngles.y > 90 && calcRotate.transform.eulerAngles.y < 180)
            {
                calcRotate.transform.eulerAngles = new Vector3(myCarObj.transform.eulerAngles.x, ((radian * 180 / Mathf.PI + 90)), 0);
                myCarObj.transform.eulerAngles = myCarObj.transform.eulerAngles + (Vector3.up * (calcRotate.transform.eulerAngles.y / 50));

                if (aniState != "right")
                {
                    myCarObj.GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animator>().SetTrigger("right");
                    aniState = "right";
                    Debug.Log("aniState : " + aniState);
                }
            }
            else if (calcRotate.transform.eulerAngles.y > 180 && calcRotate.transform.eulerAngles.y < 270)
            {
                calcRotate.transform.eulerAngles = new Vector3(myCarObj.transform.eulerAngles.x, ((radian * 180 / Mathf.PI + 90)), 0);
                myCarObj.transform.eulerAngles = myCarObj.transform.eulerAngles - (Vector3.down * ((calcRotate.transform.eulerAngles.y - 360) / 50));

                if (aniState != "left")
                {
                    myCarObj.GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animator>().SetTrigger("left");
                    aniState = "left";
                    Debug.Log("aniState : " + aniState);
                }
            }
            else
            {
                myCarObj.transform.eulerAngles = myCarObj.transform.eulerAngles - (Vector3.down * ((calcRotate.transform.eulerAngles.y - 360) / 50));

                if (aniState != "left")
                {
                    myCarObj.GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animator>().SetTrigger("left");
                    aniState = "left";
                    Debug.Log("aniState : " + aniState);
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex);
            throw;
        }

        //역주행 기능..
    }

    /// <summary>
    /// 조이스틱에서 터치를 뗐을때 원래 자리로 돌아가는 코루틴
    /// </summary>
    /// <returns></returns>
    private void JoyStickReversePosition()
    {
        if (aniState != "front")
        {
            myCarObj.GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animator>().SetTrigger("front");
            aniState = "front";
        }

        uiController.JoyStickReverse();
    }
    #endregion

    #region 차량 이동관련 메서드

    /// <summary>
    /// 오브젝트 가속
    /// </summary>
    public void VelocityUp()
    {
        //충돌상태가 아니라면
        if (!isCrash && velocityState == false)
        {
            velocityState = true;

            //엔진소리를 다시 내줌
            //engineAudio.Pause();
            //engineAudio.Play();

            //속도를 가변해줄 값이 음수인경우 0으로 변경
            if (velocityAmount <= 0)
            {
                velocityAmount = 0;
            }

            uiController.accel.GetComponent<UISprite>().spriteName = "drive_accel_press";
        }
    }

    /// <summary>
    /// 오브젝트 감속
    /// </summary>
    public void VelocityDown()
    {
        if (velocityState)
        {
            velocityState = false;

            uiController.accel.GetComponent<UISprite>().spriteName = "drive_accel";
        }
    }

    /// <summary>
    /// 속도 컨트롤 코루틴
    /// </summary>
    /// <returns></returns>
    private void VeclocityControl()
    {
        //가속상태
        if (velocityState)
        {
            //최대속력보다 작은경우만 증가시켜줌
            if (velocity < maxVelocity)
            {
                velocity += velocityAmount;

                if (speed_Check > 0)
                {
                    velocityAmount += (minVelocity / (speed_Check * speed_Check));
                }
                else
                {
                    velocityAmount += minVelocity;
                }
            }
        }
        else
        {
            //최소속력보다 큰경우만 감소시켜줌
            if (velocity > 0)
            {
                velocity -= (velocityAmount * (maxVelocity * 0.5f));
                velocityAmount += minVelocity;
            }
        }

        //test_Text.text = " velocity : " + velocity;
    }

    /// <summary>
    /// 오브젝트 이동 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveCar()
    {
        Debug.Log("Car Moving Start");

        yield return new WaitForSeconds(0.1f);

        rigidVel = Vector3.zero;

        int current_speed = 0;

        float timer = 0.05f;

        while (true)
        {
            if (pausedGame == false)
            {
                //정해진 바퀴수를 다 돌았다면
                //if (myCarRap == maxRap)
                //{
                //    GameEndEvent();
                //}

                //최대속력을 넘지 않도록 실제 속력을 고정
                if (velocity >= maxVelocity && velocityState)
                {
                    rigidVel = CalcRotatePosition(myCarObj.transform.rotation);
                    myCarObj.GetComponent<Rigidbody>().velocity = -(rigidVel * maxVelocity);
                    velocity = maxVelocity;
                    speed_Check = 3;
                }
                //최소 속력을 넘지 않도록 실제 속력을 고정
                else if (velocity <= 0 && !velocityState)
                {
                    myCarObj.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    DriveEduSoundManager.instance.DrivingSoundStop(DriveEduSoundManager.SoundType.Accel);
                    speed_Check = 0;
                    velocity = 0;
                    velocityAmount = 0;
                }
                else
                {
                    //충돌상태가 아니라면
                    if (!isCrash)
                    {
                        //회전에 따른 정면벡터 저장
                        rigidVel = CalcRotatePosition(myCarObj.transform.rotation);

                        //정면에 힘을 부여
                        myCarObj.GetComponent<Rigidbody>().velocity = -(rigidVel * velocity);

                        if(velocity <= maxVelocity * 0.66f)
                        {
                            if (velocity <= maxVelocity * 0.33f && velocity > 0)
                            {
                                if (speed_Check == 2 && !velocityState)
                                {
                                    DriveEduSoundManager.instance.DrivingSoundPlay(DriveEduSoundManager.SoundType.Brake);
                                }
                                speed_Check = 1;
                            }
                            else
                            {
                                speed_Check = 2;
                            }
                        }
                    }
                }
            }
            else
            {
                //일시정지 상태일때, 힘을 0으로 두어 멈춘것처럼 보이게 함
                myCarObj.GetComponent<Rigidbody>().velocity = Vector3.zero;

                ////배틀모드일 경우 배틀모드 차량 일시정지
                //if (battleMode)
                //{
                //    ghostObj.GetComponent<NavMeshAgent>().Stop();
                //}
            }

            speed = Mathf.Abs(Vector3.Dot(myCarObj.GetComponent<Rigidbody>().velocity, rigidVel));

            if (current_speed != speed_Check)
            {
                DriveEduSoundManager.instance.DrivingSoundPlay(DriveEduSoundManager.SoundType.Accel);
                current_speed = speed_Check;
            }

            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                if (velocityState)
                {
                    timer = 0.025f * (speed_Check * speed_Check);
                }
                else
                {
                    timer = 0.05f;
                }
                VeclocityControl();
            }

            yield return new WaitForFixedUpdate();
        }
    }

    /// <summary>
    /// 회전각에 대한 정면벡터 계산
    /// </summary>
    /// <returns></returns>
    protected Vector3 CalcRotatePosition(Quaternion angle)
    {
        Vector3 targetPos = Vector3.zero;
        Quaternion targetRotate = angle;

        Matrix4x4 value = new Matrix4x4();

        //4x4 행렬 초기화
        value = Matrix4x4.identity;

        //행렬에 대한 위치,회전,크기를 정의
        value.SetTRS(Vector3.zero, targetRotate, new Vector3(1, 1, 1));

        //회전각에대한 정면으로 가려는 값을 더해줌
        targetPos += value.MultiplyVector(new Vector4(0, 0, -1));

        //노멀라이즈
        targetPos.Normalize();

        //리턴~
        return targetPos;
    }
    #endregion

    #region 충돌,트리거 이벤트
    /// <summary>
    /// 벽 충돌 이벤트
    /// </summary>
    /// <param name="col"></param>
    public void CollisionEnter(Collision col)
    {
        //충돌 사운드 재생
        //effectAudio.clip = crashSound[Random.Range(0, crashSound.Length)];
        //effectAudio.Play();

        DriveEduSoundManager.instance.DrivingSoundPlay(DriveEduSoundManager.SoundType.Collision);
        DriveEduSoundManager.instance.DrivingSoundPlay(DriveEduSoundManager.SoundType.Brake);
        uiController.accel.GetComponent<UISprite>().spriteName = "drive_accel";
        //충돌상태로 변경

        StartCoroutine("CrashStopCheck");
    }

    /// <summary>
    /// 충돌 정지 상태 체크
    /// </summary>
    /// <returns></returns>
    private IEnumerator CrashStopCheck()
    {
        isCrash = true;

        //while (carObj.GetComponent<Rigidbody>().velocity != Vector3.zero)
        //{
        //    yield return new WaitForFixedUpdate();
        //}

        //충돌에 0.3초정도 텀을 준다.
        yield return new WaitForSeconds(0.3f);

        //속도는 0으로 초기화
        velocity = 0;
        velocityAmount = 0;

        //충돌상태 해제
        isCrash = false;

        //감속상태로 변경
        velocityState = false;
    }


    /// <summary>
    /// 이벤트 부분에 닿았을 경우 이벤트 발생여부 확인
    /// </summary>
    /// <param name="other"></param>
    public void EventCheck(Collider other)
    {
       // Debug.Log(other.name);
        // event_ready = 이벤트 시작전 앞 collider, event = 실제 이벤트 발생 collider, event_end = 이벤트 collider 뒷부분
        // 뒤에서 들어올 경우 이벤트 발생하지 않도록 하기 위해 설정
        if (other.gameObject.tag == "event_end")
        {
            tagName = other.gameObject.tag;
        }
        else if (other.gameObject.tag == "event_ready")
        {
            tagName = other.gameObject.tag;
        }
        else if (other.gameObject.tag == "event" && tagName != "event_end")
        {
            // if (!mission_ing)
            //  {
            //목적지에 있는 타겟 오브젝트
            target_Meter = other.gameObject.GetComponent<Event_Info>().target_Obj;
            reset_Obj = other.gameObject.GetComponent<Event_Info>().reset_Position_Obj;
            event_num = other.gameObject.GetComponent<Event_Info>().Event_Num;
            MapAnimationStart(1);

            if (!mission_ing)
            {
                //돌발이벤트 발생시 돌발텍스트 alpha값 조정
                event_Text.GetComponent<TweenAlpha>().enabled = false;
                event_Text.alpha = 255;

                EventImgChange(sudden_Event, event_num);
                //돌발이벤트 발생시 돌발이미지 scale,alpha값 설정
                TweenManager.tween_Manager.TweenAlpha(sudden_Event);
                TweenManager.tween_Manager.TweenScale(sudden_Event);
            }

            //사운드 발생 
            DriveEduSoundManager.instance.DrivingSoundPlay(DriveEduSoundManager.SoundType.Mission);

            //이벤트 지점에 설정되어 있는 이벤트가 몇번 이벤트인지 확인 후 실행 
            StopCoroutine(EventStart(event_num));
            StartCoroutine(EventStart(event_num));
            tagName = other.gameObject.tag;
            //  }
        }
    }

    /// <summary>
    /// 이벤트발생시 이벤트 스프라이트 변경
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="num"></param>
    private void EventImgChange(GameObject obj, int num)
    {
        if (event_Number[num].reduce_speed)
        {
            UISprite img = obj.GetComponent<UISprite>();
            img.spriteName = "drive_Slow_en";
        }
        else
        {
            UISprite img = obj.GetComponent<UISprite>();
            img.spriteName = "drive_Stop_en";
        }
    }

    /// <summary>
    /// 이벤트 발생 comment = 1.남은거리, 2. 성공, 3. 실패
    /// </summary>
    /// <param name="_event_num"></param>
    /// <returns></returns>
    private IEnumerator EventStart(int _event_num)
    {
        mission_ing = true;
        float event_Meter = Vector3.Distance(target_Meter.transform.position, myCarObj.transform.position) / 6;
        float limit_Event_Meter = event_Meter + 0.2f;
        float save_Meter = 0;

        while (true)
        {
            event_Meter = Vector3.Distance(target_Meter.transform.position, myCarObj.transform.position) / 6;

            if (event_Meter > 0.1f)
            {
                if (mission_ing)
                {
                    //역주행이 아닐 경우
                    if (limit_Event_Meter >= event_Meter)
                    {
                        event_Text.text = string.Format("[b]{0:N2}m {1}[/b]", event_Meter, event_Number[_event_num].comment[0]);

                        //반대로 1m 거리를 역주행으로 적용하기 위해 저장한다.
                        if (event_Meter < save_Meter)
                        {
                            limit_Event_Meter = event_Meter + 0.2f;
                        }
                    }
                    //역주행일 경우
                    else
                    {
                        event_Text.text = LocalizeText.Value["WrongWay"];//string.Format("[b]역주행이예요.[/b]");

                        //다시 정방향으로 돌았을경우 
                        if (save_Meter > event_Meter)
                        {
                            limit_Event_Meter = event_Meter + 0.2f;
                        }
                        // 역주행시 역주행 메세지 발생후 1.5m 더 갔을경우 또는 이벤트 종료지점에서 10m 떨어질 경우 
                        else if (event_Meter >= limit_Event_Meter + 0.5f || event_Meter > 1.5f)
                        {
                            tagName = "";
                            mission_ing = false;
                            event_Text.text = LocalizeText.Value["DriveAgain"];// string.Format("[b]다시 운전해 주세요.[/b]");
                            TweenManager.tween_Manager.TweenAlpha(event_Text.gameObject);
                            velocity = 0;
                            velocityAmount = 0;
                            myCarObj.transform.localEulerAngles = new Vector3(0, reset_Obj.transform.localEulerAngles.z, 0);
                            myCarObj.transform.position = reset_Obj.transform.position;
                            yield break;
                        }
                    }

                    if (speed == 0 && !collision)
                    {
                        //성공범위 설정
                        //if (event_Meter < 4.0f && event_Meter > 0 && !event_Number[_event_num].reduce_speed)
                        if (event_Meter > 0 && !event_Number[_event_num].reduce_speed)
                        {
                            MissionSuccess(false, _event_num);
                            yield break;
                        }
                    }
                }
                save_Meter = Vector3.Distance(target_Meter.transform.position, myCarObj.transform.position) / 6;
            }
            else
            {
                if (mission_ing)
                {
                    if (event_Number[_event_num].reduce_speed)
                    {
                        float now_Speed = (Mathf.Round(speed / .1f) * .1f);

                        if (velocity < maxVelocity && !collision)
                        {
                            MissionSuccess(true, _event_num);
                            yield break;
                        }
                        else
                        {
                            MissionFail(_event_num);
                            yield break;
                        }
                    }
                    else
                    {
                        MissionFail(_event_num);
                        yield break;
                    }
                }
                yield break;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    /// <summary>
    /// 미션 성공
    /// </summary>
    /// <param name="move"></param>
    /// <param name="event_num"></param>
    private void MissionSuccess(bool move, int event_num)
    {
        if (!move)
        {
            speed = 0;
            myCarObj.GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animator>().SetTrigger("idle");
            aniState = "idle";
        }
        event_Text.text = string.Format(event_Number[event_num].comment[1]);
        DriveEduSoundManager.instance.DrivingSoundPlay(DriveEduSoundManager.SoundType.Success);
        TweenManager.tween_Manager.TweenAlpha(event_Text.gameObject);
        mission_ing = false;
        MapAnimationStart(0);
    }

    /// <summary>
    /// 미션 실패
    /// </summary>
    /// <param name="event_num"></param>
    private void MissionFail(int event_num)
    {
        DriveEduSoundManager.instance.DrivingSoundPlay(DriveEduSoundManager.SoundType.Fail);
        DriveEduSoundManager.instance.DrivingSoundPlay(DriveEduSoundManager.SoundType.Brake);
        velocity = 0;
        velocityAmount = 0;
        event_Text.text = string.Format(event_Number[event_num].comment[2]);
        TweenManager.tween_Manager.TweenAlpha(event_Text.gameObject);
        mission_ing = false;
        myCarObj.GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animator>().SetTrigger("idle");
        aniState = "idle";
        MapAnimationStart(0);
    }
    #endregion

    #region 맵 애니메이션 호출

    public void MapAnimationStart(int ani_Num)
    {
        MapAnimationCall(event_num, ani_Num);
    }

    public void MapAnimationStop()
    {
        MapAnimationUnCalled();
    }

    private void MapAnimationCall(int event_Num, int ani_Num)
    {
        Animation map_animation = driveMap.transform.GetChild(0).GetComponent<Animation>();
        if (map_event_Number[event_Num].animation.Length > ani_Num)
        {
            map_animation.AddClip(map_event_Number[event_Num].animation[ani_Num], map_event_Number[event_Num].animation[ani_Num].name);
            map_animation.clip = map_event_Number[event_Num].animation[ani_Num];
            map_animation.Stop();
            map_animation.Play();
        }
        else if (map_event_Number[event_Num].animation.Length > 0)
        {
            map_animation.clip = map_event_Number[event_Num].animation[0];
            map_animation.Stop();
            map_animation.Play();
        }
    }

    private void MapAnimationUnCalled()
    {
        Animation map_animation = driveMap.transform.GetChild(0).GetComponent<Animation>();
        map_animation.Stop();
    }
    #endregion
}