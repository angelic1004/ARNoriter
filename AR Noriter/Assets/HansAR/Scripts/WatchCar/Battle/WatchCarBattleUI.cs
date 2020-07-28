using UnityEngine;
using System;
using System.Collections;

public class WatchCarBattleUI : MonoBehaviour
{
    // public GameObject m_battleStartBtn;
    public GameObject m_RealizePopUpUI;

    [Serializable]
    public class BattleSettingUI
    {
        public GameObject m_battleSettingUI;
        public GameObject m_battleStartBtn;
        public GameObject m_playerPortrait;
        public GameObject m_enemyPortrait;
        public Vector3 m_playerPortraitFirstPos;
        public Vector3 m_enemyPortraitFirstPos;
        public GameObject m_versus;
    }

    [Serializable]
    public class BattleUI
    {
        public GameObject m_battleMainUI;
        public GameObject m_cardUI;
        public GameObject[] m_battleCardList;
        public Vector3[] m_battleCardPos;
    }

    [Serializable]
    public class ProfileUI
    {
        public GameObject m_profileMainUI;
        public PlayerProfileUI m_playerinfo;
        public PlayerProfileUI m_enemyinfo;
    }

    [Serializable]
    public class PlayerProfileUI
    {
        public GameObject m_profileUI;
        public GameObject m_profileSprite;
        public UILabel m_profileCarNameLabel;

        public float m_power;
        public float m_normalPower;

        public float m_hp;
        public float m_normalHp;

        public GameObject m_hpBar;

        public GameObject m_buffUI;
        public GameObject[] m_buffObjList;
        public WatchCarBattleManager.CardBuffState[] m_buffUiList;
    }

    [Serializable]
    public class BuffEffectUI
    {
        public GameObject m_buffEffectMainUI;
        public GameObject m_bgImg;
        public GameObject m_playerImg;
        public GameObject m_enemyImg;

        public Vector3 m_playerImgStartPos;
        public Vector3 m_playerBgstartPos;

        public Vector3 m_enemyImgStartPos;
        public Vector3 m_enemyBgStartPos;
    }

    [Serializable]
    public class ResultUI
    {
        public GameObject m_resultMainUI;
        public GameObject m_resultPopUpUI;
        public UILabel m_resultLabel;
        public GameObject m_resultImg;
    }

    [SerializeField]
    public BattleSettingUI m_battleSettingUI;

    [SerializeField]
    public BattleUI m_battleUI;

    [SerializeField]
    public ProfileUI m_profileUI;

    [SerializeField]
    public BuffEffectUI m_buffUI;

    [SerializeField]
    public ResultUI m_resultUI;

    private Coroutine m_resultCoroutine;

    public static WatchCarBattleUI watcarBattleUI;

    void Awake()
    {
        watcarBattleUI = this;
    }

    void Start()
    {
        m_battleSettingUI.m_playerPortraitFirstPos = m_battleSettingUI.m_playerPortrait.transform.localPosition;
        m_battleSettingUI.m_enemyPortraitFirstPos = m_battleSettingUI.m_enemyPortrait.transform.localPosition;

        m_buffUI.m_playerBgstartPos = m_buffUI.m_bgImg.transform.localPosition;
        m_buffUI.m_playerImgStartPos = m_buffUI.m_playerImg.transform.localPosition;

        m_buffUI.m_enemyBgStartPos = new Vector3(-m_buffUI.m_bgImg.transform.localPosition.x,
                                                 m_buffUI.m_bgImg.transform.localPosition.y, 
                                                 m_buffUI.m_bgImg.transform.localPosition.z);

        m_buffUI.m_enemyImgStartPos = m_buffUI.m_enemyImg.transform.localPosition;

        ResultUiPopUpInit();

        WatchCarCardPosSetting();
    }

    /// <summary>
    /// 처음 시작시 UI 초기화 부분
    /// </summary>
    public void WatchCarBattleUiInit()
    {
        m_battleSettingUI.m_battleSettingUI.SetActive(true);
        m_battleSettingUI.m_battleStartBtn.SetActive(false);
        m_battleSettingUI.m_versus.SetActive(false);

        m_battleUI.m_battleMainUI.SetActive(false);

        m_profileUI.m_profileMainUI.SetActive(false);

        m_buffUI.m_buffEffectMainUI.SetActive(false);

        m_RealizePopUpUI.SetActive(false);
  
        WatchCarCardActive(true);

        BuffSettingInit();
        BuffEffectInit();
        WatchCarStateInit();
        ResultUiPopUpInit();

        MainUI.메인.인식글자UI.SetActive(true);
    }

