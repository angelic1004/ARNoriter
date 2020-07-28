using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using HedgehogTeam.EasyTouch;

public class SideMenuUI : MonoBehaviour
{
    public static SideMenuUI Instance;

    /// <summary>
    /// 사이드 메뉴 상태
    /// </summary>
    public enum SideMenuStatus
    {
        None,
        SideMenuOpened,
        ManualOpened,
        NaviOpened
    }

    /////// UI 관련 변수 ///////
    public GameObject blackBG;
    public GameObject languageDownScroll;
    public GameObject sideMenuList;
    public GameObject langSelectBtn;
    public GameObject soundBtn;
    private List<GameObject> languageObj;
    private List<UISprite> languageSelected;

    public GameObject[] tutorialPages;
    public GameObject manualGrid;
    public GameObject selectedPage;
    private GameObject currentPage;

    public GameObject manualObj;
    public GameObject[] tutorialDotUI;

    public SideMenuStatus sideMenuStatus;

    private bool isActiveRecognitionUI;             // 인식글자UI가 켜져 있었는지 상태 저장

    public bool playingBGM;                         // BGM이 재생중인지 여부
    public GameObject bgmOnOffCircle;
    public GameObject bgmBackground;
    public GameObject soundDownScroll;
    public GameObject soundSelectBtn;

	private bool isHaveTargetManager = false;
    private bool isHaveMainUI = false;

    void Awake()
    {
        Instance = this;

        currentPage = tutorialPages[0];
		
        isHaveTargetManager = IsHaveFindObject<TargetManager>("AR Viewer");
        isHaveMainUI        = IsHaveFindObject<MainUI>("2D UI");

        sideMenuList.GetComponent<UIWidget>().alpha = 0;

        Debug.LogWarning("isHaveTargetManager = " + isHaveTargetManager);
        Debug.LogWarning("isHaveMainUI = " + isHaveMainUI);        
    }

    void Start()
    {
        UiActiveControl(sideMenuList, false);
        UiActiveControl(manualObj, false);

        InitLanguage();
        InitBgmSetting();
    }

    /// <summary>
    /// 언어 초기 설정
    /// </summary>
    private void InitLanguage()
    {
        try
        {
            languageObj = new List<GameObject>();
            languageSelected = new List<UISprite>();

            foreach (Transform child in languageDownScroll.transform)
            {
                //Debug.Log("child : " + child.name);

                languageObj.Add(child.gameObject);
                languageSelected.Add(child.FindChild("Selected").GetComponent<UISprite>());
            }
        }
        catch (Exception ex)
        {
            Debug.Log(string.Format("Error Message : {0}, Function Name : InitLanguage", ex.Message));
        }

        string currentLangName = LocalizeText.CurrentLanguage.ToString();

        ResetLanguageSelectedUI(currentLangName);
    }

    /// <summary>
    /// 초기 BGM 셋팅
    /// </summary>
    private void InitBgmSetting()
    {
        GlobalDataManager.playingBGM = LoadOnBgmStatus();
        playingBGM = GlobalDataManager.playingBGM;
        AudioListener.pause = !playingBGM;

        ChangeOnOffButtonUI(playingBGM);
    }

