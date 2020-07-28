using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RacingDrive : MonoBehaviour
{
    #region Public Variable
    /// <summary>
    /// 운전하는 차를 따라갈 카메라
    /// </summary>
    public Camera driveCamera;

    /// <summary>
    /// 운전할 차 오브젝트
    /// </summary>
    //[HideInInspector]
    public GameObject myCarObj;

    /// <summary>
    /// 차의 회전값을 저장할 Transform
    /// </summary>
    public Transform calcRotate;

    /// <summary>
    /// 내 차의 회전에 다른 정면 속도값
    /// </summary>
    [HideInInspector]
    public Vector3 rigidVel;

    /// <summary>
    /// 고스트차량 회전 초기화값
    /// </summary>
    public Vector3 ghostCarResetRotate;

    /// <summary>
    /// 차량을 따라갈 카메라의 위치값
    /// </summary>
    public Vector3 cameraPosition;

    /// <summary>
    /// 고스트 차량 오브젝트
    /// </summary>
    [HideInInspector]
    public List<GameObject> ghostCarObj;

    /// <summary>
    /// 플레이어 차량 이동 방향 확인 인덱스
    /// </summary>
    public int moveTargetIndex = 1;

    /// <summary>
    /// 역주행 유무
    /// </summary>
    public bool warning = false;

    public float firstWarningDis = 0;

    /// <summary>
    /// 역주행 경고 위치(목표까지 거리)
    /// </summary>
    public float warningDis = 0;

    /// <summary>
    /// 목적지까지 가는데 차량이 가장 가까웠던 위치
    /// </summary>
    public float minDis = 0;

    /// <summary>
    /// 목적지까지 현재 차량 위치
    /// </summary>
    public float nowDis = 0;

    /// <summary>
    /// 사용중인 고스트차량 오브젝트
    /// </summary>
    //[HideInInspector]
    public GameObject[] usingGhostCarObj;

    /// <summary>
    /// 사용할 고스트차량의 갯수
    /// </summary>
    public int usingGhostCarCount;

    /// <summary>
    /// 네비게이션값을 가지고있는 오브젝트들의 이름(숫자를 제외한)
    /// </summary>
    public string naviObjName;

    /// <summary>
    /// 네베게이션 경로의 개수
    /// </summary>
    public int naviObjCount;

    /// <summary>
    /// 차,트랙 등 운전에 필요한 오브젝트의 부모
    /// </summary>
    public GameObject driveRoot;

    /// <summary>
    /// 터치한 위치를 저장할 Transform
    /// </summary>
    public Transform touchPos;

    [HideInInspector]
    /// <summary>
    /// 운전할 트랙
    /// </summary>
    public GameObject track;

    /// <summary>
    /// 사운드를 가지고있는 프리팹의 이름
    /// </summary>
    public string soundDataName;

    public static string myCarObjName;

    /// <summary>
    /// 번들에서 가져올 트랙 오브젝트의 이름
    /// </summary>
    public static string trackObjName;

    /// <summary>
    /// 번들에서 가져올 고스트차량의 이름(어미의 숫자제외)
    /// </summary>
    public string ghostObjName;

    //각종 소리 저장할 AudioSource
    [Space]
    public AudioSource bgmAudio;
    public AudioSource effectAudio;
    public AudioSource engineAudio;

    /// <summary>
    /// 차의 최대 속력
    /// </summary>
    public float maxVelocity;

    /// <summary>
    /// 가감속 1회당 차의 속력 증감값
    /// </summary>
    public float minVelocity;

    /// <summary>
    /// 몇바퀴 돌건지
    /// </summary>
    public int maxRap;

    [HideInInspector]
    /// <summary>
    /// 싱글모드 : false // 배틀모드 : true
    /// </summary>
    public static bool battleMode = false;

    public bool driveScene;

    /// <summary>
    /// 컨텐츠로 사용되는 차를 고스트차로 사용하는지 여부
    /// </summary>
    public bool contentsCarUse;

    #endregion

    #region Private Variable

    [SerializeField]
    /// <summary>
    /// 고스트 차량의 네비게이션 경로값을 가지고 있는 오브젝트
    /// </summary>
    private GameObject[] naviObj;

    //각종 소리들
    [HideInInspector]
    public AudioClip startSound;
    [HideInInspector]
    public AudioClip[] engineSound;
    [HideInInspector]
    public AudioClip[] crashSound;

    [HideInInspector]
    /// <summary>
    /// 현재 차의 최대속력
    /// </summary>
    public float currentMaxVelocity;

    /// <summary>
    /// 차의 현재 속력
    /// </summary>
    private float velocity;

    /// <summary>
    /// 더해줄 속력 값
    /// </summary>
    private float velocityAmount;

    /// <summary>
    /// 내 차의 돈 바퀴 수
    /// </summary>
    private int myCarRap;

    /// <summary>
    /// 고스트 차량의 돈 바퀴수
    /// </summary>
    private int[] ghostCarRap;

    /// <summary>
    /// 가속 상태인지 감속 상태인지
    /// </summary>
    private bool velocityState = false;

    /// <summary>
    /// 조이스틱이 움직이고 있는지
    /// </summary>
    private bool moveStart = false;

    /// <summary>
    /// 게임 중인지 아닌지
    /// </summary>
    [HideInInspector]
    public bool isGaming = false;

    /// <summary>
    /// 벽에 부딪혔는지 아닌지
    /// </summary>
    private bool isCrash = false;

    /// <summary>
    /// 일시정지 상태인지 아닌지
    /// </summary>
    private bool pausedGame = false;

    /// <summary>
    /// 내 차가 시작점을 통과할 준비가 됐는지(Rap수를 카운트하기 위한 변수)
    /// </summary>
    private bool isRapCountReady = false;

    /// <summary>
    /// 고스트 차가 시작점을 통과할 준비가 됐는지(Rap수를 카운트하기 위한 변수)
    /// </summary>
    private bool[] isGhostRapCountReady;

    /// <summary>
    /// 내 차의 애니 상태
    /// </summary>
    private string aniState = "front";

    public static string gameSelectSceneName;

    #endregion

    public static RacingDrive instance;

    /// <summary>
    /// RcingDriveUI를 가져옴
    /// </summary>
    private RacingDriveUI uiController;

    /// <summary>
    /// 에셋번들에서 오브젝트 로드 후 사용할 메서드를 저장할 델리게이트
    /// </summary>
    /// <param name="obj"></param>
    public delegate void objInit(GameObject obj);

    /// <summary>
    /// 델리게이트 지정
    /// </summary>
    public objInit objDel;

    [System.Serializable]
    public class NavAgentSet
    {
        public float navSpeed;
        public float navAngularSpeed;
        public float navAccel;
    }

    [SerializeField]
    public NavAgentSet[] navAgentSet;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        ghostCarObj = new List<GameObject>();

        uiController = RacingDriveUI.instance;

        isGhostRapCountReady = new bool[usingGhostCarCount];

        currentMaxVelocity = maxVelocity;
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
            myCarObj.transform.RotateAround(myCarObj.transform.position, Vector3.down, 0.5f);
        }

        if (Input.GetKey(KeyCode.D))
        {
            myCarObj.transform.RotateAround(myCarObj.transform.position, Vector3.up, 0.5f);
        }


        if (Input.touchCount == 0)
        {
            return;
        }

        

        #region 터치 이벤트
        for (int i = 0; i < Input.touchCount; i++)
        {
            //레이캐스트
            Touch touch = Input.GetTouch(i);
            Vector2 wp = UICamera.mainCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 0));
            Ray2D ray = new Ray2D(wp, Vector2.zero);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            
            if (hit.collider != null)
            {
                //엑셀레이터 터치
                if (hit.collider.tag == "accel")
                {
                    if (touch.phase == TouchPhase.Began)
                    {
                        VelocityUp();
                    }
                    else if (touch.phase == TouchPhase.Ended)
                    {
                        VelocityDown();
                    }
                }
                //조이스틱 터치
                else if (hit.collider.tag == "joystick")
                {

                    touchPos.position = wp;

                    if (touch.phase == TouchPhase.Began)
                    {
                        OnDragJoyStickStart();
                    }
                    else if (touch.phase == TouchPhase.Moved)
                    {
                        OnDragJoyStickOver();
                    }
                    else if (touch.phase == TouchPhase.Stationary)
                    {
                        OnDragJoyStickOver();
                    }
                    else if (touch.phase == TouchPhase.Canceled)
                    {
                        OnDragJoyStickEnd();
                    }
                    else if (touch.phase == TouchPhase.Ended)
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
                    if (touch.phase == TouchPhase.Moved)
                    {
                        OnDragJoyStickOver();
                    }
                    else if (touch.phase == TouchPhase.Stationary)
                    {
                        OnDragJoyStickOver();
                    }
                    else if (touch.phase == TouchPhase.Canceled)
                    {
                        OnDragJoyStickEnd();
                    }
                    else if (touch.phase == TouchPhase.Ended)
                    {
                        OnDragJoyStickEnd();
                    }
                }
            }
        }
        #endregion
    }

    #region 초기화 관련

    public void bundleObjCall()
    {
        if (driveScene)
        {
            Debug.Log("Sound Load Complete");
            //델리게이트 지정
            objDel = new objInit(TrackObjInit);

            //트랙 오브젝트 로드
            //AssetBundleLoader.getInstance.ReadAssetBundleRacing(GlobalDataManager.m_SelectedAssetBundleName, trackObjName, null, driveRoot);
        }
    }

    /// <summary>
    /// 트랙 오브젝트 초기화
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="value">true = 트랙 false = 고스트 차량</param>
    public void TrackObjInit(GameObject prefab)
    {

        if (prefab.name == trackObjName)
        {
            //가져온 트랙 프리펩을 저장
            track = prefab;

            //트랙을 지정한 위치로 이동,스케일값을 변경
            track.transform.parent = driveRoot.transform;
            track.transform.position = Vector3.zero;
            track.transform.localScale = new Vector3(10, 10, 10);
            track.transform.GetChild(0).transform.localEulerAngles = track.GetComponent<RacingTrackInfo>().initRotate;

            //트랙 비활성
            track.SetActive(false);
 
            Debug.Log("Track Load Complete");

            //델리게이트 변경
            objDel = new objInit(SelectCarInit);

            //고스트차량을 번들에서 로드
            //AssetBundleLoader.getInstance.ReadAssetBundleRacing(GlobalDataManager.m_SelectedAssetBundleName, myCarObjName, null, driveRoot);
        }
    }

    /// <summary>
    /// 선택된 내 차 초기화
    /// </summary>
    /// <param name="prefab"></param>
    public void SelectCarInit(GameObject prefab)
    {
        myCarObj = prefab;

        if(contentsCarUse)
        {
            if (myCarObj.GetComponent<NavMeshAgent>() != null)
            {
                Destroy(myCarObj.GetComponent<NavMeshAgent>());
                myCarObj.transform.eulerAngles = Vector3.zero;
                myCarObj.transform.GetChild(0).transform.eulerAngles =
                    new Vector3(0, 180, 0);
            }
        }

        //배틀모드일 경우
        if (battleMode)
        {
            if (!contentsCarUse)
            {
                //고스트차량의 이름을 저장할 문자열
                string[] initObjName = new string[track.GetComponent<RacingTrackInfo>().ghostStartPoint.Length];

                //미리 써놓은 문자열값과 숫자를 더해 순서대로 저장
                for (int i = 0; i < initObjName.Length; i++)
                {
                    initObjName[i] = string.Format(ghostObjName + (i + 1));
                }

                //델리게이트 변경
                objDel = new objInit(GhostObjInit);

                //고스트차량을 번들에서 로드
                //AssetBundleLoader.getInstance.ReadAssetBundleRacing(GlobalDataManager.m_SelectedAssetBundleName, null, initObjName, driveRoot);
            }
            else
            {
                ghostCarUseRandomCar();
            }
        }
        else
        {
            DriveInit();
        }
        Debug.Log("MyCar Load Complete");
    }

    /// <summary>
    /// 고스트 차량 초기화
    /// </summary>
    /// <param name="prefab">고스트 차량</param>
    public void GhostObjInit(GameObject prefab)
    {
        //불러온 프리펩의 이름에 미리 지정된 문자열이 들어있는지 비교(다른걸 불러올경우를 막기위해)
        if (prefab.name.Contains(ghostObjName))
        {
            //루트캆으로 이동,스케일 변경
            prefab.transform.parent = driveRoot.transform;
            prefab.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

            //고스트 차량에 rigidbody를 추가, 물리력을 사용하지 않게끔 함
            prefab.AddComponent<Rigidbody>();
            prefab.GetComponent<Rigidbody>().isKinematic = true;
            prefab.GetComponent<Rigidbody>().useGravity = false;

            //고스트 차량에 col스크립트 추가
            prefab.AddComponent<RacingCol>();

            //고스트 차량을 리스트에 저장
            ghostCarObj.Add(prefab);

            //비활성
            prefab.SetActive(false);

            Debug.Log("Ghostcar Load Complete");
        }
    }

    /// <summary>
    /// 내 차로 선택된 차량을 제외한 차를 사용할때 초기화할 매서드
    /// </summary>
    public void ghostCarUseRandomCar()
    {
        for (int i = 0; i < TargetManager.타깃메니저.에셋번들복제컨텐츠.Length; i++)
        {
            if (myCarObj.name == TargetManager.타깃메니저.에셋번들복제컨텐츠[i].name)
            {
                continue;
            }
            else
            {
                GameObject randomCar = Object.Instantiate(TargetManager.타깃메니저.에셋번들복제컨텐츠[i]);

                randomCar.SetActive(true);

                randomCar.layer = 19;
                randomCar.transform.SetChildLayer(19);

                //루트캆으로 이동,스케일 변경
                randomCar.transform.parent = driveRoot.transform;
                randomCar.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

                //고스트 차량에 rigidbody를 추가, 물리력을 사용하지 않게끔 함
                randomCar.AddComponent<Rigidbody>();
                randomCar.GetComponent<Rigidbody>().isKinematic = true;
                randomCar.GetComponent<Rigidbody>().useGravity = false;

                //고스트 차량에 col스크립트 추가
                randomCar.AddComponent<RacingCol>();

                ghostCarObj.Add(randomCar);

                randomCar.SetActive(false);
            }
        }
        Debug.Log("Ghostcar Load Complete");

        DriveInit();
    }

    /// <summary>
    /// 운전하기 초기화
    /// </summary>
    public void DriveInit()
    {
        //일시정지가 떠있다면 무시
        if (pausedGame == false)
        {
            #region 안쓰는 코드
            //if (ImageMarkerEvent.마커인식상태)
            //{
            //    isGaming = false;

            //    //비인식후 다시 인식시 모든 코루틴을 꺼주기 위해 실행
            //    StopAllCoroutines();

            //    //UI를 꺼줌
            //    uiController.RacingDriveUISetActive(true);

            //    //차 밑으로 내려갔던 카메라를 다시 원래위치로 되돌림
            //    driveCamera.transform.parent = driveRoot.transform;

            //    //복제된 오브젝트 삭제
            //    DestroyObj(myCarObj);
            //    //DestroyObj(track); 
            //    if (track != null)
            //    {
            //        track.SetActive(false);
            //    }

            //    MainUI.메인.인식글자UI.SetActive(true);

            //    //레이싱에 맞는 인식글자를 띄워줌
            //    MainUI.메인.ChangeRecognitionText(string.Format(LocalizeText.Value["RacingStart"]), false);

            //    //배틀모드의 초기화
            //    BattleModeInit(true);

            //    //사운드를 꺼줌
            //    engineAudio.Stop();
            //    effectAudio.Stop();
            //    bgmAudio.Stop();
            //}
            //else
            //{
            //    if (TargetManager.타깃메니저.첫인식상태)
            //    {
            #endregion
            
            TouchEventManager.터치.EasyTouchObj.SetActive(false);

            //차를 복사, 켜줌
            //myCarObj = Object.Instantiate(TargetManager.타깃메니저.에셋번들복제컨텐츠[TargetManager.타깃메니저.복제모델링인덱스]);
            myCarObj.SetActive(true);

            //차와 차의 하위 오브젝트의 레이어를 RacingDrive로 변경
            myCarObj.layer = 15;
            myCarObj.transform.SetChildLayer(15);
            myCarObj.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

            //차를 지정한 위치로 이동시킨 후, 강체와 Col메서드를 붙여줌
            myCarObj.transform.parent = driveRoot.transform;
            myCarObj.AddComponent<Rigidbody>();
            myCarObj.AddComponent<RacingCol>();

            //차의 애니메이션을 재생중지
            myCarObj.GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animator>().StartPlayback();

            //트랙을 복사,켜줌(마지막 배열에 트랙을 넣어야함)
            //track = Object.Instantiate(TargetManager.타깃메니저.에셋번들복제컨텐츠[TargetManager.타깃메니저.에셋번들복제컨텐츠.Length - 1]);
            track.SetActive(true);

            //차의 위치를 지도의 스타트 위치로 이동
            myCarObj.transform.position = track.GetComponent<RacingTrackInfo>().myCarStartPoint.transform.position;

            //엔진 오디오 사운드 넣어줌
            engineAudio.clip = engineSound[Random.Range(0, engineSound.Length)];

            //모든 수치 초기화(재시작했을시 중복 방지)
            myCarRap = 0;
            velocity = 0;
            velocityAmount = 0;
            moveTargetIndex = 1;
            warning = false;

            //각종 bool값 false;로 변경
            velocityState = false;
            moveStart = false;
            isCrash = false;
            pausedGame = false;
            isRapCountReady = false;


            //네비게이션 경로가 담긴 오브젝트들의 이름이 null이 아닌경우
            if (!string.IsNullOrEmpty(naviObjName))
            {
                //경로를 저장할 오브젝트 배열을 초기화
                naviObj = new GameObject[track.GetComponent<RacingTrackInfo>().trackNavPoint.transform.childCount];


                //미리 FBX에서 지정한 네비게이션 좌표 오브젝트를 가져와 저장
                for (int i = 0; i < track.GetComponent<RacingTrackInfo>().trackNavPoint.transform.childCount; i++)
                {
                    naviObj[i] = track.GetComponent<RacingTrackInfo>().trackNavPoint.transform.GetChild(i).gameObject;
                }
            }

            if (battleMode)
            {
                BattleModeInit();
            }

            //Follow Camera 실행
            driveCamera.transform.parent = myCarObj.transform;
            driveCamera.transform.position = myCarObj.transform.position + cameraPosition;
            driveCamera.transform.LookAt(myCarObj.transform.position);
            driveCamera.transform.RotateAround(myCarObj.transform.position, Vector3.up, myCarObj.transform.rotation.z);
            driveCamera.transform.eulerAngles = new Vector3(15f, driveCamera.transform.eulerAngles.y, driveCamera.transform.eulerAngles.z);

            TargetManager.타깃메니저.HideAllModelingContents();

            uiController.RacingDriveLabelSet(uiController.rapLabel, string.Format(myCarRap + " / " + maxRap));

            uiController.RacingDriveUISetActive();

            uiController.MinimapInit();
        }
        //}
        //}
    }

    /// <summary>
    /// 배틀모드일시 배틀모드에 관련된 초기화
    /// </summary>
    /// <param name="value">인식여부</param>
    public void BattleModeInit()
    {
        #region 안쓰는 코드
        //if (value)
        //{
        //    //사용했던 고스트차량이 있다면
        //    if (usingGhostCarObj != null && usingGhostCarObj.Length != 0)
        //    {
        //        //모든 고스트차량의 네비를꺼주고,비활성화,게임관련 bool값 false로 변경
        //        for (int i = 0; i < usingGhostCarCount; i++)
        //        {
        //            usingGhostCarObj[i].GetComponent<NavMeshAgent>().Stop();
        //            usingGhostCarObj[i].SetActive(false);
        //            isGhostRapCountReady[i] = false;
        //        }

        //        //Null로 초기화
        //        usingGhostCarObj = null;
        //    }

        //    naviObj = null;
        //}
        //else
        //{
        #endregion

        //고스트차량의 Rap수를 저장할 배열
        ghostCarRap = new int[usingGhostCarCount];

        //고스트 차량의 갯수를 네비게이션 세팅갯수만큼만 제한
        if (usingGhostCarCount <= navAgentSet.Length)
        {
            usingGhostCarObj = new GameObject[usingGhostCarCount];
        }

        //고스트 차량의 배열번호를 저장할 인덱스
        List<int> index = new List<int>();

        //고스트 차량 생성
        for (int i = 0; i < usingGhostCarCount; i++)
        {
            if (index.Count == 0)
            {
                //랜덤으로 배열번호 지정
                index.Add(Random.Range(0, ghostCarObj.Count));
            }
            else
            {
                //인덱스 0번을 제외한 다른 배열값은 이미 사용한 배열번호를 빼준뒤 배열번호 지정
                index.Add(RadomGhostActive(index));
            }

            //받은 배열번호로 고스트차량 저장, 활성화
            usingGhostCarObj[i] = ghostCarObj[index[i]];
            ghostCarObj[index[i]].SetActive(true);
            usingGhostCarObj[i].SetActive(true);

            //고스트 차량의 위치,회전 조정
            usingGhostCarObj[i].transform.position = track.GetComponent<RacingTrackInfo>().ghostStartPoint[i].transform.position;
            usingGhostCarObj[i].transform.eulerAngles = ghostCarResetRotate;

            //네비게이션을 껐다가 켜줌(재시작 시 그냥 가버리는 경우가 있음)
            usingGhostCarObj[i].GetComponent<NavMeshAgent>().enabled = false;
            usingGhostCarObj[i].GetComponent<NavMeshAgent>().enabled = true;

            //애니메이터를 일시적으로 중지
            usingGhostCarObj[i].GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animator>().StartPlayback();

            //고스트차량의 Rap수를 초기화
            ghostCarRap[i] = 0;
        }

        /*
        //네비게이션 경로가 담긴 오브젝트들의 이름이 null이 아닌경우
        if (!string.IsNullOrEmpty(naviObjName))
        {
            //경로를 저장할 오브젝트 배열을 초기화
            naviObj = new GameObject[track.GetComponent<RacingTrackInfo>().trackNavPoint.transform.childCount];


            //미리 FBX에서 지정한 네비게이션 좌표 오브젝트를 가져와 저장
            for (int i = 0; i < track.GetComponent<RacingTrackInfo>().trackNavPoint.transform.childCount; i++)
            {
                naviObj[i] = track.GetComponent<RacingTrackInfo>().trackNavPoint.transform.GetChild(i).gameObject;
            }
        }
        */
        //}
    }

    /// <summary>
    /// 랜덤 고스트차량 활성화
    /// </summary>
    /// <param name="index">배열 인덱스</param>
    /// <returns></returns>
    public int RadomGhostActive(List<int> index)
    {
        bool getRandom = false;

        while (true)
        {
            //랜덤값 생성
            int random = Random.Range(0, ghostCarObj.Count);

            //인덱스만큼 반복
            for (int i = 0; i < index.Count; i++)
            {
                //이미 사용중인 인덱스 값이 아니라면
                if (index[i] != random)
                {
                    //마지막 인덱스까지 왓다면
                    if (i == index.Count - 1)
                    {
                        //랜덤을 반환하기 위해 bool값 전환
                        getRandom = true;
                        break;
                    }
                    else
                    {
                        //계속 비교
                        continue;
                    }
                }
                else
                {
                    //이미 사용중이면 랜덤값을 다시 받기위해 break
                    break;
                }
            }

            //랜덤 인덱스값 반환
            if (getRandom)
            {
                return random;
            }

        }
    }

    /// <summary>
    /// 오브젝트 삭제 메서드
    /// </summary>
    /// <param name="obj"></param>
    public void DestroyObj(GameObject obj)
    {
        if (obj != null)
        {
            Destroy(obj);
        }
    }

    #endregion

    #region 게임시작 관련 메서드
    /// <summary>
    /// 게임 시작 이벤트
    /// </summary>
    public void DriveStartEvent()
    {
        uiController.startBtn.SetActive(false);
        StartCoroutine("DriveStart");
    }

    /// <summary>
    /// 게임 시작 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator DriveStart()
    {
        //시작 사운드 재생
        effectAudio.clip = startSound;
        effectAudio.Play();

        //신호등 켜기
        yield return new WaitForSeconds(1.0f);
        uiController.startLight[0].SetActive(true);
        yield return new WaitForSeconds(1.0f);
        uiController.startLight[1].SetActive(true);
        yield return new WaitForSeconds(1.0f);
        uiController.startLight[2].SetActive(true);
        yield return new WaitForSeconds(1.0f);


        //배경음 재생
        bgmAudio.Play();

        //애니메이션 재생 시작
        myCarObj.transform.GetChild(0).GetComponent<Animator>().SetTrigger("idle");
        myCarObj.GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animator>().StopPlayback();
        aniState = "idle";

        //차의 강체 고정
        myCarObj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;

        //게임 시작 UI 실행
        uiController.GameStart();

        //게임중으로 변경
        isGaming = true;

        //배틀모드라면 고스트 차량을 움직이게 만듬
        if (battleMode)
        {
            StartCoroutine("GhostCarMoveEvent");
        }

        //차량의 속력 컨트롤 및 이동 제어 시작
        //StartCoroutine("VeclocityControl");
        StartCoroutine("MoveCar");
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
        try
        {
            float radian = 0.0f;

            radian = uiController.JoyStickDragControl(touchPos);

            //차 회전 변경
            //라디안 값을 degree값으로 변경
            calcRotate.transform.eulerAngles = new Vector3(myCarObj.transform.eulerAngles.x, (-(radian * 180 / Mathf.PI - 90)), 0);

            //각 각도별로 지정해주는 앵글값에 따른 애니메이션 조작
            if (calcRotate.transform.eulerAngles.y == 0 || calcRotate.transform.eulerAngles.y == 180)
            {
                //0도 or 180도일때
                myCarObj.transform.rotation = myCarObj.transform.rotation;

                currentMaxVelocity = maxVelocity;
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

                currentMaxVelocity = maxVelocity - (calcRotate.transform.eulerAngles.y / 50);
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

                currentMaxVelocity = maxVelocity - (calcRotate.transform.eulerAngles.y / 50);
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

                currentMaxVelocity = maxVelocity + ((calcRotate.transform.eulerAngles.y - 360) / 50);
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

                currentMaxVelocity = maxVelocity + (((calcRotate.transform.eulerAngles.y - 360) / 50));
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
        myCarObj.transform.GetChild(0).GetComponent<Animator>().SetTrigger("front");
        aniState = "front";

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
        if (!isCrash && !velocityState)
        {
            velocityState = true;

            //엔진소리를 다시 내줌
            engineAudio.Pause();
            engineAudio.Play();

            //속도를 가변해줄 값이 음수인경우 0으로 변경
            if (velocityAmount <= 0)
            {
                velocityAmount = 0;
            }

            if (!moveStart)
            {
                myCarObj.transform.GetChild(0).GetComponent<Animator>().SetTrigger("front");
                aniState = "front";

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

            if (!!moveStart)
            {
                myCarObj.transform.GetChild(0).GetComponent<Animator>().SetTrigger("idle");
                aniState = "idle";
            }

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
            if (velocity < currentMaxVelocity)
            {
                velocity += velocityAmount;
                velocityAmount += minVelocity;
            }
        }
        else
        {
            //최소속력보다 큰경우만 감소시켜줌
            if (velocity > 0)
            {
                velocity -= (velocityAmount * 1.5f);
                velocityAmount += minVelocity;
            }
        }
    }

    /// <summary>
    /// 오브젝트 이동 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveCar()
    {
        rigidVel = Vector3.zero;
        float timer = 0;

        nowDis = Vector3.Distance(myCarObj.transform.position, naviObj[moveTargetIndex].transform.position);
        minDis = nowDis;

        while (true)
        {
           // Debug.Log("velocity : " + velocity);

            if (pausedGame == false)
            {
                nowDis = Vector3.Distance(myCarObj.transform.position, naviObj[moveTargetIndex].transform.position);

                //역주행
                if (warning)
                {
                    //역주행 -> 정주행
                    if (warningDis > nowDis + 2)
                    {
                        warning = false;
                        minDis = nowDis;
                        warningDis = 0;
                        firstWarningDis = 0;
                        RacingDriveUI.instance.WarningUIClose();
                    }
                    else
                    {
                        //계속 역주행의 경우 
                        if(warningDis < nowDis)
                        {
                            warningDis = nowDis;
                        }

                        if(nowDis > firstWarningDis + 15.0f)
                        {
                            Debug.Log("다시 주행하세요");

                            warning = false;
                           
                            RacingDriveUI.instance.WarningUIClose();

                            velocity = 0;
                            velocityAmount = 0;

                            if (moveTargetIndex > 0)
                            {
                                myCarObj.transform.position = naviObj[moveTargetIndex - 1].transform.position;
                                myCarObj.transform.localEulerAngles = new Vector3(myCarObj.transform.localEulerAngles.x, 
                                                                                  naviObj[moveTargetIndex - 1].transform.localEulerAngles.z, 
                                                                                  myCarObj.transform.localEulerAngles.z);
                            }
                            else
                            {
                                myCarObj.transform.position = naviObj[naviObj.Length - 1].transform.position;
                                myCarObj.transform.localEulerAngles = new Vector3(myCarObj.transform.localEulerAngles.x,
                                                                                 naviObj[naviObj.Length - 1].transform.localEulerAngles.z,
                                                                                 myCarObj.transform.localEulerAngles.z);
                            }


                            nowDis = Vector3.Distance(myCarObj.transform.position, naviObj[moveTargetIndex].transform.position);

                            minDis = nowDis;
                            warningDis = 0;
                            firstWarningDis = 0;

                        }
                    }
                }

                if (nowDis < minDis)
                {
                    minDis = nowDis;
                }

                if (nowDis <= 10.0f)
                {
                    //인덱스가 마지막 번호라면
                    if (moveTargetIndex >= naviObj.Length - 1)
                    {
                        moveTargetIndex = 0;
                    }
                    else
                    {
                        moveTargetIndex++;
                    }

                    nowDis = Vector3.Distance(myCarObj.transform.position, naviObj[moveTargetIndex].transform.position);
                    minDis = nowDis;
                }
                //역주행일 경우 
                else if(!warning && nowDis > minDis + 10.0f)
                {
                    warning = true;
                    warningDis = nowDis;
                    firstWarningDis = nowDis;
                    RacingDriveUI.instance.WarningUIOpen();
                    Debug.Log("경고");
                }

                if (timer >= 0.1f)
                {
                    timer = 0.0f;
                    VeclocityControl();
                }

                //정해진 바퀴수를 다 돌았다면
                if (myCarRap == maxRap)
                {
                    GameEndEvent();
                }

                //최대속력을 넘지 않도록 실제 속력을 고정
                if (velocity >= currentMaxVelocity)
                {
                    rigidVel = CalcRotatePosition(myCarObj.transform.rotation);
                    myCarObj.GetComponent<Rigidbody>().velocity = rigidVel * currentMaxVelocity;
                    velocity = currentMaxVelocity;
                }
                //최소 속력을 넘지 않도록 실제 속력을 고정
                else if (velocity <= 0 && !velocityState)
                {
                    myCarObj.GetComponent<Rigidbody>().velocity = Vector3.zero;

                    engineAudio.Pause();

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
                        myCarObj.GetComponent<Rigidbody>().velocity = rigidVel * velocity;
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

            uiController.GamePlayUIController();
            
            timer += Time.deltaTime;

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

    /// <summary>
    /// 고스트 차량 이동 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator GhostCarMoveEvent()
    {
        //네비게이션을 가져옴
        NavMeshAgent[] nav = new NavMeshAgent[usingGhostCarCount];
        int[] index = new int[usingGhostCarCount];
        double[] preRotate = new double[usingGhostCarCount];

        for (int i = 0; i < usingGhostCarCount; i++)
        {
            nav[i] = usingGhostCarObj[i].GetComponent<NavMeshAgent>();

            //네비게이션의 속도를 지정
            nav[i].speed = navAgentSet[i].navSpeed;
            nav[i].angularSpeed = navAgentSet[i].navAngularSpeed;
            nav[i].acceleration = navAgentSet[i].navAccel;

            index[i] = -1;

            preRotate[i] = System.Math.Round(usingGhostCarObj[i].transform.localEulerAngles.y, 2);

            usingGhostCarObj[i].GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animator>().StopPlayback();
        }


        //게임중일때
        while (isGaming)
        {
            for (int i = 0; i < usingGhostCarCount; i++)
            {

                double correntRotate = System.Math.Round(usingGhostCarObj[i].transform.localEulerAngles.y, 2);

                //애니메이션 바퀴 회전 제어
                if (preRotate[i] > correntRotate)
                {
                    usingGhostCarObj[i].GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animator>().SetTrigger("left");
                }
                else if (preRotate[i] < correntRotate)
                {
                    usingGhostCarObj[i].GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animator>().SetTrigger("right");
                }
                else
                {
                    usingGhostCarObj[i].GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animator>().SetTrigger("front");
                }

                preRotate[i] = correntRotate;

                //네비게이션을 따라감
                //인덱스가 -1일때(처음 이동할떄)
                if (index[i] == -1)
                {
                    nav[i].SetDestination(new Vector3(naviObj[0].transform.position.x, 0, naviObj[1].transform.position.z));
                    index[i] = 1;
                }
                else
                {
                    //지정한 점과 가까워졌을때 다음 지점으로 이동하게끔 해야함
                    if (Vector3.Distance(usingGhostCarObj[i].transform.position, naviObj[index[i]].transform.position) <= 3.0f)
                    {
                        //인덱스가 마지막 번호라면
                        if (index[i] >= naviObj.Length - 1)
                        {
                            index[i] = 0;
                        }
                        else
                        {
                            index[i]++;
                        }
                        nav[i].SetDestination(new Vector3(naviObj[index[i]].transform.position.x, 0, naviObj[index[i]].transform.position.z));
                    }
                }
            }
            yield return new WaitForFixedUpdate();
        }
    }

    #endregion

    #region 게임 종료 관련 메서드

    /// <summary>
    /// 게임 종료 발생
    /// </summary>
    public void GameEndEvent()
    {
        //랭킹에 기록을 추가
        uiController.ranking.Add(uiController.rapTimeLabel.text);
        uiController.rapTimeRankingLabel[uiController.ranking.Count - 1].transform.GetChild(1).GetComponent<UILabel>().color = new Color(0, 0, 255);

        myCarObj.GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animator>().StartPlayback();

        effectAudio.Stop();
        engineAudio.Stop();

        uiController.GameEndUISet();

        //모든 코루틴 정지
        StopAllCoroutines();

        bgmAudio.Stop();
    }

    /// <summary>
    /// 고스트 차량 게임 종료 메서드
    /// </summary>
    private void GhostCarEnd(GameObject ghostObj)
    {
        //랭킹에 기록
        uiController.ranking.Add(uiController.rapTimeLabel.text);

        //고스트 차량 정지
        ghostObj.GetComponent<NavMeshAgent>().Stop();
        ghostObj.GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animator>().StartPlayback();

        //고스트 차량 이동 코루틴 정지
        if (uiController.ranking.Count >= usingGhostCarCount)
        {
            StopCoroutine("GhostCarMoveEvent");
        }
    }

    #endregion

    #region 충돌,트리거 이벤트
    /// <summary>
    /// 벽 충돌 이벤트
    /// </summary>
    /// <param name="col"></param>
    public void CollisionEnter(Collision col)
    {
        if (col.transform.tag == "wall" && isGaming)
        {
            //충돌 사운드 재생
            effectAudio.clip = crashSound[Random.Range(0, crashSound.Length)];
            effectAudio.Play();

            myCarObj.transform.GetChild(0).GetComponent<Animator>().SetTrigger("idle");
            aniState = "idle";

            uiController.accel.GetComponent<UISprite>().spriteName = "drive_accel";

            //충돌상태로 변경
            isCrash = true;

            StartCoroutine("CrashStopCheck");

            //감속상태로 변경
            velocityState = false;
        }
    }

    /// <summary>
    /// 시작점 통과 이벤트
    /// </summary>
    /// <param name="col"></param>
    public void TriggerEnter(Collider col, bool whatCar, GameObject carObj)
    {
        //시작점을 통과했다면
        if (col.transform.tag == "event" && isGaming)
        {
            //내차가 통과한 경우
            if (isRapCountReady && whatCar)
            {
                isRapCountReady = false;
                myCarRap += 1;

                //Rap UI에 표시
                uiController.RacingDriveLabelSet(uiController.rapLabel, string.Format(myCarRap + " / " + maxRap));
            }
            //고스트 차량이 통과한 경우
            else if (!whatCar)
            {
                for (int i = 0; i < usingGhostCarCount; i++)
                {
                    if (carObj == usingGhostCarObj[i])
                    {
                        if (isGhostRapCountReady[i] == true)
                        {
                            isGhostRapCountReady[i] = false;
                            ghostCarRap[i] += 1;

                            //고스트 차량이 정해진 바퀴수를 다 돌았다면
                            if (ghostCarRap[i] == maxRap)
                            {
                                GhostCarEnd(usingGhostCarObj[i]);
                            }

                            return;
                        }
                    }
                }
            }

        }
        //중간 체크지점을 통과했다면
        else if (col.transform.tag == "event_ready" && isGaming)
        {
            //내차가 통과
            if (!isRapCountReady && whatCar)
            {
                //시작점을 통과할 준비상태로 변경
                isRapCountReady = true;
            }
            //고스트 차량이 통과
            else if (!whatCar)
            {
                for (int i = 0; i < usingGhostCarCount; i++)
                {
                    if (carObj == usingGhostCarObj[i])
                    {
                        if (isGhostRapCountReady[i] == false)
                        {
                            //시작점을 통과할 준비상태로 변경
                            isGhostRapCountReady[i] = true;

                            return;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 충돌 정지 상태 체크
    /// </summary>
    /// <returns></returns>
    private IEnumerator CrashStopCheck()
    {
        //while (carObj.GetComponent<Rigidbody>().velocity != Vector3.zero)
        //{
        //    yield return new WaitForFixedUpdate();
        //}

        //충돌에 1초정도 텀을 준다.
        yield return new WaitForSeconds(1.0f);

        //속도는 0으로 초기화
        velocity = 0;
        velocityAmount = 0;

        //충돌상태 해제
        isCrash = false;

        //감속상태로 변경
        velocityState = false;
    }
    #endregion

    #region 일시정지 관련

    /// <summary>
    /// 게임 정지 메서드
    /// </summary>
    public void GamePause()
    {
        //인식시에 모델이 나오는경우가 있어 숨겨줌
        TargetManager.타깃메니저.HideAllModelingContents();

        //일시정지 팝업을 켜줌
        MainUI.메인.PausePopupOpen();

        //일시정지 상태로 변경
        pausedGame = true;
    }

    /// <summary>
    /// 재시작 버튼 클릭 이벤트
    /// </summary>
    public void RetryBtnClick()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(gameSelectSceneName);

        //모든 코루틴 정지
        //StopAllCoroutines();

        ////일시정지 상태 해제
        //pausedGame = false;

        ////게임중 상태 해제
        //isGaming = false;

        //uiController.RetryBtnUIControl();

        ////운전에 사용되는 카메라 원래위치로 이동
        //driveCamera.transform.parent = driveRoot.transform;

        ////복제된 오브젝트 파괴
        //DestroyObj(myCarObj);

        ////고스트차량 비활성
        //for (int i = 0; i < usingGhostCarCount; i++)
        //{
        //    usingGhostCarObj[i].SetActive(false);
        //}

        ////트랙 비활성
        //track.SetActive(false);

        ////사운드 정지
        //engineAudio.Stop();
        //effectAudio.Stop();
        //bgmAudio.Stop();

    }

    /// <summary>
    /// 일시정지 취소버튼 클릭 이벤트
    /// </summary>
    public void CancleBtnClick()
    {
        //일시정지 해제
        pausedGame = false;

        //팝업 닫기
        MainUI.메인.PausePopupClose();
    }
    #endregion
}