    /// <summary>
    /// 게임중 매턴 마다 UI 초기화하는 부분
    /// </summary>
    public void WatchCarBattlePlayingUiInit()
    {
        //BuffSettingInit();
        BuffEffectInit();
        WatchCarPowerInit();
        WatchCarCardActive(true);
    }

    /// <summary>
    /// 게임이 끝나고 다시 시작클릭할 경우
    /// </summary>
    public void WatchCarBattleRestartBtnClick()
    {
        WatchCarBattleManager.watchCarBattleManager.WatchCarBattlePlayingInit();

        BuffSettingInit();
        WatchCarStateInit();
        WatchCarEffectManager.watchCarEffectManager.WatchCarEffectInit();
        m_RealizePopUpUI.SetActive(false);
    }

    /// <summary>
    /// 재시작 팝업 
    /// </summary>
    /// <param name="state"></param>
    public void RealizePopUpUICall(bool state)
    {
        if (state)
        {
            TweenManager.tween_Manager.AddTweenAlpha(m_RealizePopUpUI, 0, 1, 0.2f);
            TweenManager.tween_Manager.TweenAlpha(m_RealizePopUpUI);
        }
        else
        {
            if (m_RealizePopUpUI.activeSelf)
            {
                TweenManager.tween_Manager.AddTweenAlpha(m_RealizePopUpUI, 1, 0, 0.2f);
                TweenManager.tween_Manager.TweenAlpha(m_RealizePopUpUI);
            }
        }
    }

    /// <summary>
    /// 배틀스타트 클릭시
    /// </summary>
    /// <param name="state"></param>
    public void BattleStartBtnSet(bool state)
    {
        m_battleSettingUI.m_battleStartBtn.GetComponent<UIButton>().tweenTarget = null;

        if (state)
        {
            TweenManager.tween_Manager.AddTweenAlpha(m_battleSettingUI.m_battleStartBtn, 0, 1, 0.2f);
            TweenManager.tween_Manager.TweenAlpha(m_battleSettingUI.m_battleStartBtn);
        }
        else
        {
            if (m_battleSettingUI.m_battleStartBtn.activeSelf)
            {
                TweenManager.tween_Manager.AddTweenAlpha(m_battleSettingUI.m_battleStartBtn, 1, 0, 0.2f);
                TweenManager.tween_Manager.TweenAlpha(m_battleSettingUI.m_battleStartBtn);
            }
        }
    }

    /// <summary>
    /// 인식 후 재시작 클릭시
    /// </summary>
    public void RestartBtnClick()
    {
        WatchCarBattleManager.watchCarBattleManager.WatchCarBattleInit();
    }

    /// <summary>
    /// 인식 후 취소 클릭시
    /// </summary>
    public void CancleBtnClick()
    {
        RealizePopUpUICall(false);
    }

    /// <summary>
    /// 배틀스타트버튼 클릭
    /// </summary>
    public void BattleStartBtnClick()
    {
        BattleStartBtnSet(false);
        MainUI.메인.오버레이UI.SetActive(false);
        TargetManager.타깃메니저.컨텐츠최상위오브젝트.SetActive(false);

        WatchCarBattleProfileImgInit();
        RotateUI.회전.회전UI_숨기기();
        WatchCarBattleManager.watchCarBattleManager.WatchCarBattleStart();
        WatchCarEffectManager.watchCarEffectManager.WatchCarEffectInit();
    }

