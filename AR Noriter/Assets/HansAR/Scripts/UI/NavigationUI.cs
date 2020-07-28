using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

/// <summary>
/// 네비게이션과 바로가기 UI 클래스
/// </summary>
public class NavigationUI : MonoBehaviour
{
    public static NavigationUI Instance;

    #region NaviUI
    //무슨 컨텐츠인지(동물,공룡, ...)
    //public UISprite whatContent;
    ////무슨 모드인지(4D,스케치)
    //public UISprite whatMode;
    ////무슨 기능인지(미니맵,관찰, ...)
    //public UISprite whatActiving;

    #endregion

    #region ShortcutUI
    //네비게이션 UI를 저장
    public GameObject shortcutUI;

    //가로 앵커값 저장
    public GameObject anchorObj_Landscape;

    //세로 앵커값 저장
    public GameObject anchorObj_Portrait;

    public string activeColor;

    //디바이스 회전 상태 저장
    [HideInInspector]
    public ScreenOrientation currentScreen;

    public GameObject[] sceneState;

    [Serializable]
    public class ShortcutBtnInfo
    {
        //바로가기 버튼 오브젝트
        public GameObject shortcutBtn;
        //사용할 씬 모드
        public GlobalDataManager.SceneState usedSceneState;

        //불러올 씬 이름(없을시, LocalizeText에 있는 이름을 불러옵니다)
        public string sceneName;
        //에셋번들 이름(없을시, 디폴드값 예: 4D에 있다면 4D 번들을 사용)
        public string assetBundleName;
        //에셋번들을 사용하지 않는 씬일경우 체크
        public bool notUsingAssetBundle;
    }

    /// <summary>
    /// 하이라키뷰 Navigation UI에 컴포넌트 추가되있음.
    /// </summary>
    [SerializeField]
    public ShortcutBtnInfo[] btnInfo;
    #endregion
    
    void Awake()
    {
        Instance = this;

        //처음 디바이스 회전상태를 저장
        currentScreen = Screen.orientation;
    }

    void Start()
    {
        //바로가기 버튼 설정
        ShortcutInitialize();

        //StartCoroutine(NavigationIconSetting());

        //메뉴 위치 초기 설정
        StartCoroutine(SetUIPosition());
        MainUI.메인.딜레이팝업UI.GetComponent<UIPanel>().depth = 105;
    }

    void Update()
    {
        if (Screen.orientation != currentScreen)
        {
            //디바이스 회전시 트윈,앵커를 초기화 해줌
            currentScreen = Screen.orientation;

            ScreenRotateAnchoring();
        }
    }

    #region 바로가기 클릭 관련 함수
    /// <summary>
    /// 씬 바로가기를 초기화 해줍니다.
    /// </summary>
    public void ShortcutInitialize()
    {
        if(sceneState !=null)
        {
            for(int i=0; i<sceneState.Length; i++)
            {
                if(i==0)
                {
                    sceneState[i].transform.localPosition = new Vector3(sceneState[i].transform.position.x, 150, sceneState[i].transform.position.z);
                }
                else
                {
                    int count = 0;
                    for(int j=0; j< i; j++)
                    {
                        count += sceneState[j].transform.childCount;
                    }

                    sceneState[i].transform.localPosition = new Vector3(sceneState[i].transform.position.x, 150+(count*-100), sceneState[i].transform.position.z);
                }
            }
        }

        //씬 설정값이 있다면
        if (btnInfo != null)
        {
            for (int i = 0; i < btnInfo.Length; i++)
            {
                ////필요한 씬의 이름을 Language에서 지정해 동일한 이름을 주어야함
                //btnInfo[i].shortcutBtn.GetComponent<UILabel>().text = string.Format(LocalizeText.Value[btnInfo[i].shortcutBtn.name]);

                //지정한 이름과, 현재 컨텐츠종류(동물,탈것 등)에 이어붙여줌

                if (btnInfo[i].sceneName != "")
                {
                    btnInfo[i].shortcutBtn.name = string.Format(btnInfo[i].shortcutBtn.transform.parent.GetComponent<NavigationInfo>().categoryType.ToString() + "_" + btnInfo[i].sceneName);
                }
                else
                {
                    btnInfo[i].shortcutBtn.name = string.Format(btnInfo[i].shortcutBtn.transform.parent.GetComponent<NavigationInfo>().categoryType.ToString() + "_" + btnInfo[i].shortcutBtn.GetComponent<LocalizeText>().ValueName);
                }

                //대문자를 전부 소문자로 변환
                btnInfo[i].shortcutBtn.name = btnInfo[i].shortcutBtn.name.ToLower();
                string currentSceneName = SceneManager.GetActiveScene().name.ToLower();

                if (btnInfo[i].shortcutBtn.name == currentSceneName)
                {
                    Color btnColor = new Color();
                    ColorUtility.TryParseHtmlString(activeColor, out btnColor);
                    btnInfo[i].shortcutBtn.transform.FindChild("background").GetComponent<UISprite>().color = btnColor;
                    btnInfo[i].shortcutBtn.GetComponent<BoxCollider2D>().enabled = false;
                }

                //UI오면 LABEL을 버리고 이미지로 변경해야함.
            }
        }
        else
        {
            Debug.LogError("CategoryLabel is Null!");
        }
    }

