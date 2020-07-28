using UnityEngine;

using System;
using System.Collections;
using UnityEngine.SceneManagement;

public class PuzzleManager : Singleton<PuzzleManager> {
    [Serializable]
    public class RectPositionInfo {
        public int left;
        public int right;
        public int bottom;
        public int top;

        public float leftOffset;
        public float rightOffset;
        public float bottomOffset;
        public float topOffset;
    }

    [Serializable]
    public class PuzzlePositionInfo
    {
        public RectPositionInfo defaultPosInfo;
        public RectPositionInfo finalPosInfo;

        public Vector3 defaultScale;
        public Vector3 finalScale;
    }

    [HideInInspector]
    public int              m_CurrentPuzzleIndex;
    [HideInInspector]
    public bool             m_RecognitionStatus { get; set; }
    [HideInInspector]
    public int              m_RecognitionCount { get; set; }

    public GlobalDataManager.PuzzleProductType m_PuzzleType;
    public AudioSource      m_PuzzleAudioSource;

    public AudioClip        m_SuccessSoundPiece;
    public AudioClip        m_SuccessSoundPuzzle;

    public string           m_NonColorKeyword;

    public int              m_FindEffectCount;
    public int              m_ConfirmEffectCount;

    public float            m_correctEffectTime;
    public float            m_changePieceIntervalTime;

    public float            m_IncreaseZoomValue;
    public float            m_LimitZoomMaxValue;    

    public PuzzlePositionInfo[] m_PuzzlePositionInfo;


    private bool            m_GlimmerFinish;        // 퍼즐 조각 깜박이는 효과 종료 상태 (true : 동작하지 않음, false : 동작 중)

    private string          m_foundTargetName { get; set; }
    private string          m_lostTargetName { get; set; }
    
    private Coroutine       m_ApplyGlimmerEffect;

    private bool            m_isRunPuzzleMatching = false;   

    void Awake()
    {
        m_CurrentPuzzleIndex        = -1;       
    }

	// Use this for initialization
	void Start () {
        m_GlimmerFinish     = true;

        m_RecognitionStatus = false;
        m_RecognitionCount  = 0;

        m_foundTargetName   = string.Empty;
        m_lostTargetName    = string.Empty;

        HideAllPuzzle();

        PuzzleUI.getInstance.m_PuzzleRetryBtnUI.SetActive(false);
        PuzzleUI.getInstance.m_PuzzleShow3dBtnUI.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        

    }

    private IEnumerator GlimmerEffectPuzzlePiece(GameObject pieceObj, int effectCount, float intervalTime, bool isFound)
    {
        int localEffectCount    = 1;
        float localCurrentTime  = 0f;        

        string spriteName       = string.Empty;
        string finalSpriteName  = string.Empty;

        bool gradientValue      = false;

        float alphaValue        = 0f;

        while (m_GlimmerFinish == false)
        {
            if (localEffectCount > effectCount)
            {
                m_GlimmerFinish = true;                
                pieceObj.GetComponent<UISprite>().alpha = 1f;
            }
            else
            {
                if (localCurrentTime < intervalTime)
                {
                    localCurrentTime = localCurrentTime + Time.deltaTime;
                }
                else
                {
                    if (pieceObj.GetComponent<UISprite>().alpha < 1f)
                    {
                        localEffectCount    = localEffectCount + 1;
                        alphaValue          = 1f;
                        gradientValue       = false;
                    }
                    else
                    {
                        alphaValue          = 0.5f;
                        gradientValue       = true;
                    }

                    localCurrentTime                                = 0f;
                    
                    pieceObj.GetComponent<UISprite>().alpha         = alphaValue;
                    pieceObj.GetComponent<UISprite>().applyGradient = gradientValue;                    
                }               
            }

            yield return Time.deltaTime;
        }

        if (isFound)
        {
            m_isRunPuzzleMatching = true;
            FindPuzzlePiece();
        }        
    }

    private void HideAllPuzzle()
    {
        foreach (PuzzleUI.PuzzleUseDataSet dataSet in PuzzleUI.getInstance.m_PuzzleDataSet)
        {
            dataSet.puzzleRoot.SetActive(false);
        }
    }