    /// <summary>
    /// 버프슬롯 초기화
    /// </summary>
    public void BuffSettingInit()
    {
        m_profileUI.m_playerinfo.m_buffObjList = new GameObject[m_profileUI.m_playerinfo.m_buffUI.transform.childCount];
        m_profileUI.m_playerinfo.m_buffUiList = new WatchCarBattleManager.CardBuffState[m_profileUI.m_playerinfo.m_buffUI.transform.childCount];

        m_profileUI.m_enemyinfo.m_buffObjList = new GameObject[m_profileUI.m_enemyinfo.m_buffUI.transform.childCount];
        m_profileUI.m_enemyinfo.m_buffUiList = new WatchCarBattleManager.CardBuffState[m_profileUI.m_enemyinfo.m_buffUI.transform.childCount];

        for (int i = 0; i < m_profileUI.m_playerinfo.m_buffUI.transform.childCount; i++)
        {
            m_profileUI.m_playerinfo.m_buffObjList[i] = m_profileUI.m_playerinfo.m_buffUI.transform.GetChild(i).gameObject;
            m_profileUI.m_playerinfo.m_buffObjList[i].SetActive(false);

            m_profileUI.m_playerinfo.m_buffUiList[i] = WatchCarBattleManager.CardBuffState.none;
        }

        for (int i = 0; i < m_profileUI.m_enemyinfo.m_buffUI.transform.childCount; i++)
        {
            m_profileUI.m_enemyinfo.m_buffObjList[i] = m_profileUI.m_enemyinfo.m_buffUI.transform.GetChild(i).gameObject;
            m_profileUI.m_enemyinfo.m_buffObjList[i].SetActive(false);

            m_profileUI.m_enemyinfo.m_buffUiList[i] = WatchCarBattleManager.CardBuffState.none;
        }
    }

    /// <summary>
    /// 맨처음 카드 위치 저장
    /// </summary>
    private void WatchCarCardPosSetting()
    {
        m_battleUI.m_battleCardPos = new Vector3[m_battleUI.m_battleCardList.Length];

        for (int i = 0; i < m_battleUI.m_battleCardList.Length; i++)
        {
            m_battleUI.m_battleCardPos[i] = m_battleUI.m_battleCardList[i].transform.localPosition;
        }
    }

    /// <summary>
    /// 카드 원위치
    /// </summary>
    public void WatchCarCardPosInit()
    {
        for (int i = 0; i < m_battleUI.m_battleCardList.Length; i++)
        {
            TweenManager.tween_Manager.TweenAllDestroy(m_battleUI.m_battleCardList[i].transform.GetChild(0).gameObject);
            TweenManager.tween_Manager.TweenAllDestroy(m_battleUI.m_battleCardList[i]);

            m_battleUI.m_battleCardList[i].transform.localPosition = m_battleUI.m_battleCardPos[i];
            m_battleUI.m_battleCardList[i].transform.localScale = Vector3.one;

            m_battleUI.m_battleCardList[i].GetComponent<UISprite>().spriteName = WatchCarBattleManager.watchCarBattleManager.m_selectCardSpriteName;
            m_battleUI.m_battleCardList[i].GetComponent<UIWidget>().depth = 1;

            m_battleUI.m_battleCardList[i].transform.GetChild(0).GetComponent<UIWidget>().alpha = 1;
            m_battleUI.m_battleCardList[i].transform.GetChild(0).GetComponent<UIWidget>().depth = 2;

        }
    }

    /// <summary>
    ///  맨처음 카드초기화시 m_cardUI scale값 조정
    /// </summary>
    public void WatchCarBattleCardUiInit()
    {
        TweenManager.tween_Manager.AddTweenScale(m_battleUI.m_cardUI, Vector3.zero, Vector3.one, 0.4f);
        TweenManager.tween_Manager.TweenScale(m_battleUI.m_cardUI);
        m_battleUI.m_cardUI.SetActive(true);
        m_battleUI.m_battleMainUI.SetActive(true);
    }


    /// <summary>
    ///  맨처음 카드초기화시 m_profileMainUI 알파값 조정
    /// </summary>
    public void WatchCarBattleProfileUiInit()
    {
        TweenManager.tween_Manager.AddTweenAlpha(m_profileUI.m_profileMainUI, 0, 1, 0.4f);
        TweenManager.tween_Manager.TweenAlpha(m_profileUI.m_profileMainUI);

        m_profileUI.m_profileMainUI.SetActive(true);
    }


    /// <summary>
    /// 플레이어 및 상대편 공격력, 체력 초기화
    /// </summary>
    private void WatchCarStateInit()
    {
        WatchCarPowerInit();
        WatchCarHpInit();
    }

