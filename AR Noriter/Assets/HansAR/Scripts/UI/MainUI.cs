using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using Vuforia;

public class MainUI : MonoBehaviour
{
    //public bool 전시회모드 = false;
    public bool sceneModeDrive;

    /// <summary>
    /// NGUI 카메라 오브젝트를 받습니다.
    /// </summary>
    public GameObject 메뉴UI;
    public GameObject 종료팝업;
    public GameObject 뒤로가기팝업;

    public GameObject 오버레이UI;
    public GameObject 메인UI;
    public GameObject 튜토리얼UI;
    public GameObject 탐색UI;
    public GameObject 공부하기UI;
    public GameObject 인식글자UI;
    public GameObject 딜레이팝업UI;
    public GameObject 동요UI;

    public GameObject 튜토리얼열기;
    public GameObject 튜토리얼닫기;

    public GameObject 메인하위UI;
    public GameObject 탐색하위UI;

    public GameObject 알아보기UI;
    public GameObject 질문답변UI;

    public GameObject 공주인형뽑기UI;
    public GameObject 공주미니맵UI;

    public GameObject 한글듣고익히기UI;
    public GameObject 알파벳듣고익히기UI;

    public GameObject 색칠하기UI;

    public GameObject 애니동작UI;
    public GameObject 애니버튼목록;
    public GameObject 동영상UI;
    public GameObject 화면녹화UI;

    public GameObject navigationUI;

    public GameObject uiEventLinkManager;

    public GameObject[] 애니메이션버튼;

    //public GameObject quizModeBtnUI;
    //public GameObject[] quizModeBtn;

    public GameObject 한스로고UI;

    /// <summary>
    /// AR화면에선 운전하기버튼 UI // 운전모드에선 운전핸들 및 차량 UI
    /// </summary>
    public GameObject 운전하기UI;
    public GameObject 운전하기자동차;

    public GameObject pausePopup;

    public UIToggle 설명보기체크박스;
    public UILabel 타겟개수라벨;
    private bool 설명보기알파값 = false;
    private bool 다시보기체크 = false;
    private int 설명다시보지않음 = 0;

    private bool 메인하위UI상태 = false;
    private bool 탐색하위UI상태 = false;
    private bool 애니동작UI상태 = false;

    private CameraDevice.CameraDirection 카메라방향 = CameraDevice.CameraDirection.CAMERA_BACK; // 카메라 디바이스 후면

    public static int 방향상태; // 카메라 디바이스 전면, 후면, 가로세로 체크
    public float UI숨기는시간 = 10.0f;
    public Camera Sub카메라;

    private Vector3 처음비인식좌표값저장;
    private Vector3 처음비인식사이즈값저장;

    public GameObject bottomMenuSideA;
    public GameObject bottomMenuSideB;

    private int changeBottomMenuStatus = 0;                // 메인하위 메뉴 상태 변경 체크

    private Coroutine popupCoroutine;

    public static string prev_Scene;
    public static GameObject sketch_Car;

    public static MainUI 메인;

    private UILabel recognitionLabel;           // 인식글자 라벨

    [HideInInspector]
    public bool isDefaultRecognitionText;       // 인식글자가 기본 문구인지 체크

    public GameObject blackBG;

    void Awake()
    {
        메인 = this;

        UiObjectNullCheck();
        DontDestroyOnLoad(운전하기자동차);

        // 안쓰는 메인 끄기
        UI_상태변경(메인UI, false);
        UI_상태변경(딜레이팝업UI, true);

        UI_상태변경(인식글자UI, true);

        //// iOS일 경우..
        //if (Application.platform == RuntimePlatform.IPhonePlayer)
        //{
        //    UI_상태변경(튜토리얼UI, true); // 튜토리얼 UI 닫기
        //}
        UI_상태변경(튜토리얼UI, false);

        UI_상태변경(메인하위UI, false);
        UI_상태변경(탐색하위UI, false); // Main 하위 UI 닫기
        UI_상태변경(오버레이UI, false); // 오버레이 UI 닫기

        UI_상태변경(탐색UI, false); // 슬라이드쇼 UI 닫기
        UI_상태변경(공부하기UI, false); // 랭귀지(공부하기, 언어공부) UI 닫기
        UI_상태변경(알아보기UI, false); // 알아보기 UI
        UI_상태변경(화면녹화UI, true);

        UI_상태변경(애니동작UI, false); // 애니메이션 동작 UI
        //UI_상태변경(질문답변UI, false); // 묻고답하기 UI

        UI_상태변경(bottomMenuSideA, false);
        UI_상태변경(bottomMenuSideB, false);
        UI_상태변경(공주인형뽑기UI, false);
        UI_상태변경(공주미니맵UI, false);

        UI_상태변경(운전하기UI, false);

        UI_상태변경(navigationUI, true);

        방향상태 = 0;
    }