    private void ShowHidePuzzle(int index, bool isShow)
    {
        PuzzleUI.getInstance.m_PuzzleDataSet[index].puzzleRoot.SetActive(isShow);
    }
        
    private void ApplyGlimmerEffect(int index, int pieceIndex, int effectCount, bool isFound)
    {
        m_GlimmerFinish = false;
        m_ApplyGlimmerEffect = StartCoroutine(GlimmerEffectPuzzlePiece(PuzzleUI.getInstance.m_PuzzleDataSet[index].puzzleChildPieces[pieceIndex], effectCount, m_changePieceIntervalTime, isFound));        
    }
    
    private int FindPuzzlePiece()
    {
        int index = -1;
        int pieceIndex = -1;

        GameObject backgroundObj = null;
        TweenPosition tweenPosition = null;

        index = (int)m_PuzzleType;
        PuzzleMatching.getInstance.RunQuizEvent();        

        if (PuzzleMatching.getInstance.CheckEndOfQuiz()) 
        {
            PuzzleSoundPlay(m_SuccessSoundPuzzle);

            PuzzleUI.getInstance.m_PuzzleDataSet[index].puzzleRoot.transform.localScale = m_PuzzlePositionInfo[index].finalScale;

            backgroundObj = PuzzleUI.getInstance.m_PuzzleDataSet[index].puzzleBackground;
           
            tweenPosition = backgroundObj.GetComponent<TweenPosition>();
            tweenPosition.from = backgroundObj.transform.localPosition;            
            tweenPosition.enabled = true;
            tweenPosition.ResetToBeginning();
            tweenPosition.PlayForward();

            PuzzleUI.getInstance.m_PuzzleRetryBtnUI.SetActive(true);
            PuzzleUI.getInstance.m_PuzzleShow3dBtnUI.SetActive(true);

            return -2;
        }

        if (m_ApplyGlimmerEffect != null)
        {
            StopCoroutine(m_ApplyGlimmerEffect);
        }
        
        pieceIndex = PuzzleMatching.getInstance.GetCurrentPieceNumber();
        ApplyGlimmerEffect(index, pieceIndex, m_FindEffectCount, false);

        return index;
    }

    private void InitPuzzleGameUI()
    {
        UISprite spriteUI = null;
        UIButton buttonUI = null;
        int index = -1;
        string spriteName = string.Empty;

        index = (int)m_PuzzleType;

        foreach (GameObject obj in PuzzleUI.getInstance.m_PuzzleDataSet[index].puzzleChildPieces)
        {
            spriteUI = obj.GetComponent<UISprite>();
            buttonUI = obj.GetComponent<UIButton>();

            spriteName = string.Format("{0}{1}", m_NonColorKeyword, spriteUI.spriteName);
            buttonUI.normalSprite = spriteName;
        }
    }

    private void HidePuzzleStartBtn(int index)
    {
        PuzzleUI.getInstance.m_PuzzleDataSet[index].puzzleRoot.SetActive(true);

        PuzzleUI.getInstance.m_PuzzleNormalStartBtnUI.SetActive(false);
        PuzzleUI.getInstance.m_PuzzleRandomStartBtnUI.SetActive(false);
        PuzzleUI.getInstance.m_MoveMultiTargetBtnUI.SetActive(false);
    }

    private void SetPuzzleTouchZoom(GameObject backgroundObj)
    {
        // 컨텐츠 오브젝트에 bus object clone 을 붙인다.
        // 원본 bus object는 숨김        
        try
        {
            TouchEventManager.이동좌표계 = true;
            TouchEventManager.터치.컨텐츠오브젝트 = backgroundObj.transform.parent.gameObject;
            TouchEventManager.터치.기준콜라이더 = TouchEventManager.터치.지형2D;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            throw;
        }
    }