    private void WatchCarPowerInit()
    {
        m_profileUI.m_playerinfo.m_power = m_profileUI.m_playerinfo.m_normalPower;
        m_profileUI.m_enemyinfo.m_power = m_profileUI.m_enemyinfo.m_normalPower;
    }

    private void WatchCarHpInit()
    {
        m_profileUI.m_playerinfo.m_hp = m_profileUI.m_playerinfo.m_normalHp;
        m_profileUI.m_enemyinfo.m_hp = m_profileUI.m_enemyinfo.m_normalHp;

        m_profileUI.m_playerinfo.m_hpBar.GetComponent<UISlider>().value = 1;
        m_profileUI.m_enemyinfo.m_hpBar.GetComponent<UISlider>().value = 1;
    }

    /// <summary>
    /// 카드선택
    /// </summary>
    public void WatchCarCardClick(GameObject obj)
    {
        if (WatchCarBattleManager.watchCarBattleManager.m_nowGame == WatchCarBattleManager.GameState.none)
        {
            WatchCarBattleManager.watchCarBattleManager.CardMove(obj);

            WatchCarCardActive(false);
        }

    }

    /// <summary>
    /// 카드 선택 활성화/비활성화 (콜라이더)
    /// </summary>
    /// <param name="state"></param>
    public void WatchCarCardActive(bool state)
    {
        for (int i = 0; i < m_battleUI.m_battleCardList.Length; i++)
        {
            m_battleUI.m_battleCardList[i].transform.GetChild(0).GetComponent<BoxCollider2D>().enabled = state;
        }
    }

    /// <summary>
    /// 버프 사용시 나오는 액션(2d) 부분 초기화
    /// </summary>
    public void BuffEffectInit()
    {
        m_buffUI.m_bgImg.SetActive(false);
        m_buffUI.m_playerImg.SetActive(false);
        m_buffUI.m_enemyImg.SetActive(false);
        m_buffUI.m_buffEffectMainUI.GetComponent<UIPanel>().alpha = 1;
        m_buffUI.m_bgImg.transform.localPosition = m_buffUI.m_playerBgstartPos;
        m_buffUI.m_playerImg.transform.localPosition = m_buffUI.m_playerImgStartPos;
        m_buffUI.m_enemyImg.transform.localPosition = m_buffUI.m_enemyImgStartPos;
    }

    private void WatchCarBattleProfileImgInit()
    {
        PlayerProfileImgInit();
        EnemyProfileImgInit();
    }

    private void PlayerProfileImgInit()
    {
        UISprite profileImg = m_profileUI.m_playerinfo.m_profileSprite.GetComponent<UISprite>();
        UISprite buffEffectImg = m_buffUI.m_playerImg.GetComponent<UISprite>();
        UISprite buffEffectBgImg = m_buffUI.m_bgImg.GetComponent<UISprite>();
        UISprite portraitImg = m_battleSettingUI.m_playerPortrait.GetComponent<UISprite>();

        switch(WatchCarBattleManager.watchCarBattleManager.m_playerInfo.m_character)
        {
            case WatchCarBattleManager.Character.bluewill:
                profileImg.spriteName = "profile_bluewill";
                buffEffectImg.spriteName = "buffeffect_bluewill";
                buffEffectBgImg.spriteName = "buffeffect_BG_bluewill";
                portraitImg.spriteName = "portrait_bluewill_left";
                break;
            case WatchCarBattleManager.Character.avan:
                profileImg.spriteName = "profile_avan";
                buffEffectImg.spriteName = "buffeffect_avan";
                buffEffectBgImg.spriteName = "buffeffect_BG_avan";
                portraitImg.spriteName = "portrait_avan_left";
                break;
            case WatchCarBattleManager.Character.sona:
                profileImg.spriteName = "profile_sona";
                buffEffectImg.spriteName = "buffeffect_sona";
                buffEffectBgImg.spriteName = "buffeffect_BG_Sona";
                portraitImg.spriteName = "portrait_sona_left";
                break;
            case WatchCarBattleManager.Character.porty:
                profileImg.spriteName = "profile_porty";
                buffEffectImg.spriteName = "buffeffect_porty";
                buffEffectBgImg.spriteName = "buffeffect_BG_porty";
                portraitImg.spriteName = "portrait_porty_left";
                break;
            case WatchCarBattleManager.Character.blood:
                profileImg.spriteName = "profile_blood";
                buffEffectImg.spriteName = "buffeffect_blood";
                buffEffectBgImg.spriteName = "buffeffect_BG_blood";
                portraitImg.spriteName = "portrait_blood_left";
                break;
            default:
                Debug.Log("플레이어 캐릭터가 선택되지 않았습니다.");
                return;
        }
    }

