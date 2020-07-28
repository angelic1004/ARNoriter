using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KartItemManager : Singleton<KartItemManager>
{
    private static int SET_ITEM_GAP_TIME = 10;                  // 아이템 갭 타임 (정답이 너무 안나올 경우 대비용)

    // 스테이지1 아이템 위치 배열
    private static int[] STAGE1_ITEM_ARRAY = { 5, 10, 20, 27, 30, 40, 45, 50, 55, 60, 70, 75, 80, 90, 100, 110, 115, 120, 130, 133, 135, 140, 150, 160, 165, 170, 180, 185, 190, 195, 200 };

    private static int[] STAGE1_OIL_POSITION = { 30, 55, 70, 100, 120, 133, 140, 160, 190, 195, 200 };                  // 스테이지1 기름 위치
    private static int[] STAGE1_OBSTACLES_POSITION = { 100, 150, 190 };                                   // 스테이지1 장애물 위치
    private static int[] STAGE1_BOOSTER_POSITION = { 20, 75, 130, 165, 200 };                             // 스테이지1 부스터 위치

    // 스테이지2 아이템 위치 배열
    private static int[] STAGE2_ITEM_ARRAY = { 5, 10, 20, 25, 30, 32, 35, 40, 45, 50, 53, 55, 60,
                                               62, 65, 75, 80, 82, 85, 97, 105, 110, 113, 120, 122, 130, 131, 133, 138, 140, 145, 150, 158, 160, 165, 170, 180, 183, 188, 190, 192, 195, 200};

    private static int[] STAGE2_OIL_POSITION = { 25, 50, 82, 97, 120, 138, 145, 170, 192, 200 };                        // 스테이지2 기름 위치
    private static int[] STAGE2_OBSTACLES_POSITION = { 30, 45, 75, 113, 158, 170, 190 };                  // 스테이지2 장애물 위치
    private static int[] STAGE2_BOOSTER_POSITION = { 10, 32, 85, 130, 170, 195 };                         // 스테이지2 부스터 위치

    // 스테이지3 아이템 위치 배열
    private static int[] STAGE3_ITEM_ARRAY = { 7, 10, 15, 25, 31, 32, 33, 37, 40, 41, 50, 58, 59, 60,
                                               62, 65, 66, 69, 80, 81, 90, 93, 97, 105, 110, 115, 120, 122, 123, 128, 130, 131, 133, 140,
                                               145, 146, 155, 160, 168, 169, 170, 182, 190, 194, 196, 200};

    private static int[] STAGE3_OIL_POSITION = { 31, 50, 81, 110, 115, 122, 140, 168, 190, 200 };               // 스테이지2 기름 위치
    private static int[] STAGE3_OBSTACLES_POSITION = { 25, 50, 58, 62, 69, 80, 81, 105, 122, 131, 133, 168, 190, 200 };       // 스테이지2 장애물 위치
    private static int[] STAGE3_BOOSTER_POSITION = { 10, 41, 66, 93, 123, 145, 182, 200 };                 // 스테이지2 부스터 위치

    private static int[] NON_STAGE_ITEM_ARRAY = { 0 };


    public GameObject itemParent;
    public GameObject[] oils;
    public GameObject[] obstacles;
    public GameObject[] apple;
    public GameObject[] peach;
    public GameObject[] strawberry;
    public GameObject[] watermelon;
    public GameObject[] gapItems;                                        // 정답이 오래 안나왔을 경우 배치해주는 과일

    public GameObject[] boosters;

    public GameObject[] stageItems;
    private int posNum;

    public int[] itemPositionNumber;
    private int itemPositionIndex = 0;
    private int previousItemPosNum = -1;                                 // 이전 아이템 포지션 번호

    public int score;
    public UILabel scoreLabel;

    public GameObject collectItems;
    private List<GameObject> collectItemList;
    private int nowItemCount;

    public bool itemPosRepeat;                                           // 아이템 위치 중복 허용
    private KartGameManager.FruitChoice fruitChoice;

    public int correctItemGapTime;
    public bool overItemGapTime;
    private Transform disposeItem;


    void Awake()
    {
        score = 0;
    }

    public void SetKartItemManager(int stageNum)
    {
        itemPositionIndex = 0;
        ResetKartItemManager(stageNum);
    }

    public void SetKartItem(KartGameMapInfo info)
    {
        SetArrayObj(info.oils, "kartgame_fuel");
        SetArrayObj(info.obstacles, "kartgame_obstacle");
        SetArrayObj(info.apple, "kartgame_item");
        SetArrayObj(info.peach, "kartgame_item");
        SetArrayObj(info.strawberry, "kartgame_item");
        SetArrayObj(info.watermelon, "kartgame_item");
        SetArrayObj(info.gapitems,"kartgame_item");
        SetArrayObj(info.boosters, "kartgame_booster");

        oils = info.oils;
        obstacles = info.obstacles;
        apple = info.apple;
        peach = info.peach;
        strawberry = info.strawberry;
        watermelon = info.watermelon;
        gapItems = info.gapitems;
        boosters = info.boosters;
    }

    private void SetArrayObj(GameObject[] array, string tagStr)
    {
        for(int i = 0; i < array.Length; i++)
        {
            array[i].SetActive(false);
            array[i].tag = tagStr;
        }
    }

    //public void OnClickBackBtn()
    //{
    //    itemPositionIndex = 0;
    //    SetStage(-1);
    //}

    public void StartKartItemCoroutine()
    {
        StartCoroutine(UpdateScoreUI());
        StartCoroutine(CheckCorrectItemGapTime());
    }

    private void ResetKartItemManager(int stageNum)
    {
        itemPosRepeat = false;
        SetStage(stageNum);

        fruitChoice = KartGameManager.getInstance.fruitChoice;

        collectItemList = new List<GameObject>();
        foreach (Transform child in collectItems.transform)
        {
            collectItemList.Add(child.gameObject);
            child.gameObject.GetComponent<UISprite>().spriteName = fruitChoice.ToString().ToLower() + "_noncolor";
        }

        nowItemCount = 0;
        overItemGapTime = false;
        correctItemGapTime = SET_ITEM_GAP_TIME;
        itemPositionIndex = 0;
    }

    public void StartKartItemManager(int stageNum)
    {
        StopAllCoroutines();
        ResetKartItemManager(stageNum);
        StartKartItemCoroutine();
    }

    private void SetStage(int stageNum)
    {
        itemPositionNumber = null;

        switch (stageNum)
        {
            case 0:
                Debug.Log("STAGE1 Choiced");
                itemPositionNumber = new int[STAGE1_ITEM_ARRAY.Length];

                for (int i = 0; i < STAGE1_ITEM_ARRAY.Length; i++)
                {
                    itemPositionNumber[i] = STAGE1_ITEM_ARRAY[i];
                }

                //itemPositionNumber = STAGE1_ITEM_ARRAY;
                stageItems = new GameObject[itemPositionNumber.Length];
                ResetFixedItems(STAGE1_OIL_POSITION, itemPositionNumber, oils);
                ResetFixedItems(STAGE1_OBSTACLES_POSITION, itemPositionNumber, obstacles);
                ResetFixedItems(STAGE1_BOOSTER_POSITION, itemPositionNumber, boosters);
                break;

            case 1:
                Debug.Log("STAGE2 Choiced");
                itemPositionNumber = new int[STAGE2_ITEM_ARRAY.Length];

                for (int i = 0; i < STAGE2_ITEM_ARRAY.Length; i++)
                {
                    itemPositionNumber[i] = STAGE2_ITEM_ARRAY[i];
                }

                //itemPositionNumber = STAGE2_ITEM_ARRAY;
                stageItems = new GameObject[itemPositionNumber.Length];
                ResetFixedItems(STAGE2_OIL_POSITION, itemPositionNumber, oils);
                ResetFixedItems(STAGE2_OBSTACLES_POSITION, itemPositionNumber, obstacles);
                ResetFixedItems(STAGE2_BOOSTER_POSITION, itemPositionNumber, boosters);
                break;

            case 2:
                Debug.Log("STAGE3 Choiced");
                itemPositionNumber = new int[STAGE3_ITEM_ARRAY.Length];

                for (int i = 0; i < STAGE3_ITEM_ARRAY.Length; i++)
                {
                    itemPositionNumber[i] = STAGE3_ITEM_ARRAY[i];
                }

                //itemPositionNumber = STAGE3_ITEM_ARRAY;
                stageItems = new GameObject[itemPositionNumber.Length];
                ResetFixedItems(STAGE3_OIL_POSITION, itemPositionNumber, oils);
                ResetFixedItems(STAGE3_OBSTACLES_POSITION, itemPositionNumber, obstacles);
                ResetFixedItems(STAGE3_BOOSTER_POSITION, itemPositionNumber, boosters);
                break;

            case -1:
                Debug.Log("NON STAGE Choiced");
                itemPositionNumber = NON_STAGE_ITEM_ARRAY;
                stageItems = new GameObject[itemPositionNumber.Length];
                break;
        }

        ResetFruitItems();
    }

    private void SetItemPositionNumber(int[] stageArray)
    {
        itemPositionNumber = null;
        itemPositionNumber = new int[stageArray.Length];

        for (int i = 0; i < stageArray.Length; i++)
        {
            itemPositionNumber[i] = stageArray[i];
        }
    }

    /// <summary>
    /// 고정 아이템을 리셋합니다. (기름, 장애물 아이템)
    /// </summary>
    private void ResetFixedItems(int[] fixedItemPos, int[] stageItemArray, GameObject[] fixedItem)
    {
        int itemIndex = 0;
        for (int i = 0; i < fixedItemPos.Length; i++)
        {
            for (int j = 0; j < stageItemArray.Length; j++)
            {
                if (fixedItemPos[i] == stageItemArray[j])
                {
                    stageItems[j] = fixedItem[itemIndex];

                    if (itemIndex + 1 < fixedItem.Length)
                    {
                        itemIndex++;
                    }
                    else
                    {
                        itemIndex = 0;
                    }

                    break;
                }
            }
        }
    }

    private void ResetFruitItems()
    {
        int[] fruitIndex = new int[4];        // 0: 사과, 1: 복숭아, 2: 딸기, 3: 수박
        System.Array.Clear(fruitIndex, 0, fruitIndex.Length);
        int randNum;

        for (int i = 0; i < stageItems.Length; i++)
        {
            if (stageItems[i] == null)
            {
                randNum = Random.Range(0, 4);

                switch (randNum)
                {
                    case 0:     // 사과
                        stageItems[i] = apple[fruitIndex[randNum]];
                        break;

                    case 1:     // 복숭아
                        stageItems[i] = peach[fruitIndex[randNum]];
                        break;

                    case 2:     // 딸기
                        stageItems[i] = strawberry[fruitIndex[randNum]];
                        break;

                    case 3:     // 수박
                        stageItems[i] = watermelon[fruitIndex[randNum]];
                        break;
                }

                if (fruitIndex[randNum] + 1 < 3)
                {
                    fruitIndex[randNum]++;
                }
                else
                {
                    fruitIndex[randNum] = 0;
                }
            }
        }
    }

    private IEnumerator UpdateScoreUI()
    {
        while (!KartGameManager.getInstance.raceFinished)
        {
            score += (KartGameManager.getInstance.GetRoadLineSpeedValue() * 10);
            scoreLabel.text = score.ToString();
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void GetCollectItem(string itemName)
    {
        if (nowItemCount < collectItemList.Count)
        {
            bool isSameItem = itemName.Contains(fruitChoice.ToString().ToLower());

            if (isSameItem)
            {
                collectItemList[nowItemCount].GetComponent<UISprite>().spriteName = fruitChoice.ToString().ToLower() + "_color";
                nowItemCount++;
                score += 2000;
                correctItemGapTime = SET_ITEM_GAP_TIME;

                if (nowItemCount == collectItemList.Count)
                {
                    KartGameManager.getInstance.RaceFinishEvent();
                }
            }
            else
            {
                score += 500;
            }
        }
    }

    /// <summary>
    /// 아이템을 도로에 배치합니다.
    /// </summary>
    public void DisposeItem(GameObject roadLine, int roadMoveRepeat)
    {
        //if (itemPositionIndex < itemPositionNumber.Length && roadMoveRepeat == itemPositionNumber[itemPositionIndex])
        if (roadMoveRepeat == itemPositionNumber[itemPositionIndex])
        {
            MakeItems(roadLine.transform, itemPositionIndex);
            itemPositionIndex++;
        }
    }

    /// <summary>
    /// 아이템을 생성합니다.
    /// </summary>
    private void MakeItems(Transform roadLine, int itemIndex)
    {
        if (!overItemGapTime)
        {
            disposeItem = stageItems[itemIndex].transform;
        }
        else
        {
            disposeItem = gapItems[KartGameManager.getInstance.GetFruitChoiceNum()].transform;
            correctItemGapTime = SET_ITEM_GAP_TIME;
            overItemGapTime = false;
        }

        //stageItems[itemIndex].transform.parent = roadLine;
        //stageItems[itemIndex].transform.localPosition = Vector3.zero;
        disposeItem.parent = roadLine;
        disposeItem.localPosition = Vector3.zero;

        // 랜덤으로 아이템 위치 셋팅
        posNum = previousItemPosNum;
        if (itemPosRepeat)
        {
            posNum = Random.Range(0, 3);
        }
        else
        {
            while (posNum == previousItemPosNum)
            {
                posNum = Random.Range(0, 3);
            }
        }

        switch (posNum)
        {
            case 0:
                disposeItem.Translate(Vector3.left * 8f);
                break;

            case 1:

                break;

            case 2:
                disposeItem.Translate(Vector3.right * 8f);
                break;
        }

        disposeItem.gameObject.SetActive(true);
        previousItemPosNum = posNum;

        //////////////
        if (itemIndex + 1 == stageItems.Length)
        {
            for (int i = 0; i < itemPositionNumber.Length; i++)
            {
                itemPositionNumber[i] += 200;
            }

            itemPositionIndex = 0;
        }
    }

    private IEnumerator CheckCorrectItemGapTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            if (correctItemGapTime > 0)
            {
                correctItemGapTime -= 1;
            }
            else
            {
                overItemGapTime = true;
            }
        }
    }

    public void DeactiveItems()
    {
        for (int i = 0; i < stageItems.Length; i++)
        {
            stageItems[i].SetActive(false);
        }
    }
}
