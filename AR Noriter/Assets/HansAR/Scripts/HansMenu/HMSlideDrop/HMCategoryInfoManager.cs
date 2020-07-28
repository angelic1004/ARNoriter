using UnityEngine;
using System.Collections;
using System;

public class HMCategoryInfoManager : MonoBehaviour
{
    public static HMCategoryInfoManager Instance;

    [Serializable]
    public class CategoryOneInfo
    {
        public string labelText;
        public string spriteName;
        public string iconName;
        //public Color32 bgColor;
        public bool showDefaultDownCategory;

        public CategoryTwoInfo[] categoryTwoInfo;
    }

    [Serializable]
    public class CategoryTwoInfo
    {
        public string labelText;                     // Localize Value 값을 적어야 됨
        public string spriteName;                    // CategoryOneInfo와 같게 적용할 경우 비워둘 것
        public string iconName;
        public Color32 bgColor;

        // 3번째 카테고리 없이 바로 로딩하는 경우 작성 //
        public string assetBundleName;
        public string loadSceneName;
        public GlobalDataManager.SceneState sceneState;
        //////////////////////////////////////////////

        public DownCategoryInfo[] downCategory;
    }

    [Serializable]
    public class DownCategoryInfo
    {
        public string labelText;                      // Localize Value 값을 적어야 됨
        public string spriteName;
        public string iconName;
        public Color32 bgColor;                       // CategoryTwoInfo와 같게 적용할 경우 비워둘 것

        public string assetBundleName;
        public string loadSceneName;
        public GlobalDataManager.SceneState sceneState;
    }

    public static int CATEGORY_TWO_BUTTON_HEIGHT = 480;                // 2번째 카테고리 버튼 높이
    public static int CATEGORY_TWO_COUNT = 6;                          // 2번째 카테고리 최대 개수

    public static int CATEGORY_THREE_BUTTON_HEIGHT = 183;              // 3번째 카테고리 버튼 높이
    public static int CATEGORY_THREE_COUNT = 5;                        // 3번째 카테고리 최대 개수

    public GameObject[] categoryTwoObj;                                // 2번째 카테고리 오브젝트
    private GameObject[] downCategoryObj;                              // 3번째 카테고리 오브젝트

    public CategoryOneInfo[] categoryOneInfo;
    private CategoryTwoInfo[] categoryTwoInfo;

    private Transform downCategory;

    [HideInInspector]
    public bool[] emptyDownCategory;                                   // 다운 카테고리를 가지지 않은 경우

    void Awake()
    {
        Instance = this;

        InitCategoryThree();

        emptyDownCategory = new bool[CATEGORY_TWO_COUNT];
    }


    #region 유틸리티

    /// <summary>
    /// 라벨 텍스트를 변경합니다.
    /// </summary>
    private void ChangeLabelText(GameObject obj, string localizeValue)
    {
        // 라벨텍스트가 공백이 아닌 경우에만 변경해준다. (Localize Value)
        if (string.IsNullOrEmpty(localizeValue) == false)
        {
            obj.transform.FindChild("Label").GetComponent<UILabel>().text = LocalizeText.Value[localizeValue];
        }
    }

    /// <summary>
    /// Symbol 아이콘을 변경합니다.
    /// </summary>
    private void ChangeSymbolIcon(GameObject obj, string iconName)
    {
        // iconName이 공백이 아닌경우에만 변경해준다.
        if (iconName != "")
        {
            obj.transform.FindChild("Symbol").GetComponent<UISprite>().spriteName = iconName;

            // TODO : 딕셔너리에 복구용 iconName 저장
        }
    }

    #endregion


    #region 2번째 카테고리 관련 함수


    /// <summary>
    /// 2번째 카테고리 보여주기를 리셋합니다.
    /// </summary>
    private void ResetSecondCategoryActive(int categoryLength)
    {
        for (int i = 0; i < categoryTwoObj.Length; i++)
        {
            if(i < categoryLength)
            {
                categoryTwoObj[i].SetActive(false);
                categoryTwoObj[i].SetActive(true);
            }
            else
            {
                categoryTwoObj[i].SetActive(false);
            }
        }
    }

