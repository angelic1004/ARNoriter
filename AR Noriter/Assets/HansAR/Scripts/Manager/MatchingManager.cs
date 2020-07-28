using UnityEngine;
using System.Collections;
using HansAR;


public class MatchingManager : MonoBehaviour
{
    public enum MatchinMode
    {
        normal,
        random
    }

    public enum MatchingLevel
    {
        none,
        one,
        two,
        three,
        four,
        five
    }

    public GlobalDataManager.CategoryType loadType;

    public MatchinMode matchingMode;
    public MatchingLevel matchingLevel;

    public string loadObjName;
    public GameObject loadObj;

    private Coroutine loadCor;

    public static MatchingManager instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        //LoadCorStart();
        BundleObj();
    }

    void Update()
    {

    }

    void OnEnable()
    {
        // TargetManager.DelEventMarkerFound = cognitive;
        // TargetManager.DelEventMarkerLost = noncognitive;

        TargetManager.DelTrackingReadyEvent = AfterLoadBundleEvent;
    }

    void OnDisable()
    {
        TargetManager.DelTrackingReadyEvent = null;
    }

    private void BundleObj()
    {
        HttpRequestDataSet allDataSet = null;

        allDataSet = new HttpRequestDataSet();

        GlobalDataManager.m_ResourceFolderEnum = GlobalDataManager.m_ResourceFolderEnum;
        GlobalDataManager.m_AssetBundlePartName = "common";
        Debug.Log(" GlobalDataManager.m_ResourceFolderEnum : " + GlobalDataManager.m_ResourceFolderEnum);

        GlobalDataManager.m_SelectedAssetBundleName = string.Format("{0}_{1}", GlobalDataManager.m_ResourceFolderEnum.ToString().ToLower(),
                                                                               GlobalDataManager.m_AssetBundlePartName.ToLower());


        TargetManager.타깃메니저.에셋번들복제컨텐츠 = new GameObject[TargetManager.타깃메니저.컨텐츠모델링이름.Length];

        AssetBundleLoader.getInstance.SetAssetBundleLoadInfo(allDataSet,
                                                           GlobalDataManager.m_SelectedAssetBundleName,
                                                           null,
                                                           AssetBundleLoader.getInstance.OnLoadCompleteModeling,
                                                           AfterCompletSet,
                                                           null,
                                                           null);


        AssetBundleLoader.getInstance.SetStorageLoadObject(allDataSet,
                                                          TargetManager.타깃메니저.컨텐츠모델링이름,
                                                          TargetManager.타깃메니저.에셋번들복제컨텐츠,
                                                          TargetManager.타깃메니저.모델링오브젝트,
                                                          TargetManager.타깃메니저.AR카메라);


        AssetBundleLoader.getInstance.StartLoadAssetBundle(allDataSet);

        Debug.Log("번들로드");
    }

    public void AfterCompletSet(HttpRequestDataSet dataSet)
    {
        loadObj = dataSet.OnceModeling;
        //MainUI.메인.딜레이팝업UI.SetActive(false);

        // add script
        TargetManager.타깃메니저.StartVuforia();
    }

    private void AfterLoadBundleEvent()
    {
        MainUI.메인.딜레이팝업UI.SetActive(false);
    }

    public void Cognitive(int index)
    {
        TargetManager.EnableTracking = false;
        AutoFocusMode.getInstance.OnOffAutoFucousMode(false);
        MainUI.메인.인식글자UI.SetActive(false);
        RotateUI.회전.회전UI.SetActive(false);
        MainUI.메인.공부하기UI.SetActive(false);

        MatchingUI.instance.LevelOneInit();

        TouchEventManager.터치.기준콜라이더 = TouchEventManager.터치.지형3D;
    }


    public void noncognitive(int index)
    {
       // MatchingUI.instance.LevelOneInit();
        TouchEventManager.터치.기준콜라이더 = TouchEventManager.터치.지형2D;
    }


    private void LoadCorStart()
    {
        LoadCorStop();
        loadCor = StartCoroutine(TrackerManagerCheck());
    }

    private void LoadCorStop()
    {
        if (loadCor != null)
        {
            StopCoroutine(loadCor);
            loadCor = null;
        }
    }


    private IEnumerator TrackerManagerCheck()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();

            if (Vuforia.TrackerManager.Instance.GetTracker<Vuforia.ObjectTracker>() != null)
            {
                BundleObj();
                yield break;
            }
        }
    }
        
}