    private void EnemyProfileImgInit()
    {
        UISprite profileImg = m_profileUI.m_enemyinfo.m_profileSprite.GetComponent<UISprite>();
        UISprite buffEffectImg = m_buffUI.m_enemyImg.GetComponent<UISprite>();
        UISprite buffEffectBgImg = m_buffUI.m_bgImg.GetComponent<UISprite>();
        UISprite portraitImg = m_battleSettingUI.m_enemyPortrait.GetComponent<UISprite>();

        switch (WatchCarBattleManager.watchCarBattleManager.m_enemyInfo.m_character)
        {
            case WatchCarBattleManager.Character.bluewill:
                profileImg.spriteName = "profile_bluewill";
                buffEffectImg.spriteName = "buffeffect_bluewill";
                buffEffectBgImg.spriteName = "buffeffect_BG_bluewill";
                portraitImg.spriteName = "portrait_bluewill_right";
                break;
            case WatchCarBattleManager.Character.avan:
                profileImg.spriteName = "profile_avan";
                buffEffectImg.spriteName = "buffeffect_avan";
                buffEffectBgImg.spriteName = "buffeffect_BG_avan";
                portraitImg.spriteName = "portrait_avan_right";
                break;
            case WatchCarBattleManager.Character.sona:
                profileImg.spriteName = "profile_sona";
                buffEffectImg.spriteName = "buffeffect_sona";
                buffEffectBgImg.spriteName = "buffeffect_BG_Sona";
                portraitImg.spriteName = "portrait_sona_right";
                break;
            case WatchCarBattleManager.Character.porty:
                profileImg.spriteName = "profile_porty";
                buffEffectImg.spriteName = "buffeffect_porty";
                buffEffectBgImg.spriteName = "buffeffect_BG_porty";
                portraitImg.spriteName = "portrait_porty_right";
                break;
            case WatchCarBattleManager.Character.blood:
                profileImg.spriteName = "profile_blood";
                buffEffectImg.spriteName = "buffeffect_blood";
                buffEffectBgImg.spriteName = "buffeffect_BG_blood";
                portraitImg.spriteName = "portrait_blood_right";
                break;
            default:
                Debug.Log("플레이어 캐릭터가 선택되지 않았습니다.");
                return;
        }
    }

    private void EffectBgChange(WatchCarBattleManager.Character character)
    {
        UISprite buffEffectBgImg = m_buffUI.m_bgImg.GetComponent<UISprite>();

        switch (character)
        {
            case WatchCarBattleManager.Character.bluewill:
                buffEffectBgImg.spriteName = "buffeffect_BG_bluewill";
                break;
            case WatchCarBattleManager.Character.avan:
                buffEffectBgImg.spriteName = "buffeffect_BG_avan";
                break;
            case WatchCarBattleManager.Character.sona:
                buffEffectBgImg.spriteName = "buffeffect_BG_Sona";
                break;
            case WatchCarBattleManager.Character.porty:
                buffEffectBgImg.spriteName = "buffeffect_BG_porty";
                break;
            case WatchCarBattleManager.Character.blood:
                buffEffectBgImg.spriteName = "buffeffect_BG_blood";
                break;
            default:
                Debug.Log("플레이어 캐릭터가 선택되지 않았습니다.");
                return;
        }
    }

