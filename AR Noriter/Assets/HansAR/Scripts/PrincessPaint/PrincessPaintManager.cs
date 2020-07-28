using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using HansAR;
using System.Text.RegularExpressions;

public class PrincessPaintManager : MonoBehaviour
{
    public enum CrayonShaderMode
    {
        MULTY_MATERIAL_MODE,            // 머터리얼 여러개 쓰는 모드 (기존 방식)
        ONE_SHADER_MODE                 // 쉐이더 1개에 색상 지정하는 모드
    }

    public static PrincessPaintManager Instance;

    public static int SAVE_PRINCESS_BODY_COUNT = 3;                // 공주 몸 저장 개수

    public CrayonShaderMode crayonShaderMode;                      // 색칠할때 머터리얼 쓰는 방식 인스펙터에서 지정

    [Serializable]
    public class PrincessBodyColor                                 // 공주 몸 색상
    {
        public List<Color32> bodyColor;
    }

    [Serializable]
    public class ThumbnailBodyObj
    {
        public List<GameObject> thumbBody;
    }

    public int currentTargetIndex;                                 // 현재 타겟 인덱스

    public GameObject crayonPaintUI;
    public GameObject runwayStartBtn;                              // 런웨이 시작 버튼
    public GameObject[] princessPaintPanel;

    public List<GameObject> humanBody;                             // 공주 몸

    public Color32 selectedColor;                                  // 선택한 색상
    public GameObject crayons;                                     // 크레용 부모 오브젝트
    private List<GameObject> crayonList;                           // 크레용 리스트
    private int selectedCrayonIndex;                               // 선택한 크레용 인덱스

    public ThumbnailBodyObj[] thumbnailBody;                       // 썸네일 몸
    public GameObject[] thumbnailUI;                               // 썸네일 UI

    public PrincessBodyColor[] savePrincessBodyColor;              // 공주 몸 색상 저장
    public int savePrincessBodyColorIndex;                         // 공주 색상 저장 인덱스

    public Dictionary<int, List<Color32>> savePrincessColorDic;    // 공주 정보 저장용 딕셔너리
    private bool clearColorDic;                                    // 색상 딕셔너리 비울것인지 체크

    public GameObject modelObj;
    //public CrayonModelInfo crayonModelInfo;
    private int copyModelIndex;

    public Color32[] savedPaintColor;
    private int savePaintIndex;

    public Material[] paintMat;
    private GameObject[] showMiniMapObj;
    public GameObject saveLabel;

    public GameObject crayonPreventPanel;                          // 크레용 색칠 막기용 패널 (크레용이 나와있을시 적용)
    public List<bool> touchedPieceNums;                            // 터치한 조각 번호들 (리셋용)


    void Awake()
    {
        Instance = this;

        InitPaintManager();
    }

    void OnEnable()
    {
        StartCoroutine(SetAssetBundleContents());
        TargetManager.DelEventMarkerFound = SetMarkerFoundEvent;
        TargetManager.DelEventMarkerLost = SetMarkerLostEvent;
        TargetManager.DelTrackingReadyEvent = AfterBundleLoadEvent;
    }

    void OnDisable()
    {
        TargetManager.DelEventMarkerFound = null;
        TargetManager.DelEventMarkerLost = null;
        TargetManager.DelTrackingReadyEvent = null;
    }

    private IEnumerator SetAssetBundleContents()
    {
        while (TargetManager.타깃메니저 == null)
        {
            yield return new WaitForSeconds(0.1f);
        }

        HttpRequestDataSet allDataSet = null;

        allDataSet = new HttpRequestDataSet();

        GlobalDataManager.m_ResourceFolderEnum = GlobalDataManager.m_SelectedCategoryEnum;
        GlobalDataManager.m_AssetBundlePartName = "paint";

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
        TargetManager.타깃메니저.StartVuforia();
    }

    private void AfterBundleLoadEvent()
    {
        SetSceneUI();
    }

    private void SetSceneUI()
    {
        StartCoroutine(MainUI.메인.CloseLoadingPopUp());
        RotateUI.회전.회전UI_숨기기();
        MainUI.메인.메인UI.SetActive(false);
    }

