using UnityEngine;
using System.Collections;
using HedgehogTeam.EasyTouch;
using System.IO;
using System.Runtime.InteropServices;

public class LetterManager : MonoBehaviour
{
    public enum LetterFaceType
    {
        None,
        Texture,
        Video
    }

    public bool pikMode = false;

    public Camera ARCamera;

    public static LetterManager Instance = null;

    private static Vector3 LETTER_OPEN_POSITION = new Vector3(0, 0, 15);             // 편지 열림 상태 포지션 지정
    private static Vector3 LETTER_CLOSE_POSITION = new Vector3(7, 1.5f, 35);               // 편지 닫힘 상태 포지션 지정

    public LetterInfo letterInfo;
    public GameObject camModeUI;

    private bool isLetterOpened = false;                  // 편지지가 펼쳐져 있는지 여부
    private Animation ani;

    public Camera nugiCam;

    public GameObject[] capturePointEdgeObj;              // 캡쳐할 모서리 오브젝트 (배열순서 0:좌상, 1:우상, 2:좌하, 3:우하)
    private Vector3[] capturePointEdge;                   // 캡쳐할 텍스쳐의 모서리 벡터값

    private int faceTextureWidth, faceTextureHeight;      // 얼굴 텍스쳐 가로, 세로 길이

    public GameObject targetObj;
    private GameObject letterCapturePreview;

    // add N.C.Park
    [HideInInspector]
    public Texture backFaceTexture;

    public GameObject previewTextureObj;
    public GameObject previewVideoObj;

    public LetterFaceType currentFaceType { set; get; }

    public GameObject textBox;
    public GameObject faceBox;

    public GameObject openedLetterUI;                     // 편지지 열렸을때 UI (Take Pic, 앨범, 리셋 버튼)

    [HideInInspector]
    public bool albumAvailable;                          // 앨범 사용가능 여부
    private bool isfrontCamMode;                          // 전면 카메라 모드인지 여부
    private bool useCaptureMode;                          // 캡쳐기능 사용중 체크

    private int aniIndex;

    [HideInInspector]
    public bool isPreview;


    //public Texture testTex;

    TouchScreenKeyboard keyboard;

#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern void pickImageFromGallery(string transFormName, string eventFuncName);
    [DllImport("__Internal")]
    private static extern void pickVideoFromGallery(string transFormName, string eventFuncName);
#endif

    void Awake()
    {
        Instance = this;
        InitSelfiManager();

        currentFaceType = LetterFaceType.None;
    }

    void Start()
    {
        TargetManager.DelEventMarkerLost = ApplyInitLetterModeling;
    }

    void OnEnable()
    {
        EasyTouch.On_SimpleTap += EasyTouch_On_TouchUp;
        textBox.SetActive(false);
        faceBox.SetActive(false);
    }

    void OnDestroy()
    {
        UnsubscribeEvent();
        Instance = null;
    }

    void UnsubscribeEvent()
    {
        EasyTouch.On_SimpleTap -= EasyTouch_On_TouchUp;
    }

    private void InitSelfiManager()
    {
        albumAvailable = false;
        isfrontCamMode = false;
        useCaptureMode = false;
        capturePointEdge = new Vector3[capturePointEdgeObj.Length];
        camModeUI.SetActive(false);
        OnOffOpenedLetterUI(false);
    }

    public void EasyTouch_On_TouchUp(Gesture gesture)
    {
        try
        {
            if (gesture.pickedObject == null || letterInfo == null)
            {
                return;
            }

            if (gesture.pickedObject == faceBox)
            {
                //OnClickFaceBox();
                // preview (image, video)

                if (albumAvailable && currentFaceType != LetterFaceType.None)
                {
                    StopSetLetterPaper();
                    OnClickPreview();
                }
            }
            if (gesture.pickedObject == letterInfo.entireBody.gameObject)
            {
                StopSetLetterPaper();
                PlayLetterAni();
            }
            //else if (gesture.pickedObject == letterInfo.takePicBtn.gameObject)
            //{
            //    OnClickFaceBox();
            //    Debug.LogWarning("takePickBtn !!!!");
            //}
            //else if (gesture.pickedObject == letterInfo.albumBtn.gameObject)
            //{
            //    Debug.LogWarning("albumBtnBtn !!!!");                
            //}
            //else if (gesture.pickedObject == letterInfo.resetBtn.gameObject)
            //{
            //    OnClickReset();
            //    Debug.LogWarning("resetBtnBtn !!!!");
            //}
        }
        catch
        {

        }
    }

