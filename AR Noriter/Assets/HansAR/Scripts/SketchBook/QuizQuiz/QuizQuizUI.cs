using UnityEngine;
using System.Collections;
using System;
using System.Linq;

public class QuizQuizUI : Singleton<QuizQuizUI>
{
    /// <summary>
    /// 퀴즈 모드 변경(순차적, 랜덤)
    /// </summary>
    public enum QuizMode
    {
        NONE,
        GRADUALLY,
        RANDOM
    }

    public QuizMode m_QuizMode;

    /// <summary>
    /// quiz 내용 
    /// </summary>
    public UILabel QuizText;

    public GameObject BackBtn;

    /// <summary>
    /// 현재 문제 오브젝트
    /// </summary>
   // [HideInInspector]
    public GameObject QuizSelect;

    public GameObject[] QuizTextCircle;

    /// <summary>
    /// 퀴즈 내용 및 버튼 
    /// </summary>
    [Serializable]
    public class QuizSetting
    {
        public GameObject quiz_ChoiceObj;
        public string quiz_String;

        [HideInInspector]
        public GameObject[] quiz_ChoiceList;
    }

    /// <summary>
    /// 퀴즈 문제 종류
    /// </summary>
    [SerializeField]
    public QuizSetting[] QuizList;

    /// <summary>
    /// quizquiz 사운 설저
    /// </summary>
    [Serializable]
    public class QuizSoundSetting
    {
        public AudioSource m_QuizAudioSource;
        public AudioClip m_QuizSuccessAudioClip;
        public AudioClip m_QuizFailAudioClip;
    }

    [SerializeField]
    public QuizSoundSetting QuizSound;

    private float boxSize = 0;

    Coroutine textCoroutine;