    public void PuzzlePieceRecognition(int pieceIndex)
    {
        int index = -1;
        string spriteName = string.Empty;
        UISprite spriteUI = null;
        UIButton buttonUI = null;
        
        index = (int)m_PuzzleType;

        if (m_ApplyGlimmerEffect != null)
        {
            StopCoroutine(m_ApplyGlimmerEffect);
        }             

        spriteUI = PuzzleUI.getInstance.m_PuzzleDataSet[index].puzzleChildPieces[pieceIndex].GetComponent<UISprite>();
        buttonUI = PuzzleUI.getInstance.m_PuzzleDataSet[index].puzzleChildPieces[pieceIndex].GetComponent<UIButton>();

        spriteUI.spriteName = spriteUI.spriteName.Replace(m_NonColorKeyword, string.Empty);
        buttonUI.normalSprite = spriteUI.spriteName;
        
        ApplyGlimmerEffect(index, pieceIndex, m_ConfirmEffectCount, true);
        PuzzleSoundPlay(m_SuccessSoundPiece);
    }

    public void ShowPuzzle(int index)
    {
        HideAllPuzzle();
        ShowHidePuzzle(index, true);
    }

    public void HidePuzzle(int index)
    {
        ShowHidePuzzle(index, false);
    }
    
    public void TrackingFoundPuzzlePiece(string targetName)
    {
        int targetIndex = -1;
        bool matchingResult = false;

        if (m_isRunPuzzleMatching == false)
        {
            return;
        }

        Debug.LogWarning(string.Format("targetName={0} in TrackingFoundPuzzlePiece()", targetName));

        if (Int32.TryParse(targetName, out targetIndex))
        {
            targetIndex = Int32.Parse(targetName);
            matchingResult = PuzzleMatching.getInstance.RunPuzzleMatching(targetIndex);

            if (matchingResult)
            {
                m_isRunPuzzleMatching = false;
                PuzzlePieceRecognition(targetIndex - 1);             
            }
        }
    }

    public void TrackingLostPuzzlePiece(string targetName)
    {
        m_lostTargetName = targetName;
    }

    public void OnClickShow3dModel()
    {
        int dataSetIndex = -1;
        dataSetIndex = (int)m_PuzzleType;

        HideAllPuzzle();        

        TouchEventManager.이동좌표계             = true;
        TouchEventManager.터치.오브젝트터치사용   = true;
        TouchEventManager.터치.컨텐츠오브젝트     = PuzzleUI.getInstance.m_ContentObject;
        TouchEventManager.터치.기준콜라이더       = TouchEventManager.터치.지형2D;
        
        TargetManager.타깃메니저.마커비인식호출(TargetManager.타깃메니저.타깃정보인덱스);

        MainUI.메인.운전하기UI.SetActive(true);
        RotateUI.회전.회전UI_보이기();
        
        PuzzleUI.getInstance.m_PuzzleShow3dBtnUI.SetActive(false);
    }
    
    public void OnClickPuzzleStart()
    {
        int index = -1;

        m_isRunPuzzleMatching = true;
        PuzzleMatching.getInstance.isRandomMode = false;

        index = FindPuzzlePiece();        
        HidePuzzleStartBtn(index);
    }

    public void OnClickPuzzleRandomStart()
    {
        int index = -1;
        
        m_isRunPuzzleMatching = true;
        PuzzleMatching.getInstance.isRandomMode = true;

        index = FindPuzzlePiece();
        HidePuzzleStartBtn(index);
    }

    public void OnClickRetryPuzzle()
    {
        //SceneManager.LoadScene(GlobalDataManager.m_SelectedSceneName);
        
        int index = -1;
        UISprite spriteUI = null;

        InitPuzzleGameUI();

        index = (int)m_PuzzleType;
        m_isRunPuzzleMatching = false;

        PuzzleMatching.instance.InitMatchingData();

        PuzzleUI.getInstance.m_PuzzleDataSet[index].puzzleRoot.SetActive(false);

        PuzzleUI.getInstance.m_PuzzleRetryBtnUI.SetActive(false);
        PuzzleUI.getInstance.m_PuzzleShow3dBtnUI.SetActive(false);        

        PuzzleUI.getInstance.m_PuzzleNormalStartBtnUI.SetActive(true);
        PuzzleUI.getInstance.m_PuzzleRandomStartBtnUI.SetActive(true);
        PuzzleUI.getInstance.m_MoveMultiTargetBtnUI.SetActive(true);

        spriteUI = PuzzleUI.getInstance.m_PuzzleDataSet[index].puzzleBackground.GetComponent<UISprite>();

        spriteUI.SetAnchor(PuzzleUI.getInstance.m_RootUI,
                           m_PuzzlePositionInfo[index].defaultPosInfo.leftOffset,
                           m_PuzzlePositionInfo[index].defaultPosInfo.left,
                           m_PuzzlePositionInfo[index].defaultPosInfo.bottomOffset,
                           m_PuzzlePositionInfo[index].defaultPosInfo.bottom,
                           m_PuzzlePositionInfo[index].defaultPosInfo.rightOffset,
                           m_PuzzlePositionInfo[index].defaultPosInfo.right,
                           m_PuzzlePositionInfo[index].defaultPosInfo.topOffset,
                           m_PuzzlePositionInfo[index].defaultPosInfo.top);

        PuzzleUI.getInstance.m_PuzzleDataSet[index].puzzleRoot.transform.localScale = m_PuzzlePositionInfo[index].defaultScale;

        TouchEventManager.이동좌표계 = false;
        TouchEventManager.터치.오브젝트터치사용 = false;

        TargetManager.타깃메니저.HideAllModelingContents();

        MainUI.메인.운전하기UI.SetActive(false);
        MainUI.메인.애니버튼목록.SetActive(false);

        RotateUI.회전.회전UI_숨기기();       
    }

