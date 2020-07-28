using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

public class LocalizeSetting : MonoBehaviour
{
    private GameObject localizeUI;
    public GameObject langParent;                       // 언어목록 상위 오브젝트

    private List<UIToggle> languageToggles;             // 언어 Toggle이 셋팅될 리스트

    public UIToggle notShowCheck;                       // 다시보지않기 체크
    string saveLocalize;                                // 체크한 언어이름 저장
    public bool isStartToggle = false;                  // 토글을 선택해서 시작할 경우 체크

    void Awake()
    {
        InitLocalizeUI();
    }

    void Start()
    {
        CheckSavedData();
    }

    /// <summary>
    /// 초기 언어 설정
    /// </summary>
    public void InitLocalize()
    {
        languageToggles = new List<UIToggle>();

        localizeUI.GetComponent<UIPanel>().alpha = 1.0f;

        SetLanguageToggles();
        CheckStartToggle();
    }

    /// <summary>
    /// 저장된 데이터 확인
    /// </summary>
    private void CheckSavedData()
    {
        bool isSavedData = CheckLocalizeSavedData();

        if (isSavedData)
        {
            SkipLocalizeScene();
        }
        else
        {
            InitLocalize();
        }
    }

    /// <summary>
    /// 로컬라이즈 씬을 스킵합니다.
    /// </summary>
    private void SkipLocalizeScene()
    {
        LocalizeText.CurrentLanguage = (SystemLanguage)Enum.Parse(typeof(SystemLanguage), saveLocalize);
        Invoke("NextSceneLoad", 0.0f);
    }

    /// <summary>
    /// 언어토글 셋팅
    /// </summary>
    private void SetLanguageToggles()
    {
        for (int i = 0; i < langParent.transform.childCount; i++)
        {
            languageToggles.Add(langParent.transform.GetChild(i).GetComponent<UIToggle>());
        }
    }

    /// <summary>
    /// 스타트 토글을 사용하는지 확인합니다.
    /// </summary>
    private void CheckStartToggle()
    {
        if (isStartToggle)
        {
            // 시스템언어가 LocalizeValue에 존재한다면
            if (LocalizeValue.GetLocalizeType(Application.systemLanguage) != null)
            {
                string sysLang = Application.systemLanguage.ToString();
                SetLocalize(sysLang, true);
            }
            else
            {
                // 없을시 영어를 기본으로 설정
                SetLocalize("English", false);
            }
        }
    }

    /// <summary>
    /// 로컬라이즈 언어를 설정합니다.
    /// </summary>
    /// <param name="langName">언어 이름</param>
    /// <param name="useSystemLang">시스템 언어 사용 여부</param>
    private void SetLocalize(string langName, bool useSystemLang)
    {
        for (int i = 0; i < languageToggles.Count; i++)
        {
            if (languageToggles[i].name == langName)
            {
                // 시스템 언어 사용 여부
                if (useSystemLang)
                {
                    LocalizeText.CurrentLanguage = Application.systemLanguage;
                }
                else
                {
                    LocalizeText.CurrentLanguage = (SystemLanguage)Enum.Parse(typeof(SystemLanguage), languageToggles[i].name);
                }

                languageToggles[i].value = true;
                break;
            }
        }
    }

    /// <summary>
    /// 선택한 언어로 재설정
    /// </summary>
    public void ResetLocalize()
    {
        for (int i = 0; i < languageToggles.Count; i++)
        {
            if (languageToggles[i].value)
            {
                LocalizeText.CurrentLanguage = (SystemLanguage)Enum.Parse(typeof(SystemLanguage), languageToggles[i].name);
                break;
            }
        }
    }

    /// <summary>
    /// 확인버튼 클릭이벤트
    /// </summary>
    public void ClickOK()
    {
        if (notShowCheck.value)
        {
            saveLocalize = LocalizeText.CurrentLanguage.ToString();
            SaveSetting();
        }
                
        // 씬로드 시작
        Invoke("NextSceneLoad", 0.5f);
    }

    /// <summary>
    /// 다시보지않기 체크 로컬라이즈 저장
    /// </summary>
    private void SaveSetting()
    {
        PlayerPrefs.SetString("LocalizeCountryName", saveLocalize);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 로컬라이즈 저장했는지 확인
    /// </summary>
    /// <returns>저장데이터 있으면 true 반환</returns>
    private bool CheckLocalizeSavedData()
    {
        saveLocalize = PlayerPrefs.GetString("LocalizeCountryName");

        if (!string.IsNullOrEmpty(saveLocalize))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // 씬로드
    private void NextSceneLoad()
    {
        // 다음 씬의 값을 BuildSettings 에서 얻자
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadSceneAsync(nextSceneIndex);
        }
        else
        {
            SceneManager.LoadSceneAsync(0);
        }

        //SceneManager.LoadSceneAsync("00. Splash");
    }

    /// <summary>
    /// Localize UI 초기셋팅
    /// </summary>
    private void InitLocalizeUI()
    {
        localizeUI = langParent.transform.parent.gameObject;
        localizeUI.GetComponent<UIPanel>().alpha = 0;
    }
}