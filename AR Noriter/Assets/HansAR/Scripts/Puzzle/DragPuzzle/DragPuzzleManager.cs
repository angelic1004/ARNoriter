using UnityEngine;
using System;
using System.Collections;

using HedgehogTeam.EasyTouch;

public class DragPuzzleManager : MonoBehaviour
{
    private static float PIECES_MERGE_DISTANCE = 0.2f;   // 두 조각이 합쳐지는 기준 거리

    public int nowQuestionSuccessIndex = 0;

    [HideInInspector]
    public GameObject SceneModeMainUI;

    [HideInInspector]
    public GameObject matchingPieceParent;               // 맞출 그림들 부모 오브젝트 (셋팅용)
    private GameObject[] matchingPieces;                 // 맞출 그림들

    [Serializable]
    public class PuzzlePieceSetting
    {
        public GameObject puzzlePieceParent;                 // 드래그 할 그림들 부모 오브젝트 (셋팅용)
        public GameObject[] puzzlePieces;                   // 드래그 할 그림들
        public Vector3[] puzzlePiecePosition;               // 드래그 할 그림들 위치
    }

    //[HideInInspector]
    public PuzzlePieceSetting puzzlePieceSettingInfo;

    [Serializable]
    public class DragAudioSetting
    {
        public AudioSource dragAudioSource;
        public AudioClip dragSuccessAudioClip;
        public AudioClip dragFailAudioClip;
        public AudioClip dragEndAudioClip;
    }

    [SerializeField]
    public DragAudioSetting m_dragAudioSet;

    private GameObject touchMatchingPiece;               // 터치한 오브젝트의 맞출 그림
    private GameObject touchPiece;                       // 터치한 오브젝트
    private TweenTransform touchPieceTween;              // 터치한 조각 트윈
    private UISprite touchUISprite;                      // 터치한 조각 UI Sprite

    [HideInInspector]
    public GameObject pieceStartPos;                     // 트윈 시작 위치

    [HideInInspector]
    public GameObject tweenCtrlPos;                      // 트윈 컨트롤 위치

    void Awake()
    {
       // GlobalDataManager.m_SelectedSceneStateEnum = GlobalDataManager.SceneState.PUZZLE;
        // InitDragPuzzleManager();
    }

    void Start()
    {
        //InitDragPuzzleManager();
    }

    void OnEnable()
    {
        EasyTouch.On_TouchUp += EasyTouch_TouchUp;
        EasyTouch.On_TouchStart += EasyTouch_2D_TouchDown;
    }

    void OnDestroy()
    {
        UnsubscribeEvent();
    }

    void OnDisable()
    {
        UnsubscribeEvent();
    }

    void UnsubscribeEvent()
    {
        EasyTouch.On_TouchUp -= EasyTouch_TouchUp;
        EasyTouch.On_TouchStart -= EasyTouch_2D_TouchDown;
    }

    /// <summary>
    /// 드래그 해서 놓을 위치에 있는 오브젝트 초기화 및 저장
    /// </summary>
    protected void InitDragPuzzleManager()
    {
        Screen.orientation = ScreenOrientation.Landscape;
        InitDragMachingPieces(false);
        InitPuzzlePieceSetting();
    }

    /// <summary>
    /// 드래그 오브젝트 저장 위치로 초기화
    /// </summary>
    protected void InitPuzzlePieceSetting()
    {
        InitPuzzlePiecePrevSetting();

        puzzlePieceSettingInfo.puzzlePieces = new GameObject[puzzlePieceSettingInfo.puzzlePieceParent.transform.childCount];
        puzzlePieceSettingInfo.puzzlePiecePosition = new Vector3[puzzlePieceSettingInfo.puzzlePieceParent.transform.childCount];

        int i = 0;
        foreach (Transform child in puzzlePieceSettingInfo.puzzlePieceParent.transform)
        {
            puzzlePieceSettingInfo.puzzlePieces[i] = child.gameObject;
            puzzlePieceSettingInfo.puzzlePiecePosition[i] = child.gameObject.transform.localPosition;
            
            i++;
        }
    }