    /// <summary>
    /// 2번째 카테고리 정보 셋팅
    /// </summary>
    private void SetSecondCategoryInfo(CategoryTwoInfo categoryInfo, int index)
    {
        if(categoryInfo.downCategory.Length == 0)
        {
            ButtonInfo btnInfo = categoryTwoObj[index].GetComponent<ButtonInfo>();
            btnInfo.assetBundleName = categoryInfo.assetBundleName;
            btnInfo.loadSceneName = categoryInfo.loadSceneName;
            btnInfo.sceneState = categoryInfo.sceneState;

            emptyDownCategory[index] = true;
        }
    }

    public void ResetEmptyDownCategory()
    {
        for(int i=0; i < emptyDownCategory.Length; i++)
        {
            emptyDownCategory[i] = false;
        }
    }

    public bool CheckDirectLoading(GameObject obj)
    {
        int index = -1;

        for(int i=0; i< categoryTwoObj.Length; i++)
        {
            if(obj == categoryTwoObj[i])
            {
                index = i;
            }
        }

        if(emptyDownCategory[index])
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //////////////// HMSlideDropUIManager에서 옮겨온 부분 ////////////////

    /// <summary>
    /// 카테고리2 값들 변경
    /// </summary>
    /// i=카테고리1 인덱스, j=카테고리2 인덱스
    public void ChangeCategoryValues(int clickObjIndex)
    {
        UIButton btn;
        CategoryTwoInfo cateTwoInfo;

        int categoryTwoLength = categoryOneInfo[clickObjIndex].categoryTwoInfo.Length;

        try
        {
            for (int i = 0; i < categoryTwoLength; i++)
            {
                btn = categoryTwoObj[i].GetComponent<UIButton>();

                ChangeCategoryBtnSprite(btn, clickObjIndex, i);
                ChangeCategoryBtnColor(btn, clickObjIndex, i);

                cateTwoInfo = categoryOneInfo[clickObjIndex].categoryTwoInfo[i];

                ChangeLabelText(categoryTwoObj[i], cateTwoInfo.labelText);
                ChangeSymbolIcon(categoryTwoObj[i], cateTwoInfo.iconName);
            }

            SetThirdCategoryIndex(clickObjIndex);
        }
        catch (Exception ex)
        {
            Debug.LogError(string.Format("exception message : {0}", ex.Message));
            throw;
        }
    }
    
    /// <summary>
    /// 카테고리 버튼 스프라이트를 변경합니다.
    /// </summary>
    public void ChangeCategoryBtnSprite(UIButton btn, int i, int j)
    {
        // 버튼 스프라이트 교체
        btn.normalSprite = categoryOneInfo[i].spriteName;
    }

    /// <summary>
    /// 카테고리 버튼 색상을 변경합니다.
    /// </summary>
    public void ChangeCategoryBtnColor(UIButton btn, int i, int j)
    {
        // 버튼 색상 변경
        btn.defaultColor = categoryOneInfo[i].categoryTwoInfo[j].bgColor;
        btn.hover = btn.defaultColor;
        btn.pressed = new Color32(170, 170, 170, 255);

        // 0번째 카테고리 색상을
        if (j == 0)
        {
            // 왼쪽 바에도 적용
            HMSlideDropUIManager.Instance.leftMenuBar.GetComponent<UISprite>().color = btn.defaultColor;
        }
    }

    #endregion



    #region 3번째 카테고리 관련 함수

    /// <summary>
    /// Down Category 오브젝트를 꺼줍니다.
    /// </summary>
    public IEnumerator OffDownCategoryObj()
    {
        // 슬라이드 시간 때문에 0.5초 기다림
        yield return new WaitForSeconds(0.5f);

        for (int i=0; i< downCategoryObj.Length; i++)
        {
            downCategoryObj[i].SetActive(false);
        }
    }

    /// <summary>
    /// 3번째 카테고리 리스트를 셋팅합니다.
    /// </summary>
    /// <param name="downCategoryObj">3번째 카테고리 오브젝트</param>
    /// <param name="downCategoryCount">3번째 카테고리 목록 갯수</param>
    public void SetDownCategoryList(DownCategoryInfo[] downCategoryObj, int secondCategoryIndex)
    {
        int downCategoryCount = downCategoryObj.Length;
        downCategory = categoryTwoObj[secondCategoryIndex].transform.FindChild("Down Category");

        // 카테고리를 보여줄 카운트 크기만큼
        if (downCategoryCount > 0)
        {
            categoryTwoObj[secondCategoryIndex].GetComponent<UIPlayTween>().tweenTarget = downCategory.gameObject;
            downCategory.gameObject.SetActive(true);
            // 해당하는 개수만큼 위치 재조정
            downCategory.GetComponent<TweenPosition>().to =
                new Vector3(0, -(CATEGORY_TWO_BUTTON_HEIGHT / 2 + downCategoryCount * CATEGORY_THREE_BUTTON_HEIGHT), 0);
        }
        else
        {
            categoryTwoObj[secondCategoryIndex].GetComponent<UIPlayTween>().tweenTarget = null;
            downCategory.gameObject.SetActive(false);
            // 0개일때는 움직이지 않도록 위치 재조정
            downCategory.GetComponent<TweenPosition>().to = downCategory.GetComponent<TweenPosition>().from;
        }
    }

    /// <summary>
    /// 3번째 카테고리를 설정합니다.
    /// </summary>
    private void InitCategoryThree()
    {
        try
        {
            downCategoryObj = new GameObject[CATEGORY_TWO_COUNT * CATEGORY_THREE_COUNT];
            int index = 0;

            for (int i = 0; i < categoryTwoObj.Length; i++)
            {
                Transform downCategory = categoryTwoObj[i].transform.FindChild("Down Category");

                foreach (Transform child in downCategory)
                {
                    if (child.name.Contains("BTN"))
                    {
                        downCategoryObj[index] = child.gameObject;
                        downCategoryObj[index].SetActive(false);
                    }
                    index++;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(string.Format("exception message : {0}", ex.Message));
            throw;
        }
    }

    /// <summary>
    /// 3번째 카테고리 인덱스 설정
    /// </summary>
    public void SetThirdCategoryIndex(int firstCategoryIndex)
    {
        // 2번째 카테고리 정보
        categoryTwoInfo = categoryOneInfo[firstCategoryIndex].categoryTwoInfo;
        int index;

        ResetSecondCategoryActive(categoryTwoInfo.Length);

        Color32 black = new Color32(0, 0, 0, 0);

        try
        {
            for (int i = 0; i < categoryTwoInfo.Length; i++)
            {
                SetSecondCategoryInfo(categoryTwoInfo[i], i);
                SetDownCategoryList(categoryTwoInfo[i].downCategory, i);

                for (int j = 0; j < categoryTwoInfo[i].downCategory.Length; j++)
                {
                    index = (((i + 1) * CATEGORY_THREE_COUNT) - categoryTwoInfo[i].downCategory.Length) + j;

                    downCategoryObj[index].SetActive(true);

                    ChangeThirdCategoryValues(index, categoryTwoInfo[i].downCategory[j]);

                    // 색상 변경
                    if (categoryTwoInfo[i].downCategory[j].bgColor.Equals(black))
                    {
                        ChangeDownCategoryColor(index, categoryTwoInfo[i].bgColor);
                    }
                    else
                    {
                        ChangeDownCategoryColor(index, categoryTwoInfo[i].downCategory[j].bgColor);
                    }
                }

                ResetDownCategory();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(string.Format("exception message : {0}", ex.Message));
            throw;
        }
    }

    /// <summary>
    /// 3번째 카테고리 값들을 변경합니다.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="loadSceneName"></param>
    private void ChangeThirdCategoryValues(int index, DownCategoryInfo downCategoryInfo)
    {
        ButtonInfo btnInfo = downCategoryObj[index].GetComponent<ButtonInfo>();

        //////////// 여기에 변경할 것들 적용하면 됨 /////////////////
        btnInfo.assetBundleName = downCategoryInfo.assetBundleName;
        btnInfo.loadSceneName = downCategoryInfo.loadSceneName;
        btnInfo.sceneState = downCategoryInfo.sceneState;

        ChangeLabelText(downCategoryObj[index], downCategoryInfo.labelText);
        ChangeSymbolIcon(downCategoryObj[index], downCategoryInfo.iconName);
    }

    private void ChangeDownCategoryColor(int index, Color color)
    {
        UIButton downCateButton = downCategoryObj[index].GetComponent<UIButton>();

        downCateButton.defaultColor = color;
        downCateButton.hover = color;
        downCateButton.pressed = new Color32(170, 170, 170, 255);
    }

    /// <summary>
    /// Down Category 오브젝트를 껐다 켜줍니다. (껐다 켜줘야 NGUI에서 변경된 값이 적용될 경우)
    /// </summary>
    public void ResetDownCategory()
    {
        downCategory.gameObject.SetActive(true);
        downCategory.gameObject.SetActive(false);
    }

    #endregion
}