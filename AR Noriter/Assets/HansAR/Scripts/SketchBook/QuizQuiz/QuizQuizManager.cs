using UnityEngine;
using System.Collections;
using System.Text;

public class QuizQuizManager : Singleton<QuizQuizManager>
{
    public bool m_3Dset;

    /// <summary>
    /// quizquiz 진행 상태
    /// </summary>
    public enum QuizState
    {
        NONE,
        READY,
        PLAYING,
        END
    }
    public SketchBookTargetInfo.QuizNum m_QuizNum;

    public QuizState m_QuizState;

    private QuizQuizInfo quizQuizInfo;

    /// <summary>
    /// 텍스트 한글자씩 나오는 코루틴
    /// </summary>
    private Coroutine textCoroutine;

    private StringBuilder m_Builder;

    void Start()
    {
        m_Builder = new StringBuilder();
        m_Builder.Remove(0, m_Builder.Length);
        m_QuizState = QuizState.READY;
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0) && m_3Dset)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.tag == "quizui")
                {
                    QuizBtnClick(hit.transform.gameObject);
                }

            }
        }
    }

    /// <summary>
    /// 퀴즈 시작부분
    /// </summary>
    public void QuizTextStart()
    {
        QuizTextStop();
        QuizQuizUI.getInstance.QuizListInit();
        textCoroutine = StartCoroutine(QuizTextPrint());
    }

    /// <summary>
    /// 텍스트 출력부분 STOP
    /// </summary>
    private void QuizTextStop()
    {
        if (textCoroutine != null)
        {
            StopCoroutine(textCoroutine);
            textCoroutine = null;
        }
    }

    /// <summary>
    /// 문제 텍스트 tween 컨트롤 부분 
    /// </summary>
    public void QuizTextTween()
    {
        QuizTextStart();
        TweenManager.tween_Manager.TweenAlpha(QuizQuizUI.getInstance.QuizText.gameObject);
    }

    /// <summary>
    /// 정답 선택 출력 tween 컨트롤 부분
    /// </summary>
    public void QuizSelectTween()
    {
        TweenManager.tween_Manager.TweenAlpha(QuizQuizUI.getInstance.QuizSelect);
        TweenManager.tween_Manager.TweenAlpha(SketchBookUI.getInstance.quizOtherUI);
        if (m_3Dset)
        {
            QuizQuizUI.getInstance.QuizSelect3DSetting();
        }
    }

    /// <summary>
    /// 정답 선택 출력 tween 컨트롤 Stop 부분
    /// </summary>
    public void QuizSelectTweenStop()
    {
        TweenManager.tween_Manager.TweenAlpha(QuizQuizUI.getInstance.QuizSelect);
        QuizQuizUI.getInstance.QuizSelect.GetComponent<TweenAlpha>().enabled = false;
    }

    /// <summary>
    /// 물방울 모양 원형 test 중인 부분 tween 컨트롤 
    /// </summary>
    public void QuizCircleTween()
    {
        for (int i = 0; i < QuizQuizUI.getInstance.QuizTextCircle.Length; i++)
        {
            TweenManager.tween_Manager.TweenScale(QuizQuizUI.getInstance.QuizTextCircle[i]);
            QuizQuizUI.getInstance.QuizTextCircle[i].GetComponent<TweenScale>().enabled = false;
        }
        TweenManager.tween_Manager.TweenScale(QuizQuizUI.getInstance.QuizTextCircle[0]);
    }

    /// <summary>
    /// QuizQuiz 정답 확인
    /// </summary>
    /// <param name="obj"></param>
    public void QuizBtnClick(GameObject obj)
    {
        // SketchBookUI.getInstance.CorrectAnswerCheck()
        GameObject answerObj = QuizQuizUI.getInstance.QuizSelect.GetComponent<QuizQuizInfo>().AnswerObj;

        //정답
        if (answerObj.name == obj.name)
        {
            SketchBookUI.getInstance.CorrectAnswerCheck(obj, true);
        }
        else
        {
            SketchBookUI.getInstance.CorrectAnswerCheck(obj, false);
        }
            // SketchBookUI.getInstance.CorrectAnswerCheck()
            
       // QuizBtnClickEventPlay(obj);
    }

    public void QuizBtnClickEventPlay(GameObject obj)
    {
        // SketchBookUI.getInstance.CorrectAnswerCheck()
        GameObject answerObj = QuizQuizUI.getInstance.QuizSelect.GetComponent<QuizQuizInfo>().AnswerObj;

        //정답
        if (answerObj.name == obj.name)
        {
            //QuizQuizUI.getInstance.QuizInit();
            TouchEventManager.터치.기준콜라이더 = TouchEventManager.터치.지형2D;
            QuizQuizUI.getInstance.QuizChoiceCheck(true);
            QuizQuizUI.getInstance.QuizSelect.SetActive(false);
            MainUI.메인.애니메이션동작_UI보이기();
            RotateUI.회전.회전UI_보이기();

            Debug.Log("맞음 : 추후 이벤트, 이미지 작업 추가");
        }
        //오답
        else
        {
            QuizQuizUI.getInstance.QuizChoiceCheck(false);
            Debug.Log("틀림 : 추후 이벤트, 이미지 작업 추가");
        }
    }

    /// <summary>
    /// 문제 텍스트 순서대로 출력 되는 부분
    /// </summary>
    /// <returns></returns>
    private IEnumerator QuizTextPrint()
    {
        int stringIndex = 0;
        int quizListIndex = (int)m_QuizNum - 1;
        float nextTime = 0.1f;
        float savedNextTime = nextTime;

        //QuizCircleTween();
        QuizSelectTweenStop();

        while (true)
        {
            nextTime = nextTime - Time.deltaTime;

            if (nextTime <= 0)
            {
                if (stringIndex > QuizQuizUI.getInstance.QuizList[quizListIndex].quiz_String.Length)
                {
                    Debug.Log("텍스트 코루틴 끝");
                    SketchBookUI.getInstance.quizOtherUI.SetActive(true);
                    QuizSelectTween();
                    yield break;
                }

                QuizQuizUI.getInstance.QuizText.text 
                    = m_Builder.AppendFormat("{0}", QuizQuizUI.getInstance.QuizList[quizListIndex].quiz_String.Substring(0, stringIndex)).ToString();
                m_Builder.Remove(0, m_Builder.Length);
                nextTime = savedNextTime;
                stringIndex++;
            }
            yield return new WaitForEndOfFrame();
        }
    }

}