    /// <summary>
    /// 바로가기 버튼 이벤트
    /// </summary>
    /// <param name="selectObj"></param>
    public void ShortCutBtnClickEvent(GameObject selectObj)
    {
        TargetManager.EnableTracking = false;

        for (int i = 0; i < btnInfo.Length; i++)
        {
            //선택된 버튼을 btnInfo에서 찾음.
            if (selectObj == btnInfo[i].shortcutBtn)
            {
                if (btnInfo[i].notUsingAssetBundle == false)
                {
                    //사용할 에셋번들을 지정하여줌
                    if (selectObj.transform.parent.GetComponent<NavigationInfo>() != null && selectObj.transform.parent.GetComponent<NavigationInfo>().whatScene != NavigationInfo.SceneCategory.None)
                    {
                        //에셋번들 이름을 직정 지정한 경우(Btn Info에서 설정)
                        if (string.IsNullOrEmpty(btnInfo[i].assetBundleName) == false)
                        {
                            GlobalDataManager.m_AssetBundlePartName = btnInfo[i].assetBundleName;
                        }
                        //디폴트 번들 사용(거의 사용안함)
                        else
                        {
                            //상위 오브젝트에서 사용할 에셋번들을 구별하여 줌.
                            switch (selectObj.transform.parent.GetComponent<NavigationInfo>().whatScene)
                            {
                                case NavigationInfo.SceneCategory.FourD:
                                    //간단회화만 번들이 다르므로 구별하여 줌.
                                    if (!selectObj.name.Contains("conversation"))
                                    {
                                        GlobalDataManager.m_AssetBundlePartName = "4d";
                                    }
                                    else
                                    {
                                        GlobalDataManager.m_AssetBundlePartName = "conversation";
                                    }
                                    break;

                                case NavigationInfo.SceneCategory.Sketch:
                                    GlobalDataManager.m_AssetBundlePartName = "sketch";
                                    break;

                                //맵이나 퀴즈퀴즈 등 경우 클릭시 실사가 나오므로 4D로 설정
                                //만약 이 외, 다른 번들이 필요한경우 인포를 늘리거나, 따로 버튼인포를 넣어주는게 좋음.
                                case NavigationInfo.SceneCategory.Other:
                                    GlobalDataManager.m_AssetBundlePartName = "4d";
                                    break;

                            }
                        }
                    }
                }

                //로딩팝업을 먼저 띄운다.
                if (MainUI.메인.딜레이팝업UI != null)
                {
                    TargetManager.EnableTracking = false;
                    MainUI.메인.딜레이팝업UI.SetActive(true);
                }

                GlobalDataManager.m_ResourceFolderEnum = btnInfo[i].shortcutBtn.transform.parent.GetComponent<NavigationInfo>().categoryType;
                GlobalDataManager.m_SelectedCategoryEnum = btnInfo[i].shortcutBtn.transform.parent.GetComponent<NavigationInfo>().categoryType;

                //사용할 씬모드가 NONE 이라면
                if (btnInfo[i].usedSceneState == GlobalDataManager.SceneState.NONE)
                {
                    //씬모드를 NONE으로 변경, 씬을 로드하여줌
                    GlobalDataManager.m_SelectedSceneStateEnum = GlobalDataManager.SceneState.NONE;
                    GlobalDataManager.m_SelectedSceneName = selectObj.name;

                    //SceneManager.LoadScene(selectObj.name);                    
                }
                else
                {
                    //그 외라면 씬모드를 변경해줌.
                    GlobalDataManager.m_SelectedSceneStateEnum = btnInfo[i].usedSceneState;

                    //그 후, 스케치북씬을 실행시켜줌
                    GlobalDataManager.m_SelectedSceneName = string.Format("{0}_sketchbook", GlobalDataManager.m_SelectedCategoryEnum);

                    //SceneManager.LoadScene(String.Format(GlobalDataManager.m_SelectedCategoryEnum + "_sketchbook"));
                }

                SceneManager.LoadScene("00. Loading");
            }
        }
    }
    