    /// <summary>
    /// 모델링이 SelfiInfo를 가지고 있는지 체크합니다.
    /// </summary>
    public bool CheckHaveSelfiInfo()
    {
        int targetNum = TargetManager.타깃메니저.복제모델링인덱스;
        targetObj = TargetManager.타깃메니저.에셋번들복제컨텐츠[targetNum];
        LetterInfo info = targetObj.GetComponent<LetterInfo>();

        if (info != null)
        {
            letterInfo = info;

            if (backFaceTexture == null)
            {
                backFaceTexture = info.photoMat.mainTexture;
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Letter 머티리얼에 색칠한 텍스쳐를 적용합니다. 
    /// </summary>
    /// <param name="coloredTexture"></param>
    public void ApplyLetterMaterial(Texture2D coloredTexture)
    {
        bool haveSelfiInfo = CheckHaveSelfiInfo();

        if (haveSelfiInfo)
        {
            //letterInfo.letterMat.mainTexture = coloredTexture;
            //letterInfo.dearMat.mainTexture      = coloredTexture;
        }
    }

    public void ResetLetterManager()
    {
        isLetterOpened = false;

        if (TargetManager.타깃메니저.스케치씬사용)
        {
            StartCoroutine(ExpandScale(targetObj));
        }

        letterInfo.entireBody.gameObject.SetActive(false);
        RotateUI.회전.회전UI_숨기기();
        //if (letterInfo != null)
        //{
        //    //letterInfo.photoBox.gameObject.SetActive(false);
        //}

        LetterComponentManager.instance.UnloadVideoPlayer();

        RecognizedAniPlay();
    }

    private void RequestPreviewImage()
    {
        ShowHideMainModeling(false);
        previewTextureObj.GetComponent<MeshRenderer>().materials[0].mainTexture = letterInfo.photoMat.mainTexture;

        if (!previewTextureObj.activeSelf)
        {
            previewTextureObj.transform.localPosition = new Vector3(0f, -25f, -0.15f);
            previewTextureObj.transform.localRotation = Quaternion.Euler(new Vector3(90f, 0f, 0f));
            previewTextureObj.transform.localScale = new Vector3(10f, 10f, 10f);

            previewTextureObj.SetActive(true);
        }

        StopSetLetterPaper();
    }

    private void RequestPreviewVideo()
    {
        ShowHideMainModeling(false);
        string loadVideoPath = LetterComponentManager.instance.mediaPlayerCtrlInst.GetCurrentFileName();

        if (!previewVideoObj.activeSelf)
        {
            previewVideoObj.transform.localPosition = new Vector3(0f, -25f, -0.15f);
            previewVideoObj.transform.localRotation = Quaternion.Euler(new Vector3(90f, 0f, 0f));
            previewVideoObj.transform.localScale = new Vector3(10f, 10f, 10f);

            previewVideoObj.SetActive(true);
        }

        previewVideoObj.GetComponent<MediaPlayerCtrl>().Load(loadVideoPath);

        StopSetLetterPaper();
    }

    private void InitLetterFaceTexture()
    {
        if (backFaceTexture == null)
        {
            return;
        }

        letterInfo.photoMat.mainTexture = backFaceTexture;
        currentFaceType = LetterFaceType.None;
    }

    public void OnClickPreview()
    {
        targetObj.GetComponent<LetterInfo>().entireBody.gameObject.SetActive(false);
        isPreview = true;

        if (albumAvailable)
        {
            // 스크린샷 미리보기, 동영상 미리보기 UI 활용
            if (currentFaceType == LetterFaceType.Texture)
            {
                OnOffOpenedLetterUI(false);
                RotateUI.회전.회전UI_숨기기();

                RequestPreviewImage();
            }
            else if (currentFaceType == LetterFaceType.Video)
            {
                RequestPreviewVideo();
            }
            else if (currentFaceType == LetterFaceType.None)
            {
                // 알림(?)
            }
        }

        StartCoroutine(OnEntireBody());
    }

    public void OnClickAlbumPic()
    {
        SendRequestToGallery(true);

        //letterInfo.photoBox.gameObject.SetActive(false);
        targetObj.GetComponent<LetterInfo>().entireBody.gameObject.SetActive(false);
    }

    public void OnClickAlbumAvi()
    {
        InitLetterVideoPlayer();
        
        if(currentFaceType == LetterFaceType.Video)
        {
            LetterComponentManager.instance.mediaPlayerCtrlInst.Stop();
        }

        SendRequestToGallery(false);

        //letterInfo.photoBox.gameObject.SetActive(false);
        targetObj.GetComponent<LetterInfo>().entireBody.gameObject.SetActive(false);
    }

    public void OnClickTyping()
    {
        InitTextLabel();
    }

    public void OnClickReset()
    {
        targetObj.GetComponent<LetterInfo>().entireBody.gameObject.SetActive(false);

        // letterface 텍스쳐 초기화
        if (currentFaceType == LetterFaceType.Texture)
        {
            InitLetterFaceTexture();
        }
        else if (currentFaceType == LetterFaceType.Video)
        {
            // unload            
            LetterComponentManager.instance.UnloadVideoPlayer();
        }
        else if (currentFaceType == LetterFaceType.None)
        {
            // 알림(?)
        }

        StartCoroutine(OnEntireBody());
        albumAvailable = false;
        currentFaceType = LetterFaceType.None;
        // test code
        //OnClickTyping();
    }

    private IEnumerator OnEntireBody()
    {
        yield return new WaitForSeconds(0.3f);

        targetObj.GetComponent<LetterInfo>().entireBody.gameObject.SetActive(true);
    }


    /// <summary>
    /// 얼굴 박스 클릭 이벤트
    /// </summary>
    public void OnClickFaceBox()
    {
        textBox.SetActive(false);
        faceBox.SetActive(false);
        useCaptureMode = true;
        targetObj.SetActive(false);
        OnOffOpenedLetterUI(false);

        MainUI.메인.애니메이션동작_UI숨기기();
        RotateUI.회전.회전UI_숨기기();
        RotateUI.회전.콜라이더_비활성화();
        textBox.GetComponent<BoxCollider2D>().enabled = false;
        faceBox.GetComponent<BoxCollider2D>().enabled = false;
        //StopSetLetterPaper();

        // 에디터에서는 회전 안시킴(테스트 하기위해서)
        if (!isfrontCamMode && Application.platform != RuntimePlatform.WindowsEditor)
        {
            MainUI.메인.카메라변경();
            isfrontCamMode = !isfrontCamMode;
        }

        //SwitchCamMode();
        //isfrontCamMode = true;
        camModeUI.SetActive(true);
        pikMode = true;

    }

    private void InitLetterVideoPlayer()
    {
        int targetNum = TargetManager.타깃메니저.복제모델링인덱스;
        targetObj = TargetManager.타깃메니저.에셋번들복제컨텐츠[targetNum];
        LetterInfo info = targetObj.GetComponent<LetterInfo>();

        if (info == null)
        {
            return;
        }

        if (info.videoPlayerObj == null)
        {
            return;
        }

        if (info.videoPlayerObj.transform.childCount <= 0)
        {
            LetterComponentManager.instance.AddVideoObject(info.videoPlayerObj);
        }
    }

    private void InitTextLabel()
    {
        int targetNum = TargetManager.타깃메니저.복제모델링인덱스;
        targetObj = TargetManager.타깃메니저.에셋번들복제컨텐츠[targetNum];
        LetterInfo info = targetObj.GetComponent<LetterInfo>();

        if (info == null)
        {
            return;
        }

        //if (info.textAreaObj == null)
        //{
        //    return;
        //}

        //if (info.textAreaObj.transform.childCount <= 0)
        //{
        //    LetterComponentManager.instance.AddTextLabelObject(info.textAreaObj);
        //}
    }

    public void OnClickSelfiButton()
    {
        textBox.SetActive(true);
        faceBox.SetActive(true);
        camModeUI.SetActive(false);

        SetCaptureTextureLength();
        StartCoroutine(CaptureScreen());

        //CaptureScreen();
        //SwitchCamMode();

        // 에디터에서는 회전 안시킴(테스트 하기위해서)
        if (isfrontCamMode && Application.platform != RuntimePlatform.WindowsEditor)
        {
            MainUI.메인.카메라변경();
            isfrontCamMode = !isfrontCamMode;
        }

        targetObj.SetActive(true);

        if (TargetManager.타깃메니저.스케치씬사용)
        {
            ani.PlayQueued(AnimationManager.애니메이션.애니클립[3].name, QueueMode.PlayNow);
            ani.PlayQueued(AnimationManager.애니메이션.애니클립[2].name, QueueMode.CompleteOthers);
        }
        AnimationManager.애니메이션.saveAniClipNum = 2;

        MainUI.메인.애니메이션동작_UI보이기();
        RotateUI.회전.회전UI_보이기();
        RotateUI.회전.콜라이더_활성화();
        OnOffOpenedLetterUI(true);
        textBox.GetComponent<BoxCollider2D>().enabled = true;
        faceBox.GetComponent<BoxCollider2D>().enabled = true;
        //StartSetLetterPaper();

        if (currentFaceType == LetterFaceType.Video)
        {
            LetterComponentManager.instance.UnloadVideoPlayer();
        }

        if (TargetManager.타깃메니저.스케치씬사용)
        {
            StartCoroutine(ReduceScale(targetObj));
        }
        currentFaceType = LetterFaceType.Texture;

        albumAvailable = true;
        useCaptureMode = false;

        pikMode = false;
        // 테스트
        //LetterComponentManager.instance.PlayLetterVideo(string.Empty, string.Empty);
    }

    public void SwitchCamMode()
    {
        MainUI.메인.카메라변경();
        isfrontCamMode = !isfrontCamMode;

        if (isLetterOpened)
        {
            TargetManager.타깃메니저.비인식후_좌표값 = LETTER_OPEN_POSITION;
        }
        else
        {
            TargetManager.타깃메니저.비인식후_좌표값 = LETTER_CLOSE_POSITION;
        }

        RotateUI.회전.컨텐츠_회전_초기화();
    }

    /// <summary>
    /// 캡쳐할 텍스쳐 가로, 세로 길이를 설정합니다.
    /// </summary>
    private void SetCaptureTextureLength()
    {
        for (int i = 0; i < capturePointEdgeObj.Length; i++)
        {
            capturePointEdge[i] = capturePointEdgeObj[i].transform.position;
            capturePointEdge[i] = nugiCam.WorldToScreenPoint(capturePointEdge[i]);
        }

        faceTextureWidth = (int)(capturePointEdge[1].x - capturePointEdge[0].x);
        faceTextureHeight = (int)(capturePointEdge[0].y - capturePointEdge[3].y);
    }

    /// <summary>
    /// 카메라 이미지를 캡처 합니다.
    /// </summary>
    private IEnumerator CaptureScreen()
    {
        Texture2D captureTex = new Texture2D(faceTextureWidth, faceTextureHeight, TextureFormat.RGB24, false);
        Camera arCam = Camera.main; // ColoringManager.컬러링매니저.arCam;
        arCam.GetComponent<Camera>().Render();

        RenderTexture.active = arCam.GetComponent<Camera>().targetTexture;

        int startPosX = (int)capturePointEdge[0].x;
        int startPosY = (int)(capturePointEdge[3].y);

        Rect captureRect = new Rect(startPosX, startPosY, faceTextureWidth, faceTextureHeight);
        captureTex.ReadPixels(captureRect, 0, 0, false);
        captureTex.Apply();

        yield return new WaitForEndOfFrame();

        RenderTexture.active = null;
        letterInfo.photoMat.mainTexture = captureTex;

        //letterCapturePreview.GetComponent<MeshRenderer>().material.mainTexture = captureTex;

        yield return new WaitForEndOfFrame();

        string fileName = "HansScreenShot";
        string albumName = "HansApp";

        string date = System.DateTime.Now.ToString("hh-mm-ss_dd-MM-yy");
        string screenshotFilename = fileName + "_" + date + ".jpg";
        string path = Application.persistentDataPath + "/" + screenshotFilename;

        if (Application.platform == RuntimePlatform.Android)
        {
            string androidPath = Path.Combine(albumName, screenshotFilename);
            path = Path.Combine(Application.persistentDataPath, androidPath);
            string pathonly = Path.GetDirectoryName(path);
            Directory.CreateDirectory(pathonly);
        }

        byte[] bytes = captureTex.EncodeToJPG();
        ScreenshotManager.Instance.StartSave(bytes, fileName, path);
    }

    private void PlayLetterAni()
    {
        ani = AnimationManager.애니메이션.애니;

        ani.wrapMode = WrapMode.Once;

        if (isLetterOpened)
        {
            // 편지지를 닫고 헤엄치는 애니 실행
            ani.CrossFadeQueued(AnimationManager.애니메이션.애니클립[3].name, 0, QueueMode.PlayNow);
            //ani.CrossFadeQueued(AnimationManager.애니메이션.애니클립[0].name, 1.0f, QueueMode.CompleteOthers);

           // ani.clip = AnimationManager.애니메이션.애니클립[3];
          //  ani.wrapMode = WrapMode.Once;
          //  ani.Play();

            aniIndex = 0;
            AnimationManager.애니메이션.saveAniClipNum = 0;

            if (TargetManager.타깃메니저.스케치씬사용)
            {
                // 타겟오브젝트 스케일 확대
                StartCoroutine(ExpandScale(targetObj));
            }
            //letterInfo.photoBox.gameObject.SetActive(false);

            //if (currentFaceType != LetterFaceType.None)
            //{
            //SetFunctionButtonStatus(false);
            //SetFunctionButtonColliderStatus(false);
            //}

            OnOffOpenedLetterUI(false);

            if (currentFaceType == LetterFaceType.Video)
            {
                LetterComponentManager.instance.mediaPlayerCtrlInst.Stop();

                string clipName = string.Empty;

                clipName = AnimationManager.애니메이션.애니클립[3].name;

                StartCoroutine(ActiveVideoObject(ani, clipName, isLetterOpened));
            }
            StopSetLetterPaper();
        }
        else
        {
            // 편지지를 열고 정지시키는 애니 실행
            
            ani.CrossFadeQueued(AnimationManager.애니메이션.애니클립[1].name, 0, QueueMode.PlayNow);
            //ani.CrossFadeQueued(AnimationManager.애니메이션.애니클립[2].name, 1.0f, QueueMode.CompleteOthers);

           // ani.clip = AnimationManager.애니메이션.애니클립[1];
           // ani.wrapMode = WrapMode.Once;
          //  ani.Play();


            aniIndex = 2;
            AnimationManager.애니메이션.saveAniClipNum = 2;

            if (TargetManager.타깃메니저.스케치씬사용)
            {
                // 타겟오브젝트 스케일 축소
                StartCoroutine(ReduceScale(targetObj));
            }
            //letterInfo.photoBox.gameObject.SetActive(true);
            

            StartCoroutine("SetLetterPaper");
        }

        StartCoroutine("aniStateCheck");

        // 비디오 오브젝트 상태를 변경 (비활성화 / 활성화)
        //if (currentFaceType == LetterFaceType.Video)
        //{
        //    string clipName = string.Empty;

        //    if (isLetterOpened)
        //    {
        //        clipName = AnimationManager.애니메이션.애니클립[3].name;
        //    }
        //    else
        //    {
        //        clipName = AnimationManager.애니메이션.애니클립[2].name;
        //    }

        //    StartCoroutine(ActiveVideoObject(ani, clipName, isLetterOpened));
        //}

        isLetterOpened = !isLetterOpened;
    }

    private IEnumerator ActiveVideoObject(Animation ani, string clipName, bool status)
    {
        while (true)
        {
            if (ani.IsPlaying(clipName))
            {
                LetterComponentManager.instance.videoPlayerRootObj.SetActive(!status);
                if (status == false)
                {
                    LetterComponentManager.instance.mediaPlayerCtrlInst.Pause();
                }

                yield break;
            }

            yield return null;
        }
    }

    /// <summary>
    /// 스케일 확대
    /// </summary>
    private IEnumerator ExpandScale(GameObject obj)
    {
        float timer = 0;
        float maxSize = 1f;             // 최대크기
        float growFactor = 1f;          // 확대되는 속도 조정용 (default : 1)

        while (maxSize > obj.transform.localScale.x)
        {
            timer += Time.deltaTime;
            obj.transform.localScale += Vector3.one * Time.deltaTime * growFactor;
            yield return null;
        }

        TargetManager.타깃메니저.비인식후_좌표값 = LETTER_CLOSE_POSITION;
        RotateUI.회전.컨텐츠_회전_초기화();
    }

    /// <summary>
    /// 스케일 축소
    /// </summary>
    private IEnumerator ReduceScale(GameObject obj)
    {
        float timer = 0;
        float minSize = 0.6f;           // 최소크기
        float growFactor = 1f;          // 촉소되는 속도 조정용 (default : 1)

        while (minSize < obj.transform.localScale.x)
        {
            timer += Time.deltaTime;
            obj.transform.localScale -= Vector3.one * Time.deltaTime * growFactor;
            yield return null;
        }

        TargetManager.타깃메니저.비인식후_좌표값 = LETTER_OPEN_POSITION;
        RotateUI.회전.컨텐츠_회전_초기화();
    }

    public void OnOffLetterCaptureBox()
    {
        bool isShow = letterCapturePreview.activeSelf;

        if (isShow)
        {
            letterCapturePreview.SetActive(false);
        }
        else
        {
            letterCapturePreview.SetActive(true);
        }
    }

    private void SetFunctionButtonStatus(bool isActive)
    {
        //letterInfo.TakePicBtnObj.SetActive(isActive);
        //letterInfo.AlbumBtnObj.SetActive(isActive);
        //letterInfo.ResetBtnObj.SetActive(isActive);
    }

    private void SetFunctionButtonColliderStatus(bool isActive)
    {
        //letterInfo.takePicBtn.gameObject.SetActive(isActive);
        //letterInfo.albumBtn.gameObject.SetActive(isActive);
        //letterInfo.resetBtn.gameObject.SetActive(isActive);     
    }

    public void ApplyInitLetterModeling(int index)
    {
        if (pikMode)
        {
            return;
        }

        Debug.Log("비인식");

        if (TargetManager.타깃메니저.스케치씬사용)
        {
            if (ColoringManager.컬러링매니저.GetCaptureStatus())
            {
                TouchEventManager.터치.기준콜라이더 = letterInfo.entireBody.gameObject;

                letterInfo.entireBody.gameObject.SetActive(true);
                RotateUI.회전.회전UI_보이기();

                InitLetterFaceTexture();
                //SetFunctionButtonStatus(true);
                //SetFunctionButtonColliderStatus(true);
            }
        }
        else
        {
            Debug.Log("비인식2");
            TargetManager.타깃메니저.컨텐츠최상위오브젝트.transform.parent = TargetManager.타깃메니저.비인식후_좌표오브젝트.transform;
            TouchEventManager.터치.기준콜라이더 = letterInfo.entireBody.gameObject;

            letterInfo.entireBody.gameObject.SetActive(true);
            RotateUI.회전.회전UI_보이기();

            InitLetterFaceTexture();
        }
    }

    public void ShowHideMainModeling(bool isShow)
    {
        int targetNum = TargetManager.타깃메니저.복제모델링인덱스;
        targetObj = TargetManager.타깃메니저.에셋번들복제컨텐츠[targetNum];

        targetObj.SetActive(isShow);
    }

    public void OnOffOpenedLetterUI(bool isActive)
    {
        openedLetterUI.SetActive(isActive);
    }

    public IEnumerator aniStateCheck()
    {
        while (true)
        {
            if (!ani.isPlaying)
            {
                ani.clip = AnimationManager.애니메이션.애니클립[aniIndex];
                ani.wrapMode = WrapMode.Loop;
                ani.Play();
                break;
            }

            yield return new WaitForFixedUpdate();
        }
    }

    public IEnumerator SetLetterPaper()
    {
        yield return new WaitForSeconds(letterInfo.letterTextOpenFrame);

        OnOffOpenedLetterUI(true);
        textBox.SetActive(true);
        faceBox.SetActive(true);

        Transform textLeftTopPos;
        Transform textRightBottomPos;

        textBox.GetComponent<UILabel>().width = letterInfo.textboxWidth;
        textBox.GetComponent<UILabel>().height = letterInfo.textboxHeight;

        Transform faceLeftTopPos;
        Transform faceRightBottomPos;

        faceBox.GetComponent<UISprite>().width = letterInfo.faceboxWidth;
        faceBox.GetComponent<UISprite>().height = letterInfo.faceboxHeight;

        if (currentFaceType == LetterFaceType.Video)
        {
            if (LetterComponentManager.instance.mediaPlayerCtrlInst.IsPlaying() == false)
            {
                LetterComponentManager.instance.SetActiveVideoPlayer(true);

                string clipName = string.Empty;

                clipName = AnimationManager.애니메이션.애니클립[2].name;

                StartCoroutine(ActiveVideoObject(ani, clipName, !isLetterOpened));

                LetterComponentManager.instance.mediaPlayerCtrlInst.Stop();
                LetterComponentManager.instance.mediaPlayerCtrlInst.Play();
            }
        }

        while (true)
        {
            textLeftTopPos = letterInfo.textPosLeftTop.transform;
            textRightBottomPos = letterInfo.textPosRightBottom.transform;

            textBox.transform.position = new Vector3((textRightBottomPos.position.x + textLeftTopPos.position.x) / 2, (textRightBottomPos.position.y + textLeftTopPos.position.y) / 2, ((textRightBottomPos.position.z + textLeftTopPos.position.z) / 2) - 0.1f);

            textBox.transform.eulerAngles = new Vector3((textRightBottomPos.eulerAngles.x + textLeftTopPos.eulerAngles.x) / 2, ((textRightBottomPos.eulerAngles.y + textLeftTopPos.eulerAngles.y) / 2), ((textRightBottomPos.eulerAngles.z + textLeftTopPos.eulerAngles.z) / 2));

            faceLeftTopPos = letterInfo.faceboxLeftTop.transform;
            faceRightBottomPos = letterInfo.faceboxRightBottom.transform;

            faceBox.transform.position = new Vector3((faceRightBottomPos.position.x + faceLeftTopPos.position.x) / 2, (faceRightBottomPos.position.y + faceLeftTopPos.position.y) / 2, ((faceRightBottomPos.position.z + faceLeftTopPos.position.z) / 2) - 0.1f);

            faceBox.transform.eulerAngles = new Vector3((faceRightBottomPos.eulerAngles.x + faceLeftTopPos.eulerAngles.x) / 2, ((faceRightBottomPos.eulerAngles.y + faceLeftTopPos.eulerAngles.y) / 2), ((faceRightBottomPos.eulerAngles.z + faceLeftTopPos.eulerAngles.z) / 2));

            yield return new WaitForFixedUpdate();
        }
    }

    public void RecognizedAniPlay()
    {
        //AnimationManager.애니메이션.애니메이션01_재생();

        int 인덱스 = TargetManager.타깃메니저.복제모델링인덱스;
        GameObject 오브젝트 = TargetManager.타깃메니저.에셋번들복제컨텐츠[인덱스];

        오브젝트.GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animation>().clip = 오브젝트.GetComponent<ModelInfo>().애니메이션정보.애니클립[0];
        오브젝트.GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animation>().Play();
    }

    #region 안드로이드 갤러리/iOS 포토라이브러리 호출
    private void SendRequestToGallery(bool isImage)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidJavaClass pluginClass = new AndroidJavaClass("com.hansapp.pickfromgallery.UnityPlugin");

            if (isImage)
            {
                pluginClass.CallStatic("pickImageFromGallery", transform.name, "SelectedImageFromGallery");
            }
            else
            {
                pluginClass.CallStatic("pickVideoFromGallery", transform.name, "SelectedVideoFromGallery");
            }
        }
#if UNITY_IOS
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (isImage)
            {
                pickImageFromGallery(transform.name, "SelectedImageFromGallery");
            }
            else
            {
                pickVideoFromGallery(transform.name, "SelectedVideoFromGallery");
            }
        }
#endif
        else
        {
            Debug.LogWarning("갤러리를 지원하지 않는 플랫폼 입니다.");
        }
    }

