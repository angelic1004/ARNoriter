using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using System.IO;

[Serializable]
public class ProductMenuButtonData
{
    public string productName;                                     // 상품명
    public string spriteName;                                      // Sprite 이름
    public string loadSceneName;                                   // 이동할 씬이름
    public string totalAssetSize;                                  // 에셋 크기
    public string assetVersion;                                    // 에셋 버전
}

public class ProductListUI : MonoBehaviour
{
    public GameObject sampleMenuButton;                            // 복제할 샘플메뉴버튼
    public GameObject menuBtnGrid;                                 // 메뉴버튼 그리드
    public GameObject menuList;                                    // 메뉴 리스트

    public ProductMenuButtonData[] menuButtonData;                 // 메뉴버튼 데이터

    // 읽어올 Json 파일 이름
    private const string LOAD_JSON_FILE_NAME = "AssetsMenuButtonData" + ".json";

    // 저장할 Json 파일 이름
    private const string SAVE_JSON_FILE_NAME = "AssetsMenuButtonData" + ".json";

    void Awake()
    {

    }

    void Start()
    {
        GetJsonDataFromFile(ref menuButtonData);

        SetProductMenuButton();
    }

    /// <summary>
    /// Product 메뉴버튼을 세팅합니다.
    /// </summary>
    private void SetProductMenuButton()
    {
        GameObject[] copyMenu = new GameObject[menuButtonData.Length];

        sampleMenuButton.SetActive(true);

        for (int i = 0; i < menuButtonData.Length; i++)
        {
            copyMenu[i] = CreateMenuButton();

            SetMenuButtonValues(copyMenu[i], menuButtonData[i]);
        }

        sampleMenuButton.SetActive(false);

        menuList.GetComponent<UIScrollView>().ResetPosition();
    }

    /// <summary>
    /// 메뉴 버튼 복제 생성
    /// </summary>
    private GameObject CreateMenuButton()
    {
        GameObject copyObject = null;

        // 복제
        copyObject = Instantiate(sampleMenuButton) as GameObject;
        copyObject.transform.SetParent(menuBtnGrid.transform);

        // Transform 리셋
        copyObject.transform.position = Vector3.zero;
        copyObject.transform.rotation = Quaternion.identity;
        copyObject.transform.localScale = Vector3.one;

        return copyObject;
    }

    /// <summary>
    /// 복사된 메뉴버튼 데이터 설정
    /// </summary>
    private void SetMenuButtonValues(GameObject copyMenu, ProductMenuButtonData menuButtonData)
    {
        copyMenu.name = menuButtonData.productName;
        copyMenu.GetComponent<UISprite>().spriteName = menuButtonData.spriteName;

        //copyMenu.GetComponent<UITexture>().mainTexture  = Resources.Load("dino") as Texture;

        ProductMenuButtonInfo info = copyMenu.GetComponent<ProductMenuButtonInfo>();

        info.productNameLabel.text = menuButtonData.productName;
        info.loadSceneName = menuButtonData.loadSceneName;

        AddOnClickEvent(this, copyMenu.GetComponent<UIButton>(), "LoadCategoryScene", info.loadSceneName, typeof(string));
    }

    /// <summary>
    /// 파일로부터 Json 데이터를 얻어옵니다.
    /// </summary>
    public void GetJsonDataFromFile(ref ProductMenuButtonData[] data)
    {
        string loadText;

        string path = Application.dataPath + "/" + LOAD_JSON_FILE_NAME;
        loadText = File.ReadAllText(path);

        data = JsonHelper.FromJson<ProductMenuButtonData>(loadText);
    }

    /// <summary>
    /// Json 파일로 저장합니다.
    /// </summary>
    public void SaveJsonDataToFile()
    {
        string jsonString = JsonHelper.ToJson(menuButtonData, true);
        Debug.Log(jsonString);

        string path = Application.dataPath + "/" + SAVE_JSON_FILE_NAME;
        File.WriteAllText(path, jsonString);
    }


    public void AddOnClickEvent(MonoBehaviour target, UIButton btn, string method, object value, Type type)
    {
        EventDelegate clickEventDelegate = new EventDelegate(target, method);
        EventDelegate.Parameter param = new EventDelegate.Parameter();

        param.value = value;
        param.expectedType = type;

        clickEventDelegate.parameters[0] = param;

        EventDelegate.Add(btn.onClick, clickEventDelegate);
    }    

    private void LoadCategoryScene(string sceneName)
    {
        //SceneManager.LoadScene(sceneName);
        SceneManager.LoadScene("00. QRCode");
    }
}