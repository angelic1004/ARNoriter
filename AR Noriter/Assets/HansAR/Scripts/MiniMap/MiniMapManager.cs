using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using System.Reflection;

using HansAR;

public class MiniMapManager : MonoBehaviour {
    public enum MiniMapPaintType
    {
        None = 0,
        Color,
        Paint
    }


    private MiniMapInfo.MiniMapResourceInfo[]   miniMapResourceInfo;
    public bool                                 isDoingZoomInOut;

    private Vector3                             boxColliderDefaultSize;    

    [SerializeField]
    private string                              miniMapModelingName;

    public Texture2D                            readTextureData { get; set; }
    public GameObject                           trackingModel { get; set; }

    public string                               trackingModelName { get; set; }

    public bool                                 isMiniMapScaleUp { get; set; }    
    public bool                                 isCaptureBtnOnMiniMap { get; set; }
    public bool                                 isCollectingForModeling { get; set; }

    public ScreenOrientation                    currentOrientation { get; set; }

    [HideInInspector]
    public Vector3[]                            miniMapCurrentTransform;    // [0] : position, [1] : rotate, [2] : scale
      
    public GameObject                           miniMapRoot;    
    public GameObject                           miniMapModeling;

    public GameObject                           miniMapUI;
    public GameObject                           miniMapCollectingBtn;
    public GameObject                           miniMapScaleControlBtn;
    public GameObject                           miniMapZoomControlBtn;

    //public GameObject                           miniMapScaleUpDownBtn;
    //public GameObject                           miniMapZoomOutBtn;
    //public GameObject                           miniMapZoomInBtn;    

    public Vector3                              miniMapPortraitPos;    
    public Vector3                              miniMapRotate;
    public Vector3                              miniMapMinScale;

    public Vector3                              miniMapLandScapePos;
    public Vector3                              miniMapMaxPos;

    public Vector3                              miniMapMaxRotate;
    public Vector3                              miniMapMaxScale;

    public float                                miniMapMoveSpeed;
    public float                                changeScaleRateValue;

    public float                                scaleUpDownMaxValue;
    public float                                scaleUpDownMinValue;
    public float                                scaleUpDownIncreaseValue;

    public float                                boxColliderMinSize;    

    public static MiniMapManager                instance;

    public bool                                 usedRunway;
    public bool                                 usedCameraRotate;

    public bool                                 usedAdditionModel;    // 예를들어 경찰서, 경찰서를 한세트로 하는 미니맵 모델을 사용 할 겅유 TRUE
    public bool                                 usedPointAniamtion;   // 모델링 포인트 애니메이션이 따로있을 경우
    public GameObject                           btnScaleUp;       // [버튼] 크게 ([버튼 ] ScaleControl 하위에 있음)

    public MiniMapPaintType                     minimapPaintType = MiniMapPaintType.None;

    public int tempAniIndex = 0;
    private GameObject playMiniMapModel;

    private bool clickedExpandButton = false;
    
    void Awake()
    {
        playMiniMapModel    = null;
        instance            = this;
    }

	// Use this for initialization
	void Start () {        
        isMiniMapScaleUp            = false;
        isDoingZoomInOut            = false;
        isCaptureBtnOnMiniMap       = false;
        isCollectingForModeling     = false;

        currentOrientation          = Screen.orientation;

        // 기본 위치, 회전, 크기값이 있으니, miniMapCurrentTransform을 사용하는 이유는 크기 변경이나 위치가 변경 될 떄
        // 코루틴 내 반복문에서 사용 하여 값을 변경 후 적용 하기 때문에 변경되는 값을 저장 할 변수가 필요함.
        // 그 역할을 miniMapCurrentTransform 변수가 함.
        miniMapCurrentTransform[0]  = miniMapPortraitPos;
        miniMapCurrentTransform[1]  = miniMapRotate;
        miniMapCurrentTransform[2]  = miniMapMinScale;

        if (miniMapScaleControlBtn != null)
        {
            if (miniMapScaleControlBtn.transform.childCount == 2)
            {
                miniMapScaleControlBtn.transform.GetChild(1).gameObject.SetActive(false);
            }

        }

        if (miniMapZoomControlBtn != null)
        {
            miniMapZoomControlBtn.SetActive(false);
        }        

        // event 
        if (TargetManager.타깃메니저.스케치씬사용 && TargetManager.타깃메니저.UsedMiniMap)
        {
            ColoringManager.OnRecognitionData += ApplySketchModelingData;
        }

        // city_minimap.hans 에셋번들 파일에서 CityMinimap 모델링을 로드 합니다.
        LoadMiniMap();

        //StartCoroutine(CheckAnimatorIdle());
    }