    public void GetVideoGalleryPath()
    {
        SendRequestToGallery(false);
    }

    public void GetImageGalleryPath()
    {
        SendRequestToGallery(true);
    }

    /// <summary>
    /// 갤러리에서 선택한 리소스 경로 받는 콜백 함수
    /// </summary>
    /// <param name="path"></param>
    public void SelectedImageFromGallery(string path)
    {
        if (!string.IsNullOrEmpty(path))
        {
            // WWW 클래스를 이용하여 사용자가 선택한 파일을 로드           
            string loadPath = string.Empty;
            loadPath = string.Format("file:///{0}", path);

            OnClickReset();
            StartCoroutine(LoadingImageFile(letterInfo, loadPath));
        }
        else
        {
            // 사용자가 파일 선택을 취소했거나 오류가 발생한 경우                          
            //letterInfo.photoBox.gameObject.SetActive(true);
            targetObj.GetComponent<LetterInfo>().entireBody.gameObject.SetActive(true);
        }
    }

    public void SelectedVideoFromGallery(string path)
    {
        if (!string.IsNullOrEmpty(path))
        {
            // WWW 클래스를 이용하여 사용자가 선택한 파일을 로드            
            OnClickReset();
            LoadingVideoFile(path);
        }
        else
        {
            // 사용자가 파일 선택을 취소했거나 오류가 발생한 경우            
            //letterInfo.photoBox.gameObject.SetActive(true);
            targetObj.GetComponent<LetterInfo>().entireBody.gameObject.SetActive(true);


            if (currentFaceType == LetterFaceType.Video)
            {
                LetterComponentManager.instance.mediaPlayerCtrlInst.Play();
            }
        }
    }

