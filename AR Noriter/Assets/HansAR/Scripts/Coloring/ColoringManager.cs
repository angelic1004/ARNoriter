using UnityEngine;
using System.Collections;
using System.IO;
using System;
using Vuforia;
using System.Collections.Generic;
using HansAR;
using UnityEngine.SceneManagement;

public class ColoringManager : MonoBehaviour
{
    // delegate declaration
    public delegate void ColoringRecognitionData(GameObject modelObj, Texture2D readTexture);

    public static event ColoringRecognitionData OnRecognitionData;
    //-- delegate 선언

    public static ColoringManager 컬러링매니저;

    // 색칠 추출시 영역 퍼센트 지정 (변환텍스처의 퍼센트)
    public int sketchWidthPercent = 100;
    public int sketchHeightPercent = 100;

    /// <summary>
    /// 이미지타겟 전체를 읽어야 하는 경우 지정
    /// </summary>
    public int[] nocutTargetNum = new int[0];

    // 대상 이미지타겟의 모서리
    private Transform[] targetEdge;

    public GameObject checkPanel;                        // 체크 패널 (색칠 완료되었는지 체크)
    public Material bluePanelMat;                        // 파란색 패널 머터리얼
    public Material redPanelMat;                         // 빨간색 패널 머터리얼

    public Camera arCam;                                 // AR카메라
    private Transform arCamParent;                       // AR 카메라 상위 (ARCamera)

    // 모서리 2D 스크린 좌료로 저장
    private Vector3[] screenEdge;

    [HideInInspector]
    public GameObject modelObj;                          // 모델 오브젝트

    [HideInInspector]
    public Material coloringMat;                         // 색칠 머터리얼
    private Transform pixelSave;                         // 픽셀 저장

    public GameObject camAngleObj;                       // 카메라 앵글 체크용 오브젝트
    private float camAngle;

    // 변환 텍스처크기
    private int setTextureWidth, setTextureHeight;

    // 색칠 추출하는 영역
    private int extHeight;
    private int extWidth;

    /// <summary>
    /// AR 카메라 사이즈
    /// </summary>
    private float screenWidth, screenHeight;
    private int deviceWidth;                              // 단말기 화면 너비 (가로/세로 전환 체크용)

    [HideInInspector]
    public int savedTargetIndex;                         // 인덱스저장

    public GameObject[] sketchSaveBtn;                   // 색칠저장버튼 (숫자)

    /// <summary>
    /// 색칠저장기능 슬라이드방식
    /// </summary>
    public bool slideTypeSave = true;

    /// <summary>
    /// 색칠저장기능 버튼방식
    /// </summary>
    public bool buttonTypeSave = false;

    private Texture2D convertedImage;                   // 변환된 이미지

    // 그림 저장용 딕셔너리
    private Dictionary<int, int> savedTexKey = new Dictionary<int, int>();
    private Dictionary<int, Texture2D> savedTexDic = new Dictionary<int, Texture2D>();
    private int currentTargetIndex;

    private float captureDelay = 0.5f;                  // 색칠하기 캡쳐 딜레이
    private float setCaptureDelay = 0.5f;               // 색칠하기 캡쳐 딜레이 셋팅

    private bool sketchFinish;                           // 색칠캡처여부
    private bool runningCaptureCoroutine = false;       // 색칠하기 코루틴이 진행중인지 체크
    private Transform currentTarget;
    public bool reverseSketchMode;                      // 좌우 반전캡쳐 모드

    private Texture2D captureImage;
    private Camera colorCam;

    private GameObject copyModelobj;
    private GameObject coloringCapture;

    private Dictionary<int, Texture2D> originalTex;                     // 색칠 원본 텍스쳐

    public string partName;

    private bool isCompleteAssetLoad = false;

    void Awake()
    {
        컬러링매니저 = this;

        // event variable     
        OnRecognitionData = null;

        //InitSketchValues();
        //CheckMinimapUsage();
    }

    void Start()
    {
        CheckMinimapUsage();
    }

    void OnEnable()
    {
        StartCoroutine(SetAssetBundleContents());
        TargetManager.DelEventMarkerFound = SetMarketRecognitionEvent;
        TargetManager.DelEventMarkerLost = MarkerUnawareEvent;
        TargetManager.DelTrackingReadyEvent = AfterBundleLoadEvent;
    }

