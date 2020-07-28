using UnityEngine;
using System.Collections;

public class QnA_Manager : MonoBehaviour
{
    /// <summary>
    /// 권기영 : 푸비와 부 프리팹입니다.
    /// </summary>
    public GameObject 푸비부;

    /// <summary>
    /// 권기영 : 푸비 애니메이션입니다.
    /// </summary>
    public Animation 푸비애니메이션;

    /// <summary>
    /// 권기영 : 부 애니메이션입니다.
    /// </summary>
    public Animation 부애니메이션;

    /// <summary>
    /// 증강된 프리팹에 들어있는 속성값을 받기위한 변수입니다.
    /// </summary>
    ModelInfo.질문답변속성값 질문답변속성;

    /// <summary>
    /// 사운드를 출력할 오브젝트를 에디터에서 지정해줍니다.
    /// </summary>
    public AudioSource 질문사운드;
    public AudioSource 답변사운드;

    /// <summary>
    /// 텍스트를 출력할 오브젝트를 에디터에서 지정해줍니다.
    /// </summary>
    public UILabel 질문답변텍스트;

    public GameObject 연속버튼;
    //public GameObject 질문버튼;
    //public GameObject 답변버튼;

    private bool 질문재생상태 = false;
    private bool 답변재생상태 = false;
    private bool 연속재생상태 = false;

    private GameObject 증강모델링;
    private int 증강인덱스 = -1;

    public float 딜레이시간 = 1.0f;
    private float 딜레이시간_보관용 = 0.0f;

    public static QnA_Manager 질문답변매니저;
    

    public enum 체크상태 : int
    {
        연속체크상태 = 1,
        질문체크상태 = 2,
        답변체크상태 = 3
    }

    private 체크상태 체크상태저장;

    void Awake()
    {
        딜레이시간_보관용 = 딜레이시간;
        딜레이시간 = 0;

        질문답변매니저 = this;
    }

    void Start()
    {

    }

    void FixedUpdate()
    {
        if (TargetManager.타깃메니저.질문답변사용)
        {
            질문재생상태 = 질문사운드.GetComponent<AudioSource>().isPlaying;
            답변재생상태 = 답변사운드.GetComponent<AudioSource>().isPlaying;

            if (!질문재생상태 && !답변재생상태)
            {
                딜레이시간 -= Time.fixedDeltaTime;
                if (체크상태저장 == 체크상태.연속체크상태 && 딜레이시간 <= 0)
                {
                    if (!연속재생상태)
                    {
                        질문체크_클릭();
                    }
                    else
                    {
                        답변체크_클릭();
                    }
                }
                else if (체크상태저장 == 체크상태.질문체크상태 && 딜레이시간 <= 0)
                {
                    질문체크_클릭();
                }
                else if (체크상태저장 == 체크상태.답변체크상태 && 딜레이시간 <= 0)
                {
                    답변체크_클릭();
                }
            }
            //권기영: 질문답변사용시 푸비부의 애니메이션 상태를 확인합니다.
            푸비부애니메이션상태확인();
        }

        
    }

    public void 질문답변UI출력()
    {
        if (TargetManager.타깃메니저.질문답변사용)
        {
            MainUI.메인.질문답변UI.SetActive(true);

            //권기영 : 푸비와 부를 보여줍니다.
            푸비부.SetActive(true);
            연속체크();
        }
    }

    private void 증강모델링체크및초기화()
    {
        증강인덱스 = TargetManager.타깃메니저.복제모델링인덱스;
        증강모델링 = TargetManager.타깃메니저.에셋번들복제컨텐츠[증강인덱스];

        질문답변속성 = 증강모델링.GetComponent<ModelInfo>().질문답변속성;

        // 텍스트 초기화
        질문답변텍스트.text = string.Empty;

        // 재생되고 있는 사운드 중지
        질문사운드.GetComponent<AudioSource>().Stop();
        답변사운드.GetComponent<AudioSource>().Stop();

        질문사운드.GetComponent<AudioSource>().clip = null;
        답변사운드.GetComponent<AudioSource>().clip = null;
    }

    private void 질문체크_클릭()
    {
        if (TargetManager.타깃메니저.질문답변사용)
        {
            딜레이시간 = 딜레이시간_보관용;

            질문답변텍스트.text = 질문답변속성.질문_텍스트;

            질문사운드.GetComponent<AudioSource>().clip = 질문답변속성.질문;
            질문사운드.GetComponent<AudioSource>().Play();

            연속재생상태 = true;

            //권기영 : 푸비가 질문하므로 푸비의 리액션 애니메이션을 실행합니다.
            리액션실행(푸비애니메이션);
        }
    }

    private void 답변체크_클릭()
    {
        if (TargetManager.타깃메니저.질문답변사용)
        {
            딜레이시간 = 딜레이시간_보관용;

            질문답변텍스트.text = 질문답변속성.답변_텍스트;

            답변사운드.GetComponent<AudioSource>().clip = 질문답변속성.답변;
            답변사운드.GetComponent<AudioSource>().Play();

            연속재생상태 = false;

            //권기영 : 부가 답변하므로 부의 리액션 애니메이션을 실행합니다.
            리액션실행(부애니메이션);
        }
    }

    public void 연속체크()
    {
        연속버튼.GetComponent<UIToggle>().value = true;

        증강모델링체크및초기화();
        체크상태저장 = 체크상태.연속체크상태;
        딜레이시간 = 0;
        연속재생상태 = false;
    }

    public void 질문체크()
    {
        증강모델링체크및초기화();
        체크상태저장 = 체크상태.질문체크상태;
        딜레이시간 = 0;
        부애니메이션.wrapMode = WrapMode.Loop;
        부애니메이션.Play("idle");

    }

    public void 답변체크()
    {
        증강모델링체크및초기화();
        체크상태저장 = 체크상태.답변체크상태;
        딜레이시간 = 0;
        푸비애니메이션.wrapMode = WrapMode.Loop;
        푸비애니메이션.Play("idle");
    }
    
    public void 외부참조초기화()
    {
        
        연속체크();
    }


    /// <summary>
    /// 권기영 : 현재 푸비 또는 부의 애니메이션이 실행중인지 확인합니다.
    ///          실행중이 아니라면 다시 idle로 설정하고 loop 시킵니다.
    /// </summary>
    private void 푸비부애니메이션상태확인()
    {
        //권기영 : 푸비의 현재 플레이 애니메이션이 reaction일때 역시 부의 애니메이션을 idle로 돌려놓습니다.  
        if (!부애니메이션.isPlaying || 푸비애니메이션.IsPlaying("reaction"))
        {
            부애니메이션.wrapMode = WrapMode.Loop;
            부애니메이션.Play("idle");
        }

        if (!푸비애니메이션.isPlaying || 부애니메이션.IsPlaying("reaction"))
        {
            푸비애니메이션.wrapMode = WrapMode.Loop;
            푸비애니메이션.Play("idle");
        }
        

    }

    /// <summary>
    /// 권기영 : 해당하는 캐릭터의 reaction 애니메이션을 한번만 실행합니다.
    /// </summary>
    /// <param name="캐릭터"></param>
    private void 리액션실행(Animation 캐릭터)
    {
        //권기영 : 재인식시 reaction 애니메이션이 다시 처음부터 실행되도록  stop 해줍니다.
        캐릭터.Stop();
        캐릭터.wrapMode = WrapMode.Once;
        캐릭터.Play("reaction");
    }
}