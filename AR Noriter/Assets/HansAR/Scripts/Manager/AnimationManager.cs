using UnityEngine;
using System.Collections;

public class AnimationManager : MonoBehaviour
{
    public static AnimationManager 애니메이션;

    public bool 동작UI사용여부 = false;

    public bool 한번재생 = false;

    public string 클립저장;

    public Animation 애니;
    public AnimationState 애니상태;
    public AnimationClip[] 애니클립;

    public int saveAniClipNum;

    /// <summary>
    /// 여러개의 오브젝트에 있는 애니메이션을 동시 실행시에 사용
    /// </summary>
    public GameObject[] 다중타겟;

    void Awake()
    {
        애니메이션 = this;
    }

    void Start()
    {

    }

    void FixedUpdate()
    {
        애니메이션_다시재생();

        if (LanguageManager.공부매니저.세이펑사용 && 애니 != null && !애니.isPlaying)
        {
            LanguageManager.공부매니저.공부하기사운드.GetComponent<AudioSource>().Stop();
            애니메이션01_재생();
        }
    }

    /// <summary>
    /// 버튼 클릭시 애니메이션 재생
    /// </summary>
    /// <param name="애니이름번호"></param>
    private void 애니메이션재생(int 애니이름번호)
    {
        int 인덱스 = TargetManager.타깃메니저.복제모델링인덱스;
        GameObject 오브젝트 = TargetManager.타깃메니저.에셋번들복제컨텐츠[인덱스];

        다중타겟 = 오브젝트.GetComponent<ModelInfo>().애니메이션다중타겟;
        애니클립 = 오브젝트.GetComponent<ModelInfo>().애니메이션정보.애니클립;

        // 오브젝트 다수 일 경우
        if (다중타겟.Length > 1)
        {
            for (int i = 0; i < 다중타겟.Length; i++)
            {
                애니 = 다중타겟[i].GetComponent<Animation>();
                애니.Stop();
                애니.wrapMode = WrapMode.Loop;
                애니.clip = 애니클립[i];
                애니.Play();
            }
        }
        // 오브젝트가 한개일 경우
        else
        {
            애니 = 오브젝트.GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animation>();
            if (애니 != null)
            {
                //애니.Stop();
                if (애니클립.Length > 0)
                {
                    MainUI.메인.애니메이션동작_목록보이기(애니클립.Length);
                    애니.wrapMode = WrapMode.Loop;

                    if (애니이름번호 < 애니클립.Length)
                    {
                       // 애니.clip = 애니클립[애니이름번호]; // 기본 0번 애니메이션 재생
                        애니.CrossFadeQueued(애니클립[애니이름번호].name, 1.0f, QueueMode.PlayNow);

                       // 애니.Play();
                    }
                }
                else
                {
                    MainUI.메인.애니동작UI.SetActive(false);
                }
            }
            else
            {
                MainUI.메인.애니동작UI.SetActive(false);
            }
        }


        if (EffectSoundManager.이펙트.이펙트사용)
        {
            if (애니클립.Length > 0)
            {
                //애니메이션이 2개 이상일 경우 
                if (애니클립.Length > 1)
                {
                    if (애니이름번호 == 애니클립.Length - 1)
                    {
                        if (TargetManager.타깃메니저.타깃정보[TargetManager.타깃메니저.타깃정보인덱스].마커타깃오브젝트)
                        {
                            {
                                EffectSoundManager.이펙트.이펙트사운드_중지();
                                EffectSoundManager.이펙트.이펙트사운드_재생(오브젝트);
                            }
                        }
                        else
                        {
                            if (!EffectSoundManager.이펙트.이펙트사운드.GetComponent<AudioSource>().isPlaying)
                            {
                                EffectSoundManager.이펙트.이펙트사운드_재생(오브젝트);
                            }
                        }
                    }
                    else
                    {
                        EffectSoundManager.이펙트.이펙트사운드_중지();
                    }
                }
                // 애니메이션이 1개일 경우
                else
                {
                    if (TargetManager.타깃메니저.타깃정보[TargetManager.타깃메니저.타깃정보인덱스].마커타깃오브젝트)
                    {
                        {
                            EffectSoundManager.이펙트.이펙트사운드_중지();
                            EffectSoundManager.이펙트.이펙트사운드_재생(오브젝트);
                        }
                    }
                    else
                    {
                        if (!EffectSoundManager.이펙트.이펙트사운드.GetComponent<AudioSource>().isPlaying)
                        {
                            EffectSoundManager.이펙트.이펙트사운드_재생(오브젝트);
                        }
                    }
                }
            }
        }

        if (DubbingManager.더빙.알아보기사용여부)
        {
            MainUI.메인.알아보기UI.SetActive(true);
        }
        else
        {
            MainUI.메인.알아보기UI.SetActive(false);
        }
    }

    public void 애니메이션01_재생_슬라이드_탐색()
    {
        애니메이션재생(0);
    }

    public void 애니메이션01_재생()
    {
        애니메이션재생(0);
    }

    public void 애니메이션02_재생()
    {
        애니메이션재생(1);
    }

    public void 애니메이션03_재생()
    {
        애니메이션재생(2);
    }

    public void 애니메이션04_재생()
    {
        애니메이션재생(3);
    }

    /// <summary>
    /// 컨텐츠 터치이벤트 발생시 지정된 애니메이션을 실행시키기 위한 매서드
    /// </summary>
    /// <param name="i"></param>
    public void 지정애니메이션_재생(int i)
    {
        한번재생 = true;
        애니메이션재생(i);
    }

    public void 애니메이션_다시재생()
    {
        //컨텐츠 터치이벤트 발생 후 애니메이션이 다 재생됐을시 클릭전의 애니메이션으로 돌아옵니다.
        if (애니 != null)
        {
            if (한번재생 == true && 클립저장 != null && !애니.isPlaying)
            {
                한번재생 = false;
                for (int i = 0; i <= 애니클립.Length - 1; i++)
                {
                    if (클립저장 == 애니클립[i].name)
                    {
                        애니메이션재생(i);
                    }
                }
            }
        }
    }

    public void SelectedAnimationPlay(int index)
    {
        애니메이션재생(index);
    }
}