    void OnDisable()
    {
        TargetManager.DelEventMarkerFound = null;
        TargetManager.DelEventMarkerLost = null;
        TargetManager.DelTrackingReadyEvent = null;
    }

    /*
    void FixedUpdate()
    {
        if (TargetManager.trackableStatus)
        //if (ImageMarkerEvent.마커인식상태)
        {
            if (slideTypeSave)
            {
                ColoringSlideManager.csManager.colorOverlayUI.SetActive(false);
            }

            MainUI.메인.색칠하기UI_닫기();

            if (!sketchFinish && !runningCaptureCoroutine)
            {
                checkPanel.SetActive(true);
                checkPanel.GetComponent<MeshRenderer>().enabled = true;
                //InputSketchInfo();
                RecognizePanel();
                coloringCapture.SetActive(false);
            }
            else
            {
                checkPanel.GetComponent<MeshRenderer>().enabled = false;
            }
        }
        else
        {
            captureDelay = setCaptureDelay;
            checkPanel.SetActive(false);

            if (sketchFinish)
            {
                if (buttonTypeSave)
                {
                    UpdateSavedCount();

                    if (TargetManager.타깃메니저.UsedMiniMap == false)
                    {
                        MainUI.메인.색칠하기UI_열기();
                    }
                }
                // 슬라이드에 텍스쳐가 2개 이상 저장되었을때 슬라이드 버튼UI 켬
                else if (slideTypeSave && ColoringSlideManager.csManager.useColoringSlide && ColoringSlideManager.csManager.saveCount >= 2)
                {
                    ColoringSlideManager.csManager.colorOverlayUI.SetActive(true);
                }

                sketchFinish = false;
            }
        }
    }

    */

    private IEnumerator RecognizeColoring()
    {
        yield return new WaitForSeconds(0.5f);

        //TargetManager.EnableTracking = true;
        isCompleteAssetLoad = true;

        while (true)
        {
            yield return new WaitForFixedUpdate();

            if (TargetManager.trackableStatus)
            {
                if (slideTypeSave)
                {
                    ColoringSlideManager.csManager.colorOverlayUI.SetActive(false);
                }

                MainUI.메인.색칠하기UI_닫기();

                if (!sketchFinish && !runningCaptureCoroutine)
                {
                    checkPanel.SetActive(true);
                    checkPanel.GetComponent<MeshRenderer>().enabled = true;
                    InputSketchInfo();
                    RecognizePanel();
                    coloringCapture.SetActive(false);
                }
                else
                {
                    checkPanel.GetComponent<MeshRenderer>().enabled = false;
                }
            }
            else
            {
                captureDelay = setCaptureDelay;
                checkPanel.SetActive(false);

                if (sketchFinish)
                {
                    if (buttonTypeSave)
                    {
                        UpdateSavedCount();

                        if (TargetManager.타깃메니저.UsedMiniMap == false)
                        {
                            MainUI.메인.색칠하기UI_열기();
                        }
                    }
                    // 슬라이드에 텍스쳐가 2개 이상 저장되었을때 슬라이드 버튼UI 켬
                    else if (slideTypeSave && ColoringSlideManager.csManager.useColoringSlide && ColoringSlideManager.csManager.saveCount >= 2)
                    {
                        ColoringSlideManager.csManager.colorOverlayUI.SetActive(true);
                    }

                    sketchFinish = false;
                }
            }
        }
    }

    private IEnumerator SetAssetBundleContents()
    {
        while (TargetManager.타깃메니저 == null)
        {
            yield return new WaitForSeconds(0.1f);
        }

        HttpRequestDataSet allDataSet = null;

        allDataSet = new HttpRequestDataSet();

        GlobalDataManager.m_AssetBundlePartName = partName;

        GlobalDataManager.m_SelectedAssetBundleName = string.Format("{0}_{1}", GlobalDataManager.m_ResourceFolderEnum.ToString().ToLower(),
                                                                               GlobalDataManager.m_AssetBundlePartName.ToLower());

        Debug.Log("AssetBundleName : " + GlobalDataManager.m_SelectedAssetBundleName);

        TargetManager.타깃메니저.에셋번들복제컨텐츠 = new GameObject[TargetManager.타깃메니저.컨텐츠모델링이름.Length];

        AssetBundleLoader.getInstance.SetAssetBundleLoadInfo(allDataSet,
                                                           GlobalDataManager.m_SelectedAssetBundleName,
                                                           null,
                                                           AssetBundleLoader.getInstance.OnLoadCompleteModeling,
                                                           AfterBundleLoadComplete,
                                                           null,
                                                           null);


        AssetBundleLoader.getInstance.SetStorageLoadObject(allDataSet,
                                                          TargetManager.타깃메니저.컨텐츠모델링이름,
                                                          TargetManager.타깃메니저.에셋번들복제컨텐츠,
                                                          TargetManager.타깃메니저.모델링오브젝트,
                                                          TargetManager.타깃메니저.AR카메라);


        AssetBundleLoader.getInstance.StartLoadAssetBundle(allDataSet);
    }