    private void LoadingVideoFile(string videoFullPath)
    {
        string videoFilePath = string.Empty;
        string videoFilenName = string.Empty;

        string videoFileTitle = string.Empty;
        string audioFileExt = string.Empty;
        string audioFilePath = string.Empty;
        string audioFullPath = string.Empty;

        int findSlashIndex = -1;
        int findDotIndex = -1;

        findSlashIndex = videoFullPath.LastIndexOf("/");

        if (findSlashIndex == -1)
        {
            return;
        }

        videoFilePath = videoFullPath.Substring(0, findSlashIndex);
        videoFilenName = videoFullPath.Substring((findSlashIndex + 1), (videoFullPath.Length - (findSlashIndex + 1)));

        findDotIndex = videoFilenName.LastIndexOf(".");

        if (findDotIndex == -1)
        {
            return;
        }

        audioFileExt = "wav";
        videoFileTitle = videoFilenName.Substring(0, findDotIndex);
        audioFullPath = string.Format("{0}/{1}.{2}", videoFilePath, videoFileTitle, audioFileExt);

        albumAvailable = true;
        currentFaceType = LetterFaceType.Video;

        //letterInfo.photoBox.gameObject.SetActive(true);
        targetObj.GetComponent<LetterInfo>().entireBody.gameObject.SetActive(true);

        LetterComponentManager.instance.PlayLetterVideo(videoFullPath, audioFullPath);
    }


