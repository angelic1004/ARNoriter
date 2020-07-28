using UnityEngine;
using System.Collections;
using System;

public class AutoLocalize : MonoBehaviour
{
    private string savedLocalize;                        // 체크한 언어 이름 저장

    public bool setEnglish;                              // 영어로 기본언어 설정

    void Awake()
    {
        CheckSavedData();
    }

    /// <summary>
    /// 디폴트 언어를 셋팅합니다.
    /// </summary>
    private void SetDefaultLocalize()
    {
        // 시스템언어가 LocalizeValue에 존재한다면
        if (LocalizeValue.GetLocalizeType(Application.systemLanguage) != null && setEnglish == false)
        {
            LocalizeText.CurrentLanguage = Application.systemLanguage;
            SaveLocalizeSetting();
        }
        else
        {
            // 없을시 영어를 기본으로 설정
            LocalizeText.CurrentLanguage = SystemLanguage.English;
        }
    }

    /// <summary>
    /// 저장된 데이터 확인
    /// </summary>
    private void CheckSavedData()
    {
        // 저장된 언어가 있는지 체크
        bool isSavedData = CheckLocalizeSavedData();

        if (isSavedData)
        {
            // 저장된 언어
            SystemLanguage lang = (SystemLanguage)Enum.Parse(typeof(SystemLanguage), savedLocalize);

            // 저장된 언어로 앱 언어 변경
            if (lang != LocalizeText.CurrentLanguage)
            {
                LocalizeText.CurrentLanguage = lang;
            }
        }
        else
        {
            // 저장된 언어가 없을시 디폴트 언어 설정
            SetDefaultLocalize();
        }
    }

    /// <summary>
    /// 로컬라이즈 저장했는지 확인
    /// </summary>
    /// <returns>저장데이터 있으면 true 반환</returns>
    private bool CheckLocalizeSavedData()
    {
        savedLocalize = PlayerPrefs.GetString("DefaultLocalize");

        if (!string.IsNullOrEmpty(savedLocalize))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 로컬라이즈 셋팅 저장
    /// </summary>
    private void SaveLocalizeSetting()
    {
        savedLocalize = LocalizeText.CurrentLanguage.ToString();
        PlayerPrefs.SetString("DefaultLocalize", savedLocalize);
        PlayerPrefs.Save();
    }
}