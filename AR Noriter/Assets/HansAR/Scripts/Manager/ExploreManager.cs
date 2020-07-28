using UnityEngine;
using System.Collections;

/// <summary>
/// 탐색 모드 (성대혁)
/// </summary>
public class ExploreManager : MonoBehaviour
{
    TargetManager 타깃 = null;

    /// <summary>
    /// 탐색 모드일경우 자동으로 넘어가는 시간을 설정 합니다.
    /// </summary>
    public float 탐색시간 = 20.0f;
    private float 탐색시간_보관용 = 0.0f;

    public bool 탐색타이머사용 = false;

    public bool 타겟인식여부 = false;

    public static ExploreManager 탐색매니저;

    void Awake()
    {
        탐색매니저 = this;
        탐색시간_보관용 = 탐색시간; //인스펙터뷰에서 지정한 타이머를 보관합니다.
    }

    void Start()
    {
        타깃 = TargetManager.타깃메니저; //타깃매니저 스크립트를 가져옵니다.
    }

    void FixedUpdate()
    {
        // 자동슬라이드를 쓰고있는지, 모델이 인식상태인지를 확인합니다.
        if (탐색타이머사용 && TargetManager.trackableStatus)
        {
            //위의 조건이 맞다면 자동타이머가 돌아갑니다.
            탐색시간 -= Time.fixedDeltaTime;

            if (탐색시간 <= 0)
            {
                탐색모드_오른쪽(); //타이머시간이 0이하로 떨어지면 자동으로 다음 타겟을 비춥니다.
            }
        }
    }

    /// <summary>
    /// 성대혁 : 넘김버튼을 누르거나 자동으로 넘어갈때 컨텐츠를 보여주기 위한 메서드 입니다.
    /// </summary>
    /// <param name="자동넘김방향">true == 왼쪽, false == 오른쪽</param>
    private void 탐색모델링_자동넘김(bool 자동넘김방향)
    {
        #region
        
        int 인덱스 = 타깃.타깃정보인덱스; //인식된 이미지타겟의 번호를 가져옵니다.
        int 인덱스최대값 = 타깃.타깃정보[인덱스].증강될컨텐츠번호.Length; //이미지 타겟에 들어있는 모델개수를 구합니다.
        int 하위인덱스 = 타깃.하위인덱스배열번호; //보여줄 이미지가 들어있는 배열의 번호를 가져옵니다.

        타깃.HideAllModelingContents();

        if (타겟인식여부)
        {
            하위인덱스 = 0;
            타겟인식여부 = false;
        }

        if (자동넘김방향) //왼쪽넘김버튼 기능
        {
            if (하위인덱스 > 0)
            {
                타깃.ShowCopyContents(--하위인덱스); //이전 컨텐츠를 증강시킵니다.
            }
            else if (하위인덱스 <= 0)
            {
                //이전에 더이상 컨텐츠가 없는 경우 제일 마지막 컨텐츠를 증강시킵니다.
                하위인덱스 = (인덱스최대값 - 1);
                타깃.ShowCopyContents(하위인덱스);
            }
        }
        else //오른쪽넘김버튼 기능
        {
            if (하위인덱스 < (인덱스최대값 - 1))
            {
                타깃.ShowCopyContents(++하위인덱스); //다음 컨텐츠를 증강시킵니다.
            }
            else if (하위인덱스 >= (인덱스최대값 - 1))
            {
                //더이상 다음 컨텐츠가 없는경우 제일 처음 컨텐츠를 증강시킵니다.
                하위인덱스 = 0;
                타깃.ShowCopyContents(하위인덱스);
            }
        }

        DubbingManager.더빙.더빙사운드_중지();
        AnimationManager.애니메이션.애니메이션01_재생_슬라이드_탐색();

        //출력한 인덱스값을 저장합니다.
        타깃.복제컨텐츠인덱스_저장(하위인덱스); 

        #endregion
    }

    /// <summary>
    /// 왼쪽버튼을 누를경우의 메서드를 호출하고, 자동타이머의 시간을 초기화합니다.
    /// </summary>
    public void 탐색모드_왼쪽()
    {

        탐색모델링_자동넘김(true);
        탐색시간 = 탐색시간_보관용;

        타깃.슬라이드모델링저장();
        LanguageManager.공부매니저.공부하기_정보출력();

        QnA_Manager.질문답변매니저.외부참조초기화();

        RotateUI.회전.컨텐츠_회전_초기화();
    }

    /// <summary>
    /// 오른쪽버튼을 누를경우의 메서드를 호출하고, 자동타이머의 시간을 초기화합니다.
    /// </summary>
    public void 탐색모드_오른쪽()
    {

        탐색모델링_자동넘김(false);
        탐색시간 = 탐색시간_보관용;

        타깃.슬라이드모델링저장();
        LanguageManager.공부매니저.공부하기_정보출력();

        QnA_Manager.질문답변매니저.외부참조초기화();
        
        RotateUI.회전.컨텐츠_회전_초기화();
    }

    /// <summary>
    /// 탐색모드 중지버튼을 누르면 텍스트메세지를 출력하고, 기능을 중지하거나 실행합니다.
    /// </summary>
    public void 탐색모드_중지()
    {

        // 탐색 기능을 사용 하는 마커라면..
        if (타깃.타깃정보[타깃.타깃정보인덱스].탐색여부)
        {
            if (탐색타이머사용)
            {
                // "자동 탐색모드가 해제 되었습니다."
                ToastMessageUI.토스트.토스트메시지_출력(LocalizeText.Value["AutoSearchOff"]);
            }
            else
            {
                // "자동 탐색모드가 설정 되었습니다."
                ToastMessageUI.토스트.토스트메시지_출력(LocalizeText.Value["AutoSearchOn"]);
            }

            탐색타이머사용 = !탐색타이머사용;
        }
    }
}