    private IEnumerator LoadingImageFile(LetterInfo letterInfo, string url)
    {
        Texture2D tex;
        tex = new Texture2D(4, 4, TextureFormat.DXT1, false);
        using (WWW www = new WWW(url))
        {
            yield return www;
            www.LoadImageIntoTexture(tex);

            letterInfo.photoMat.mainTexture = tex;
            currentFaceType = LetterFaceType.Texture;
            albumAvailable = true;

            //letterInfo.photoBox.gameObject.SetActive(true);
            targetObj.GetComponent<LetterInfo>().entireBody.gameObject.SetActive(true);
        }
    }

    public void ResetLetterSetting()
    {
        if (targetObj.name.Equals("Bluefish_sketch"))
        {
            OnOffOpenedLetterUI(false);
            previewTextureObj.SetActive(false);
            camModeUI.SetActive(false);
            isLetterOpened = false;

            //if (letterInfo != null)
            //{
            //    //letterInfo.photoBox.gameObject.SetActive(false);
            //}
        }
    }

    // 녹화한 영상을 '미리보기 재생' 할 경우 편지지 오브젝트 비활성화, 확대보기 재생 중이였으면 언로드 및 비활성화 처리
    // '미리보기 재생' 을 닫기 할 경우 편지지 오브젝트 활성화
    public void LetterVideoSoundOnOff(GameObject obj)
    {
        if (currentFaceType == LetterFaceType.Video)
        {
            if (string.Compare(obj.name, "미리보기 재생") == 0)
            {
                ShowHideMainModeling(false);

                if (previewVideoObj.activeSelf)
                {
                    previewVideoObj.GetComponent<MediaPlayerCtrl>().UnLoad();
                    previewVideoObj.SetActive(false);
                }
            }
            else if (string.Compare(obj.name, "[버튼] 닫기") == 0)
            {
                ShowHideMainModeling(true);
            }
        }
    }