    private void AfterBundleLoadComplete(HttpRequestDataSet dataSet)
    {
        TargetManager.타깃메니저.StartVuforia();
    }

    private void AfterBundleLoadEvent()
    {
        //Debug.Log("AfterBundleLoadEvent");

        InitSketchValues();
        SetSketchOriginalTextures();

        SetSceneUI();
        StartCoroutine(RecognizeColoring());
    }

    private void SetSceneUI()
    {
        StartCoroutine(MainUI.메인.CloseLoadingPopUp());
        RotateUI.회전.회전UI_숨기기();
        MainUI.메인.인식글자UI.SetActive(true);
    }

    #region 초기화 함수들

    /// <summary>
    /// 스케치 값 초기화
    /// </summary>
    private void InitSketchValues()
    {
        targetEdge = new Transform[4];
        screenEdge = new Vector3[4];

        arCamParent = arCam.transform.parent;

        checkPanel.SetActive(false);

        sketchFinish = false;
        setTextureWidth = 512;
        setTextureHeight = 512;
        currentTargetIndex = -1;

        // 가로, 세로 퍼센트만큼 잘라서 지정
        extWidth = setTextureWidth * sketchWidthPercent / 100;
        extHeight = setTextureWidth * sketchHeightPercent / 100;

        screenWidth = Screen.width;
        screenHeight = Screen.height;

        reverseSketchMode = false;

        colorCam = arCam.GetComponent<Camera>();
    }

    /// <summary>
    /// 색칠영역 퍼센트를 지정해줍니다.
    /// </summary>
    private void SetPercent()
    {
        if (nocutTargetNum.Length > 0)
        {
            for (int i = 0; i < nocutTargetNum.Length; i++)
            {
                // 색칠타겟 밑에 글자가 없으면 전체를 읽음
                if (savedTargetIndex == nocutTargetNum[i])
                {
                    extWidth = setTextureWidth;
                    extHeight = setTextureHeight;

                    break;
                }
                else
                {
                    // 가로, 세로 퍼센트만큼 잘라서 지정
                    extWidth = setTextureWidth * sketchWidthPercent / 100;
                    extHeight = setTextureWidth * sketchHeightPercent / 100;
                }
            }
        }
    }

    /// <summary>
    /// 스케치 원본 텍스쳐를 셋팅합니다.
    /// </summary>
    public void SetSketchOriginalTextures()
    {
        originalTex = new Dictionary<int, Texture2D>();

        int contentNum;
        GameObject copyObj;

        for (int i = 0; i < TargetManager.타깃메니저.타깃정보.Length; i++)
        {
            contentNum = TargetManager.타깃메니저.타깃정보[i].증강될컨텐츠번호[0];
            copyObj = TargetManager.타깃메니저.에셋번들복제컨텐츠[contentNum];

            if (originalTex.ContainsKey(contentNum) == false)
            {
                originalTex.Add(contentNum, copyObj.GetComponent<ColoringInfo>().색칠하기속성.색칠텍스쳐);
            }
        }
    }

    /// <summary>
    /// 미니맵 기능을 사용하는지 체크합니다.
    /// </summary>
    private void CheckMinimapUsage()
    {
        if (TargetManager.타깃메니저.UsedMiniMap)
        {
            slideTypeSave = false;
            buttonTypeSave = false;
        }
    }

    #endregion


    #region 캡쳐, 색칠 관련 함수들

    public void 캡처버튼_클릭()
    {
        StartCoroutine(ScreenCapture());
    }

