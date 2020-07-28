using UnityEngine;

using System;
using System.IO;
using System.Collections;

public class ShotManager : MonoBehaviour
{
    private const string ScreenshotAlbumName    = "KikyAction";
    private const string ScreenshotTitlePrefix  = "Screenshot";
    private const string ScreenshotExtension    = "jpg";

    private string lastCaptureUrl;

    public GameObject screenshotPreviewUI;
    public GameObject textureObj;
    public Material screenshotMaterial;
    public GameObject shareBtn_ScreenShot;

    public static ShotManager shotmanager;

    void Awake()
    {
        lastCaptureUrl  = string.Empty;
        shotmanager     = this;
    }
    void Start()
    {

        if (screenshotPreviewUI.activeSelf)
        {
            screenshotPreviewUI.SetActive(false);
        }

        Change_ShareImg(shareBtn_ScreenShot);

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 미리보기UI 닫기 
    /// </summary>
    public void ScreenShot_Exit()
    {
        if (MainUI.메인.sceneModeDrive && MainUI.메인.운전하기UI != null)
        {
            MainUI.메인.운전하기UI.SetActive(true);
        }
        screenshotPreviewUI.SetActive(false);
        Time.timeScale = 1;
    }


    /// <summary>
    /// 스크린샷 저장
    /// </summary>
    public void ScreenShot_Save()
    {
        
        //MainUI.메인.메인하위UI_컨트롤();
        textureObj.SetActive(false);
        screenshotPreviewUI.SetActive(false);

        if (MainUI.메인.메뉴UI.transform.parent.GetComponent<UIPanel>() != null)
        {
            MainUI.메인.메뉴UI.transform.parent.GetComponent<UIPanel>().alpha = 0;
        }
        else
        {
            MainUI.메인.메뉴UI.transform.parent.gameObject.AddComponent<UIPanel>();
            MainUI.메인.메뉴UI.transform.parent.GetComponent<UIPanel>().alpha = 0;
        }
        //MainUI.메인.메뉴UI.SetActive(false);

        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
        {
            Application.CaptureScreenshot("HansScreenShot.jpg");
        }
        else
        {
            // 파일이름, 앨범이름, 저장형식
            ScreenshotManager.SaveScreenshot("HansScreenShot", "HansApp", ".jpg");
        }
        Invoke("NGUI카메라UI_보이기", 1.0f);
        
        //ScreenShot_SaveAs();
    }

    public void ScreenShot_AllSave()
    {

        //MainUI.메인.메인하위UI_컨트롤();
        textureObj.SetActive(false);
        screenshotPreviewUI.SetActive(false);
        //MainUI.메인.메뉴UI.SetActive(false);

        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
        {
            Application.CaptureScreenshot("HansScreenShot.jpg");
        }
        else
        {
            // 파일이름, 앨범이름, 저장형식
            ScreenshotManager.SaveScreenshot("HansScreenShot", "HansApp", ".jpg");
        }
        Invoke("NGUI카메라UI_보이기", 1.0f);


        //ScreenShot_SaveAs();
    }

    public void ScreenShot_SaveAs()
    {
        textureObj.SetActive(false);
        screenshotPreviewUI.SetActive(false);

        if (MainUI.메인.메뉴UI.transform.parent.GetComponent<UIPanel>() != null)
        {
            MainUI.메인.메뉴UI.transform.parent.GetComponent<UIPanel>().alpha = 0;
        }
        else
        {
            MainUI.메인.메뉴UI.transform.parent.gameObject.AddComponent<UIPanel>();
            MainUI.메인.메뉴UI.transform.parent.GetComponent<UIPanel>().alpha = 0;
        }

        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
        {
            string savePath = string.Empty;

            savePath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/'));
            savePath = string.Format("{0}/{1}/{2}", savePath, ScreenshotAlbumName, ScreenshotTitlePrefix);

            if (Directory.Exists(savePath) == false) {
                Directory.CreateDirectory(savePath);
            }

            savePath = string.Format("{0}/{1}_{2}.{3}", savePath, ScreenshotTitlePrefix.ToLower(), DateTime.Now.ToString("yyyyMMddHHmmss"), ScreenshotExtension.ToLower());

            Application.CaptureScreenshot(savePath);
            lastCaptureUrl = savePath;
        }
        else
        {
            // 파일이름, 앨범이름, 저장형식
            // ScreenshotTitlePrefix 변수명을 폴더명으로도 사용 함
            ScreenshotManager.SaveScreenshot(ScreenshotTitlePrefix.ToLower(), string.Format("{0}/{1}", ScreenshotAlbumName, ScreenshotTitlePrefix), ScreenshotExtension.ToLower());
            lastCaptureUrl = string.Empty;
        }

        Invoke("NGUI카메라UI_보이기", 1.0f);
    }

    /// <summary>
    /// 스크린샷 공유
    /// </summary>
    public void ScreenShot_Share()
    {
        if (File.Exists(ScreenshotManager.filePath))
        {
            ShareWithApp.ShareImage(ScreenshotManager.filePath);
        }
    }

    /// <summary>
    /// 스크린샷을 찍을 떄 꺼진 UI를 다시 켭니다. 
    /// </summary>
    private void NGUI카메라UI_보이기()
    {
        if (DrivingUI.drivingUI != null)
        {
            DrivingUI.drivingUI.DrivingScreenShotUI();
        }

        MainUI.메인.메뉴UI.transform.parent.GetComponent<UIPanel>().alpha = 1;
        //MainUI.메인.메뉴UI.SetActive(true);
        screenshotPreviewUI.SetActive(true);

        if (MainUI.메인.sceneModeDrive && MainUI.메인.운전하기UI != null)
        {
            MainUI.메인.운전하기UI.SetActive(false);
        }

        StartCoroutine(ScreenShotImgFile_Load());
        Time.timeScale = 0;
    }

    /// <summary>
    /// 스크린샷 이미지를 로드 합니다.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ScreenShotImgFile_Load()
    {
        yield return new WaitForEndOfFrame();

        Rect screenArea = new Rect(0, 0, Screen.width, Screen.height);
        Texture2D texture = new Texture2D((int)screenArea.width, (int)screenArea.height, TextureFormat.RGB24, false);
        texture.ReadPixels(screenArea, 0, 0);
        texture.Apply();
        
        string localPath = string.Empty;
        string imgPath = string.Empty;

        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
        {
            localPath = Application.dataPath;
            imgPath = string.Format("{0}/HansScreenShot.jpg", localPath.Substring(0, localPath.LastIndexOf('/')));            
        }
        else
        {
            imgPath = ScreenshotManager.filePath;
        }

        if (File.Exists(imgPath))
        {
            byte[] byteTexture = File.ReadAllBytes(imgPath);
            if (byteTexture.Length > 0)
            {
                textureObj.transform.localScale = new Vector3(Screen.width / 2, Screen.height / 2, 1);
                texture.LoadImage(byteTexture);
                screenshotMaterial.mainTexture = texture;
            }
            textureObj.SetActive(true);
        }        

        /*
        if (string.IsNullOrEmpty(lastCaptureUrl))
        {
            lastCaptureUrl = ScreenshotManager.filePath;
        }
        
        if (File.Exists(lastCaptureUrl))
        {
            byte[] byteTexture = File.ReadAllBytes(lastCaptureUrl);
            if (byteTexture.Length > 0)
            {
                textureObj.transform.localScale = new Vector3(Screen.width / 2, Screen.height / 2, 1);
                texture.LoadImage(byteTexture);
                screenshotMaterial.mainTexture = texture;
            }
            textureObj.SetActive(true);
        }
        */
    }

    /// <summary>
    /// 플랫폼에 맞게 공유버튼 스프라이트 이미지 변경
    /// </summary>
    /// <param name="obj"></param>
    private void Change_ShareImg(GameObject obj)
    {
        if (obj != null)
        {
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.Android)
            {
                UISprite img = obj.GetComponent<UISprite>();
                img.spriteName = "ui_bc_share_android";
                obj.GetComponent<UIButton>().normalSprite = "ui_bc_share_android";
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                UISprite img = obj.GetComponent<UISprite>();
                img.spriteName = "ui_bc_share_ios7after";
                obj.GetComponent<UIButton>().normalSprite = "ui_bc_share_ios7after";
            }
        }
    }

}
