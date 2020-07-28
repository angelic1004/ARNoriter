using UnityEngine;
using System.Collections;
using HedgehogTeam.EasyTouch;
using HansAR;
using System.Collections.Generic;

public class WordGameManager : MonoBehaviour
{
    int level = 0;

    int[] usedQuestion;

    public BoxCollider col2D;

    private GameObject currentQuestionObj;

    public GameObject questionObjRoot;
    public GameObject diceObjRoot;

    public static WordGameManager instance;

    [HideInInspector]
    public char[] spelling;

    public GameObject ground;

    private GameObject dicePrefab;

    public Transform leftTop;

    public Transform rightBottom;

    private GameObject[] dice;

    private GameObject destroyDice;

    public BoxCollider objCollider;

    public GameObject wall;

    private GameObject preQusetionObj;

    private GameObject[] preDice;

    /// <summary>
    /// 오브젝트를 삭제하기 이전에 이곳으로 옮겨 오브젝트가 혹시나 보이는경우를 방지함
    /// </summary>
    public GameObject trashCan;

    public GameObject sideMenuBtn;

    //public List<GameObject> trashObj;

    int[] usedAlphabet;

    private int diceAmount;

    public int targetIndex;

    private string currentQuestion;

    [HideInInspector]
    public bool[] answerCheck;

    [HideInInspector]
    public bool isPause;

    private bool retryAssetLoad = false;

    void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start()
    {
        retryAssetLoad = false;
        TargetManager.타깃메니저.ApplySceneUI(null);
        WordGameInit();
        //trashObj = new List<GameObject>();
    }

    void OnEnable()
    { 
        StartCoroutine("SetAssetBundleContents");
        EasyTouch.On_SimpleTap += EasyTouch_On_SimpleTap;
        TargetManager.DelTrackingReadyEvent = AfterDiceLoad;
    }

    void UnsubscribeEvent()
    {
        EasyTouch.On_SimpleTap -= EasyTouch_On_SimpleTap;
        TargetManager.DelTrackingReadyEvent = null;
    }

    void OnDisable()
    {
        UnsubscribeEvent();
    }

    void OnDestroy()
    {
        UnsubscribeEvent();
    }

    #region 단어게임 초기화 관련
    public void WordGameInit()
    {
        usedQuestion = new int[TargetManager.타깃메니저.컨텐츠모델링이름.Length];

        level = 1;
    }

    public void SerchTargetIndex(string trackableTargetName)
    {
        string markerNameOfList = string.Empty;

        for (int i = 0; i < TargetManager.타깃메니저.타깃정보.Length; i++)
        {
            markerNameOfList = TargetManager.타깃메니저.타깃정보[i].마커타깃오브젝트.GetComponent<Vuforia.ImageTargetBehaviour>().TrackableName;

            // 뷰포리아에 등록된 마커 이름이 같다면..
            if (string.Compare(trackableTargetName, markerNameOfList, true) == 0)
            {
                targetIndex = i;
                //StartCoroutine("FoundModelLoad");
                CurrentQuestionPosSet(targetIndex);
                break;
            }
        }
    }

    private IEnumerator SetAssetBundleContents()
    {
        while (TargetManager.타깃메니저 == null)
        {
            yield return new WaitForSeconds(0.1f);
        }

        HttpRequestDataSet allDataSet = null;

        allDataSet = new HttpRequestDataSet();

        GlobalDataManager.m_AssetBundlePartName = "wordgame";

        GlobalDataManager.m_SelectedAssetBundleName = string.Format("{0}_{1}", GlobalDataManager.m_ResourceFolderEnum.ToString().ToLower(),
                                                                               GlobalDataManager.m_AssetBundlePartName.ToLower());

        string[] loadAsset = new string[TargetManager.타깃메니저.컨텐츠모델링이름.Length + 1];

        for(int i=0; i< TargetManager.타깃메니저.컨텐츠모델링이름.Length; i++)
        {
            loadAsset[i] = TargetManager.타깃메니저.컨텐츠모델링이름[i];
        }

        loadAsset[loadAsset.Length - 1] = "WordGame_Dice";
        
        TargetManager.타깃메니저.에셋번들복제컨텐츠 = new GameObject[loadAsset.Length];

        AssetBundleLoader.getInstance.SetAssetBundleLoadInfo(allDataSet,
                                                           GlobalDataManager.m_SelectedAssetBundleName,
                                                           null,
                                                           AssetBundleLoader.getInstance.OnLoadCompleteModeling,
                                                           AfterBundleLoadComplete,
                                                           null,
                                                           null);


        AssetBundleLoader.getInstance.SetStorageLoadObject(allDataSet,
                                                          loadAsset,
                                                          TargetManager.타깃메니저.에셋번들복제컨텐츠,
                                                          TargetManager.타깃메니저.모델링오브젝트,
                                                          TargetManager.타깃메니저.AR카메라);


        AssetBundleLoader.getInstance.StartLoadAssetBundle(allDataSet);
    }

