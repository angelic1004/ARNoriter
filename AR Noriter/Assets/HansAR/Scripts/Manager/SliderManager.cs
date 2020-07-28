using UnityEngine;
using System.Collections;

public class SliderManager : MonoBehaviour
{
    TargetManager 타깃 = null;

    /// <summary>
    /// 슬라이드 모드일경우 자동으로 넘어가는 시간을 설정 합니다.
    /// </summary>
    public float 슬라이드시간 = 20.0f;
    private float 슬라이드시간_보관용 = 0.0f;

    public bool 탐색타이머사용 = false;

    [HideInInspector]
    public int 인덱스 = 0;

    void Awake()
    {
        슬라이드시간_보관용 = 슬라이드시간; // 인스펙터뷰에서 지정한 타이머를 보관합니다.
    }
    
    void Start()
    {
        타깃 = TargetManager.타깃메니저; // 타깃매니저 스크립트를 가져옵니다.
    }

    void FixedUpdate()
    {
        // 자동슬라이드를 쓰고있는지, 모델이 인식상태인지를 확인합니다.
        if (탐색타이머사용 && !TargetManager.trackableStatus)
        {
            // 위의 조건이 맞다면 자동타이머가 돌아갑니다.
            슬라이드시간 -= Time.fixedDeltaTime;

            if (슬라이드시간 <= 0)
            {
                슬라이드모드_오른쪽(); // 타이머시간이 0이하로 떨어지면 자동으로 다음 타겟을 비춥니다.
            }
        }
    }
    
    /// <summary>
    /// 성대혁 : 넘김버튼을 누르거나 자동으로 넘어갈때 컨텐츠를 보여주기 위한 메서드 입니다.
    /// </summary>
    /// <param name="자동넘김방향">true == 왼쪽, false == 오른쪽</param>
    private void 슬라이드모델링_자동넘김(bool 자동넘김방향)
    {
        인덱스 = 0;
        int 인덱스최대값 = 타깃.모델컨텐츠저장.Count;

        for (int i = 0; i < 타깃.모델컨텐츠저장.Count; i++)
        {
            if (타깃.복제모델링인덱스 == 타깃.모델컨텐츠저장[i])
            {
                인덱스 = i;
                break;
            }
        }

        타깃.HideAllModelingContents();

        if (자동넘김방향) // 왼쪽넘김버튼 기능
        {
            if (인덱스 > 0)
            {                
                타깃.슬라이드컨텐츠_보이기(타깃.모델컨텐츠저장[--인덱스]);
            }
            else if (인덱스 <= 0)
            {
                인덱스 = (인덱스최대값 - 1);
                타깃.슬라이드컨텐츠_보이기(타깃.모델컨텐츠저장[인덱스]);
            }
        }
        else // 오른쪽넘김버튼 기능
        {
            if (인덱스 < (인덱스최대값 - 1))
            {
                타깃.슬라이드컨텐츠_보이기(타깃.모델컨텐츠저장[++인덱스]); // 다음 컨텐츠를 증강시킵니다.
            }
            else if (인덱스 >= (인덱스최대값 - 1))
            {
                // 더이상 다음 컨텐츠가 없는경우 제일 처음 컨텐츠를 증강시킵니다.
                인덱스 = 0;
                타깃.슬라이드컨텐츠_보이기(타깃.모델컨텐츠저장[인덱스]);
            }
        }

        DubbingManager.더빙.더빙사운드_중지();
        AnimationManager.애니메이션.애니메이션01_재생_슬라이드_탐색();

        QnA_Manager.질문답변매니저.외부참조초기화();

        // 컨텐츠가 1개일 경우에는 사운드 안나오게 변경
        if (인덱스최대값 != 1)
        {
            LanguageManager.공부매니저.공부하기_정보출력();
        }

        if(TargetManager.타깃메니저.슬라이드초기화사용)
        {
            RotateUI.회전.컨텐츠_회전_초기화();
        }

        if(RacingFourD.instance != null)
        {
            RacingFourD.instance.StopCarMove();
            RacingFourD.instance.RayPenelSetting();
            RacingFourD.instance.RacingFourDInit();
        }
    }

    /// <summary>
    /// 왼쪽버튼을 누를경우의 메서드를 호출하고, 자동타이머의 시간을 초기화합니다.
    /// </summary>
    public void 슬라이드모드_왼쪽()
    {
        슬라이드모델링_자동넘김(true);
        슬라이드시간 = 슬라이드시간_보관용;

        // 스케치씬 모델정보 새로고침
        if (TargetManager.타깃메니저.스케치씬사용)
        {
            ColoringManager.컬러링매니저.슬라이드새로고침(타깃.모델컨텐츠저장[인덱스]);
        }
        else
        {
            //DubbingManager.더빙.더빙사운드_클릭재생();
        }

        if (TargetManager.타깃메니저.usedSelfiMode)
        {
            LetterManager.Instance.ResetLetterSetting();
        }

        if(TargetManager.타깃메니저.observeManager != null)
        {
            ObserveManager.instance.SlideExplainSet();
        }
    }

    /// <summary>
    /// 오른쪽버튼을 누를경우의 메서드를 호출하고, 자동타이머의 시간을 초기화합니다.
    /// </summary>
    public void 슬라이드모드_오른쪽()
    {
        슬라이드모델링_자동넘김(false);
        슬라이드시간 = 슬라이드시간_보관용;

        // 스케치씬 모델정보 새로고침
        if (TargetManager.타깃메니저.스케치씬사용)
        {
            ColoringManager.컬러링매니저.슬라이드새로고침(타깃.모델컨텐츠저장[인덱스]);
        }
        else 
        {
           // DubbingManager.더빙.더빙사운드_클릭재생();
        }

        if (TargetManager.타깃메니저.usedSelfiMode)
        {
            LetterManager.Instance.ResetLetterSetting();
        }

        if (TargetManager.타깃메니저.observeManager != null)
        {
            ObserveManager.instance.SlideExplainSet();
        }
    }
}
