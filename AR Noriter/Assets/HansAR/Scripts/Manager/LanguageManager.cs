using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// 성대혁 작성 : 공부하기 씬에서 사용되는 기능을 구현하기 위한 클래스입니다.
/// </summary>
public class LanguageManager : MonoBehaviour
{
    ListenInfo.듣고익히기속성값 듣고익히기속성;

    /// <summary>
    /// 증강된 프리팹에 들어있는 속성값을 받기위한 변수입니다.
    /// </summary>
    ModelInfo.공부하기속성값 공부하기속성;

    /// <summary>
    /// 사운드를 출력할 오브젝트를 에디터에서 지정해줍니다.
    /// </summary>
    public GameObject 공부하기사운드;

    /// <summary>
    /// 텍스트를 출력할 오브젝트를 에디터에서 지정해줍니다.
    /// </summary>
    public GameObject 컨텐츠이름;

    public GameObject 영어버튼;
    public GameObject 한국어버튼;
    public GameObject 중국어버튼;
    public GameObject 일본어버튼;
    public GameObject 베트남어버튼;
    public GameObject 인도네시아어버튼;

    /// <summary>
    /// 출력할 언어를 선택, 저장합니다.
    /// </summary>
    private int 출력인덱스;

    /// <summary>
    /// 세이펑 및 간단회화를 사용할때만 체크 해야됨
    /// </summary>
    public bool 세이펑사용 = false;

    /// <summary>
    /// 언어버튼을 사용하지 않을때를 위한 기능입니다.
    /// </summary>
    public bool 영어버튼숨김 = false;
    public bool 한국어버튼숨김 = false;
    public bool 중국어버튼숨김 = false;
    public bool 일본어버튼숨김 = false;
    public bool 베트남어버튼숨김 = false;
    public bool 인도네시아어버튼숨김 = false;

    public bool 영어스펠링사용 = false;
    private bool 영어스펠링듣기 = false;
    private bool 영어스펠링소유 = false;


    private GameObject clickObj;

    public static LanguageManager 공부매니저;

    public enum 언어출력 : int
    {
        영어 = 1,
        한국어 = 2,
        중국어 = 3,
        일본어 = 4,
        베트남어 = 5,
        인도네시아어
    }

    void Awake()
    {
        공부매니저 = this;
    }


    void Start()
    {
        // 언어버튼중 사용하지 않는 버튼을 비활성화 시킵니다.
        if (영어버튼숨김 && 영어버튼 != null)
        {
            영어버튼.SetActive(false);
        }

        if (한국어버튼숨김 && 영어버튼 != null)
        {
            한국어버튼.SetActive(false);
        }

        if (중국어버튼숨김 && 영어버튼 != null)
        {
            중국어버튼.SetActive(false);
        }

        if (일본어버튼숨김 && 영어버튼 != null)
        {
            일본어버튼.SetActive(false);
        }

        if (베트남어버튼숨김 && 영어버튼 != null)
        {
            베트남어버튼.SetActive(false);
        }

        if (인도네시아어버튼숨김 && 영어버튼 != null)
        {
            인도네시아어버튼.SetActive(false);
        }

        LanguageBtnAlphaInit();
    }

    void FixedUpdate()
    {
        if(영어스펠링사용 && 영어스펠링듣기 && !공부하기사운드.GetComponent<AudioSource>().isPlaying)
        {
            영어출력();
        }
    }

    private void LanguageBtnAlphaInit()
    {
        if(영어버튼 != null)
        {
            영어버튼.GetComponent<UIButton>().tweenTarget = null;
            영어버튼.GetComponent<UIWidget>().alpha = 0.5f;
        }

        if(한국어버튼 != null)
        {
            한국어버튼.GetComponent<UIButton>().tweenTarget = null;
            한국어버튼.GetComponent<UIWidget>().alpha = 0.5f;
        }

        if(중국어버튼 != null)
        {
            중국어버튼.GetComponent<UIButton>().tweenTarget = null;
            중국어버튼.GetComponent<UIWidget>().alpha = 0.5f;
        }

        if(일본어버튼 != null)
        {
            일본어버튼.GetComponent<UIButton>().tweenTarget = null;
            일본어버튼.GetComponent<UIWidget>().alpha = 0.5f;
        }

        if (베트남어버튼 != null)
        {
            베트남어버튼.GetComponent<UIButton>().tweenTarget = null;
            베트남어버튼.GetComponent<UIWidget>().alpha = 0.5f;

        }

        if (인도네시아어버튼 != null)
        {
            인도네시아어버튼.GetComponent<UIButton>().tweenTarget = null;
            인도네시아어버튼.GetComponent<UIWidget>().alpha = 0.5f;
        }

        clickObj = 영어버튼;
    }