    /// <summary>
    /// 카메라 이미지를 캡처 합니다.
    /// </summary>
    private IEnumerator ScreenCapture()
    {
        //yield return new WaitForEndOfFrame();

        MainUI.메인.SetActiveRecognitionUI(false);
        checkPanel.GetComponent<MeshRenderer>().material = bluePanelMat;
        checkPanel.SetActive(false); // 페널 닫기
        modelObj.gameObject.SetActive(false);
        MainUI.메인.OnOffRecMessage(false);

        captureImage = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

        arCam.GetComponent<Camera>().Render();

        RenderTexture.active = arCam.GetComponent<Camera>().targetTexture;

        yield return StartCoroutine(ReadScreenPixel());

        RenderTexture.active = null;
        checkPanel.SetActive(true); // 페널 열기
        MainUI.메인.OnOffRecMessage(true);

        //StartCoroutine(이미지변환());
        StartCoroutine(ChangeColorControl());
    }

    private IEnumerator ReadScreenPixel()
    {
        yield return new WaitForEndOfFrame();

        captureImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        captureImage.Apply();
    }

    /// <summary>
    /// 이미지변환 제어용 코루틴
    /// </summary>
    public IEnumerator ChangeColorControl()
    {
        yield return StartCoroutine(ConvertImage());

        ShowSketchModel();

        captureDelay = setCaptureDelay;
        sketchFinish = true;
        runningCaptureCoroutine = false;

        if (TargetManager.타깃메니저.UsedMiniMap == false)
        {
            TargetManager.타깃메니저.슬라이드모델링저장();
        }

        // 미니맵을 다시 켜줌
        if (TargetManager.타깃메니저.UsedMiniMap)
        {
            MiniMapManager.instance.OnOffMiniMap(true);
        }

        if (TargetManager.타깃메니저.usedSelfiMode)
        {
            LetterManager.Instance.textBox.GetComponent<UILabel>().text = LocalizeText.Value["letter_textbox"];
            LetterManager.Instance.StopSetLetterPaper();
            LetterManager.Instance.ApplyLetterMaterial(convertedImage);
            LetterManager.Instance.ResetLetterManager();
        }

        if (sketchFinish && !runningCaptureCoroutine)
        {
            checkPanel.SetActive(true); // 페널 열기
        }

        if (OnRecognitionData != null)
        {
            Texture2D saveTex = new Texture2D(setTextureWidth, setTextureHeight, TextureFormat.RGB24, false);
            Color32[] pix = convertedImage.GetPixels32();
            saveTex.SetPixels32(pix);
            saveTex.Apply();

            OnRecognitionData(modelObj, saveTex);
        }
    }

    /// <summary>
    /// 이미지를 변환하여 바꾸어 줍니다.
    /// </summary>
    private IEnumerator ConvertImage()
    {
        //SetPercent();

        // pixelSave 초기화
        if (reverseSketchMode)
        {
            reverseSketchMode = CheckReverseSketchMode();
            pixelSave.transform.position = targetEdge[1].transform.position;
        }
        else
        {
            pixelSave.transform.position = targetEdge[0].transform.position;
        }

        // 2D 스크린 좌표 저장    
        Vector3 screenPos;
        Color pix;

        // 가로 이동거리
        float widthDistance = Mathf.Abs(targetEdge[0].transform.position.x * 2) / setTextureWidth;

        // 세로 이동거리
        float heightDistance = Mathf.Abs(targetEdge[0].transform.position.y * 2) / setTextureHeight;

        // 초기화 기준이 되는 좌표
        float startX;

        if (reverseSketchMode)
        {
            startX = targetEdge[1].transform.position.x;
        }
        else
        {
            startX = targetEdge[0].transform.position.x;
        }

        float startY = pixelSave.position.y;

        convertedImage = new Texture2D(setTextureWidth, setTextureHeight, TextureFormat.RGB24, false);

        //Camera colorCam = AR카메라.GetComponent<Camera>();
        Vector3 initPixelSave = new Vector3(startX, startY, 0);

        // 변환이미지 높이만큼 
        for (int y = 0; y < extHeight; y++)
        {
            // 변환이미지 너비만큼
            for (int x = 0; x < extWidth; x++)
            {
                // 스크린좌표로 변환
                screenPos = colorCam.WorldToScreenPoint(pixelSave.position);

                // pix 읽어서 텍스쳐에 지정
                pix = captureImage.GetPixel(Mathf.FloorToInt(screenPos.x), Mathf.FloorToInt(screenPos.y));
                convertedImage.SetPixel(x, (setTextureHeight - y) - 1, pix);

                // 오른쪽으로 이동
                if (reverseSketchMode)
                {
                    pixelSave.Translate(-widthDistance, 0, 0);
                }
                else
                {
                    pixelSave.Translate(widthDistance, 0, 0);
                }
            }

            // X, Y 좌표 초기화
            initPixelSave.y = startY;
            pixelSave.transform.position = initPixelSave;

            // 아래로 이동
            startY -= heightDistance;
            pixelSave.Translate(0, 0, -heightDistance);
        }

        convertedImage.Apply();

        yield return new WaitForEndOfFrame();

        coloringMat.mainTexture = convertedImage;

        if (buttonTypeSave)
        {
            SaveSketchTexture(convertedImage);
        }
        else if (ColoringSlideManager.csManager.useColoringSlide)
        {
            ColoringSlideManager.csManager.TextureSave(savedTargetIndex, convertedImage);
        }

        Destroy(captureImage);

        ////// 테스트용 이미지 생성
        ////byte[] bytes = convertedImage.EncodeToPNG();
        ////File.WriteAllBytes("TestImage.png", bytes);
    }

