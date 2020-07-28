using UnityEngine;
using System;
using System.Collections;
public class WatchCarBattleManager : MonoBehaviour
{
    public enum GameState
    {
        none,
        win,
        lose
    }

    /// <summary>
    /// 차량 종류
    /// </summary>
    public enum Character
    {
        none,
        bluewill,
        avan,
        sona,
        porty,
        blood
    }
    
    public enum CardState
    {
        none,
        success,
        fail,
        buff
    }

    public enum CardBuffState
    {
        none,
        doubleAttack,
        shield,
        heal
    }

    /// <summary>
    /// 배틀 시작 여부
    /// </summary>
    public bool m_battlePlaying = false;
    public bool m_realizePopupSet = false;

    [Serializable]
    public class CharacterInfo
    {
        public Character m_character;
        public GameObject portrait;
        public GameObject m_modelObj;
    }

    [Serializable]
    public class BulletPos
    {
        public Vector3[] m_bluewillBulletPos;
        public Vector3[] m_avanBulletPos;
        public Vector3[] m_sonaBulletPos;
        public Vector3[] m_potyBulletPos;
        public Vector3[] m_bloodBulletPos;

        public Vector3[] m_bluewillDoublePos;
        public Vector3[] m_avanDoublePos;
        public Vector3[] m_sonaDoublePos;
        public Vector3[] m_potyDoublePos;
        public Vector3[] m_bloodDoublePos;
    }

    public GameState m_nowGame;

    /// <summary>
    /// 이번턴에 나올 카드
    /// </summary>
    public CardState m_nowCard;

    /// <summary>
    /// 이번턴이 버프카드라면 버프종류
    /// </summary>
    public CardBuffState m_nowBuffCard;

    /// <summary>
    /// 선택한 카드
    /// </summary>
    public GameObject m_selectCard;

    public string m_selectCardSpriteName;

    /// <summary>
    /// 내 정보
    /// </summary>
    [SerializeField]
    public CharacterInfo m_playerInfo;

    /// <summary>
    /// 상대 정보 
    /// </summary>
    [SerializeField]
    public CharacterInfo m_enemyInfo;


    public BulletPos m_bulletPos;

    /// <summary>
    /// 모델링 이름
    /// </summary>
    public string[] m_contentName;

    /// <summary>
    /// 와치카 모델컨텐츠
    /// </summary>
    public GameObject[] m_copyContent;

    public string m_groundName;
    public GameObject m_ground;

    /// <summary>
    /// 와치카 상위 오브젝트(타겟매니저의 컨텐츠오브젝트 역할)
    /// </summary>
    public GameObject m_contentRoot;

    public bool doubleAttack = false;

    public bool shield = false;

    /// <summary>
    /// 모델링 초기화를 위한 좌표값
    /// </summary>
    private Vector3 m_carFirstPos;

    /// <summary>
    /// 모델링 초기화를 위한 크기값
    /// </summary>
    private Vector3 m_carFirstScale;

    private Coroutine m_hpBarCoroutine;

    private Coroutine m_battleAniCoroutine;

    private Coroutine m_portraitMoveCoroutine;

    private Coroutine m_cardOpenCoroutine;

    private Coroutine m_battleModelAniCoroutine;

    public static WatchCarBattleManager watchCarBattleManager;
    
    void Awake()
    {
        watchCarBattleManager = this;
    }