    private void LanguageBtnClickAlpha(GameObject obj)
    {
        TweenManager.tween_Manager.TweenAllDestroy(clickObj);

        TweenManager.tween_Manager.AddTweenAlpha(clickObj, clickObj.GetComponent<UIWidget>().alpha, 0.5f, 0.3f);
        TweenManager.tween_Manager.AddTweenScale(clickObj,
                                                 clickObj.transform.localScale,
                                                 Vector3.one,
                                                 0.3f);

        TweenManager.tween_Manager.TweenAlpha(clickObj);
        TweenManager.tween_Manager.TweenScale(clickObj);


        TweenManager.tween_Manager.TweenAllDestroy(obj);

        TweenManager.tween_Manager.AddTweenAlpha(obj, obj.GetComponent<UIWidget>().alpha, 1.0f, 0.3f);
        TweenManager.tween_Manager.AddTweenScale(obj,
                                                 obj.transform.localScale,
                                                 new Vector3(1.2f, 1.2f, 1.2f),
                                                 0.3f,
                                                 UITweener.Style.Once,
                                                 TweenManager.tween_Manager.scaleAnimationCurve);

        TweenManager.tween_Manager.TweenAlpha(obj);
        TweenManager.tween_Manager.TweenScale(obj);

        clickObj = obj;
    }


    /// <summary>
    /// 증강된 컨텐츠가 활성화상태가 되면, 증강된 컨텐츠 내부에
    /// 있는 메서드가 호출되어 속성값을 가져온뒤, 디폴트값인 영어를 호출합니다.
    /// </summary>
    /// <param name="속성값"></param>
    private void 속성값호출(ModelInfo.공부하기속성값 속성값)
    {
        공부하기속성 = 속성값;
        영어출력();
    }

    /// <summary>
    /// 
    /// </summary>
    public void 공부하기_정보출력()
    {
        if (TargetManager.타깃메니저.공부하기사용)
        {
            int 인덱스 = TargetManager.타깃메니저.복제모델링인덱스;
            GameObject 모델링 = TargetManager.타깃메니저.에셋번들복제컨텐츠[인덱스];

            컨텐츠이름.SetActive(true);

            if (모델링.GetComponent<ListenInfo>() != null)
            {
                듣고익히기속성 = 모델링.GetComponent<ListenInfo>().듣고익히기속성;
                영어스펠링소유 = true;
            }
            else
            {
                영어스펠링소유 = false;
            }

            속성값호출(모델링.GetComponent<ModelInfo>().공부하기속성); 
        }
    }

    /// <summary>
    /// 영어 사운드와 텍스트를 출력하기 위한 메서드입니다.
    /// </summary>
    public void 영어출력()
    {
        //영어버튼을 사용하지 않는다면, 바로 한국어로 넘겨서 예외처리를 대신합니다.
        if (영어버튼숨김)
        {
            한국어출력();
        }
        else //영어버튼을 사용함
        {
            // 일단 세이펑은 애니메이션이 1개임 만약 늘어나면.. 흠...
            if (세이펑사용)
            {
                AnimationManager.애니메이션.애니메이션01_재생();
            }

            if (공부하기속성.영어 != null)
            {
                if (영어스펠링사용 && 영어스펠링소유)
                {
                    if (영어스펠링듣기)
                    {
                        //텍스트상자에 영어단어를 보내고 사운드를 사운드소스에 저장하고 플레이합니다.
                        출력인덱스 = Convert.ToInt32(언어출력.영어);

                        //컨텐츠이름.GetComponent<UILabel>().text = 공부하기속성.영어_텍스트;
                        공부하기사운드.GetComponent<AudioSource>().clip = 공부하기속성.영어;
                        공부하기사운드.GetComponent<AudioSource>().Play();
                        영어스펠링듣기 = false;
                    }
                    else
                    {
                        컨텐츠이름.GetComponent<UILabel>().text = 공부하기속성.영어_텍스트;
                        공부하기사운드.GetComponent<AudioSource>().clip = 듣고익히기속성.알파벳사운드;
                        공부하기사운드.GetComponent<AudioSource>().Play();
                        영어스펠링듣기 = true;
                    }
                }
                else
                {
                    컨텐츠이름.GetComponent<UILabel>().text = 공부하기속성.영어_텍스트;
                    공부하기사운드.GetComponent<AudioSource>().clip = 공부하기속성.영어;
                    공부하기사운드.GetComponent<AudioSource>().Play();
                }

            }

            LanguageBtnClickAlpha(영어버튼);
        }
    }

    public void 한국어출력()
    {
        //한국어버튼을 사용하지 않는다면, 바로 중국어로 넘겨서 예외처리를 대신합니다.
        if (한국어버튼숨김)
        {
            중국어출력();
        }
        else //한국어버튼을 사용함
        {
            // 일단 세이펑은 애니메이션이 1개임 만약 늘어나면.. 흠...
            if (세이펑사용)
            {
                AnimationManager.애니메이션.애니메이션01_재생();
            }

            if (공부하기속성.한국어 != null)
            {
                //텍스트상자에 한국어단어를 보내고 사운드를 사운드소스에 저장하고 플레이합니다.
                출력인덱스 = Convert.ToInt32(언어출력.한국어);

                컨텐츠이름.GetComponent<UILabel>().text = 공부하기속성.한국어_텍스트;
                공부하기사운드.GetComponent<AudioSource>().clip = 공부하기속성.한국어;
                공부하기사운드.GetComponent<AudioSource>().Play();
            }

            LanguageBtnClickAlpha(한국어버튼);
        }
    }

