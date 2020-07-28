using UnityEngine;
using System.Collections;

using HansAR;
using UnityEngine.SceneManagement;

public class MazeGameManager : MonoBehaviour
{
    /// <summary>
    /// 현재 레벨
    /// </summary>
    public static int level;

    /// <summary>
    /// 현재 씬의 이름(앞쪽을 잘라, 미로맵 오브젝트의 이름을 가져옴)
    /// </summary>
    public static string sceneName;

    /// <summary>
    /// 불러올 오브젝트의 인덱스 번호(MazeSelect씬에서 가져옴)
    /// </summary>
    public static int targetIndex;

    /// <summary>
    /// 미니맵에서의 오브젝트 위치를 표시하는 오브젝트
    /// </summary>
    public GameObject moveObjMinimapPos;

    /// <summary>
    /// 오브젝트를 따라다니는 카메라
    /// </summary>
    public Camera gameCamera;

    /// <summary>
    /// 이동하고있는 현재 오브젝트
    /// </summary>
    private GameObject movedObj;

    /// <summary>
    /// 현재 맵
    /// </summary>
    [HideInInspector]
    public GameObject map;

    /// <summary>
    /// 현재 위치
    /// </summary>
    private GameObject currentPos;

    /// <summary>
    /// 이전 위치
    /// </summary>
    private GameObject prePos;

    /// <summary>
    /// 현재 위치의 방향 인포
    /// </summary>
    private MazePosInfo currentMazePosInfo;

    /// <summary>
    /// 현재 오브젝트 navMesh
    /// </summary>
    private NavMeshAgent agent;

    /// <summary>
    /// 외길인지 아닌지
    /// </summary>
    private bool noEntryWay;

    public static MazeGameManager instance;

    /// <summary>
    /// 위쪽 화살표를 선택했을떄의 다음 이동할 위치정보 오브젝트
    /// </summary>
    private GameObject selectedFrontPos;

    /// <summary>
    /// 왼쪽 화살표를 선택했을떄의 다음 이동할 위치정보 오브젝트
    /// </summary>
    private GameObject selectedLeftPos;

    /// <summary>
    /// 아래쪽 화살표를 선택했을떄의 다음 이동할 위치정보 오브젝트
    /// </summary>
    private GameObject selectedRightPos;

    /// <summary>
    /// 맵 이름
    /// </summary>
    private string mapName;

    private bool retryAssetLoad = false;

    public bool usingModelAni;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        AssetLoading();
        //TargetManager.타깃메니저.ApplySceneUI(null);
        MainUI.메인.uiEventLinkManager.SetActive(false);
        MainUI.메인.인식글자UI.SetActive(false);

        MazeGameUI.instance.ArrowSetActiveAll(false);

        AutoFocusMode.getInstance.OnOffAutoFucousMode(false);