    public void touchTextbox()
    {
        StartCoroutine("InputTextBox");
    }

    private IEnumerator InputTextBox()
    {
        TouchScreenKeyboard.hideInput = true;
        keyboard = TouchScreenKeyboard.Open(textBox.GetComponent<UIInput>().value, TouchScreenKeyboardType.Default);
        Debug.Log("진입");

        keyboard.text = "";

        while (true)
        {
            if (keyboard != null)
            {
                if (keyboard.done)
                {
                    Debug.Log("Done");
                    keyboard = null;
                    TouchScreenKeyboard.hideInput = false;
                    break;
                }

                if (textBox.GetComponent<UILabel>().text != keyboard.text)
                {
                    Debug.Log("Text");
                    textBox.GetComponent<UIInput>().value = keyboard.text;
                }

                Debug.Log("Text = " + keyboard.text);

                yield return new WaitForFixedUpdate();
            }
        }
    }

    public void StartSetLetterPaper()
    {
        textBox.SetActive(true);
        faceBox.SetActive(true);
        StartCoroutine("SetLetterPaper");
    }

    public void StopSetLetterPaper()
    {
        StopCoroutine("SetLetterPaper");
        textBox.SetActive(false);
        faceBox.SetActive(false);
    }

    #endregion

    public void SetLetterTextScale(GameObject go, Vector3 objectScale)
    {
        Vector3 textScale = new Vector3(go.transform.localScale.x / objectScale.x, go.transform.localScale.y / objectScale.y, textBox.transform.localScale.z);
        textBox.transform.localScale = textScale;
    }

