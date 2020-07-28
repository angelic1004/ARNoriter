using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MatchingUI : MonoBehaviour
{
    public bool talk;
    public GameObject matchingUiObj;

    public UILabel totalLabel;
    public UILabel sucessLabel;
    public UILabel remainLabel;
    public UILabel callengeLabel;
    public UILabel timeLabel;
    public UILabel insertLabel;

    public Transform cardRoot;

    public Transform cardPosRoot;

    public GameObject cardPrefab;

    public GameObject charStateObj;

    public GameObject restartUiObj;

    public enum CitySelectCard
    {
        AirPlane,
        Ambulance,
        Helicopter,
        DumpTruck,
        Excavator,
        Firetruck,
        FuitCar,
        MixerTruck,
        PoliceCar,
        SchoolBus,
        Ship,
        SportsCar,
        Taxi,
        TowTruck,
        Train,
        num
    }

    public enum AnimalSelectCard
    {
        bear,
        crocodile,
        frog,
        giraffe,
        hippo,
        hyena,
        koala,
        lion,
        monkey,
        panda,
        penguin,
        rabbit,
        rhino,
        tiger,
        zebra,
        num
    }

    public enum SeaAnimalSelectCard
    {

        beluga,
        blacktipshark,
        bluetang,
        clownfish,
        giantgrouper,
        greynurseshark,
        lionfish,
        mantaray,
        moonjellyfish,
        moray,
        napoleonfish,
        porcupinefish,
        puffer,
        raccoonfish,
        turttle,
        num
    }

    // 알파벳 enum 만들어야 함 (마지막에 num 꼭 추가)
    public enum AlphabetSelectCard
    {
        alphabet_apple,
        alphabet_bee,
        alphabet_cake,
        alphabet_dog,
        alphabet_egg,
        alphabet_frog,
        alphabet_grape,
        alphabet_house,
        alphabet_ice,
        alphabet_jetplane,
        alphabet_ketchup,
        alphabet_lion,
        alphabet_mouse,
        alphabet_night,
        alphabet_owl,
        alphabet_parrot,
        alphabet_queen,
        alphabet_robot,
        alphabet_snake,
        alphabet_train,
        alphabet_ukulele,
        alphabet_vase,
        alphabet_water,
        alphabet_xiphias,
        alphabet_yoghurt,
        alphabet_zebra,
        num
    }

    public enum DinoSelectCard
    {
        allosaurus,
        ankylosaurus,
        apatosaurus,
        brachiosaurus,
        carnotaurus,
        compsognathus,
        gallimimus,
        pachycephalosaurus,
        parasaurolophus,
        pteranodon,
        sinoceratops,
        stegosaurus,
        triceratops,
        tyrannosaurus,
        velociraptor,
        num
    }

    [Serializable]
    public class Result
    {
        public GameObject resultPopUp;

        public UILabel resultLable;

        public GameObject prevLevelBtn;
        public GameObject nextLevelBtn;

    }

    [Serializable]
    public class CharAddSpriteName
    {
        public string idle;
        public string success;
        public string fail;
        public string hint;
        public string cardBack;
    }

    [Serializable]
    public class LevelCardSet
    {
        public string charName;
        public Color32 checkColor;
        public Color32 borderColor;
    }

    [Serializable]
    public class TalkList
    {
        public string initText;
        public string sameText;
        public string successText;
        public string failText;
        public string endText;

    }

    [Serializable]
    public class LevelCardPos
    {
        public Transform[] cardPos;
    }

    [SerializeField]
    public Result result;

    [SerializeField]
    public CharAddSpriteName chaAddSprName;

    [SerializeField]
    public LevelCardSet[] levelCardSet;

    [SerializeField]
    public TalkList talkList;

    [SerializeField]
    public LevelCardPos[] cardLevelPos;

    private GameObject[] cardObj;

    private GameObject[] paircardObj;

    private CitySelectCard[] cityCardList;

    private AnimalSelectCard[] animalCardList;

    private SeaAnimalSelectCard[] seaAnimalCardList;

    private AlphabetSelectCard[] alphabetCardList;

    private DinoSelectCard[] dinoCardList;

    private Transform[] cardPosList;

    private GameObject firClickCard;

    private GameObject clickObj;
    private GameObject hintObj = null;
    private GameObject sameHintObj = null;

    private bool mousePos = false;
    private int plusDepCount = 1;

    private bool waitOver = false;

    private Coroutine viewCor;
    private Coroutine hintCor;

    private int totalNum;
    private int sucessNum;
    private int remainNum;
    private int callengeNum;

    private StringBuilder m_Builder;

    /// <summary>
    /// 텍스트 한글자씩 나오는 코루틴
    /// </summary>
    private Coroutine textCoroutine;

    private Coroutine charResetCoroutine;

    public static MatchingUI instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        m_Builder = new StringBuilder();
        m_Builder.Remove(0, m_Builder.Length);

        matchingUiObj.GetComponent<UIPanel>().alpha = 0;
        restartUiObj.GetComponent<UIPanel>().alpha = 0;

        CardPosInit();
       // LevelOneInit();
    }

    void Update()
    {
        if(mousePos)
        {
            clickObj.transform.position = new Vector3(UICamera.mainCamera.ScreenToWorldPoint(Input.mousePosition).x, 
                                                      UICamera.mainCamera.ScreenToWorldPoint(Input.mousePosition).y,
                                                      0);
        }
    }

    private void CharSprChange(string addSprName)
    {
        if (charStateObj != null)
        {
            charStateObj.GetComponent<UISprite>().spriteName = SpriteNameSumReturn(addSprName);
        }
    }

    /// <summary>
    /// 캐릭터명 + 상태명으로 스프라이트명을 리턴
    /// </summary>
    /// <param name="charName"></param>
    /// <param name="addSprName"></param>
    /// <returns></returns>
    private string SpriteNameSumReturn(string addSprName)
    {
        string sprName = string.Empty;

        if (addSprName != chaAddSprName.hint && addSprName != chaAddSprName.cardBack)
        {
            Debug.Log("addSprName : " + addSprName);
            CharSprChangeCorStart();
        }

        return sprName = string.Format("{0}_{1}", levelCardSet[(int)MatchingManager.instance.matchingLevel - 1].charName, addSprName);
    }

    private void CardPosInit()
    {
        cardPosList = new Transform[cardPosRoot.childCount];

        for (int i=0; i< cardPosList.Length; i++)
        {
            cardPosList[i] = cardPosRoot.GetChild(i).transform;
        }

       // LevelOneInit();
    }

    private string BackCardName()
    {
        string backCardName = string.Empty;

        backCardName = SpriteNameSumReturn(chaAddSprName.cardBack);

        return backCardName;
    }

    private void CardColorSet(MatchingManager.MatchingLevel level)
    {
        cardPrefab.GetComponent<UISprite>().spriteName = SpriteNameSumReturn(chaAddSprName.cardBack);
        cardPrefab.transform.FindChild("check_img").GetComponent<UISprite>().color = levelCardSet[(int)level - 1].checkColor;
        cardPrefab.transform.FindChild("border_img").GetComponent<UISprite>().color = levelCardSet[(int)level - 1].borderColor;
    }

    /// <summary>
    /// 레벨1로 설정
    /// </summary>
    public void LevelOneInit()
    {
        MatchingManager.instance.matchingLevel = MatchingManager.MatchingLevel.one;
        CardColorSet(MatchingManager.instance.matchingLevel);
        MatchingInit();
    }

    /// <summary>
    /// 레벨2로 설정
    /// </summary>
    public void LevelTwoInit()
    {
        MatchingManager.instance.matchingLevel = MatchingManager.MatchingLevel.two;
        CardColorSet(MatchingManager.instance.matchingLevel);
        MatchingInit();
    }

    /// <summary>
    /// 레벨3으로 설정
    /// </summary>
    public void LevelThreeInit()
    {
        MatchingManager.instance.matchingLevel = MatchingManager.MatchingLevel.three;
        CardColorSet(MatchingManager.instance.matchingLevel);
        MatchingInit();
    }

    /// <summary>
    /// 레벨4로 설정
    /// </summary>
    public void LevelFourInit()
    {
        MatchingManager.instance.matchingLevel = MatchingManager.MatchingLevel.four;
        CardColorSet(MatchingManager.instance.matchingLevel);
        MatchingInit();
    }

    /// <summary>
    /// 레벨5로 설정
    /// </summary>
    public void LevelFiveInit()
    {
        MatchingManager.instance.matchingLevel = MatchingManager.MatchingLevel.five;
        CardColorSet(MatchingManager.instance.matchingLevel);
        MatchingInit();
    }

    /// <summary>
    /// 이전 스테이지
    /// </summary>
    public void PrevBtnClick()
    {
        int prevLevel = (int)MatchingManager.instance.matchingLevel - 1;
        MatchingManager.instance.matchingLevel = (MatchingManager.MatchingLevel)prevLevel;
        CardColorSet(MatchingManager.instance.matchingLevel);
        MatchingInit();
    }

    /// <summary>
    /// 다음 스테이지
    /// </summary>
    public void NextBtnClick()
    {
        int nextLevel = (int)MatchingManager.instance.matchingLevel + 1;
        MatchingManager.instance.matchingLevel = (MatchingManager.MatchingLevel)nextLevel;
        CardColorSet(MatchingManager.instance.matchingLevel);
        MatchingInit();
    }

    /// <summary>
    /// 재시작 버튼 클릭
    /// </summary>
    public void ReStartBtnClick()
    {
        MatchingInit();
        ReStartPopUpClose();
    }

    public void GameEndBtnClick()
    {
        UiPopUpTween(matchingUiObj, false);
        ReStartPopUpClose();

        MainUI.메인.인식글자UI.SetActive(true);
        TargetManager.EnableTracking = true;
        AutoFocusMode.getInstance.OnOffAutoFucousMode(true);
    }

    public void ReStartPopUpOpen()
    {
        UiPopUpTween(restartUiObj, true);
    }

    public void ReStartPopUpClose()
    {
        UiPopUpTween(restartUiObj, false);
    }

    private void UiPopUpTween(GameObject obj, bool state)
    {
        float tweenTime = 0.3f;

        TweenManager.tween_Manager.TweenAllDestroy(obj);

        if (state)
        {
            if (obj.GetComponent<UIPanel>() != null)
            {
                TweenManager.tween_Manager.AddTweenAlpha(obj, obj.GetComponent<UIPanel>().alpha, 1, tweenTime);
                TweenManager.tween_Manager.TweenAlpha(obj);
            }
            else if (obj.GetComponent<UIWidget>() != null)
            {
                TweenManager.tween_Manager.AddTweenAlpha(obj, obj.GetComponent<UIWidget>().alpha, 1, tweenTime);
                TweenManager.tween_Manager.TweenAlpha(obj);
            }
        }
        else
        {
            if (obj.GetComponent<UIPanel>() != null)
            {
                TweenManager.tween_Manager.AddTweenAlpha(obj, obj.GetComponent<UIPanel>().alpha, 0, tweenTime);
                TweenManager.tween_Manager.TweenAlpha(obj);
            }
            else if (obj.GetComponent<UIWidget>() != null)
            {
                TweenManager.tween_Manager.AddTweenAlpha(obj, obj.GetComponent<UIWidget>().alpha, 0, tweenTime);
                TweenManager.tween_Manager.TweenAlpha(obj);
            }
        }
    }

    /// <summary>
    /// 카드 클릭 이벤트
    /// </summary>
    /// <param name="obj"></param>
    public void CardClick(GameObject obj)
    {
        CardColliderSet(false);

        HintCorStop();

        if (MatchingManager.instance.matchingMode == MatchingManager.MatchinMode.random)
        {
            StartCoroutine(ClickRandomRotCard(obj));
          
        }
        else
        {
            StartCoroutine(ClickRotCard(obj));
        }
      
    }

    /// <summary>
    /// 드래그 시작(랜덤 모드에서만 실행)
    /// </summary>
    /// <param name="obj"></param>
    public void CardDragStart(GameObject obj)
    {
        if (MatchingManager.instance.matchingMode == MatchingManager.MatchinMode.random)
        {
            clickObj = obj;
            obj.GetComponent<UIWidget>().depth += plusDepCount;
            obj.transform.FindChild("Label").GetComponent<UIWidget>().depth = obj.GetComponent<UIWidget>().depth + 1;
            mousePos = true;
            Debug.Log("드래그스타트");
        }
    }

    /// <summary>
    /// 드래그 끝(랜덤 모드에서만 실행)
    /// </summary>
    /// <param name="obj"></param>
    public void CardDragEnd(GameObject obj)
    {
        if (MatchingManager.instance.matchingMode == MatchingManager.MatchinMode.random)
        {
            plusDepCount = obj.GetComponent<UIWidget>().depth + 1;
            mousePos = false;
            Debug.Log("드래그엔드");
        }
    }

    /// <summary>
    /// 스코어 확인
    /// </summary>
    private void ScoreLabelSet()
    {
        totalLabel.text = string.Format("전체 카드수 : {0}", totalNum);
        sucessLabel.text = string.Format("{0}", sucessNum); 
        remainLabel.text = string.Format("남은 카드수 : {0}", remainNum); 
        callengeLabel.text = string.Format("{0}", callengeNum);
    }

    /// <summary>
    /// 결과확인 
    /// </summary>
    private void FinishCheck()
    {
        result.nextLevelBtn.SetActive(true);
        result.prevLevelBtn.SetActive(true);

        if (totalNum == sucessNum && remainNum == 0)
        {
            HintCorStop();

            if ((int)MatchingManager.instance.matchingLevel > 4)
            {
                result.nextLevelBtn.SetActive(false);
            }

            if ((int)MatchingManager.instance.matchingLevel < 2)
            {
                result.prevLevelBtn.SetActive(false);
            }

            Debug.Log("끝 결과창 출력");
            TweenManager.tween_Manager.TweenAllDestroy(result.resultPopUp);
            TweenManager.tween_Manager.AddTweenAlpha(result.resultPopUp, 0, 1, 0.3f);
            TweenManager.tween_Manager.TweenAlpha(result.resultPopUp);

            TextPrintStart(talkList.endText);
        }
        else
        {
            //HintCorStart();
        }
    }

    /// <summary>
    /// 매칭게임 초기화
    /// </summary>
    private void MatchingInit()
    {
        result.resultPopUp.SetActive(false);
        UiPopUpTween(matchingUiObj, true);
        CharSprChange(chaAddSprName.idle);
        CardInit();
    }

    /// <summary>
    /// 카드 개수 및 설정값, 카드 뎁스값 초기화 
    /// </summary>
    private void CardInit()
    {
        int cardNum = 0;// (int)(Mathf.Pow(2, (int)MatchingManager.instance.matchingLevel));

        
        switch(MatchingManager.instance.matchingLevel)
        {
            case MatchingManager.MatchingLevel.one:
                cardNum = 2;
                break;

            case MatchingManager.MatchingLevel.two:
                cardNum = 3;
                break;

            case MatchingManager.MatchingLevel.three:
                cardNum = 5;
                break;

            case MatchingManager.MatchingLevel.four:
                cardNum = 7;
                break;

            case MatchingManager.MatchingLevel.five:
                cardNum = 9;
                break;

            default:
                Debug.Log("레벨지정부분 오류");
                break;
        }
        
        if(GlobalDataManager.m_ResourceFolderEnum == GlobalDataManager.CategoryType.City)
        {
            cityCardList = new CitySelectCard[cardNum];
        }
        else if(GlobalDataManager.m_ResourceFolderEnum == GlobalDataManager.CategoryType.Animal)
        {
            animalCardList = new AnimalSelectCard[cardNum];
        }
        else if(GlobalDataManager.m_ResourceFolderEnum == GlobalDataManager.CategoryType.SeaAnimal)
        {
            seaAnimalCardList = new SeaAnimalSelectCard[cardNum];
        }
        else if (GlobalDataManager.m_ResourceFolderEnum == GlobalDataManager.CategoryType.Alphabet)
        {
            alphabetCardList = new AlphabetSelectCard[cardNum];
        }
        else if (GlobalDataManager.m_ResourceFolderEnum == GlobalDataManager.CategoryType.Dino)
        {
            dinoCardList = new DinoSelectCard[cardNum];
        }

        cardObj = new GameObject[cardNum];
        paircardObj = new GameObject[cardNum];

        HintCorStop();

        ScoreLabelSet();

        CardDel();

        CardSetting();
    }

    private void ScoreInit()
    {
        int cardNum = 0;// (int)(Mathf.Pow(2, (int)MatchingManager.instance.matchingLevel));

        switch (MatchingManager.instance.matchingLevel)
        {
            case MatchingManager.MatchingLevel.one:
                cardNum = 2;
                break;

            case MatchingManager.MatchingLevel.two:
                cardNum = 3;
                break;

            case MatchingManager.MatchingLevel.three:
                cardNum = 5;
                break;

            case MatchingManager.MatchingLevel.four:
                cardNum = 7;
                break;

            case MatchingManager.MatchingLevel.five:
                cardNum = 9;
                break;

            default:
                Debug.Log("레벨지정부분 오류");
                break;
        }

        totalNum = cardNum;
        remainNum = cardNum;
        sucessNum = 0;
        callengeNum = 0;

        plusDepCount = 1;

        timeLabel.text = string.Empty;

        ScoreLabelSet();
    }

    /// <summary>
    /// 카드 종류 중복없이 셋팅
    /// </summary>
    private void CardSetting()
    {
        bool isSame = false;


        if (GlobalDataManager.m_ResourceFolderEnum == GlobalDataManager.CategoryType.City)
        {
            for (int i = 0; i < cityCardList.Length; ++i)
            {
                while (true)
                {
                    cityCardList[i] = (CitySelectCard)UnityEngine.Random.Range(0, (int)CitySelectCard.num);

                    // cardSetting[i].selectCard = cardList[i];

                    isSame = false;

                    for (int j = 0; j < i; ++j)
                    {
                        if (cityCardList[j] == cityCardList[i])
                        {
                            isSame = true;
                            break;
                        }
                    }

                    if (!isSame)
                    {
                        break;
                    }
                }
            }
        }
        else if (GlobalDataManager.m_ResourceFolderEnum == GlobalDataManager.CategoryType.Animal)
        {
            for (int i = 0; i < animalCardList.Length; ++i)
            {
                while (true)
                {
                    animalCardList[i] = (AnimalSelectCard)UnityEngine.Random.Range(0, (int)AnimalSelectCard.num);

                    // cardSetting[i].selectCard = cardList[i];

                    isSame = false;

                    for (int j = 0; j < i; ++j)
                    {
                        if (animalCardList[j] == animalCardList[i])
                        {
                            isSame = true;
                            break;
                        }
                    }

                    if (!isSame)
                    {
                        break;
                    }
                }
            }
        }

        else if (GlobalDataManager.m_ResourceFolderEnum == GlobalDataManager.CategoryType.SeaAnimal)
        {
            for (int i = 0; i < seaAnimalCardList.Length; ++i)
            {
                while (true)
                {
                    seaAnimalCardList[i] = (SeaAnimalSelectCard)UnityEngine.Random.Range(0, (int)SeaAnimalSelectCard.num);

                    // cardSetting[i].selectCard = cardList[i];

                    isSame = false;

                    for (int j = 0; j < i; ++j)
                    {
                        if (seaAnimalCardList[j] == seaAnimalCardList[i])
                        {
                            isSame = true;
                            break;
                        }
                    }

                    if (!isSame)
                    {
                        break;
                    }
                }
            }
        }

        else if (GlobalDataManager.m_ResourceFolderEnum == GlobalDataManager.CategoryType.Alphabet)
        {
            for (int i = 0; i < alphabetCardList.Length; ++i)
            {
                while (true)
                {
                    alphabetCardList[i] = (AlphabetSelectCard)UnityEngine.Random.Range(0, (int)AlphabetSelectCard.num);

                    // cardSetting[i].selectCard = cardList[i];

                    isSame = false;

                    for (int j = 0; j < i; ++j)
                    {
                        if (alphabetCardList[j] == alphabetCardList[i])
                        {
                            isSame = true;
                            break;
                        }
                    }

                    if (!isSame)
                    {
                        break;
                    }
                }
            }
        }
        
        else if (GlobalDataManager.m_ResourceFolderEnum == GlobalDataManager.CategoryType.Dino)
        {
            for (int i = 0; i < dinoCardList.Length; ++i)
            {
                while (true)
                {
                   dinoCardList[i] = (DinoSelectCard)UnityEngine.Random.Range(0, (int)DinoSelectCard.num);

                    // cardSetting[i].selectCard = cardList[i];

                    isSame = false;

                    for (int j = 0; j < i; ++j)
                    {
                        if (dinoCardList[j] == dinoCardList[i])
                        {
                            isSame = true;
                            break;
                        }
                    }

                    if (!isSame)
                    {
                        break;
                    }
                }
            }
        }

        CheckModeMatchingStart();
    }

    /// <summary>
    /// 뒤집어서 나올 카드 spr명
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    private string CityCardImgSet(CitySelectCard card)
    {
        string spriteName = string.Empty;
        spriteName = card.ToString() + "_card";
        /*
        switch (card)
        {
            case CitySelectCard.AirPlane:
                spriteName = "AirPlane_card";
                break;

            case CitySelectCard.Ambulance:
                spriteName = "Ambulance_card";
                break;

            case CitySelectCard.DumpTruck:
                spriteName = "DumpTruck_card";
                break;

            case CitySelectCard.Excavator:
                spriteName = "Excavator_card";
                break;

            case CitySelectCard.Firetruck:
                spriteName = "Firetruck_card";
                break;

            case CitySelectCard.FuitCar:
                spriteName = "FuitCar_card";
                break;

            case CitySelectCard.Helicopter:
                spriteName = "Helicopter_card";
                break;

            case CitySelectCard.MixerTruck:
                spriteName = "MixerTruck_card";
                break;

            case CitySelectCard.PoliceCar:
                spriteName = "PoliceCar_card";
                break;

            case CitySelectCard.SchoolBus:
                spriteName = "SchoolBus_card";
                break;

            case CitySelectCard.Ship:
                spriteName = "Ship_card";
                break;

            case CitySelectCard.SportsCar:
                spriteName = "SportsCar_card";
                break;

            case CitySelectCard.Taxi:
                spriteName = "Taxi_card";
                break;

            case CitySelectCard.TowTruck:
                spriteName = "TowTruck_card";
                break;

            case CitySelectCard.Train:
                spriteName = "Train_card";
                break;

            default:
                Debug.Log("카드종류 에러");
                break;
        }
        */
        return spriteName;
    }


    /// <summary>
    /// 뒤집어서 나올 카드 spr명
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    private string AnimalCardImgSet(AnimalSelectCard card)
    {
        string spriteName = string.Empty;
        spriteName = card.ToString() + "_card";
        /*
        switch (card)
        {
            case AnimalSelectCard.bear:
                spriteName = "bear_card";
                break;

            case AnimalSelectCard.crocodile:
                spriteName = "crocodile_card";
                break;

            case AnimalSelectCard.frog:
                spriteName = "frog_card";
                break;

            case AnimalSelectCard.giraffe:
                spriteName = "Giraffe_card";
                break;

            case AnimalSelectCard.hippo:
                spriteName = "hippo_card";
                break;

            case AnimalSelectCard.hyena:
                spriteName = "hyena_card";
                break;

            case AnimalSelectCard.koala:
                spriteName = "koala_card";
                break;

            case AnimalSelectCard.lion:
                spriteName = "lion_card";
                break;

            case AnimalSelectCard.monkey:
                spriteName = "monkey_card";
                break;

            case AnimalSelectCard.panda:
                spriteName = "panda_card";
                break;

            case AnimalSelectCard.penguin:
                spriteName = "penguin_card";
                break;

            case AnimalSelectCard.rabbit:
                spriteName = "rabbit_card";
                break;

            case AnimalSelectCard.rhino:
                spriteName = "rhino_card";
                break;

            case AnimalSelectCard.tiger:
                spriteName = "tiger_card";
                break;

            case AnimalSelectCard.zebra:
                spriteName = "zebra_card";
                break;

            default:
                Debug.Log("카드종류 에러");
                break;
        }
        */
        return spriteName;
    }


    /// <summary>
    /// 뒤집어서 나올 카드 spr명
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    private string SeaAnimalCardImgSet(SeaAnimalSelectCard card)
    {
        string spriteName = string.Empty;
        spriteName = card.ToString() + "_card";

        return spriteName;
    }

    private string AlphabetCardImgSet(AlphabetSelectCard card)
    {
        string spriteName   = string.Empty;
        spriteName          = card.ToString() + "_card";

        return spriteName;
    }

    private string DinoCardImgSet(DinoSelectCard card)
    {
        string spriteName = string.Empty;
        spriteName = card.ToString() + "_card";

        return spriteName;
    }

    /// <summary>
    /// 기본, 랜덤 모드 구분
    /// </summary>
    public void CheckModeMatchingStart()
    {
        ScoreInit();
        TextPrintStart(talkList.initText);

        if (MatchingManager.instance.matchingMode == MatchingManager.MatchinMode.random)
        {
            RandomMatchingStart();
        }
        else
        {
            MatchingStart();
        }
    }

    /*
    /// <summary>
    /// 카드 섞어주기
    /// </summary>
    private void CardRandomSetting()
    {
        //cardList = new SelectCard[cardSetting.Length];

        for (int i = 0; i < cardList.Length; ++i)
        {
            int ranIdx = UnityEngine.Random.Range(0, cardList.Length);

            SelectCard tmp = cardList[ranIdx];//  cardList[ranIdx];

            cardList[ranIdx] = cardList[i];

            cardList[i] = tmp;
        }
    }
    */

    /// <summary>
    /// 카드 삭제
    /// </summary>
    private void CardDel()
    {
        if (cardRoot.childCount > 0)
        {
            for (int i = 0; i < cardRoot.childCount; i++)
            {
                Destroy(cardRoot.GetChild(i).gameObject);
            }
        }
    }

    /// <summary>
    /// 기본 모드 카드 배치
    /// </summary>
    public void MatchingStart()
    {
        CardDel();

        GameObject oriCardPrefab = null;
        GameObject pairCardPrefab = null;

        float tweenTime = 0.3f;

        int posIndex = 0;

        if (GlobalDataManager.m_ResourceFolderEnum == GlobalDataManager.CategoryType.City)
        {
            for (int i = 0; i < cityCardList.Length; i++)
            {
                oriCardPrefab = (Instantiate(cardPrefab) as GameObject);
                pairCardPrefab = (Instantiate(cardPrefab) as GameObject);

                oriCardPrefab.SetActive(false);
                pairCardPrefab.SetActive(false);

                oriCardPrefab.transform.parent = cardRoot;
                pairCardPrefab.transform.parent = cardRoot;

                CardPosSet(oriCardPrefab, posIndex);
                oriCardPrefab.transform.localEulerAngles = Vector3.zero;
                posIndex++;

                CardPosSet(pairCardPrefab, posIndex);
                pairCardPrefab.transform.localEulerAngles = Vector3.zero;
                posIndex++;

                cardObj[i] = oriCardPrefab;
                paircardObj[i] = pairCardPrefab;

                cardObj[i].name = cityCardList[i].ToString();
                paircardObj[i].name = cityCardList[i].ToString();

                cardObj[i].GetComponent<UISprite>().spriteName = CityCardImgSet(cityCardList[i]);
                paircardObj[i].GetComponent<UISprite>().spriteName = CityCardImgSet(cityCardList[i]);

                cardObj[i].transform.FindChild("check_img").gameObject.SetActive(false);
                paircardObj[i].transform.FindChild("check_img").gameObject.SetActive(false);

                cardObj[i].transform.FindChild("Label").GetComponent<UILabel>().text = cardObj[i].name;
                paircardObj[i].transform.FindChild("Label").GetComponent<UILabel>().text = paircardObj[i].name;

                cardObj[i].transform.FindChild("Label").GetComponent<UIWidget>().alpha = 0;
                paircardObj[i].transform.FindChild("Label").GetComponent<UIWidget>().alpha = 0;

                CardBackImgSet(cardObj[i]);
                CardBackImgSet(paircardObj[i]);

                TweenManager.tween_Manager.AddTweenScale(cardObj[i], Vector3.zero, Vector3.one, tweenTime, UITweener.Style.Once, TweenManager.tween_Manager.scaleAnimationCurve);
                TweenManager.tween_Manager.AddTweenScale(paircardObj[i], Vector3.zero, Vector3.one, tweenTime, UITweener.Style.Once, TweenManager.tween_Manager.scaleAnimationCurve);

                TweenManager.tween_Manager.TweenScale(cardObj[i]);
                TweenManager.tween_Manager.TweenScale(paircardObj[i]);
            }
        }
        else if (GlobalDataManager.m_ResourceFolderEnum == GlobalDataManager.CategoryType.Animal)
        {
            for (int i = 0; i < animalCardList.Length; i++)
            {
                oriCardPrefab = (Instantiate(cardPrefab) as GameObject);
                pairCardPrefab = (Instantiate(cardPrefab) as GameObject);

                oriCardPrefab.SetActive(false);
                pairCardPrefab.SetActive(false);

                oriCardPrefab.transform.parent = cardRoot;
                pairCardPrefab.transform.parent = cardRoot;

                CardPosSet(oriCardPrefab, posIndex);
                oriCardPrefab.transform.localEulerAngles = Vector3.zero;
                posIndex++;

                CardPosSet(pairCardPrefab, posIndex);
                pairCardPrefab.transform.localEulerAngles = Vector3.zero;
                posIndex++;

                cardObj[i] = oriCardPrefab;
                paircardObj[i] = pairCardPrefab;

                cardObj[i].name = animalCardList[i].ToString();
                paircardObj[i].name = animalCardList[i].ToString();

                cardObj[i].GetComponent<UISprite>().spriteName = AnimalCardImgSet(animalCardList[i]);
                paircardObj[i].GetComponent<UISprite>().spriteName = AnimalCardImgSet(animalCardList[i]);

                cardObj[i].transform.FindChild("check_img").gameObject.SetActive(false);
                paircardObj[i].transform.FindChild("check_img").gameObject.SetActive(false);

                cardObj[i].transform.FindChild("Label").GetComponent<UILabel>().text = cardObj[i].name;
                paircardObj[i].transform.FindChild("Label").GetComponent<UILabel>().text = paircardObj[i].name;

                cardObj[i].transform.FindChild("Label").GetComponent<UIWidget>().alpha = 0;
                paircardObj[i].transform.FindChild("Label").GetComponent<UIWidget>().alpha = 0;

                CardBackImgSet(cardObj[i]);
                CardBackImgSet(paircardObj[i]);

                TweenManager.tween_Manager.AddTweenScale(cardObj[i], Vector3.zero, Vector3.one, tweenTime, UITweener.Style.Once, TweenManager.tween_Manager.scaleAnimationCurve);
                TweenManager.tween_Manager.AddTweenScale(paircardObj[i], Vector3.zero, Vector3.one, tweenTime, UITweener.Style.Once, TweenManager.tween_Manager.scaleAnimationCurve);

                TweenManager.tween_Manager.TweenScale(cardObj[i]);
                TweenManager.tween_Manager.TweenScale(paircardObj[i]);
            }
        }
        else if (GlobalDataManager.m_ResourceFolderEnum == GlobalDataManager.CategoryType.SeaAnimal)
        {
            for (int i = 0; i < seaAnimalCardList.Length; i++)
            {
                oriCardPrefab = (Instantiate(cardPrefab) as GameObject);
                pairCardPrefab = (Instantiate(cardPrefab) as GameObject);

                oriCardPrefab.SetActive(false);
                pairCardPrefab.SetActive(false);

                oriCardPrefab.transform.parent = cardRoot;
                pairCardPrefab.transform.parent = cardRoot;

                CardPosSet(oriCardPrefab, posIndex);
                oriCardPrefab.transform.localEulerAngles = Vector3.zero;
                posIndex++;

                CardPosSet(pairCardPrefab, posIndex);
                pairCardPrefab.transform.localEulerAngles = Vector3.zero;
                posIndex++;

                cardObj[i] = oriCardPrefab;
                paircardObj[i] = pairCardPrefab;

                cardObj[i].name = seaAnimalCardList[i].ToString();
                paircardObj[i].name = seaAnimalCardList[i].ToString();

                cardObj[i].GetComponent<UISprite>().spriteName = SeaAnimalCardImgSet(seaAnimalCardList[i]);
                paircardObj[i].GetComponent<UISprite>().spriteName = SeaAnimalCardImgSet(seaAnimalCardList[i]);

                cardObj[i].transform.FindChild("check_img").gameObject.SetActive(false);
                paircardObj[i].transform.FindChild("check_img").gameObject.SetActive(false);

                cardObj[i].transform.FindChild("Label").GetComponent<UILabel>().text = cardObj[i].name;
                paircardObj[i].transform.FindChild("Label").GetComponent<UILabel>().text = paircardObj[i].name;

                cardObj[i].transform.FindChild("Label").GetComponent<UIWidget>().alpha = 0;
                paircardObj[i].transform.FindChild("Label").GetComponent<UIWidget>().alpha = 0;

                CardBackImgSet(cardObj[i]);
                CardBackImgSet(paircardObj[i]);

                TweenManager.tween_Manager.AddTweenScale(cardObj[i], Vector3.zero, Vector3.one, tweenTime, UITweener.Style.Once, TweenManager.tween_Manager.scaleAnimationCurve);
                TweenManager.tween_Manager.AddTweenScale(paircardObj[i], Vector3.zero, Vector3.one, tweenTime, UITweener.Style.Once, TweenManager.tween_Manager.scaleAnimationCurve);

                TweenManager.tween_Manager.TweenScale(cardObj[i]);
                TweenManager.tween_Manager.TweenScale(paircardObj[i]);
            }
        }
        else if (GlobalDataManager.m_ResourceFolderEnum == GlobalDataManager.CategoryType.Alphabet)
        {
            for (int i = 0; i < alphabetCardList.Length; i++)
            {
                oriCardPrefab = (Instantiate(cardPrefab) as GameObject);
                pairCardPrefab = (Instantiate(cardPrefab) as GameObject);

                oriCardPrefab.SetActive(false);
                pairCardPrefab.SetActive(false);

                oriCardPrefab.transform.parent = cardRoot;
                pairCardPrefab.transform.parent = cardRoot;

                CardPosSet(oriCardPrefab, posIndex);
                oriCardPrefab.transform.localEulerAngles = Vector3.zero;
                posIndex++;

                CardPosSet(pairCardPrefab, posIndex);
                pairCardPrefab.transform.localEulerAngles = Vector3.zero;
                posIndex++;

                cardObj[i] = oriCardPrefab;
                paircardObj[i] = pairCardPrefab;

                cardObj[i].name = alphabetCardList[i].ToString();
                paircardObj[i].name = alphabetCardList[i].ToString();

                cardObj[i].GetComponent<UISprite>().spriteName = AlphabetCardImgSet(alphabetCardList[i]);
                paircardObj[i].GetComponent<UISprite>().spriteName = AlphabetCardImgSet(alphabetCardList[i]);

                cardObj[i].transform.FindChild("check_img").gameObject.SetActive(false);
                paircardObj[i].transform.FindChild("check_img").gameObject.SetActive(false);

                cardObj[i].transform.FindChild("Label").GetComponent<UILabel>().text = cardObj[i].name;
                paircardObj[i].transform.FindChild("Label").GetComponent<UILabel>().text = paircardObj[i].name;

                cardObj[i].transform.FindChild("Label").GetComponent<UIWidget>().alpha = 0;
                paircardObj[i].transform.FindChild("Label").GetComponent<UIWidget>().alpha = 0;

                CardBackImgSet(cardObj[i]);
                CardBackImgSet(paircardObj[i]);

                TweenManager.tween_Manager.AddTweenScale(cardObj[i], Vector3.zero, Vector3.one, tweenTime, UITweener.Style.Once, TweenManager.tween_Manager.scaleAnimationCurve);
                TweenManager.tween_Manager.AddTweenScale(paircardObj[i], Vector3.zero, Vector3.one, tweenTime, UITweener.Style.Once, TweenManager.tween_Manager.scaleAnimationCurve);

                TweenManager.tween_Manager.TweenScale(cardObj[i]);
                TweenManager.tween_Manager.TweenScale(paircardObj[i]);
            }
        }
        else if (GlobalDataManager.m_ResourceFolderEnum == GlobalDataManager.CategoryType.Dino)
        {
            for (int i = 0; i < dinoCardList.Length; i++)
            {
                oriCardPrefab = (Instantiate(cardPrefab) as GameObject);
                pairCardPrefab = (Instantiate(cardPrefab) as GameObject);

                oriCardPrefab.SetActive(false);
                pairCardPrefab.SetActive(false);

                oriCardPrefab.transform.parent = cardRoot;
                pairCardPrefab.transform.parent = cardRoot;

                CardPosSet(oriCardPrefab, posIndex);
                oriCardPrefab.transform.localEulerAngles = Vector3.zero;
                posIndex++;

                CardPosSet(pairCardPrefab, posIndex);
                pairCardPrefab.transform.localEulerAngles = Vector3.zero;
                posIndex++;

                cardObj[i] = oriCardPrefab;
                paircardObj[i] = pairCardPrefab;

                cardObj[i].name = dinoCardList[i].ToString();
                paircardObj[i].name = dinoCardList[i].ToString();

                cardObj[i].GetComponent<UISprite>().spriteName = DinoCardImgSet(dinoCardList[i]);
                paircardObj[i].GetComponent<UISprite>().spriteName = DinoCardImgSet(dinoCardList[i]);

                cardObj[i].transform.FindChild("check_img").gameObject.SetActive(false);
                paircardObj[i].transform.FindChild("check_img").gameObject.SetActive(false);

                cardObj[i].transform.FindChild("Label").GetComponent<UILabel>().text = cardObj[i].name;
                paircardObj[i].transform.FindChild("Label").GetComponent<UILabel>().text = paircardObj[i].name;

                cardObj[i].transform.FindChild("Label").GetComponent<UIWidget>().alpha = 0;
                paircardObj[i].transform.FindChild("Label").GetComponent<UIWidget>().alpha = 0;

                CardBackImgSet(cardObj[i]);
                CardBackImgSet(paircardObj[i]);

                TweenManager.tween_Manager.AddTweenScale(cardObj[i], Vector3.zero, Vector3.one, tweenTime, UITweener.Style.Once, TweenManager.tween_Manager.scaleAnimationCurve);
                TweenManager.tween_Manager.AddTweenScale(paircardObj[i], Vector3.zero, Vector3.one, tweenTime, UITweener.Style.Once, TweenManager.tween_Manager.scaleAnimationCurve);

                TweenManager.tween_Manager.TweenScale(cardObj[i]);
                TweenManager.tween_Manager.TweenScale(paircardObj[i]);
            }
        }

        Invoke("CardShake", tweenTime);
    }

    private void CardPosSet(GameObject obj, int index)
    {
        Debug.Log("index : " + index);

        Transform tr = cardLevelPos[(int)MatchingManager.instance.matchingLevel - 1].cardPos[index];

        obj.transform.position = tr.position;
        obj.GetComponent<UIWidget>().width = tr.GetComponent<UIWidget>().width;
        obj.GetComponent<UIWidget>().height = tr.GetComponent<UIWidget>().height;
    }

    /// <summary>
    /// 카드 좌우 간격
    /// </summary>
    /// <param name="widthRange"></param>
    /// <param name="i"></param>
    /// <returns></returns>
    private float WidthSize(float widthRange, int i)
    {
        float width = 0;

        if (i % 2 == 0)
        {
            width = -widthRange + (widthRange * 0.25f);
        }
        else
        {
            width = widthRange - (widthRange * 0.75f);
        }
        
        switch (MatchingManager.instance.matchingLevel)
        {
            case MatchingManager.MatchingLevel.three:
                if (i / 2 == 1)
                {
                    if (i % 2 == 0)
                    {
                        width = -widthRange;
                    }
                    else
                    {
                        width = 0;
                    }
                }
                else if (i / 2 == 3)
                {
                    if (i % 2 == 0)
                    {
                        width = -widthRange;
                    }
                    else
                    {
                        width = 0;
                    }
                }
                break;
        }
        
        return width;
    }

    /// <summary>
    /// 카드 높이 간격 설정
    /// </summary>
    /// <param name="heightRange"></param>
    /// <param name="i"></param>
    /// <returns></returns>
    private float HeightSize(float heightRange, int i)
    {
        float height = 0;

        switch (MatchingManager.instance.matchingLevel)
        {
            case MatchingManager.MatchingLevel.one:
                height = 0;
                break;

            case MatchingManager.MatchingLevel.two:
                if (i / 2 == 0)
                {
                    height = heightRange * 0.5f;
                }
                else
                {
                    height = -heightRange * 0.5f;
                }
                break;

            case MatchingManager.MatchingLevel.three:
                if (i / 2 == 0)
                {
                    height = heightRange;
                }
                else if (i / 2 == 1)
                {
                    height = heightRange * 0.25f;
                }
                else if (i / 2 == 2)
                {
                    height = -heightRange * 0.25f;
                }
                else
                {

                    height = -heightRange;
                }
                break;
        }

        return height;
    }

    /// <summary>
    /// 카드 랜덤모드 배치
    /// </summary>
    public void RandomMatchingStart()
    {
        CardDel();

        GameObject oriCardPrefab = null;
        GameObject pairCardPrefab = null;

        float widthRange = 1080 * 0.7f;// transform.Find("UI Root").GetComponent<UIRoot>().manualWidth * 0.7f;
        float heightRange = 1080 * 0.3f;//transform.Find("UI Root").GetComponent<UIRoot>().manualHeight * 0.3f;

        float tweenTime = 0.3f;

        for (int i = 0; i < cityCardList.Length; i++)
        {
            oriCardPrefab = (Instantiate(cardPrefab) as GameObject);
            pairCardPrefab = (Instantiate(cardPrefab) as GameObject);

            oriCardPrefab.SetActive(false);
            pairCardPrefab.SetActive(false);

            oriCardPrefab.transform.parent = cardRoot;
            pairCardPrefab.transform.parent = cardRoot;

            oriCardPrefab.transform.localPosition = new Vector3(UnityEngine.Random.Range(-widthRange, widthRange), UnityEngine.Random.Range(-heightRange, heightRange), 0);
            oriCardPrefab.transform.localEulerAngles = new Vector3(0, 0, UnityEngine.Random.Range(-180, 180));

            pairCardPrefab.transform.localPosition = new Vector3(UnityEngine.Random.Range(-widthRange, widthRange), UnityEngine.Random.Range(-heightRange, heightRange), 0);
            pairCardPrefab.transform.localEulerAngles = new Vector3(0, 0, UnityEngine.Random.Range(-180, 180));

            cardObj[i] = oriCardPrefab;
            paircardObj[i] = pairCardPrefab;

            cardObj[i].name = cityCardList[i].ToString();
            paircardObj[i].name = cityCardList[i].ToString();

            cardObj[i].GetComponent<UISprite>().spriteName = CityCardImgSet(cityCardList[i]);
            paircardObj[i].GetComponent<UISprite>().spriteName = CityCardImgSet(cityCardList[i]);

            cardObj[i].transform.FindChild("Label").GetComponent<UILabel>().text = cardObj[i].name;
            paircardObj[i].transform.FindChild("Label").GetComponent<UILabel>().text = paircardObj[i].name;

            //CardImgSet(cardObj[i]);
           // CardImgSet(paircardObj[i]);

            TweenManager.tween_Manager.AddTweenScale(cardObj[i], Vector3.zero, Vector3.one, tweenTime, UITweener.Style.Once, TweenManager.tween_Manager.scaleAnimationCurve);
            TweenManager.tween_Manager.AddTweenScale(paircardObj[i], Vector3.zero, Vector3.one, tweenTime, UITweener.Style.Once, TweenManager.tween_Manager.scaleAnimationCurve);

            TweenManager.tween_Manager.TweenScale(cardObj[i]);
            TweenManager.tween_Manager.TweenScale(paircardObj[i]);
        }
       // ViewCorStart();
    }

    /// <summary>
    /// 카드 섞기
    /// </summary>
    private void CardShake()
    {
        Vector3 firPos;
        Vector3 secPos;

        GameObject secObj;

        if (MatchingManager.instance.matchingMode == MatchingManager.MatchinMode.normal)
        {
            Transform[] objList = new Transform[cardRoot.childCount];

            for (int i = 0; i < cardRoot.childCount; i++)
            {
                objList[i] = cardRoot.GetChild(i).transform;
            }

            if (objList.Length > 0)
            {
                for (int i = 0; i < objList.Length; i++)
                {

                    secObj = objList[UnityEngine.Random.Range(0, objList.Length)].gameObject;

                    if (objList[i].gameObject != secObj)
                    {
                        firPos = objList[i].localPosition;
                        secPos = secObj.transform.localPosition;

                        objList[i].localPosition = secPos;
                        secObj.transform.localPosition = firPos;
                    }
                }
            }

            ViewCorStart();
        }
        else
        {
            RandomMatchingStart();
        }
    }

    /*
    private void CardImgSet(GameObject obj)
    {
        //TODO : 변경 이미지 obj이미지 이름 변경
        obj.GetComponent<UISprite>().spriteName = "background_popup";
        //obj.GetComponent<UISprite>().spriteName = obj.name;
    }
    */

    private void CardBackImgSet(GameObject obj)
    {
        //TODO : 변경 이미지 obj이미지 이름 변경
        obj.GetComponent<UISprite>().spriteName = BackCardName();
        obj.transform.FindChild("check_img").gameObject.SetActive(true);
        //obj.GetComponent<UISprite>().spriteName = obj.name;
    }


    private void AllCardImgSet()
    {
        if (GlobalDataManager.m_ResourceFolderEnum == GlobalDataManager.CategoryType.City)
        {
            for (int i = 0; i < cardObj.Length; i++)
            {
                StartCoroutine(FrontRotCard(cardObj[i], CityCardImgSet(cityCardList[i])));
                StartCoroutine(FrontRotCard(paircardObj[i], CityCardImgSet(cityCardList[i])));
            }
        }
        else if (GlobalDataManager.m_ResourceFolderEnum == GlobalDataManager.CategoryType.Animal)
        {
            for (int i = 0; i < cardObj.Length; i++)
            {
                StartCoroutine(FrontRotCard(cardObj[i], AnimalCardImgSet(animalCardList[i])));
                StartCoroutine(FrontRotCard(paircardObj[i], AnimalCardImgSet(animalCardList[i])));
            }
        }
        else if (GlobalDataManager.m_ResourceFolderEnum == GlobalDataManager.CategoryType.SeaAnimal)
        {
            for (int i = 0; i < cardObj.Length; i++)
            {
                StartCoroutine(FrontRotCard(cardObj[i], SeaAnimalCardImgSet(seaAnimalCardList[i])));
                StartCoroutine(FrontRotCard(paircardObj[i], SeaAnimalCardImgSet(seaAnimalCardList[i])));
            }
        }
        else if (GlobalDataManager.m_ResourceFolderEnum == GlobalDataManager.CategoryType.Alphabet)
        {
            for (int i = 0; i < cardObj.Length; i++)
            {
                StartCoroutine(FrontRotCard(cardObj[i], AlphabetCardImgSet(alphabetCardList[i])));
                StartCoroutine(FrontRotCard(paircardObj[i], AlphabetCardImgSet(alphabetCardList[i])));
            }
        }
        else if (GlobalDataManager.m_ResourceFolderEnum == GlobalDataManager.CategoryType.Dino)
        {
            for (int i = 0; i < cardObj.Length; i++)
            {
                StartCoroutine(FrontRotCard(cardObj[i],DinoCardImgSet(dinoCardList[i])));
                StartCoroutine(FrontRotCard(paircardObj[i],DinoCardImgSet(dinoCardList[i])));
            }
        }
        /*
        for (int i = 0; i < cardRoot.childCount; i++)
        {
            StartCoroutine(FrontRotCard(cardRoot.GetChild(i).gameObject));
        }
        */
    }

    private void CardImgBack()
    {
       // string backImgName = "btn_square";

        for (int i = 0; i < cardRoot.childCount; i++)
        {
            StartCoroutine(BackRotCard(cardRoot.GetChild(i).gameObject));
            //cardRoot.GetChild(i).GetComponent<UISprite>().spriteName = backImgName;
        }
    }

    private void CardColliderSet(bool state)
    {
        for (int i = 0; i < cardRoot.childCount; i++)
        {
            cardRoot.GetChild(i).GetComponent<BoxCollider2D>().enabled = state;
        }
    }


    private void TextPrintStart(string text)
    {
        TextPrintStop();
        textCoroutine = StartCoroutine(TextPrintCor(text));
    }

    private void TextPrintStop()
    {
        if (textCoroutine != null)
        {
            StopCoroutine(textCoroutine);
            textCoroutine = null;
        }
    }


    private void ViewCorStart()
    {
        ViewCorStop();
        viewCor = StartCoroutine(ViewCard());

    }

    private void ViewCorStop()
    {
        if (viewCor != null)
        {
            StopCoroutine(viewCor);
            viewCor = null;
        }
    }


    private void HintCorStart()
    {
        HintCorStop();
        hintCor = StartCoroutine(HintCheck());
    }

    private void HintCorStop()
    {
        if (hintCor != null)
        {
            StopCoroutine(hintCor);
            hintCor = null;

            if (hintObj != null && sameHintObj != null)
            {
                TweenManager.tween_Manager.TweenAllDestroy(hintObj);
                TweenManager.tween_Manager.TweenAllDestroy(sameHintObj);

                hintObj.GetComponent<UIWidget>().alpha = 1.0f;
                sameHintObj.GetComponent<UIWidget>().alpha = 1.0f;

                hintObj = null;
                sameHintObj = null;
            }
        }
    }

    private void CharSprChangeCorStart()
    {
        CharSprChangeCorStop();
        charResetCoroutine = StartCoroutine(CharReturnSpr());
    }

    private void CharSprChangeCorStop()
    {
        if(charResetCoroutine != null)
        {
            StopCoroutine(charResetCoroutine);
            charResetCoroutine = null;
        }
    }


    private IEnumerator TextPrintCor(string insertText)
    {
        insertLabel.text = string.Empty;

        if (!talk)
        {
            yield break;
        }

        int stringIndex = 0;
        float nextTime = 0.05f;
        float savedNextTime = nextTime;

        float waitTime = 0.3f;

        TweenManager.tween_Manager.TweenAllDestroy(insertLabel.gameObject);
        TweenManager.tween_Manager.AddTweenAlpha(insertLabel.gameObject, insertLabel.GetComponent<UIWidget>().alpha, 1, waitTime);
        TweenManager.tween_Manager.TweenAlpha(insertLabel.gameObject);

        yield return new WaitForSeconds(waitTime);

        //HintCorStart();

        while (true)
        {
            nextTime = nextTime - Time.deltaTime;

            if (nextTime <= 0)
            {
                if (stringIndex > insertText.Length)
                {
                    yield return new WaitForSeconds(waitTime*3);

                    TweenManager.tween_Manager.TweenAllDestroy(insertLabel.gameObject);
                    TweenManager.tween_Manager.AddTweenAlpha(insertLabel.gameObject, insertLabel.GetComponent<UIWidget>().alpha, 0, waitTime);
                    TweenManager.tween_Manager.TweenAlpha(insertLabel.gameObject);
                    Debug.Log("텍스트 코루틴 끝");
                    yield break;
                }

                insertLabel.text
                     = m_Builder.AppendFormat("{0}", insertText.Substring(0, stringIndex)).ToString();
                m_Builder.Remove(0, m_Builder.Length);
                nextTime = savedNextTime;
                stringIndex++;
            }
            yield return new WaitForEndOfFrame();
        }

    }

    private IEnumerator ViewCard()
    {
        CardColliderSet(false);
        HintCorStop();

        yield return new WaitForSeconds(0.3f);

        AllCardImgSet();

        float waitTime = 3.0f * (int)MatchingManager.instance.matchingLevel;

        timeLabel.GetComponent<UIWidget>().alpha = 1;
        TweenManager.tween_Manager.TweenAllDestroy(timeLabel.gameObject);

        while (true)
        {
            yield return new WaitForEndOfFrame();

            timeLabel.text = Math.Truncate(waitTime).ToString();
            waitTime -= Time.deltaTime;

            if (waitTime <= 0)
            {
                TweenManager.tween_Manager.AddTweenAlpha(timeLabel.gameObject, 1, 0, 0.3f);
                TweenManager.tween_Manager.TweenAlpha(timeLabel.gameObject);

                CardImgBack();

                yield return new WaitForSeconds(0.2f);

                CardColliderSet(true);
                HintCorStart();
                yield break;
            }

        }
    }


    private IEnumerator FrontRotCard(GameObject obj, string spriteName)
    {
        //TODO : 이미지 이름 변경
        string ImgName = spriteName;

        float waitTime = 0.3f;

        TweenManager.tween_Manager.TweenAllDestroy(obj);

        TweenManager.tween_Manager.AddTweenScale(obj, Vector3.one, new Vector3(1.2f, 1.2f, 1), waitTime, UITweener.Style.Once, TweenManager.tween_Manager.animationCurveList[0]);
        TweenManager.tween_Manager.AddTweenRotation(obj, Vector3.zero, new Vector3(0, 360, 0), waitTime);

        TweenManager.tween_Manager.TweenScale(obj);
        TweenManager.tween_Manager.TweenRotation(obj);

        yield return new WaitForSeconds(waitTime);

        obj.GetComponent<UISprite>().spriteName = ImgName;
        obj.transform.FindChild("check_img").gameObject.SetActive(false);

        yield break;
    }

    private IEnumerator BackRotCard(GameObject obj)
    {
        //TODO : 변경 이미지 back 이미지 이름 변경
        string backImgName = BackCardName();

        float waitTime = 0.3f;

        TweenManager.tween_Manager.TweenAllDestroy(obj);

        TweenManager.tween_Manager.AddTweenScale(obj, Vector3.one, new Vector3(1.2f, 1.2f, 1), waitTime, UITweener.Style.Once, TweenManager.tween_Manager.animationCurveList[0]);
        TweenManager.tween_Manager.AddTweenRotation(obj, Vector3.zero, new Vector3(0, 360, 0), waitTime);

        TweenManager.tween_Manager.TweenScale(obj);
        TweenManager.tween_Manager.TweenRotation(obj);

        yield return new WaitForSeconds(waitTime);

        obj.GetComponent<UISprite>().spriteName = backImgName;
        obj.transform.FindChild("check_img").gameObject.SetActive(true);

        yield break;
    }

    private IEnumerator ClickRotCard(GameObject obj)
    {
        string ImgName = string.Empty;

        for (int i =0; i<cardObj.Length; i++)
        {
            if (obj.name == cardObj[i].name)
            {
                if (GlobalDataManager.m_ResourceFolderEnum == GlobalDataManager.CategoryType.City)
                {
                    ImgName = CityCardImgSet(cityCardList[i]);
                }
                else if (GlobalDataManager.m_ResourceFolderEnum == GlobalDataManager.CategoryType.Animal)
                {
                    ImgName = AnimalCardImgSet(animalCardList[i]);
                }
                else if (GlobalDataManager.m_ResourceFolderEnum == GlobalDataManager.CategoryType.SeaAnimal)
                {
                    ImgName = SeaAnimalCardImgSet(seaAnimalCardList[i]);
                }
                else if (GlobalDataManager.m_ResourceFolderEnum == GlobalDataManager.CategoryType.Alphabet)
                {
                    ImgName = AlphabetCardImgSet(alphabetCardList[i]);
                }
                else if (GlobalDataManager.m_ResourceFolderEnum == GlobalDataManager.CategoryType.Dino)
                {
                    ImgName = DinoCardImgSet(dinoCardList[i]);
                }
                break;
            }
        }

        //TODO : 변경 이미지 back 이미지 이름 변경
        string backImgName = BackCardName();

        float waitTime = 0.3f;

        TweenManager.tween_Manager.TweenAllDestroy(obj);

        TweenManager.tween_Manager.AddTweenScale(obj, Vector3.one, new Vector3(1.4f, 1.4f, 1), waitTime, UITweener.Style.Once, TweenManager.tween_Manager.animationCurveList[0]);
        TweenManager.tween_Manager.AddTweenRotation(obj, obj.transform.localEulerAngles, new Vector3(0, 360, 0), waitTime);

        TweenManager.tween_Manager.TweenScale(obj);
        TweenManager.tween_Manager.TweenRotation(obj);

        obj.GetComponent<UIWidget>().alpha = 1.0f;

        yield return new WaitForSeconds(waitTime);

        obj.GetComponent<UISprite>().spriteName = ImgName;
        obj.transform.FindChild("check_img").gameObject.SetActive(false);

        if (firClickCard == null)
        {
            firClickCard = obj;
        }
        else
        {
            if (firClickCard != obj && firClickCard.name == obj.name)
            {
                float delTime = 0.15f;
                //TODO : 성공이벤트 이펙트, 사운드 등등 추가
                yield return new WaitForSeconds(waitTime);

                TweenManager.tween_Manager.TweenAllDestroy(firClickCard);
                TweenManager.tween_Manager.TweenAllDestroy(obj);

                TweenManager.tween_Manager.AddTweenScale(firClickCard, firClickCard.transform.localScale, Vector3.zero, delTime, UITweener.Style.Once);
                TweenManager.tween_Manager.AddTweenScale(obj, obj.transform.localScale, Vector3.zero, delTime, UITweener.Style.Once);

                TweenManager.tween_Manager.TweenScale(firClickCard);
                TweenManager.tween_Manager.TweenScale(obj);

                sucessNum++;
                remainNum = totalNum - sucessNum;

                TextPrintStart(talkList.successText);
                CharSprChange(chaAddSprName.success);
                Debug.Log("성공");

            }
            else if(firClickCard == obj)
            {
                obj.GetComponent<UISprite>().spriteName = backImgName;
                obj.transform.FindChild("check_img").gameObject.SetActive(true);
                CharSprChange(chaAddSprName.idle);
                TextPrintStart(talkList.sameText);
                Debug.Log("동일 카드");
            }
            else
            {
                float resetTime = 0.5f;
                //TODO : 실패이벤트 이펙트, 사운드 등등 추가
                yield return new WaitForSeconds(resetTime);

                TweenManager.tween_Manager.TweenAllDestroy(firClickCard);
                TweenManager.tween_Manager.TweenAllDestroy(obj);


                TweenManager.tween_Manager.AddTweenScale(firClickCard, firClickCard.transform.localScale, new Vector3(1.4f, 1.4f, 1), waitTime, UITweener.Style.Once, TweenManager.tween_Manager.animationCurveList[0]);
                TweenManager.tween_Manager.AddTweenRotation(firClickCard,
                                                            firClickCard.transform.localEulerAngles,
                                                            new Vector3(firClickCard.transform.localEulerAngles.x, 360, firClickCard.transform.localEulerAngles.z),
                                                            waitTime);

                TweenManager.tween_Manager.AddTweenScale(obj, obj.transform.localScale, new Vector3(1.4f, 1.4f, 1), waitTime, UITweener.Style.Once, TweenManager.tween_Manager.animationCurveList[0]);
                TweenManager.tween_Manager.AddTweenRotation(obj,
                                                            obj.transform.localEulerAngles,
                                                            new Vector3(obj.transform.localEulerAngles.x, 360, obj.transform.localEulerAngles.z),
                                                            waitTime);

                TweenManager.tween_Manager.TweenScale(firClickCard);
                TweenManager.tween_Manager.TweenRotation(firClickCard);

                TweenManager.tween_Manager.TweenScale(obj);
                TweenManager.tween_Manager.TweenRotation(obj);

             //   yield return new WaitForSeconds(resetTime);

                firClickCard.GetComponent<UISprite>().spriteName = backImgName;
                obj.GetComponent<UISprite>().spriteName = backImgName;
                firClickCard.transform.FindChild("check_img").gameObject.SetActive(true);
                obj.transform.FindChild("check_img").gameObject.SetActive(true);

                TextPrintStart(talkList.failText);
                CharSprChange(chaAddSprName.fail);
                Debug.Log("실패");
            }
            callengeNum++;
            firClickCard = null;
            HintCorStart();
        }

        //HintCorStop();
        ScoreLabelSet();
        FinishCheck();
        CardColliderSet(true);
        yield break;
    }


    private IEnumerator ClickRandomRotCard(GameObject obj)
    {
        //TODO : 변경 이미지 obj이미지 이름 변경
        string ImgName = "background_popup";

        float waitTime = 0.2f;

        TweenManager.tween_Manager.TweenAllDestroy(obj); 
        TweenManager.tween_Manager.AddTweenScale(obj, 
                                                 obj.transform.localScale,
                                                 new Vector3(1.4f, 1.4f, 1),
                                                 waitTime,
                                                 UITweener.Style.Once,
                                                 TweenManager.tween_Manager.scaleAnimationCurve);
        TweenManager.tween_Manager.TweenScale(obj);

        obj.GetComponent<UIWidget>().alpha = 1.0f;

        yield return new WaitForSeconds(waitTime);

        obj.GetComponent<UISprite>().spriteName = ImgName;


        if (firClickCard == null)
        {
            firClickCard = obj;
        }
        else
        {
            if (firClickCard != obj && firClickCard.name == obj.name)
            {
                float delTime = 0.15f;
                //TODO : 성공이벤트 이펙트, 사운드 등등 추가
                yield return new WaitForSeconds(waitTime);

                TweenManager.tween_Manager.TweenAllDestroy(firClickCard);
                TweenManager.tween_Manager.TweenAllDestroy(obj);

                TweenManager.tween_Manager.AddTweenScale(firClickCard, firClickCard.transform.localScale, Vector3.zero, delTime, UITweener.Style.Once);
                TweenManager.tween_Manager.AddTweenScale(obj, obj.transform.localScale, Vector3.zero, delTime, UITweener.Style.Once);

                TweenManager.tween_Manager.TweenScale(firClickCard);
                TweenManager.tween_Manager.TweenScale(obj);

                sucessNum += 2;
                remainNum = totalNum - sucessNum;

                TextPrintStart(talkList.successText);
                Debug.Log("성공");

            }
            else if(firClickCard == obj)
            {
                TweenManager.tween_Manager.TweenAllDestroy(obj);
                TweenManager.tween_Manager.AddTweenScale(obj, obj.transform.localScale, Vector3.one, waitTime * 0.5f);
                TweenManager.tween_Manager.TweenScale(obj);

                TextPrintStart(talkList.sameText);
            }
            else
            {
                float resetTime = 0.5f;
                //TODO : 실패이벤트 이펙트, 사운드 등등 추가
                yield return new WaitForSeconds(resetTime);

                TweenManager.tween_Manager.TweenAllDestroy(firClickCard);
                TweenManager.tween_Manager.TweenAllDestroy(obj);


                TweenManager.tween_Manager.AddTweenScale(firClickCard, firClickCard.transform.localScale, Vector3.one, waitTime * 0.5f);
                TweenManager.tween_Manager.AddTweenScale(obj, obj.transform.localScale, Vector3.one, waitTime * 0.5f);

                TweenManager.tween_Manager.TweenScale(firClickCard);
                TweenManager.tween_Manager.TweenScale(obj);

                //   yield return new WaitForSeconds(resetTime);
                TextPrintStart(talkList.failText);
                Debug.Log("실패");
            }
            callengeNum++;
            firClickCard = null;
            //HintCorStart();
        }

        ScoreLabelSet();
        FinishCheck();
        CardColliderSet(true);
        yield break;
    }

    private IEnumerator HintCheck()
    {
        float hintTime = 5.0f;

        while(true)
        {
            yield return new WaitForEndOfFrame();

            hintTime -= Time.deltaTime;

            if(hintTime <= 0)
            {
                if (cardRoot.childCount > 0)
                {
                    for (int i = 0; i < cardRoot.childCount; i++)
                    {
                        if(cardRoot.transform.GetChild(i).localScale.x > 0)
                        {
                            hintObj = cardRoot.transform.GetChild(i).gameObject;
                            break;
                        }
                    }

                    if(hintObj != null)
                    {
                        for (int i = 0; i < cardRoot.childCount; i++)
                        {
                            if (cardRoot.GetChild(i).gameObject != hintObj
                                && cardRoot.GetChild(i).gameObject.name == hintObj.name)
                            {
                                hintObj = cardRoot.transform.GetChild(i).gameObject;
                                break;
                            }
                        }

                        sameHintObj = cardRoot.Find(hintObj.name).gameObject;

                        TweenManager.tween_Manager.TweenAllDestroy(hintObj);
                        TweenManager.tween_Manager.TweenAllDestroy(sameHintObj);

                        TweenManager.tween_Manager.AddTweenAlpha(hintObj, 1, 0.5f, 1, UITweener.Style.PingPong);
                        TweenManager.tween_Manager.AddTweenAlpha(sameHintObj, 1, 0.5f, 1, UITweener.Style.PingPong);

                        TweenManager.tween_Manager.TweenAlpha(hintObj);
                        TweenManager.tween_Manager.TweenAlpha(sameHintObj);
                    }
                }
                CharSprChange(chaAddSprName.hint);

                yield break;
            }
        }
    }

    private IEnumerator CharReturnSpr()
    {
        UISprite charSpr = charStateObj.GetComponent<UISprite>();
        float waitTime = 0.8f;

        yield return new WaitForSeconds(waitTime);

        charSpr.spriteName = levelCardSet[(int)MatchingManager.instance.matchingLevel - 1].charName + "_" + chaAddSprName.idle;

        yield break;
    }
}

