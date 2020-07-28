using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using HedgehogTeam.EasyTouch;

public class CircleMenuManager : BaseMainUI
{
    /// <summary>
    /// 셋팅모드 여부
    /// </summary>
    public bool isSettingMode = false;

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
        runwayreal,
        runwaypaint,
        sketch,
        dancebattle,
        dancedance,
        racinggame,
        trackdrive,
        freekick,
        penaltykick,
        video,
        puzzle,
        driveedu,
        watchcarbattle
        
    }

    /// <summary>
    /// 셋팅모드 메뉴 종류(카테고리1)
    /// </summary>
    public enum SettingType
    {
        none,
        language,
        exit,
        skin
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
        exit,
        blackSkin,
        pinkSkin,
        skyboxSkin
    }

    public enum UIColor
    {
        none,
        skybox,
        black,
        pink
    }

    public UIColor uiColor;

    /// <summary>
    /// circle ui 부분(중앙부분 원형)
    /// </summary>
    [Serializable]
    public class CircleUIInfo
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

        /// <summary>
        /// 클릭시 도는 원뿔형태의 원
        /// </summary>
        public GameObject coneObj;

        /// <summary>
        /// 겉원(도넛모양)
        /// </summary>
        public GameObject bgBorder;

        /// <summary>
        /// 클릭 후 겉의 원형(도넛형태)과 
        /// 배경을 동일한 색상으로 변경해주는 원(도넛형태의 원을 가림)
        /// </summary>
        public UISprite bgShade;

        /// <summary>
        /// 중앙부분 원형 뒤로가기 버튼
        /// </summary>
        public GameObject closeBtn;

        /// <summary>
        /// 전체 배경
        /// </summary>
        public GameObject bg;
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

        /// <summary>
        /// 중앙부분 우측상단부에 있는 뒤로가기 활성화 여부 확인 img 
        /// </summary>
        public GameObject mainBackCheckImg;
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
        /// 셋팅 모드의 경우 해당 위치 카테고리 설정
        /// </summary>
        public SettingType settingType;

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
        public string spriteName;

        /// <summary>
        /// menuLabel에 들어갈 내용
        /// </summary>
        public string labelName;

        /// <summary>
        /// 셋팅모드의 경우 menuBtn 및 menuBtn_back에 들어갈 sprite 명
        /// </summary>
        public string settingSpriteName;

        /// <summary>
        /// 셋팅모드의 경우 menuLabel에 들어갈 내용
        /// </summary>
        public string settingLabel;

        /// <summary>
        /// 하위 카테고리 정보(카테고리 2,3)
        /// </summary>
        public SceneRootInfo[] subMenuList;

        /// <summary>
        /// 셋팅모드의 경우 하위 카테고리 정보(카테고리 2,3)
        /// </summary>
        public SettingSubInfo[] subSettingList;

    }

    /// <summary>
    /// 하위 카테고리 정보(카테고리 2,3)
    /// </summary>
    [Serializable]
    public class SceneRootInfo
    {
        public MenuState subMenuState;
        public string spriteName;
        public string labelName;
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
    /// 일반 <-> 셋팅모드 변경 버튼
    /// </summary>
    public GameObject settingBtn;

    /// <summary>
    /// 중앙부분 circle UI(원형)
    /// </summary>
    [SerializeField]
    public CircleUIInfo circleUISet;

    /// <summary>
    /// 중앙부분 표시되는 UI(이미지 라벨)
    /// </summary>
    [SerializeField]
    public CenterObjInfo centerObjSet;

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

    /// <summary>
    /// 원뿔 회전 코루틴
    /// </summary>
    private Coroutine coneRotCoroutine;

    /// <summary>
    /// 원뿔 원위치 회전 코루틴
    /// </summary>
    private Coroutine coneRotReverseCoroutine;

    /// <summary>
    /// 원뿔 각도, fillamount 값 변경 코루틴(카테고리1 -> 카테고리 2,3) 
    /// </summary>
    private Coroutine subMenuOpenCoroutine;

    /// <summary>
    /// 원뿔 각도, fillamount 값 원위치로 변경 코루틴(카테고리 2,3 -> 뒤로가기) 
    /// </summary>
    private Coroutine subMenuCloseCoroutine;

    /// <summary>
    /// circleui 사용 여부
    /// </summary>
    //public static bool CircleUI = false;

    public static CircleMenuManager instance;

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
        BgShadeInit();
        ConeInit();
        MainViewInit();
        subMenuBtnInit();
        SetCategoryOnOff();
        MainMenuBtnInit();
        UISkinSetting();
    }

    /// <summary>
    /// ui버튼 bg 색상 변경
    /// </summary>
    /// <returns></returns>
    private Color32 UISkinBgSetting()
    {
        Color32 color;

        switch (uiColor)
        {
            case UIColor.black:
                color = new Color32(255, 255, 255, 100);
                //color = new Color32(100, 100, 120, 255);
                break;

            case UIColor.pink:
                color = new Color32(255, 255, 255, 100);
                //color = new Color32(150, 0, 0, 255);
                break;

            case UIColor.skybox:
                color = new Color32(255, 255, 255, 100);
                //color = new Color32(90, 105, 105, 255);
                break;
            default:
                Debug.Log("색상이 없습니다. 기본색상인 black으로 재설정됩니다.");
                uiColor = UIColor.black;
                color = new Color32(255, 255, 255, 100);
                //color = new Color32(100, 100, 120, 255);
                break;
        }

        return color;
    }

    /// <summary>
    /// ui 색상 변경
    /// </summary>
    private void UISkinSetting()
    {
        string setLabel;

        switch(uiColor)
        {
            case UIColor.black:
                UICamera.mainCamera.clearFlags = CameraClearFlags.SolidColor;
                setLabel = "Black";
                centerObjSet.mainBackCheckImg.GetComponent<UISprite>().applyGradient = true;
                //UISkinTween(centerObjSet.mainBackCheckImg, Color.white);
                UISkinTween(circleUISet.coneObj, new Color32(50, 50, 60, 77));
                UISkinTween(circleUISet.bgShade.gameObject, new Color32(30, 31, 37, 255));
                UISkinTween(circleUISet.closeBtn, new Color32(30, 30, 40, 255));
                UISkinTween(circleUISet.bgBorder, new Color32(45, 45, 45, 150));
                UISkinTween(circleUISet.bg, new Color32(30, 30, 40, 255));
                break;

            case UIColor.pink:
                UICamera.mainCamera.clearFlags = CameraClearFlags.SolidColor;
                setLabel = "Pink";
                centerObjSet.mainBackCheckImg.GetComponent<UISprite>().applyGradient = true;
                //UISkinTween(centerObjSet.mainBackCheckImg, Color.white);
                UISkinTween(circleUISet.coneObj, new Color32(240, 90, 90, 77));
                UISkinTween(circleUISet.bgShade.gameObject, new Color32(240, 120, 120, 255));
                UISkinTween(circleUISet.closeBtn, new Color32(240, 120, 120, 255));
                UISkinTween(circleUISet.bgBorder, new Color32(240, 140, 140, 150));
                UISkinTween(circleUISet.bg, new Color32(240, 120, 120, 255));
                break;

            case UIColor.skybox:
                UICamera.mainCamera.clearFlags = CameraClearFlags.Skybox;
                setLabel = "SkyBox";
                centerObjSet.mainBackCheckImg.GetComponent<UISprite>().applyGradient = false;
                //UISkinTween(centerObjSet.mainBackCheckImg, Color.black);
                UISkinTween(circleUISet.coneObj, new Color32(115, 220, 250, 77));
                UISkinTween(circleUISet.bgShade.gameObject, new Color32(50, 210, 250, 255));
                UISkinTween(circleUISet.closeBtn, new Color32(50, 210, 250, 255));
                UISkinTween(circleUISet.bgBorder, new Color32(50, 190, 230, 150));
                UISkinTween(circleUISet.bg, new Color32(0, 0, 0, 0));
                break;

            default:
                Debug.Log("색상이 없습니다. 기본색상인 black으로 재설정됩니다.");
                uiColor = UIColor.black;
                UISkinSetting();
                return;
        }


        for (int i = 0; i < menuBtnList.Length; i++)
        {
            if (menuBtnList[i].settingType == SettingType.skin)
            {
                menuBtnList[i].settingLabel = setLabel;
                break;
            }
        }

        UISkinTween(centerObjSet.mainSprite_back.gameObject, Color.white, 0);
        PopUpUISkinTween();
    }

    /// <summary>
    /// 타겟 컬러로 해당 오브젝트 색상 변경
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="targetColor"></param>
    private void UISkinTween(GameObject obj, Color32 targetColor)
    {
        if(obj.GetComponent<UIButton>()!=null)
        {
            obj.GetComponent<UIButton>().tweenTarget = null;
        }

        if(obj.GetComponent<UISprite>() != null)
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
    /// 팝업 ui들을 현재 선택색상에 맞게 변경
    /// </summary>
    private void PopUpUISkinTween()
    {
        Color32 blackColor = new Color32(30, 30, 40, 255);
        Color32 pinkColor = new Color32(240, 120, 120, 255);
        Color32 skyboxColor = new Color32(50, 210, 250, 255);

        switch (uiColor)
        {
            case UIColor.black:
                UISkinTween(m_ClosePopupObj.transform.FindChild("popupBG").gameObject, blackColor);
                UISkinTween(m_ClosePopupObj.transform.FindChild("exitBtn").gameObject, blackColor);
                UISkinTween(m_ClosePopupObj.transform.FindChild("cancleBtn").gameObject, blackColor);
                UISkinTween(m_ClosePopupObj.transform.FindChild("circle").gameObject, blackColor);

                UISkinTween(CircleMenuUI.circleInstance.m_ErrorPopupObj.transform.FindChild("popupBG").gameObject, blackColor);
                UISkinTween(CircleMenuUI.circleInstance.m_ErrorPopupObj.transform.FindChild("exitBtn").gameObject, blackColor);
                UISkinTween(CircleMenuUI.circleInstance.m_ErrorPopupObj.transform.FindChild("cancleBtn").gameObject, blackColor);
                UISkinTween(CircleMenuUI.circleInstance.m_ErrorPopupObj.transform.FindChild("circle").gameObject, blackColor);

                UISkinTween(CircleMenuUI.circleInstance.m_DataPopupObj.transform.FindChild("popupBG").gameObject, blackColor);
                UISkinTween(CircleMenuUI.circleInstance.m_DataPopupObj.transform.FindChild("downBtn").gameObject, blackColor);
                UISkinTween(CircleMenuUI.circleInstance.m_DataPopupObj.transform.FindChild("cancleBtn").gameObject, blackColor);
                UISkinTween(CircleMenuUI.circleInstance.m_DataPopupObj.transform.FindChild("circle").gameObject, blackColor);

                UISkinTween(CircleMenuUI.circleInstance.m_MessagePopupObj.transform.FindChild("popupBG").gameObject, blackColor);
                UISkinTween(CircleMenuUI.circleInstance.m_MessagePopupObj.transform.FindChild("exitBtn").gameObject, blackColor);
                UISkinTween(CircleMenuUI.circleInstance.m_MessagePopupObj.transform.FindChild("cancleBtn").gameObject, blackColor);
                UISkinTween(CircleMenuUI.circleInstance.m_MessagePopupObj.transform.FindChild("circle").gameObject, blackColor);

                break;

            case UIColor.pink:
                UISkinTween(m_ClosePopupObj.transform.FindChild("popupBG").gameObject, pinkColor);
                UISkinTween(m_ClosePopupObj.transform.FindChild("exitBtn").gameObject, pinkColor);
                UISkinTween(m_ClosePopupObj.transform.FindChild("cancleBtn").gameObject, pinkColor);
                UISkinTween(m_ClosePopupObj.transform.FindChild("circle").gameObject, pinkColor);

                UISkinTween(CircleMenuUI.circleInstance.m_ErrorPopupObj.transform.FindChild("popupBG").gameObject, pinkColor);
                UISkinTween(CircleMenuUI.circleInstance.m_ErrorPopupObj.transform.FindChild("exitBtn").gameObject, pinkColor);
                UISkinTween(CircleMenuUI.circleInstance.m_ErrorPopupObj.transform.FindChild("cancleBtn").gameObject, pinkColor);
                UISkinTween(CircleMenuUI.circleInstance.m_ErrorPopupObj.transform.FindChild("circle").gameObject, pinkColor);

                UISkinTween(CircleMenuUI.circleInstance.m_DataPopupObj.transform.FindChild("popupBG").gameObject, pinkColor);
                UISkinTween(CircleMenuUI.circleInstance.m_DataPopupObj.transform.FindChild("downBtn").gameObject, pinkColor);
                UISkinTween(CircleMenuUI.circleInstance.m_DataPopupObj.transform.FindChild("cancleBtn").gameObject, pinkColor);
                UISkinTween(CircleMenuUI.circleInstance.m_DataPopupObj.transform.FindChild("circle").gameObject, pinkColor);

                UISkinTween(CircleMenuUI.circleInstance.m_MessagePopupObj.transform.FindChild("popupBG").gameObject, pinkColor);
                UISkinTween(CircleMenuUI.circleInstance.m_MessagePopupObj.transform.FindChild("exitBtn").gameObject, pinkColor);
                UISkinTween(CircleMenuUI.circleInstance.m_MessagePopupObj.transform.FindChild("cancleBtn").gameObject, pinkColor);
                UISkinTween(CircleMenuUI.circleInstance.m_MessagePopupObj.transform.FindChild("circle").gameObject, pinkColor);
                break;

            case UIColor.skybox:
                UISkinTween(m_ClosePopupObj.transform.FindChild("popupBG").gameObject, skyboxColor);
                UISkinTween(m_ClosePopupObj.transform.FindChild("exitBtn").gameObject, skyboxColor);
                UISkinTween(m_ClosePopupObj.transform.FindChild("cancleBtn").gameObject, skyboxColor);
                UISkinTween(m_ClosePopupObj.transform.FindChild("circle").gameObject, skyboxColor);

                UISkinTween(CircleMenuUI.circleInstance.m_ErrorPopupObj.transform.FindChild("popupBG").gameObject, skyboxColor);
                UISkinTween(CircleMenuUI.circleInstance.m_ErrorPopupObj.transform.FindChild("exitBtn").gameObject, skyboxColor);
                UISkinTween(CircleMenuUI.circleInstance.m_ErrorPopupObj.transform.FindChild("cancleBtn").gameObject, skyboxColor);
                UISkinTween(CircleMenuUI.circleInstance.m_ErrorPopupObj.transform.FindChild("circle").gameObject, skyboxColor);

                UISkinTween(CircleMenuUI.circleInstance.m_DataPopupObj.transform.FindChild("popupBG").gameObject, skyboxColor);
                UISkinTween(CircleMenuUI.circleInstance.m_DataPopupObj.transform.FindChild("downBtn").gameObject, skyboxColor);
                UISkinTween(CircleMenuUI.circleInstance.m_DataPopupObj.transform.FindChild("cancleBtn").gameObject, skyboxColor);
                UISkinTween(CircleMenuUI.circleInstance.m_DataPopupObj.transform.FindChild("circle").gameObject, skyboxColor);

                UISkinTween(CircleMenuUI.circleInstance.m_MessagePopupObj.transform.FindChild("popupBG").gameObject, skyboxColor);
                UISkinTween(CircleMenuUI.circleInstance.m_MessagePopupObj.transform.FindChild("exitBtn").gameObject, skyboxColor);
                UISkinTween(CircleMenuUI.circleInstance.m_MessagePopupObj.transform.FindChild("cancleBtn").gameObject, skyboxColor);
                UISkinTween(CircleMenuUI.circleInstance.m_MessagePopupObj.transform.FindChild("circle").gameObject, skyboxColor);
                break;
        }
    }

    /// <summary>
    /// 다운로드 여부 확인 후 카테고리1 버튼 fillAmount 조정
    /// </summary>
    public void SetCategoryOnOff()
    {
        bool downFolderExist;       // 다운로드 폴더가 존재하는지 여부

        Color32 undownloadColor = UISkinBgSetting();

        for (int i = 0; i < menuBtnList.Length; i++)
        {
            downFolderExist = downloadManager.CheckLocalDownFolderExist(menuBtnList[i].categoryType);
             
            //다운로드 되어있음
            if (downFolderExist)
            {
                menuBtnList[i].menuBtn.GetComponent<UISprite>().fillAmount = 1.0f;

                UISkinTween(menuBtnList[i].menuBtn_back.gameObject, Color.white);
                UISkinTween(menuBtnList[i].menuBtn.transform.FindChild("Label").gameObject, Color.white);

                //menuBtnList[i].menuBtn.transform.FindChild("Label").GetComponent<UILabel>().color = Color.white;
            }
            //다운로드 되어있지 않음
            else
            {
                menuBtnList[i].menuBtn.GetComponent<UISprite>().fillAmount = 0;

                UISkinTween(menuBtnList[i].menuBtn_back.gameObject, undownloadColor);
                UISkinTween(menuBtnList[i].menuBtn.transform.FindChild("Label").gameObject, Color.white);
                //UISkinTween(menuBtnList[i].menuBtn.transform.FindChild("Label").gameObject, undownloadColor);

                //menuBtnList[i].menuBtn_back.GetComponent<UISprite>().color = undownloadColor;
                // menuBtnList[i].menuBtn.transform.FindChild("Label").GetComponent<UILabel>().color = undownloadColor;
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

            GlobalDataManager.m_SelectedCategoryEnum    = menuBtnList[clickIndex].categoryType;
            GlobalDataManager.m_ResourceFolderEnum      = menuBtnList[clickIndex].categoryType;

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
                CircleMenuUI.circleInstance.SetProgressbarTarget(centerObjSet.mainProgress.gameObject);
                downloadManager.RequestAssetbundleDownload();
            }
            else
            {
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
                   // HMSlideDropDownloaderUI.getInstance.OpenPopup();
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
    /// bgShade부분 초기화 
    /// </summary>
    private void BgShadeInit()
    {
        
        circleUISet.bgShade.fillAmount = 0;
    }

    /// <summary>
    /// coneObj 초기화 fillAmount = 0.1f
    /// </summary>
    private void ConeInit()
    {
        circleUISet.coneObj.GetComponent<UIWidget>().alpha = 0.3f;
        circleUISet.coneObj.GetComponent<UISprite>().fillAmount = 0.1f;
        circleUISet.coneObj.transform.localEulerAngles = new Vector3(0, 0, -circleUISet.coneObj.GetComponent<UISprite>().fillAmount * 180);
    }

    /// <summary>
    /// settingBtn Collider on/off
    /// </summary>
    /// <param name="state"></param>
    public void SettingBtnColliderSet(bool state)
    {
        if (settingBtn.GetComponent<UIWidget>().hasBoxCollider)
        {
            settingBtn.GetComponent<BoxCollider2D>().enabled = state;
        }
        else
        {
            settingBtn.GetComponent<CircleCollider2D>().enabled = state;
        }

    }

    /// <summary>
    /// 일반, 셋팅 모드에 따른 중앙부분 이미지, 라벨 변경
    /// </summary>
    private void MainViewInit()
    {
        Color32 purpleColor = new Color32(180, 85, 255, 255);
        Color32 greenColor = new Color32(120, 220, 200, 255);

        settingBtn.GetComponent<UIButton>().tweenTarget = null;
        TweenManager.tween_Manager.TweenAllDestroy(settingBtn);

        UISkinTween(centerObjSet.mainSprite_back.gameObject, Color.white, 0);

        //일반 모드
        if (!isSettingMode)
        {
            centerObjSet.mainSprite.spriteName = "sub_icon_dancebattle";
            centerObjSet.mainSprite_back.spriteName = "sub_icon_dancebattle";

            centerObjSet.mainLabel.text = "Hans AR";

            TweenManager.tween_Manager.AddTweenColor(settingBtn, settingBtn.GetComponent<UISprite>().color, greenColor, 0.8f);
        }
        //셋팅 모드
        else
        {
            centerObjSet.mainSprite.spriteName = "setting_icon";
            centerObjSet.mainSprite_back.spriteName = "setting_icon";

            centerObjSet.mainLabel.text = "Setting";

            TweenManager.tween_Manager.AddTweenColor(settingBtn, settingBtn.GetComponent<UISprite>().color, purpleColor, 0.8f);
        }

        TweenManager.tween_Manager.TweenColor(settingBtn);

        centerObjSet.mainSprite.width = centerObjSet.mainSprite.height;

        MainBackImgActive(false);
        CloseBtnState(false);
        SelectMainView();

    }

    /// <summary>
    /// 카테고리 2,3 버튼 초기화(모두 active 비활성화)
    /// </summary>
    private void subMenuBtnInit()
    {
        for(int i=0; i< circleUISet.coneObj.transform.childCount;i++)
        {
            circleUISet.coneObj.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 카테고리 2,3 버튼 원뿔형태의 원과 각도반대로 변경(카테고리2,3 버튼 똑바로 보이도록)
    /// </summary>
    /// <param name="index"></param>
    /// <param name="angleZ"></param>
    private void subMenuBtnSet(int index, float angleZ)
    {
        for(int i=0; i<index; i++)
        {
            circleUISet.coneObj.transform.GetChild(i).localEulerAngles = new Vector3(0, 0, -angleZ);
        }
    }

    /// <summary>
    /// 일반모드 카테고리 2,3 버튼 on off
    /// </summary>
    /// <param name="index"></param>
    /// <param name="state"></param>
    private void subMenuBtnView(int index, bool state)
    {
        for(int i=0; i<index; i++)
        {

            TweenManager.tween_Manager.TweenAllDestroy(circleUISet.coneObj.transform.GetChild(i).gameObject);
            //on
            if (state)
            {
                //spritename이 인스펙터창에서 입력되지 않은경우 subMenuState 종류에 따라 sprite 자동 변경
                if (string.Equals(menuBtnList[circleUISet.clickObjIndex].subMenuList[i].spriteName, string.Empty))
                {
                    subMenuBtnSpriteSet(circleUISet.coneObj.transform.GetChild(i).gameObject,
                                        menuBtnList[circleUISet.clickObjIndex].subMenuList[i].subMenuState);
                }

                TweenManager.tween_Manager.AddTweenScale(circleUISet.coneObj.transform.GetChild(i).gameObject,
                                                         Vector3.zero,
                                                         Vector3.one,
                                                         0.2f,
                                                         UITweener.Style.Once,
                                                         TweenManager.tween_Manager.scaleAnimationCurve);
            }
            //off
            else
            {
                TweenManager.tween_Manager.AddTweenScale(circleUISet.coneObj.transform.GetChild(i).gameObject,
                                                         Vector3.one,
                                                         Vector3.zero,
                                                         0.2f,
                                                         UITweener.Style.Once,
                                                         TweenManager.tween_Manager.normalAnimationCurve);
            }
            TweenManager.tween_Manager.TweenScale(circleUISet.coneObj.transform.GetChild(i).gameObject);
            circleUISet.coneObj.transform.GetChild(i).GetComponent<BoxCollider2D>().enabled = state;
        }
    }

    /// <summary>
    /// 셋팅모드 카테고리 2,3 버튼 on off
    /// </summary>
    /// <param name="index"></param>
    /// <param name="state"></param>
    private void SettingSubBtnView(int index, bool state)
    {
        for (int i = 0; i < index; i++)
        {
            //spritename이 인스펙터창에서 입력되지 않은경우 subMenuState 종류에 따라 sprite 자동 변경
            if (string.Equals(menuBtnList[circleUISet.clickObjIndex].subSettingList[i].spriteName, string.Empty))
            {
                subSettingBtnSpriteSet(circleUISet.coneObj.transform.GetChild(i).gameObject,
                                    menuBtnList[circleUISet.clickObjIndex].subSettingList[i].setBtnType);
            }

            TweenManager.tween_Manager.TweenAllDestroy(circleUISet.coneObj.transform.GetChild(i).gameObject);

            if (state)
            {
                TweenManager.tween_Manager.AddTweenScale(circleUISet.coneObj.transform.GetChild(i).gameObject,
                                                         Vector3.zero,
                                                         Vector3.one,
                                                         0.2f,
                                                         UITweener.Style.Once,
                                                         TweenManager.tween_Manager.scaleAnimationCurve);
            }
            else
            {
                TweenManager.tween_Manager.AddTweenScale(circleUISet.coneObj.transform.GetChild(i).gameObject,
                                                         Vector3.one,
                                                         Vector3.zero,
                                                         0.2f,
                                                         UITweener.Style.Once,
                                                         TweenManager.tween_Manager.normalAnimationCurve);
            }
            TweenManager.tween_Manager.TweenScale(circleUISet.coneObj.transform.GetChild(i).gameObject);
            circleUISet.coneObj.transform.GetChild(i).GetComponent<BoxCollider2D>().enabled = state;
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

        switch(ms)
        {
            case MenuState.language:
                setSpriteName = "crgr02_study";
                break;

            case MenuState.observe:
                setSpriteName = "crgr02_observe";
                break;

            case MenuState.runwayreal:
                setSpriteName = "sub_icon_runway";
                break;

            case MenuState.runwaypaint:
                setSpriteName = "sub_icon_coloring";
                break;

            case MenuState.sketch:
                setSpriteName = "crgr01_painting";
                break;

            case MenuState.dancebattle:
                setSpriteName = "sub_icon_dancebattle";
                break;

            case MenuState.dancedance:
                setSpriteName = "sub_icon_dancedance";
                break;

            case MenuState.racinggame:
                setSpriteName = "crgr00_transportor";
                break;

            case MenuState.trackdrive:
                setSpriteName = "sub_icon_3d_racing";
                break;

            case MenuState.freekick:
                setSpriteName = "soccer_free_icon";
                break;

            case MenuState.penaltykick:
                setSpriteName = "soccer_penalty_icon";
                break;

            case MenuState.video:
                setSpriteName = "soccer_video_icon";
                break;

            case MenuState.puzzle:
                setSpriteName = "crgr01_puzzle";
                break;

            case MenuState.driveedu:
                setSpriteName = "crgr00_transportor";
                break;

            case MenuState.watchcarbattle:
                setSpriteName = "sub_icon_3d_drive";
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

            case SettingBtnType.exit:
                setSpriteName = "soccer_penalty_icon";
                break;

            case SettingBtnType.blackSkin:
                setSpriteName = "soccer_penalty_icon";
                break;

            case SettingBtnType.pinkSkin:
                setSpriteName = "icon_downloading";
                break;

            case SettingBtnType.skyboxSkin:
                setSpriteName = "icon_download";
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
        else if(obj.GetComponent<UISprite>() !=null)
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

    private void SkinSpriteChange(GameObject obj)
    {
        switch(uiColor)
        {
            case UIColor.black:
                subSettingBtnSpriteSet(obj, SettingBtnType.blackSkin);
                break;
            case UIColor.pink:
                subSettingBtnSpriteSet(obj, SettingBtnType.pinkSkin);
                break;
            case UIColor.skybox:
                subSettingBtnSpriteSet(obj, SettingBtnType.skyboxSkin);
                break;
            default:
                Debug.Log("색상부분 설정이 올바르지 않습니다 black으로 설정합니다.");
                subSettingBtnSpriteSet(obj, SettingBtnType.blackSkin);
                break;
        }
    }

    /// <summary>
    /// 카테고리1 버튼 클릭시
    /// </summary>
    /// <param name="obj"></param>
    public void MainMenuBtnClick(GameObject obj)
    {
        SettingBtnColliderSet(false);

        //일반모드
        if (!isSettingMode)
        {
            MainMenuClickEvent(obj);
        }
        //셋팅모드
        else
        {
            SettingMainMenuClickEvent(obj);
        }
    }

    /// <summary>
    /// 셋팅모드 전환버튼 클릭시
    /// </summary>
    public void SettingBtnClick()
    {
        isSettingMode = !isSettingMode;

        if (circleUISet.bgShade.fillAmount < 1)
        {
            MainViewInit();
        }

        CloseBtnClick();
        MainMenuBtnInit();
    }

    /// <summary>
    /// 카테고리1 버튼 셋팅(일반, 셋팅 모드)
    /// </summary>
    private void MainMenuBtnInit()
    {
        //셋팅모드
        if(isSettingMode)
        {
            for (int i = 0; i < menuBtnList.Length; i++)
            {
                //셋팅타입이 설정되어있는 버튼만 활성화
                if (menuBtnList[i].settingType != SettingType.none)
                {
                    menuBtnList[i].menuBtn.GetComponent<UISprite>().fillAmount = 1.0f;

                    if (menuBtnList[i].menuBtn.GetComponent<UIButton>() != null)
                    {
                        menuBtnList[i].menuBtn.GetComponent<UIButton>().tweenTarget = null;
                        menuBtnList[i].menuBtn.GetComponent<UISprite>().spriteName = menuBtnList[i].settingSpriteName;
                    }
                    else if(menuBtnList[i].menuBtn.GetComponent<UISprite>() != null)
                    {
                        menuBtnList[i].menuBtn.GetComponent<UISprite>().spriteName = menuBtnList[i].settingSpriteName;
                    }

                    menuBtnList[i].menuBtn_back.GetComponent<UISprite>().spriteName = menuBtnList[i].settingSpriteName;

                    //언어설정인 경우 현재 언어에 해당하는 이미지, 텍스트로 변경
                    if (menuBtnList[i].settingType == SettingType.language)
                    {
                        LanguageSpriteChange(menuBtnList[i].menuBtn);
                        LanguageSpriteChange(menuBtnList[i].menuBtn_back.gameObject);
                        menuBtnList[i].menuLabel.text = LanguageLabelSetting(LocalizeText.CurrentLanguage.ToString());
                    }
                    else if (menuBtnList[i].settingType == SettingType.skin)
                    {
                        SkinSpriteChange(menuBtnList[i].menuBtn);
                        SkinSpriteChange(menuBtnList[i].menuBtn_back.gameObject);
                        menuBtnList[i].menuLabel.text = LocalizeText.Value[menuBtnList[i].settingLabel];
                    }
                    else
                    {
                        menuBtnList[i].menuLabel.text = LocalizeText.Value[menuBtnList[i].settingLabel];
                    }
                    
                    menuBtnList[i].menuBtn.GetComponent<UISprite>().color = Color.white;
                    menuBtnList[i].menuLabel.color = Color.white;

                    TweenManager.tween_Manager.TweenAllDestroy(menuBtnList[i].menuBtn);
                    TweenManager.tween_Manager.AddTweenScale(menuBtnList[i].menuBtn, Vector3.zero, Vector3.one, 0.5f, UITweener.Style.Once, TweenManager.tween_Manager.scaleAnimationCurve);
                    TweenManager.tween_Manager.TweenScale(menuBtnList[i].menuBtn);
                }
                else
                {
                    menuBtnList[i].menuBtn.GetComponent<UISprite>().fillAmount = 0.0f;

                    if (menuBtnList[i].menuBtn.GetComponent<UIButton>() != null)
                    {
                        menuBtnList[i].menuBtn.GetComponent<UIButton>().tweenTarget = null;
                        menuBtnList[i].menuBtn.GetComponent<UISprite>().spriteName = string.Empty;
                    }
                    else if (menuBtnList[i].menuBtn.GetComponent<UISprite>() != null)
                    {
                        menuBtnList[i].menuBtn.GetComponent<UISprite>().spriteName = string.Empty;
                    }
                    
                    menuBtnList[i].menuBtn_back.GetComponent<UISprite>().spriteName = string.Empty;
                    menuBtnList[i].menuLabel.text = string.Empty;
                }
            }
        }
        //일반모드
        else
        {
            for (int i = 0; i < menuBtnList.Length; i++)
            {
                if (menuBtnList[i].menuBtn.GetComponent<UIButton>() != null)
                {
                    menuBtnList[i].menuBtn.GetComponent<UIButton>().tweenTarget = null;
                    menuBtnList[i].menuBtn.GetComponent<UISprite>().spriteName = menuBtnList[i].spriteName;
                }
                else if (menuBtnList[i].menuBtn.GetComponent<UISprite>() != null)
                {
                    menuBtnList[i].menuBtn.GetComponent<UISprite>().spriteName = menuBtnList[i].spriteName;
                }

                menuBtnList[i].menuBtn_back.GetComponent<UISprite>().spriteName = menuBtnList[i].spriteName;
                menuBtnList[i].menuLabel.text = LocalizeText.Value[menuBtnList[i].labelName];

                TweenManager.tween_Manager.TweenAllDestroy(menuBtnList[i].menuBtn);
                TweenManager.tween_Manager.AddTweenScale(menuBtnList[i].menuBtn, Vector3.zero, Vector3.one, 0.5f, UITweener.Style.Once, TweenManager.tween_Manager.scaleAnimationCurve);
                TweenManager.tween_Manager.TweenScale(menuBtnList[i].menuBtn);
            }

            SetCategoryOnOff();
        }
    }

    /// <summary>
    /// 일반모드 버튼 클릭할 경우(카테고리1 버튼클릭)
    /// </summary>
    /// <param name="obj"></param>
    private void MainMenuClickEvent(GameObject obj)
    {
        //모든 카테고리1 버튼 collider 비활성화
        MainMenuColliderEnabled(false);

        for (int i=0; i < menuBtnList.Length; i++)
        {
            //중앙부분 sprite, label 클릭 obj에 맞게 변경
            if(obj == menuBtnList[i].menuBtn)
            {
                circleUISet.clickObj = obj;
                circleUISet.clickObjIndex = i;
                centerObjSet.mainSprite.spriteName = obj.GetComponent<UISprite>().spriteName;
                centerObjSet.mainSprite_back.spriteName = obj.GetComponent<UISprite>().spriteName;
                centerObjSet.mainLabel.text = LocalizeText.Value[menuBtnList[i].labelName]; //menuBtnList[i].labelName;
                centerObjSet.mainProgress.value = menuBtnList[i].menuBtn.GetComponent<UISprite>().fillAmount;

                if (centerObjSet.mainProgress.value == 1)
                {
                    UISkinTween(centerObjSet.mainSprite_back.gameObject, Color.white, 0);
                }
                else
                {
                    UISkinTween(centerObjSet.mainSprite_back.gameObject, UISkinBgSetting(), 0);
                }

                if (i == 4)
                {
                    centerObjSet.mainSprite.width = (int)(centerObjSet.mainSprite.height * 0.75f);
                }
                else
                {
                    centerObjSet.mainSprite.width = centerObjSet.mainSprite.height;
                }
                SelectMainView();
            }
        }

        if (circleUISet.clickObj != null)
        {
            ConeRotCoroutineStart();
        }
        else
        {
            Debug.Log("클릭된 버튼이 잘못되었습니다.");
        }
    }

    /// <summary>
    /// 중앙 label 부분 text변경
    /// </summary>
    private void MainMenuLabelTextChange()
    {
        if (!isSettingMode)
        {
            for (int i = 0; i < menuBtnList.Length; i++)
            {
                if (circleUISet.clickObj == menuBtnList[i].menuBtn)
                {
                    centerObjSet.mainLabel.text = LocalizeText.Value[menuBtnList[i].labelName];
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 뒤로가기 가능 여부 표시 img on/off
    /// </summary>
    /// <param name="active"></param>
    private void MainBackImgActive(bool active)
    {
        if(active)
        {
            TweenManager.tween_Manager.TweenAllDestroy(centerObjSet.mainBackCheckImg);
            TweenManager.tween_Manager.AddTweenScale(centerObjSet.mainBackCheckImg,
                                                     centerObjSet.mainBackCheckImg.transform.localScale, 
                                                     Vector3.one, 
                                                     0.1f, 
                                                     UITweener.Style.Once, 
                                                     TweenManager.tween_Manager.scaleAnimationCurve);
        }
        else
        {
            TweenManager.tween_Manager.TweenAllDestroy(centerObjSet.mainBackCheckImg);
            TweenManager.tween_Manager.AddTweenScale(centerObjSet.mainBackCheckImg,
                                                     centerObjSet.mainBackCheckImg.transform.localScale,
                                                     Vector3.zero,
                                                     0.1f,
                                                     UITweener.Style.Once,
                                                     TweenManager.tween_Manager.normalAnimationCurve);
        }

        TweenManager.tween_Manager.TweenScale(centerObjSet.mainBackCheckImg);
    }

    /// <summary>
    /// 셋팅모드 버튼 클릭할 경우(카테고리1 버튼클릭)
    /// </summary>
    /// <param name="obj"></param>
    private void SettingMainMenuClickEvent(GameObject obj)
    {
        //모든 카테고리1 버튼 collider 비활성화
        MainMenuColliderEnabled(false);

        for (int idx = 0; idx < menuBtnList.Length; idx++)
        {
            if (obj.name.Equals(menuBtnList[idx].menuBtn.name))
            {
                clickObjectIndex = idx;
                break;
            }
        }

        //중앙부분 sprite, label 클릭 obj에 맞게 변경
        for (int i = 0; i < menuBtnList.Length; i++)
        {
            if (obj == menuBtnList[i].menuBtn)
            {
                if (menuBtnList[i].settingType != SettingType.none)
                {
                    circleUISet.clickObj = obj;
                    circleUISet.clickObjIndex = i;
                    centerObjSet.mainSprite.spriteName = menuBtnList[i].settingSpriteName;
                    centerObjSet.mainSprite_back.spriteName = menuBtnList[i].settingSpriteName;

                    if (menuBtnList[i].settingType == SettingType.language)
                    {
                        centerObjSet.mainSprite.spriteName = obj.GetComponent<UISprite>().spriteName;
                        centerObjSet.mainSprite_back.spriteName = obj.GetComponent<UISprite>().spriteName;
                        centerObjSet.mainLabel.text = LanguageLabelSetting(LocalizeText.CurrentLanguage.ToString());
                    }
                    else if(menuBtnList[i].settingType == SettingType.skin)
                    {
                        centerObjSet.mainSprite.spriteName = obj.GetComponent<UISprite>().spriteName;
                        centerObjSet.mainSprite_back.spriteName = obj.GetComponent<UISprite>().spriteName;
                        centerObjSet.mainLabel.text = LocalizeText.Value[menuBtnList[i].settingLabel];
                    }
                    else
                    {
                        centerObjSet.mainLabel.text = LocalizeText.Value[menuBtnList[i].settingLabel];
                    }

                    centerObjSet.mainSprite.width = centerObjSet.mainSprite.height;

                    SelectMainView();
                }
                else
                {
                    MainMenuColliderEnabled(true);
                    return;
                }
            }
        }

        if (circleUISet.clickObj != null)
        {
            ConeRotCoroutineStart();
        }
        else
        {
            Debug.Log("클릭된 버튼이 잘못되었습니다.");
        }
    }

    /// <summary>
    ///  모든 카테고리1 버튼 collider on/off
    /// </summary>
    /// <param name="state"></param>
    public void MainMenuColliderEnabled(bool state)
    {
        for(int i=0; i<menuBtnList.Length; i++)
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
        TweenManager.tween_Manager.AddTweenScale(centerObjSet.mainProgress.gameObject, Vector3.zero, Vector3.one, 0.5f, UITweener.Style.Once, TweenManager.tween_Manager.scaleAnimationCurve);
        TweenManager.tween_Manager.TweenScale(centerObjSet.mainProgress.gameObject);
    }

    /// <summary>
    /// 중앙부분 뒤로가기 버튼 클릭
    /// </summary>
    public void CloseBtnClick()
    {
        CloseBtnState(false);
        CloseBtnClickEvent();
    }

    /// <summary>
    /// 뒤로가기 버튼클릭 이벤트
    /// </summary>
    private void CloseBtnClickEvent()
    {
        //카테고리 2,3 부분 활성화의 경우
        if (circleUISet.bgShade.fillAmount == 1)
        {
            SubMenuCloseCoroutineStart();
        }
    }

    /// <summary>
    /// close 버튼 클릭 collier on/off 
    /// </summary>
    /// <param name="state"></param>
    private void CloseBtnState(bool state)
    {
        //close 버튼 클릭 collier on/off 
        circleUISet.closeBtn.GetComponent<CircleCollider2D>().enabled = state;

        TweenManager.tween_Manager.TweenAllDestroy(circleUISet.coneObj);

        //뒤로가기 활성화의 경우
        if (state)
        {
            //conobj의 alpha값을 진하게 보여준다.
            TweenManager.tween_Manager.AddTweenAlpha(circleUISet.coneObj, circleUISet.coneObj.GetComponent<UIWidget>().alpha, 1, 0.2f);
        }
        //뒤로가기 비활성화의 경우
        else
        {
            //conobj의 alpha값을 연하게 보여준다.
            TweenManager.tween_Manager.AddTweenAlpha(circleUISet.coneObj, circleUISet.coneObj.GetComponent<UIWidget>().alpha, 0.3f, 0.2f);
        }
        TweenManager.tween_Manager.TweenAlpha(circleUISet.coneObj);

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
            /*
            case GlobalDataManager.CategoryType.Princess:
                sp.spriteName = "princess_loading_1";
                spAni.namePrefix = "princess_loading_";
                spAni.framesPerSecond = 8;
                break;                
            case GlobalDataManager.ProductType.Soccer:
                sp.spriteName = "soccer_loading_1";
                spAni.namePrefix = "soccer_loading_";
                spAni.framesPerSecond = 12;
                break;

            case GlobalDataManager.ProductType.RacingCar:
                sp.spriteName = "racingcar_loading_1";
                spAni.namePrefix = "racingcar_loading_";
                spAni.framesPerSecond = 15;
                break;

            case GlobalDataManager.ProductType.WatchCar:
                sp.spriteName = "watchcar_loading_1";
                spAni.namePrefix = "watchcar_loading_";
                spAni.framesPerSecond = 11;
                break;

            case GlobalDataManager.ProductType.NasCar:
                sp.spriteName = "nascar_loading_1";
                spAni.namePrefix = "nascar_loading_";
                spAni.framesPerSecond = 19;
                break;

            case GlobalDataManager.ProductType.PrincessDance:
                sp.spriteName = "princess_loading_1";
                spAni.namePrefix = "princess_loading_";
                spAni.framesPerSecond = 8;
                break;
                */
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

        if(isSettingMode)
        {
            SettingTypeClick(obj);
            return;
        }

        int loadSceneIndex = -1;
        string loadSceneName = string.Empty;

        for (int i=0; i< circleUISet.coneObj.transform.childCount;i++)
        {
            if(obj == circleUISet.coneObj.transform.GetChild(i).gameObject)
            {
                loadSceneIndex                          = i;
                loadSceneName                           = menuBtnList[clickObjectIndex].subMenuList[i].sceneName;

                GlobalDataManager.m_SelectedSceneName   = string.Format("{0}_{1}", GlobalDataManager.m_SelectedCategoryEnum.ToString().ToLower(), loadSceneName);
                GlobalDataManager.m_AssetBundlePartName = menuBtnList[clickObjectIndex].subMenuList[i].bundleName;

                Debug.Log("GlobalDataManager.m_AssetBundlePartName: " + GlobalDataManager.m_AssetBundlePartName);

                break;
            }
        }
        
   
        //SceneManager.LoadScene(GlobalDataManager.m_SelectedSceneName);
        StartCoroutine(OpenLoadScene());

        //loadingPopup.SetActive(true);
        InitLoadingPopup(true);
        ResetLoadingScreenOrientation(loadSceneName);
        
    }

    /// <summary>
    /// 셋팅모드에서 카테고리2,3 버튼 클릭시 
    /// </summary>
    /// <param name="obj"></param>
    private void SettingTypeClick(GameObject obj)
    {
        int loadSceneIndex = -1;
        string loadSceneName = string.Empty;
        string savedLocalize = LocalizeText.CurrentLanguage.ToString();

        for (int i = 0; i < circleUISet.coneObj.transform.childCount; i++)
        {
            if (obj == circleUISet.coneObj.transform.GetChild(i).gameObject)
            {
                loadSceneIndex = i;
            }
        }

        //언어변경 or 색상변경
        switch (menuBtnList[clickObjectIndex].subSettingList[loadSceneIndex].setBtnType)
        {
            case SettingBtnType.korean:
                LocalizeText.CurrentLanguage = (SystemLanguage)Enum.Parse(typeof(SystemLanguage), "Korean");
                PlayerPrefs.SetString("DefaultLocalize", savedLocalize);
                PlayerPrefs.Save();
                break;

            case SettingBtnType.english:
                LocalizeText.CurrentLanguage = (SystemLanguage)Enum.Parse(typeof(SystemLanguage), "English");
                PlayerPrefs.SetString("DefaultLocalize", savedLocalize);
                PlayerPrefs.Save();
                break;

            case SettingBtnType.indonesian:
                LocalizeText.CurrentLanguage = (SystemLanguage)Enum.Parse(typeof(SystemLanguage), "Indonesian");
                PlayerPrefs.SetString("DefaultLocalize", savedLocalize);
                PlayerPrefs.Save();
                break;
                
            case SettingBtnType.blackSkin:
                uiColor = UIColor.black;
                UISkinSetting();
                break;

            case SettingBtnType.pinkSkin:
                uiColor = UIColor.pink;
                UISkinSetting();
                break;

            case SettingBtnType.skyboxSkin:
                uiColor = UIColor.skybox;
                UISkinSetting();
                break;

            case SettingBtnType.exit:
                break;

            default:
                Debug.Log("setting typeclick type부분에 이상이 있습니다.");
                return;
        }

        //중앙부분 이미지, 텍스트 및 언어 변경 버튼 부분 이미지, 텍스트 변경
        SettingTypeClickSpriteSet(menuBtnList[clickObjectIndex].subSettingList[loadSceneIndex].setBtnType);

    }

    /// <summary>
    /// 중앙부분 이미지, 텍스트 및 언어 변경 버튼 부분 이미지, 텍스트 변경
    /// </summary>
    /// <param name="setType"></param>
    private void SettingTypeClickSpriteSet(SettingBtnType setType)
    {
        switch(setType)
        {
            case SettingBtnType.korean:
            case SettingBtnType.english:
            case SettingBtnType.indonesian:
                menuBtnList[clickObjectIndex].menuLabel.text = LanguageLabelSetting(LocalizeText.CurrentLanguage.ToString());
                centerObjSet.mainLabel.text = LanguageLabelSetting(LocalizeText.CurrentLanguage.ToString());

                LanguageSpriteChange(centerObjSet.mainSprite.gameObject);
                LanguageSpriteChange(centerObjSet.mainSprite_back.gameObject);
                LanguageSpriteChange(menuBtnList[clickObjectIndex].menuBtn);
                LanguageSpriteChange(menuBtnList[clickObjectIndex].menuBtn_back.gameObject);
                break;

            case SettingBtnType.blackSkin:
            case SettingBtnType.pinkSkin:
            case SettingBtnType.skyboxSkin:
                centerObjSet.mainLabel.text = LocalizeText.Value[menuBtnList[clickObjectIndex].settingLabel];

                SkinSpriteChange(centerObjSet.mainSprite.gameObject);
                SkinSpriteChange(centerObjSet.mainSprite_back.gameObject);
                SkinSpriteChange(menuBtnList[clickObjectIndex].menuBtn);
                SkinSpriteChange(menuBtnList[clickObjectIndex].menuBtn_back.gameObject);
                break;

            default:
                break;
        }

        SettingLabelInit();
        Invoke("CloseBtnClick", 0.2f);
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

    private void SettingLabelInit()
    {
        for (int i = 0; i < menuBtnList.Length; i++)
        {
            if (menuBtnList[i].settingType != SettingType.none)
            {
                if (menuBtnList[i].settingType == SettingType.language)
                {
                    menuBtnList[i].menuLabel.text = LanguageLabelSetting(LocalizeText.CurrentLanguage.ToString());
                }
                else
                {
                    menuBtnList[i].menuLabel.text = LocalizeText.Value[menuBtnList[i].settingLabel];
                }
            }
        }
    }

    /// <summary>
    /// coneobj 클릭지점으로 회전 코루틴 호출
    /// </summary>
    private void ConeRotCoroutineStart()
    {
        ConeRotCoroutineStop();
        coneRotCoroutine = StartCoroutine(RotConeObj());
    }

    /// <summary>
    /// coneobj 클릭지점으로 회전 코루틴 중지
    /// </summary>
    private void ConeRotCoroutineStop()
    {
        if (coneRotCoroutine != null)
        {
            StopCoroutine(coneRotCoroutine);
            coneRotCoroutine = null;
        }
    }

    /// <summary>
    /// submenu 열기 부분 코루틴 호출
    /// </summary>
    public void SubMenuOpenCoroutineStart()
    {
        MainMenuLabelTextChange();
        SubMenuOpenCoroutineStop();

        SettingBtnColliderSet(true);
        MainBackImgActive(true);
        UISkinTween(centerObjSet.mainSprite_back.gameObject, Color.white,0);

        if (isSettingMode)
        {
            subMenuOpenCoroutine = StartCoroutine(SubSettingMenuOpen());
        }
        else
        {
            subMenuOpenCoroutine = StartCoroutine(SubMenuOpen());
        }
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

    /// <summary>
    /// coneobj 시작지점으로 회전 코루틴 호출
    /// </summary>
    private void ConeRotReverseCoroutineStart()
    {
        ConeRotReverseCoroutineStop();
        coneRotReverseCoroutine = StartCoroutine(RotConeObjReverse());
    }

    /// <summary>
    /// coneobj 시작지점으로 회전 코루틴 중지
    /// </summary>
    private void ConeRotReverseCoroutineStop()
    {
        if (coneRotReverseCoroutine != null)
        {
            StopCoroutine(coneRotReverseCoroutine);
            coneRotReverseCoroutine = null;
        }
    }

    /// <summary>
    /// submenu 닫기 코루틴 호출
    /// </summary>
    private void SubMenuCloseCoroutineStart()
    {
        SubMenuCloseCoroutineStop();

        subMenuCloseCoroutine = StartCoroutine(SubMenuClose());
    }

    /// <summary>
    /// submenu 닫기 코루틴 중지
    /// </summary>
    private void SubMenuCloseCoroutineStop()
    {
        if (subMenuCloseCoroutine != null)
        {
            StopCoroutine(subMenuCloseCoroutine);
            subMenuCloseCoroutine = null;
        }
    }

    /// <summary>
    /// coneObj 회전 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator RotConeObj()
    {
        float speed = 10.0f;
        float breakTime = 4.8f / speed;
        float time = 0;

        Quaternion targetRot;

        Vector3 diff = circleUISet.clickObj.transform.position - circleUISet.coneObj.transform.position;
        diff.Normalize();

        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        
        targetRot = Quaternion.Euler(0f, 0f, rot_z - (90 + (circleUISet.coneObj.GetComponent<UISprite>().fillAmount * 180)));
        
        while (true)
        {
            yield return new WaitForEndOfFrame();

            time += Time.deltaTime;

            circleUISet.coneObj.transform.rotation = Quaternion.Slerp(circleUISet.coneObj.transform.rotation, targetRot, Time.deltaTime * speed);

            if (circleUISet.coneObj.transform.rotation == targetRot || time > breakTime)
            {
                time = 0;
                if (!isSettingMode)
                {
                    OnClickCategoryButton(circleUISet.clickObj);
                }
                else
                {
                    SubMenuOpenCoroutineStart();
                }
                //SubMenuOpenCoroutineStart();
                yield break;
            }
        }
    }

    /// <summary>
    /// coneobj 원위치로 회전 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator RotConeObjReverse()
    {
        float speed = 10.0f;
        float breakTime = 4.8f / speed;
        float time = 0;

        Quaternion targetRot;

        Vector3 diff = new Vector3(0,0,-18.0f) - circleUISet.coneObj.transform.localPosition;
        diff.Normalize();

        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;

        targetRot = Quaternion.Euler(0f, 0f, rot_z - (circleUISet.coneObj.GetComponent<UISprite>().fillAmount * 180));

        //MainViewInit();

        while (true)
        {
            yield return new WaitForEndOfFrame();

            time += Time.deltaTime;

            circleUISet.coneObj.transform.rotation = Quaternion.Slerp(circleUISet.coneObj.transform.rotation, targetRot, Time.deltaTime * speed);

            if (circleUISet.coneObj.transform.rotation == targetRot || time > breakTime)
            {
                MainMenuColliderEnabled(true);
                time = 0;
                yield break;
            }
        }
    }

    /// <summary>
    /// 일반모드 카테고리 2,3 open 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator SubMenuOpen()
    {
        firstAngle = circleUISet.coneObj.transform.localEulerAngles.z;

        float targetfillAmount = 0;
        float sumRot = 0;
        float coneSpeed = 400.0f;
        float conefillSpeed = coneSpeed / 180;
        float bgShadeSpeed = conefillSpeed * 1.8f;

        bool coneFillSet = false;
        bool angleSet = false;
        bool bgShadeFillSet = false;

        int subBtnNum = 0;

        for (int i = 0; i < menuBtnList.Length; i++)
        {
            if (circleUISet.clickObj == menuBtnList[i].menuBtn)
            {
                targetfillAmount = (menuBtnList[i].subMenuList.Length * 0.1f);
                sumRot = (targetfillAmount - 0.1f) * 180f;
                subBtnNum = menuBtnList[i].subMenuList.Length;

                subMenuBtnSet(subBtnNum, (firstAngle - sumRot));
                break;
            }
        }

        while (true)
        {
            yield return new WaitForEndOfFrame();

            //bgShade fillAmount 체크 0->1
            if (!bgShadeFillSet)
            {
                if (circleUISet.bgShade.fillAmount < 1)
                {
                    circleUISet.bgShade.fillAmount += (Time.deltaTime * bgShadeSpeed);
                }
                else
                {
                    bgShadeFillSet = true;
                }
            }

            //coneObj 서브메뉴(카테고리2,3)개수에 따른 각도변경 체크
            if (!angleSet)
            {
                if (firstAngle - sumRot < circleUISet.coneObj.transform.localEulerAngles.z)
                {
                    circleUISet.coneObj.transform.localEulerAngles = new Vector3(0, 0, circleUISet.coneObj.transform.localEulerAngles.z - (Time.deltaTime * coneSpeed));
                }
                else
                {
                    angleSet = true;
                }
            }

            //coneObj 서브메뉴(카테고리2,3)개수에 따른 fillAmount 변경 체크
            if (!coneFillSet)
            {
                if (targetfillAmount > circleUISet.coneObj.GetComponent<UISprite>().fillAmount)
                {
                    circleUISet.coneObj.GetComponent<UISprite>().fillAmount += (Time.deltaTime * conefillSpeed);
                }
                else
                {
                    coneFillSet = true;
                }
            }

            //bgshade, coneObj 모든 설정 완료
            if (angleSet && coneFillSet && bgShadeFillSet)
            {
                subMenuBtnView(subBtnNum, true);
                CloseBtnState(true);

                //일반모드의 경우
                if (!isSettingMode)
                {
                    SetCategoryOnOff();
                }
                yield break;
            }

        }
    }

    /// <summary>
    /// 카테고리 2,3 close 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator SubMenuClose()
    {
        float coneSpeed = 400.0f;
        float conefillSpeed = coneSpeed / 180;
        float bgShadeSpeed = conefillSpeed * 1.8f;

        bool coneFillSet = false;
        bool angleSet = false;
        bool bgShadeFillSet = true;

        int subBtnNum = 0;

        for (int i = 0; i < circleUISet.coneObj.transform.childCount; i++)
        {
            if (circleUISet.coneObj.transform.GetChild(i).gameObject.activeSelf && circleUISet.coneObj.transform.GetChild(i).localScale.x > 0)
            {
                subBtnNum += 1;
            }
        }

        subMenuBtnView(subBtnNum, false);

        yield return new WaitForSeconds(0.2f);

        MainViewInit();

        while (true)
        {
            yield return new WaitForEndOfFrame();

            //bgShade fillAmount 체크 1->0
            if (!bgShadeFillSet)
            {
                if (circleUISet.bgShade.fillAmount > 0)
                {
                    circleUISet.bgShade.fillAmount -= (Time.deltaTime * bgShadeSpeed);
                }
                else
                {
                    circleUISet.bgShade.fillAmount = 0;
                    bgShadeFillSet = true;
                }
            }

            //coneObj 서브메뉴(카테고리2,3)개수에 따른 각도변경 체크
            if (!angleSet)
            {
                if (firstAngle > circleUISet.coneObj.transform.localEulerAngles.z)
                {
                    circleUISet.coneObj.transform.localEulerAngles = new Vector3(0, 0, circleUISet.coneObj.transform.localEulerAngles.z + (Time.deltaTime * coneSpeed));
                }
                else
                {
                    circleUISet.coneObj.transform.localEulerAngles = new Vector3(0, 0, firstAngle);
                    angleSet = true;
                    bgShadeFillSet = false;
                }
            }

            //coneObj 서브메뉴(카테고리2,3)개수에 따른 fillAmount 변경 체크
            if (!coneFillSet)
            {
                if (0.1 < circleUISet.coneObj.GetComponent<UISprite>().fillAmount)
                {
                    circleUISet.coneObj.GetComponent<UISprite>().fillAmount -= (Time.deltaTime * conefillSpeed);
                }
                else
                {
                    circleUISet.coneObj.GetComponent<UISprite>().fillAmount = 0.1f;
                    coneFillSet = true;
                }
            }
            
            //bgshade, coneObj 모든 설정 완료
            if (angleSet && coneFillSet && bgShadeFillSet)
            {
                ConeRotReverseCoroutineStart();
                yield break;
            }

        }
    }

    /// <summary>
    /// 셋팅모드 카테고리 2,3 open 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator SubSettingMenuOpen()
    {
        firstAngle = circleUISet.coneObj.transform.localEulerAngles.z;

        float targetfillAmount = 0;
        float sumRot = 0;
        float coneSpeed = 400.0f;
        float conefillSpeed = coneSpeed / 180;
        float bgShadeSpeed = conefillSpeed * 1.8f;

        bool coneFillSet = false;
        bool angleSet = false;
        bool bgShadeFillSet = false;

        int subBtnNum = 0;

        for (int i = 0; i < menuBtnList.Length; i++)
        {
            if (circleUISet.clickObj == menuBtnList[i].menuBtn)
            {
                targetfillAmount = (menuBtnList[i].subSettingList.Length * 0.1f);
                sumRot = (targetfillAmount - 0.1f) * 180f;
                subBtnNum = menuBtnList[i].subSettingList.Length;

                subMenuBtnSet(subBtnNum, (firstAngle - sumRot));
                break;
            }
        }

        while (true)
        {
            yield return new WaitForEndOfFrame();

            //bgShade fillAmount 체크 0->1
            if (!bgShadeFillSet)
            {
                if (circleUISet.bgShade.fillAmount < 1)
                {
                    circleUISet.bgShade.fillAmount += (Time.deltaTime * bgShadeSpeed);
                }
                else
                {
                    bgShadeFillSet = true;
                }
            }

            //coneObj 서브메뉴(카테고리2,3)개수에 따른 각도변경 체크
            if (!angleSet)
            {
                if (firstAngle - sumRot < circleUISet.coneObj.transform.localEulerAngles.z)
                {
                    circleUISet.coneObj.transform.localEulerAngles = new Vector3(0, 0, circleUISet.coneObj.transform.localEulerAngles.z - (Time.deltaTime * coneSpeed));
                }
                else
                {
                    angleSet = true;
                }
            }

            //coneObj 서브메뉴(카테고리2,3)개수에 따른 fillAmount 변경 체크
            if (!coneFillSet)
            {
                if (targetfillAmount > circleUISet.coneObj.GetComponent<UISprite>().fillAmount)
                {
                    circleUISet.coneObj.GetComponent<UISprite>().fillAmount += (Time.deltaTime * conefillSpeed);
                }
                else
                {
                    coneFillSet = true;
                }
            }

            //bgshade, coneObj 모든 설정 완료
            if (angleSet && coneFillSet && bgShadeFillSet)
            {
                SettingSubBtnView(subBtnNum, true);
                CloseBtnState(true);

                //일반모드의 경우
                if (!isSettingMode)
                {
                    SetCategoryOnOff();
                }
                yield break;
            }

        }
    }

    /// <summary>
    /// 씬로드
    /// </summary>
    /// <returns></returns>
    public IEnumerator OpenLoadScene()
    {
        SceneManager.LoadSceneAsync(GlobalDataManager.m_SelectedSceneName);
        yield return null;
    }

}