    public static QuizQuizUI Instance;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        QuizInit();
    }


    void Update()
    {

    }

    /// <summary>
    /// quiz 문제 alpha값 변경
    /// </summary>
    public void QuizInit()
    {
        QuizText.alpha = 0;

        for (int i = 0; i < QuizList.Length; i++)
        {
            QuizList[i].quiz_ChoiceObj.GetComponent<UISprite>().alpha = 0;
        }

        if(QuizSelect != null)
        {
            QuizSelect.SetActive(false);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void QuizListInit()
    {
        QuizInit();

        ConfigChildChoice();
        SketchBookUI.getInstance.quizOtherUI.SetActive(false);
    }

    /// <summary>
    /// choice : true 정답, flase 오답
    /// </summary>
    /// <param name="choice"></param>
    public void QuizChoiceCheck(bool choice)
    {
        QuizSelectCheck(choice);
    }

    /// <summary>
    /// 정답리스트 설정(배치)
    /// </summary>
    private void ConfigChildChoice()
    {
        int choiceCount;
        int choiceIndex;

        QuizSelect = QuizList[(int)QuizQuizManager.getInstance.m_QuizNum - 1].quiz_ChoiceObj;
        int idx = (int)QuizQuizManager.getInstance.m_QuizNum - 1;

        try
        {
            choiceCount = 0;
            choiceIndex = 0;

            string nonText = QuizList[idx].quiz_ChoiceObj.GetComponent<QuizQuizInfo>().nonText;

            choiceCount = GetChildChoiceCount(idx);

            if (choiceCount > 0)
            {
                QuizList[idx].quiz_ChoiceList = new GameObject[choiceCount];

                foreach (Transform trans in QuizList[idx].quiz_ChoiceObj.transform)
                {
                    QuizList[idx].quiz_ChoiceList[choiceIndex] = trans.gameObject;

                    if (QuizQuizManager.getInstance.m_3Dset)
                    {
                        Destroy(QuizList[idx].quiz_ChoiceList[choiceIndex].GetComponent<BoxCollider2D>());
                    }

                    if (m_QuizMode == QuizMode.GRADUALLY)
                    {
                        QuizList[idx].quiz_ChoiceList[choiceIndex].GetComponent<UIButton>().normalSprite = nonText + (choiceIndex + 1);
                        QuizList[idx].quiz_ChoiceList[choiceIndex].GetComponent<UISprite>().spriteName = nonText + (choiceIndex + 1);
                    }

                    choiceIndex = choiceIndex + 1;
                }

                if (m_QuizMode == QuizMode.RANDOM)
                {
                    QuizChocieRandomSetting(idx);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }

        boxSize = QuizList[idx].quiz_ChoiceList[0].GetComponent<BoxCollider2D>().size.x;
    }

    /// <summary>
    /// quiz문제에 따라 개수 셋팅
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private int GetChildChoiceCount(int index)
    {
        int choiceCount = 0;

        foreach (Transform trans in QuizList[index].quiz_ChoiceObj.transform)
        {
            choiceCount = choiceCount + 1;
        }
        return choiceCount;
    }

    /// <summary>
    /// 퀴즈선택지 위치 랜덤 셋팅
    /// </summary>
    /// <param name="idx"></param>
    private void QuizChocieRandomSetting(int idx)
    {
        int ranLength = QuizList[idx].quiz_ChoiceList.Length;
        int rightAnser = QuizList[idx].quiz_ChoiceObj.GetComponent<QuizQuizInfo>().rightAnswerNum;
        string nonText = QuizList[idx].quiz_ChoiceObj.GetComponent<QuizQuizInfo>().nonText;
        int ranIdx = 0;

        for (int i = 0; i < ranLength; i++)
        {
            ranIdx = UnityEngine.Random.Range(i, ranLength);

            GameObject tmp = QuizList[idx].quiz_ChoiceList[ranIdx];

            QuizList[idx].quiz_ChoiceList[ranIdx] = QuizList[idx].quiz_ChoiceList[i];
            QuizList[idx].quiz_ChoiceList[i] = tmp;

            int spriteNum = Convert.ToInt32(QuizList[idx].quiz_ChoiceList[i].name);

            QuizList[idx].quiz_ChoiceList[i].GetComponent<UIButton>().normalSprite = nonText + (i + 1);
            QuizList[idx].quiz_ChoiceList[i].GetComponent<UISprite>().spriteName = nonText + (i + 1);

            //정답 위치
            if (rightAnser == i)
            {
                QuizList[idx].quiz_ChoiceObj.GetComponent<QuizQuizInfo>().AnswerObj = QuizList[idx].quiz_ChoiceList[i];
            }
        }
    }

    /// <summary>
    /// 정답, 오답시 상태 변경 및 사운드 플레이
    /// </summary>
    /// <param name="select"></param>
    private void QuizSelectCheck(bool select)
    {
        QuizSound.m_QuizAudioSource.Stop();

        if (select)
        {
            TargetManager.타깃메니저.HideAllModelingContents();

            if (TargetManager.trackableStatus)
            {
                TargetManager.타깃메니저.컨텐츠오브젝트_위치(true);
                QuizQuizManager.getInstance.m_QuizState = QuizQuizManager.QuizState.END;
            }
            else
            {
                TargetManager.타깃메니저.컨텐츠오브젝트_위치(false);
                QuizQuizManager.getInstance.m_QuizState = QuizQuizManager.QuizState.READY;
            }

            QuizSound.m_QuizAudioSource.clip = QuizSound.m_QuizSuccessAudioClip;
            TargetManager.타깃메니저.복제컨텐츠_바로보이기(QuizSelect.GetComponent<QuizQuizInfo>().show3DModelNum);
            AnimationManager.애니메이션.애니메이션01_재생();
        }
        else
        {
            QuizSound.m_QuizAudioSource.clip = QuizSound.m_QuizFailAudioClip;
        }
        QuizSound.m_QuizAudioSource.Play();
    }

    /// <summary>
    /// quizquiz 3d로 설정하는 경우 box콜라이더 변경 및 크기 변경
    /// </summary>
    public void QuizSelect3DSetting()
    {
        int choiceCount;
        int choiceIndex;

        int idx = (int)QuizQuizManager.getInstance.m_QuizNum - 1;

        try
        {
            choiceCount = 0;
            choiceIndex = 0;

            choiceCount = GetChildChoiceCount(idx);

            if (choiceCount > 0)
            {
                QuizList[idx].quiz_ChoiceList = new GameObject[choiceCount];

                foreach (Transform trans in QuizList[idx].quiz_ChoiceObj.transform)
                {
                    QuizList[idx].quiz_ChoiceList[choiceIndex] = trans.gameObject;

                    BoxCollider addBox = QuizList[idx].quiz_ChoiceList[choiceIndex].AddComponent<BoxCollider>();
                    addBox.size = new Vector3(boxSize, boxSize, boxSize);

                    choiceIndex = choiceIndex + 1;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    public void QuizBackBtnClick()
    {
        if(QuizSelect.activeSelf)
        {
            QuizSelect.SetActive(false);
            SketchBookUI.getInstance.quizOtherUI.SetActive(false);
            MainUI.메인.인식글자UI.SetActive(true);
            
            if(MainUI.메인.navigationUI != null)
            {
                Debug.Log("들어옴");
                MainUI.메인.navigationUI.SetActive(true);
            }
        }
        else
        {
            QuizSelect.SetActive(true);
            RotateUI.회전.회전UI_숨기기();
            TargetManager.타깃메니저.HideAllModelingContents();
            MainUI.메인.애니메이션동작_UI숨기기();
        }
    }
}