    /// <summary>
    /// 패널 인식
    /// </summary>
    private void RecognizePanel()
    {
        // 각도체크용
        camAngleObj.transform.rotation = Quaternion.Euler(0, 0, arCamParent.transform.localEulerAngles.z);
        camAngle = Quaternion.Angle(camAngleObj.transform.rotation, arCamParent.transform.rotation);

        // 2D 좌표계료 변경
        screenEdge[0] = arCam.GetComponent<Camera>().WorldToScreenPoint(targetEdge[0].position);
        screenEdge[1] = arCam.GetComponent<Camera>().WorldToScreenPoint(targetEdge[1].position);
        screenEdge[2] = arCam.GetComponent<Camera>().WorldToScreenPoint(targetEdge[2].position);
        screenEdge[3] = arCam.GetComponent<Camera>().WorldToScreenPoint(targetEdge[3].position);

        // 사각형 모서리 인식
        if ((screenEdge[0].x > 0 && screenEdge[0].x < screenWidth && screenEdge[0].y > 0 && screenEdge[0].y < screenHeight) &&
            (screenEdge[1].x > 0 && screenEdge[1].x < screenWidth && screenEdge[1].y > 0 && screenEdge[1].y < screenHeight) &&
            (screenEdge[2].x > 0 && screenEdge[2].x < screenWidth && screenEdge[2].y > 0 && screenEdge[2].y < screenHeight) &&
            (screenEdge[3].x > 0 && screenEdge[3].x < screenWidth && screenEdge[3].y > 0 && screenEdge[3].y < screenHeight))
        {
            // 사각형 영역 각도 확인
            if (camAngle < 45 && camAngle > 0)
            {
                //// 인식 성공
                //MainUI.메인.SetActiveRecognitionUI(false);
                //checkPanel.GetComponent<MeshRenderer>().material = bluePanelMat;

                if (!sketchFinish)
                {
                    captureDelay -= Time.fixedDeltaTime;

                    checkPanel.GetComponent<MeshRenderer>().material = bluePanelMat;

                    //// 미니맵을 꺼줌
                    //if (TargetManager.타깃메니저.UsedMiniMap)
                    //{
                    //    MiniMapManager.instance.OnOffMiniMap(false);
                    //}

                    if (captureDelay <= 0 && !runningCaptureCoroutine)
                    {
                        runningCaptureCoroutine = true;

                        checkPanel.SetActive(false); // 페널 닫기

                        if (modelObj.activeSelf)
                        {
                            modelObj.gameObject.SetActive(false);
                        }

                        if (TargetManager.타깃메니저.UsedMiniMap)
                        {
                            MiniMapManager.instance.OnOffMiniMap(false);
                        }

                        StartCoroutine(ScreenCapture());
                    }
                }
            }
            else
            {
                HideSketchModel();
                captureDelay = setCaptureDelay;
                checkPanel.GetComponent<MeshRenderer>().material = redPanelMat;
            }
        }
        else
        {
            // 모서리 인식 실패
            HideSketchModel();
            captureDelay = setCaptureDelay;
            checkPanel.GetComponent<MeshRenderer>().material = redPanelMat;
            checkPanel.SetActive(true);
        }
    }

