using UnityEngine;
using System;
using System.Collections.Generic;

public class SketchBookUI : Singleton<SketchBookUI>
{
    /// <summary>
    /// quizquiz ui
    /// </summary>
    public GameObject quizUI;
    public GameObject quizOtherUI;
    public GameObject otherUI;
    public GameObject preNextUI;
    public GameObject pagingRoot;

    /// <summary>
    /// 드래그매니저 obj
    /// </summary>
    public GameObject DragManager;

    public GameObject SuccessPrefab;
    public GameObject FailPrefab;

    [HideInInspector]
    public GameObject ShowPrefab;

    /// <summary>
    /// drag종류 quiz 지문(텍스트)
    /// </summary>
    public UILabel DragQuizText;

    /// <summary>
    /// 타겟없이 사용할 것인지 체크 (타겟 안비춰도 바로 DragUI를 띄워줌)
    /// </summary>
    public bool noUsingTarget = false;
    public GlobalDataManager.SceneState[] sequenceStickerContents;
    public SketchBookTouchEvent sketchBookTouchEventInst;

    /// <summary>
    /// drag quiz ui
    /// </summary>
    [Serializable]
    public class DragUiSetting
    {
        public string QuizText;
        public GameObject SceneMainUI;
        public GameObject MatchinPieceParent;
        public GameObject PuzzlePieceParent;
        public GameObject PieceStartPos;
        public GameObject TweenCtrlPos;
    }

    /// <summary>
    /// drag quiz 종류별로 퀴즈 설정
    /// </summary>
    [Serializable]
    public class MainSetting
    {
        public DragUiSetting[] Question;
    }

    [SerializeField]
    public MainSetting[] SceneModeMainUI;

    private DragUiSetting nowQuestion;
    private StickerManager stickerManager;

    private bool CorrectAnswerPrefabIsPlying = false;
    private int stickerContentsIdx = 0;

    void Start()
    {
        if (noUsingTarget)
        {
            MainUI.메인.UI_상태변경(MainUI.메인.인식글자UI, false);

            // 타겟을 사용하지 않는 환경일 경우 Swipe Callback 이벤트를 추가 합니다.
            if (sketchBookTouchEventInst != null)
            {
                sketchBookTouchEventInst.OnSwipeCompleEvent += ViewSketchBookContents;
            }
            
            StartSketchBookContents(sequenceStickerContents[stickerContentsIdx]);            
            preNextUI.SetActive(true);            
        }
        else
        {
            Ui_Off();
        }        
    }
    
    void FixedUpdate()
    {
        if(CorrectAnswerPrefabIsPlying)
        {
            if(!ShowPrefab.GetComponent<UISpriteAnimation>().isPlaying)
            {
                if(TargetManager.타깃메니저.SceneMode == GlobalDataManager.SceneState.QUIZ_QUIZ)
                {
                    QuizQuizManager.getInstance.QuizBtnClickEventPlay(ShowPrefab.transform.parent.gameObject);
                }

                CorrectAnswerPrefabDestroy();
            }
        }
    }

    void OnDestroy()
    {
        // 타겟을 사용하지 않는 환경일 경우 Swipe Callback 이벤트를 삭제 합니다.
        if (noUsingTarget)
        {
            sketchBookTouchEventInst.OnSwipeCompleEvent -= ViewSketchBookContents;
        }
    }

    void UnsubscribeEvent()
    {
        OnDestroy();
    }

    /// <summary>
    /// SketchBookTouchEvent.cs 에서 Callback 함수로 사용되는 함수
    /// Swipe 완료 시 후 처리 되는 함수
    /// </summary>
    /// <param name="isNextContentView"></param>
    private void ViewSketchBookContents(bool isNextContentView)
    {
        if (isNextContentView)
        {
            // 다음 컨텐츠 요청시 인덱스 값 조정
            stickerContentsIdx = stickerContentsIdx + 1;

            if (stickerContentsIdx >= sequenceStickerContents.Length)
            {
                stickerContentsIdx = sequenceStickerContents.Length - 1;
            }

            ApplyStickerBookContents();
        }
        else
        {
            // 기존 컨텐츠가 CRANE 이었으면 종료 하는 함수를 호출 해 줍니다.
            if (GlobalDataManager.m_SelectedSceneStateEnum == GlobalDataManager.SceneState.CRANE)
            {
                PrincessManager.instance.ClawGameQuit();
            }

            // 이전 컨텐츠 요청시 인덱스 값 조정
            stickerContentsIdx = stickerContentsIdx - 1;

            if (stickerContentsIdx < 0)
            {
                stickerContentsIdx = 0;
            }

            ApplyStickerBookContents();
        }
    }