    /// <summary>
    /// 버프 사용시 나오는 액션(2d) 배경 움직임 부분
    /// </summary>
    public void BuffEffectMove()
    {
        float tweenTime = 1.0f;

        if (WatchCarBattleManager.watchCarBattleManager.m_nowCard == WatchCarBattleManager.CardState.success
            || WatchCarBattleManager.watchCarBattleManager.m_nowBuffCard == WatchCarBattleManager.CardBuffState.doubleAttack)
        {
            EffectBgChange(WatchCarBattleManager.watchCarBattleManager.m_playerInfo.m_character);

            TweenManager.tween_Manager.TweenAllDestroy(m_buffUI.m_buffEffectMainUI);
            TweenManager.tween_Manager.TweenAllDestroy(m_buffUI.m_playerImg);
            TweenManager.tween_Manager.TweenAllDestroy(m_buffUI.m_bgImg);

            TweenManager.tween_Manager.AddTweenPosition(m_buffUI.m_playerImg, m_buffUI.m_playerImgStartPos,
                                                    new Vector3(m_buffUI.m_playerImgStartPos.x + (m_buffUI.m_playerImg.GetComponent<UIWidget>().width / 3), m_buffUI.m_playerImgStartPos.y, m_buffUI.m_playerImgStartPos.z),
                                                    tweenTime / 2);

            TweenManager.tween_Manager.AddTweenPosition(m_buffUI.m_bgImg, m_buffUI.m_playerBgstartPos,
                                                        new Vector3(-m_buffUI.m_playerBgstartPos.x, m_buffUI.m_playerBgstartPos.y, m_buffUI.m_playerBgstartPos.z),
                                                        tweenTime);

            TweenManager.tween_Manager.TweenPosition(m_buffUI.m_playerImg);
            TweenManager.tween_Manager.TweenPosition(m_buffUI.m_bgImg);

            m_buffUI.m_buffEffectMainUI.GetComponent<UIPanel>().alpha = 1;

            if(WatchCarBattleManager.watchCarBattleManager.m_nowCard == WatchCarBattleManager.CardState.success)
            {
                WatchCarBattleSoundManager.watchcarSoundManager.SkillAttackSoundCall();
            }
            else if(WatchCarBattleManager.watchCarBattleManager.m_nowBuffCard == WatchCarBattleManager.CardBuffState.doubleAttack)
            {
                WatchCarBattleSoundManager.watchcarSoundManager.SkillDoubleSoundCall();
            }

            m_buffUI.m_bgImg.SetActive(true);
            m_buffUI.m_playerImg.SetActive(true);
            m_buffUI.m_buffEffectMainUI.SetActive(true);
        }
        else if(WatchCarBattleManager.watchCarBattleManager.m_nowCard == WatchCarBattleManager.CardState.fail)
        {
            EffectBgChange(WatchCarBattleManager.watchCarBattleManager.m_enemyInfo.m_character);

            TweenManager.tween_Manager.TweenAllDestroy(m_buffUI.m_buffEffectMainUI);
            TweenManager.tween_Manager.TweenAllDestroy(m_buffUI.m_enemyImg);
            TweenManager.tween_Manager.TweenAllDestroy(m_buffUI.m_bgImg);

            TweenManager.tween_Manager.AddTweenPosition(m_buffUI.m_enemyImg, m_buffUI.m_enemyImgStartPos,
                                                    new Vector3(m_buffUI.m_enemyImgStartPos.x - (m_buffUI.m_enemyImg.GetComponent<UIWidget>().width / 3), m_buffUI.m_enemyImgStartPos.y, m_buffUI.m_enemyImgStartPos.z),
                                                    tweenTime / 2);

            TweenManager.tween_Manager.AddTweenPosition(m_buffUI.m_bgImg, m_buffUI.m_enemyBgStartPos,
                                                        new Vector3(0, m_buffUI.m_enemyBgStartPos.y, m_buffUI.m_enemyBgStartPos.z),
                                                        tweenTime);

            TweenManager.tween_Manager.TweenPosition(m_buffUI.m_enemyImg);
            TweenManager.tween_Manager.TweenPosition(m_buffUI.m_bgImg);

            m_buffUI.m_buffEffectMainUI.GetComponent<UIPanel>().alpha = 1;

            WatchCarBattleSoundManager.watchcarSoundManager.SkillAttackSoundCall();

            m_buffUI.m_bgImg.SetActive(true);
            m_buffUI.m_enemyImg.SetActive(true);
            m_buffUI.m_buffEffectMainUI.SetActive(true);
        }
    }

    /// <summary>
    /// 버프 액션 끝나고 사라지는 부분
    /// </summary>
    public void BuffEffectMoveStop()
    {
        TweenManager.tween_Manager.AddTweenAlpha(m_buffUI.m_buffEffectMainUI, 1, 0, 0.4f);
        TweenManager.tween_Manager.TweenAlpha(m_buffUI.m_buffEffectMainUI);
    }

