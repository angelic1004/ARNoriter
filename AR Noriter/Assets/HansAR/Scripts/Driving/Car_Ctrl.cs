using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

using HansAR;

public class Car_Ctrl : MonoBehaviour
{
    public GameObject collision_Prefab_Position;
    public GameObject collision_Prefab_Ngui_Position;
    public GameObject collision_Prefab;

    /// <summary>
    /// 자동차 
    /// </summary>
    public GameObject car;

    /// <summary>
    /// 스케치씬에서 받아와지는 오브젝트(car)가 없을 경우
    /// </summary>
    public GameObject null_Car;

    /// <summary>
    /// 자동차 핸들
    /// </summary>
    public GameObject handle;

    /// <summary>
    /// 속도계바늘
    /// </summary>
    public GameObject speedoMeter_needle;
    /// <summary>
    /// ngui 바늘
    /// </summary>
    public Camera ngui_Camera;

    /// <summary>
    /// 돌발 이벤트
    /// </summary>
    public GameObject sudden_Event;

    /// <summary>
    /// 돌발이벤트 텍스트
    /// </summary>
    public UILabel event_Text;

    public GameObject brake_UI;

    public GameObject parentObj;

    /// <summary>
    /// 핸들 각도 제한 
    /// </summary>
    public float handle_Car_Rotate_Limit;

    /// <summary>
    /// 현재 설정된 속도에 따른 설정(외부에서 사운드 조정)
    /// </summary>
    public int speed_Check = 0;

    /// <summary>
    /// 돌발이벤트 셋팅
    /// </summary>
    [Serializable]
    public class EventSetting
    {
        //저속
        public bool reduce_speed;
        public string[] comment;
    }

    [SerializeField]
    public EventSetting[] event_Number;

    /// <summary>
    /// 돌발이벤트 종류
    /// </summary>
    public int event_num = 0;

    private float accelerate = 1.0f;

    /// <summary>
    /// 마우스 위치저장
    /// </summary>
    private Vector3 input_Position;

    /// <summary>
    /// 자동차 속도
    /// </summary>
    private float speed = 0;
    /// <summary>
    /// 자동차 속도 저장
    /// </summary>
    private float saved_speed = 0;
    /// <summary>
    /// 증가 속도
    /// </summary>
    private float speed_Sensitivity = 0.01f;
    /// <summary>
    /// 악셀 클릭여부
    /// </summary>
    private bool click_Accel = false;
    /// <summary>
    /// 핸들 클릭여부
    /// </summary>
    private bool click_Handle = false;
    /// <summary>
    /// 핸들 놓았을 경우 리셋 범위
    /// </summary>
    private float handle_Reset_Range = 5.0f;
    /// <summary>
    /// 마우스(터치)와 핸들 각도
    /// </summary>
    private float wheel_Z_angle;
    private GameObject target_Meter;
    private GameObject reset_Obj;
    public string tagName;

    private bool mission_ing = false;
    private bool collision = false;
    private bool pause = false;
    private bool addMessage = false;

    public string[] loadModelName;

    [HideInInspector]
    public GameObject[] loadModel;

    private GameObject mapModel;

    public static Car_Ctrl car_ctrl;

    void Awake()
    {
        car_ctrl = this;
    }

    // Use this for initialization
    void Start()
    {
        //MainUI.메인.딜레이팝업UI.SetActive(true);

        GlobalDataManager.m_ResourceFolderEnum = GlobalDataManager.CategoryType.Common;
        GlobalDataManager.m_SelectedAssetBundleName = string.Format("{0}_{1}", GlobalDataManager.m_ResourceFolderEnum.ToString().ToLower(),
                                                                               GlobalDataManager.m_AssetBundlePartName.ToLower());

        //디바이스 화면 가로로 고정(LandscapeLeft)
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = false;

        StartCoroutine(ReadAssetBundleDriveMode(GlobalDataManager.m_SelectedAssetBundleName, loadModelName, parentObj));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        collision_Prefab_Ngui_Position.transform.position = collision_Prefab_Position.transform.position;
        collision_Prefab_Ngui_Position.transform.localRotation = Camera.main.transform.localRotation;
        //핸들클릭
        if (click_Handle)
        {
            HandleDragStay();
        }

        //악셀 클릭 
        if (click_Accel && !collision)
        {
            car.transform.Translate(Vector3.forward * speed / 10);
        }
    }