    void OnEnable()
    {
        TargetManager.DelEventMarkerLost += MarkerLostEvent;
    }

    void OnDestroy()
    {
        // event
        if (TargetManager.타깃메니저.스케치씬사용 && TargetManager.타깃메니저.UsedMiniMap)
        {
            ColoringManager.OnRecognitionData -= ApplySketchModelingData;
        }
        else
        {
            
        }

        TargetManager.DelEventMarkerLost -= MarkerLostEvent;
    }

    void UnsubscribeEvent()
    {
        OnDestroy();
    }    

    private int LoadMiniMap()
    {
        if (string.IsNullOrEmpty(miniMapModelingName))
        {
            return -1;
        }
        
        HttpRequestDataSet dataSet                  = null;
        string miniMapBundleName                    = string.Empty;

        dataSet                                     = new HttpRequestDataSet();
        miniMapBundleName                           = string.Format("{0}_minimap", GlobalDataManager.m_ResourceFolderEnum.ToString().ToLower());

        switch (minimapPaintType)
        {
            case MiniMapPaintType.Paint:                                
            case MiniMapPaintType.Color:
                miniMapBundleName                   = string.Format("{0}_{1}", miniMapBundleName, minimapPaintType.ToString().ToLower());
                break;
            default:
                break;
        }

        AssetBundleLoader.getInstance.SetAssetBundleLoadInfo(dataSet,
                                                             miniMapBundleName,
                                                             null,
                                                             AssetBundleLoader.getInstance.OnLoadCompleteOnceModeling,
                                                             AppendMinimapModeling,
                                                             TargetManager.타깃메니저.ApplySceneUI,
                                                             null);

        AssetBundleLoader.getInstance.SetStorageLoadObject(dataSet, miniMapModelingName, miniMapRoot);
        AssetBundleLoader.getInstance.StartLoadAssetBundle(dataSet);

        return 0;
    }

    private void AppendMinimapModeling(HttpRequestDataSet dataSet)
    {
        SetMiniMapModeling(dataSet.OnceModeling);
    }
	
	
    // private functions
    private IEnumerator ChangeScaleOfTrackingModel(GameObject managerObj, GameObject obj, float durationTime, float rateValue)
    {
        float totalTime = 0f;

        do
        {
            totalTime = totalTime + Time.deltaTime;

            if (obj.transform.localScale.x < 0)
            {
                continue;
            }

            obj.transform.localScale = obj.transform.localScale - (obj.transform.localScale * (Time.deltaTime * rateValue));

            yield return new WaitForEndOfFrame();
        } while (totalTime < durationTime);

        managerObj.GetComponent<MiniMapSlerp>().isMoveMiniMap = false;

        // 증강 컨텐츠 모두 숨김
        TargetManager.타깃메니저.HideAllModelingContents();
        
        if (TargetManager.타깃메니저.스케치씬사용 == false)
        {
            MiniMapTouchManager.instance.ColliderObject2D.GetComponent<BoxCollider>().enabled = false;
        }

        // 수족관 내 포획한 물고기 활성화
        //ShowFishObject(FishModelingName, ReadTextureData);
        //ShowMiniMapObject(trackingModelName, readTextureData);

        // 원래 Transform 값 적용
        obj.transform.localPosition     = TargetManager.타깃메니저.비인식후_좌표값;
        obj.transform.localEulerAngles  = TargetManager.타깃메니저.비인식후_회전값;
        obj.transform.localScale        = TargetManager.타깃메니저.비인식후_사이즈값;

        yield return null;
    }