    public void ResultUiPopUpCall()
    {
        RestulViewCoroutineStart();
        //ResultUiPopUpSet(true);
    }

    public void ResultUiPopUpReStart()
    {
        ResultUiPopUpUnClose();
        WatchCarBattleRestartBtnClick();
    }

    public void ResultUiPopUpExit()
    {
        ResultUiPopUpUnClose();
        RestartBtnClick();
    }

    public void ResultUiPopUpUnClose()
    {
        ResultUiPopUpSet(false);
    }

    public void RestulViewCoroutineStart()
    {
        RestulViewCoroutineStop();
        m_resultCoroutine = StartCoroutine(ResultView());
    }

    public void RestulViewCoroutineStop()
    {
        if(m_resultCoroutine != null)
        {
            StopCoroutine(m_resultCoroutine);
            m_resultCoroutine = null;
        }
    }

    private void ResultUiPopUpInit()
    {
        m_resultUI.m_resultMainUI.SetActive(false);
        m_resultUI.m_resultPopUpUI.SetActive(false);
        m_resultUI.m_resultImg.SetActive(false);
    }

    private void ResultUiPopUpSet(bool state)
    {
        if (state)
        {
            TweenManager.tween_Manager.TweenAllDestroy(m_resultUI.m_resultPopUpUI);
            TweenManager.tween_Manager.AddTweenAlpha(m_resultUI.m_resultPopUpUI, 0, 1, 0.3f);
            TweenManager.tween_Manager.TweenAlpha(m_resultUI.m_resultPopUpUI);
        }
        else
        {
            if (m_resultUI.m_resultPopUpUI.activeSelf)
            {
                TweenManager.tween_Manager.TweenAllDestroy(m_resultUI.m_resultPopUpUI);
                TweenManager.tween_Manager.TweenAllDestroy(m_resultUI.m_resultMainUI);

                TweenManager.tween_Manager.AddTweenAlpha(m_resultUI.m_resultPopUpUI, 1, 0, 0.3f);
                TweenManager.tween_Manager.AddTweenAlpha(m_resultUI.m_resultMainUI, 1, 0, 0.3f);

                TweenManager.tween_Manager.TweenAlpha(m_resultUI.m_resultPopUpUI);
                TweenManager.tween_Manager.TweenAlpha(m_resultUI.m_resultMainUI);
            }

            if(m_resultUI.m_resultImg.activeSelf)
            {
                WinImgPopUpSet(false);
            }
        }
    }


    private void WinImgPopUpSet(bool state)
    {
        if (state)
        {
            TweenManager.tween_Manager.TweenAllDestroy(m_resultUI.m_resultImg);
            TweenManager.tween_Manager.AddTweenAlpha(m_resultUI.m_resultImg, 0, 1, 0.3f);
            TweenManager.tween_Manager.TweenAlpha(m_resultUI.m_resultImg);
        }
        else
        {
            if (m_resultUI.m_resultImg.activeSelf)
            {
                TweenManager.tween_Manager.TweenAllDestroy(m_resultUI.m_resultImg);
                TweenManager.tween_Manager.AddTweenAlpha(m_resultUI.m_resultImg, 1, 0, 0.3f);
                TweenManager.tween_Manager.TweenAlpha(m_resultUI.m_resultImg);
            }
        }
    }


    private IEnumerator ResultView()
    {
        float delayTime = 0.8f;

        TweenManager.tween_Manager.TweenAllDestroy(m_resultUI.m_resultMainUI);
        m_resultUI.m_resultMainUI.SetActive(true);
        m_resultUI.m_resultMainUI.GetComponent<UIPanel>().alpha = 1;

        m_resultUI.m_resultImg.SetActive(false);

        if (WatchCarBattleManager.watchCarBattleManager.m_nowGame == WatchCarBattleManager.GameState.win)
        {
            WinImgPopUpSet(true);
            yield return new WaitForSeconds(delayTime);
        }
        else
        {
            m_resultUI.m_resultImg.SetActive(false);
        }

        //WinImgPopUpSet(false);
        yield return new WaitForSeconds(delayTime/2);

        ResultUiPopUpSet(true);
        yield break;
    }
}