    /// <summary>
    /// 마커 인식 이벤트
    /// </summary>
    public void SetMarkerFoundEvent(int targetNum)
    {
        Debug.Log("PrincessPaintManager : 마커 인식 이벤트");

        //TargetManager.타깃메니저.HideAllModelingContents();
        //crayonPaintUI.SetActive(true);

        //TargetManager.EnableTracking = false;
        modelObj = TargetManager.타깃메니저.GetCurrentCopyModel();
        string targetName = TargetManager.타깃메니저.타깃정보[targetNum].마커타깃오브젝트.name.ToLower();

        SetCrayonMat(targetName);
         
        //Debug.Log("타겟번호 : " + targetNum);

        copyModelIndex = SetPaintPanelIndex(targetName);
        StartPrincessPaint(copyModelIndex);

        MiniMapManager.instance.miniMapModeling.SetActive(false);
        MiniMapManager.instance.miniMapScaleControlBtn.SetActive(false);
    }

    private int SetPaintPanelIndex(string targetName)
    {
        int panelIndex = -1;

        for (int i = 0; i < princessPaintPanel.Length; i++)
        {
            if (Regex.IsMatch(princessPaintPanel[i].name, targetName, RegexOptions.IgnoreCase))
            {
                panelIndex = i;
                break;
            }
        }

        if(panelIndex == -1)
        {
            Debug.LogError("Paint Panel과 매칭되는 타겟 이름이 없습니다.");
        }

        return panelIndex;
    }

    /// <summary>
    /// 마커 인식 이벤트
    /// </summary>
    public void SetMarkerLostEvent(int targetNum)
    {
        TargetManager.타깃메니저.HideAllModelingContents();
    }

    private void InitPaintManager()
    {
        crayonPaintUI.SetActive(false);
        crayonList = new List<GameObject>();
        savePrincessColorDic = new Dictionary<int, List<Color32>>();
        paintMat = new Material[4];
        humanBody = new List<GameObject>();

        foreach (Transform child in crayons.transform)
        {
            crayonList.Add(child.gameObject);
        }

        selectedCrayonIndex = -1;

        savePrincessBodyColorIndex = 0;

        currentTargetIndex = -1;
        //SwitchSelectedCrayon(-1);
        clearColorDic = false;
        SetRunwayStartBtn(false);
        saveLabel.SetActive(false);
        crayonPreventPanel.SetActive(false);

        StartCoroutine(InitSelectedCrayon());
    }

    /// <summary>
    /// 그림조각에 색을 칠합니다. (OnClick : Hair, Body, Leg)
    /// </summary>
    public void PaintPicturePiece(GameObject piece)
    {
        UIButton button = piece.GetComponent<UIButton>();
        button.defaultColor = selectedColor;

        for (int i = 0; i < humanBody.Count; i++)
        {
            if (piece.name == humanBody[i].name)
            {
                SetShderColor(i, selectedColor);
                touchedPieceNums[i] = true;
                break;
            }
        }
    }

    private void SetShderColor(int matIdx, Color32 changeColor)
    {
        if (crayonShaderMode == CrayonShaderMode.MULTY_MATERIAL_MODE)
        {
            //만약 적용이 되지않는다면, material의 hansapp/texture with color를 확인해본다
            paintMat[matIdx].SetColor("_Color", changeColor);
        }
        else
        {
            switch (matIdx)
            {
                case 0:
                    paintMat[0].SetColor("_C1", changeColor);
                    break;

                case 1:
                    paintMat[0].SetColor("_C2", changeColor);
                    break;

                case 2:
                    paintMat[0].SetColor("_C3", changeColor);
                    break;
            }
        }
    }

    /// <summary>
    /// 크레용을 선택합니다. (OnClick : Crayon 하위 오브젝트들)
    /// </summary>
    public void OnClickCrayon(GameObject obj)
    {
        int newCrayonIndex = FindCrayonNumber(obj);
        SwitchSelectedCrayon(newCrayonIndex, obj);
        selectedColor = obj.transform.FindChild("Color").GetComponent<UISprite>().color;

        crayonPreventPanel.SetActive(true);
    }