    protected void InitDragMachingPieces(bool enabled)
    {
        matchingPieces = new GameObject[matchingPieceParent.transform.childCount];

        int i = 0;
        if (!enabled)
        {
            foreach (Transform child in matchingPieceParent.transform)
            {
                matchingPieces[i] = child.gameObject;
                matchingPieces[i].GetComponent<BoxCollider2D>().enabled = false;
                matchingPieces[i].GetComponent<TweenAlpha>().enabled = false;
                matchingPieces[i].GetComponent<UISprite>().alpha = 0;
                i++;
            }
        }
        else
        {
            foreach (Transform child in matchingPieceParent.transform)
            {
                matchingPieces[i] = child.gameObject;
                matchingPieces[i].GetComponent<BoxCollider2D>().enabled = true;
                matchingPieces[i].GetComponent<UISprite>().alpha = 0;
                i++;
            }
        }
    }

    /// <summary>
    /// 드래그 오브젝트 원래 위치 저장
    /// </summary>
    private void InitPuzzlePiecePrevSetting()
    {
        if (puzzlePieceSettingInfo.puzzlePieces.Length > 0)
        {
            for (int i = 0; i < puzzlePieceSettingInfo.puzzlePieces.Length; i++)
            {
                puzzlePieceSettingInfo.puzzlePieces[i].SetActive(true);
                puzzlePieceSettingInfo.puzzlePieces[i].transform.localPosition = puzzlePieceSettingInfo.puzzlePiecePosition[i];
                //puzzlePieceSettingInfo.puzzlePieces[i].GetComponent<UISprite>().SetAnchor(null, 0, 0, 0, 0);
                // puzzlePieceSettingInfo.puzzlePieces[i].GetComponent<UISprite>().SetDimensions(matchingPieces[i].GetComponent<UISprite>().width, matchingPieces[i].GetComponent<UISprite>().height);
            }
        }
    }

    private void EasyTouch_2D_TouchDown(Gesture gesture)
    {
        if (gesture.touchCount > 1)
        {
            return;
        }

        if (gesture.pickedObject != null && gesture.pickedObject.tag == "puzzle_piece")
        {
            touchPiece = gesture.pickedObject;
            touchPieceTween = touchPiece.GetComponent<TweenTransform>();

            // 트윈이 실행 중일 때는 리턴
            if (touchPieceTween.enabled)
            {
                return;
            }

            touchUISprite = touchPiece.GetComponent<UISprite>();
            touchUISprite.depth = 4;

            pieceStartPos.transform.localPosition = touchPiece.transform.localPosition;

            // 터치한 조각과 같은 이름의 맞출 그림을 찾아서 지정함
            for (int i = 0; i < matchingPieces.Length; i++)
            {
                if (touchPiece.name == matchingPieces[i].name)
                {
                    touchMatchingPiece = matchingPieces[i];
                    Debug.Log("touchMatchingPiece : " + touchMatchingPiece);
                    break;
                }
            }
        }
    }

    private void EasyTouch_TouchUp(Gesture gesture)
    {
        if (touchPiece != null)
        {
            PlayPieceTween();

            touchUISprite.depth = 4;
        }
    }

