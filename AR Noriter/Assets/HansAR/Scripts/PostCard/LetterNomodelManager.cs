using UnityEngine;
using System.Collections;
using System.IO;
using System;
using Vuforia;
using System.Collections.Generic;
using HansAR;
using UnityEngine.SceneManagement;

public class LetterNomodelManager : MonoBehaviour
{
    public static LetterNomodelManager instance;

    private Coroutine bundleLordCor;
    private string partName = "letter";
    private Texture2D dummyTexture;
    private int textureIndex = 0;
    void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnEnable()
    {
        BundleLordSart();

        TargetManager.DelEventMarkerFound = MarkerFound;

        TargetManager.DelTrackingReadyEvent = AfterLoadBundleEvent;
    }

    void OnDisable()
    {
        TargetManager.DelEventMarkerFound = null;
        TargetManager.DelTrackingReadyEvent = null;
    }

    
    private void BundleLordSart()
    {
        BuntdleLordStop();
        bundleLordCor = StartCoroutine(SetAssetBundleContents());
    }

    private void BuntdleLordStop()
    {
        if(bundleLordCor !=null)
        {
            StopCoroutine(bundleLordCor);
            bundleLordCor = null;
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

        GlobalDataManager.m_AssetBundlePartName = partName;

        GlobalDataManager.m_SelectedAssetBundleName = string.Format("{0}_{1}", GlobalDataManager.m_ResourceFolderEnum.ToString().ToLower(),
                                                                               GlobalDataManager.m_AssetBundlePartName.ToLower());

        Debug.Log("AssetBundleName : " + GlobalDataManager.m_SelectedAssetBundleName);

        TargetManager.타깃메니저.에셋번들복제컨텐츠 = new GameObject[TargetManager.타깃메니저.컨텐츠모델링이름.Length];

        AssetBundleLoader.getInstance.SetAssetBundleLoadInfo(allDataSet,
                                                           GlobalDataManager.m_SelectedAssetBundleName,
                                                           null,
                                                           AssetBundleLoader.getInstance.OnLoadCompleteModeling,
                                                           AfterBundleLoadComplete,
                                                           null,
                                                           null);


        AssetBundleLoader.getInstance.SetStorageLoadObject(allDataSet,
                                                          TargetManager.타깃메니저.컨텐츠모델링이름,
                                                          TargetManager.타깃메니저.에셋번들복제컨텐츠,
                                                          TargetManager.타깃메니저.모델링오브젝트,
                                                          TargetManager.타깃메니저.AR카메라);


        AssetBundleLoader.getInstance.StartLoadAssetBundle(allDataSet);
    }

    private void AfterBundleLoadComplete(HttpRequestDataSet dataSet)
    {
        //SetSceneUI();        
        //SetSketchOriginalTextures();

        // add script
        TargetManager.타깃메니저.StartVuforia();
    }

    private void AfterLoadBundleEvent()
    {
        SetSceneUI();
    }


    public void MarkerFound(int index)
    {
        if (TargetManager.타깃메니저.usedSelfiMode)
        {
            textureIndex = index;

            TargetManager.타깃메니저.컨텐츠오브젝트_위치(true);

            RotateUI.회전.컨텐츠_회전_초기화();

            LetterManager.Instance.textBox.GetComponent<UILabel>().text = LocalizeText.Value["letter_textbox"];
            LetterManager.Instance.StopSetLetterPaper();
            SelfiInfoSet();

            LetterManager.Instance.ResetLetterManager();
            MainUI.메인.인식글자UI.SetActive(false);
        }
    }

    private void SetSceneUI()
    {
        StartCoroutine(MainUI.메인.CloseLoadingPopUp());
        RotateUI.회전.회전UI_숨기기();
        MainUI.메인.인식글자UI.SetActive(true);
    }

    public void SelfiInfoSet()
    {
        LetterInfo info = TargetManager.타깃메니저.에셋번들복제컨텐츠[0].GetComponent<LetterInfo>();
        LetterManager.Instance.targetObj = TargetManager.타깃메니저.에셋번들복제컨텐츠[0].gameObject;

        Debug.Log("textureIndex : " + textureIndex);
        Debug.Log("info : " + info);
        if (info != null)
        {
            LetterManager.Instance.letterInfo = info;
            info.letterMat.mainTexture = info.textureList[textureIndex];

            if (LetterManager.Instance.backFaceTexture == null)
            {
                LetterManager.Instance.backFaceTexture = info.photoMat.mainTexture;
            }
            info.gameObject.SetActive(true);

            TargetManager.타깃메니저.복제모델링인덱스 = 0;
            AnimationManager.애니메이션.애니메이션01_재생();
            AnimationManager.애니메이션.애니.wrapMode = WrapMode.Once;
        }
    }

}