    void Start()
    {
        recognitionLabel = 인식글자UI.GetComponentInChildren<UILabel>();
        isDefaultRecognitionText = true;

        //권기영 : 아이폰이나 윈도우에디터의 경우
        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            GetData();
            if (설명다시보지않음 != 0)
            {
                다시보기체크 = true;
            }
            else
            {
                다시보기체크 = false;
            }
        }
        //권기영 : 안드로이드의 경우 
        else
        {
            다시보기체크 = true;
        }

        if (!sceneModeDrive)
        {
            처음비인식좌표값저장 = TargetManager.타깃메니저.비인식후_좌표값;
            처음비인식사이즈값저장 = TargetManager.타깃메니저.비인식후_사이즈값;            
        }
    }

    //권기영 : 처음시작시 튜토리얼 체크여부 확인 저장
    void SaveData()
    {
        PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "kw", 설명다시보지않음);
        PlayerPrefs.Save();
    }

    //권기영 : 튜토리얼 체크여부 확인하기 위해 가져옵니다.
    void GetData()
    {
        설명다시보지않음 = PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "kw");
    }



    #region ##### public 함수 #####

    /// <summary>
    /// 튜토리얼 LINK 버튼 클릭시 홈페이지로 이동합니다.
    /// </summary>
    public void 튜토리얼_홈페이지이동()
    {
        Application.OpenURL("http://hansapp.co.kr/page/5_1.php?code=iphone&part=1");
    }

    /// <summary>
    /// 튜토리얼 페이지를 활성화시킵니다.
    /// </summary>
    public void 튜토리얼_열기()
    {
        튜토리얼_중지();

        튜토리얼열기.SetActive(true);
        popupCoroutine = StartCoroutine(ExplanationPopUp(true));
    }

    /// <summary>
    /// 메인 버튼을 클릭 했을경우 하위 UI 이벤트 입니다.
    /// </summary>
    public void 메인하위UI_컨트롤()
    {
        UI_상태변경(화면녹화UI, 메인하위UI상태);
        UI_상태변경(메인하위UI, 메인하위UI상태 = !메인하위UI상태);
    }

    public void 튜토리얼_중지()
    {
        if (popupCoroutine != null)
        {
            StopCoroutine(popupCoroutine);
            popupCoroutine = null;
        }
    }


    /// <summary>
    /// 튜토리얼 페이지를 닫습니다.
    /// </summary>
    public void 튜토리얼_닫기()
    {
        튜토리얼_중지();
        popupCoroutine = StartCoroutine(ExplanationPopUp(false));
    }

    /// <summary>
    /// 탐색 버튼을 클릭 했을경우 하위 UI 이벤트 입니다.
    /// </summary>
    public void 탐색하위UI_컨트롤()
    {
        UI_상태변경(탐색하위UI, 탐색하위UI상태 = !탐색하위UI상태);
    }

    /// <summary>
    /// 메인 하단 메뉴 상태를 변경합니다
    /// </summary>
    public void ChangeBottomMenuStatus()
    {
        switch (changeBottomMenuStatus)
        {
            case 0:
                UI_상태변경(bottomMenuSideA, true);
                UI_상태변경(bottomMenuSideB, false);

                changeBottomMenuStatus = 1;

                break;

            case 1:
                UI_상태변경(bottomMenuSideA, false);
                UI_상태변경(bottomMenuSideB, true);

                changeBottomMenuStatus = 2;

                break;

            case 2:
                UI_상태변경(bottomMenuSideA, false);
                UI_상태변경(bottomMenuSideB, false);

                changeBottomMenuStatus = 0;

                break;
        }
    }

    /// <summary>
    /// UI 를 보이거나, 닫거나 합니다.
    /// </summary>
    public void UI_상태변경(GameObject 오브젝트, bool 상태)
    {
        if (오브젝트 != null)
        {
            오브젝트.SetActive(상태);
        }
    }

    /// <summary>
    /// 한스 홈페이지를 웹뷰로 오픈 합니다.
    /// </summary>
    public void 홈페이지이동()
    {
        Application.OpenURL("http://www.hansapp.co.kr/");
    }

    /// <summary>
    /// 한스 카페를 웹뷰로 오픈 합니다.
    /// </summary>
    public void 카페이동()
    {
        Application.OpenURL("http://cafe.naver.com/hansarworld");
    }

    /// <summary>
    /// 스크린샷을 찍어 갤러리에 저장 합니다.
    /// </summary>
    public void 스크린샷저장()
    {
        메뉴UI.SetActive(false);
        //한스로고UI.SetActive(true);

        if (Application.platform == RuntimePlatform.WindowsEditor ||
            Application.platform == RuntimePlatform.OSXEditor)
        {
            Application.CaptureScreenshot("HansScreenShot.jpg");
        }
        else
        {
            // 파일이름, 앨범이름, 저장형식
            ScreenshotManager.SaveScreenshot("HansScreenShot", "HansApp", ".jpg");
        }

        

        Invoke("NGUI카메라UI_보이기", 1.0f);
    }

    /// <summary>
    /// 캠화면을 전몇 및 후면 카메라로 변경 합니다.
    /// </summary>
    public void 카메라변경()
    {
        GameObject obj = null;
        //메인하위UI_컨트롤();

        // 카메라 방향상태를 체크
        if (방향상태 != 0)
        {
            // 후면일경우 전면으로 적용
            카메라방향 = CameraDevice.CameraDirection.CAMERA_BACK;

            // 방향 0은 기본 카메라
            방향상태 = 0;

        
            if (TargetManager.타깃메니저.observeManager != null)
            {
                if(ObserveManager.instance.labelObjRoot.transform.Find("comment") != null)
                {
                    obj = ObserveManager.instance.labelObjRoot.transform.Find("comment").gameObject;
                    obj.GetComponent<UISprite>().transform.localScale = new Vector3(1, 1, 1);
                }
            }
            
        }
        else
        {
            카메라방향 = CameraDevice.CameraDirection.CAMERA_FRONT;

            // 전면일경우 후면 가로, 세로 체크하여 적용
            if (Screen.orientation == ScreenOrientation.Portrait)
            {
                // 방향 2는 셀카모드 세로 상태
                방향상태 = 2;
            }
            else
            {
                // 방향 1은 셀카모드 가로 상태
                방향상태 = 1;
            }
            
            /*
            if (TargetManager.타깃메니저.observeManager != null)
            {
                if (ObserveManager.instance.labelObjRoot.transform.Find("comment").gameObject != null)
                {
                    obj = ObserveManager.instance.labelObjRoot.transform.Find("comment").gameObject;
                    obj.GetComponent<UISprite>().transform.localScale = new Vector3(1, -1, 1);
                }
            }
            */
            
        }

        CameraDevice.Instance.Stop();
        CameraDevice.Instance.Deinit();
        CameraDevice.Instance.Init(카메라방향);
        CameraDevice.Instance.Start();

        //ObjChangeSetting(방향상태);

        // 적용을 위해 초기화
        RotateUI.회전.컨텐츠_회전_초기화();        
    }

    /// <summary>
    /// 씬 사용 도중, 다른 씬으로 넘어가기 위한 메서드
    /// </summary>
    /// <param name="click_object">로드할 씬이름을 얻어올 오브젝트</param>
    public void LoadAnotherScene(GameObject click_object)
    {
        GameObject obj = Instantiate(TargetManager.타깃메니저.에셋번들복제컨텐츠[TargetManager.타깃메니저.복제모델링인덱스],
           Vector3.zero, transform.rotation) as GameObject;
        if (AnimationManager.애니메이션.애니 != null)
        {
            AnimationManager.애니메이션.애니.Stop();
        }

        if (!obj.activeSelf)
        {
            obj.SetActive(true);
        }
        obj.transform.parent = 운전하기자동차.transform;

        sketch_Car = 운전하기자동차;

        //지금 실행중인 씬의 이름을 다음씬에 넘겨줌(필요없다면 생략가능)
        prev_Scene = SceneManager.GetActiveScene().name;

        //ButtonInfo에 로드할 씬이름이 비어있지 않다면
        if (click_object.GetComponent<ButtonInfo>().loadSceneName != "")
        {
            //SceneManager.LoadScene(click_object.GetComponent<ButtonInfo>().loadSceneName);

            ButtonInfo btnInfo = click_object.GetComponent<ButtonInfo>();
            if (btnInfo != null)
            {
                GlobalDataManager.m_SelectedSceneName = string.Format("{0}_{1}", GlobalDataManager.m_SelectedCategoryEnum.ToString(), btnInfo.loadSceneName);
                GlobalDataManager.m_AssetBundlePartName = btnInfo.assetBundleName;
            }

            //GlobalDataManager.GlobalInvokeLoadScene(0.3f);   // 씬 이동           
            GlobalDataManager.GlobalLoadScene();
            //참고, 씬 이름이 틀리다면 빌드 세팅 에러가 남.
        }
        else
        {
            Debug.LogError("LoadSceneName is NULL");
        }
    }

    #endregion



    #region ##### public 함수 #####

    /// <summary>
    /// 어플리케이션을 종료 합니다.
    /// </summary>
    public void 프로그램종료()
    {
        종료팝업_닫기();
        Application.Quit();
    }

    public void 종료팝업_닫기()
    {

        TweenAlpha 트윈알파 = 종료팝업.GetComponent<TweenAlpha>();
        Destroy(트윈알파);

        종료팝업.SetActive(false);

        if (SideMenuUI.Instance != null)
        {
            SideMenuUI.Instance.TurnOffBlackBG();
        }
    }

    public void 종료팝업_열기()
    {
        종료팝업.GetComponent<UIPanel>().alpha = 0; // 패널 알파 초기화
        TweenAlpha.Begin(종료팝업, 0.3f, 1); // 트윈 알파 컨포넌트 추가

        종료팝업.SetActive(true);
    }

    public void 뒤로가기팝업_열기()
    {
        뒤로가기팝업.GetComponent<UIPanel>().alpha = 0; // 패널 알파 초기화
        TweenAlpha.Begin(뒤로가기팝업, 0.3f, 1); // 트윈 알파 컨포넌트 추가

        뒤로가기팝업.SetActive(true);
    }

    public void 뒤로가기팝업_닫기()
    {
        TweenAlpha 트윈알파 = 뒤로가기팝업.GetComponent<TweenAlpha>();
        Destroy(트윈알파);

        뒤로가기팝업.SetActive(false);

        if (SideMenuUI.Instance != null)
        {
            SideMenuUI.Instance.TurnOffBlackBG();
        }
    }

    public void 뒤로가기_메인()
    {
        TargetManager.타깃메니저.postcardEnd = true;
        TargetManager.EnableTracking = false;
        OnBlackBackGround();

        //Invoke("씬로딩", 0.3f);

        씬로딩();
    }

    /// <summary>
    /// 모든 UI를 보이게 합니다.
    /// </summary>
    private void MenuUI_ShowAll()
    {
        UI_상태변경(메인UI, true); // 메인메뉴 UI
    }

    private void NGUI카메라UI_보이기()
    {
        //한스로고UI.SetActive(false);
        메뉴UI.SetActive(true);
    }

    public void 메인메뉴이동()
    {
        OnBlackBackGround();

        //딜레이팝업UI.SetActive(true);

        // 색칠저장기능 사용시 가비지컬렉터 실행
        if (GetComponent<ColoringManager>() != null)
        {
            if (ColoringManager.컬러링매니저.buttonTypeSave || ColoringManager.컬러링매니저.slideTypeSave)
            {
                GC.Collect();
            }
        }

        씬로딩();
    }

    private void 자동회전적용()
    {
        // 자동 회전 적용
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = false;
    }

    /// <summary>
    /// 검정색 백그라운드 화면을 켜줍니다.
    /// </summary>
    private void OnBlackBackGround()
    {
        if (blackBG != null)
        {
            blackBG.SetActive(true);
        }
        else
        {
            Debug.Log("돌아가기 백그라운드를 지정해주세요.");
        }
    }

    private void 씬로딩()
    {
        //SceneManager.LoadScene("01. Main");
        SceneManager.LoadSceneAsync("01. HansMain");
    }

    private bool FindTargetManagerObject(string name)
    {
        object findObj = null;

        findObj = GameObject.Find(name).GetComponent<TargetManager>();

        if (findObj != null)
        {
            return true;
        }

        return false;
    }

    private int GetAllTargetCount()
    {
        int itemCount   = 0;       
        itemCount       = TargetManager.타깃메니저.타깃정보.Length;

        return itemCount;
    }

    public void 애니메이션동작_UI숨기기()
    {
        애니동작UI.SetActive(false);
    }

    public void 애니메이션동작_UI보이기()
    {
        애니동작UI.SetActive(true);
    }

    public void 애니메이션동작_버튼()
    {
        애니동작UI상태 = !애니동작UI상태;
        애니버튼목록.SetActive(애니동작UI상태);
    }

    public void 애니메이션동작_목록보이기(int 개수)
    {
        // 버튼 모두 숨기기
        for (int i = 0; i < 애니메이션버튼.Length; i++)
        {
            애니메이션버튼[i].SetActive(false);
        }

        if(개수 <= 1)
        {
            return;
        }

        애니버튼목록.SetActive(애니동작UI상태 = true);

        // 애니메이션 개수 만큼 버튼 열기
        for (int i = 0; i < 애니메이션버튼.Length; i++)
        {
            if (i < 개수)
            {
                애니메이션버튼[i].SetActive(true);
                //Debug.Log("애니이름 : " + 애니이름목록[i]);
            }
        }
    }

    public void 색칠하기UI_열기()
    {
        색칠하기UI.SetActive(true);
    }
    public void 색칠하기UI_닫기()
    {
        색칠하기UI.SetActive(false);
    }

    public void 비활성화_함수호출()
    {
        if (TouchEventManager.터치 != null)
        {
            TouchEventManager.터치.비활성화_호출();
        }
    }

    //권기영 : 다시보지 않음을 체크했는지 안했는지 확인해서 저장
    public void 설명다시보지않음확인()
    {
        //권기영 : 이전에 
        if (!다시보기체크)
        {
            if (설명보기체크박스.value == true)
            {
                설명보기체크박스.gameObject.SetActive(false);
                설명다시보지않음 = 1;
                SaveData();
            }

        }
        튜토리얼_닫기();
    }

    //권기영 : 시작시 튜토리얼을 띄웁니다.
    public void 처음튜토리얼띄우기()
    {
        // "인식가능 타겟 개수 : "
        타겟개수라벨.text = LocalizeText.Value["TargetCount_F"] + GetAllTargetCount();
        if (다시보기체크)
        {
            튜토리얼_닫기();
            설명보기체크박스.gameObject.SetActive(false);
        }
        else
        {
            튜토리얼_열기();
            비활성화_함수호출();
        }
    }

    //전, 후방 카메라 전환 버튼 클릭시 함수 
    public void ObjChangeSetting(int num)
    {
        Vector3 destPosition = Vector3.zero;
        Vector3 destScale = Vector3.zero;

        //아이폰일때
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            //정면 세로 
            if (num == 1)
            {
                if (Sub카메라 != null)
                {
                    Sub카메라.transform.localEulerAngles = new Vector3(0, 0, Sub카메라.transform.localEulerAngles.z - 180);
                }

                destPosition = new Vector3(처음비인식좌표값저장.x, -처음비인식좌표값저장.y, 처음비인식좌표값저장.z);
                destScale = new Vector3(-처음비인식사이즈값저장.x, 처음비인식사이즈값저장.y, 처음비인식사이즈값저장.z);
            }
            else
            {
                if (Sub카메라 != null)
                {
                    Sub카메라.transform.localEulerAngles = new Vector3(0, 0, 0);
                }
                //후면
                if (num == 0)
                {
                    destPosition = new Vector3(처음비인식좌표값저장.x, 처음비인식좌표값저장.y, 처음비인식좌표값저장.z);
                    destScale = new Vector3(처음비인식사이즈값저장.x, 처음비인식사이즈값저장.y, 처음비인식사이즈값저장.z);
                }
                //정면
                else
                {
                    destPosition = new Vector3(-처음비인식좌표값저장.x, 처음비인식좌표값저장.y, 처음비인식좌표값저장.z);
                    destScale = new Vector3(-처음비인식사이즈값저장.x, 처음비인식사이즈값저장.y, 처음비인식사이즈값저장.z);
                }
            }
        }
        //아이폰이 아닐경우(안드로이드) 
        else
        {
            //정면 가로
            if (num == 2)
            {
                if (Sub카메라 != null)
                {
                    Sub카메라.transform.localEulerAngles = new Vector3(0, 0, Sub카메라.transform.localEulerAngles.z - 180);
                }

                destPosition = new Vector3(처음비인식좌표값저장.x, -처음비인식좌표값저장.y, 처음비인식좌표값저장.z);
                destScale = new Vector3(-처음비인식사이즈값저장.x, 처음비인식사이즈값저장.y, 처음비인식사이즈값저장.z);
            }
            else
            {
                if (Sub카메라 != null)
                {
                    Sub카메라.transform.localEulerAngles = new Vector3(0, 0, 0);
                }
                //후면
                if (num == 0)
                {

                    destPosition = new Vector3(처음비인식좌표값저장.x, 처음비인식좌표값저장.y, 처음비인식좌표값저장.z);
                    destScale = new Vector3(처음비인식사이즈값저장.x, 처음비인식사이즈값저장.y, 처음비인식사이즈값저장.z);
                }
                //정면
                else
                {
                    destPosition = new Vector3(-처음비인식좌표값저장.x, 처음비인식좌표값저장.y, 처음비인식좌표값저장.z);
                    destScale = new Vector3(-처음비인식사이즈값저장.x, 처음비인식사이즈값저장.y, 처음비인식사이즈값저장.z);
                }
            }
        }

        TargetManager.타깃메니저.비인식후_좌표값 = destPosition;
        TargetManager.타깃메니저.비인식후_사이즈값 = destScale;        
    }

    /// <summary>
    /// 인식글자UI를 상태를 설정합니다.
    /// </summary>
    public void SetActiveRecognitionUI(bool status)
    {
        인식글자UI.SetActive(status);
    }

    /// <summary>
    /// 인식글자 문구를 변경합니다.
    /// </summary>
    /// <param name="sentence">변경할 문구</param>
    /// <param name="isDefault">디폴트 상태체크</param>
    public void ChangeRecognitionText(string sentence, bool isDefault)
    {
        recognitionLabel.text = sentence;
        isDefaultRecognitionText = isDefault;
    }

    /// <summary>
    /// OverlayUI를 켜거나 끕니다.
    /// </summary>
    public void OnOffOverlayUI(bool state)
    {
        if (state)
        {
            //인식한 모델의 개수가 2개 이상일 경우에만 오버레이ui를 보여줍니다.
            if (TargetManager.타깃메니저.슬라이드사용 && TargetManager.타깃메니저.모델컨텐츠저장.Count >= 2)
            {
                오버레이UI.SetActive(true);
            }
        }
        else
        {
            오버레이UI.SetActive(false);
        }
    }

    public void UIAllUnActive()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            UI_상태변경(튜토리얼UI, true); // 튜토리얼 UI 닫기
        }
        UI_상태변경(메인하위UI, false);
        UI_상태변경(탐색하위UI, false); // Main 하위 UI 닫기
        UI_상태변경(오버레이UI, false); // 오버레이 UI 닫기

        UI_상태변경(탐색UI, false); // 슬라이드쇼 UI 닫기
        UI_상태변경(공부하기UI, false); // 랭귀지(공부하기, 언어공부) UI 닫기
        UI_상태변경(알아보기UI, false); // 알아보기 UI
        UI_상태변경(화면녹화UI, true);

        UI_상태변경(애니동작UI, false); // 애니메이션 동작 UI
        //UI_상태변경(질문답변UI, false); // 묻고답하기 UI

        UI_상태변경(bottomMenuSideA, false);
        UI_상태변경(bottomMenuSideB, false);
        UI_상태변경(공주인형뽑기UI, false);
        UI_상태변경(공주미니맵UI, false);

        UI_상태변경(운전하기UI, false);
    }

    /// <summary>
    /// 로딩 팝업을 꺼줍니다.
    /// </summary>
    public IEnumerator CloseLoadingPopUp()
    {
        // 느린 디바이스에서 멈추는 현상때문에 1초 딜레이를 줌
        yield return new WaitForSeconds(0.5f);
        UI_상태변경(튜토리얼열기, true);
        처음튜토리얼띄우기();

        // 팝업창 끔
        UI_상태변경(딜레이팝업UI, false);
    }

    public IEnumerator ExplanationPopUp(bool select)
    {
        while (true)
        {
            if (select)
            {
                튜토리얼열기.GetComponent<UIPanel>().alpha += 0.1f;

                if (튜토리얼열기.GetComponent<UIPanel>().alpha >= 1)
                {

                    yield break;
                }
            }
            else
            {
                튜토리얼열기.GetComponent<UIPanel>().alpha -= 0.1f;

                if (튜토리얼열기.GetComponent<UIPanel>().alpha <= 0)
                {
                    튜토리얼열기.SetActive(false);
                    yield break;
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }
    #endregion

    /*
    /// <summary>
    /// 문제 버튼 세팅(스티커,퀴즈,퍼즐 등)
    /// </summary>
    /// <param name="count"></param>
    public void ContentsBtnSetting(int count)
    {
        if (count <= 1)
        {
            Debug.LogWarning("The number of problems is one or less.");
            return;
        }

        //맵 모드가 아닌경우
        if (TargetManager.타깃메니저.SceneMode != GlobalDataManager.SceneState.MAP)
        {
            if (quizModeBtn != null)
            {
                //문제 갯수별로 문제 선택버튼 활성
                for (int i = 0; i < quizModeBtn.Length; i++)
                {
                    if (i < count)
                    {
                        quizModeBtn[i].SetActive(true);
                    }
                    else
                    {
                        quizModeBtn[i].SetActive(false);
                    }
                }
            }
            else
            {
                Debug.LogWarning("Contents Btn is Null!! Please insert Btn");
            }
        }
        else
        {
            Debug.LogWarning("Scene Mode is Map. Map Mode does not use Contents Button.");
        }
    }

    /// <summary>
    /// 문제 버튼 클릭 이벤트
    /// </summary>
    /// <param name="clickedObj">클릭버튼 이름을 가져옴</param>
    public void CallQuestion(GameObject clickedObj)
    {
        //Enum과 클릭한 오브젝트의 이름을 비교합니다.
        foreach (SketchBookTargetInfo.QuizNum value in Enum.GetValues(typeof(SketchBookTargetInfo.QuizNum)))
        {
            //같다면
            if (clickedObj.name.ToLower() == Convert.ToString(value).ToLower())
            {

                //이전의 문제를 닫음
                SketchBookUI.getInstance.Ui_Off();

                //퀴즈퀴즈가 아니라면
                if (TargetManager.타깃메니저.SceneMode != GlobalDataManager.SceneState.QUIZ_QUIZ)
                {
                    //띄우기 위한 문제의 state를 받아옴
                    SketchBookUI.getInstance.DragManager.GetComponent<StickerManager>().m_QuestionNum = value;
                }
                else
                {
                    ContentsBtnDisable();
                    //Raady상태로 변경
                    QuizQuizManager.getInstance.m_QuizState = QuizQuizManager.QuizState.READY;

                    //띄우기 위한 문제의 state를 받아옴
                    QuizQuizManager.getInstance.m_QuizNum = value;

                    //퀴즈모드 초기화
                    TargetManager.타깃메니저.QuizQuizMode(true);
                }
                break;
            }
        }

        //UI를 켬
        SketchBookUI.getInstance.Ui_On(TargetManager.타깃메니저.SceneMode);
        인식글자UI.SetActive(false);
    }

    public void ContentsBtnEnable()
    {
        for (int i = 0; i < quizModeBtn.Length; i++)
        {
            if (quizModeBtn[i] != null)
            {
                quizModeBtn[i].GetComponent<BoxCollider2D>().enabled = true;
            }
            else
            {
                break;
            }
        }
    }

    private void ContentsBtnDisable()
    {
        for (int i = 0; i < quizModeBtn.Length; i++)
        {
            if (quizModeBtn[i] != null)
            {
                quizModeBtn[i].GetComponent<BoxCollider2D>().enabled = false;
            }
            else
            {
                break;
            }
        }
    }
    */

    /// <summary>
    /// hud 카메라 범위 설정
    /// </summary>
    /// <param name="state"></param>
    public void HudCamArea(UiEventLinkManager.CameraCaptureArea state)
    {
        if (uiEventLinkManager != null)
        {
            uiEventLinkManager.GetComponent<UiEventLinkManager>().HudCamAreaSet(state);
        }
    }


    public void ButtonStateChange(GameObject changeObj, bool state)
    {
        if (changeObj != null)
        {
            changeObj.SetActive(state);
        }
    }

    /// <summary>
    /// UI오브젝트가 빠져있는지 체크합니다.
    /// </summary>
    private void UiObjectNullCheck()
    {
        if (딜레이팝업UI == null)
        {
            Debug.LogWarning("딜레이팝업UI가 빠져있습니다. MainUI 확인바람");
        }
    }

    public void PausePopupOpen()
    {
        if (pausePopup != null)
        {
            pausePopup.SetActive(true);
        }
        else
        {
            Debug.LogError("Pause Popup is NULL!!");
        }
    }

    public void PausePopupClose()
    {
        if (pausePopup != null)
        {
            pausePopup.SetActive(false);
        }
        else
        {
            Debug.LogError("Pause Popup is NULL!!");
        }
    }

    public void NavigationUIOpen()
    {
        if (navigationUI != null)
        {
            navigationUI.SetActive(true);
            NavigationUI.Instance.ShortCutUIActivate(true);
        }
        else
        {
            Debug.LogError("NavigationUI is Null!! Please put in the navigationUI in MainUI");
        }
    }

    public void NavigationUIClose()
    {
        if (navigationUI != null)
        {
            NavigationUI.Instance.ShortCutUIActivate(false);
        }
        else
        {
            Debug.LogError("NavigationUI is Null!! Please put in the navigationUI in MainUI");
        }
    }

    public void UIOpen(GameObject uiObj)
    {
        if (uiObj != null)
        {
            uiObj.SetActive(true);
        }
        else
        {
            Debug.LogWarning("uiObj is Null!! please Check Object");
        }
    }

    public void UIClose(GameObject uiObj)
    {
        if (uiObj != null)
        {
            uiObj.SetActive(false);
        }
        else
        {
            Debug.LogWarning("uiObj is Null!! please Check Object");
        }
    }

    /// <summary>
    /// 녹화 메시지 On Off
    /// </summary>
    public void OnOffRecMessage(bool activate)
    {
        if (uiEventLinkManager != null)
        {
            GameObject labelObj = uiEventLinkManager.GetComponent<UiEventLinkManager>()._statusLabel;
            labelObj.SetActive(activate);
        }
    }
}