        TargetManager.타깃메니저.StartVuforia();
    }

    /// <summary>
    /// 에셋번들을 로드합니다.
    /// </summary>
    public void AssetLoading()
    {
        GlobalDataManager.m_AssetBundlePartName = "mazegame";
        //StartCoroutine("LoadAsset");
        StartCoroutine("mapLoad");
    }

    /// <summary>
    /// 움직일 오브젝트 로드
    /// </summary>
    /// <returns></returns>
    //public IEnumerator LoadAsset()
    //{
    //    HttpRequestDataSet dataSet = null;
    //    dataSet = new HttpRequestDataSet();

    //    GlobalDataManager.m_SelectedAssetBundleName = string.Format("{0}_{1}", GlobalDataManager.m_ResourceFolderEnum.ToString().ToLower(),
    //                                                                           GlobalDataManager.m_AssetBundlePartName.ToLower());

    //    string[] loadAssetName = new string[2];

    //    string scenename = SceneManager.GetActiveScene().name;

    //    mapName = scenename.Substring(scenename.Length - 7, 7);
    //    loadAssetName[0] = mapName;

    //    loadAssetName[1] = TargetManager.타깃메니저.컨텐츠모델링이름[TargetManager.타깃메니저.타깃정보[targetIndex].증강될컨텐츠번호[0]];


    //    TargetManager.타깃메니저.에셋번들복제컨텐츠 = new GameObject[loadAssetName.Length];

    //    AssetBundleLoader.getInstance.SetAssetBundleLoadInfo(dataSet,
    //                                                         GlobalDataManager.m_SelectedAssetBundleName,
    //                                                         null,
    //                                                         AssetBundleLoader.getInstance.OnLoadCompleteModeling,
    //                                                         AssetInit,
    //                                                         null,
    //                                                         null
    //    );

    //    AssetBundleLoader.getInstance.SetStorageLoadObject(dataSet, loadAssetName, TargetManager.타깃메니저.에셋번들복제컨텐츠, gameObject, TargetManager.타깃메니저.AR카메라);
    //    AssetBundleLoader.getInstance.StartLoadAssetBundle(dataSet);

    //    yield return null;
    //    //AssetBundleLoader.getInstance.ReadAssetBundleMazeGame(GlobalDataManager.m_SelectedAssetBundleName, movedObjName, gameObject);
    //}

    public void AssetInit(HttpRequestDataSet dataSet)
    {
        for (int i = 0; i < dataSet.contentsModelingNames.Length; i++)
        {
            if (dataSet.contentsModelingNames[i] == TargetManager.타깃메니저.컨텐츠모델링이름[TargetManager.타깃메니저.타깃정보[targetIndex].증강될컨텐츠번호[0]])
            {
                movedObj = dataSet.assetBundleCopyObjects[i];

                movedObj.GetComponent<NavMeshAgent>().enabled = false;

                movedObj.transform.position = map.GetComponent<MazeInfo>().mazePosParent.transform.GetChild(0).transform.position;

                movedObj.transform.localScale = Vector3.one * 5;

                movedObj.AddComponent<Rigidbody>();

                movedObj.GetComponent<NavMeshAgent>().enabled = true;

                agent = movedObj.GetComponent<NavMeshAgent>();

                movedObj.SetActive(true);

                currentPos = map.GetComponent<MazeInfo>().mazePosParent.transform.GetChild(0).gameObject;

                currentMazePosInfo = map.GetComponent<MazeInfo>().mazePosParent.transform.GetChild(0).GetComponent<MazePosInfo>();

                gameCamera.gameObject.transform.position = movedObj.transform.position;
                gameCamera.gameObject.transform.position = new Vector3(gameCamera.gameObject.transform.position.x, gameCamera.gameObject.transform.position.y + 4.0f, gameCamera.gameObject.transform.position.z - 5.0f);
                gameCamera.gameObject.transform.eulerAngles = new Vector3(15, 0, 0);

                gameCamera.transform.parent = movedObj.transform;

                if (usingModelAni)
                {
                    if (movedObj.GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animation>() != null && movedObj.GetComponent<ModelInfo>().애니메이션정보.애니클립.Length >= 2)
                    {
                        movedObj.GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animation>().clip = movedObj.GetComponent<ModelInfo>().애니메이션정보.애니클립[0];
                        movedObj.GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animation>().wrapMode = WrapMode.Loop;
                        movedObj.GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animation>().Play();
                    }
                    else
                    {
                        usingModelAni = false;
                        Debug.LogWarning("No Entry Animation");
                    }
                }

                MazeGameUI.instance.MinimapInitCall();
            }
            else
            {
                map = dataSet.assetBundleCopyObjects[i];

                map.transform.position = Vector3.zero;
                map.transform.eulerAngles = Vector3.zero;
                map.transform.localScale = Vector3.one * 50;

                map.SetActive(true);
            }
        }
    }

    /// <summary>
    /// 움직일 오브젝트 로드
    /// </summary>
    /// <returns></returns>
    public IEnumerator movingObjLoad()
    {
        HttpRequestDataSet dataSet = null;
        dataSet = new HttpRequestDataSet();

        GlobalDataManager.m_SelectedAssetBundleName = string.Format("{0}_{1}", GlobalDataManager.m_ResourceFolderEnum.ToString().ToLower(),
                                                                               GlobalDataManager.m_AssetBundlePartName.ToLower());

        AssetBundleLoader.getInstance.SetAssetBundleLoadInfo(dataSet,
                                                             GlobalDataManager.m_SelectedAssetBundleName,
                                                             null,
                                                             AssetBundleLoader.getInstance.OnLoadCompleteOnceModeling,
                                                             MovedObjAssetInit,
                                                             null,
                                                             null
        );

        AssetBundleLoader.getInstance.SetStorageLoadObject(dataSet, TargetManager.타깃메니저.컨텐츠모델링이름[TargetManager.타깃메니저.타깃정보[targetIndex].증강될컨텐츠번호[0]], gameObject);
        AssetBundleLoader.getInstance.StartLoadAssetBundle(dataSet);

        yield return null;
        //AssetBundleLoader.getInstance.ReadAssetBundleMazeGame(GlobalDataManager.m_SelectedAssetBundleName, movedObjName, gameObject);
    }

    /// <summary>
    /// 맵 로드
    /// </summary>
    /// <returns></returns>
    public IEnumerator mapLoad()
    {
        yield return new WaitForSeconds(0.1f);

        HttpRequestDataSet dataSet = null;
        dataSet = new HttpRequestDataSet();

        string scenename = SceneManager.GetActiveScene().name;

        mapName = scenename.Substring(scenename.Length - 7, 7);


        GlobalDataManager.m_SelectedAssetBundleName = string.Format("{0}_{1}", GlobalDataManager.m_ResourceFolderEnum.ToString().ToLower(),
                                                                               GlobalDataManager.m_AssetBundlePartName.ToLower());

        AssetBundleLoader.getInstance.SetAssetBundleLoadInfo(dataSet,
                                                             GlobalDataManager.m_SelectedAssetBundleName,
                                                             null,
                                                             AssetBundleLoader.getInstance.OnLoadCompleteOnceModeling,
                                                             MapAssetInit,
                                                             null,
                                                             null
        );
        AssetBundleLoader.getInstance.SetStorageLoadObject(dataSet, mapName, gameObject);
        AssetBundleLoader.getInstance.StartLoadAssetBundle(dataSet);

        //AssetBundleLoader.getInstance.ReadAssetBundleMazeGame(GlobalDataManager.m_SelectedAssetBundleName, mapName, gameObject);
    }

    public void MapAssetInit(HttpRequestDataSet dataSet)
    {
        GameObject prefab = null;
        prefab = dataSet.OnceModeling;

        if (prefab.name == mapName)
        {
            map = prefab;

            map.transform.position = Vector3.zero;
            map.transform.eulerAngles = Vector3.zero;
            map.transform.localScale = Vector3.one * 50;

            map.SetActive(true);

            StartCoroutine("movingObjLoad");
        }
    }

    /// <summary>
    /// 에셋로드 후속처리 매서드
    /// </summary>
    public void MovedObjAssetInit(HttpRequestDataSet dataSet)
    {
        try
        {
            GameObject prefab = null;
            prefab = dataSet.OnceModeling;

            if (prefab.name == TargetManager.타깃메니저.컨텐츠모델링이름[TargetManager.타깃메니저.타깃정보[targetIndex].증강될컨텐츠번호[0]])
            {
                movedObj = prefab;

                movedObj.GetComponent<NavMeshAgent>().enabled = false;

                movedObj.transform.position = map.GetComponent<MazeInfo>().mazePosParent.transform.GetChild(0).transform.position;

                movedObj.transform.localScale = Vector3.one * 5;

                movedObj.AddComponent<Rigidbody>();

                movedObj.GetComponent<NavMeshAgent>().enabled = true;

                agent = movedObj.GetComponent<NavMeshAgent>();

                movedObj.SetActive(true);

                currentPos = map.GetComponent<MazeInfo>().mazePosParent.transform.GetChild(0).gameObject;

                currentMazePosInfo = map.GetComponent<MazeInfo>().mazePosParent.transform.GetChild(0).GetComponent<MazePosInfo>();

                gameCamera.gameObject.transform.position = movedObj.transform.position;
                gameCamera.gameObject.transform.position = new Vector3(gameCamera.gameObject.transform.position.x, gameCamera.gameObject.transform.position.y + 4.0f, gameCamera.gameObject.transform.position.z - 5.0f);
                gameCamera.gameObject.transform.eulerAngles = new Vector3(15, 0, 0);

                gameCamera.transform.parent = movedObj.transform;

                if (usingModelAni)
                {
                    if (movedObj.GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animation>() != null && movedObj.GetComponent<ModelInfo>().애니메이션정보.애니클립.Length >= 2)
                    {
                        movedObj.GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animation>().clip = movedObj.GetComponent<ModelInfo>().애니메이션정보.애니클립[0];
                        movedObj.GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animation>().wrapMode = WrapMode.Loop;
                        movedObj.GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animation>().Play();
                    }
                    else
                    {
                        usingModelAni = false;
                        Debug.LogWarning("No Entry Animation");
                    }
                }

                MazeGameUI.instance.MinimapInitCall();
            }
            else
            {
                Debug.LogError("Wrong Asset. Please Check Asset\n 잘못된 에셋입니다. 에셋을 다시 확인해주세요");
            }
        }
        catch (System.Exception e)
        {
            RetryAssetLoad(e);
        }
    }

    /// <summary>
    /// 게임 시작
    /// </summary>
    public void GameStart()
    {
        //시작합니다 안내후 처음에 자동으로 갈림길까지 감

        Debug.Log("게임 시작");
        WhatKindPath();

        MainUI.메인.인식글자UI.SetActive(false);
    }

    /// <summary>
    /// 무슨길인지 판단함(외길,막다른길,갈림길,골)
    /// </summary>
    public void WhatKindPath()
    {
        //골
        if (currentMazePosInfo.goalPos)
        {
            if (usingModelAni)
            {
                movedObj.GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animation>().clip = movedObj.GetComponent<ModelInfo>().애니메이션정보.애니클립[0];
                movedObj.GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animation>().wrapMode = WrapMode.Loop;
                movedObj.GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animation>().Play();
            }

            MazeGameUI.instance.InputExplanationText("goalText");

            MazeGameUI.instance.resultUI.SetActive(true);
            MazeGameUI.instance.minimapBtn.GetComponent<Collider2D>().enabled = false;
            MazeGameUI.instance.pauseBtn.GetComponent<Collider2D>().enabled = false;

            if (level >= 3)
            {
                MazeGameUI.instance.nextLevelBtn.SetActive(false);
            }
            else
            {
                MazeGameUI.instance.nextLevelBtn.SetActive(true);
            }
            //다음레벨 버튼 추가
        }
        else
        {
            //막다른길이 아니라면
            if (!currentMazePosInfo.deadEndPos)
            {
                if (currentMazePosInfo.roadCount == 2)
                {
                    if (currentMazePosInfo.upPos != null
                        && prePos != currentMazePosInfo.upPos)
                    {
                        MoveObject(currentMazePosInfo.upPos);
                    }
                    else if (currentMazePosInfo.downPos != null
                        && prePos != currentMazePosInfo.downPos)
                    {
                        MoveObject(currentMazePosInfo.downPos);
                    }
                    else if (currentMazePosInfo.leftPos != null
                        && prePos != currentMazePosInfo.leftPos)
                    {
                        MoveObject(currentMazePosInfo.leftPos);
                    }
                    else if (currentMazePosInfo.rightPos != null
                        && prePos != currentMazePosInfo.rightPos)
                    {
                        MoveObject(currentMazePosInfo.rightPos);
                    }
                    else
                    {
                        Debug.LogError("길을 찾을 수 없음 Info를 확인바람");
                    }
                }
                //갈림길을 만났을때
                else
                {
                    //막힌길 카운팅
                    int passedCount = 0;

                    GameObject upPosBtn = null;
                    GameObject downPosBtn = null;
                    GameObject leftPosBtn = null;
                    GameObject rightPosBtn = null;

                    if (prePos == currentMazePosInfo.upPos)
                    {
                        downPosBtn = MazeGameUI.instance.frontArrow;

                        leftPosBtn = MazeGameUI.instance.rightArrow;
                        rightPosBtn = MazeGameUI.instance.leftArrow;

                        selectedFrontPos = currentMazePosInfo.downPos;
                        selectedLeftPos = currentMazePosInfo.rightPos;
                        selectedRightPos = currentMazePosInfo.leftPos;

                    }
                    else if (prePos == currentMazePosInfo.downPos)
                    {
                        upPosBtn = MazeGameUI.instance.frontArrow;

                        leftPosBtn = MazeGameUI.instance.leftArrow;
                        rightPosBtn = MazeGameUI.instance.rightArrow;

                        selectedFrontPos = currentMazePosInfo.upPos;
                        selectedLeftPos = currentMazePosInfo.leftPos;
                        selectedRightPos = currentMazePosInfo.rightPos;
                    }
                    else if (prePos == currentMazePosInfo.leftPos)
                    {
                        upPosBtn = MazeGameUI.instance.leftArrow;
                        downPosBtn = MazeGameUI.instance.rightArrow;

                        rightPosBtn = MazeGameUI.instance.frontArrow;

                        selectedFrontPos = currentMazePosInfo.rightPos;
                        selectedLeftPos = currentMazePosInfo.upPos;
                        selectedRightPos = currentMazePosInfo.downPos;
                    }
                    else if (prePos == currentMazePosInfo.rightPos)
                    {
                        upPosBtn = MazeGameUI.instance.rightArrow;
                        downPosBtn = MazeGameUI.instance.leftArrow;

                        leftPosBtn = MazeGameUI.instance.frontArrow;

                        selectedFrontPos = currentMazePosInfo.leftPos;
                        selectedLeftPos = currentMazePosInfo.downPos;
                        selectedRightPos = currentMazePosInfo.upPos;
                    }

                    if (currentMazePosInfo.upPos != null)
                    {
                        if (currentMazePosInfo.upPos.GetComponent<MazePosInfo>().isPassed)
                        {
                            passedCount += 1;

                            if (upPosBtn != null)
                            {
                                upPosBtn.SetActive(false);
                            }
                        }
                        else
                        {
                            if (upPosBtn != null)
                            {
                                upPosBtn.SetActive(true);
                            }
                        }
                    }
                    else
                    {
                        if (upPosBtn != null)
                        {
                            upPosBtn.SetActive(false);
                        }
                    }

                    if (currentMazePosInfo.downPos != null)
                    {
                        if (currentMazePosInfo.downPos.GetComponent<MazePosInfo>().isPassed)
                        {
                            passedCount += 1;

                            if (downPosBtn != null)
                            {
                                downPosBtn.SetActive(false);
                            }
                        }
                        else
                        {
                            if (downPosBtn != null)
                            {
                                downPosBtn.SetActive(true);
                            }

                        }
                    }
                    else
                    {
                        if (downPosBtn != null)
                        {
                            downPosBtn.SetActive(false);
                        }
                    }

                    if (currentMazePosInfo.leftPos != null)
                    {
                        if (currentMazePosInfo.leftPos.GetComponent<MazePosInfo>().isPassed)
                        {
                            passedCount += 1;

                            if (leftPosBtn != null)
                            {
                                leftPosBtn.SetActive(false);
                            }
                        }
                        else
                        {
                            if (leftPosBtn != null)
                            {
                                leftPosBtn.SetActive(true);
                            }

                        }
                    }
                    else
                    {
                        if (leftPosBtn != null)
                        {
                            leftPosBtn.SetActive(false);
                        }
                    }

                    if (currentMazePosInfo.rightPos != null)
                    {
                        if (currentMazePosInfo.rightPos.GetComponent<MazePosInfo>().isPassed)
                        {
                            passedCount += 1;
                            if (rightPosBtn != null)
                            {
                                rightPosBtn.SetActive(false);
                            }
                        }
                        else
                        {
                            if (rightPosBtn != null)
                            {
                                rightPosBtn.SetActive(true);
                            }
                        }
                    }
                    else
                    {
                        if (rightPosBtn != null)
                        {
                            rightPosBtn.SetActive(false);
                        }
                    }

                    if (prePos.GetComponent<MazePosInfo>().isPassed == true)
                    {
                        MazeGameUI.instance.returnArrow.SetActive(false);
                    }
                    else
                    {
                        MazeGameUI.instance.returnArrow.SetActive(true);
                    }

                    //갈림길에서 남은길이 하나뿐일떄, 지나가면서 다시못오게 길을 막아줍니다.
                    if (passedCount == currentMazePosInfo.roadCount - 1)
                    {
                        noEntryWay = true;
                    }
                    else
                    {
                        noEntryWay = false;
                    }

                    if (usingModelAni)
                    {
                        movedObj.GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animation>().clip = movedObj.GetComponent<ModelInfo>().애니메이션정보.애니클립[0];
                        movedObj.GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animation>().wrapMode = WrapMode.Loop;
                        movedObj.GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animation>().Play();
                    }

                    MazeGameUI.instance.InputExplanationText("selectArrowText");
                }
            }
            //막다른길
            else
            {
                noEntryWay = true;

                if (prePos != null)
                {
                    if (usingModelAni)
                    {
                        movedObj.GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animation>().clip = movedObj.GetComponent<ModelInfo>().애니메이션정보.애니클립[0];
                        movedObj.GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animation>().wrapMode = WrapMode.Loop;
                        movedObj.GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animation>().Play();
                    }

                    MazeGameUI.instance.InputExplanationText("deadEndText");
                    MazeGameUI.instance.returnBtn.SetActive(true);
                }
                //처음 시작지점은 이전 지점이 없으므로 나있는 길을 찾아준다.
                else
                {
                    if (currentMazePosInfo.upPos != null)
                    {
                        MoveObject(currentMazePosInfo.upPos);
                    }
                    else if (currentMazePosInfo.downPos != null)
                    {
                        MoveObject(currentMazePosInfo.downPos);
                    }
                    else if (currentMazePosInfo.leftPos != null)
                    {
                        MoveObject(currentMazePosInfo.leftPos);
                    }
                    else if (currentMazePosInfo.rightPos != null)
                    {
                        MoveObject(currentMazePosInfo.rightPos);
                    }
                    else
                    {
                        Debug.LogError("길을 찾을 수 없음 Info를 확인바람");
                    }
                }
            }
        }
    }

    /// <summary>
    /// 막다른길 버튼 터치시 이동 매서드
    /// </summary>
    public void ReturnPreRoad()
    {
        MazeGameUI.instance.ArrowSetActiveAll(false);
        MoveObject(prePos);
    }

    /// <summary>
    /// 방향선택시 실질적으로 이동시켜주는 매서드
    /// </summary>
    /// <param name="movingPos">가려하는 위치 오브젝트</param>
    public void MoveObject(GameObject movingPos)
    {
        if (noEntryWay)
        {
            currentPos.GetComponent<MazePosInfo>().isPassed = true;
        }
        prePos = currentPos;
        currentPos = movingPos;
        currentMazePosInfo = currentPos.GetComponent<MazePosInfo>();

        agent.SetDestination(movingPos.transform.position);

        MazeGameUI.instance.InputExplanationText("moveText");

        if (usingModelAni)
        {
            movedObj.GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animation>().clip = movedObj.GetComponent<ModelInfo>().애니메이션정보.애니클립[1];
            movedObj.GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animation>().wrapMode = WrapMode.Loop;
            movedObj.GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animation>().Play();
        }

        StartCoroutine("UpdateMovingState");

    }

    /// <summary>
    /// 실시간으로 오브젝트의 위치를 파악해 도착위치에 근접했다면 다음 방향을 지정함
    /// </summary>
    /// <returns></returns>
    public IEnumerator UpdateMovingState()
    {
        while (true)
        {
            if (Vector3.Distance(new Vector3(movedObj.transform.position.x, 0, movedObj.transform.position.z),
                new Vector3(currentPos.transform.position.x, 0, currentPos.transform.position.z)) <= 0.5f)
            {
                agent.Stop();
                agent.ResetPath();
                WhatKindPath();
                break;
            }
            yield return new WaitForFixedUpdate();
        }
    }

    /// <summary>
    /// 갈림길 선택
    /// </summary>
    /// <param name="selectArrow">선택한 방향</param>
    public void SelectIntersection(GameObject selectArrow)
    {
        if (selectArrow == MazeGameUI.instance.frontArrow)
        {
            MoveObject(selectedFrontPos);
        }
        else if (selectArrow == MazeGameUI.instance.leftArrow)
        {
            MoveObject(selectedLeftPos);
        }
        else if (selectArrow == MazeGameUI.instance.rightArrow)
        {
            MoveObject(selectedRightPos);
        }

        MazeGameUI.instance.ArrowSetActiveAll(false);
    }

    /// <summary>
    /// 게임 재시작 이벤트
    /// </summary>
    public void GameRetry()
    {
        map.SetActive(false);

        map.SetActive(true);

        movedObj.GetComponent<NavMeshAgent>().enabled = false;

        movedObj.transform.position = map.GetComponent<MazeInfo>().mazePosParent.transform.GetChild(0).transform.position;

        movedObj.transform.localEulerAngles = Vector3.zero;

        currentPos = map.GetComponent<MazeInfo>().mazePosParent.transform.GetChild(0).gameObject;

        currentMazePosInfo = map.GetComponent<MazeInfo>().mazePosParent.transform.GetChild(0).GetComponent<MazePosInfo>();

        prePos = null;

        movedObj.GetComponent<NavMeshAgent>().enabled = true;

        MazeGameUI.instance.startBtn.SetActive(true);
        MazeGameUI.instance.minimapBtn.GetComponent<BoxCollider2D>().enabled = true;
    }

    /// <summary>
    /// 다음 레벨 이벤트
    /// </summary>
    public void NextLevel()
    {
        level++;

        SceneManager.LoadSceneAsync(string.Format("{0}_{1}", sceneName, level));
    }

    /// <summary>
    /// 현재 오브젝트의 위치를 표시해줄 오브젝트의 초기화
    /// </summary>
    public void MoveObjMinimapSet()
    {
        GameObject minimapPos = Instantiate(moveObjMinimapPos);

        minimapPos.SetActive(true);
        minimapPos.transform.parent = movedObj.transform;
        //내차 미니맵 오브젝트 위치,회전,크기 조정
        minimapPos.transform.localPosition = new Vector3(0, 1, 0);
        minimapPos.transform.localEulerAngles = Vector3.zero;

        //minimapPos.transform.localPosition = movedObj.transform.localPosition;
        //minimapPos.transform.localPosition += (Vector3.up * 0.3f);
        minimapPos.transform.Rotate(new Vector3(90, -180, 0));
        minimapPos.transform.localScale = new Vector3(0.7f, 0.7f, 1);

    }

    private void LoadPrefabDestroy()
    {
        if (map != null)
        {
            Destroy(map);
            map = null;
        }

        if (movedObj != null)
        {
            Destroy(movedObj);
            movedObj = null;
        }
    }

    private void RetryAssetLoad(System.Exception e)
    {
        if (retryAssetLoad == false)
        {
            Debug.LogError("Qestion Create Retry [Error : " + e + "]");
            StopAllCoroutines();
            LoadPrefabDestroy();
            AssetLoading();
            MainUI.메인.uiEventLinkManager.SetActive(false);
            MainUI.메인.인식글자UI.SetActive(false);

            MazeGameUI.instance.ArrowSetActiveAll(false);
            retryAssetLoad = true;
        }
        else
        {
            Debug.LogError("Reloading the asset bundle failed.Please Check AssetBundle");
        }
    }
}