    /// <summary>
    /// 미니맵의 크기를 확대/축소 합니다. 
    /// </summary>
    /// <param name="obj">미니맵 오브젝트</param>
    /// <param name="isIncreaseValue">true : 확대, false : 축소</param>
    /// <param name="durationTime">확대/축소가 동작하는 시간</param>    
    private IEnumerator ChangeScaleOfMiniMapModel(GameObject obj, bool isIncreaseValue, float durationTime)
    {
        float applyTime                 = 0f;
        float variationValue            = 0f;

        Vector3 currentScaleValue       = Vector3.zero;
        Vector3 currentPositionValue    = Vector3.zero;
        Vector3 currentRotateValue      = Vector3.zero;

        if (isIncreaseValue)
        {
            currentScaleValue           = miniMapMaxScale;
            currentRotateValue          = miniMapMaxRotate;
            currentPositionValue        = miniMapMaxPos;
        }
        else
        {
            currentScaleValue           = miniMapMinScale;
            currentRotateValue          = miniMapRotate;
            currentPositionValue        = miniMapPortraitPos;
        }

        if (MainUI.방향상태 != 0)
        {
            if (MiniMapRotateUI.instance.isLeftRightReverse)
            {
                currentPositionValue.x = -currentPositionValue.x;
            }
            else
            {
                currentPositionValue.y = -currentPositionValue.y;
            }
        }
            
        miniMapCurrentTransform[0]      = currentPositionValue;
        miniMapCurrentTransform[1]      = currentRotateValue;
        miniMapCurrentTransform[2]      = currentScaleValue;

        variationValue                  = Time.fixedDeltaTime / (durationTime / (Mathf.Abs(currentScaleValue.x - obj.transform.localScale.x)));
        isDoingZoomInOut                = true;

        do
        {
            if (isIncreaseValue)
            {
                if (obj.transform.localScale.x < currentScaleValue.x)
                {
                    obj.transform.localScale += new Vector3(variationValue, variationValue, variationValue);
                }

                if (obj.transform.localScale.x > currentScaleValue.x)
                {
                    obj.transform.localScale = currentScaleValue;
                }
            }
            else
            {
                if (obj.transform.localScale.x > currentScaleValue.x)
                {
                    obj.transform.localScale -= new Vector3(variationValue, variationValue, variationValue);
                }

                if (obj.transform.localScale.x < currentScaleValue.x)
                {
                    obj.transform.localScale = currentScaleValue;
                }
            }

            obj.transform.localPosition = Vector3.MoveTowards(obj.transform.localPosition, currentPositionValue, Time.fixedDeltaTime * miniMapMoveSpeed);
            applyTime += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        } while (applyTime < durationTime);
       
        isDoingZoomInOut = false;
        MiniMapRotateUI.instance.InitMiniMapRotate();
    }