    public void CLoseTakePicUI()
    {
        textBox.SetActive(true);
        faceBox.SetActive(true);
        useCaptureMode = false;
        targetObj.SetActive(true);
        OnOffOpenedLetterUI(true);

        MainUI.메인.애니메이션동작_UI숨기기();
        RotateUI.회전.회전UI_보이기();
        textBox.GetComponent<BoxCollider2D>().enabled = true;
        faceBox.GetComponent<BoxCollider2D>().enabled = true;
        StartSetLetterPaper();

        // 에디터에서는 회전 안시킴(테스트 하기위해서)
        if (!isfrontCamMode && Application.platform != RuntimePlatform.WindowsEditor)
        {
            MainUI.메인.카메라변경();
            isfrontCamMode = !isfrontCamMode;
        }

        //SwitchCamMode();
        //isfrontCamMode = true;
        camModeUI.SetActive(false);
    }

    //public void AlbumPicTest()
    //{
    //    targetObj.GetComponent<LetterInfo>().entireBody.gameObject.SetActive(false);
    //    OnClickReset();

    //    letterInfo.photoMat.mainTexture = testTex;
    //    currentFaceType = LetterFaceType.Texture;
    //    albumAvailable = true;

    //    //letterInfo.photoBox.gameObject.SetActive(true);
    //    targetObj.GetComponent<LetterInfo>().entireBody.gameObject.SetActive(true);

    //}

    public void AlbumAviTest()
    {
        InitLetterVideoPlayer();

        albumAvailable = true;
        currentFaceType = LetterFaceType.Video;

        //letterInfo.photoBox.gameObject.SetActive(true);
        targetObj.GetComponent<LetterInfo>().entireBody.gameObject.SetActive(true);

        LetterComponentManager.instance.PlayLetterVideo(Application.streamingAssetsPath + "/1.mp4", Application.streamingAssetsPath + "/1.mp3");
    }
}