    /// <summary>
    /// 크레용 번호를 찾습니다.
    /// </summary>
    private int FindCrayonNumber(GameObject obj)
    {
        int number = 0;

        for (int i = 0; i < crayonList.Count; i++)
        {
            if (obj.name == crayonList[i].name)
            {
                number = i;
                break;
            }
        }

        return number;
    }

    private IEnumerator InitSelectedCrayon()
    {
        GameObject firstCrayon = crayonList[0];

        while (firstCrayon == null)
        {
            yield return new WaitForFixedUpdate();
        }

        selectedColor = firstCrayon.transform.FindChild("Color").GetComponent<UISprite>().color;

        MoveCrayon(0, -75f);

        selectedCrayonIndex = 0;
    }

    /// <summary>
    /// 선택된 크레용을 바꿉니다.
    /// </summary>
    private void SwitchSelectedCrayon(int newCrayonIndex, GameObject obj)
    {
        if (selectedCrayonIndex != newCrayonIndex)  // 다른 크레용 선택시
        {
            if (selectedCrayonIndex > -1)
            {
                MoveCrayon(selectedCrayonIndex, false);

                // 기존 크레용을 오른쪽로 이동 (짧게 보이도록)
                //MoveCrayon(selectedCrayonIndex, 100f);
            }
            else
            {
                Debug.Log("===START===");

                // 0번 크레용으로 초기화
                selectedColor = crayonList[0].transform.FindChild("Color").GetComponent<UISprite>().color;
            }

            // 새로 선택한 크레용을 왼쪽으로 이동 (길게 보이도록)
            //MoveCrayon(newCrayonIndex, -100f);
            MoveCrayon(newCrayonIndex, true);

            selectedCrayonIndex = newCrayonIndex;
        }
        else   // 동일한 크레용 선택시
        {
            if (obj.transform.localPosition.x > -500)
            {
                MoveCrayon(newCrayonIndex, true);
            }
            else
            {
                MoveCrayon(newCrayonIndex, false, 215);
            }
        }
    }

    /// <summary>
    /// 크레용을 움직입니다.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="moveLength">움직일 거리</param>
    private void MoveCrayon(int index, float distance)
    {
        Vector3 position;

        position = crayonList[index].transform.localPosition;
        position.x += distance;

        crayonList[index].transform.localPosition = position;
    }

    private void MoveCrayon(int index, bool isLeftMove, int fromX = 290)
    {
        crayonList[index].GetComponent<TweenPosition>().from.x = fromX;

        if (isLeftMove)
        {
            crayonList[index].GetComponent<TweenPosition>().PlayForward();
        }
        else
        {
            crayonList[index].GetComponent<TweenPosition>().PlayReverse();
        }
    }

    public void OnClickChildCrayon(GameObject obj)
    {
        GameObject crayonColor = crayonList[selectedCrayonIndex].transform.FindChild("Color").gameObject;

        selectedColor = obj.GetComponent<UIButton>().defaultColor;
        crayonColor.GetComponent<UISprite>().color = selectedColor;

        MoveCrayon(selectedCrayonIndex, false, 215);
        crayonList[selectedCrayonIndex].GetComponent<TweenPosition>().PlayReverse();

        savedPaintColor[savePaintIndex] = selectedColor;

        crayonPreventPanel.SetActive(false);
    }

    /// <summary>
    /// 리셋 컬러 버튼 클릭 이벤트 (OnClick : ResetColor 버튼)
    /// </summary>
    public void OnClickResetColorBtn()
    {
        ResetPaintPanelColor();
    }

    private void ResetPaintPanelColor()
    {
        // 색상 초기화
        for (int i = 0; i < humanBody.Count; i++)
        {
            humanBody[i].GetComponent<UIButton>().defaultColor = Color.white;
        }
    }

    ///// <summary>
    ///// 페인트 저장 클릭 이벤트 (Onclick : Save Paint Color)
    ///// </summary>
    //public void OnClickSavePaintColorBtn()
    //{
    //    Color32 bodyColor;