    private void InputSketchInfo()
    {
        // 단말기 방향체크
        deviceWidth = Screen.width;
        if (screenWidth != deviceWidth)
        {
            screenWidth = Screen.width;
            screenHeight = Screen.height;
        }

        // 색칠저장 체크
        if (buttonTypeSave && currentTargetIndex != savedTargetIndex)
        {
            UpdateSavedCount();
            currentTargetIndex = savedTargetIndex;
        }

        // 모델링 불러오기
        savedTargetIndex = TargetManager.타깃메니저.타깃정보[TargetManager.타깃메니저.타깃정보인덱스].증강될컨텐츠번호[0];
        copyModelobj = TargetManager.타깃메니저.GetCurrentCopyModel();
        modelObj = copyModelobj.GetComponent<ModelInfo>().애니메이션타겟;
        coloringMat = copyModelobj.GetComponent<ColoringInfo>().색칠하기속성.색칠머테리얼;
        TouchEventManager.터치.SetMarkerCollider(copyModelobj.GetComponent<ColoringInfo>().색칠하기속성.콜라이더);

        // 현재타겟의 컬러링캡쳐 게임오브젝트 활성화
        currentTarget = copyModelobj.transform.parent.parent.parent;

        coloringCapture = currentTarget.FindChild("Coloring Capture").gameObject;
        coloringCapture.SetActive(true);

        // 컬러링매니저에 수치 입력
        targetEdge[0] = coloringCapture.transform.FindChild("왼쪽 위");
        targetEdge[1] = coloringCapture.transform.FindChild("오른쪽 위");
        targetEdge[2] = coloringCapture.transform.FindChild("오른쪽 아래");
        targetEdge[3] = coloringCapture.transform.FindChild("왼쪽 아래");
        pixelSave = coloringCapture.transform.FindChild("픽셀저장");
        checkPanel.transform.localScale = new Vector3((targetEdge[1].localPosition.x) * 2 * 100.0f,
                                                    (targetEdge[0].localPosition.z) * 2 * 100.0f, 100.0f);

        //Debug.Log("Input Sketch Info");
    }