    /// <summary>
    /// 스케치씬에서 넘어왔을 경우 오브젝트 복사, 아닐경우 null_Car로 대체
    /// </summary>
    private void DrivingInit()
    {
        //맵 초기화
        if (mapModel.GetComponent<MapInfo>() != null)
        {
            MapInfo mapInfo = mapModel.GetComponent<MapInfo>();

            DrivingAnimation.driving_Anim.event_Number[0].animation = mapInfo.trafficLight1;
            DrivingAnimation.driving_Anim.event_Number[1].animation = mapInfo.trafficLight2;
            DrivingAnimation.driving_Anim.map = mapInfo.mapFBX;
        }

        //자동차 초기화
        if (MainUI.sketch_Car != null)
        {
            MainUI.sketch_Car.transform.parent = car.transform;

            //이전 씬에서 넘어온 기준 자동차의 애니메이션이 중간에 멈춘상태로 들어오는 경우가 있음.
            //그를 위해 이전 애니메이션을 한번 초기화후 다시 정지 시켜줌.
            StartCoroutine(PreAnimationInit(MainUI.sketch_Car.GetComponentInChildren<Animation>()));

            if (MainUI.sketch_Car.GetComponentInChildren<DrivingInfo>() != null)
            {
                AniInit(MainUI.sketch_Car.GetComponentInChildren<DrivingInfo>(), MainUI.sketch_Car);
            }

            if (MainUI.sketch_Car.transform.GetChild(0).GetComponent<ColoringInfo>() != null)
            {
                MainUI.sketch_Car.transform.GetChild(0).GetComponent<ColoringInfo>().색칠하기속성.콜라이더.GetComponent<BoxCollider>().enabled = false;
            }
            Debug.Log("sketch_Car : " + MainUI.sketch_Car);
        }
        //자동차가 안넘어왔을경우 null_car로 대체
        else
        {
            null_Car.SetActive(true);

            if (null_Car.GetComponentInChildren<DrivingInfo>() != null)
            {
                AniInit(null_Car.GetComponentInChildren<DrivingInfo>(), null_Car);
            }
        }
        DrivingAnimation.driving_Anim.AnimationInit();
    }

    #region 핸들드래그 이벤트