    //    // 딕셔너리에 저장될 색상값 동적 생성
    //    List<Color32> dicColorSave = new List<Color32>();

    //    //Debug.Log("Save BodyColor Index : " + savePrincessBodyColorIndex);

    //    // 현재 색칠한 컬러 저장
    //    for (int i = 0; i < savePrincessBodyColor[savePrincessBodyColorIndex].bodyColor.Count; i++)
    //    {
    //        bodyColor = humanBody[i].GetComponent<UIButton>().defaultColor;

    //        savePrincessBodyColor[savePrincessBodyColorIndex].bodyColor[i] = bodyColor;

    //        dicColorSave.Add(bodyColor);
    //    }

    //    ApplyThumbnailColor(savePrincessBodyColorIndex);

    //    SavePrincessColorInfo(savePrincessBodyColorIndex, dicColorSave);

    //    if (!thumbnailUI[savePrincessBodyColorIndex].activeSelf)
    //    {
    //        thumbnailUI[savePrincessBodyColorIndex].SetActive(true);
    //    }

    //    // 저장 인덱스 설정
    //    if (savePrincessBodyColorIndex < savePrincessBodyColor.Length - 1)
    //    {
    //        savePrincessBodyColorIndex++;
    //    }
    //    else
    //    {
    //        savePrincessBodyColorIndex = 0;
    //    }

    //    ResetPaintPanelColor();
    //    SetRunwayStartBtn(true);

    //    RunwayManager.Instance.UpdateRunwayRealManager(currentTargetIndex);
    //}

    /// <summary>
    /// 터치하지 않은 조각의 색상을 흰색으로 리셋함
    /// </summary>
    private void ResetUntouchedPieceColor()
    {
        for (int i = 0; i < touchedPieceNums.Count; i++)
        {
            if (touchedPieceNums[i] == false)
            {
                SetShderColor(i, Color.white);
            }

            touchedPieceNums[i] = false;
        }
    }

    public void OnClickSavePaintBtn()
    {
        StartCoroutine(ShowSaveLabel());

        ResetUntouchedPieceColor();

        if (showMiniMapObj != null)
        {
            for (int i = 0; i < showMiniMapObj.Length; i++)
            {
                showMiniMapObj[i].SetActive(true);
            }

            showMiniMapObj = null;
        }
    }

    private IEnumerator ShowSaveLabel()
    {
        saveLabel.SetActive(true);

        yield return new WaitForSeconds(1.5f);

        saveLabel.SetActive(false);
        ResetPaintPanelColor();
        SetRunwayStartBtn(true);
    }

    public void OnClickStartMiniMap()
    {
        crayonPaintUI.SetActive(false);

        MiniMapManager.instance.SetActivationMiniMapModel(true);
        MiniMapManager.instance.miniMapUI.SetActive(true);
        MiniMapManager.instance.miniMapCollectingBtn.SetActive(false);
        MiniMapManager.instance.miniMapScaleControlBtn.SetActive(true);

        MiniMapManager.instance.OnClickMiniMapScaleUpDown(MiniMapManager.instance.miniMapScaleControlBtn.transform.GetChild(0).gameObject);
    }

    /// <summary>
    /// 썸네일에 색상 적용
    /// </summary>
    private void ApplyThumbnailColor(int index)
    {
        for (int i = 0; i < savePrincessBodyColor[index].bodyColor.Count; i++)
        {
            // 썸네일에 색상 적용
            thumbnailBody[index].thumbBody[i].GetComponent<UISprite>().color = savePrincessBodyColor[index].bodyColor[i];

            //// 모델링 머터리얼에 색상 적용
            //humanMaterial[i].SetColor("_EmissionColor", savePrincessBodyColor[index].bodyColor[i]);
        }
    }

    /// <summary>
    /// 공주 색상 정보를 딕셔너리에 저장
    /// </summary>
    private void SavePrincessColorInfo(int bodyColorIndex, List<Color32> savedColor)
    {
        // 현재 인덱스*10 + 바디컬러 인덱스를 더해서 딕셔너리 키값 지정
        int keyValue = currentTargetIndex * 10 + bodyColorIndex;

        //Debug.Log("keyValue : " + keyValue);

        // 저장 중복 체크 후 저장
        if (savePrincessColorDic.ContainsKey(keyValue) == false)
        {
            savePrincessColorDic.Add(keyValue, savedColor);
        }
        else
        {
            savePrincessColorDic[keyValue] = savedColor;
        }
    }