    /// <summary>
    /// Reverse 스케치 모드 적용할건지 체크합니다.
    /// </summary>
    private bool CheckReverseSketchMode()
    {
        // 현재타겟 이름 소문자로 변환
        string targetName = currentTarget.name.ToLower();

        // 타겟이름에 reverse가 포함되어 있을시 true
        if (targetName.Contains("reverse"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 캡쳐 상태를 가져옵니다. (캡쳐완료 : true)
    /// </summary>
    public bool GetCaptureStatus()
    {
        // 스케치 완료되고 캡쳐 코루틴이 멈추었으면
        if (sketchFinish && !runningCaptureCoroutine)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion


    #region 색칠 저장 함수들

    /// <summary>
    /// 색칠한 그림을 저장합니다.
    /// </summary>
    private void SaveSketchTexture(Texture2D convertedImage)
    {
        // 그림 키값 설정
        if (savedTexKey.ContainsKey(savedTargetIndex) == true)
        {
            // 순차대로 키값 저장
            if (savedTexKey[savedTargetIndex] < sketchSaveBtn.Length - 1)
            {
                savedTexKey[savedTargetIndex] += 1;
            }
        }
        else
        {
            // 초기 그림을 저장
            savedTexKey.Add(savedTargetIndex, 0);
        }

        // 키값 검사 변수
        int containsKey = savedTargetIndex * 10 + savedTexKey[savedTargetIndex];

        // 해당 키값에 그림이 저장되어 있지 않다면
        if (savedTexDic.ContainsKey(containsKey) == false)
        {
            // 이미지 새롭게 저장
            savedTexDic.Add(containsKey, convertedImage);
        }
        else
        {
            // 맨 앞 이미지 제거(메모리관리)
            Destroy(savedTexDic[containsKey - (sketchSaveBtn.Length - 1)]);

            // 기존 이미지에 덮어씌우기
            for (int i = 0; i < sketchSaveBtn.Length - 1; i++)
            {
                savedTexDic[containsKey - (sketchSaveBtn.Length - 1) + i] = savedTexDic[containsKey - (sketchSaveBtn.Length - 1) + i + 1];
            }

            savedTexDic[containsKey] = convertedImage;
        }
    }

    private void UpdateSavedCount()
    {
        int savedCount = 0;       // 색칠개수 초기화

        for (int i = 0; i < sketchSaveBtn.Length; i++)
        {
            // 해당인덱스 savedTexDic에 그림이 몇개 있는지 검색
            if (savedTexDic.ContainsKey(savedTargetIndex * 10 + i) == true)
            {
                savedCount++;
            }
            else
            {
                break;
            }
        }
        ShowSavedSketchUI(savedCount);
    }

    public void UpdateSavedCount(int contentNum)
    {
        int savedCount = 0;       // 색칠개수 초기화

        for (int i = 0; i < sketchSaveBtn.Length; i++)
        {
            // 해당인덱스 savedTexDic에 그림이 몇개 있는지 검색
            if (savedTexDic.ContainsKey(contentNum * 10 + i) == true)
            {
                savedCount++;
            }
            else
            {
                break;
            }
        }
        ShowSavedSketchUI(savedCount);
    }

    /// <summary>
    /// 색칠저장 한 개수만큼 UI을 보여줍니다.
    /// </summary>
    private void ShowSavedSketchUI(int count)
    {
        MainUI.메인.색칠하기UI_닫기();

        // 버튼 모두 숨기기
        for (int i = 0; i < sketchSaveBtn.Length; i++)
        {
            sketchSaveBtn[i].SetActive(false);
        }

        // 색칠저장버튼 제어
        if (count > 0)
        {
            if (TargetManager.타깃메니저.UsedMiniMap == false)
            {
                MainUI.메인.색칠하기UI_열기();
            }

            // 색칠그림 개수만큼 버튼 열기
            for (int i = 0; i < sketchSaveBtn.Length; i++)
            {
                if (i < count)
                {
                    sketchSaveBtn[i].SetActive(true);
                }
            }
        }
    }

    #endregion


    #region 색칠 UI 관련 함수들

    public void 원본이미지_변환()
    {
        MainUI.메인.비활성화_함수호출();

        coloringMat.mainTexture = originalTex[savedTargetIndex];
    }


    /// <summary>
    /// 슬라이드 방향버튼을 누를때마다 모델링을 새로고침 합니다.
    /// </summary>
    public void 슬라이드새로고침(int 슬라이드인덱스)
    {
        savedTargetIndex = 슬라이드인덱스;

        // 모델링 새로고침
        GameObject obj = TargetManager.타깃메니저.에셋번들복제컨텐츠[슬라이드인덱스];
        modelObj = obj.GetComponent<ModelInfo>().애니메이션타겟;

        coloringMat = obj.GetComponent<ColoringInfo>().색칠하기속성.색칠머테리얼;
        TouchEventManager.터치.기준콜라이더 = TargetManager.타깃메니저.에셋번들복제컨텐츠[슬라이드인덱스].GetComponent<ColoringInfo>().색칠하기속성.콜라이더;

        if (!modelObj.activeSelf)
        {
            modelObj.SetActive(true);
        }

        UpdateSavedCount();
    }

    /// <summary>
    /// 색칠저장 숫자버튼 클릭시 이벤트
    /// </summary>
    public void 색칠1번그림()
    {
        MainUI.메인.비활성화_함수호출();

        coloringMat.mainTexture = savedTexDic[savedTargetIndex * 10];
    }
    public void 색칠2번그림()
    {
        MainUI.메인.비활성화_함수호출();

        coloringMat.mainTexture = savedTexDic[savedTargetIndex * 10 + 1];
    }
    public void 색칠3번그림()
    {
        MainUI.메인.비활성화_함수호출();

        coloringMat.mainTexture = savedTexDic[savedTargetIndex * 10 + 2];
    }
    public void 색칠4번그림()
    {
        MainUI.메인.비활성화_함수호출();

        coloringMat.mainTexture = savedTexDic[savedTargetIndex * 10 + 3];
    }

    /// <summary>
    /// 스케치 모델 오브젝트를 숨깁니다.
    /// </summary>
    private void HideSketchModel()
    {
        RotateUI.회전.회전UI_숨기기();
        MainUI.메인.애니메이션동작_UI숨기기();
        MainUI.메인.ChangeRecognitionText(LocalizeText.Value["SketchRecognition_OnTarget"], false);
        MainUI.메인.색칠하기UI_닫기();
        MainUI.메인.SetActiveRecognitionUI(true);

        modelObj.gameObject.SetActive(false);

        if (TargetManager.타깃메니저.UsedMiniMap)
        {
            MiniMapManager.instance.miniMapCollectingBtn.SetActive(false);
        }

        if(TargetManager.타깃메니저.usedSelfiMode)
        {
            LetterManager.Instance.textBox.SetActive(false);
            LetterManager.Instance.faceBox.SetActive(false);
        }
    }

    /// <summary>
    /// 스케치 모델 오브젝트를 보입니다.
    /// </summary>
    public void ShowSketchModel()
    {
        if (TargetManager.trackableStatus)
        {
            MainUI.메인.SetActiveRecognitionUI(false);
            MainUI.메인.ChangeRecognitionText(LocalizeText.Value["SketchRecognition_Default"], true);

            if (!TargetManager.타깃메니저.UsedMiniMap)
            {
                RotateUI.회전.회전UI_보이기();
            }
            else
            {
                MiniMapManager.instance.miniMapCollectingBtn.SetActive(false);
                RotateUI.회전.회전UI_숨기기();
            }
        }
        else
        {
            RotateUI.회전.회전UI_보이기();
            MainUI.메인.애니메이션동작_UI보이기();
            MainUI.메인.OnOffOverlayUI(true);

            if (TargetManager.타깃메니저.UsedMiniMap)
            {
                MiniMapManager.instance.miniMapUI.SetActive(true);
                MiniMapManager.instance.miniMapCollectingBtn.SetActive(true);
            }
        }

        RotateUI.회전.컨텐츠_회전_초기화();
        TargetManager.타깃메니저.ShowModelingObj();
        modelObj.gameObject.SetActive(true);
        AnimationManager.애니메이션.애니메이션01_재생();

        //if (TargetManager.타깃메니저.UsedMiniMap)
        //{
        //    MiniMapManager.instance.miniMapUI.SetActive(true);
        //    MiniMapManager.instance.miniMapCollectingBtn.SetActive(true);

        //    MainUI.메인.애니메이션동작_UI보이기();
        //}
    }

    /// <summary>
    /// 스케치UI를 끕니다.
    /// </summary>
    public void TurnOffSketchUI()
    {
        MainUI.메인.SetActiveRecognitionUI(false);
        MainUI.메인.ChangeRecognitionText(LocalizeText.Value["SketchRecognition_Default"], true);
        RotateUI.회전.회전UI_숨기기();
        MainUI.메인.애니메이션동작_UI숨기기();
        MainUI.메인.색칠하기UI_닫기();
        MainUI.메인.오버레이UI.SetActive(false);

        if (TargetManager.타깃메니저.UsedMiniMap)
        {
            MiniMapManager.instance.miniMapCollectingBtn.SetActive(false);
        }
    }

    /// <summary>
    /// 마커 비인식 스케치 이벤트
    /// </summary>
    public void MarkerUnawareEvent(int targetNum)
    {
        if (isCompleteAssetLoad)
        {
            MainUI.메인.ChangeRecognitionText(LocalizeText.Value["SketchRecognition_Default"], true);

            if (sketchFinish)
            {
                MainUI.메인.애니메이션동작_UI보이기();

                if (TargetManager.타깃메니저.UsedMiniMap == false)
                {
                    MainUI.메인.색칠하기UI_열기();
                }
                MainUI.메인.OnOffOverlayUI(true);

                AnimationManager.애니메이션.애니메이션01_재생();
            }
            else
            {
                MainUI.메인.애니메이션동작_UI숨기기();
                MainUI.메인.색칠하기UI_닫기();
                MainUI.메인.OnOffOverlayUI(false);

                if (TargetManager.타깃메니저.UsedMiniMap)
                {
                    MiniMapManager.instance.miniMapCollectingBtn.SetActive(false);
                }
            }
        }
    }

    /// <summary>
    /// 마커 인식 이벤트
    /// </summary>
    public void SetMarketRecognitionEvent(int targetNum)
    {
        if (isCompleteAssetLoad)
        {
            TurnOffSketchUI();
            savedTargetIndex = GetCurrentTargetNum(targetNum);

            copyModelobj = TargetManager.타깃메니저.GetCurrentCopyModel();
            TouchEventManager.터치.SetMarkerCollider(copyModelobj.GetComponent<ColoringInfo>().색칠하기속성.콜라이더);
            InputSketchInfo();
        }
    }

    private int GetCurrentTargetNum(int targetNum)
    {
        return TargetManager.타깃메니저.타깃정보[targetNum].증강될컨텐츠번호[0];
    }

    #endregion
}