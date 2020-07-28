using UnityEngine;
using System;
using System.Text;
using System.Collections;

using Vuforia;


public class StickerManager : DragPuzzleManager
{
    public GameObject backBtn;

    public SketchBookTargetInfo.QuizNum m_QuestionNum;

    /// <summary>
    /// 텍스트 한글자씩 나오는 코루틴
    /// </summary>
    private Coroutine textCoroutine;

    private StringBuilder m_Builder;

    void Awake()
    {
       
    }

    void Start()
    {
        m_Builder = new StringBuilder();
        m_Builder.Remove(0, m_Builder.Length);
    }

    void Update()
    {

    }

    /// <summary>
    /// drag 종류 셋팅
    /// </summary>
    public void InitDragQuiz()
    {
        InitDragPuzzleManager();
        InitPuzzlePieceSetting();
    }

    public void SketchPuzzleBoxInit(bool enabled)
    {
        SketchPuzzleBoxSetting(enabled);
    }

    /// <summary>
    /// 정답을 맞춘 스티커를 클릭시
    /// </summary>
    /// <param name="obj"></param>
    public void StickerClickEvent(GameObject obj)
    {
        int objNum = SceneModeMainUI.GetComponent<QuestionInfo>().m_3dModelNum[Convert.ToInt32(obj.name)];
        StickerManager sticker = SketchBookUI.getInstance.DragManager.GetComponent<StickerManager>();

        if (TargetManager.타깃메니저.SceneMode != GlobalDataManager.SceneState.PUZZLE)
        {
            TargetManager.타깃메니저.HideAllModelingContents();
            TargetManager.타깃메니저.복제모델링인덱스 = objNum;
            TargetManager.타깃메니저.마커비인식호출(objNum);

            SceneModeMainUI.SetActive(false);
            SketchBookUI.getInstance.DragQuizText.gameObject.SetActive(false);

            RotateUI.회전.회전UI_보이기();
            MainUI.메인.애니메이션동작_UI보이기();

            AnimationManager.애니메이션.애니메이션01_재생();
            backBtn.transform.parent = SketchBookUI.getInstance.otherUI.transform;            
            backBtn.SetActive(true);
        }
        else
        {
            if (sticker.nowQuestionSuccessIndex >= sticker.puzzlePieceSettingInfo.puzzlePieces.Length)
            {
                TargetManager.타깃메니저.HideAllModelingContents();
                TargetManager.타깃메니저.복제모델링인덱스 = objNum;
                TargetManager.타깃메니저.마커비인식호출(objNum);

                SceneModeMainUI.SetActive(false);
                SketchBookUI.getInstance.DragQuizText.gameObject.SetActive(false);

                RotateUI.회전.회전UI_보이기();
                MainUI.메인.애니메이션동작_UI보이기();

                backBtn.transform.parent = SketchBookUI.getInstance.otherUI.transform;
                backBtn.SetActive(true);
            }
            else
            {
                Debug.Log("아직 퍼즐이 전부 맞춰지지 않았습니다.");
            }
        }

        bool 인식중 = TargetManager.타깃메니저.QuizQuizSceneModeCheck(TargetManager.trackableStatus);

    }

    /// <summary>
    /// drag 뒤로가기 버튼 클릭시 
    /// </summary>
    public void StickerBackClickEvent()
    {
        if (!SceneModeMainUI.activeSelf)
        {
            SceneModeMainUI.SetActive(true);
            SketchBookUI.getInstance.DragQuizText.gameObject.SetActive(true);

            backBtn.transform.parent = SceneModeMainUI.transform;

            if (SketchBookUI.getInstance.noUsingTarget)
            {
                backBtn.SetActive(false);
            }            

            TargetManager.타깃메니저.HideAllModelingContents();
            MainUI.메인.애니메이션동작_UI숨기기();
            RotateUI.회전.회전UI_숨기기();
        }
        else
        {
            SceneModeMainUI.SetActive(false);
            SketchBookUI.getInstance.DragQuizText.gameObject.SetActive(false);

            if (!SketchBookUI.getInstance.noUsingTarget)
            {
                MainUI.메인.인식글자UI.SetActive(true);
            }

            backBtn.SetActive(false);

            if (MainUI.메인.navigationUI != null)
            {
                Debug.Log("들어옴");
                MainUI.메인.navigationUI.SetActive(true);
            }
        }
    }
    /// <summary>
    /// 드래그 지문(텍스트) 보여줌
    /// </summary>
    /// <param name="nowQuestion"></param>
    public void DragTextStart(SketchBookUI.DragUiSetting nowQuestion)
    {
        DragTextStop();
        SketchBookUI.getInstance.DragQuizText.gameObject.SetActive(true);
        TweenManager.tween_Manager.TweenAlpha(SketchBookUI.getInstance.DragQuizText.gameObject);
        textCoroutine = StartCoroutine(DragTextPrint(nowQuestion));
    }

    /// <summary>
    /// 드래그 지문(텍스트) 보여주는 것을 멈춥니다.
    /// </summary>
    private void DragTextStop()
    {
        if (textCoroutine != null)
        {
            StopCoroutine(textCoroutine);
            textCoroutine = null;
        }
    }

    /// <summary>
    /// 문제 텍스트 순서대로 출력 되는 부분
    /// </summary>
    /// <returns></returns>
    private IEnumerator DragTextPrint(SketchBookUI.DragUiSetting nowQuestion)
    {
        int stringIndex = 0;
        //int quizListIndex = (int)m_StickerQuizNum - 1;
        float nextTime = 0.1f;
        float savedNextTime = nextTime;

        //QuizCircleTween();
        DragUiSelectTweenStop(nowQuestion);

        while (true)
        {
            nextTime = nextTime - Time.deltaTime;

            if (nextTime <= 0)
            {
                if (stringIndex > nowQuestion.QuizText.Length)
                {
                    Debug.Log("텍스트 코루틴 끝");
                    DragUiSelectTween(nowQuestion);
                    yield break;
                }

                SketchBookUI.getInstance.DragQuizText.text
                     = m_Builder.AppendFormat("{0}", nowQuestion.QuizText.Substring(0, stringIndex)).ToString();
                m_Builder.Remove(0, m_Builder.Length);
                nextTime = savedNextTime;
                stringIndex++;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    /// <summary>
    /// drag 퀴즈(퍼즐) 정답 선택 출력 tween 컨트롤 부분
    /// </summary>
    public void DragUiSelectTween(SketchBookUI.DragUiSetting nowQuestion)
    {
        TweenManager.tween_Manager.TweenAlpha(nowQuestion.SceneMainUI);
    }

    /// <summary>
    /// drag 퀴즈(퍼즐) 정답 선택 출력 tween 컨트롤 Stop 부분
    /// </summary>
    public void DragUiSelectTweenStop(SketchBookUI.DragUiSetting nowQuestion)
    {
        TweenManager.tween_Manager.TweenAlpha(nowQuestion.SceneMainUI);
        nowQuestion.SceneMainUI.GetComponent<TweenAlpha>().enabled = false;
    }
}