    /// <summary>
    /// 클릭시작 좌표 저장 
    /// </summary>
    public void HandleInputPress()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            input_Position = Input.mousePosition;
        }
        else
        {
            input_Position = Input.touches[0].position;
        }
    }

    /// <summary>
    /// 드래그 엔드와 같이 사용
    /// </summary>
    public void HandleInputRelease()
    {
        Debug.Log("릴리즈");
        click_Handle = false;
        StopCoroutine(HandleReset());
        StartCoroutine(HandleReset());
        if (speed > 0)
        {
            DrivingAnimation.driving_Anim.ForwardAnimation();
        }
        else
        {
            DrivingAnimation.driving_Anim.IdleAnimation();
        }
    }

    /// <summary>
    /// 드래그 시작(핸들이 클릭이 아니라 드래그를 시작헀다는걸 알기위해 만듬)
    /// </summary>
    public void HandleDragStart()
    {
        Debug.Log("드래그 시작");
        click_Handle = true;
    }

    /// <summary>
    /// 드래그 하는 중 
    /// </summary>
    private void HandleDragStay()
    {
        Vector3 drag_Position;
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            drag_Position = Input.mousePosition;
        }
        else
        {
            drag_Position = Input.touches[0].position;
        }

        //앞바퀴의 각도를 구한다. (핸들과는 반대 방향)
        wheel_Z_angle = getAngle(handle.transform.position, ngui_Camera.ScreenToWorldPoint(drag_Position))
                      - getAngle(handle.transform.position, ngui_Camera.ScreenToWorldPoint(input_Position));

        //핸들각도 제한
        if (wheel_Z_angle < -handle_Car_Rotate_Limit)
        {
            wheel_Z_angle = -handle_Car_Rotate_Limit;
        }
        else if (wheel_Z_angle > handle_Car_Rotate_Limit)
        {
            wheel_Z_angle = handle_Car_Rotate_Limit;
        }
        else
        {
            //앞바퀴 애니메이션
            if (wheel_Z_angle < 0)
            {
                DrivingAnimation.driving_Anim.LeftAnimation();
            }
            else if (wheel_Z_angle > 0)
            {
                DrivingAnimation.driving_Anim.RightAnimation();
            }
        }

        //앞바퀴와 반대방향으로 핸들을 돌린다.
        handle.transform.eulerAngles = new Vector3(0, 0, -wheel_Z_angle);

        //자동차를 앞바퀴의 각도에 따라 회전 시킨다.
        car.transform.Rotate(new Vector3(0, wheel_Z_angle / 50, 0));
    }

    /// <summary>
    /// 두 점의 좌표값으로 각도를 구하는 함수 
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    private float getAngle(Vector3 from, Vector3 to)
    {
        return Mathf.Atan2(to.x - from.x, to.y - from.y) * Mathf.Rad2Deg;
    }

    /// <summary>
    /// 두점에 위치한 오브젝트의 좌표값으로 각도를 구하는 함수 
    /// </summary>
    /// <param name="x1"></param>
    /// <param name="y1"></param>
    /// <param name="x2"></param>
    /// <param name="y2"></param>
    /// <returns></returns>
    private float getAngle(float x1, float y1, float x2, float y2)
    {
        float dx = x2 - x1;
        float dy = y2 - y1;
        float rad = Mathf.Atan2(dx, dy);
        float degree = rad * Mathf.Rad2Deg;

        return degree;
    }

    #endregion

    #region 버튼이벤트 

    /// <summary>
    /// 악셀 up 버튼
    /// </summary>
    public void CarAccelUp()
    {
        if (speed <= 0)
        {
            DrivingAnimation.driving_Anim.ForwardAnimation();
        }
        else
        {
            DrivingAnimation.driving_Anim.ForwardAnimation();
        }

        if (saved_speed >= accelerate * 3)
        {
            saved_speed = accelerate * 3;
            speed_Check = 3;
        }
        else
        {
            if (!collision)
            {
                saved_speed = saved_speed + accelerate;
                speed_Check++;
                Driving_Sound.driving_Sound.DrivingSoundPlay(Driving_Sound.SoundType.Accel);

                if (Driving_Sound.driving_Sound.safetyBelt_audio_Source.isPlaying)
                {
                    Driving_Sound.driving_Sound.DrivingSoundStop(Driving_Sound.SoundType.Safety);
                }
            }
        }
        speed_Sensitivity = 0.01f;
        StopCoroutine(SpeedCheck());
        StartCoroutine(SpeedCheck());
    }

    /// <summary>
    /// 악셀 down 버튼
    /// </summary>
    public void CarAccelDown()
    {
        saved_speed = saved_speed - accelerate;
        speed_Check--;

        if (saved_speed <= 0.1f)
        {
            Driving_Sound.driving_Sound.DrivingSoundStop(Driving_Sound.SoundType.Accel);
            saved_speed = 0;
            speed_Check = 0;
        }
        else
        {
            if (!collision)
            {
                Driving_Sound.driving_Sound.DrivingSoundPlay(Driving_Sound.SoundType.Accel);
            }
        }

        speed_Sensitivity = 0.01f;
        StopCoroutine(SpeedCheck());
        StartCoroutine(SpeedCheck());
    }

    /// <summary>
    /// 브레이크 버튼 
    /// </summary>
    public void CarBrake()
    {
        if (saved_speed != 0)
        {
            switch (speed_Check)
            {
                case 0:
                    speed_Sensitivity = 0.05f;
                    break;
                case 1:
                    speed_Sensitivity = 0.05f;
                    break;
                case 2:
                    speed_Sensitivity = 0.035f;
                    break;
                case 3:
                    speed_Sensitivity = 0.03f;
                    break;
            }
            Driving_Sound.driving_Sound.DrivingSoundPlay(Driving_Sound.SoundType.Brake);
            DrivingAnimation.driving_Anim.IdleAnimation();
            saved_speed = 0;
            speed_Check = 0;

            StopCoroutine(SpeedCheck());
            StartCoroutine(SpeedCheck());
        }
    }

    /// <summary>
    /// 일시정지
    /// </summary>
    /// <param name="obj"></param>
    public void DrivingPause(GameObject obj)
    {
        if (obj.GetComponent<TweenAlpha>() != null)
        {
            if (!obj.GetComponent<TweenAlpha>().enabled)
            {
                if (!pause)
                {
                    obj.GetComponent<TweenAlpha>().from = 0;
                    obj.GetComponent<TweenAlpha>().to = 1;
                    Driving_Sound.driving_Sound.DrivingSoundStop(Driving_Sound.SoundType.Accel);
                    TweenManager.tween_Manager.TweenAlpha(obj);
                    Time.timeScale = 0;
                }
                else
                {
                    obj.GetComponent<TweenAlpha>().from = 1;
                    obj.GetComponent<TweenAlpha>().to = 0;
                    if (speed != 0)
                    {
                        Driving_Sound.driving_Sound.DrivingSoundPlay(Driving_Sound.SoundType.Accel);
                    }
                    TweenManager.tween_Manager.TweenAlpha(obj);
                    Time.timeScale = 1;
                }
                pause = !pause;
            }
        }
        else
        {
            if (!pause)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
        }
    }

    /// <summary>
    /// 자동차 소리만 정지
    /// </summary>
    public void AccelSoundPause()
    {
        if (!pause)
        {
            Driving_Sound.driving_Sound.DrivingSoundStop(Driving_Sound.SoundType.Accel);
        }
        else
        {
            if (speed != 0)
            {
                Driving_Sound.driving_Sound.DrivingSoundPlay(Driving_Sound.SoundType.Accel);
            }
        }
        pause = !pause;

    }

    /// <summary>
    /// 이전씬으로 이동
    /// </summary>
    public void PrevScene()
    {
        MainUI.sketch_Car = null;
        Time.timeScale = 1;
        SceneManager.LoadScene(MainUI.prev_Scene);
        // next_Scene
    }

    #endregion

    #region 충돌이벤트

    /// <summary>
    /// 벽에 충돌할 경우
    /// </summary>
    /// <param name="other"></param>
    void OnCollisionEnter(Collision other)
    {
        //충돌시 해당 지점이 벽이라면 
        if (other.gameObject.tag == "wall")
        {
            if (!addMessage)
            {
                Driving_Sound.driving_Sound.DrivingSoundStop(Driving_Sound.SoundType.Accel);
                Driving_Sound.driving_Sound.DrivingSoundPlay(Driving_Sound.SoundType.Collision);
                StopCoroutine(AddMessage());
                StartCoroutine(AddMessage());
            }
            speed = 0;
            collision = true;
            speedoMeter_needle.transform.localEulerAngles = new Vector3(0, 0, 90);
            DrivingAnimation.driving_Anim.IdleAnimation();
            Debug.Log("벽");
        }
        else
        {
            EventCheck(other);
        }
    }

    /// <summary>
    /// 계속 벽과 충돌중일 경우 
    /// </summary>
    /// <param name="other"></param>
    void OnCollisionStay(Collision other)
    {
        if (other.gameObject.tag == "wall")
        {
            speed = 0f;
        }
        else if (other.gameObject.tag == "event")
        {
            if (!other.collider.isTrigger)
            {
                other.collider.isTrigger = true;
            }
        }
    }

    /// <summary>
    /// 벽에서 벗어날 때 
    /// </summary>
    /// <param name="other"></param>
    void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag == "wall")
        {
            collision = false;
            if (speed_Check != 0)
            {
                Driving_Sound.driving_Sound.DrivingSoundPlay(Driving_Sound.SoundType.Accel);
            }

            StopCoroutine(SpeedCheck());
            StartCoroutine(SpeedCheck());
            Debug.Log("벽나감");
        }
    }

    /// <summary>
    /// 해당 이벤트 지점 다시 도착시 이벤트 또 다시 발생하도록
    /// isTrigger 를 다시 false 해준다(car와 충돌시 isTrigger가 true 되도록 되어있음)
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "event")
        {
            other.isTrigger = false;
        }
    }

    /// <summary>
    /// 이벤트 부분에 닿았을 경우 이벤트 발생여부 확인
    /// </summary>
    /// <param name="other"></param>
    private void EventCheck(Collision other)
    {
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
            DrivingAnimation.driving_Anim.MapAnimationStart(1);
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
            Driving_Sound.driving_Sound.DrivingSoundPlay(Driving_Sound.SoundType.Mission);
            //충돌하지 않도록 이벤트 발생 인식후 istrigger = true;
            other.collider.isTrigger = true;
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
            img.spriteName = "drive_Slow";
        }
        else
        {
            UISprite img = obj.GetComponent<UISprite>();
            img.spriteName = "drive_Stop";
        }
    }

    #endregion

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
            DrivingAnimation.driving_Anim.IdleAnimation();
        }
        event_Text.text = string.Format(event_Number[event_num].comment[1]);
        Driving_Sound.driving_Sound.DrivingSoundPlay(Driving_Sound.SoundType.Success);
        TweenManager.tween_Manager.TweenAlpha(event_Text.gameObject);
        mission_ing = false;
        DrivingAnimation.driving_Anim.MapAnimationStart(0);
    }

    /// <summary>
    /// 미션 실패
    /// </summary>
    /// <param name="event_num"></param>
    private void MissionFail(int event_num)
    {
        Driving_Sound.driving_Sound.DrivingSoundPlay(Driving_Sound.SoundType.Fail);
        Driving_Sound.driving_Sound.DrivingSoundPlay(Driving_Sound.SoundType.Brake);
        speed = 0;
        saved_speed = 0;
        speed_Check = 0;
        speedoMeter_needle.transform.localEulerAngles = new Vector3(0, 0, 90);
        event_Text.text = string.Format(event_Number[event_num].comment[2]);
        TweenManager.tween_Manager.TweenAlpha(event_Text.gameObject);
        mission_ing = false;
        DrivingAnimation.driving_Anim.IdleAnimation();
        DrivingAnimation.driving_Anim.MapAnimationStart(0);
    }


    /// <summary>
    /// 핸들을 돌리고 나서 핸들이 다시 제위치
    /// </summary>
    /// <returns></returns>
    private IEnumerator HandleReset()
    {
        float delay = handle.transform.eulerAngles.z;

        while (true)
        {
            if (handle.transform.eulerAngles.z > 0 - handle_Reset_Range && handle.transform.eulerAngles.z < 0 + handle_Reset_Range)
            {
                handle.transform.eulerAngles = Vector3.zero;
                yield break;
            }

            if (handle.transform.eulerAngles.z < 180)
            {
                delay -= Time.deltaTime * 100;
                handle.transform.eulerAngles = new Vector3(0, 0, delay);

            }
            else if (handle.transform.eulerAngles.z > 180)
            {
                delay += Time.deltaTime * 100;
                handle.transform.eulerAngles = new Vector3(0, 0, delay);
            }
            yield return new WaitForEndOfFrame();
        }
    }

    /// <summary>
    /// 자동차 속도 변경, 속도계 변경
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpeedCheck()
    {
        click_Accel = true;
        while (true)
        {
            if (speed < saved_speed)
            {
                speed += speed_Sensitivity;
                speedoMeter_needle.transform.localEulerAngles = new Vector3(0, 0, 90 - speed * 50);

                if (speed >= saved_speed)
                    yield break;
            }
            else
            {
                speed -= speed_Sensitivity;
                speedoMeter_needle.transform.localEulerAngles = new Vector3(0, 0, 90 - speed * 50);

                if (saved_speed == 0 && speed <= saved_speed)
                {
                    speed = 0;
                    speedoMeter_needle.transform.localEulerAngles = new Vector3(0, 0, 90);
                    click_Accel = false;
                    yield break;
                }
            }

            yield return new WaitForEndOfFrame();
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
        float event_Meter = Vector3.Distance(target_Meter.transform.position, car.transform.position) / 6;
        float limit_Event_Meter = event_Meter + 1;
        float save_Meter = 0;

        while (true)
        {
            event_Meter = Vector3.Distance(target_Meter.transform.position, car.transform.position) / 6;

            if (event_Meter > 1.0f)
            {
                if (mission_ing)
                {
                    //역주행이 아닐 경우
                    if (limit_Event_Meter >= event_Meter)
                    {
                        event_Text.text = string.Format("[b]{0:N2}m {1}[/b]", event_Meter - 1.0f, event_Number[_event_num].comment[0]);

                        //반대로 1m 거리를 역주행으로 적용하기 위해 저장한다.
                        if (event_Meter < save_Meter)
                        {
                            limit_Event_Meter = event_Meter + 1;
                        }
                    }
                    //역주행일 경우
                    else
                    {
                        event_Text.text = string.Format("[b]역주행이예요.[/b]");

                        //다시 정방향으로 돌았을경우 
                        if (save_Meter > event_Meter)
                        {
                            limit_Event_Meter = event_Meter + 1;
                        }
                        // 역주행시 역주행 메세지 발생후 1.5m 더 갔을경우 또는 이벤트 종료지점에서 10m 떨어질 경우 
                        else if (event_Meter >= limit_Event_Meter + 1.5f || event_Meter > 10.0f)
                        {
                            tagName = "";
                            mission_ing = false;
                            event_Text.text = string.Format("[b]다시 운전해 주세요.[/b]");
                            TweenManager.tween_Manager.TweenAlpha(event_Text.gameObject);
                            speed = 0;
                            CarBrake();
                            car.transform.localEulerAngles = new Vector3(0, reset_Obj.transform.localEulerAngles.z, 0);
                            car.transform.position = reset_Obj.transform.position;
                            yield break;
                        }
                    }

                    if (speed == 0 && !collision && saved_speed == 0)
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
                save_Meter = Vector3.Distance(target_Meter.transform.position, car.transform.position) / 6;
            }
            else
            {
                if (mission_ing)
                {
                    if (event_Number[_event_num].reduce_speed)
                    {
                        float now_Speed = (Mathf.Round(speed / .1f) * .1f);

                        if (now_Speed <= accelerate && !collision)
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

    private IEnumerator AddMessage()
    {
        if (car != null && !addMessage)
        {
            addMessage = true;
            GameObject newDamageText;
            newDamageText = Instantiate(collision_Prefab, Vector3.zero, Quaternion.identity) as GameObject;
            newDamageText.transform.parent = collision_Prefab_Ngui_Position.transform;
            newDamageText.transform.localScale = new Vector3(3.0f, 3.0f, 3.0f);
            newDamageText.transform.localPosition = new Vector3(0, 0, 0);
            newDamageText.transform.rotation = Camera.main.transform.rotation;
            FollowCam.followCam.ShakeStart(0.2f);
        }
        yield return new WaitForSeconds(0.5f);

        addMessage = false;
        yield break;
    }

    public void DriveSet()
    {
        for (int i = 0; i < loadModelName.Length; i++)
        {
            GlobalDataManager.ShaderRefresh(loadModel[i]);

            if (loadModel[i].name == "map")
            {
                mapModel = loadModel[i];
                mapModel.transform.position = mapModel.transform.parent.position;
            }
            else if (loadModel[i].name == "null_Car")
            {
                null_Car = loadModel[i];
                null_Car.transform.parent = car.transform;
                null_Car.transform.position = car.transform.position;
            }
            //GlobalDataManager.ShaderRefresh(parentObj.transform.FindChild(loadModelName[i]).gameObject);
        }
        DrivingInit();
    }

    /// <summary>
    /// HttpRequestDataSet 개체를 생성하며, 기본적으로 사용하는 값을 세팅.
    /// </summary>
    /// <param name="fileName">다운로드 할 파일명 (확장자 포함)</param>
    /// <returns>HttpRequestDataSet 개체</returns>    
    private HttpRequestDataSet CreateDriveAssetBundleInfotData(string fileName)
    {
        HttpRequestDataSet requestDataSet       = null;
        requestDataSet                          = new HttpRequestDataSet();

        requestDataSet.requestURL               = GlobalDataManager.GetRequestFilePath(fileName, GlobalDataManager.RequestUrlType.Server);

        requestDataSet.requestFileTitle         = GlobalDataManager.GetAssetBundleVersionFileName();
        requestDataSet.requestFileExt           = GlobalDataManager.m_ExtJsonFile;

        requestDataSet.destinationFilePath      = GlobalDataManager.GetRequestFilePath(null, GlobalDataManager.RequestUrlType.Client);
        requestDataSet.destinationFileFullPath  = GlobalDataManager.GetRequestFilePath(fileName, GlobalDataManager.RequestUrlType.Client);

        return requestDataSet;
    }

    IEnumerator ReadAssetBundleDriveMode(string assetBundleName, string[] contentsModelingNames, GameObject rootObject)
    {
        string jsonDestPath = string.Empty;
        string jsonFileName = string.Empty;

        HttpRequestDataSet infoParam    = null;

        infoParam                       = CreateDriveAssetBundleInfotData(assetBundleName);

        jsonDestPath = GlobalDataManager.GetRequestFilePath(null, GlobalDataManager.RequestUrlType.Client);
        jsonFileName = GlobalDataManager.GetAssetBundleVersionFileName();

        infoParam.contentsModelingNames = contentsModelingNames;
        infoParam.rootObject            = rootObject;

        infoParam.onRequestComplete     = LoadAssetBundleDriveMode;
        infoParam.onErrorWWW            = ErrorView;
        
        Coroutine assetLoadCoroutine    = StartCoroutine(WrapperWWW.FileDownloadWWW(infoParam));
        yield return assetLoadCoroutine;

        DriveSet();
    }

    private void LoadAssetBundleDriveMode(HttpRequestDataSet infoParam, object obj)
    {
        WWW www = obj as WWW;
        AssetBundle bundle = www.assetBundle;

        GameObject prefab = null;

        loadModel = new GameObject[infoParam.contentsModelingNames.Length];
        for (int i = 0; i < infoParam.contentsModelingNames.Length; i++)
        {
            prefab = (Instantiate(bundle.LoadAsset(infoParam.contentsModelingNames[i])) as GameObject);

            if (prefab != null)
            {
                prefab.name = prefab.name.Split('(')[0]; // (Clone) 이름 삭제
                prefab.transform.parent = infoParam.rootObject.transform;

                loadModel[i] = prefab;

                //prefab.SetActive(false);
                //GlobalDataManager.ShaderRefresh(infoParam.assetBundleCopyObjects[i]);
            }
        }

        bundle.Unload(false);
    }

    private void ErrorView(HttpRequestDataSet dataSet, string msg)
    {
        Debug.LogError(string.Format("WWWManager Error : {0}", msg));
    }

    private void AniInit(DrivingInfo driveInfo, GameObject active_car)
    {
        DrivingAnimation.driving_Anim.ani_Avatar = driveInfo.driveAvatar;
        DrivingAnimation.driving_Anim.ani_Controller = driveInfo.AniController;

        Vector3 tempPosition;

        tempPosition = car.GetComponent<CapsuleCollider>().center;
        tempPosition.z = driveInfo.headPosition.z;
        car.GetComponent<CapsuleCollider>().center = tempPosition;

        tempPosition = collision_Prefab_Position.transform.position;
        tempPosition.z = driveInfo.headPosition.z;
        collision_Prefab_Position.transform.position = tempPosition;

        active_car.transform.localPosition = driveInfo.position;
        active_car.transform.localEulerAngles = driveInfo.rotation;
        active_car.transform.localScale = driveInfo.scale;
    }

    private IEnumerator PreAnimationInit(Animation ani)
    {
        if (ani != null)
        {
            ani[ani.clip.name].time = 0.0f;
            ani.Play();
            yield return new WaitForEndOfFrame();
            ani.Stop();
        }
    }
}