    /// <summary>
    /// 바로가기 UI 활성,비활성 매서드
    /// </summary>
    public void ShortCutUIActivate(bool isActive)
    {
        //활성
        if (isActive)
        {
            shortcutUI.GetComponent<TweenAlpha>().PlayForward();
        }
        //비활성
        else
        {
            shortcutUI.GetComponent<TweenAlpha>().PlayReverse();
        }
    }
    
    public void NavigationUICloseEnd()
    {
        if (shortcutUI.GetComponent<UISprite>().alpha == 0)
        {
            this.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 네비게이션UI 위치 조정
    /// </summary>
    public IEnumerator SetUIPosition()
    {

        //너무 빨리 앵커를 끄면 제대로 앵커 위치가 안잡혀 텀을 두고 실행
        yield return new WaitForSeconds(0.1f);

        //디바이스에 맞게 트윈값을 저장
        //ScreenRotateTweenSet();

        //ShortCutUIActivate();
        //shortcutUI.GetComponent<UISprite>().alpha = 0;

        //디바이스에 맞게 앵커값을 재조정
        ScreenRotateAnchoring();

        MainUI.메인.NavigationUIClose();
    }

    /// <summary>
    /// 화면 상태(가로,세로)별 앵커링
    /// </summary>
    public void ScreenRotateAnchoring()
    {

        GameObject AnchoredObj = null;

        Debug.Log("화면 방향 :" + Screen.orientation);

        //네비게이션 UI가 열려있을경우
        //if (shortcutUIIsActive)
        //{
        //디바이스가 가로인지 세로인지에 따라 따라갈 앵커를 지정
        if (Screen.orientation == ScreenOrientation.LandscapeLeft)
        {
            AnchoredObj = anchorObj_Landscape;
        }
        else if (Screen.orientation == ScreenOrientation.Portrait)
        {
            AnchoredObj = anchorObj_Portrait;
        }

        if (AnchoredObj == null)
        {
            Debug.LogError("AnchoredObj is Null. Please Check");
            return;
        }

        //Left
        shortcutUI.GetComponent<UISprite>().leftAnchor.target = AnchoredObj.GetComponent<UISprite>().leftAnchor.target;
        shortcutUI.GetComponent<UISprite>().leftAnchor.relative = AnchoredObj.GetComponent<UISprite>().leftAnchor.relative;
        shortcutUI.GetComponent<UISprite>().leftAnchor.absolute = 0;

        //Right
        shortcutUI.GetComponent<UISprite>().rightAnchor.target = AnchoredObj.GetComponent<UISprite>().rightAnchor.target;
        shortcutUI.GetComponent<UISprite>().rightAnchor.relative = AnchoredObj.GetComponent<UISprite>().rightAnchor.relative;
        shortcutUI.GetComponent<UISprite>().rightAnchor.absolute = 0;

        //Top
        shortcutUI.GetComponent<UISprite>().topAnchor.target =
         AnchoredObj.GetComponent<UISprite>().topAnchor.target;
        shortcutUI.GetComponent<UISprite>().topAnchor.relative = AnchoredObj.GetComponent<UISprite>().topAnchor.relative;
        shortcutUI.GetComponent<UISprite>().topAnchor.absolute = 0;

        //Bottom
        shortcutUI.GetComponent<UISprite>().bottomAnchor.target = AnchoredObj.GetComponent<UISprite>().bottomAnchor.target;
        shortcutUI.GetComponent<UISprite>().bottomAnchor.relative = AnchoredObj.GetComponent<UISprite>().bottomAnchor.relative;
        shortcutUI.GetComponent<UISprite>().bottomAnchor.absolute = 0;

        //앵커 리셋
        shortcutUI.GetComponent<UISprite>().ResetAndUpdateAnchors();
    }
    #endregion



    /// <summary>
    /// 현재 씬이 무슨 씬인지를 표출할 UI를 설정
    /// </summary>
    //private IEnumerator NavigationIconSetting()
    //{
    //    yield return new WaitForEndOfFrame();

    //    TargetManager targetManager = TargetManager.타깃메니저;

    //    //현재 씬의 컨텐츠 종류(동물,탈것)를 얻어 아이콘을 지정
    //    switch (GlobalDataManager.m_SelectedCategoryEnum)
    //    {
    //        case GlobalDataManager.CategoryType.None:
    //            whatContent.spriteName = "crgr00_animal";
    //            break;
    //        case GlobalDataManager.CategoryType.Animal:
    //            whatContent.spriteName = "crgr00_animal";
    //            break;

    //        case GlobalDataManager.CategoryType.Dino:
    //            whatContent.spriteName = "crgr00_dinosaur";
    //            break;

    //        /*
    //        case GlobalDataManager.CategoryType.Princess:
    //            whatContent.spriteName = "crgr00_princess";
    //            break;

    //        case GlobalDataManager.ProductType.Vehicle:
    //            whatContent.spriteName = "crgr00_transportor";
    //            break;

    //        case GlobalDataManager.ProductType.Insect:
    //            whatContent.spriteName = "crgr00_insect";
    //            break;
    //        */
    //        default:
    //            whatContent.spriteName = "";
    //            break;
    //    }

    //    //스케치인지 아닌지 여부에 따라 아이콘 지정
    //    if (targetManager.스케치씬사용)
    //    {
    //        whatMode.spriteName = "crgr01_painting";
    //    }
    //    else
    //    {
    //        whatMode.spriteName = "crgr01_4d";
    //    }

    //    //실행할 기능 아이콘을 지정
    //    //미니맵이 아니면서, 씬모드가 None 인경우
    //    if (targetManager.SceneMode == GlobalDataManager.SceneState.NONE)
    //    {
    //        if (targetManager.공부하기사용)
    //        {
    //            whatActiving.spriteName = "crgr02_study";
    //        }
    //        else if (targetManager.질문답변사용)
    //        {
    //            whatActiving.spriteName = "crgr02_QnA";
    //        }
    //        else
    //        {
    //            if (!targetManager.스케치씬사용)
    //            {
    //                whatActiving.spriteName = "crgr02_observe";
    //            }
    //            else
    //            {
    //                whatActiving.enabled = false;
    //            }
    //        }
    //    }
    //    //씬모드가 퀴즈,스티커 등 인경우
    //    else
    //    {
    //        switch (targetManager.SceneMode)
    //        {
    //            case GlobalDataManager.SceneState.QUIZ_QUIZ:
    //                whatActiving.spriteName = "crgr01_quiz";
    //                break;

    //            case GlobalDataManager.SceneState.PUZZLE:
    //                whatActiving.spriteName = "crgr01_puzzle";
    //                break;

    //            case GlobalDataManager.SceneState.STICKER:
    //                whatActiving.spriteName = "crgr01_sticker";
    //                break;
    //            case GlobalDataManager.SceneState.MAP:
    //                whatActiving.spriteName = "crgr01_review";
    //                break;
    //        }
    //    }
    //}
}
