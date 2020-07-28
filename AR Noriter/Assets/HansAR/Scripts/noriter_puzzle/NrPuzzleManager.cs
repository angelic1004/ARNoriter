using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using HansAR;

public class NrPuzzleManager : MonoBehaviour
{

    private Coroutine loadCor;

    public static NrPuzzleManager instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        TargetManager.타깃메니저.StartVuforia();
        MainUI.메인.딜레이팝업UI.SetActive(false);
        
        //LoadCorStart();
        //MainUI.메인.딜레이팝업UI.SetActive(false);

        //BundleObj();
    }

    void Update()
    {

    }

    void OnEnable()
    {
        TargetManager.DelEventMarkerFound = Cognitive;
        TargetManager.DelEventMarkerLost = Noncognitive;

        TargetManager.DelTrackingReadyEvent = AfterLoadBundleEvent;
    }

    void OnDisable()
    {
        TargetManager.DelEventMarkerFound = null;
        TargetManager.DelEventMarkerLost = null;

        TargetManager.DelTrackingReadyEvent = null;

    }

    private void BundleObj()
    {
        HttpRequestDataSet allDataSet = null;

        allDataSet = new HttpRequestDataSet();

        GlobalDataManager.m_ResourceFolderEnum = GlobalDataManager.m_ResourceFolderEnum;
        //GlobalDataManager.m_ResourceFolderEnum = GlobalDataManager.CategoryType.City;
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
        //MainUI.메인.딜레이팝업UI.SetActive(false);

        // add script
        TargetManager.타깃메니저.StartVuforia();
    }

    private void AfterLoadBundleEvent()
    {
        MainUI.메인.딜레이팝업UI.SetActive(false);
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


    public void Cognitive(int index)
    {
        TargetManager.EnableTracking = false;
        AutoFocusMode.getInstance.OnOffAutoFucousMode(false);

        MainUI.메인.인식글자UI.SetActive(false);
        TargetManager.타깃메니저.모델링오브젝트.SetActive(false);

        NrPuzzleUI.instance.nowPieceMode = NrPuzzleUI.Piece.Four;
        NrPuzzleUI.instance.targetIndex = index;
        //NrPuzzleUI.instance.targetIndex = TargetManager.타깃메니저.복제모델링인덱스;

        LevelingSetting(NrPuzzleUI.instance.nowPieceMode);

        TouchEventManager.터치.기준콜라이더 = TouchEventManager.터치.지형3D;
    }

    private void Noncognitive(int index)
    {
        TouchEventManager.터치.기준콜라이더 = TouchEventManager.터치.지형2D;
    }


    public void LevelingSetting(NrPuzzleUI.Piece mode)
    {
        switch (mode)
        {
            case NrPuzzleUI.Piece.Four:
                FourPieceModeInit();
                break;

            case NrPuzzleUI.Piece.Nine:
                NinePieceModeInit();
                break;

            default:
                Debug.LogWarning("퍼즐 선택 모드 없음");
                return;
        }
    }


    private void FourPieceModeInit()
    {
        NrPuzzleUI.instance.FourPieceUiInit();
    }

    private void NinePieceModeInit()
    {
        NrPuzzleUI.instance.NinePieceUiInit();
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