    /// <summary>
    /// 퍼즐 조각의 트윈을 재생합니다.
    /// </summary>
    private void PlayPieceTween()
    {
        if (touchMatchingPiece == null)
        {
            //Debug.LogWarning("touchMatchingPiece value is null");
            return;
        }

        // 터치한 조각과 매칭할 조각과의 거리
        var distance            = 0f;
        distance                = Vector3.Distance(touchPiece.transform.position, touchMatchingPiece.transform.position);

        Debug.Log("두 조각 간 거리: " + distance);

        tweenCtrlPos.transform.localPosition    = touchPiece.transform.localPosition;
        touchPieceTween.from    = tweenCtrlPos.transform;

        TweenTransform tt       = touchPieceTween.GetComponent<TweenTransform>();
        EventDelegate ed        = new EventDelegate();

        ed.target               = this;
        ed.methodName           = "PieceCorrect";
        ed.parameters[0].value  = touchPiece;    
        ed.parameters[1].value  = touchMatchingPiece;

        tt.RemoveOnFinished(ed);

        // 두 조각이 설정된 거리보다 가까우면 두 조각을 겹치게 하고 멀면 제자리로 돌아감
        if (distance < PIECES_MERGE_DISTANCE)
        {
            touchPieceTween.to          = touchMatchingPiece.transform;
            touchPieceTween.duration    = 0.3f;
            nowQuestionSuccessIndex++;

            Debug.Log("touchMatchingPiece : " + touchMatchingPiece);
            tt.onFinished.Add(ed);
        }
        else
        {
            touchPieceTween.to          = pieceStartPos.transform;
            touchPieceTween.duration    = 0.4f;

            DragSoundPlay(false);
        }

        touchPieceTween.ResetToBeginning();
        touchPieceTween.PlayForward();    

        if (touchMatchingPiece != null)
        {
            touchMatchingPiece = null;
        }

        if (touchPiece != null)
        {
            touchPiece = null;
        }
    }

    /// <summary>
    /// 정답일 경우 해당 위치에 있는 오브젝트의 alpha값을 올려 붙은 것처럼 보여준다.
    /// 그리고 드래그한 오브젝트는 active false 해준다.
    /// </summary>
    private void PieceCorrect(GameObject touchPieceObj, GameObject matchingPieceObj)
    {
        Debug.Log("touchPieceObj : " + touchPieceObj.name);
        Debug.Log("matchingPieceObj : " + matchingPieceObj.name);

        matchingPieceObj.GetComponent<UISprite>().alpha         = 255;
        matchingPieceObj.GetComponent<BoxCollider2D>().enabled  = true;
        matchingPieceObj.GetComponent<UIButton>().tweenTarget   = null;

        TweenManager.tween_Manager.TweenAlpha(matchingPieceObj);
        SketchBookUI.getInstance.CorrectAnswerCheck(matchingPieceObj, true);

        DragSoundPlay(true);
        SketchPuzzleBoxSetting(true);

        touchPieceObj.SetActive(false);
        touchPieceObj = null;        
    }

    /// <summary>
    /// 정답일 경우 사운드 재생
    /// </summary>
    private void DragSoundPlay(bool awareness)
    {
        m_dragAudioSet.dragAudioSource.Stop();
        if(awareness)
        {
            if (nowQuestionSuccessIndex >= puzzlePieceSettingInfo.puzzlePieces.Length)
            {
                m_dragAudioSet.dragAudioSource.clip = m_dragAudioSet.dragEndAudioClip;
                Debug.Log("끝사운드");
            }
            else
            {
                m_dragAudioSet.dragAudioSource.clip = m_dragAudioSet.dragSuccessAudioClip;
                Debug.Log("성공사운드");
            }
            m_dragAudioSet.dragAudioSource.Play();
        }
        /*
        else
        {
            m_dragAudioSet.dragAudioSource.clip = m_dragAudioSet.dragFailAudioClip;
            Debug.Log("실패사운드");
        }
        
        m_dragAudioSet.dragAudioSource.Play();
        */
    }

    protected void SketchPuzzleBoxSetting(bool enabled)
    {
        if (GlobalDataManager.m_SelectedSceneStateEnum == GlobalDataManager.SceneState.PUZZLE)
        {
            if (enabled)
            {
                if (nowQuestionSuccessIndex >= puzzlePieceSettingInfo.puzzlePieces.Length)
                {
                    SceneModeMainUI.GetComponent<BoxCollider2D>().enabled = true;
                }
            }
            else
            {
                SceneModeMainUI.GetComponent<BoxCollider2D>().enabled = false;
            }
        }
    }
}