    /// <summary>
    /// 언어 선택시 표시해주는 UI 업데이트
    /// </summary>
    private void ResetLanguageSelectedUI(string langName)
    {
        try
        {
            for (int i = 0; i < languageObj.Count; i++)
            {
                if (langName == languageObj[i].name)
                {
                    languageSelected[i].color = new Color32(153, 153, 204, 240);         // 연두색
                }
                else
                {
                    languageSelected[i].color = new Color32(255, 183, 183, 100);           // 회색
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log(string.Format("Error Message : {0}, Function Name : ResetLanguageSelectedUI", ex.Message));
        }
    }

    /// <summary>
    /// 언어 설정을 변경합니다.
    /// </summary>
    public void ChangeLanguageSetting(GameObject obj)
    {
        string language = obj.name;

        LocalizeText.CurrentLanguage = (SystemLanguage)Enum.Parse(typeof(SystemLanguage), language);

        ResetLanguageSelectedUI(language);

        // 현재 언어 설정 저장
        string savedLocalize = LocalizeText.CurrentLanguage.ToString();
        PlayerPrefs.SetString("DefaultLocalize", savedLocalize);
        PlayerPrefs.Save();

        if(TargetManager.타깃메니저.observeManager !=null)
        {
            ObserveManager.instance.ExplainSave(TargetManager.타깃메니저.에셋번들복제컨텐츠[TargetManager.타깃메니저.복제모델링인덱스]);
        }

        CloseSideMenu();
    }

    /// <summary>
    /// 사이드 메뉴를 엽니다.
    /// </summary>
    public void OpenSideMenu()
    {
        UiActiveControl(blackBG, true);
        sideMenuStatus = SideMenuStatus.SideMenuOpened;
        OffRecognitionUI();

        DestroyImmediate(sideMenuList.GetComponent<TweenAlpha>());
        TweenManager.tween_Manager.AddTweenAlpha(sideMenuList, sideMenuList.GetComponent<UIWidget>().alpha, 1, 0.3f);
        TweenManager.tween_Manager.TweenAlpha(sideMenuList);

        if (TargetManager.타깃메니저.usedSelfiMode)
        {
            if (TouchEventManager.터치.기준콜라이더 != null)
            {
                TouchEventManager.터치.기준콜라이더.SetActive(false);
            }
        }

        if (TargetManager.타깃메니저.usedFishing)
        {
            FishingGameManager.instance.SideMenuOpenPause();
        }
        // 3D 터치를 막아둠 (SideMenu 조작시 모델링 움직이지 않도록)
        //Prevent3dTouch();

        if (isHaveTargetManager == false)
        {
            return;
        }
       
    }
	private bool IsHaveFindObject<T>(string rootObjName)
    {
        GameObject parentObj    = null;

        parentObj = GameObject.Find(rootObjName);

        if (parentObj == null)
        {
            return false;
        }

        if (parentObj.GetComponent<T>() == null)
        {
            return false;
        }

        return true;
    }
    /// <summary>
    /// 사이드 메뉴를 닫습니다.
    /// </summary>
    public void CloseSideMenu()
    {
       

        switch (sideMenuStatus)
        {
            case SideMenuStatus.SideMenuOpened:

                // 사이드 메뉴 닫기
                sideMenuList.GetComponent<TweenTransform>().PlayReverse();
                languageDownScroll.GetComponent<TweenAlpha>().PlayReverse();
                langSelectBtn.GetComponent<TweenRotation>().PlayReverse();
                soundBtn.GetComponent<TweenTransform>().PlayReverse();
                soundDownScroll.GetComponent<TweenAlpha>().PlayReverse();
                soundSelectBtn.GetComponent<TweenRotation>().PlayReverse();
                
                soundBtn.transform.parent.GetComponent<UIScrollView>().ResetPosition();

                DestroyImmediate(sideMenuList.GetComponent<TweenAlpha>());
                TweenManager.tween_Manager.AddTweenAlpha(sideMenuList, sideMenuList.GetComponent<UIWidget>().alpha, 0, 0.3f);
                TweenManager.tween_Manager.TweenAlpha(sideMenuList);

                break;

            case SideMenuStatus.ManualOpened:

                // 매뉴얼 닫기
                manualObj.GetComponent<TweenAlpha>().PlayReverse();
                
                break;

            case SideMenuStatus.NaviOpened:

                // 네비게이션 메뉴 닫기
                MainUI.메인.NavigationUIClose();

                break;
        }

        UiActiveControl(blackBG, false);

        if (isActiveRecognitionUI)
        {
            MainUI.메인.인식글자UI.SetActive(true);
            isActiveRecognitionUI = false;
        }

        // 3D 터치를 다시 켜줌
        //Resume3dTouch();

        if(TargetManager.타깃메니저.usedKartGame)
        {
            KartGameManager.getInstance.PreventKartMove();
        }

        if (TargetManager.타깃메니저.usedFishing)
        {
            FishingGameManager.instance.SideMenuClosePause();
        }

        sideMenuStatus = SideMenuStatus.None;

        if(TargetManager.타깃메니저.usedSelfiMode)
        {
            if (TouchEventManager.터치.기준콜라이더 != null)
            {
                TouchEventManager.터치.기준콜라이더.SetActive(true);
            }
        }
    }

    // TODO : 레이어 이름을 여러가지 사용하는 경우가 있어서 일단 빼둠
    ///// <summary>
    ///// 3D 터치를 막아둡니다.
    ///// </summary>
    //private void Prevent3dTouch()
    //{
    //    EasyTouch.instance.pickableLayers3D = LayerMask.GetMask("Nothing");
    //}

  //  /// <summary>
  //  /// 3D 터치를 다시 활성화합니다.
  //  /// </summary>
  //  private void Resume3dTouch()
  //  {
		////EasyTouch.instance.pickableLayers3D = LayerMask.GetMask("Contents");
		
  //      if (TargetManager.타깃메니저.UsedMiniMap)
  //      {
  //          EasyTouch.instance.pickableLayers3D = LayerMask.GetMask("Contents", "Minimap");
  //      }
  //      else
  //      {
  //          EasyTouch.instance.pickableLayers3D = LayerMask.GetMask("Contents");
  //      }
  //  }

    public void UiActiveControl(GameObject uiObj, bool ctrBool)
    {
        if (uiObj != null)
        {
            uiObj.SetActive(ctrBool);
        }
        else
        {
            Debug.LogError("Object NULL 확인 바랍니다.");
        }
    }

    /// <summary>
    /// 현재 선택된 매뉴얼 페이지를 체크합니다.
    /// </summary>
    IEnumerator CheckSelectedManualPage()
    {
        selectedPage = tutorialPages[0];

        while (sideMenuStatus == SideMenuStatus.ManualOpened)
        {
            // 선택된 페이지 = 메뉴얼Grid의 중간에 선택된 오브젝트
            selectedPage = manualGrid.GetComponent<UICenterOnChild>().centeredObject;

            manualGrid.GetComponent<UICenterOnChild>().Recenter();

            // 다른 페이지를 선택했을시 리셋
            if (currentPage != selectedPage)
            {
                ResetManualPageTween(selectedPage);
                currentPage = selectedPage;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    /// <summary>
    /// 매뉴얼 페이지 트윈을 리셋합니다.
    /// </summary>
    private void ResetManualPageTween(GameObject selectedPage)
    {
        try
        {
            for (int i = 0; i < tutorialPages.Length; i++)
            {
                if (selectedPage == tutorialPages[i])
                {
                    // 선택된 페이지는 Scale 확대
                    tutorialPages[i].GetComponent<TweenScale>().PlayForward();

                    // Dot UI 색상변경 (붉은색)
                    tutorialDotUI[i].GetComponent<UISprite>().color = new Color32(243, 82, 105, 178);
                }
                else
                {
                    // 선택되지 않은 페이지는 Scale 축소
                    tutorialPages[i].GetComponent<TweenScale>().PlayReverse();

                    // Dot UI 색상변경 (흰색)
                    tutorialDotUI[i].GetComponent<UISprite>().color = new Color32(255, 255, 255, 255);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log(string.Format("Error Message : {0}, Function Name : ResetManualPageTween", ex.Message));
        }
    }

    /// <summary>
    /// 메뉴얼을 엽니다.
    /// </summary>
    public void OpenManual()
    {
        CloseSideMenu();
        
        sideMenuStatus = SideMenuStatus.ManualOpened;

        TutorialUI.instance.TutorialStartClick();
        /*
        ResetManualPageTween(tutorialPages[0]);
        StartCoroutine(OffBoxCollider2D(tutorialPages[0], 0.3f));
        manualGrid.transform.parent.GetComponent<UIScrollView>().ResetPosition();

        manualObj.GetComponent<UIPanel>().alpha = 0;
        UiActiveControl(manualObj, true);
        manualObj.GetComponent<TweenAlpha>().PlayForward();

        StartCoroutine(CheckSelectedManualPage());

        UiActiveControl(blackBG, true);
        OffRecognitionUI();
        */
    }

    private IEnumerator OffBoxCollider2D(GameObject obj, float time)
    {
        obj.GetComponent<BoxCollider2D>().enabled = false;

        yield return new WaitForSeconds(time);

        obj.GetComponent<BoxCollider2D>().enabled = true;
    }

    /// <summary>
    /// 인식글자UI를 끕니다.
    /// </summary>
    public void OffRecognitionUI()
    {
		if (isHaveMainUI == false)
        {
            return;
        }
        if (MainUI.메인.인식글자UI.activeSelf)
        {
            MainUI.메인.인식글자UI.SetActive(false);
            isActiveRecognitionUI = true;
        }
    }

    /// <summary>
    /// 네비게이션UI를 켭니다.
    /// </summary>
    public void OpenNavigationUI()
    {
        CloseSideMenu();        
       
        MainUI.메인.NavigationUIOpen();
        sideMenuStatus = SideMenuStatus.NaviOpened;

        UiActiveControl(blackBG, true);
        OffRecognitionUI();
    }

    /// <summary>
    /// 배경음악을 켜고 끕니다.
    /// </summary>
    public void OnOffBGM()
    {
        if(playingBGM)
        {
            // BGM 끔
            playingBGM = false;

            //BGM 끄기
            AudioListener.pause = true;

            ChangeOnOffButtonUI(playingBGM);

            //// Off 상태로 UI 변경
            //bgmOnOffCircle.transform.localPosition = new Vector3(-78f, 0, 0);
            //bgmBackground.GetComponent<UISprite>().color = new Color32(100, 100, 100, 240);         // 회색
        }
        else
        {
            // BGM 켬
            playingBGM = true;

            //BGM 켜기
            AudioListener.pause = false;

            ChangeOnOffButtonUI(playingBGM);

            //// On 상태로 UI 변경
            //bgmOnOffCircle.transform.localPosition = new Vector3(0, 0, 0);
            //bgmBackground.GetComponent<UISprite>().color = new Color32(80, 210, 100, 240);          // 연두색
        }

        GlobalDataManager.playingBGM = playingBGM;
        SaveOnBgmStatus(playingBGM);
    }

    private void ChangeOnOffButtonUI(bool turnOn)
    {
        Vector3 btnPos = Vector3.zero;
        Color32 btnColor = new Color32(0, 0, 0, 0);

        if (turnOn)
        {
            btnColor = new Color32(80, 210, 100, 240);          // 연두색
        }
        else
        {
            btnPos = new Vector3(-78f, 0, 0);
            btnColor = new Color32(100, 100, 100, 240);        // 회색
        }

        bgmOnOffCircle.transform.localPosition = btnPos;
        bgmBackground.GetComponent<UISprite>().color = btnColor;
    }

    /// <summary>
    /// 종료 팝업을 보입니다. ( [BTN] End의 클릭 이벤트 )
    /// </summary>
    public void ClickEndButton()
    {
        CloseSideMenu();
        UiActiveControl(blackBG, true);

        MainUI.메인.종료팝업_열기();
    }

    /// <summary>
    /// 뒤로가기 팝업을 보입니다. ( [BTN] Back의 클릭 이벤트 )
    /// </summary>
    public void ClickBackButton()
    {
        CloseSideMenu();
        UiActiveControl(blackBG, true);

        if (isHaveMainUI == false) {
            return;
        }

        MainUI.메인.뒤로가기팝업_열기();
    }

    /// <summary>
    /// BlackBG를 끕니다. (외부 호출용)
    /// </summary>
    public void TurnOffBlackBG()
    {
        if (blackBG != null)
        {
            UiActiveControl(blackBG, false);
        }
    }

    /// <summary>
    /// BGM 상태를 저장합니다.
    /// </summary>
    private void SaveOnBgmStatus(bool playingBGM)
    {
        int bgmStatus = -1;

        if (playingBGM)
        {
            bgmStatus = 1;
        }
        else
        {
            bgmStatus = 0;
        }

        PlayerPrefs.SetInt("ON_BGM", bgmStatus);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 저장된 BGM 상태를 로드합니다.
    /// </summary>
    /// <returns>true: BGM ON 상태, false: BGM OFF 상태</returns>
    public bool LoadOnBgmStatus()
    {
        int bgmStatus = -1;
        string keyValue = "ON_BGM";
        bool onBGM = true;

        if (PlayerPrefs.HasKey(keyValue))
        {
            bgmStatus = PlayerPrefs.GetInt("ON_BGM");

            if (bgmStatus == 0)         // 저장된 키값 : BGM OFF
            {
                onBGM = false;
            }
            else if (bgmStatus == 1)    // 저장된 키값 : BGM ON
            {
                onBGM = true;
            }
        }
        else
        {
            // 키 값이 없을때 default : BGM ON
            onBGM = true;
        }

        return onBGM;
    }
}