    private void DragMainUiNullButtonTarget(int sceneModeNum)
    {
        SceneModeMainUI[sceneModeNum].Question[(int)stickerManager.m_QuestionNum - 1].SceneMainUI.GetComponent<UIButton>().tweenTarget = null;
    }

    /// <summary>
    /// drag퀴즈 ui셋팅 
    /// </summary>
    /// <param name="sceneModeNum"></param>
    private void DragUISet(int sceneModeNum)
    {
        try
        {
            if ((int)stickerManager.m_QuestionNum - 1 <= SceneModeMainUI[sceneModeNum].Question.Length - 1)
            {
                nowQuestion = SceneModeMainUI[sceneModeNum].Question[(int)stickerManager.m_QuestionNum - 1];
                stickerManager.DragTextStart(nowQuestion);
                nowQuestion.SceneMainUI.SetActive(true);

                stickerManager.SceneModeMainUI = nowQuestion.SceneMainUI;

                if (noUsingTarget == false)
                {
                    stickerManager.backBtn.SetActive(true);
                    stickerManager.backBtn.transform.parent = stickerManager.SceneModeMainUI.transform;
                }

                stickerManager.matchingPieceParent = nowQuestion.MatchinPieceParent;
                stickerManager.puzzlePieceSettingInfo.puzzlePieceParent = nowQuestion.PuzzlePieceParent;
                stickerManager.pieceStartPos = nowQuestion.PieceStartPos;
                stickerManager.tweenCtrlPos = nowQuestion.TweenCtrlPos;
                stickerManager.nowQuestionSuccessIndex = 0;

                stickerManager.InitDragQuiz();
            }
            else
            {
                Debug.Log("문제가 존재하지 않습니다.");
                return;
            }
        }
        catch
        {

        }
    }

    /// <summary>
    /// 시작 시 (Start 함수) Sticker Book 컨텐츠를 보여주는 함수 (타겟 사용 하지 않는 환경)
    /// </summary>
    /// <param name="type"></param>
    private void StartSketchBookContents(GlobalDataManager.SceneState type)
    {
        DragManager.GetComponent<StickerManager>().m_QuestionNum    = SketchBookTargetInfo.QuizNum.ONE;        
        GlobalDataManager.m_SelectedSceneStateEnum                  = type;
                
        if (TargetManager.타깃메니저.SceneMode == GlobalDataManager.SceneState.QUIZ_QUIZ)
        {
            QuizQuizManager.getInstance.m_QuizState = QuizQuizManager.QuizState.READY;
            QuizQuizManager.getInstance.m_QuizNum   = SketchBookTargetInfo.QuizNum.ONE;

            TargetManager.타깃메니저.QuizQuizMode(true);

            //MainUI.메인.ContentsBtnSetting(QuizQuizUI.Instance.QuizList.Length);
        }                
        
        Ui_On(GlobalDataManager.m_SelectedSceneStateEnum);        
    }    

    /// <summary>
    /// 해당하는 ui on(스케치북 종류 ui)
    /// </summary>
    /// <param name="checkState"></param>
    public void Ui_On(GlobalDataManager.SceneState checkState)
    {
        Debug.Log("sketchbookui 설정에 따른 ui 키는 부분");

        stickerManager = DragManager.GetComponent<StickerManager>();

        Ui_Off();

        //if (MainUI.메인.navigationUI != null)
        //{
        //    Debug.Log("들오나");
        //    MainUI.메인.navigationUI.SetActive(false);
        //}

        switch (checkState)
        {
            case GlobalDataManager.SceneState.DRAG_AND_DROP:
                DragUISet(0);
                break;

            case GlobalDataManager.SceneState.MAP:
                DragUISet(1);
                break;

            case GlobalDataManager.SceneState.PUZZLE:
                DragMainUiNullButtonTarget(2);
                DragUISet(2);
                stickerManager.SketchPuzzleBoxInit(false);
                break;

            case GlobalDataManager.SceneState.QUIZ_QUIZ:
                quizUI.SetActive(true);
                break;

            case GlobalDataManager.SceneState.STICKER:
                DragUISet(0);
                break;

            case GlobalDataManager.SceneState.CRANE:
                PrincessManager.instance.QuestionInit();
                //PrincessManager.instance.princessClawUI.SetActive(true);
                TweenManager.tween_Manager.TweenAlpha(PrincessManager.instance.princessClawUI);
                break;

            default:
                Debug.Log("UI Type None or Error ");
                return;
        }
    }