    private void AfterBundleLoadComplete(HttpRequestDataSet dataSet)
    {
        OriginalDiceSet(TargetManager.타깃메니저.에셋번들복제컨텐츠.Length - 1);
        TargetManager.타깃메니저.StartVuforia();
    }

    private void AfterDiceLoad()
    {
    }

    //private IEnumerator FoundModelLoad()
    //{
    //    string modelName = TargetManager.타깃메니저.컨텐츠모델링이름[TargetManager.타깃메니저.타깃정보[targetIndex].증강될컨텐츠번호[0]];

    //    HttpRequestDataSet dataSet = null;
    //    dataSet = new HttpRequestDataSet();
    //    GlobalDataManager.m_AssetBundlePartName = "wordgame";

    //    GlobalDataManager.m_SelectedAssetBundleName = string.Format("{0}_{1}", GlobalDataManager.m_ResourceFolderEnum.ToString().ToLower(),
    //                                                                           GlobalDataManager.m_AssetBundlePartName.ToLower());

    //    AssetBundleLoader.getInstance.SetAssetBundleLoadInfo(dataSet,
    //                                                         GlobalDataManager.m_SelectedAssetBundleName,
    //                                                         null,
    //                                                         AssetBundleLoader.getInstance.OnLoadCompleteOnceModeling,
    //                                                         CurrentQuestionPosSet,
    //                                                         null,
    //                                                         null
    //    );

    //    AssetBundleLoader.getInstance.SetStorageLoadObject(dataSet, modelName, gameObject);
    //    AssetBundleLoader.getInstance.StartLoadAssetBundle(dataSet);

    //    yield return null;
    //}
    
    /// <summary>
    /// 주사위 모델 로드
    /// </summary>
    //private IEnumerator DiceLoad()
    //{
    //    if (dicePrefab != null)
    //    {
    //        Destroy(dicePrefab);
    //    }

    //    string diceName = "WordGame_Dice";

    //    HttpRequestDataSet dataSet = null;
    //    dataSet = new HttpRequestDataSet();

    //    GlobalDataManager.m_SelectedAssetBundleName = string.Format("{0}_{1}", GlobalDataManager.m_ResourceFolderEnum.ToString().ToLower(),
    //                                                                           GlobalDataManager.m_AssetBundlePartName.ToLower());

    //    AssetBundleLoader.getInstance.SetAssetBundleLoadInfo(dataSet,
    //                                                         GlobalDataManager.m_SelectedAssetBundleName,
    //                                                         null,
    //                                                         AssetBundleLoader.getInstance.OnLoadCompleteOnceModeling,
    //                                                         OriginalDiceSet,
    //                                                         null,
    //                                                         null
    //    );

    //    AssetBundleLoader.getInstance.SetStorageLoadObject(dataSet, diceName, gameObject);
    //    AssetBundleLoader.getInstance.StartLoadAssetBundle(dataSet);
    //    //AssetBundleLoader.getInstance.ReadAssetBundleWordGame(GlobalDataManager.m_SelectedAssetBundleName,
    //    //diceName, gameObject);

    //    yield return null;
    //}

    public void OriginalDiceSet(int index)
    {
        dicePrefab = TargetManager.타깃메니저.에셋번들복제컨텐츠[index];

        TargetManager.타깃메니저.에셋번들복제컨텐츠[index] = null;

        System.Array.Resize(ref TargetManager.타깃메니저.에셋번들복제컨텐츠, TargetManager.타깃메니저.에셋번들복제컨텐츠.Length - 1);

        dicePrefab.transform.parent = gameObject.transform;
        dicePrefab.transform.localPosition = Vector3.zero;
        dicePrefab.SetActive(false);

        WordUI.instance.gameUI.SetActive(true);

        //SetCurrentQuestion();
    }