    /// <summary>
    /// 공주컬러 딕셔너리 키값이 존재하는지 체크 (외부호출용)
    /// </summary>
    public bool CheckPrincessColorDicKey(int keyValue)
    {
        if (savePrincessColorDic.ContainsKey(keyValue))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void OnOffPrincessPaintUI(bool active)
    {
        crayonPaintUI.SetActive(active);
    }

    private void ResetThumbnailUI()
    {
        for (int i = 0; i < thumbnailUI.Length; i++)
        {
            thumbnailUI[i].SetActive(false);
        }
    }

    /// <summary>
    /// 프린세스 페인트 실행 (외부 호출용)
    /// </summary>
    public void StartPrincessPaint(int targetIndex)
    {
        HidePrincessPaintPanel();
        MainUI.메인.인식글자UI.SetActive(false);

        ClearColorDictionary();
        ResetThumbnailUI();

        GameObject princess = princessPaintPanel[targetIndex];
        princess.SetActive(true);

        crayonPaintUI.SetActive(true);

        int i = 0;
        int totalCrayonSpriteLength;         // 총 색칠할 스프라이트 개수 (일단 하드코딩 추후에 수정)

        if (crayonShaderMode == CrayonShaderMode.MULTY_MATERIAL_MODE)
        {
            // 기존 방식일때 스프라이트 개수 4개
            totalCrayonSpriteLength = 4;
        }
        else
        {
            // 쉐이더 방식일때 스프라이트 개수 3개
            totalCrayonSpriteLength = 3;
        }

        humanBody.Clear();
        foreach (Transform child in princess.transform)
        {
            if (i < totalCrayonSpriteLength)
            {
                humanBody.Add(child.gameObject);
            }

            ResetThumbnailSprites(i, child.gameObject.GetComponent<UISprite>());
            i++;
        }

        
        // 썸네일에 Line 있는경우 없는경우 예외처리 (하드코딩)
        if (i < thumbnailBody[0].thumbBody.Count)
        {
            OnOffThumbnailBody(5, false);
        }
        else
        {
            OnOffThumbnailBody(5, true);
        }

        currentTargetIndex = targetIndex;
        savePrincessBodyColorIndex = 0;

        ResetTouchedPieceNums();

        MiniMapManager.instance.miniMapScaleControlBtn.SetActive(false);
    }

    private void ClearColorDictionary()
    {
        if (clearColorDic)
        {
            Debug.Log("===Dictionary Clear==");
            savePrincessColorDic.Clear();
            RunwayManager.Instance.ResetRunwayManager();
            SetRunwayStartBtn(false);

            clearColorDic = false;
        }
    }

    /// <summary>
    /// 공주 페인트 패널을 숨깁니다.
    /// </summary>
    public void HidePrincessPaintPanel()
    {
        for (int i = 0; i < princessPaintPanel.Length; i++)
        {
            princessPaintPanel[i].SetActive(false);
        }
    }

    /// <summary>
    /// 썸네일 스프라이트 리셋
    /// </summary>
    public void ResetThumbnailSprites(int elementNum, UISprite princessSprite)
    {
        UISprite sprite;

        for (int i = 0; i < thumbnailBody.Length; i++)
        {
            sprite = thumbnailBody[i].thumbBody[elementNum].GetComponent<UISprite>();

            sprite.atlas = princessSprite.atlas;
            sprite.spriteName = princessSprite.spriteName;

            sprite.color = Color.white;
        }
    }

    private void OnOffThumbnailBody(int num, bool active)
    {
        for (int i = 0; i < thumbnailBody.Length; i++)
        {
            thumbnailBody[i].thumbBody[num].SetActive(active);
        }
    }

    /// <summary>
    /// 런웨이 버튼 클릭이벤트 (참조 : Runway Start 버튼)
    /// </summary>
    public void OnClickRunwayStartButton()
    {
        crayonPaintUI.SetActive(false);
        MiniMapManager.instance.miniMapModeling.SetActive(true);
        MiniMapManager.instance.miniMapUI.SetActive(true);

        MiniMapManager.instance.OnClickMiniMapScaleUpDown(MiniMapManager.instance.btnScaleUp);
    }

    public void SwitchColorDicClearStatus(bool isClear)
    {
        clearColorDic = isClear;
    }

    /// <summary>
    /// 런웨이 시작 버튼을 활성화/비활성화 시킵니다.
    /// </summary>
    /// <param name="isActive">활성화 여부</param>
    public void SetRunwayStartBtn(bool isActive)
    {
        GameObject child = runwayStartBtn.transform.GetChild(0).gameObject;
        UIButton btn = runwayStartBtn.GetComponent<UIButton>();

        if (isActive)
        {
            runwayStartBtn.GetComponent<BoxCollider2D>().enabled = true;
            runwayStartBtn.GetComponent<UISprite>().color = Color.white;
            ApplyButtonColor(btn, Color.white);
            child.GetComponent<UISprite>().color = new Color32(214, 134, 247, 255);
        }
        else
        {
            runwayStartBtn.GetComponent<BoxCollider2D>().enabled = false;
            Color32 gray = new Color32(185, 185, 185, 185);
            ApplyButtonColor(btn, gray);
            child.GetComponent<UISprite>().color = new Color32(120, 120, 120, 255);
        }
    }

    private void ApplyButtonColor(UIButton btn, Color32 color)
    {
        btn.defaultColor = color;
        btn.hover = color;
        btn.pressed = color;
        btn.disabledColor = color;
    }

    /// <summary>
    /// 페인트 모델 머터리얼 컬러를 바꿉니다
    /// </summary>
    public void OnClickApplyColorBtn()
    {
        //ApplyPaintColor();
    }

    private void SetCrayonMat(string targetName)
    {
        MiniMapInfo info = MiniMapManager.instance.miniMapModeling.GetComponent<MiniMapInfo>();
        //string targetName = TargetManager.타깃메니저.타깃정보[targetNum].마커타깃오브젝트.name.ToLower();

        bool isApplyPaint = false;

        for (int i = 0; i < info.miniMapResourceInfo.Length; i++)
        {
            if (Regex.IsMatch(info.miniMapResourceInfo[i].miniMapCompareModelName, targetName, RegexOptions.IgnoreCase))
            {
                if (isApplyPaint) { break; }

                SetPaintMat(info.miniMapResourceInfo[i].miniMapMaterials);
                showMiniMapObj = info.miniMapResourceInfo[i].miniMapObjects;

                isApplyPaint = true;
            }
        }

        //crayonModelInfo = modelObj.GetComponent<CrayonModelInfo>();
    }

    //private void ApplyPaintColor()
    //{
    //    for (int i = 0; i < crayonModelInfo.paintMaterial.Length; i++)
    //    {
    //        crayonModelInfo.paintMaterial[i].SetColor("_Color", savedPaintColor[i]);
    //    }
    //}

    private void SetPaintMat(Material[] modelMat)
    {
        if (crayonShaderMode == CrayonShaderMode.MULTY_MATERIAL_MODE)
        {
            if (modelMat.Length == paintMat.Length)
            {
                for (int i = 0; i < paintMat.Length; i++)
                {
                    paintMat[i] = modelMat[i];
                }
            }
            else
            {
                Debug.LogError("모델링과 색칠 Matrial 개수가 같지 않습니다.");
            }
        }
        else
        {
            paintMat[0] = modelMat[0];
        }
    }

    public void OnClickPreventCrayonClickPanel()
    {
        MoveCrayon(selectedCrayonIndex, false, 215);
        crayonPreventPanel.SetActive(false);
    }

    private void ResetTouchedPieceNums()
    {
        touchedPieceNums.Clear();

        for (int i = 0; i < humanBody.Count; i++)
        {
            touchedPieceNums.Add(false);
        }
    }
}   