    public void 중국어출력()
    {
        //중국어버튼을 사용하지 않는다면, 바로 일본어로 넘겨서 예외처리를 대신합니다.
        if (중국어버튼숨김)
        {
            일본어출력();
        }
        else //중국어 버튼을 사용함
        {
            // 일단 세이펑은 애니메이션이 1개임 만약 늘어나면.. 흠...
            if (세이펑사용)
            {
                AnimationManager.애니메이션.애니메이션01_재생();
            }

            if (공부하기속성.중국어 != null)
            {
                //텍스트상자에 중국어단어를 보내고 사운드를 사운드소스에 저장하고 플레이합니다.
                출력인덱스 = Convert.ToInt32(언어출력.중국어);

                컨텐츠이름.GetComponent<UILabel>().text = 공부하기속성.중국어_텍스트;
                공부하기사운드.GetComponent<AudioSource>().clip = 공부하기속성.중국어;
                공부하기사운드.GetComponent<AudioSource>().Play();
            }

            LanguageBtnClickAlpha(중국어버튼);
        }
    }

    public void 일본어출력()
    {
        //일본어버튼을 사용하지 않는다면, 바로 베트남어로 넘겨서 예외처리를 대신합니다.
        if (일본어버튼숨김)
        {
            베트남어출력();
        }
        else //일본어 버튼을 사용함
        {
            // 일단 세이펑은 애니메이션이 1개임 만약 늘어나면.. 흠...
            if (세이펑사용)
            {
                AnimationManager.애니메이션.애니메이션01_재생();
            }

            if (공부하기속성.일본어 != null)
            {
                //텍스트상자에 일본어어단어를 보내고 사운드를 사운드소스에 저장하고 플레이합니다.
                출력인덱스 = Convert.ToInt32(언어출력.일본어);

                컨텐츠이름.GetComponent<UILabel>().text = 공부하기속성.일본어_텍스트;
                공부하기사운드.GetComponent<AudioSource>().clip = 공부하기속성.일본어;
                공부하기사운드.GetComponent<AudioSource>().Play();
            }

            LanguageBtnClickAlpha(일본어버튼);
        }
    }

    public void 베트남어출력()
    {
        //베트남버튼을 사용하지 않는다면, 바로 영어로 넘겨서 예외처리를 대신합니다.
        if (베트남어버튼숨김)
        {
            영어출력();
        }
        else //베트남어 버튼을 사용함
        {
            // 일단 세이펑은 애니메이션이 1개임 만약 늘어나면.. 흠...
            if (세이펑사용)
            {
                 AnimationManager.애니메이션.애니메이션01_재생();
            }

            if (공부하기속성.베트남어 != null)
            {
                //텍스트상자에 베트남어단어를 보내고 사운드를 사운드소스에 저장하고 플레이합니다.
                출력인덱스 = Convert.ToInt32(언어출력.베트남어);

                컨텐츠이름.GetComponent<UILabel>().text = 공부하기속성.베트남어_텍스트;
                공부하기사운드.GetComponent<AudioSource>().clip = 공부하기속성.베트남어;
                공부하기사운드.GetComponent<AudioSource>().Play();
            }

            LanguageBtnClickAlpha(베트남어버튼);
        }
    }

    public void 인도네시아어출력()
    {
        //인도네시아버튼을 사용하지 않는다면, 바로 영어로 넘겨서 예외처리를 대신합니다.
        if (인도네시아어버튼숨김)
        {
            영어출력();
        }
        else //인도네시아어 버튼을 사용함
        {
            // 일단 세이펑은 애니메이션이 1개임 만약 늘어나면.. 흠...
            if (세이펑사용)
            {
                AnimationManager.애니메이션.애니메이션01_재생();
            }

            if (공부하기속성.인도네시아어 != null)
            {
                //텍스트상자에 인도네시아어단어를 보내고 사운드를 사운드소스에 저장하고 플레이합니다.
                출력인덱스 = Convert.ToInt32(언어출력.인도네시아어);

                컨텐츠이름.GetComponent<UILabel>().text = 공부하기속성.인도네시아어_텍스트;
                공부하기사운드.GetComponent<AudioSource>().clip = 공부하기속성.인도네시아어;
                공부하기사운드.GetComponent<AudioSource>().Play();
            }

            LanguageBtnClickAlpha(인도네시아어버튼);
        }
    }

    public void 영어스펠링듣기초기화()
    {
        영어스펠링듣기 = false;
    }
}