    public void CurrentQuestionPosSet(int index)
    {
        //GameObject prefab = dataset.OnceModeling;

        GameObject prefab = Instantiate(TargetManager.타깃메니저.에셋번들복제컨텐츠[TargetManager.타깃메니저.타깃정보[index].증강될컨텐츠번호[0]]);

        currentQuestionObj = prefab;

        currentQuestionObj.transform.parent = questionObjRoot.transform;
        currentQuestionObj.transform.localPosition = Vector3.zero;
        currentQuestionObj.transform.localEulerAngles = Vector3.zero;
        currentQuestionObj.transform.localScale = Vector3.one;

        currentQuestionObj.SetActive(true);

        SetCurrentQuestion();
    }

    #endregion

    #region 게임 시작 관련
    public void GameStart(string targetName)
    {
        AutoFocusMode.getInstance.OnOffAutoFucousMode(false);

        col2D.enabled = false;

        CheckPreObjClean();

        SerchTargetIndex(targetName);
        level = 1;
    }

    //public int RandomQuestionSelect(int index)
    //{
    //    int calcCol = 0;
    //    while (true)
    //    {
    //        int random = Random.Range(0, index);

    //        for (int i = 0; i < index; i++)
    //        {
    //            if (usedQuestion[i] == random)
    //            {
    //                if (calcCol >= 100)
    //                {
    //                    Debug.LogError("100회 넘음");
    //                    return -1;
    //                }
    //                else
    //                {
    //                    calcCol += 1;
    //                    continue;
    //                }
    //            }
    //        }

    //        return random;
    //    }
    //}

    /// <summary>
    /// 현재 문제 세팅(현재 문제를 명시해줄 알파뱃 카드를 초기화 하고, 다음 매서드를 실행)
    /// </summary>
    /// <param name="level"></param>
    public void SetCurrentQuestion()
    {
        try
        {
            //임시
            TargetManager.타깃메니저.컨텐츠최상위오브젝트.transform.localPosition = TargetManager.타깃메니저.비인식후_좌표값;
            TargetManager.타깃메니저.컨텐츠최상위오브젝트.transform.localEulerAngles = TargetManager.타깃메니저.비인식후_회전값;
            TargetManager.타깃메니저.컨텐츠최상위오브젝트.transform.localScale = TargetManager.타깃메니저.비인식후_사이즈값;

            currentQuestion = currentQuestionObj.GetComponent<ModelInfo>().공부하기속성.영어_텍스트.ToUpper();

            currentQuestion = currentQuestion.Trim();

            spelling = new char[currentQuestion.Length];

            WordUI.instance.alphabetCard = new GameObject[currentQuestion.Length];

            answerCheck = new bool[currentQuestion.Length];

            WordUI.instance.CreateAlphabetCard(currentQuestion, currentQuestion.Length);

            if (spelling.Length <= 5)
            {
                diceAmount = 8;
            }
            else
            {
                diceAmount = spelling.Length + 3;
            }

            dice = new GameObject[diceAmount];
            usedAlphabet = new int[diceAmount];

            DiceAlphabetSet();
        }
        catch (System.Exception e)
        {
            if (retryAssetLoad == false)
            {
                Debug.LogError("Qestion Create Retry [Error : " + e + "]");
                StopAllCoroutines();
                CheckPreObjClean();
                StartCoroutine("DiceLoad");
                retryAssetLoad = true;
            }
            else
            {
                Debug.LogError("Reloading the asset bundle failed.Please Check AssetBundle");
            }
        }
    }

    /// <summary>
    /// 주사위에 들어갈 알파뱃의 아스키코드를 세팅
    /// </summary>
    public void DiceAlphabetSet()
    {
        int questionAlphabet = 0;

        //정답 다이스 세팅
        while (true)
        {
            int num = Random.Range(0, usedAlphabet.Length);

            if (usedAlphabet[num] == 0)
            {
                usedAlphabet[num] = System.Convert.ToInt32(spelling[questionAlphabet]) - 64;

                questionAlphabet++;
            }
            else
            {
                continue;
            }

            if (questionAlphabet == spelling.Length)
            {
                break;
            }
        }

        //정답을 제외한 다이스 세팅
        for (int i = 0; i < dice.Length; i++)
        {
            if (usedAlphabet[i] == 0)
            {
                bool breaker = false;

                while (true)
                {
                    int num = Random.Range(1, 27);

                    for (int j = 0; j <= dice.Length; j++)
                    {
                        if (j == dice.Length)
                        {
                            usedAlphabet[i] = num;

                            breaker = true;
                        }
                        else if (usedAlphabet[j] == num)
                        {
                            break;
                        }
                    }

                    if (breaker)
                    {
                        break;
                    }
                }
            }
        }

        StartCoroutine("DiceSet");
    }