    /// <summary>
    /// 모든 ui off(스케치북 종류 ui)
    /// </summary>
    public void Ui_Off()
    {
        Debug.Log("sketchbookui ui 끄는 부분");

        quizUI.SetActive(false);
        DragManager.GetComponent<StickerManager>().backBtn.SetActive(false);

        for (int i = 0; i < SceneModeMainUI.Length; i++)
        {
            for (int j = 0; j < SceneModeMainUI[i].Question.Length; j++)
            {
                SceneModeMainUI[i].Question[j].SceneMainUI.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 부모 오브젝트(matching obj), 성공/실패
    /// </summary>
    /// <param name="parentObj"></param>
    /// <param name="awareness"></param>
    public void CorrectAnswerCheck(GameObject parentObj, bool awareness)
    {
        CorrectAnswerPrefabDestroy();

        if (awareness)
        {
            if (SuccessPrefab != null)
            {
                ShowPrefab = Instantiate(SuccessPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            }            
        }
        else
        {
            if (FailPrefab != null)
            {
                ShowPrefab = Instantiate(FailPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            }            
        }

        if (ShowPrefab != null)
        {
            ShowPrefab.transform.parent         = parentObj.transform;
            ShowPrefab.transform.localScale     = new Vector3(1.0f, 1.0f, 1.0f);
            ShowPrefab.transform.localPosition  = new Vector3(0, 0, 0);

            ShowPrefab.GetComponent<UISpriteAnimation>().Play();

            CorrectAnswerPrefabIsPlying         = true;
        }        
    }

    public void CorrectAnswerPrefabDestroy()
    {
        CorrectAnswerPrefabIsPlying = false;

        if (ShowPrefab != null)
        {
            DestroyObject(ShowPrefab);
        }
    }

    /// <summary>
    /// stickerContentsIdx 값에 해당되는 스티커 북 컨텐츠를 화면에 표시    
    /// </summary>
    private void ApplyStickerBookContents()
    {
        SettingQuizNumber(stickerContentsIdx);
        GlobalDataManager.m_SelectedSceneStateEnum = sequenceStickerContents[stickerContentsIdx];

        Ui_On(GlobalDataManager.m_SelectedSceneStateEnum);

        // 하단의 현재 페이지를 나타내는 서클 모두의 크기를 초기화
        foreach (Transform child in pagingRoot.transform)
        {
            child.localScale = new Vector3(1, 1, 1);
        }

        // 하단의 현재 페이지를 나타내는 서클 크기를 키움
        TweenManager.tween_Manager.TweenScale(pagingRoot.transform.GetChild(stickerContentsIdx).gameObject);        
    }

    /// <summary> 
    /// 하단의 현재 페이지 표시 서클을 누르면 해당 페이지를 화면에 표시 (현재 사용하지 않음)
    /// </summary>
    /// <param name="obj"></param>
    public void OnClickStickerPointBtn(GameObject obj)
    {
        if (obj == null)
        {
            return;
        }

        stickerContentsIdx = Convert.ToInt32(obj.name);      

        if (GlobalDataManager.m_SelectedSceneStateEnum == sequenceStickerContents[stickerContentsIdx] &&
            stickerContentsIdx == Convert.ToInt32(DragManager.GetComponent<StickerManager>().m_QuestionNum))
        {
            return;
        }

        if (GlobalDataManager.m_SelectedSceneStateEnum == GlobalDataManager.SceneState.CRANE)
        {
            PrincessManager.instance.ClawGameQuit();
        }

        ApplyStickerBookContents();
    }

    private void SettingQuizNumber(int currentContentIdx)
    {
        if (GlobalDataManager.m_SelectedSceneStateEnum == GlobalDataManager.SceneState.PUZZLE)
        {
            if (currentContentIdx == 1)
            {
                DragManager.GetComponent<StickerManager>().m_QuestionNum = SketchBookTargetInfo.QuizNum.ONE;
            }
            else if (currentContentIdx == 2)
            {
                DragManager.GetComponent<StickerManager>().m_QuestionNum = SketchBookTargetInfo.QuizNum.TWO;
            }
        }
    }

    /*
    private int QuestionNumberSetting(GlobalDataManager.SceneState checkState)
    {
        switch (checkState)
        {
            case GlobalDataManager.SceneState.MAP:
                return (int)stickerManager.m_MapQuizNum - 1;

            case GlobalDataManager.SceneState.PUZZLE:
                return (int)stickerManager.m_SketchPuzzleQuizNum - 1;

            case GlobalDataManager.SceneState.STICKER:
                return (int)stickerManager.m_StickerQuizNum - 1;

            default:
                Debug.Log("UI Type None or Error ");
                return 0;
        }
    }
    */
}
