using UnityEngine;
using System.Collections;

public class ControlLangManager : MonoBehaviour
{
    public enum Language
    {
        english,
        korean,
        china,
        japen,
        vietnam,
        indonesia,
        none
    }

    public Language nowLanguage;

    public GameObject languageUi;
    public GameObject langBtn;

    public GameObject langLabelObj;

    private int langIndex = 0;

    private Coroutine LangCor;

    public static ControlLangManager instance;

    void Awake()
    {
        instance = this;
    }
    // Use this for initialization
    void Start()
    {
        LanguageInit();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void LanguageInit()
    {
        langIndex = 0;

        if (LanguageManager.공부매니저.영어버튼숨김)
        {
            LanguageBtnClick();
        }
        else
        {
            langBtn.GetComponent<UISprite>().spriteName = "lang_en";
            nowLanguage = Language.english;
        }

        languageUi.SetActive(false);
        MainUI.메인.공부하기UI = languageUi;
        LanguageManager.공부매니저.컨텐츠이름 = langLabelObj;
    }

    private void LanguageSet(Language setLange)
    {
        if(langBtn.GetComponent<UIButton>() !=null)
        {
            langBtn.GetComponent<UIButton>().tweenTarget = null;
        }

        switch(setLange)
        {
            case Language.english: 
                if(LanguageManager.공부매니저.영어버튼숨김)
                {
                    langIndex++;
                    nowLanguage = (Language)langIndex;
                    LanguageSet(nowLanguage);
                }
                else
                {
                    langBtn.GetComponent<UISprite>().spriteName = "lang_en";
                    LanguageManager.공부매니저.영어출력();
                }
                break;

            case Language.korean:
                if (LanguageManager.공부매니저.한국어버튼숨김)
                {
                    langIndex++;
                    nowLanguage = (Language)langIndex;
                    LanguageSet(nowLanguage);
                }
                else
                {
                    langBtn.GetComponent<UISprite>().spriteName = "lang_kor";
                    LanguageManager.공부매니저.한국어출력();
                }
                break;

            case Language.china:
                if (LanguageManager.공부매니저.중국어버튼숨김)
                {
                    langIndex++;
                    nowLanguage = (Language)langIndex;
                    LanguageSet(nowLanguage);
                }
                else
                {
                    langBtn.GetComponent<UISprite>().spriteName = "lang_ch";
                    LanguageManager.공부매니저.중국어출력();
                }
                break;

            case Language.japen:
                if (LanguageManager.공부매니저.일본어버튼숨김)
                {
                    langIndex++;
                    nowLanguage = (Language)langIndex;
                    LanguageSet(nowLanguage);
                }
                else
                {
                    langBtn.GetComponent<UISprite>().spriteName = "lang_ja";
                    LanguageManager.공부매니저.일본어출력();
                }
                break;

            case Language.vietnam:
                if (LanguageManager.공부매니저.베트남어버튼숨김)
                {
                    langIndex++;
                    nowLanguage = (Language)langIndex;
                    LanguageSet(nowLanguage);
                }
                else
                {
                    langBtn.GetComponent<UISprite>().spriteName = "lang_vi";
                    LanguageManager.공부매니저.베트남어출력();
                }
                break;

            case Language.indonesia:
                if (LanguageManager.공부매니저.인도네시아어버튼숨김)
                {
                    langIndex++;
                    nowLanguage = (Language)langIndex;
                    LanguageSet(nowLanguage);
                }
                else
                {
                    langBtn.GetComponent<UISprite>().spriteName = "lang_in";
                    LanguageManager.공부매니저.인도네시아어출력();
                }
                break;

            case Language.none:
                    langIndex = 0;
                    nowLanguage = (Language)langIndex;
                    LanguageSet(nowLanguage);
                break;
        }
    }

    public void LanguageBtnClick()
    {
        LangBtnCorStart();
    }

    private void LangBtnCorStart()
    {
        LangBtnCorStop();
        LangCor = StartCoroutine(LangTweenCor());
    }

    private void LangBtnCorStop()
    {
        if(LangCor !=null)
        {
            StopCoroutine(LangCor);
            LangCor = null;
        }
    }


    private IEnumerator LangTweenCor()
    {
        float tweenTime = 0.2f;
        langIndex++;
        nowLanguage = (Language)langIndex;

        TweenManager.tween_Manager.TweenAllDestroy(languageUi);

        TweenManager.tween_Manager.AddTweenScale(languageUi,
                                               languageUi.transform.localScale,
                                               new Vector3(0.4f, 0.4f, 0.4f),//Vector3.zero,
                                               tweenTime);
        TweenManager.tween_Manager.TweenScale(languageUi);

        yield return new WaitForSeconds(tweenTime);
        
        LanguageSet(nowLanguage);

        TweenManager.tween_Manager.TweenAllDestroy(languageUi);

        TweenManager.tween_Manager.AddTweenScale(languageUi,
                                                  languageUi.transform.localScale,
                                                  Vector3.one,
                                                  tweenTime,
                                                  UITweener.Style.Once,
                                                  TweenManager.tween_Manager.scaleAnimationCurve);

        TweenManager.tween_Manager.TweenScale(languageUi);

        yield break;
    }

}