    /// <summary>
    /// 세팅된 아스키코드를 토대로 주사위를 생성
    /// </summary>
    /// <returns></returns>
    public IEnumerator DiceSet()
    {
        float xPoint = 0;
        float zPoint = 0;

        //answerIndex = Random.Range(0, dice.Length);

        WordUI.instance.loadingUI.SetActive(false);
        WordUI.instance.isRecog = false;

        for (int i = 0; i < diceAmount; i++)
        {
            if (xPoint != 0)
            {
                if (xPoint >= 0)
                {
                    xPoint = Random.Range(leftTop.position.x, rightBottom.position.x / 2);
                }
                else
                {
                    xPoint = Random.Range(leftTop.position.x / 2, rightBottom.position.x);
                }
            }
            else
            {
                xPoint = Random.Range(leftTop.position.x, rightBottom.position.x);
            }

            zPoint = Random.Range(leftTop.position.z, rightBottom.position.z);

            dice[i] = Instantiate(dicePrefab);
            dice[i].SetActive(true);
            dice[i].transform.parent = diceObjRoot.transform;
            dice[i].transform.localScale = Vector3.one * 50;
            dice[i].transform.localEulerAngles = new Vector3(45, 0, 45);
            dice[i].transform.localPosition = new Vector3(xPoint, 20, zPoint);
            dice[i].GetComponent<Rigidbody>().AddForce(Vector3.down * 1000);

            dice[i].AddComponent<DiceInfo>();
            dice[i].GetComponent<DiceInfo>().alphabet = System.Convert.ToChar(usedAlphabet[i] + 64);
            dice[i].GetComponent<DiceInfo>().meshRenderer.material = dice[i].GetComponent<DiceInfo>().alphabetMaterial[usedAlphabet[i] - 1];

            GlobalDataManager.ShaderRefresh(dice[i]);

            yield return new WaitForSeconds(0.2f);
        }
    }
    #endregion

    #region 버튼 이벤트
    public void DiceSelect(GameObject obj)
    {
        char alphabet = obj.GetComponent<DiceInfo>().alphabet;

        for (int i = 0; i < spelling.Length; i++)
        {
            if (alphabet == spelling[i] && answerCheck[i] == false)
            {
                if (WordUI.instance.alphabetCard[i] != null)
                {
                    WordUI.instance.AlphabetCardAlphaColorSet(WordUI.instance.alphabetCard[i].transform.GetChild(0).GetComponent<UILabel>(), 255f);
                    WordUI.instance.AlphabetCardColorSet(WordUI.instance.alphabetCard[i]);

                    destroyDice = obj;
                    StartCoroutine("DestroySelectDiceObj");

                    answerCheck[i] = true;

                    ClearCheck();
                }

                return;
            }
        }

        StartCoroutine(WrongDiceTouch(obj));
    }

    private IEnumerator WrongDiceTouch(GameObject obj)
    {

        for (float i = 0; i < 20; i++)
        {
            obj.GetComponent<Rigidbody>().AddForce(new Vector3(0, 50, 0), ForceMode.Acceleration);

            obj.GetComponent<Rigidbody>().AddTorque(new Vector3(-25, 0, 0));
        }

        yield return new WaitForSeconds(1.5f);

        for (float i = 0; i < 20; i++)
        {
            obj.GetComponent<Rigidbody>().AddForce(new Vector3(0, -200 * (i / 20), 0), ForceMode.Acceleration);
            yield return new WaitForSeconds(0.1f);
        }

    }

    IEnumerator DestroySelectDiceObj()
    {
        GameObject touchedDice = destroyDice;

        for (int i = 0; i < 10; i++)
        {
            touchedDice.transform.localScale = new Vector3(touchedDice.transform.localScale.x, touchedDice.transform.localScale.y, touchedDice.transform.localScale.z) * (10 - i) / 10;

            yield return new WaitForFixedUpdate();
        }

        touchedDice.SetActive(false);
        Destroy(touchedDice);
    }

    #endregion

