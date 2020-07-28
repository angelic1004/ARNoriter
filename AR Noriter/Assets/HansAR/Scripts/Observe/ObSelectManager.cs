using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using HansAR;

public class ObSelectManager : MonoBehaviour
{
    public bool modelLabelSet = false;
    public GameObject nowLabelObj;

    public GameObject modelLabelPrefab;

    private StringBuilder m_Builder;

    private Vector3 insertPos;
    private Transform objTr;

    private string labelName = "BG";

    private string glassSprName = "replay_popup_background";
    private string labelSprName = "replay_popup_background";

    private string saveSetText = string.Empty;

    public bool labelOpen = false;

    private Coroutine loadCor;
    private Coroutine dragTextCor;
    private Coroutine followPosCor;
    private Coroutine labelSizeCor;

    public GameObject labelObjRoot;

    public GameObject explainUi;
    public GameObject explainBG;
    public GameObject explainLabel;
    public GameObject explainBtn;

    public GlobalDataManager.CategoryType loadType;

    public string loadObjName;
    public GameObject loadObj;

    public string objidleTriggerName;
    public string objWalkTriggerName;

    public bool labelCorSet = false;

    public static ObSelectManager instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        m_Builder = new StringBuilder();
        m_Builder.Remove(0, m_Builder.Length);

        TouchEventManager.터치.obSelectManager = this;

        PrefabsInit();

        LoadCorStart();

