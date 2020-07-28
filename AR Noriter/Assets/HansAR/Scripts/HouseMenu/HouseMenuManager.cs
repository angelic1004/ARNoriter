using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class HouseMenuManager : BaseMainUI
{

    /// <summary>
    /// 다운로드 매니저 
    /// </summary>
    public DownloadManager downloadManager;

    /// <summary>
    /// QR INFO 
    /// </summary>
    public QRCodeInfo m_QRCodeInfo;

    /// <summary>
    /// passtargetInfo
    /// </summary>
    public PassTargetInfo m_PassTargetInfo;

    /// <summary>
    /// loadingPopup
    /// </summary>
    public GameObject loadingPopup;

    /// <summary>
    /// 연결 메뉴 종류(카테고리2,3)
    /// </summary>
    public enum MenuState
    {
        language,
        observe,
        sketch,
        paint,
        video,
        puzzle,
        driveedu,
        watchcarbattle,
        word,
        matching,
        maze,
        postcard,
        kartGame,
        fishing,
        
        // 알파벳에서 사용하는 용도
        observe_animal,
        observe_object1,
        observe_object2,
        observe_vegetable
    }

    /// <summary>
    /// 셋팅모드 메뉴 종류(카테고리1)
    /// </summary>
    public enum SettingType
    {
        none,
        language,
        exit,
    }

    /// <summary>
    /// 셋팅모드 연결 메뉴 종류(카테고리2,3)
    /// </summary>
    public enum SettingBtnType
    {
        none,
        korean,
        english,
        indonesian,
        exit
    }

    /// <summary>
    /// circle ui 부분
    /// </summary>
    [Serializable]
    public class HouseUIInfo
    {
        /// <summary>
        /// 클릭 obj
        /// </summary>
        [HideInInspector]
        public GameObject clickObj;

        /// <summary>
        /// 클릭 obj의 인덱스
        /// </summary>
        [HideInInspector]
        public int clickObjIndex;

        public GameObject clickPopUpObj;

        public GameObject clickPopUpBg;

        public GameObject clickPopUpInBg;

        public GameObject cancleBtn;

        public GameObject circleUi;

        public UILabel nowMainCartegory;
    }


    /// <summary>
    /// 버튼에 따른 카테고리 정보
    /// </summary>
    [Serializable]
    public class CategoryInfo
    {
        /// <summary>
        /// 카테고리 1 설정
        /// </summary>
        public GlobalDataManager.CategoryType categoryType;


        /// <summary>
        /// 카테고리에 해당하는 버튼(카테고리1 버튼)
        /// </summary>
        public GameObject menuBtn;

        /// <summary>
        /// 카테고리 버튼 bg(카테고리1 다운로드 되지 않았을 경우 보여주기용)
        /// </summary>
        public UISprite menuBtn_back;

        /// <summary>
        /// 카테고리 버튼 밑 부분 카테고리명 labeltext
        /// (LocalizeText.Value 로 가져와서 사용)  
        /// </summary>
        public UILabel menuLabel;

        /// <summary>
        /// menuBtn 및 menuBtn_back에 들어갈 sprite 명
        /// </summary>
        //public string spriteName;

        /// <summary>
        /// menuLabel에 들어갈 내용
        /// </summary>
        //public string labelName;

        /// <summary>
        /// 하위 카테고리 정보(카테고리 2,3)
        /// </summary>
        public SceneRootInfo[] subMenuList;

    }


    /// <summary>
    /// 중앙부분에 표시되는 UI
    /// </summary>
    [Serializable]
    public class CenterObjInfo
    {
        /// <summary>
        /// 다운로드시 중앙부분 progressbar
        /// </summary>
        public UIProgressBar mainProgress;

        /// <summary>
        /// 다운로드시 중앙부분 progressbar foregroundImg
        /// </summary>
        public UISprite mainSprite;

        /// <summary>
        /// 다운로드시 중앙부분 progressbar backroundImg
        /// </summary>
        public UISprite mainSprite_back;

        /// <summary>
        /// 중앙부분 labeltext (다운로드내용 표시, 카테고리명)
        /// </summary>
        public UILabel mainLabel;

        public UIScrollView subMenuScrollView;

        public GameObject[] subMenuBtnList;
    }

    /// <summary>
    /// 중앙부분 표시되는 UI(이미지 라벨)
    /// </summary>
    [SerializeField]
    public CenterObjInfo centerObjSet;

    /// <summary>
    /// 하위 카테고리 정보(카테고리 2,3)
    /// </summary>
    [Serializable]
    public class SceneRootInfo
    {
        public MenuState subMenuState;
       // public UILabel label;
        public string spriteName;
        public string sceneName;
        public string bundleName;
    }

    /// <summary>
    /// 셋팅모드의 경우 하위 카테고리 정보(카테고리 2,3)
    /// </summary>
    [Serializable]
    public class SettingSubInfo
    {
        public SettingBtnType setBtnType;
        public string spriteName;
        public string labelName;
    }

    /// <summary>
    /// 중앙부분 circle UI(원형)
    /// </summary>
    [SerializeField]
    public HouseUIInfo houseUISet;

    /// <summary>
    /// 버튼에 따른 카테고리 정보
    /// </summary>
    [SerializeField]
    public CategoryInfo[] menuBtnList;

    /// <summary>
    /// 클릭한 메뉴 버튼의 index(카테고리1)
    /// </summary>
    private int clickObjectIndex;

    /// <summary>
    /// 카테고리1 버튼 클릭시 원뿔형태의 원의 현재 각도
    /// </summary>
    private float firstAngle;

    /// <summary>
    /// 카테고리 2,3 버튼 클릭시 로드할 씬이름
    /// </summary>
    private string loadSceneName;

    private Coroutine mainMenuClickCoroutine;
    private Coroutine subMenuOpenCoroutine;

    private bool backActive = false;

    public static HouseMenuManager instance;

    void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start()
    {
        InitAll();
    }

    public override void OnClickPreDepthButton()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 맨처음 초기화 부분
    /// </summary>
    private void InitAll()
    {
        SetCategoryOnOff();
        MainMenuBtnInit();
        ClickPopUpInit();
    }

    /// <summary>
    /// ui버튼 bg 색상 변경
    /// </summary>
    /// <returns></returns>
    private Color32 UISkinBgSetting()
    {
        Color32 color;

        color = new Color32(255, 255, 255, 100);

        return color;
    }


    /// <summary>
    /// 타겟 컬러로 해당 오브젝트 색상 변경
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="targetColor"></param>
    private void UISkinTween(GameObject obj, Color32 targetColor)
    {
        if (obj.GetComponent<UIButton>() != null)
        {
            obj.GetComponent<UIButton>().tweenTarget = null;
        }

        if (obj.GetComponent<UISprite>() != null)
        {
            TweenManager.tween_Manager.TweenAllDestroy(obj);
            TweenManager.tween_Manager.AddTweenColor(obj, obj.GetComponent<UISprite>().color, targetColor, 0.2f);
            TweenManager.tween_Manager.TweenColor(obj);
        }
        else
        {
            TweenManager.tween_Manager.TweenAllDestroy(obj);
            TweenManager.tween_Manager.AddTweenColor(obj, obj.GetComponent<UIWidget>().color, targetColor, 0.2f);
            TweenManager.tween_Manager.TweenColor(obj);
        }
    }


    /// <summary>
    /// 타겟 컬러로 해당 오브젝트 색상 변경
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="targetColor"></param>
    private void UISkinTween(GameObject obj, Color32 targetColor, float tweenTime)
    {
        if (obj.GetComponent<UIButton>() != null)
        {
            obj.GetComponent<UIButton>().tweenTarget = null;
        }

        if (obj.GetComponent<UISprite>() != null)
        {
            TweenManager.tween_Manager.TweenAllDestroy(obj);
            TweenManager.tween_Manager.AddTweenColor(obj, obj.GetComponent<UISprite>().color, targetColor, tweenTime);
            TweenManager.tween_Manager.TweenColor(obj);
        }
        else
        {
            TweenManager.tween_Manager.TweenAllDestroy(obj);
            TweenManager.tween_Manager.AddTweenColor(obj, obj.GetComponent<UIWidget>().color, targetColor, tweenTime);
            TweenManager.tween_Manager.TweenColor(obj);
        }
    }


    /// <summary>
    /// 다운로드 여부 확인 후 카테고리1 버튼 fillAmount 조정
    /// </summary>
    public void SetCategoryOnOff()
    {
        bool downFolderExist = false;       // 다운로드 폴더가 존재하는지 여부

        Color32 undownloadColor = UISkinBgSetting();

        for (int i = 0; i < menuBtnList.Length; i++)
        {
            downFolderExist = downloadManager.CheckLocalDownFolderExist(menuBtnList[i].categoryType);

            //다운로드 되어있음
            if (downFolderExist)
            {
                menuBtnList[i].menuBtn.GetComponent<UISprite>().fillAmount = 1.0f;

                UISkinTween(menuBtnList[i].menuBtn_back.gameObject, Color.white);
               // UISkinTween(menuBtnList[i].menuBtn.transform.FindChild("Label").gameObject, Color.white);
            }
            //다운로드 되어있지 않음
            else
            {
                menuBtnList[i].menuBtn.GetComponent<UISprite>().fillAmount = 0;

                UISkinTween(menuBtnList[i].menuBtn_back.gameObject, undownloadColor);
               // UISkinTween(menuBtnList[i].menuBtn.transform.FindChild("Label").gameObject, Color.white);
            }
        }
    }

    public override void OnClickCategoryButton(GameObject obj)
    {
        try
        {
            if (obj == null)
            {
                throw new Exception(string.Format("GameObject value is null"));
            }

            //clickPrevention.SetActive(true);
            clickObjectIndex = -1;

            // 클릭한 오브젝트 인덱스 구하기
            for (int idx = 0; idx < menuBtnList.Length; idx++)
            {
                if (obj.name.Equals(menuBtnList[idx].menuBtn.name))
                {
                    clickObjectIndex = idx;
                    break;
                }
            }

            // 이곳에서 QRScene 으로 이동 시킬 것이지 체크
            if (m_QRCodeInfo.usedQRScene)
            {
                if (QRCodeUI.needScanning)
                {
                    if (QRCodeUI.CheckCertResult(clickObjectIndex) == false)
                    {
                        QRCodeUI.backupSceneName = menuBtnList[clickObjectIndex].categoryType.ToString().ToLower();
                        QRCodeUI.메뉴인덱스1단계 = clickObjectIndex;

                        SceneManager.LoadScene(m_QRCodeInfo.nameQRScene);

                        return;
                    }
                }
                else
                {
                    // backupSceneName 값이 없으면 '뒤로가기' 버튼을 눌러 넘어온 것으로 판단하여 다음 스텝을 진행 하지 않음 
                    if (string.IsNullOrEmpty(QRCodeUI.backupSceneName))
                    {
                        return;
                    }
                }
            }
            ApplyNextMotion(clickObjectIndex);
        }
        catch (Exception ex)
        {
            Debug.LogError(string.Format("exception message : {0}", ex.Message));
            throw;
        }
    }


    private void ApplyNextMotion(int clickIndex)
    {
        try
        {
            if (clickIndex == -1)
            {
                throw new Exception(string.Format("clickIndex value is -1"));
            }

            GlobalDataManager.m_SelectedCategoryEnum = menuBtnList[clickIndex].categoryType;
            //GlobalDataManager.m_ResourceFolderEnum = menuBtnList[clickIndex].productType;

            if (downloadManager.ConfrimNetworkConnection())
            {
                if (m_PassTargetInfo.usedPassScene && GlobalDataManager.m_ConfirmCertification == false)
                {
                    // 타겟으로 인증을 하면 에셋번들을 다운로드 할 수 있도록 인증 씬으로 이동 함
                    // 로컬에 선택한 카테고리의 버전 파일이 존재하면 인증 씬으로 이동하지 않고 다음 스텝을 실행
                    if (GlobalDataManager.GetResultFindVersionFile() == 0)
                    {
                        //GlobalDataManager.m_MainMenuScrollValue = categoryOne.transform.localPosition.y;

                        GlobalDataManager.m_SelectedSceneName = m_PassTargetInfo.namePassScene;
                        GlobalDataManager.GlobalLoadScene();

                        return;
                    }
                }

                //프로그래스바 타겟 오브젝트 설정하는부분
                HouseMenuUI.houseInstance.SetProgressbarTarget(centerObjSet.mainProgress.gameObject);
                downloadManager.RequestAssetbundleDownload();
            }
            else
            {
                Debug.Log("downloadManager.ConfirmHasVersionFile() : " + downloadManager.ConfirmHasVersionFile());
                if (downloadManager.ConfirmHasVersionFile())
                {
                    downloadManager.StopDownloadCoroutine();
                    centerObjSet.mainLabel.text = LocalizeText.Value["Net_UpdateFinish"];

                    SetCategoryOnOff();

                    SubMenuOpenCoroutineStart();
                    //슬라이드로 카테고리 이동하는 부분
                    //SlideCategory();
                }
                else
                {
                    Debug.Log("네트워크 불안정 메세지 팝업 부분 넣기");
                    HouseMenuUI.getInstance.OpenPopup();
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(string.Format("exception messgae : {0}", ex.Message));
            throw;
        }
    }


    /// <summary>
    /// 일반모드 카테고리 2,3버튼 이미지 변경(종류에 따라 자동변경)
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="ms"></param>
    private void subMenuBtnSpriteSet(GameObject obj, MenuState ms)
    {
        string setSpriteName = string.Empty;
        string setText = string.Empty;
        LocalizeText setLocalText = null;
        UILabel setLabel = null;

        if (obj.transform.GetChild(0).GetComponent<LocalizeText>() != null)
        {
            setLocalText = obj.transform.GetChild(0).GetComponent<LocalizeText>();
            setLabel = obj.transform.GetChild(0).GetComponent<UILabel>();
        }

        setSpriteName = string.Format("{0}_{1}_btn", GlobalDataManager.m_SelectedCategoryEnum.ToString().ToLower(), ms.ToString().ToLower());

        switch (ms)
        {
            case MenuState.language:
                setText = "Language";
                break;
            
            case MenuState.observe:
                setText = "Observe";
                break;

            case MenuState.sketch:
                setText = "Sketch";
                break;

            case MenuState.paint:
                setText = "Paint";
                break;
                
            case MenuState.video:
                setText = "Video";
                break;

            case MenuState.puzzle:
                setText = "Puzzle";
                break;

            case MenuState.driveedu:
                setText = "RoadDriving";
                break;

            case MenuState.watchcarbattle:
                setText = "BattleGame";
                break;
            case MenuState.word:
                setText = "Word";
                break;

            case MenuState.matching:
                setText = "Matching";
                break;

            case MenuState.maze:
                setText = "Maze";
                break;

            case MenuState.postcard:
                setText = "PostCard";
                break;

            case MenuState.kartGame:
                setText = "Kart";
                break;

            case MenuState.fishing:
                setText = "Fishing";
                break;

            // 알파벳에서 추가되는 관찰하기 항목
            case MenuState.observe_animal:
                setText = "Observe_Animal";
                break;
            case MenuState.observe_object1:
                setText = "Observe_Object1";
                break;
            case MenuState.observe_object2:
                setText = "Observe_Object2";
                break;
            case MenuState.observe_vegetable:
                setText = "Observe_Vegetable";
                break;
            default:
                Debug.Log("sub버튼의 state부분에 이상이 있습니다.");
                return;
        }

        if (obj.GetComponent<UIButton>() != null)
        {
            obj.GetComponent<UIButton>().tweenTarget = null;
            obj.GetComponent<UISprite>().spriteName = setSpriteName;
        }
        else if (obj.GetComponent<UISprite>() != null)
        {
            obj.GetComponent<UISprite>().spriteName = setSpriteName;
        }

        if (setLocalText != null)
        {
            setLocalText.ValueName = setText;
            setLabel.text = LocalizeText.Value[setText];
        }
    }

    /// <summary>
    /// 셋팅모드 카테고리 2,3버튼 이미지 변경(종류에 따라 자동변경)
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="st"></param>
    private void subSettingBtnSpriteSet(GameObject obj, SettingBtnType st)
    {
        string setSpriteName = string.Empty;

        switch (st)
        {
            case SettingBtnType.korean:
                setSpriteName = "icon_download";
                break;

            case SettingBtnType.english:
                setSpriteName = "icon_downloaded";
                break;

            case SettingBtnType.indonesian:
                setSpriteName = "icon_downloading";
                break;

            default:
                Debug.Log("subSetting 버튼의 type부분에 이상이 있습니다.");
                return;
        }

        if (obj.GetComponent<UIButton>() != null)
        {
            obj.GetComponent<UIButton>().tweenTarget = null;
            obj.GetComponent<UISprite>().spriteName = setSpriteName;
        }
        else if (obj.GetComponent<UISprite>() != null)
        {
            obj.GetComponent<UISprite>().spriteName = setSpriteName;
        }

    }

    /// <summary>
    /// 언어설정에 따라 obj의 sprite 변경
    /// </summary>
    /// <param name="obj"></param>
    private void LanguageSpriteChange(GameObject obj)
    {
        switch (LocalizeText.CurrentLanguage)
        {
            case SystemLanguage.Korean:
                subSettingBtnSpriteSet(obj, SettingBtnType.korean);
                break;
            case SystemLanguage.English:
                subSettingBtnSpriteSet(obj, SettingBtnType.english);
                break;
            case SystemLanguage.Indonesian:
                subSettingBtnSpriteSet(obj, SettingBtnType.indonesian);
                break;
            default:
                Debug.Log("설정되지 않은 언어입니다.");
                return;
        }
    }

    /// <summary>
    /// 카테고리1 버튼 클릭시
    /// </summary>
    /// <param name="obj"></param>
    public void MainMenuBtnClick(GameObject obj)
    {
        for(int i=0; i< menuBtnList.Length; i++)
        {
            if(obj == menuBtnList[i].menuBtn)
            {
                GlobalDataManager.m_ResourceFolderEnum = menuBtnList[i].categoryType;
                break;
            }
        }

        MainMenuClickEvent(obj);
    }

    public void CloseBtnClick(GameObject obj)
    {
        if (backActive)
        {
            MainMenuClickCoroutineStop();
            SubMenuOpenCoroutineStop();
            obj.GetComponent<UIButton>().tweenTarget = null;
            TweenManager.tween_Manager.TweenAllDestroy(houseUISet.clickPopUpObj);
            TweenManager.tween_Manager.AddTweenAlpha(houseUISet.clickPopUpObj, 1, 0, 0.2f);
            TweenManager.tween_Manager.TweenAlpha(houseUISet.clickPopUpObj);
            MainMenuColliderEnabled(true);
            backActive = false;
        }
    }

    public void NetWorkCloseBtnClick()
    {
        MainMenuClickCoroutineStop();
        SubMenuOpenCoroutineStop();
        TweenManager.tween_Manager.TweenAllDestroy(houseUISet.clickPopUpObj);
        TweenManager.tween_Manager.AddTweenAlpha(houseUISet.clickPopUpObj, 1, 0, 0.2f);
        TweenManager.tween_Manager.TweenAlpha(houseUISet.clickPopUpObj);
        MainMenuColliderEnabled(true);
        backActive = false;
    }


    /// <summary>
    /// 카테고리1 버튼 셋팅(일반, 셋팅 모드)
    /// </summary>
    private void MainMenuBtnInit()
    {
        for (int i = 0; i < menuBtnList.Length; i++)
        {
            CategruMainBtnSet(i);
        }

        SetCategoryOnOff();
    }

    private void CategruMainBtnSet(int index)
    {
        GameObject btnObj = menuBtnList[index].menuBtn;
        UISprite btnBackSpr = menuBtnList[index].menuBtn_back;
        UILabel btnLabel = menuBtnList[index].menuLabel;

        string sprName = string.Empty;
        string labelText = string.Empty;

        if (btnObj.GetComponent<UIButton>() != null)
        {
            btnObj.GetComponent<UIButton>().tweenTarget = null;
        }
       
        switch (menuBtnList[index].categoryType)
        {
            case GlobalDataManager.CategoryType.Common:
                sprName = "btn_game";
                labelText = "FourD";
                break;

            case GlobalDataManager.CategoryType.Alphabet:
                sprName = "btn_alphabet";
                labelText = "Alphabet";
                break;

            case GlobalDataManager.CategoryType.Animal:
                sprName = "btn_zoo";
                labelText = "Animal";
                break;

            case GlobalDataManager.CategoryType.Bug:
                sprName = "btn_city";
                labelText = "Insect";
                break;

            case GlobalDataManager.CategoryType.City:
                sprName = "btn_city";
                labelText = "City";
                break;

            case GlobalDataManager.CategoryType.Dino:
                sprName = "btn_dino";
                labelText = "Dino";
                break;

            case GlobalDataManager.CategoryType.SeaAnimal:
                sprName = "btn_Sea";
                labelText = "Sea";
                break;

            case GlobalDataManager.CategoryType.Princess:
                sprName = "btn_princess";
                labelText = "Soccer";
                Debug.Log("CategoryType none 입니다.");
                break;

            case GlobalDataManager.CategoryType.None:
                sprName = "btn_game";
                labelText = "Soccer";
                Debug.Log("CategoryType none 입니다.");
                break;

        }

        if(sprName != string.Empty)
        {
            btnObj.GetComponent<UISprite>().spriteName = sprName;
            btnBackSpr.spriteName = sprName;
        }

        if(labelText != string.Empty)
        {
            btnLabel.GetComponent<LocalizeText>().ValueName = labelText;
            btnLabel.text = LocalizeText.Value[labelText];
            btnLabel.gameObject.SetActive(false);
        }

        TweenManager.tween_Manager.TweenAllDestroy(btnObj);
        TweenManager.tween_Manager.AddTweenScale(btnObj, 
                                                 Vector3.zero,
                                                 Vector3.one, 0.5f,
                                                 UITweener.Style.Once,
                                                 TweenManager.tween_Manager.scaleAnimationCurve);
        TweenManager.tween_Manager.TweenScale(btnObj);
    }


    /// <summary>
    /// 일반모드 버튼 클릭할 경우(카테고리1 버튼클릭)
    /// </summary>
    /// <param name="obj"></param>
    private void MainMenuClickEvent(GameObject obj)
    {
        //모든 카테고리1 버튼 collider 비활성화
        MainMenuColliderEnabled(false);

        TweenManager.tween_Manager.TweenAllDestroy(centerObjSet.mainProgress.gameObject);
        centerObjSet.mainProgress.transform.localScale = Vector3.zero;
        centerObjSet.mainProgress.transform.localPosition = menuBtnList[0].menuBtn.transform.localPosition;
        houseUISet.clickPopUpInBg.transform.localScale = Vector3.zero;
        houseUISet.circleUi.GetComponent<UISprite>().alpha = 0;

        for (int i = 0; i < centerObjSet.subMenuBtnList.Length; i++)
        {
            TweenManager.tween_Manager.TweenAllDestroy(centerObjSet.subMenuBtnList[i]);
            centerObjSet.subMenuBtnList[i].SetActive(false);
        }

        for (int i = 0; i < menuBtnList.Length; i++)
        {
            //중앙부분 sprite, label 클릭 obj에 맞게 변경
            if (obj == menuBtnList[i].menuBtn)
            {
                houseUISet.clickObj = obj;
                houseUISet.clickObjIndex = i;

                centerObjSet.mainSprite.spriteName = obj.GetComponent<UISprite>().spriteName;
                centerObjSet.mainSprite.width = obj.GetComponent<UISprite>().width;
                centerObjSet.mainSprite.height = obj.GetComponent<UISprite>().height;

                centerObjSet.mainSprite_back.spriteName = obj.GetComponent<UISprite>().spriteName;
                centerObjSet.mainSprite_back.width = obj.GetComponent<UISprite>().width;
                centerObjSet.mainSprite_back.height = obj.GetComponent<UISprite>().height;

                centerObjSet.mainLabel.text = LocalizeText.Value[menuBtnList[i].menuLabel.GetComponent<LocalizeText>().ValueName]; //menuBtnList[i].labelName;
                houseUISet.nowMainCartegory.text = LocalizeText.Value[menuBtnList[i].menuLabel.GetComponent<LocalizeText>().ValueName];

                centerObjSet.mainProgress.value = menuBtnList[i].menuBtn.GetComponent<UISprite>().fillAmount;
                centerObjSet.mainSprite_back.color = UISkinBgSetting();
            }
        }

        MainMenuClickCoroutineStart();
    }


    /// <summary>
    ///  모든 카테고리1 버튼 collider on/off
    /// </summary>
    /// <param name="state"></param>
    public void MainMenuColliderEnabled(bool state)
    {
        for (int i = 0; i < menuBtnList.Length; i++)
        {
            menuBtnList[i].menuBtn.GetComponent<BoxCollider2D>().enabled = state;
        }
    }

    /// <summary>
    /// 중앙부분 이미지 tweenScale 
    /// </summary>
    private void SelectMainView()
    {
        TweenManager.tween_Manager.TweenAllDestroy(centerObjSet.mainProgress.gameObject);
        TweenManager.tween_Manager.AddTweenScale(centerObjSet.mainProgress.gameObject, Vector3.zero, Vector3.one, 0.3f, UITweener.Style.Once, TweenManager.tween_Manager.scaleAnimationCurve);
        TweenManager.tween_Manager.TweenScale(centerObjSet.mainProgress.gameObject);
    }

    /// <summary>
    /// ProductType 에 따른 로딩이미지 변경(이미지 애니메이션)
    /// </summary>
    /// <param name="state"></param>
    private void InitLoadingPopup(bool state)
    {
        GlobalDataManager.CategoryType prod = GlobalDataManager.m_SelectedCategoryEnum;
        UIAtlas at = loadingPopup.transform.FindChild("Loading Image").GetComponent<UISprite>().atlas;
        UISprite sp = loadingPopup.transform.FindChild("Loading Image").GetComponent<UISprite>();
        UISpriteAnimation spAni = loadingPopup.transform.FindChild("Loading Image").GetComponent<UISpriteAnimation>();

        at.name = "MainMenu";

        
        switch (prod)
        {
            case GlobalDataManager.CategoryType.City:
                sp.spriteName = "noriter_loading_1";
                spAni.namePrefix = "noriter_loading_";
                spAni.framesPerSecond = 8;
                break;
            case GlobalDataManager.CategoryType.Animal:
                sp.spriteName = "noriter_loading_1";
                spAni.namePrefix = "noriter_loading_";
                spAni.framesPerSecond = 8;
                break;
            case GlobalDataManager.CategoryType.SeaAnimal:
                sp.spriteName = "noriter_loading_1";
                spAni.namePrefix = "noriter_loading_";
                spAni.framesPerSecond = 8;
                break;
            case GlobalDataManager.CategoryType.Dino:
                sp.spriteName = "noriter_loading_1";
                spAni.namePrefix = "noriter_loading_";
                spAni.framesPerSecond = 8;
                break;

        }


        loadingPopup.SetActive(state);
    }

    /// <summary>
    /// 로딩 스크린 회전 리셋
    /// </summary>
    private void ResetLoadingScreenOrientation(string loadSceneInfo)
    {
        bool setPortrait = false;

        switch (loadSceneInfo)
        {
            //case "observe":
            //case "language":
            case "conversation":

                setPortrait = true;             // 세로모드
                break;

            default:
                setPortrait = false;            // 가로모드
                break;
        }

        if (!setPortrait)
        {
            // 가로모드 적용
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        }
    }


    /// <summary>
    /// 씬을 로드합니다.
    /// </summary>
    public void LoadScene(GameObject obj)
    {
        //ButtonInfo btnInfo = obj.GetComponent<ButtonInfo>();

        int loadSceneIndex = -1;
        string loadSceneName = string.Empty;

        for (int i = 0; i < menuBtnList[clickObjectIndex].subMenuList.Length; i++)
        {
            if (obj == centerObjSet.subMenuBtnList[i])
            {
                loadSceneIndex  = i;
                loadSceneName   = menuBtnList[clickObjectIndex].subMenuList[i].sceneName;

                GlobalDataManager.m_SelectedSceneName   = string.Format("{0}_{1}", GlobalDataManager.m_SelectedCategoryEnum.ToString().ToLower(), loadSceneName);
                GlobalDataManager.m_AssetBundlePartName = menuBtnList[clickObjectIndex].subMenuList[i].bundleName;

                Debug.Log("GlobalDataManager.m_AssetBundlePartName: " + GlobalDataManager.m_AssetBundlePartName);
                break;
            }
        }


        //SceneManager.LoadScene(GlobalDataManager.m_SelectedSceneName);
        StartCoroutine(OpenLoadScene());

        //loadingPopup.SetActive(true);
        //InitLoadingPopup(true);

        ResetLoadingScreenOrientation(loadSceneName);
    }

    /// <summary>
    /// 설정언어에 따른 현재 언어 부분 text 변경
    /// </summary>
    /// <param name="lan"></param>
    /// <returns></returns>
    private string LanguageLabelSetting(string lan)
    {
        string valueString = string.Empty;

        switch (lan)
        {
            case "Korean":
                valueString = LocalizeText.Value["KOR"];
                break;
            case "English":
                valueString = LocalizeText.Value["ENG"];
                break;
            case "Indonesian":
                valueString = LocalizeText.Value["Indonesian"];
                break;
            default:
                Debug.Log("설정되지 않은 언어입니다.");
                return valueString;
        }

        return valueString;
    }



    /// <summary>
    /// submenu 열기 부분 코루틴 호출
    /// </summary>
    public void MainMenuClickCoroutineStart()
    {
        MainMenuClickCoroutineStop();

        ClickPopUpInit();

        mainMenuClickCoroutine = StartCoroutine(MainMenuBtnClickEvent());
    }

    /// <summary>
    /// submenu 열기 부분 코루틴 중지
    /// </summary>
    private void MainMenuClickCoroutineStop()
    {
        if (mainMenuClickCoroutine != null)
        {
            StopCoroutine(mainMenuClickCoroutine);
            mainMenuClickCoroutine = null;
        }
    }

    /// <summary>
    /// submenu 열기 부분 코루틴 호출
    /// </summary>
    public void SubMenuOpenCoroutineStart()
    {
        SubMenuOpenCoroutineStop();

        subMenuOpenCoroutine = StartCoroutine(SubSettingMenuOpen());
    }

    /// <summary>
    /// submenu 열기 부분 코루틴 중지
    /// </summary>
    private void SubMenuOpenCoroutineStop()
    {
        if (subMenuOpenCoroutine != null)
        {
            StopCoroutine(subMenuOpenCoroutine);
            subMenuOpenCoroutine = null;
        }
    }


    private void ClickPopUpInit()
    {
        houseUISet.clickPopUpObj.SetActive(false);
        houseUISet.clickPopUpBg.GetComponent<UIWidget>().alpha = 0;
        houseUISet.clickPopUpBg.GetComponent<UIButton>().tweenTarget = null;
        centerObjSet.mainProgress.gameObject.transform.localScale = Vector3.zero;

        for(int i =0; i<centerObjSet.subMenuBtnList.Length; i++)
        {
            TweenManager.tween_Manager.TweenAllDestroy(centerObjSet.subMenuBtnList[i]);
            centerObjSet.subMenuBtnList[i].SetActive(false);
            centerObjSet.subMenuBtnList[i].GetComponent<UIWidget>().alpha = 0;
            centerObjSet.subMenuBtnList[i].GetComponent<UIButton>().tweenTarget = null;
        }

    }

    private IEnumerator MainMenuBtnClickEvent()
    {
        float delayTime = 0.2f;

        houseUISet.clickPopUpObj.SetActive(true);

        TweenManager.tween_Manager.TweenAllDestroy(houseUISet.clickPopUpBg);
        TweenManager.tween_Manager.AddTweenAlpha(houseUISet.clickPopUpBg, 0, 0.6f, delayTime);
        TweenManager.tween_Manager.TweenAlpha(houseUISet.clickPopUpBg);
        houseUISet.clickPopUpObj.GetComponent<UIPanel>().alpha = 1;
        //UISkinTween(centerObjSet.mainSprite_back.gameObject, Color.white, 0);

        yield return new WaitForSeconds(0.5f);

        SelectMainView();

        if (houseUISet.clickObj != null)
        {
            OnClickCategoryButton(houseUISet.clickObj);
        }
        else
        {
            Debug.Log("클릭된 버튼이 잘못되었습니다.");
        }
    }

    private IEnumerator SubSettingMenuOpen()
    {
        float delayTime = 0.2f;
        int subNum = 0;

        SetCategoryOnOff();

        yield return new WaitForSeconds(0.5f);

        TweenManager.tween_Manager.TweenAllDestroy(houseUISet.clickPopUpInBg);
        TweenManager.tween_Manager.AddTweenScale(houseUISet.clickPopUpInBg, Vector3.zero, Vector3.one, 0.3f, UITweener.Style.Once, TweenManager.tween_Manager.scaleAnimationCurve);
        TweenManager.tween_Manager.TweenScale(houseUISet.clickPopUpInBg);

        yield return new WaitForSeconds(0.3f);

        TweenManager.tween_Manager.TweenAllDestroy(houseUISet.circleUi);
        TweenManager.tween_Manager.AddTweenAlpha(houseUISet.circleUi, 0, 1, 0.3f, UITweener.Style.Once, TweenManager.tween_Manager.scaleAnimationCurve);
        TweenManager.tween_Manager.TweenAlpha(houseUISet.circleUi);


        yield return new WaitForSeconds(0.3f);

        for (int i = 0; i < menuBtnList.Length; i++)
        {
            //중앙부분 sprite, label 클릭 obj에 맞게 변경
            if (houseUISet.clickObj == menuBtnList[i].menuBtn)
            {
                centerObjSet.mainLabel.text = LocalizeText.Value[menuBtnList[i].menuLabel.GetComponent<LocalizeText>().ValueName];
                houseUISet.nowMainCartegory.text = LocalizeText.Value[menuBtnList[i].menuLabel.GetComponent<LocalizeText>().ValueName];
                break;
            }
        }

        for (int i = 0; i < menuBtnList[houseUISet.clickObjIndex].subMenuList.Length; i++)
        {
            if (string.Equals(menuBtnList[houseUISet.clickObjIndex].subMenuList[i].spriteName, string.Empty))
            {
                subMenuBtnSpriteSet(centerObjSet.subMenuBtnList[i],
                                    menuBtnList[houseUISet.clickObjIndex].subMenuList[i].subMenuState);
            }
            else
            {
                centerObjSet.subMenuBtnList[i].GetComponent<UISprite>().spriteName
                        = menuBtnList[houseUISet.clickObjIndex].subMenuList[i].spriteName;
            }

            centerObjSet.subMenuBtnList[i].SetActive(true);
        }

        centerObjSet.subMenuScrollView.ResetPosition();

        while (true)
        {
            yield return new WaitForSeconds(delayTime * 0.5f);

            if (menuBtnList[houseUISet.clickObjIndex].subMenuList.Length <= subNum)
            {
                backActive = true;

                Debug.Log("서브메뉴 등장 끝");
                yield break;
            }

            TweenManager.tween_Manager.TweenAllDestroy(centerObjSet.subMenuBtnList[subNum]);
            TweenManager.tween_Manager.AddTweenScale(centerObjSet.subMenuBtnList[subNum],
                                                     Vector3.zero,
                                                     Vector3.one,
                                                     delayTime * 0.5f);

            TweenManager.tween_Manager.TweenScale(centerObjSet.subMenuBtnList[subNum]);
            centerObjSet.subMenuBtnList[subNum].GetComponent<UIWidget>().alpha = 1;
            subNum++;
        }
    }

    /// <summary>
    /// 씬로드
    /// </summary>
    /// <returns></returns>
    public IEnumerator OpenLoadScene()
    {
        //SceneManager.LoadSceneAsync(GlobalDataManager.m_SelectedSceneName);
        SceneManager.LoadScene("00. Loading");

        yield return null;
    }

    
}