    void Start()
    {
        WatchCarBattleInit();
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            LaserBulletLaunch();
        }
    }
    /// <summary>
    /// 와치카 배틀 모든 부분 초기화
    /// </summary>
    public void WatchCarBattleInit()
    {
        WatchCarBattleUI.watcarBattleUI.WatchCarBattleUiInit();
        CarInit();

        WatchCarTargetScan(Character.none, Character.none);
        m_nowGame = GameState.none;
        m_nowCard = CardState.none;
        m_nowBuffCard = CardBuffState.none;

        //UiEventLinkManager.uieventmanager.rotateUI.SetActive(true);

        TargetManager.타깃메니저.HideAllModelingContents();
        TargetManager.타깃메니저.첫인식상태 = false;

        m_battlePlaying = false;
        m_realizePopupSet = false;
    }

    /// <summary>
    /// 맨처음 카드 보여줄때(카드: scale, 프로필: alpha 값 조정)
    /// </summary>
    public void WatchCarBattleGameInit()
    {
        //카드 나올때 battle ui 알파값 조정
        WatchCarBattleUI.watcarBattleUI.WatchCarBattleCardUiInit();
        WatchCarBattleUI.watcarBattleUI.WatchCarBattleProfileUiInit();
        CardInit();
    }

    /// <summary>
    /// 플레이중 초기화(다음 카드 선택 또는 게임끝 확인)
    /// </summary>
    public void WatchCarBattlePlayingInit()
    {
        //m_selectCard = null;
        WatchCarBattleUI.watcarBattleUI.WatchCarBattlePlayingUiInit();
        m_nowGame = GameState.none;
        CardInit();
    }

    /// <summary>
    /// 인식시 내캐릭터 및 상대캐릭터 설정
    /// </summary>
    /// <param name="player"></param>
    /// <param name="enemy"></param>
    public void WatchCarTargetScan(Character player, Character enemy)
    {
        m_playerInfo.m_character = player;
        m_enemyInfo.m_character = enemy;

        if (m_playerInfo.m_character != Character.none && m_enemyInfo.m_character != Character.none)
        {
            m_playerInfo.m_modelObj = m_copyContent[(int)m_playerInfo.m_character - 1];
            m_enemyInfo.m_modelObj = m_copyContent[(int)m_enemyInfo.m_character - 1];
        }
    }

    public void WatchCarTargetScanRandom()
    {
        m_playerInfo.m_character = (Character)UnityEngine.Random.Range(1, 6); //player;

        if(m_playerInfo.m_character != Character.blood)
        {
            m_enemyInfo.m_character = Character.blood;
        }
        else
        {
            m_enemyInfo.m_character = Character.bluewill;
        }


        if (m_playerInfo.m_character != Character.none && m_enemyInfo.m_character != Character.none)
        {
            m_playerInfo.m_modelObj = m_copyContent[(int)m_playerInfo.m_character - 1];
            m_enemyInfo.m_modelObj = m_copyContent[(int)m_enemyInfo.m_character - 1];
        }
    }

    /// <summary>
    /// 게임상태 플레이중으로 변경
    /// </summary>
    public void WatchCarBattleStart()
    {
        m_battlePlaying = true;
        PlyerCharacterMove();
    }

    /// <summary>
    /// 와치카 설정 초기화(위치, 크기)
    /// </summary>
    private void CarInit()
    {
        m_contentRoot.transform.parent = TargetManager.타깃메니저.비인식후_좌표오브젝트.transform;
        m_contentRoot.transform.localEulerAngles = TargetManager.타깃메니저.비인식후_회전값;
        m_contentRoot.transform.localPosition = new Vector3(TargetManager.타깃메니저.비인식후_좌표값.x,
                                                            0,
                                                            TargetManager.타깃메니저.비인식후_좌표값.z);
        m_contentRoot.transform.localScale = new Vector3(12, 12, 12);

        for(int i=0; i< m_copyContent.Length; i++)
        {
            m_copyContent[i].transform.localScale = Vector3.one;
            m_copyContent[i].transform.localEulerAngles = Vector3.zero;
            m_copyContent[i].transform.localPosition = new Vector3(0, -0.25f, 0);
            m_copyContent[i].SetActive(false);
        }

        WatchCarBattleAniManager.watchBattleAniManager.BattleModelInit();
        m_contentRoot.SetActive(false);
    }

    
    public void PortraitMoveStart()
    {
        PortraitMoveCoroutineStart();
    }
    private void PortraitMoveCoroutineStart()
    {
        PortraitMoveCoroutineStop();
        m_portraitMoveCoroutine = StartCoroutine(PortraitMoveSet());
    }

    private void PortraitMoveCoroutineStop()
    {
        if(m_portraitMoveCoroutine != null)
        {
            StopCoroutine(m_portraitMoveCoroutine);
            m_portraitMoveCoroutine = null;
        }
    }

    /// <summary>
    /// 배틀시작 버튼 클릭시 캐릭터 사라지는 부분
    /// </summary>
    private void PlyerCharacterMove()
    {
        TweenManager.tween_Manager.AddTweenScale(TargetManager.타깃메니저.컨텐츠최상위오브젝트,
                                                    TargetManager.타깃메니저.비인식후_사이즈값,
                                                    Vector3.zero,
                                                    0.4f, UITweener.Style.Once, TweenManager.tween_Manager.animationCurveList[0]);

        TweenManager.tween_Manager.TweenScale(TargetManager.타깃메니저.컨텐츠최상위오브젝트);

        PortraitMoveStart();
    }

    /// <summary>
    /// 플레이어 초상화 position 변경부분
    /// </summary>
    private void PlayerPortraitMoveStart()
    {
        GameObject playerPortrait = WatchCarBattleUI.watcarBattleUI.m_battleSettingUI.m_playerPortrait;
        Vector3 playerPortraitPos = WatchCarBattleUI.watcarBattleUI.m_battleSettingUI.m_playerPortraitFirstPos;

        TweenManager.tween_Manager.TweenAllDestroy(playerPortrait);

        TweenManager.tween_Manager.AddTweenPosition(playerPortrait, playerPortraitPos,
                                                  new Vector3(playerPortraitPos.x + (playerPortrait.GetComponent<UIWidget>().width - 5),
                                                              playerPortraitPos.y,
                                                              playerPortraitPos.z),
                                                              0.2f);

        TweenManager.tween_Manager.AddTweenAlpha(playerPortrait, 0, 1, 0.1f);

        TweenManager.tween_Manager.TweenPosition(playerPortrait);
        TweenManager.tween_Manager.TweenAlpha(playerPortrait);
    }

    /// <summary>
    /// 상대 초상화 position 변경부분
    /// </summary>
    private void EnemyPortraitMoveStart()
    {
        GameObject enemyPortrait = WatchCarBattleUI.watcarBattleUI.m_battleSettingUI.m_enemyPortrait;
        Vector3 enemyPortraitPos = WatchCarBattleUI.watcarBattleUI.m_battleSettingUI.m_enemyPortraitFirstPos;

        TweenManager.tween_Manager.TweenAllDestroy(enemyPortrait);

        TweenManager.tween_Manager.AddTweenPosition(enemyPortrait, enemyPortraitPos,
                                                new Vector3(enemyPortraitPos.x - (enemyPortrait.GetComponent<UIWidget>().width - 5),
                                                            enemyPortraitPos.y,
                                                            enemyPortraitPos.z),
                                                            0.2f);

        TweenManager.tween_Manager.AddTweenAlpha(enemyPortrait, 0, 1, 0.1f);

        TweenManager.tween_Manager.TweenPosition(enemyPortrait);
        TweenManager.tween_Manager.TweenAlpha(enemyPortrait);
    }

    /// <summary>
    /// Portrait versus 보이기
    /// </summary>
    private void VersusMoveStart()
    {
        GameObject versus = WatchCarBattleUI.watcarBattleUI.m_battleSettingUI.m_versus;

        //플레이어 차량 크기 변경
        TweenManager.tween_Manager.AddTweenScale(versus,
                                                  Vector3.zero,
                                                  Vector3.one,
                                                  0.3f, UITweener.Style.Once, TweenManager.tween_Manager.scaleAnimationCurve);

        TweenManager.tween_Manager.TweenScale(versus);
    }

    /// <summary>
    /// Portrait versus 숨김
    /// </summary>
    private void PortraitVersusHide()
    {
        Debug.Log("돌아감");
        GameObject playerPortrait = WatchCarBattleUI.watcarBattleUI.m_battleSettingUI.m_playerPortrait;
        Vector3 playerPortraitPos = WatchCarBattleUI.watcarBattleUI.m_battleSettingUI.m_playerPortraitFirstPos;

        GameObject enemyPortrait = WatchCarBattleUI.watcarBattleUI.m_battleSettingUI.m_enemyPortrait;
        Vector3 enemyPortraitPos = WatchCarBattleUI.watcarBattleUI.m_battleSettingUI.m_enemyPortraitFirstPos;
        
        GameObject versus = WatchCarBattleUI.watcarBattleUI.m_battleSettingUI.m_versus;

        TweenManager.tween_Manager.TweenAllDestroy(playerPortrait);
        TweenManager.tween_Manager.TweenAllDestroy(enemyPortrait);
        TweenManager.tween_Manager.TweenAllDestroy(versus);

        TweenManager.tween_Manager.AddTweenPosition(playerPortrait, 
                                                    playerPortrait.transform.localPosition,
                                                    playerPortraitPos,
                                                    0.3f);

        TweenManager.tween_Manager.AddTweenPosition(enemyPortrait,
                                                    enemyPortrait.transform.localPosition,
                                                    enemyPortraitPos,
                                                    0.3f);

        TweenManager.tween_Manager.AddTweenScale(versus,
                                                  Vector3.one,
                                                  Vector3.zero,
                                                  0.4f, UITweener.Style.Once, TweenManager.tween_Manager.animationCurveList[0]);


        TweenManager.tween_Manager.AddTweenAlpha(playerPortrait, 1, 0, 0.3f);
        TweenManager.tween_Manager.AddTweenAlpha(enemyPortrait, 1, 0, 0.3f);

        TweenManager.tween_Manager.TweenPosition(playerPortrait);
        TweenManager.tween_Manager.TweenPosition(enemyPortrait);

        TweenManager.tween_Manager.TweenAlpha(playerPortrait);
        TweenManager.tween_Manager.TweenAlpha(enemyPortrait);

        TweenManager.tween_Manager.TweenScale(versus);
    }

    /// <summary>
    /// 플레이어 차량 등장
    /// </summary>
    private void MyCarAppear()
    {
        CarInit();
        PortraitVersusHide();

        if (m_playerInfo.m_character != Character.none)
        {
            //플레이어 차량 위치 변경
            TweenManager.tween_Manager.AddTweenPosition(m_playerInfo.m_modelObj,
                                                        Vector3.zero,
                                                        new Vector3(-0.33f, -0.053f, -0.064f),
                                                        0.4f);
            //플레이어 차량 크기 변경
            TweenManager.tween_Manager.AddTweenScale(m_playerInfo.m_modelObj,
                                                      Vector3.zero,
                                                      Vector3.one,
                                                      0.6f, UITweener.Style.Once, TweenManager.tween_Manager.scaleAnimationCurve);
            //플레이어 차량 회전
            TweenManager.tween_Manager.AddTweenRotation(m_playerInfo.m_modelObj,
                                                        Vector3.zero,//new Vector3(0, -0.25f, 0),
                                                        new Vector3(3, -150f, 0),
                                                        0.8f);

            m_playerInfo.m_modelObj.SetActive(true);

            TweenManager.tween_Manager.TweenPosition(m_playerInfo.m_modelObj);
            TweenManager.tween_Manager.TweenScale(m_playerInfo.m_modelObj);
            TweenManager.tween_Manager.TweenRotation(m_playerInfo.m_modelObj);

        }

        EnemyCarAppear();
    }

    /// <summary>
    /// 상대 차량 등장
    /// </summary>
    private void EnemyCarAppear()
    {
        if (m_enemyInfo.m_character != Character.none)
        {
            //상대 차량 위치 변경
            TweenManager.tween_Manager.AddTweenPosition(m_enemyInfo.m_modelObj,
                                                       Vector3.zero,
                                                       new Vector3(0.677f, 0.006f, 0.839f),
                                                       0.4f);
            //상대 차량 크기 변경
            TweenManager.tween_Manager.AddTweenScale(m_enemyInfo.m_modelObj,
                                                      Vector3.zero,
                                                      Vector3.one,
                                                      0.6f, UITweener.Style.Once, TweenManager.tween_Manager.scaleAnimationCurve);
            //상대 차량 회전
            TweenManager.tween_Manager.AddTweenRotation(m_enemyInfo.m_modelObj,
                                                      Vector3.zero,//new Vector3(0, -0.25f, 0),
                                                      new Vector3(0, 45f, 0),
                                                      0.8f);

            m_enemyInfo.m_modelObj.SetActive(true);

            TweenManager.tween_Manager.TweenPosition(m_enemyInfo.m_modelObj);
            TweenManager.tween_Manager.TweenScale(m_enemyInfo.m_modelObj);
            TweenManager.tween_Manager.TweenRotation(m_enemyInfo.m_modelObj);

            m_contentRoot.SetActive(true);
            m_realizePopupSet = true;

            WatchCarBattleGameInit();
            GroundAppear();
        }
    }

    private void GroundAppear()
    {
        //바닥 위치
        TweenManager.tween_Manager.AddTweenPosition(m_ground, Vector3.zero,new Vector3(1.4f, -0.1f, 0), 0.4f);

        //바닥 크기 변경
        TweenManager.tween_Manager.AddTweenScale(m_ground,
                                                  Vector3.zero,
                                                  Vector3.one,
                                                  0.6f, UITweener.Style.Once, TweenManager.tween_Manager.scaleAnimationCurve);
        //바닥 회전
        TweenManager.tween_Manager.AddTweenRotation(m_ground,
                                                    new Vector3(-86.0f, -135.0f, 180.0f), 
                                                    new Vector3(-86.0f, -135.0f, 180.0f), 0.8f);

        m_ground.SetActive(true);

        TweenManager.tween_Manager.TweenPosition(m_ground);
        TweenManager.tween_Manager.TweenScale(m_ground);
        TweenManager.tween_Manager.TweenRotation(m_ground);
        WatchCarEffectManager.watchCarEffectManager.MyCarCheckEffectCall();
    }

    /// <summary>
    /// 카드 랜덤 선택
    /// </summary>
    private void CardInit()
    {
        WatchCarBattleUI.BattleUI battleUI = WatchCarBattleUI.watcarBattleUI.m_battleUI;

        m_nowCard = CardState.none;
        m_nowBuffCard = CardBuffState.none;

        battleUI.m_battleCardList[0].GetComponent<WatchCarCardInfo>().m_cardState = CardState.success;
        battleUI.m_battleCardList[1].GetComponent<WatchCarCardInfo>().m_cardState = CardState.fail;
        battleUI.m_battleCardList[2].GetComponent<WatchCarCardInfo>().m_cardState = CardState.fail;
        battleUI.m_battleCardList[3].GetComponent<WatchCarCardInfo>().m_cardState = CardState.buff;

        int nowNum = UnityEngine.Random.Range(0, 4);

        m_nowCard = battleUI.m_battleCardList[nowNum].GetComponent<WatchCarCardInfo>().m_cardState;

        if (m_nowCard == CardState.buff)
        {
            m_nowBuffCard = (CardBuffState)UnityEngine.Random.Range(1, 4);
        }

        CardSpriteSet();
    }

    /// <summary>
    /// 카드 설정에 따른 스프라이트 결정
    /// </summary>
    private void CardSpriteSet()
    {
        switch (m_nowCard)
        {
            case CardState.success:
                m_selectCardSpriteName = "card_attack";
                WatchCarBattleUI.watcarBattleUI.WatchCarCardPosInit();
                break;

            case CardState.fail:
                m_selectCardSpriteName = "card_under_attack";
                WatchCarBattleUI.watcarBattleUI.WatchCarCardPosInit();
                break;

            case CardState.buff:
                BuffCardSpriteSet();
                break;

            default:
                Debug.Log("카드 설정 확인 바랍니다.");
                return;
        }
    }

    /// <summary>
    /// 카드가 버프카드일 경우
    /// </summary>
    private void BuffCardSpriteSet()
    {
        switch(m_nowBuffCard)
        {
            case CardBuffState.doubleAttack:
                m_selectCardSpriteName = "card_double_attack";
                break;

            case CardBuffState.shield:
                m_selectCardSpriteName = "card_shield";
                break;

            case CardBuffState.heal:
                m_selectCardSpriteName = "card_recovery";
                break;

            default:
                Debug.Log("버프카드 설정 확인 바랍니다.");
                return;
        }

        WatchCarBattleUI.watcarBattleUI.WatchCarCardPosInit();
    }

    /// <summary>
    /// 카드 클릭 -> 선택카드가 중앙부분으로 이동
    /// </summary>
    public void CardMove(GameObject obj)
    {
        m_selectCard = obj;

        int cardNum = 0;
        float tweenTime = 0.6f;

        for (int i =0; i< WatchCarBattleUI.watcarBattleUI.m_battleUI.m_battleCardList.Length; i++)
        {
            if(WatchCarBattleUI.watcarBattleUI.m_battleUI.m_battleCardList[i] == obj.transform.parent.gameObject)
            {
                cardNum = i;
                obj.transform.parent.GetComponent<UIWidget>().depth = 3;
                obj.transform.GetComponent<UIWidget>().depth = 4;
                break;
            }
        }

        TweenManager.tween_Manager.AddTweenPosition(obj.transform.parent.gameObject, WatchCarBattleUI.watcarBattleUI.m_battleUI.m_battleCardPos[cardNum],
                                                    Vector3.zero, tweenTime);

        TweenManager.tween_Manager.AddTweenScale(obj.transform.parent.gameObject, 
                                                    WatchCarBattleUI.watcarBattleUI.m_battleUI.m_battleCardList[cardNum].transform.localScale,
                                                    WatchCarBattleUI.watcarBattleUI.m_battleUI.m_battleCardList[cardNum].transform.localScale *1.5f,
                                                    tweenTime, 
                                                    UITweener.Style.Once, 
                                                    TweenManager.tween_Manager.animationCurveList[2]);
        TweenManager.tween_Manager.AddTweenRotation(obj.transform.parent.gameObject,
                                                    Vector3.zero,
                                                    new Vector3(0, 1080, 0),
                                                    tweenTime);

        TweenManager.tween_Manager.TweenPosition(obj.transform.parent.gameObject);
        TweenManager.tween_Manager.TweenScale(obj.transform.parent.gameObject);
        TweenManager.tween_Manager.TweenRotation(obj.transform.parent.gameObject);

        CardOpenCoroutineStart();
    }

    public void LaserBulletLaunch()
    {
        GameObject bullet = null;
        GameObject startObj = null;
        GameObject tartgetObj = null;

        Character attackCar = Character.none;

        Vector3[] bulletPos = null;
        Vector3 bulletScale = new Vector3(0.6f, 0.6f, 0);

        bool porty = false;

        if (m_nowCard == CardState.buff)
        {
            if (m_nowBuffCard != CardBuffState.doubleAttack)
            {
                Debug.Log("힐, 방어막 버프는 발사안함");
                return;
            }
        }
        if (m_nowCard == CardState.success)
        {
            startObj = m_playerInfo.m_modelObj;
            tartgetObj = m_enemyInfo.m_modelObj;
            attackCar = m_playerInfo.m_character;
        }
        else if(m_nowCard == CardState.fail)
        {
            startObj = m_enemyInfo.m_modelObj;
            tartgetObj = m_playerInfo.m_modelObj;
            attackCar = m_enemyInfo.m_character;
        }
        else if(m_nowBuffCard == CardBuffState.doubleAttack)
        {
            startObj = m_playerInfo.m_modelObj;
            tartgetObj = m_enemyInfo.m_modelObj;
            attackCar = m_playerInfo.m_character;
        }
        else
        {
            Debug.Log("현재 카드가 none 레이저 발사 안함");
            return;
        }

        switch (attackCar)
        {
            case Character.bluewill:
                if(!doubleAttack)
                {
                    bulletPos = m_bulletPos.m_bluewillBulletPos;
                }
                else
                {
                    if(attackCar == m_playerInfo.m_character)
                    {
                        bulletPos = m_bulletPos.m_bluewillDoublePos;
                    }
                    else
                    {
                        bulletPos = m_bulletPos.m_bluewillBulletPos;
                    }
                }
                break;

            case Character.avan:
                if (!doubleAttack)
                {
                    bulletPos = m_bulletPos.m_avanBulletPos;
                }
                else
                {
                    if (attackCar == m_playerInfo.m_character)
                    {
                        bulletPos = m_bulletPos.m_avanDoublePos;
                    }
                    else
                    {
                        bulletPos = m_bulletPos.m_avanBulletPos;
                    }
                }
                break;

            case Character.sona:
                if (!doubleAttack)
                {
                    bulletPos = m_bulletPos.m_sonaBulletPos;
                }
                else
                {
                    if (attackCar == m_playerInfo.m_character)
                    {
                        bulletPos = m_bulletPos.m_sonaDoublePos;
                    }
                    else
                    {
                        bulletPos = m_bulletPos.m_sonaBulletPos;
                    }
                }
                break;

            case Character.porty:

                porty = true;

                if (!doubleAttack)
                {
                    bulletPos = m_bulletPos.m_potyBulletPos;
                }
                else
                {
                    if (attackCar == m_playerInfo.m_character)
                    {
                        bulletPos = m_bulletPos.m_potyDoublePos;
                    }
                    else
                    {
                        bulletPos = m_bulletPos.m_potyBulletPos;
                    }
                }
                break;

            case Character.blood:
                if (!doubleAttack)
                {
                    bulletPos = m_bulletPos.m_bloodBulletPos;
                }
                else
                {
                    if (attackCar == m_playerInfo.m_character)
                    {
                        bulletPos = m_bulletPos.m_bloodDoublePos;
                    }
                    else
                    {
                        bulletPos = m_bulletPos.m_bloodBulletPos;
                    }
                }
                break;
        }

        if (!porty)
        {
            for (int i = 0; i < bulletPos.Length; i++)
            {
                bullet = Instantiate(WatchCarEffectManager.watchCarEffectManager.m_effectInfoObj.GetComponent<ParticleInfo>().m_watchCarBattle.m_railGun, Vector3.zero, Quaternion.identity) as GameObject;
                bullet.transform.parent = startObj.transform;
                bullet.transform.localPosition = bulletPos[i];
                bullet.transform.localScale = bulletScale;
                bullet.transform.LookAt(tartgetObj.transform);

                DoubleRailGunSet(bullet);

                if (attackCar == m_playerInfo.m_character)
                {
                    bullet.transform.localEulerAngles = new Vector3(bullet.transform.localEulerAngles.x - 1.6f, bullet.transform.localEulerAngles.y, bullet.transform.localEulerAngles.z);
                }
                else
                {
                    bullet.transform.localEulerAngles = new Vector3(bullet.transform.localEulerAngles.x - 1.45f, bullet.transform.localEulerAngles.y, bullet.transform.localEulerAngles.z);
                }

                bullet.name = bullet.name.Split('(')[0];
                bullet.SetActive(true);
                bullet.AddComponent<TriggerDestroy>();
                bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * 2000.0f);
            }
        }
        else
        {
            for (int i = 0; i < bulletPos.Length; i++)
            {
                bullet = Instantiate(WatchCarEffectManager.watchCarEffectManager.m_effectInfoObj.GetComponent<ParticleInfo>().m_watchCarBattle.m_missile, Vector3.zero, Quaternion.identity) as GameObject;
                bullet.transform.parent = startObj.transform;
                bullet.transform.localPosition = bulletPos[i];
                if (doubleAttack)
                {
                    bullet.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                }
                else
                {
                    bullet.transform.localScale = Vector3.one;
                }

                bullet.transform.LookAt(tartgetObj.transform);

                if (attackCar == m_playerInfo.m_character)
                {
                    bullet.transform.localEulerAngles = new Vector3(bullet.transform.localEulerAngles.x - 1.6f, bullet.transform.localEulerAngles.y, bullet.transform.localEulerAngles.z);
                }
                else
                {
                    bullet.transform.localEulerAngles = new Vector3(bullet.transform.localEulerAngles.x - 1.45f, bullet.transform.localEulerAngles.y, bullet.transform.localEulerAngles.z);
                }

                bullet.name = bullet.name.Split('(')[0];
                bullet.SetActive(true);
                bullet.AddComponent<TriggerDestroy>();
                bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * 2000.0f);
            }
        }
    }

    private void DoubleRailGunSet(GameObject bullet)
    {
        if (m_nowCard != CardState.fail && doubleAttack)
        {
            Transform[] tran = bullet.GetComponentsInChildren<Transform>(true);

            // 하위 레이어까지 전부 변경
            foreach (Transform child in tran)
            {
                if (child.gameObject.GetComponent<ParticleSystem>() != null)
                {
                    child.gameObject.GetComponent<ParticleSystem>().startSize += child.gameObject.GetComponent<ParticleSystem>().startSize;
                }
            }
        }
    }

    /// <summary>
    /// 카드 클릭 오픈 코루틴
    /// </summary>
    private void CardOpenCoroutineStart()
    {
        CardOpenCoroutineStop();
        m_cardOpenCoroutine = StartCoroutine(CardPOpenSet());
    }

    /// <summary>
    /// 카드 오픈 코루틴 stop
    /// </summary>
    private void CardOpenCoroutineStop()
    {
        if (m_cardOpenCoroutine != null)
        {
            StopCoroutine(m_cardOpenCoroutine);
            m_cardOpenCoroutine = null;
        }
    }

    /// <summary>
    /// 중앙으로 이동한 카드가 열림
    /// </summary>
    private void CardOpen()
    {
        if (m_selectCard != null)
        {
            TweenManager.tween_Manager.AddTweenAlpha(m_selectCard, 1, 0, 0.3f);
            TweenManager.tween_Manager.TweenAlpha(m_selectCard);
            CardApply();
        }
        else
        {
            Debug.Log("선택 카드가 없습니다 선택카드 부분 확인 바람");
        }
    }

    /// <summary>
    /// 카드 종류에 따른 실행 부분 호출
    /// </summary>
    private void CardApply()
    {
        switch (m_nowCard)
        {
            case CardState.success:
                SuccessFailCardApply();
                break;

            case CardState.fail:
                SuccessFailCardApply();
                break;

            case CardState.buff:
                BuffCardApply();
                SuccessFailCardApply();
                break;

            default:
                Debug.Log("카드 설정 확인 바랍니다.");
                return;
        }
    }

    /// <summary>
    /// 카드 상태에 따른 오픈 딜레이 코루틴
    /// </summary>
    private void SuccessFailCardApply()
    {
        BattleModelAniCoroutineStart();
        //HpBarCoroutineStart();
    }

    /// <summary>
    /// 버프카드일 경우 실행 부분
    /// </summary>
    private void BuffCardApply()
    {
        int buffNum = 0;

       // WatchCarBattleUI.watcarBattleUI.BuffEffectMove();

        for (int i = 0; i < WatchCarBattleUI.watcarBattleUI.m_profileUI.m_playerinfo.m_buffUiList.Length; i++)
        {
            if (WatchCarBattleUI.watcarBattleUI.m_profileUI.m_playerinfo.m_buffUiList[i] == CardBuffState.none
                || WatchCarBattleUI.watcarBattleUI.m_profileUI.m_playerinfo.m_buffUiList[i] == m_nowBuffCard)
            {
                buffNum = i;
                break;
            }
            else if(i == WatchCarBattleUI.watcarBattleUI.m_profileUI.m_playerinfo.m_buffUiList.Length -1)
            {
                Debug.Log("버프가 가특참");
            }
        }

        if (m_nowBuffCard == CardBuffState.shield)
        {
            WatchCarBattleUI.watcarBattleUI.m_profileUI.m_playerinfo.m_buffUiList[buffNum] = m_nowBuffCard;
        }

        /*
        switch (m_nowBuffCard)
        {
            case CardBuffState.doubleAttack:
                WatchCarBattleUI.watcarBattleUI.m_profileUI.m_playerinfo.m_buffObjList[buffNum].GetComponent<UISprite>().spriteName = "buff_double_attack";
                WatchCarEffectManager.watchCarEffectManager.UiBuffDoubleAttackEffect(WatchCarBattleUI.watcarBattleUI.m_profileUI.m_playerinfo.m_buffObjList[buffNum]);
                m_selectCardSpriteName = "btn_popup";
                break;

            case CardBuffState.shield:
                WatchCarBattleUI.watcarBattleUI.m_profileUI.m_playerinfo.m_buffObjList[buffNum].GetComponent<UISprite>().spriteName = "buff_shield";
                WatchCarEffectManager.watchCarEffectManager.UiBuffSheildEffect(WatchCarBattleUI.watcarBattleUI.m_profileUI.m_playerinfo.m_buffObjList[buffNum]);
                m_selectCardSpriteName = "btn_original";
                break;

            case CardBuffState.heal:
                Debug.Log("힐은 버프창에 넣지 않고 바로 적용");
                //WatchCarBattleUI.watcarBattleUI.m_profileUI.m_playerinfo.m_buffObjList[buffNum].GetComponent<UISprite>().spriteName = "CameraBack";
                //m_selectCardSpriteName = "CameraBack";
                return;

            default:
                Debug.Log("버프카드 설정 확인 바랍니다.");
                return;
        }
        
        TweenManager.tween_Manager.AddTweenScale(WatchCarBattleUI.watcarBattleUI.m_profileUI.m_playerinfo.m_buffObjList[buffNum],
                                                Vector3.zero, 
                                                Vector3.one, 
                                                0.2f, 
                                                UITweener.Style.Once, 
                                                TweenManager.tween_Manager.scaleAnimationCurve);

        TweenManager.tween_Manager.TweenScale(WatchCarBattleUI.watcarBattleUI.m_profileUI.m_playerinfo.m_buffObjList[buffNum]);
        WatchCarBattleUI.watcarBattleUI.m_profileUI.m_playerinfo.m_buffObjList[buffNum].SetActive(true);
        */
    }

    /// <summary>
    /// hp바 증감 코루틴
    /// </summary>
    private void HpBarCoroutineStart()
    {
        HpBarCoroutineStop();
        m_hpBarCoroutine = StartCoroutine(HpBarControl());
    }

    /// <summary>
    /// hp바 증감 코루틴 stop
    /// </summary>
    private void HpBarCoroutineStop()
    {
        if (m_hpBarCoroutine != null)
        {
            StopCoroutine(m_hpBarCoroutine);
            m_hpBarCoroutine = null;
        }
    }

    /// <summary>
    /// 애니메이션 이펙트 자동 설정
    /// </summary>
    /// <param name="obj">설정 차량 오브젝트</param>
    private void BattleAutoAniSet(GameObject obj)
    {
        Debug.Log("m_nowCard : " + m_nowCard);
        if (obj != null)
        {
            switch (m_nowCard)
            {
                case CardState.success:
                    if (obj == m_playerInfo.m_modelObj)
                    {
                        WatchCarBattleAniManager.watchBattleAniManager.BattleModelAttack(m_playerInfo.m_modelObj);
                        WatchCarEffectManager.watchCarEffectManager.PlayerNormalEffectSet();
                    }
                    else if (obj == m_enemyInfo.m_modelObj)
                    {
                        WatchCarBattleAniManager.watchBattleAniManager.BattleModelUnderAttack(m_enemyInfo.m_modelObj);
                    }
                    break;

                case CardState.fail:
                    if (obj == m_playerInfo.m_modelObj)
                    {
                        WatchCarBattleAniManager.watchBattleAniManager.BattleModelUnderAttack(m_playerInfo.m_modelObj);
                        WatchCarEffectManager.watchCarEffectManager.PlayerNormalEffectSet();
                    }
                    else if (obj == m_enemyInfo.m_modelObj)
                    {
                        WatchCarBattleAniManager.watchBattleAniManager.BattleModelAttack(m_enemyInfo.m_modelObj);
                        WatchCarEffectManager.watchCarEffectManager.EnemyNormalEffectSet();
                    }
                    break;

                case CardState.buff:
                    if (obj == m_playerInfo.m_modelObj)
                    {
                        if (m_nowBuffCard == CardBuffState.doubleAttack)
                        {
                            WatchCarBattleAniManager.watchBattleAniManager.BattleModelDoubleAttack(m_playerInfo.m_modelObj);
                            WatchCarEffectManager.watchCarEffectManager.PlayerNormalEffectSet();
                        }
                        else
                        {
                            WatchCarBattleAniManager.watchBattleAniManager.BattleModelSkill(m_playerInfo.m_modelObj);
                            WatchCarEffectManager.watchCarEffectManager.BuffEffectSet();
                        }
                    }
                    else if (obj == m_enemyInfo.m_modelObj)
                    {
                        if (m_nowBuffCard == CardBuffState.doubleAttack)
                        {
                            WatchCarBattleAniManager.watchBattleAniManager.BattleModelUnderAttack(m_enemyInfo.m_modelObj);
                        }
                        else
                        {
                            WatchCarBattleAniManager.watchBattleAniManager.BattleModelIdle(m_enemyInfo.m_modelObj);
                        }
                    }
                    break;
            }
        }
    }


    public void BattleModelAniCoroutineStart()
    {
        BattleModelAniCoroutineStop();
        m_battleModelAniCoroutine = StartCoroutine(BattleAniSet());
    }

    private void BattleModelAniCoroutineStop()
    {
        if (m_battleModelAniCoroutine != null)
        {
            StopCoroutine(m_battleModelAniCoroutine);
            m_battleModelAniCoroutine = null;
        }
    }

    private void BattleOutCome()
    {
        WatchCarBattleUI.watcarBattleUI.WatchCarCardActive(false);

        if (WatchCarBattleUI.watcarBattleUI.m_profileUI.m_playerinfo.m_hp <=0)
        {
            m_nowGame = GameState.lose;
            WatchCarBattleAniManager.watchBattleAniManager.BattleModelVictory(m_enemyInfo.m_modelObj);
            Debug.Log("패배");
            
        }
        else if(WatchCarBattleUI.watcarBattleUI.m_profileUI.m_enemyinfo.m_hp <= 0)
        {
            m_nowGame = GameState.win;
            WatchCarBattleAniManager.watchBattleAniManager.BattleModelVictory(m_playerInfo.m_modelObj);
            Debug.Log("승리");
        }
        WatchCarBattleSoundManager.watchcarSoundManager.WinLoseSoundCall();
        WatchCarBattleUI.watcarBattleUI.ResultUiPopUpCall();
    }

    /// <summary>
    /// hp바 증가 감소
    /// </summary>
    private IEnumerator HpBarControl()
    {
        //적용할 hpbar
        GameObject hpbar = null;

        int doubleIndex = -1;
        int shieldIndex = -1;

        //증가, 감소 이후 hpbar vlaue 값
        float afterHpValue = 0;

        //증가, 감소 할 수치
        float hitDamage = 0;

        //true : 증가, false :감소
        bool variation = false;

        for (int i = 0; i < WatchCarBattleUI.watcarBattleUI.m_profileUI.m_playerinfo.m_buffUiList.Length; i++)
        {
            if(WatchCarBattleUI.watcarBattleUI.m_profileUI.m_playerinfo.m_buffUiList[i] == CardBuffState.shield)
            {
                shieldIndex = i;
                shield = true;
            }
        }

        if (m_nowCard == CardState.success)
        {
            variation = false;

            hpbar = WatchCarBattleUI.watcarBattleUI.m_profileUI.m_enemyinfo.m_hpBar;

            hitDamage = WatchCarBattleUI.watcarBattleUI.m_profileUI.m_playerinfo.m_power
                / WatchCarBattleUI.watcarBattleUI.m_profileUI.m_enemyinfo.m_normalHp;

            WatchCarBattleUI.watcarBattleUI.m_profileUI.m_playerinfo.m_power =
               WatchCarBattleUI.watcarBattleUI.m_profileUI.m_playerinfo.m_normalPower;

            afterHpValue = hpbar.GetComponent<UISlider>().value - hitDamage;

            WatchCarBattleUI.watcarBattleUI.m_profileUI.m_enemyinfo.m_hp
                -= WatchCarBattleUI.watcarBattleUI.m_profileUI.m_playerinfo.m_power;
        }
        else if (m_nowCard == CardState.fail)
        {
            variation = false;

            hpbar = WatchCarBattleUI.watcarBattleUI.m_profileUI.m_playerinfo.m_hpBar;

            if (shield)
            {
                hitDamage = 0;
                WatchCarBattleUI.watcarBattleUI.m_profileUI.m_enemyinfo.m_power = 0;

                WatchCarBattleUI.watcarBattleUI.m_profileUI.m_playerinfo.m_buffUiList[shieldIndex] = CardBuffState.none;
               // WatchCarBattleUI.watcarBattleUI.m_profileUI.m_playerinfo.m_buffObjList[shieldIndex].SetActive(false);
            }
            else
            {
                WatchCarBattleUI.watcarBattleUI.m_profileUI.m_enemyinfo.m_power = 
                    WatchCarBattleUI.watcarBattleUI.m_profileUI.m_enemyinfo.m_normalPower;

                hitDamage = WatchCarBattleUI.watcarBattleUI.m_profileUI.m_enemyinfo.m_power 
                            / WatchCarBattleUI.watcarBattleUI.m_profileUI.m_playerinfo.m_normalHp;
            }

            afterHpValue = hpbar.GetComponent<UISlider>().value - hitDamage;

            WatchCarBattleUI.watcarBattleUI.m_profileUI.m_playerinfo.m_hp
               -= WatchCarBattleUI.watcarBattleUI.m_profileUI.m_enemyinfo.m_power;
        }
        else
        {
            if (m_nowBuffCard == CardBuffState.heal)
            {
                Debug.Log("힐~");
                variation = true;

                hpbar = WatchCarBattleUI.watcarBattleUI.m_profileUI.m_playerinfo.m_hpBar;
                hitDamage = WatchCarBattleUI.watcarBattleUI.m_profileUI.m_playerinfo.m_normalPower
                            / WatchCarBattleUI.watcarBattleUI.m_profileUI.m_playerinfo.m_normalHp;

                afterHpValue = hpbar.GetComponent<UISlider>().value + hitDamage;

                WatchCarBattleUI.watcarBattleUI.m_profileUI.m_playerinfo.m_hp
                    += WatchCarBattleUI.watcarBattleUI.m_profileUI.m_playerinfo.m_power;
            }
            else if (m_nowBuffCard == CardBuffState.doubleAttack)
            {
                if (doubleAttack)
                {
                    variation = false;

                    hpbar = WatchCarBattleUI.watcarBattleUI.m_profileUI.m_enemyinfo.m_hpBar;
                    hitDamage = (WatchCarBattleUI.watcarBattleUI.m_profileUI.m_playerinfo.m_normalPower) * 2
                        / WatchCarBattleUI.watcarBattleUI.m_profileUI.m_enemyinfo.m_normalHp;

                    WatchCarBattleUI.watcarBattleUI.m_profileUI.m_playerinfo.m_power =
                        WatchCarBattleUI.watcarBattleUI.m_profileUI.m_playerinfo.m_normalPower * 2;

                    afterHpValue = hpbar.GetComponent<UISlider>().value - hitDamage;

                    WatchCarBattleUI.watcarBattleUI.m_profileUI.m_enemyinfo.m_hp
                        -= WatchCarBattleUI.watcarBattleUI.m_profileUI.m_playerinfo.m_power;
                }


            }
            else 
            {
                Debug.Log("체력변동 없음");
                yield return new WaitForSeconds(1.0f);
                WatchCarBattlePlayingInit();
                yield break;
            }
        }
       
        while (true)
        {
            if (variation)
            {
                hpbar.GetComponent<UISlider>().value += Time.deltaTime;

                if(hpbar.GetComponent<UISlider>().value >= 1)
                {
                    hpbar.GetComponent<UISlider>().value = 1;
                    Debug.Log("힐~2");
                    yield return new WaitForSeconds(1.0f);
                    WatchCarBattlePlayingInit();
                    yield break;
                }
                else if(hpbar.GetComponent<UISlider>().value >= afterHpValue)
                {
                    hpbar.GetComponent<UISlider>().value = afterHpValue;
                    Debug.Log("힐~3");
                    yield return new WaitForSeconds(1.0f);
                    WatchCarBattlePlayingInit();
                    yield break;
                }
            }
            else
            {
                hpbar.GetComponent<UISlider>().value -= Time.deltaTime;

                if (hpbar.GetComponent<UISlider>().value <= 0)
                {
                    hpbar.GetComponent<UISlider>().value = 0;

                    yield return new WaitForSeconds(1.0f);
                    BattleOutCome();
                    WatchCarBattlePlayingInit();
                    //WatchCarBattlePlayingInit();
                    yield break;
                }
                else if (hpbar.GetComponent<UISlider>().value <= afterHpValue)
                {
                    hpbar.GetComponent<UISlider>().value = afterHpValue;

                    yield return new WaitForSeconds(1.0f);
                    //WatchCarBattlePlayingInit();
                    WatchCarBattlePlayingInit();
                    yield break;
                }
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator BattleAniSet()
    {
        float delayTime = 1.2f;
        GameObject firstAniObj = null;
        GameObject secondAniObj = null;

        doubleAttack = false;
        shield = false;

        switch (m_nowCard)
        {
            case CardState.success:
                firstAniObj = m_playerInfo.m_modelObj;
                secondAniObj = m_enemyInfo.m_modelObj;
                WatchCarBattleUI.watcarBattleUI.BuffEffectMove();
                break;

            case CardState.fail:
                firstAniObj = m_enemyInfo.m_modelObj;
                secondAniObj = m_playerInfo.m_modelObj;
                WatchCarBattleUI.watcarBattleUI.BuffEffectMove();
                break;

            case CardState.buff:
                firstAniObj = m_playerInfo.m_modelObj;
                secondAniObj = m_enemyInfo.m_modelObj;
                if(m_nowBuffCard == CardBuffState.doubleAttack)
                {
                    doubleAttack = true;
                    WatchCarBattleUI.watcarBattleUI.BuffEffectMove();
                }
                break;
        }

        if(m_nowBuffCard != CardBuffState.heal && m_nowBuffCard != CardBuffState.shield)
        {
            yield return new WaitForSeconds(1.0f);
            WatchCarBattleUI.watcarBattleUI.BuffEffectMoveStop();
            yield return new WaitForSeconds(0.5f);
        }
        //첫번째 오브젝트 애니메이션 실행
        BattleAutoAniSet(firstAniObj);
        WatchCarBattleSoundManager.watchcarSoundManager.SkillSoundCall();

        if (m_nowBuffCard != CardBuffState.heal && m_nowBuffCard != CardBuffState.shield)
        {
            WatchCarEffectManager.watchCarEffectManager.CallTurnEffect();
        }
        yield return new WaitForSeconds(delayTime);

        LaserBulletLaunch();
        WatchCarBattleSoundManager.watchcarSoundManager.AttackSoundCall();
        WatchCarBattleUI.watcarBattleUI.WatchCarCardPosInit();

        HpBarCoroutineStart();
        //두번째 오브젝트 애니메이션 실행
        //WatchCarEffectManager.watchCarEffectManager.BeAttackedEffect();
        yield return new WaitForSeconds(0.2f);
        //두번째 오브젝트 애니메이션 실행
        BattleAutoAniSet(secondAniObj);
        WatchCarBattleSoundManager.watchcarSoundManager.DefendSoundCall();

        yield break;
    }

    private IEnumerator PortraitMoveSet()
    {
        float delayTime = 0.4f;

        PlayerPortraitMoveStart();

        yield return new WaitForSeconds(delayTime);
        TargetManager.타깃메니저.HideAllModelingContents();
        //상대초상화 이미지 등장
        EnemyPortraitMoveStart();

        yield return new WaitForSeconds(delayTime);
        //중앙부분 vs 이미지 등장
        VersusMoveStart();
        WatchCarBattleSoundManager.watchcarSoundManager.VersusSoundCall();

        yield return new WaitForSeconds(delayTime * 8);
        //초상화가 사라지고 캐릭터가 나타나는 딜레이
        MyCarAppear();
        yield break;
    }

    private IEnumerator CardPOpenSet()
    {
        float delayTime = 0.6f;

        yield return new WaitForSeconds(delayTime);
        //카드 오픈
        CardOpen();

        yield break;
    }
}