        ExplainInit();
    }

    void Update()
    {

    }

    void OnEnable()
    {
        TargetManager.DelEventMarkerFound = Cognitive;
        TargetManager.DelEventMarkerLost = Noncognitive;
    }

    private void BundleObj()
    {
        HttpRequestDataSet oneDataSet = null;
        HttpRequestDataSet allDataSet = null;

        oneDataSet = new HttpRequestDataSet();
        allDataSet = new HttpRequestDataSet();

        //GlobalDataManager.m_ResourceFolderEnum = GlobalDataManager.m_ResourceFolderEnum;
        GlobalDataManager.m_ResourceFolderEnum = GlobalDataManager.CategoryType.City;
        GlobalDataManager.m_AssetBundlePartName = "common";

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

        ////////////////////////////////////////////////////////////////////////////////////////
        /*
        AssetBundleLoader.getInstance.SetAssetBundleLoadInfo(oneDataSet,
                                                            GlobalDataManager.m_SelectedAssetBundleName,
                                                            null,
                                                            AssetBundleLoader.getInstance.OnLoadCompleteOnceModeling,
                                                            AfterCompletSet,
                                                            null,
                                                            null);

        AssetBundleLoader.getInstance.SetStorageLoadObject(oneDataSet, loadObjName, TargetManager.타깃메니저.모델링오브젝트);



        AssetBundleLoader.getInstance.StartLoadAssetBundle(oneDataSet);
        */
        Debug.Log("번들로드");
    }

    public void AfterCompletSet(HttpRequestDataSet dataSet)
    {
        //loadObj = dataSet.OnceModeling;
        MainUI.메인.딜레이팝업UI.SetActive(false);
    }

    public void CorAllStop()
    {
        labelOpen = false;
        DragTextStop();
        FollowPosCorStop();
        LabelSizeCorStop();
    }

    public void SlideCall(int index)
    {
        ModelLabelSetting(TargetManager.타깃메니저.에셋번들복제컨텐츠[index]);
    }


    public void Cognitive(int index)
    {
        FollowPosCorStop();

        MainUI.메인.인식글자UI.SetActive(false);
        RotateUI.회전.회전UI.SetActive(false);
        MainUI.메인.공부하기UI.SetActive(false);

        LanguageManager.공부매니저.공부하기_정보출력();

        MainUI.메인.애니메이션동작_UI보이기();

        ExplainSave(TargetManager.타깃메니저.에셋번들복제컨텐츠[TargetManager.타깃메니저.복제모델링인덱스]);

        explainUi.SetActive(true);

        ObserveRotateManger.instance.CognitiveCam();
        TouchEventManager.터치.기준콜라이더 = TouchEventManager.터치.지형3D;
        ObserveRotateManger.instance.기준콜라이더 = null;
    }


    public void Noncognitive(int index)
    {
        MainUI.메인.공부하기UI.SetActive(true);
        RotateUI.회전.회전UI.SetActive(true);

        LanguageManager.공부매니저.공부하기_정보출력();

        AnimationManager.애니메이션.애니메이션01_재생();

        ObserveRotateManger.instance.NonCognitiveCam();
        TouchEventManager.터치.기준콜라이더 = null;
        ObserveRotateManger.instance.기준콜라이더 = TouchEventManager.터치.지형2D;
    }

    public void SetRoadIdle()
    {
        RoadWalk(false);
    }

    public void SetRoadWalk()
    {
        RoadSet(true);
        RoadWalk(true);
    }

    private void RoadSet(bool state)
    {
        loadObj.SetActive(state);
    }

    private void RoadWalk(bool state)
    {
        if (TargetManager.타깃메니저.에셋번들복제컨텐츠[TargetManager.타깃메니저.복제모델링인덱스].GetComponent<ModelInfo>().애니메이션정보.애니클립.Length <= 0)
        {
            return;
        }

        Animation targetAni = TargetManager.타깃메니저.에셋번들복제컨텐츠[TargetManager.타깃메니저.복제모델링인덱스].GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animation>();

        Transform[] tran = loadObj.transform.GetChild(0).GetComponentsInChildren<Transform>();

        if (state)
        {
            foreach (Transform t in tran)
            {
                if (t.GetComponent<RoadScroll>() != null)
                {
                    t.GetComponent<RoadScroll>().ScrollX = -2.0f;
                }
            }

            AnimationManager.애니메이션.애니메이션01_재생();
        }
        else
        {
            foreach (Transform t in tran)
            {
                if (t.GetComponent<RoadScroll>() != null)
                {
                    t.GetComponent<RoadScroll>().ScrollX = 0;
                }
            }
            AnimationManager.애니메이션.애니.Stop();
            // targetAnimator.SetTrigger(objidleTriggerName);
        }
    }

    private void PrefabsInit()
    {
        modelLabelPrefab.SetActive(false);
    }

    private void ExplainInit()
    {
        explainBG.GetComponent<UIWidget>().width = 0;
        explainBG.GetComponent<UIWidget>().height = 0;

        explainLabel.GetComponent<UILabel>().text = string.Empty;

        explainBG.SetActive(false);
        explainLabel.SetActive(false);
        explainUi.SetActive(false);
    }

    public void ExplainSave(GameObject obj)
    {
        if (labelOpen)
        {
            ModelLabelClickEvent(explainBG);
            ControlManager.instance.BottomMenuBtnClick(explainBtn);
            labelOpen = false;
        }

        CorAllStop();

        explainBG.GetComponent<UIWidget>().width = 0;
        explainBG.GetComponent<UIWidget>().height = 0;

        explainLabel.GetComponent<UILabel>().text = string.Empty;

        explainBG.SetActive(false);
        explainLabel.SetActive(false);


        if (obj.GetComponent<ModelLabelInfo>() != null)
        {
            labelCorSet = true;
            saveSetText = LocalizeText.Value[obj.GetComponent<ModelLabelInfo>().labelPosList.localizeValue];
        }
    }

    public void ModelLabelSetting(GameObject obj)
    {
        if (modelLabelSet)
        {
            ModelLabelListDel();

            if (obj.GetComponent<ModelLabelInfo>() != null)
            {
                labelCorSet = true;

                saveSetText = LocalizeText.Value[obj.GetComponent<ModelLabelInfo>().labelPosList.localizeValue];
                insertPos = obj.GetComponent<ModelLabelInfo>().labelPosList.setPos;
                objTr = obj.GetComponent<ModelLabelInfo>().labelPosList.targetObjTr;

                if (objTr != null)
                {
                    SpriteSet(obj, insertPos, objTr);
                }
            }
            else
            {
                Debug.Log("obj.GetComponent<ModelLabelInfo>() = null");
            }
        }
        else
        {
            Debug.Log("modelLabelSet 사용안함");
        }
    }

    public void ModelLabelClickEvent(GameObject obj)
    {
        Debug.Log("터치 됨");
        if (!labelOpen)
        {
            if (obj.name.Contains(labelName))
            {
                obj.GetComponent<UISprite>().width = 0;
                obj.GetComponent<UISprite>().height = 0;
                obj.GetComponent<UISprite>().spriteName = labelSprName;
                obj.SetActive(true);

                LabelSizeCorStart(obj, 400, 800, 3);

                labelOpen = !labelOpen;
                Debug.Log("라벨 클릭 이벤트");
            }
        }
        else
        {
            DragTextStop();

            obj.GetComponent<UISprite>().spriteName = glassSprName;
            explainLabel.GetComponent<UILabel>().text = string.Empty;

            LabelSizeCorStart(obj, 0, 0, 7);
            labelOpen = !labelOpen;
        }
    }

    private void ModelLabelListDel()
    {
        labelCorSet = false;

        try
        {
            for (int i = 0; i < labelObjRoot.transform.childCount; i++)
            {
                Destroy(labelObjRoot.transform.GetChild(i).gameObject, 0.2f);
            }
        }
        catch
        {
            Debug.Log("오브젝트 삭제 안됨");
        }
    }


    private void SpriteSet(GameObject obj, Vector3 changePos, Transform objTr)
    {
        GameObject prefab = null;

        prefab = (Instantiate(modelLabelPrefab) as GameObject);
        //prefab.SetActive(false);

        if (prefab != null)
        {
            prefab.name = labelName;
            prefab.transform.parent = labelObjRoot.transform;// transform.Find("UI Root");
            prefab.transform.position = Vector3.zero;
            prefab.transform.rotation = Quaternion.identity;
            prefab.transform.localScale = Vector3.one;
            prefab.tag = "modelLabel";


            LabelSizeCorStart(prefab, 0, 0, 0.7f);

            nowLabelObj = prefab;
        }

        followPosCor = StartCoroutine(FollowPos(objTr, changePos, prefab));
    }


    public void DragTextStart(GameObject obj, string text)
    {
        DragTextStop();
        dragTextCor = StartCoroutine(DragTextPrint(obj, text));
    }

    private void DragTextStop()
    {
        if (dragTextCor != null)
        {
            StopCoroutine(dragTextCor);
            dragTextCor = null;
        }
    }

    /// <summary>
    /// 문제 텍스트 순서대로 출력 되는 부분
    /// </summary>
    /// <returns></returns>
    private IEnumerator DragTextPrint(GameObject obj, string text)
    {
        int stringIndex = 0;
        //int quizListIndex = (int)m_StickerQuizNum - 1;
        float nextTime = 0.01f;
        float savedNextTime = nextTime;

        explainLabel.SetActive(true);

        while (true)
        {
            nextTime = nextTime - Time.deltaTime;

            if (obj == null && !obj.activeSelf)
            {
                yield break;
            }

            if (nextTime <= 0)
            {
                if (stringIndex > text.Length)
                {
                    Debug.Log("텍스트 코루틴 끝");
                    yield break;
                }

                obj.GetComponent<UILabel>().text
                      = m_Builder.AppendFormat("{0}", text.Substring(0, stringIndex)).ToString();
                m_Builder.Remove(0, m_Builder.Length);
                nextTime = savedNextTime;
                stringIndex++;
            }
            yield return new WaitForEndOfFrame();
        }
    }


    private void FollowPosCorStop()
    {
        LabelSizeCorStop();

        if (followPosCor != null)
        {
            StopCoroutine(followPosCor);
            followPosCor = null;
        }

    }

    private void LabelSizeCorStart(GameObject obj, float endWidth, float endHeight, float speed)
    {
        LabelSizeCorStop();
        labelSizeCor = StartCoroutine(LabelSizeCor(obj, endWidth, endHeight, speed));
    }


    private void LabelSizeCorStop()
    {
        if (labelSizeCor != null)
        {
            StopCoroutine(labelSizeCor);
            labelSizeCor = null;
        }
    }


    private IEnumerator FollowPos(Transform tr, Vector3 changePos, GameObject obj)
    {
        float setTime = 1.0f;

        yield return new WaitForSeconds(setTime * 0.5f);

        obj.GetComponent<UIWidget>().color = new Color(1, 1, 1, 0);
        obj.transform.localScale = new Vector3(-1, 1, 1);

        TweenManager.tween_Manager.TweenAllDestroy(obj);
        TweenManager.tween_Manager.AddTweenAlpha(obj, 0, 1, setTime * 0.5f);
        TweenManager.tween_Manager.TweenAlpha(obj);

        obj.SetActive(true);

        while (true)
        {
            if (obj == null && !obj.activeSelf)
            {
                yield break;
            }
            obj.transform.position = new Vector3(tr.position.x + changePos.x,
                                                 tr.position.y, //+ changePos.y,
                                                 tr.position.z + changePos.z);
            obj.transform.LookAt(Camera.main.transform);

            if (!labelCorSet)
            {
                yield break;
            }
            yield return new WaitForEndOfFrame();
        }
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


    private IEnumerator LabelSizeCor(GameObject obj, float endWidth, float endheight, float speed)
    {
        bool widthDir = false;
        bool heightDir = false;

        bool sucWidth = false;
        bool sucHeight = false;

        float setSpeed = speed * 500;

        float startWidth = obj.GetComponent<UIWidget>().width;
        float startHeight = obj.GetComponent<UIWidget>().height;

        float speedTest = startWidth / startHeight;

        Debug.Log("사이즈 조정 코루틴");

        if (startWidth > endWidth)
        {
            widthDir = false;
        }
        else
        {
            widthDir = true;
        }

        if (startHeight > endheight)
        {
            heightDir = false;
        }
        else
        {
            heightDir = true;
        }

        while (true)
        {
            yield return new WaitForEndOfFrame();

            if (!sucWidth)
            {
                if (widthDir)
                {
                    startWidth += Time.deltaTime * setSpeed;

                    if (startWidth >= endWidth)
                    {
                        sucWidth = true;
                    }
                }
                else
                {
                    startWidth -= Time.deltaTime * setSpeed * speedTest;

                    if (startWidth <= endWidth)
                    {
                        sucWidth = true;
                    }
                }
            }

            if (!sucHeight)
            {
                if (heightDir)
                {
                    startHeight += Time.deltaTime * setSpeed;

                    if (startHeight >= endheight)
                    {
                        sucHeight = true;
                    }
                }
                else
                {
                    startHeight -= Time.deltaTime * setSpeed;

                    if (startHeight <= endheight)
                    {
                        sucHeight = true;
                    }
                }
            }

            if (sucWidth && sucHeight)
            {
                if (labelOpen)
                {
                    if (obj == null && !obj.activeSelf)
                    {
                        yield break;
                    }

                    DragTextStart(explainLabel, saveSetText);
                }
                else
                {
                    obj.SetActive(false);
                }

                obj.GetComponent<UIWidget>().width = (int)endWidth;
                obj.GetComponent<UIWidget>().height = (int)endheight;

                Debug.Log("사이즈 조정 코루틴 끝");
                yield break;
            }

            obj.GetComponent<UIWidget>().width = (int)startWidth;
            obj.GetComponent<UIWidget>().height = (int)startHeight;
        }
    }
}
