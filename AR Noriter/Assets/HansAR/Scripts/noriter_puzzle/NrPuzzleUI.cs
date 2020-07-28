using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class NrPuzzleUI : MonoBehaviour
{

    public int targetIndex;
    public int clearIndex = 0;
    public enum Piece
    {
        None,
        Four,
        Nine
    }

    [Serializable]
    public class PuzzleUiSet
    {
        public GameObject ui;
        public GameObject pieceRootObj;
        public GameObject dragRootObj;

        [HideInInspector]
        public GameObject[] puzzlePiece;

        [HideInInspector]
        public GameObject[] dragPiece;

        [HideInInspector]
        public GameObject[] dragTargetPiece;

       // [HideInInspector]
        public Vector3[] startPos;

        [HideInInspector]
        public Vector3[] targetScale;
    }


    [Serializable]
    public class PuzzleInfo
    {
        public string bgSprName;
        public string[] fourSprName;
        public string[] nineSprName;
    }

    public Piece nowPieceMode;

    [HideInInspector]
    public GameObject clickObj;

    public GameObject puzzleUi;

    public GameObject restartUi;

    public GameObject resultUi;
    public GameObject prevBtn;
    public GameObject nextBtn;

    public GameObject puzzleBgObj;

    public UILabel stageLabel;

    [SerializeField]
    public PuzzleUiSet fourUi;

    [SerializeField]
    public PuzzleUiSet nineUi;


    [SerializeField]
    public PuzzleInfo[] puzzleInfo;

    private Coroutine DragCor;

    public static NrPuzzleUI instance;


    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        puzzleUi.GetComponent<UIPanel>().alpha = 0;
        PuzzleUiInit();
    }

    void Update()
    {

    }

    /// <summary>
    /// 퍼즐 셋팅 초기화
    /// </summary>
    private void PuzzleUiInit()
    {
        fourUi.puzzlePiece = new GameObject[fourUi.pieceRootObj.transform.childCount];
        fourUi.dragPiece = new GameObject[fourUi.puzzlePiece.Length];
        fourUi.dragTargetPiece = new GameObject[fourUi.puzzlePiece.Length];

        fourUi.startPos = new Vector3[fourUi.puzzlePiece.Length];
        fourUi.targetScale = new Vector3[fourUi.puzzlePiece.Length];


        for (int i = 0; i < fourUi.puzzlePiece.Length; i++)
        {
            fourUi.puzzlePiece[i] = fourUi.pieceRootObj.transform.GetChild(i).gameObject;
            fourUi.puzzlePiece[i].SetActive(true);

            fourUi.dragPiece[i] = fourUi.dragRootObj.transform.GetChild(i).gameObject;
            //fourUi.startPos[i] = fourUi.dragPiece[i].transform.localPosition;
            //fourUi.dragPiece[i].SetActive(false);
        }

        nineUi.puzzlePiece = new GameObject[nineUi.pieceRootObj.transform.childCount];
        nineUi.dragPiece = new GameObject[nineUi.puzzlePiece.Length];
        nineUi.dragTargetPiece = new GameObject[nineUi.puzzlePiece.Length];

        nineUi.startPos = new Vector3[nineUi.puzzlePiece.Length];
        nineUi.targetScale = new Vector3[nineUi.puzzlePiece.Length];

        for (int i = 0; i < nineUi.puzzlePiece.Length; i++)
        {
            nineUi.puzzlePiece[i] = nineUi.pieceRootObj.transform.GetChild(i).gameObject;
            nineUi.puzzlePiece[i].SetActive(true);

            nineUi.dragPiece[i] = nineUi.dragRootObj.transform.GetChild(i).gameObject;
            //nineUi.startPos[i] = nineUi.dragPiece[i].transform.localPosition;
            //nineUi.dragPiece[i].SetActive(false);
        }
    }

    /// <summary>
    /// 4피스 ui 초기화
    /// </summary>
    public void FourPieceUiInit()
    {
        PuzzleUiActive(false);
        PuzzlePieceActive(Piece.Four, false);

        fourUi.ui.GetComponent<UIPanel>().alpha = 1;//SetActive(true);
        TweenAlphaObj(puzzleUi, 0.5f, true);

        clearIndex = 0;
        stageLabel.GetComponent<LocalizeText>().ValueName = "Stage_One";
        stageLabel.text = LocalizeText.Value["Stage_One"];
        SprSet(targetIndex);
    }

    /// <summary>
    /// 9피스 ui 초기화
    /// </summary>
    public void NinePieceUiInit()
    {
        PuzzleUiActive(false);
        PuzzlePieceActive(Piece.Nine, false);

        nineUi.ui.GetComponent<UIPanel>().alpha = 1;//SetActive(true);
        TweenAlphaObj(puzzleUi, 0.5f, true);

        clearIndex = 0;
        stageLabel.GetComponent<LocalizeText>().ValueName = "Stage_Two";
        stageLabel.text = LocalizeText.Value["Stage_Two"];

        SprSet(targetIndex);
    }

    private void SprSet(int index)
    {
        if (puzzleBgObj != null)
        {
            puzzleBgObj.GetComponent<UISprite>().spriteName = puzzleInfo[index].bgSprName;
        }

        switch (nowPieceMode)
        {
            case Piece.Four:
                DragPieceSetting(fourUi.dragPiece, puzzleInfo[index].fourSprName);
                break;

            case Piece.Nine:
                DragPieceSetting(nineUi.dragPiece, puzzleInfo[index].nineSprName);
                break;
        }

        PuzzlePieceSprSet(index);
    }

    private void PuzzlePieceSprSet(int index)
    {
        switch (nowPieceMode)
        {
            case Piece.Four:
                for (int i = 0; i < puzzleInfo[index].fourSprName.Length; i++)
                {
                    fourUi.puzzlePiece[i].GetComponent<UISprite>().spriteName = puzzleInfo[index].fourSprName[i];
                }
                break;

            case Piece.Nine:
                for (int i = 0; i < puzzleInfo[index].nineSprName.Length; i++)
                {
                    nineUi.puzzlePiece[i].GetComponent<UISprite>().spriteName = puzzleInfo[index].nineSprName[i];
                }
                break;
        }
    }


    private void DragPieceColSet(bool state)
    {
        switch (nowPieceMode)
        {
            case Piece.Four:
                for (int i = 0; i < fourUi.dragPiece.Length; i++)
                {
                    fourUi.dragPiece[i].GetComponent<BoxCollider2D>().enabled = state;
                }
                break;

            case Piece.Nine:
                for (int i = 0; i < nineUi.dragPiece.Length; i++)
                {
                    nineUi.dragPiece[i].GetComponent<BoxCollider2D>().enabled = state;
                }
                break;
        }
    }


    public void PopUpOpen(GameObject obj)
    {
        if (obj != null)
        {
            TweenManager.tween_Manager.TweenAllDestroy(obj);

            if (obj.GetComponent<UIPanel>() != null)
            {
                TweenManager.tween_Manager.AddTweenAlpha(obj, obj.GetComponent<UIPanel>().alpha, 1, 0.2f);
            }
            else
            {
                TweenManager.tween_Manager.AddTweenAlpha(obj, obj.GetComponent<UIWidget>().alpha, 1, 0.2f);
            }
            TweenManager.tween_Manager.AddTweenScale(obj, obj.transform.localScale, Vector3.one, 0.2f);

            TweenManager.tween_Manager.TweenAlpha(obj);
            TweenManager.tween_Manager.TweenScale(obj);
        }
        else
        {
            Debug.Log("오브젝트 없음");
        }
    }

    public void PopUpClose(GameObject obj)
    {
        if (obj != null)
        {
            TweenManager.tween_Manager.TweenAllDestroy(obj);

            if (obj.GetComponent<UIPanel>() != null)
            {
                TweenManager.tween_Manager.AddTweenAlpha(obj, obj.GetComponent<UIPanel>().alpha, 0, 0.2f);
            }
            else
            {
                TweenManager.tween_Manager.AddTweenAlpha(obj, obj.GetComponent<UIWidget>().alpha, 0, 0.2f);
            }
            TweenManager.tween_Manager.TweenAlpha(obj);
        }
        else
        {
            Debug.Log("오브젝트 없음");
        }
    }

    public void RestartPopUpOpenClick()
    {
        PopUpOpen(restartUi);
    }

    public void RestartClick()
    {
        NrPuzzleManager.instance.LevelingSetting(nowPieceMode);

        PopUpClose(restartUi);
    }

    public void RestartEndBtnClick()
    {
        PopUpClose(puzzleUi);
        PopUpClose(restartUi);

        TargetManager.EnableTracking = true;
        AutoFocusMode.getInstance.OnOffAutoFucousMode(true);
        MainUI.메인.인식글자UI.SetActive(true);
    }

    public void RestartPopCancle()
    {
        PopUpClose(restartUi);
    }


    /// <summary>
    /// 
    /// </summary>
    public void ResultPopUpOpen()
    {
        PopUpOpen(resultUi);
    }

    public void ResultRestartBtnClick()
    {
        NrPuzzleManager.instance.LevelingSetting(nowPieceMode);

        PopUpClose(resultUi);
    }

    public void ResultPrevBtnClick()
    {
        nowPieceMode = Piece.Four;
        ResultRestartBtnClick();
    }

    public void ResultNextBtnClick()
    {
        nowPieceMode = Piece.Nine;
        ResultRestartBtnClick();
    }

    public void ResultEndBtnClick()
    {
        PopUpClose(puzzleUi);
        PopUpClose(resultUi);

        TargetManager.EnableTracking = true;
        AutoFocusMode.getInstance.OnOffAutoFucousMode(true);
        MainUI.메인.인식글자UI.SetActive(true);
    }



    public void OnPressEvent(GameObject obj)
    {
        PressObjPosSave(obj);
        DragPieceColSet(false);
        obj.GetComponent<BoxCollider2D>().enabled = true;
        clickObj = obj;

        float tweenTime = 0.2f;

        TweenManager.tween_Manager.TweenAllDestroy(obj);
        TweenManager.tween_Manager.AddTweenScale(obj,
                                                 obj.transform.localScale,
                                                 Vector3.one,
                                                 tweenTime,
                                                 UITweener.Style.Once,
                                                 TweenManager.tween_Manager.scaleAnimationCurve);

        TweenManager.tween_Manager.TweenScale(obj);
    }

    private void PressObjPosSave(GameObject obj)
    {
        switch(nowPieceMode)
        {
            case Piece.Four:
                for(int i=0; i < fourUi.dragPiece.Length; i++)
                {
                    if(obj == fourUi.dragPiece[i])
                    {
                        fourUi.startPos[i] = obj.transform.localPosition;
                        break;
                    }
                }
                break;

            case Piece.Nine:
                for (int i = 0; i < nineUi.dragPiece.Length; i++)
                {
                    if (obj == nineUi.dragPiece[i])
                    {
                        nineUi.startPos[i] = obj.transform.localPosition;
                        break;
                    }
                }
                break;

            default:
                Debug.Log("난이도 확인 필요");
                break;
        }
    }


    public void OnReleaseEvent()
    {
        if (clickObj != null)
        {
            DragCorStart(clickObj);
        }

    }

    private void DragCorStart(GameObject obj)
    {
        DragCorStop();
        DragCor = StartCoroutine(DragCorEvent(obj));
    }

    private void DragCorStop()
    {
        if (DragCor != null)
        {
            StopCoroutine(DragCor);
            DragCor = null;
        }
    }

    /// <summary>
    /// 4피스, 9피스 ui active 설정 변경
    /// </summary>
    /// <param name="state"></param>
    private void PuzzleUiActive(bool state)
    {
        if (!state)
        {
            fourUi.ui.GetComponent<UIPanel>().alpha = 0;// SetActive(state);
            nineUi.ui.GetComponent<UIPanel>().alpha = 0; //SetActive(state);
        }
        else
        {
            fourUi.ui.GetComponent<UIPanel>().alpha = 1;// SetActive(state);
            nineUi.ui.GetComponent<UIPanel>().alpha = 1; //SetActive(state);
        }
    }


    private void PuzzlePieceActive(Piece mode, bool state)
    {
        switch (mode)
        {
            case Piece.Four:
                for (int i = 0; i < fourUi.puzzlePiece.Length; i++)
                {
                    fourUi.puzzlePiece[i].SetActive(state);
                }
                break;

            case Piece.Nine:
                for (int i = 0; i < nineUi.puzzlePiece.Length; i++)
                {
                    nineUi.puzzlePiece[i].SetActive(state);
                }
                break;
        }
    }

    private void PuzzleFinish()
    {
        int limitIndex = 0;

        prevBtn.SetActive(false);
        nextBtn.SetActive(false);

        switch (nowPieceMode)
        {
            case Piece.Four:
                limitIndex = 4;
                nextBtn.SetActive(true);
                break;

            case Piece.Nine:
                limitIndex = 9;
                prevBtn.SetActive(true);
                break;
        }

        if (clearIndex >= limitIndex)
        {
            ResultPopUpOpen();
            Debug.Log("끝 결과창 나오는곳");
        }
    }


    private void DragPieceSetting(GameObject[] dragPieceList, string[] sprNameList)
    {
        bool isSame = false;

        int sprIndex = -1;

        for (int i = 0; i < dragPieceList.Length; ++i)
        {
            TweenManager.tween_Manager.TweenAllDestroy(dragPieceList[i].transform.GetChild(0).gameObject);

            while (true)
            {
                sprIndex = UnityEngine.Random.Range(0, sprNameList.Length);
                dragPieceList[i].transform.GetChild(0).GetComponent<UISprite>().spriteName = sprNameList[sprIndex];
                dragPieceList[i].GetComponent<UIWidget>().alpha = 1;
                dragPieceList[i].GetComponent<UIWidget>().ResetAnchors();

                switch (nowPieceMode)
                {
                    case Piece.Four:
                        fourUi.dragTargetPiece[i] = fourUi.puzzlePiece[sprIndex];
                        dragPieceList[i].transform.GetChild(0).GetComponent<UIWidget>().width = fourUi.puzzlePiece[sprIndex].GetComponent<UIWidget>().width;
                        dragPieceList[i].transform.GetChild(0).GetComponent<UIWidget>().height = fourUi.puzzlePiece[sprIndex].GetComponent<UIWidget>().height;

                        dragPieceList[i].transform.localScale = fourUi.puzzlePiece[sprIndex].transform.localScale * 0.6f;
                        //dragPieceList[i].transform.localPosition = fourUi.startPos[i];
                        dragPieceList[i].SetActive(true);

                        //fourUi.startPos[i] = dragPieceList[i].transform.localPosition;
                        break;

                    case Piece.Nine:
                        nineUi.dragTargetPiece[i] = nineUi.puzzlePiece[sprIndex];
                        dragPieceList[i].transform.GetChild(0).GetComponent<UIWidget>().width = nineUi.puzzlePiece[sprIndex].GetComponent<UIWidget>().width;
                        dragPieceList[i].transform.GetChild(0).GetComponent<UIWidget>().height = nineUi.puzzlePiece[sprIndex].GetComponent<UIWidget>().height;

                        dragPieceList[i].transform.localScale = nineUi.puzzlePiece[sprIndex].transform.localScale * 0.6f;
                        //dragPieceList[i].transform.localPosition = nineUi.startPos[i];
                        dragPieceList[i].SetActive(true);

                       // nineUi.startPos[i] = dragPieceList[i].transform.localPosition;

                        break;
                }
                

                isSame = false;

                for (int j = 0; j < i; ++j)
                {
                    if (dragPieceList[j].transform.GetChild(0).GetComponent<UISprite>().spriteName == dragPieceList[i].transform.GetChild(0).GetComponent<UISprite>().spriteName)
                    {
                        isSame = true;
                        break;
                    }
                }

                if (!isSame)
                {
                    break;
                }
            }
        }
    }


    private void TweenAlphaObj(GameObject obj, float tweenTime, bool state)
    {
        TweenManager.tween_Manager.TweenAllDestroy(obj);

        if (state)
        {
            TweenManager.tween_Manager.AddTweenAlpha(obj, 0, 1, tweenTime);

        }
        else
        {
            TweenManager.tween_Manager.AddTweenAlpha(obj, 1, 0, tweenTime);
        }

        TweenManager.tween_Manager.TweenAlpha(obj);
    }


    private IEnumerator DragCorEvent(GameObject obj)
    {

        obj.GetComponent<BoxCollider2D>().enabled = false;

        GameObject targetObj = null;

        float tweenTime = 0.2f;

        Vector3 startPosSave = Vector3.zero;

        float limitDis = 0.2f;
        float distance = 0;

        switch (nowPieceMode)
        {
            case Piece.Four:
                for (int i = 0; i < fourUi.dragPiece.Length; i++)
                {
                    if (clickObj == fourUi.dragPiece[i])
                    {
                        targetObj = fourUi.dragTargetPiece[i];
                        startPosSave = fourUi.startPos[i];
                    }
                }

                break;
            case Piece.Nine:
                for (int i = 0; i < nineUi.dragPiece.Length; i++)
                {
                    if (clickObj == nineUi.dragPiece[i])
                    {
                        targetObj = nineUi.dragTargetPiece[i];
                        startPosSave = nineUi.startPos[i];
                    }
                }
                break;
        }

        distance = Vector3.Distance(clickObj.transform.position, targetObj.transform.position);

        TweenManager.tween_Manager.TweenAllDestroy(clickObj);

        if (distance < limitDis)
        {
            tweenTime = 0.15f;

            TweenManager.tween_Manager.AddTweenPosition(clickObj, clickObj.transform.localPosition, targetObj.transform.localPosition, tweenTime);
            TweenManager.tween_Manager.AddTweenAlpha(clickObj, clickObj.GetComponent<UIWidget>().alpha, 0, tweenTime);

            TweenManager.tween_Manager.TweenPosition(clickObj);
            TweenManager.tween_Manager.TweenAlpha(clickObj);

            yield return new WaitForSeconds(tweenTime);

            TweenManager.tween_Manager.TweenAllDestroy(clickObj);
            TweenManager.tween_Manager.AddTweenPosition(clickObj, clickObj.transform.localPosition, startPosSave, 0.1f);
            TweenManager.tween_Manager.TweenPosition(clickObj);

            TweenManager.tween_Manager.TweenAllDestroy(targetObj);
            TweenManager.tween_Manager.AddTweenAlpha(targetObj, targetObj.GetComponent<UIWidget>().alpha, 1, tweenTime);
            TweenManager.tween_Manager.TweenAlpha(targetObj);

            yield return new WaitForSeconds(tweenTime);

            tweenTime = 0.2f;

            TweenManager.tween_Manager.AddTweenScale(targetObj,
                                                     targetObj.transform.localScale,
                                                     targetObj.transform.localScale,
                                                     tweenTime,
                                                     UITweener.Style.Once,
                                                     TweenManager.tween_Manager.scaleAnimationCurve);

            TweenManager.tween_Manager.TweenScale(targetObj);

            clearIndex++;

            yield return new WaitForSeconds(tweenTime);
        }
        else
        {
            tweenTime = 0.1f;
            TweenManager.tween_Manager.AddTweenPosition(clickObj, clickObj.transform.localPosition, startPosSave, tweenTime);
            TweenManager.tween_Manager.AddTweenScale(clickObj, clickObj.transform.localScale, new Vector3(0.6f, 0.6f, 0.6f), tweenTime);

            TweenManager.tween_Manager.TweenPosition(clickObj);
            TweenManager.tween_Manager.TweenScale(clickObj);

            yield return new WaitForSeconds(tweenTime);
        }

        obj.GetComponent<BoxCollider2D>().enabled = true;


        clickObj = null;

        DragPieceColSet(true);

        PuzzleFinish();

        yield break;
    }

}