    public void OnClickMoveMultiTarget()
    {
        
        GlobalDataManager.m_AssetBundlePartName  = string.Format("multi");
        GlobalDataManager.m_SelectedSceneName    = string.Format("{0}_puzzleLanguage", GlobalDataManager.m_SelectedCategoryEnum.ToString());
        
        GlobalDataManager.GlobalLoadScene();
    }

    public void OnClickSoundPlay(GameObject clickObj)
    {
        int pieceIndex = 0;
        AudioClip clip = null;

        if (Int32.TryParse(clickObj.GetComponent<UISprite>().spriteName, out pieceIndex) == false)
        {
            return;
        }

        clip = PuzzleContentInfo.getInstance.m_AudioClipNumbers.audioClipKorea[pieceIndex - 1];

        PuzzleSoundPlay(clip);
    }

    private void PuzzleSoundPlay(AudioClip clip)
    {
        if (m_PuzzleAudioSource == null || clip == null)
        {
            return;
        }

        m_PuzzleAudioSource.clip = clip;

        if (m_PuzzleAudioSource.isPlaying)
        {
            m_PuzzleAudioSource.Stop();
        }

        m_PuzzleAudioSource.Play();
    }

    public void OnClickPuzzleZoomInit()
    {
        int dataSetIndex = -1;
        dataSetIndex = (int)m_PuzzleType;
    }

    public void OnClickPuzzleZoomIn()
    {
        int dataSetIndex = -1;
        dataSetIndex = (int)m_PuzzleType;
    }

    public void OnClickPuzzleZoomOut()
    {
        int dataSetIndex = -1;
        dataSetIndex = (int)m_PuzzleType;
    }

    public void OnFinishedTweenPosition(GameObject backgroundObj)
    {
        int index = -1;
        index = (int)m_PuzzleType;
        
        backgroundObj.GetComponent<UISprite>().SetAnchor(PuzzleUI.getInstance.m_RootUI,
                                                         m_PuzzlePositionInfo[index].finalPosInfo.leftOffset,
                                                         m_PuzzlePositionInfo[index].finalPosInfo.left,
                                                         m_PuzzlePositionInfo[index].finalPosInfo.bottomOffset,
                                                         m_PuzzlePositionInfo[index].finalPosInfo.bottom,
                                                         m_PuzzlePositionInfo[index].finalPosInfo.rightOffset,
                                                         m_PuzzlePositionInfo[index].finalPosInfo.right,
                                                         m_PuzzlePositionInfo[index].finalPosInfo.topOffset,
                                                         m_PuzzlePositionInfo[index].finalPosInfo.top);

        backgroundObj.GetComponent<TweenPosition>().enabled = false;
        SetPuzzleTouchZoom(backgroundObj);

        PuzzleUI.getInstance.m_PuzzleShow3dBtnUI.SetActive(true);
        PuzzleUI.getInstance.m_PuzzleRetryBtnUI.SetActive(true);        

        PuzzleMatching.getInstance.ClearLabelText(string.Empty);
    }    
}