    private void ClearCheck()
    {
        for (int i = 0; i < answerCheck.Length; i++)
        {
            if (answerCheck[i] == false)
            {
                return;
            }
        }

        //currentQuestionObj.GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animation>().clip = currentQuestionObj.GetComponent<ModelInfo>().애니메이션정보.애니클립[0];
        //currentQuestionObj.GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animation>().Play();

        sideMenuBtn.GetComponent<BoxCollider2D>().enabled = false;

        WordUI.instance.pauseBtn.GetComponent<BoxCollider2D>().enabled = false;
        WordUI.instance.resultUI.SetActive(true);
        if (level <= 2)
        {
            WordUI.instance.nextLevelBtn.SetActive(true);

        }
        else
        {
            WordUI.instance.nextLevelBtn.SetActive(false);
        }
    }

    public void RetrySet()
    {
        StopAllCoroutines();
        DestroyDiceObj();
        WordUI.instance.AlphabetCardDestry();

        SetCurrentQuestion();

        WordUI.instance.AlphabetCardLevelSet(level);
    }

    /// <summary>
    /// 레벨이 2거나 3일때는 알파뱃을 숨깁니다.
    /// </summary>
    public void NextLevelSet()
    {
        StopAllCoroutines();

        level++;

        DestroyDiceObj();
        WordUI.instance.AlphabetCardDestry();

        SetCurrentQuestion();

        WordUI.instance.AlphabetCardLevelSet(level);
    }

    #region 터치 이벤트
    public void EasyTouch_On_SimpleTap(Gesture ges)
    {
        if (isPause == false)
        {
            if (ges.pickedObject != null && ges.pickedObject.tag == "dice")
            {
                DiceSelect(ges.pickedObject);
            }
        }
    }

    public void DestroyDiceObj()
    {
        TrashDestroyStop();

        preDice = dice;
        dice = null;

        for (int i = 0; i < preDice.Length; i++)
        {
            if (preDice[i] != null)
            {
                preDice[i].transform.parent = trashCan.transform;
                preDice[i].transform.localScale = Vector3.zero;
                preDice[i].SetActive(false);
                //trashObj.Add(preDice[i]);
            }
        }

        TrashDestroyStart();
        preDice = null;
    }

    public void QusetionObjDestroy()
    {
        if (currentQuestionObj != null)
        {
            TrashDestroyStop();

            preQusetionObj = currentQuestionObj;
            preQusetionObj.transform.parent = trashCan.transform;
            currentQuestionObj = null;

            preQusetionObj.transform.localScale = Vector3.zero;
            preQusetionObj.SetActive(false);
            //trashObj.Add(preQusetionObj);

            TrashDestroyStart();
            preQusetionObj = null;
        }
    }

    public void GameQuit()
    {
        StopAllCoroutines();

        DestroyDiceObj();
        QusetionObjDestroy();
        WordUI.instance.AlphabetCardDestry();

        col2D.enabled = true;

        MainUI.메인.인식글자UI.SetActive(true);

        AutoFocusMode.getInstance.OnOffAutoFucousMode(true);
    }

    public void TrashDestroyStop()
    {
        StopCoroutine("TrashDestroy");
    }

    public void TrashDestroyStart()
    {
        StartCoroutine("TrashDestroy");
    }

    private IEnumerator TrashDestroy()
    {
        GameObject destroyObj;

        for (int i = 0; i < trashCan.transform.childCount; i++)
        {
            destroyObj = trashCan.transform.GetChild(i).gameObject;
            //trashObj.RemoveAt(i);
            Destroy(destroyObj);
        }

        yield return new WaitForFixedUpdate();
    }

    private void CheckPreObjClean()
    {
        if (questionObjRoot.transform.childCount != 0)
        {
            for (int i = 0; i < questionObjRoot.transform.childCount; i++)
            {
                GameObject destroyObj = questionObjRoot.transform.GetChild(i).gameObject;
                Destroy(destroyObj);
            }
        }
        if (diceObjRoot.transform.childCount != 0)
        {
            for (int i = 0; i < diceObjRoot.transform.childCount; i++)
            {
                GameObject destroyObj = diceObjRoot.transform.GetChild(i).gameObject;
                Destroy(destroyObj);
            }
        }
        if (WordUI.instance.parentAlphabetCard.transform.childCount != 0)
        {
            for (int i = 0; i < WordUI.instance.parentAlphabetCard.transform.childCount; i++)
            {
                GameObject destroyObj = WordUI.instance.parentAlphabetCard.transform.GetChild(i).gameObject;
                Destroy(destroyObj);
            }
        }
    }
}
#endregion