    private void InitMiniMapObject(bool isActive)
    {
        if (miniMapResourceInfo == null) return;

        try
        {
            // 미니맵 내 모델링 활성 / 비활성 하는 코드
            foreach (MiniMapInfo.MiniMapResourceInfo resourceInfo in miniMapResourceInfo)
            {
                if (usedAdditionModel)
                {
                    foreach (GameObject obj in resourceInfo.miniMapObjects)
                    {
                        obj.SetActive(isActive);
                        //obj.GetComponent<Animator>().StopPlayback();
                    }
                }
                else
                {
                    foreach (GameObject Obj in resourceInfo.miniMapObjects)
                    {
                        // GetChild(0) 의 값은 miniMapRoot -> 미니맵 모델 -> miniMap(애니메이션) 모델을 갖게 됨                
                        miniMapModeling.transform.GetChild(0).FindChild(Obj.name).gameObject.SetActive(isActive);
                    }
                }                           
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(string.Format("Function:{0}, Message:{1}", MethodBase.GetCurrentMethod().Name, ex.Message));
            throw;
        }
    }

    private void SetMiniMapTransformValue(GameObject obj)
    {
        obj.name                                = miniMapModelingName;
        obj.transform.parent                    = miniMapRoot.transform;
        obj.transform.localPosition             = Vector3.zero;

        miniMapRoot.transform.localPosition     = miniMapCurrentTransform[0];
        miniMapRoot.transform.localEulerAngles  = miniMapCurrentTransform[1];
        miniMapRoot.transform.localScale        = miniMapCurrentTransform[2];
    }

    private void ApplySketchModelingData(GameObject obj, Texture2D texture)
    {
        if (obj == null || texture == null)
        {
            Debug.LogError(string.Format("parameter value is null =====> obj = {0}, textuer = {1}", obj, texture));
            return;
        }

        trackingModel           = obj;
        readTextureData         = texture;
        trackingModelName       = obj.name;
        isCollectingForModeling = false;
        
        SetActivationMiniMapModel(true);

        //miniMapUI.SetActive(true);
        //miniMapCollectingBtn.SetActive(true);
    }

    private int SetActivationMiniMapModelFromIndex(int idx, bool isActive, Texture2D texture)
    {
        if (miniMapResourceInfo == null) return -1;

        try
        {
            if (usedAdditionModel)
            {
                foreach (GameObject obj in miniMapResourceInfo[idx].miniMapObjects)
                {
                    obj.SetActive(isActive);
                    GlobalDataManager.ShaderRefresh(obj);

                    //playMiniMapModel = obj;
                }
            }
            else
            {
                foreach (GameObject miniMapObj in miniMapResourceInfo[idx].miniMapObjects)
                {
                    // GetChild(0) 의 값은 AquariumRoot -> 수족관 모델 -> aquarium(애니메이션) 모델을 갖게 됨               
                    miniMapModeling.transform.GetChild(0).FindChild(miniMapObj.name).gameObject.SetActive(isActive);
                    GlobalDataManager.ShaderRefresh(miniMapObj);
                }
            }            

            if (texture != null)
            {
                foreach (Material mat in miniMapResourceInfo[idx].miniMapMaterials)
                {
                    mat.mainTexture = texture;
                }

                //miniMapResourceInfo[idx].miniMapMaterials.mainTexture = texture;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(string.Format("Function:{0}, Message:{1}", MethodBase.GetCurrentMethod().Name, ex.Message));
            throw;
        }

        return 0;
    }

    private int SetActivationMiniMapModelFromName(string modelName, bool isActive, Texture2D texture)
    {
        if (miniMapResourceInfo == null)
        {
            return -1;
        }

        int findIndex       = -1;
        string compareStr   = string.Empty;

        try
        {
            for (int loopIdx = 0; loopIdx < miniMapResourceInfo.Length; loopIdx++)
            {
                compareStr = miniMapResourceInfo[loopIdx].miniMapCompareModelName;

                if (modelName.ToLower().Equals(compareStr.ToLower()))
                {
                    findIndex = loopIdx;
                    break;
                }
            }

            if (findIndex != -1)
            {
                SetActivationMiniMapModelFromIndex(findIndex, isActive, texture);                
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(string.Format("Function:{0}, Message:{1}", MethodBase.GetCurrentMethod().Name, ex.Message));
            throw;
        }

        return 0;
    }

    private float GetBoxColliderIncreaseValue()
    {
        float value = 0f;        
        value         = (boxColliderDefaultSize.x - boxColliderMinSize) / (scaleUpDownMaxValue - miniMapMaxScale.x);

        return value;
    }

    // public functions
    public GameObject GetMiniMapRoot()
    {
        return miniMapRoot;
    }

    public GameObject GetMiniMapModeling()
    {
        return miniMapModeling;
    }

    public string GetMiniMapRootObjName()
    {
        return miniMapRoot.name;
    }

    public string GetMiniMapModelName()
    {
        return miniMapModelingName;
    }

    public void SetActivationMiniMapModel(bool isActive)
    {
        if (miniMapModeling == null) return;

        miniMapModeling.SetActive(isActive);
    }

    public void SetMiniMapModeling(GameObject obj)
    {
        try
        {
            miniMapModeling = obj;
            SetMiniMapTransformValue(miniMapModeling);

            /*
            if (miniMapModeling.activeSelf == false)
            {
                miniMapModeling.SetActive(true);
            }
            */

            // minimap 모델링의 boxcollider 의 크기
            boxColliderDefaultSize = miniMapModeling.transform.GetChild(1).GetComponent<BoxCollider>().size;

            if (!usedRunway)
            {
                // FBX 프로젝트에서 AquariumInfo.cs 대신 MiniMapInfo.cs 로 변경 해야 함
                miniMapResourceInfo = miniMapModeling.GetComponent<MiniMapInfo>().miniMapResourceInfo;
                InitMiniMapObject(false);

                //miniMapModeling.GetComponent<ModelInfo>().애니메이션타겟.GetComponent<Animation>().wrapMode = WrapMode.Loop;
            }

            if (!usedCameraRotate)
            {
                gameObject.GetComponent<MiniMapSlerp>().endTransform = miniMapRoot.transform;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            throw;
        }
    }

    public void MiniMapResetPosition()
    {
        if (miniMapRoot != null)
        {
            miniMapRoot.transform.localPosition = miniMapCurrentTransform[0];
        }
    }

    public void ResetMinimapScale()
    {
        if(miniMapRoot != null)
        {
            miniMapRoot.transform.localScale = miniMapMaxScale;
        }
    }

    public void MiniMapZoomInOut(GameObject obj)
    {
        if (obj == null || isDoingZoomInOut) return;

        isMiniMapScaleUp = !isMiniMapScaleUp;
        
        MiniMapRotateUI.instance.SetActiveMiniMapRotateUI(isMiniMapScaleUp);

        // 미니맵 스케일의 확대/축소 하는 coroutine 실행
        StartCoroutine(ChangeScaleOfMiniMapModel(obj, isMiniMapScaleUp, 2));
    }

    public void ShowMiniMapObject(int idx, Texture2D texture)
    {
        SetActivationMiniMapModelFromIndex(idx, true, texture);
    }

    public void ShowMiniMapObject(string name, Texture2D texture)
    {
        SetActivationMiniMapModelFromName(name, true, texture);
    }

    public void OnClickPlayStoryAnimation(int modelIdx)
    {
        if (usedAdditionModel)
        {
            //if (playMiniMapModel != null)
            //{
                QueueMode mode  = QueueMode.PlayNow;
                //Animation ani   = playMiniMapModel.GetComponent<Animation>();

                Animation ani = miniMapModeling.GetComponent<MiniMapInfo>().miniMapResourceInfo[modelIdx].miniMapObjects[0].GetComponent<Animation>();

                for (int idx = 1; idx < ani.GetClipCount(); idx++)
                {
                    if (idx > 1)
                    {
                        mode = QueueMode.CompleteOthers;
                    }                    

                    // 최초 QueueMode.PlayNow 적용, idx > 1 이후 QueueMode.CompleteOthers 적용
                    ani.PlayQueued(miniMapModeling.GetComponent<MiniMapInfo>().miniMapResourceInfo[modelIdx].miniMapAnimations[idx].name, mode);
                }

                ani.PlayQueued(miniMapModeling.GetComponent<MiniMapInfo>().miniMapResourceInfo[modelIdx].miniMapAnimations[0].name, QueueMode.CompleteOthers);                
            //}
        }        
    }

    private void SettingClickCollecting()
    {
        MainUI.메인.애니메이션동작_UI숨기기();
        MainUI.메인.오버레이UI.SetActive(false);

        AnimationManager.애니메이션.동작UI사용여부 = false;

        miniMapCollectingBtn.SetActive(false);

        if (!usedCameraRotate)
        {
            MiniMapSlerp miniMapSlerp = null;
            miniMapSlerp = gameObject.GetComponent<MiniMapSlerp>();
            miniMapSlerp.isMoveMiniMap = true;
        }

        isCollectingForModeling = true;

        // 증강 컨텐츠 모두 숨김
        TargetManager.타깃메니저.HideAllModelingContents();

        if (TargetManager.타깃메니저.스케치씬사용 == false)
        {
            MiniMapTouchManager.instance.ColliderObject2D.GetComponent<BoxCollider>().enabled = false;
        }
    }

    public void OnClickCollecting()
    {
        SettingClickCollecting();

        ShowMiniMapObject(trackingModelName, readTextureData);
        OnClickMiniMapScaleUpDown(miniMapScaleControlBtn.transform.GetChild(0).gameObject);

        // 수집하기 모델의 스케일을 변경하는 coroutine 호출
        //StartCoroutine(ChangeScaleOfTrackingModel(gameObject, miniMapSlerp.startTransform.gameObject, miniMapSlerp.durationTime, changeScaleRateValue));

        //GameObject modelObj = TargetManager.타깃메니저.에셋번들복제컨텐츠[TargetManager.타깃메니저.복제모델링인덱스];
        //StartCoroutine(CameraRotateManager.Instance.ZoomOutCharacter(modelObj));        
    }

    public void OnClickCollectingOnceTarget()
    {
        MiniMapInfo.MiniMapResourceInfo[] resourceInfos = null;
        int findIndex = -1;

        try
        {
            SettingClickCollecting();

            resourceInfos = miniMapModeling.GetComponent<MiniMapInfo>().miniMapResourceInfo;

            for (int idx = 0; idx < resourceInfos.Length; idx++)
            {
                if (string.Compare(trackingModelName, resourceInfos[idx].miniMapCompareModelName) == 0)
                {
                    if (!resourceInfos[idx].miniMapObjects[0].activeSelf)
                    {
                        findIndex = idx;
                        break;
                    }
                }
            }

            if (findIndex != -1)
            {
                SetActivationMiniMapModelFromIndex(findIndex, true, readTextureData);
            }

            OnClickMiniMapScaleUpDown(miniMapScaleControlBtn.transform.GetChild(0).gameObject);
        }
        catch
        {

        }
    }

    public void OnClickMiniMapScaleUpDown(GameObject btnObj)
    {
        if (btnObj == null) { return; }        
        if (isDoingZoomInOut) { return; }

        if (string.Compare(btnObj.name, "[버튼] 크게") == 0 && isMiniMapScaleUp)
        {
            return;
        }
        else if (string.Compare(btnObj.name, "[버튼] 작게") == 0 && isMiniMapScaleUp == false)
        {
            return;
        }

        isMiniMapScaleUp = !isMiniMapScaleUp;

        btnObj.GetComponent<BoxCollider2D>().enabled = false;

        //크게
        if (isMiniMapScaleUp)
        {
            clickedExpandButton = true;
            miniMapZoomControlBtn.SetActive(true);

            MainUI.메인.오버레이UI.SetActive(false);
            TargetManager.타깃메니저.HideAllModelingContents();

            if (TargetManager.타깃메니저.스케치씬사용)
            {
                ColoringManager.컬러링매니저.TurnOffSketchUI();
            }
            else
            {
                RotateUI.회전.회전UI_숨기기();
                MainUI.메인.애니메이션동작_UI숨기기();
                MainUI.메인.색칠하기UI_닫기();
                MainUI.메인.오버레이UI.SetActive(false);
            }

            StartCoroutine(CameraRotateManager.Instance.ZoomInRotateCam());
            TargetManager.EnableTracking = false;
        }
        //작게
        else
        {
            clickedExpandButton = false;

            miniMapZoomControlBtn.SetActive(false);
            StartCoroutine(CameraRotateManager.Instance.ZoomOutRotateCam());
        }
       
        //MiniMapRotateUI.instance.miniMapRotateUI.SetActive(isMiniMapScaleUp);
        MiniMapRotateUI.instance.SetActiveMiniMapRotateUI(isMiniMapScaleUp);

        // 미니맵 스케일의 확대/축소 하는 coroutine 실행
        //StartCoroutine(ChangeScaleOfMiniMapModel(miniMapRoot, isMiniMapScaleUp, 2));
    }

    /// <summary>
    /// 미니맵 스케일 컨트롤 버튼을 교체합니다.
    /// </summary>
    /// <param name="isScaleUp">true = 스케일 확대</param>
    public void ChangeMiniMapScaleControlBtn(bool isScaleUp)
    {
        GameObject btnScaleUp = miniMapScaleControlBtn.transform.GetChild(0).gameObject;
        GameObject btnScaleDown = miniMapScaleControlBtn.transform.GetChild(1).gameObject;

        btnScaleUp.SetActive(isScaleUp);
        btnScaleDown.SetActive(!isScaleUp);

        btnScaleUp.GetComponent<BoxCollider2D>().enabled = true;
        btnScaleDown.GetComponent<BoxCollider2D>().enabled = true;

        if(isScaleUp)
        {
            TargetManager.EnableTracking = true;
        }
    }

    public void OnClickMiniMapZoomOut()
    {
        Vector3 currentObjScale             = Vector3.zero;        
        BoxCollider collider                = null;        
        float increaseValue                 = 0f;

        currentObjScale                     = miniMapRoot.transform.localScale;       
        collider                            = miniMapModeling.transform.GetChild(1).GetComponent<BoxCollider>();        

        if ((currentObjScale.x + scaleUpDownIncreaseValue) < scaleUpDownMaxValue)
        {
            miniMapRoot.transform.localScale = new Vector3(currentObjScale.x + scaleUpDownIncreaseValue, currentObjScale.y + scaleUpDownIncreaseValue, currentObjScale.z + scaleUpDownIncreaseValue);
            
            increaseValue = GetBoxColliderIncreaseValue() * scaleUpDownIncreaseValue;
            collider.size = new Vector3(boxColliderDefaultSize.x, collider.size.y - increaseValue, boxColliderDefaultSize.z);

        }
    }

    public void OnClickMiniMapZoomIn()
    {
        Vector3 currentObjScale             = Vector3.zero;
        BoxCollider collider                = null;
        float increaseValue                 = 0f;

        currentObjScale                     = miniMapRoot.transform.localScale;
        collider                            = miniMapModeling.transform.GetChild(1).GetComponent<BoxCollider>();

        if ((currentObjScale.x - scaleUpDownIncreaseValue) > scaleUpDownMinValue)
        {
            miniMapRoot.transform.localScale = new Vector3(currentObjScale.x - scaleUpDownIncreaseValue, currentObjScale.y - scaleUpDownIncreaseValue, currentObjScale.z - scaleUpDownIncreaseValue);

            increaseValue = GetBoxColliderIncreaseValue() * scaleUpDownIncreaseValue;
            collider.size = new Vector3(boxColliderDefaultSize.x, collider.size.y + increaseValue, boxColliderDefaultSize.z);

        }
    }

    public void InitMiniMapBoxColliderSize()
    {
        miniMapModeling.transform.GetChild(1).GetComponent<BoxCollider>().size = boxColliderDefaultSize;
    }

    /// <summary>
    /// 미니맵을 켜거나 끕니다. (외부참조용)
    /// </summary>
    public void OnOffMiniMap(bool status)
    {
        if(miniMapRoot != null)
        {
            miniMapRoot.SetActive(status);
        }
    }

    /// <summary>
    /// ZoomInOut가 진행중인지 체크합니다. (외부참조용)
    /// </summary>
    public bool CheckDoingZoomInOut()
    {
        return isDoingZoomInOut;
    }


    // 비인식일 경우 delegate event 함수
    public void MarkerLostEvent(int targetIdx)
    {
        if (clickedExpandButton == false)
        {
            if (TargetManager.타깃메니저.스케치씬사용)
            {
                if (ColoringManager.컬러링매니저.GetCaptureStatus())
                {
                    miniMapUI.SetActive(true);
                    miniMapCollectingBtn.SetActive(true);
                    RotateUI.회전.회전UI_보이기();
                }
                else
                {
                    if (miniMapModeling.activeSelf)
                    {
                        miniMapUI.SetActive(true);
                    }
                    else
                    {
                        miniMapUI.SetActive(false);
                    }

                    RotateUI.회전.회전UI_숨기기();
                }
            }
            else
            {
                miniMapUI.SetActive(true);
                miniMapCollectingBtn.SetActive(true);
                RotateUI.회전.회전UI_보이기();
            }
        }
        else
        {
            if (TargetManager.타깃메니저.스케치씬사용)
            {
                TargetManager.타깃메니저.HideAllModelingContents();
                RotateUI.회전.회전UI_숨기기();
                MainUI.메인.애니메이션동작_UI숨기기();
                MainUI.메인.색칠하기UI_닫기();
                MainUI.메인.오버레이UI.SetActive(false);
            }
        }
    }
}
