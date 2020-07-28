using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections;

public class SplashManager : MonoBehaviour {

    public UIPanel 도움말;
    public UIToggle 체크박스;
    public static string 씬이름 = "";

    private int 설명다시보지않음;
    private bool 다시보기체크;
    private bool 도움말시작 = false;

    public GameObject splashImg;
    public GameObject hansArCI;

    void Awake()
    {
        // 자동 회전 적용
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = false;
    }

    void Start()
    {
        if (splashImg != null)
        {
            UISprite spr = splashImg.GetComponent<UISprite>();

            switch (LocalizeText.CurrentLanguage)
            {
                case SystemLanguage.Korean:
                    spr.spriteName = "splash";
                    break;

                case SystemLanguage.English:
                    spr.spriteName = "splash_en";
                    break;

                default:
                    spr.spriteName = "splash_en";
                    break;
            }
        }

        /*
        GetData();

        if (설명다시보지않음 != 0)
        {
            다시보기체크 = true;
        }
        else
        {
            다시보기체크 = false;
        }

        Invoke("도움말띄우기", 1.0f);
        */

        Invoke("씬로딩시작", 1.0f);
    }

    void Update()
    {
        if (도움말시작)
        {
            if (도움말.alpha >= 1)
            {
                도움말시작 = false;
            }
            else
            {
                도움말.alpha += Time.deltaTime * 2;

            }
        }
        else
        {
            if (도움말.alpha != 0 && 도움말.alpha != 1)
            {
                도움말.alpha -= Time.deltaTime * 2;

                if (도움말.alpha == 0)
                {
                    도움말.gameObject.SetActive(false);
                }
            }
        }
    }

    private void 씬로딩시작()
    {
        //SceneManager.LoadSceneAsync("00. Product");
        SceneManager.LoadSceneAsync("01. HansMain");        
    }

    void SaveData()
    {
        PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "kw", 설명다시보지않음);
        PlayerPrefs.Save();
    }

    //권기영 : 튜토리얼 체크여부 확인하기 위해 가져옵니다.
    void GetData()
    {
        설명다시보지않음 = PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "kw");
    }

    private void 도움말띄우기()
    {

        if (다시보기체크)
        {
            도움말시작 = false;
            도움말닫고시작();
        }
        else
        {
            도움말시작 = true;
        }
    }

    public void 도움말닫고시작()
    {
        if (체크박스.value)
        {
            설명다시보지않음 = 1;
            SaveData();
        }

        도움말.alpha -= 0.1f;

        hansArCI.GetComponent<TweenAlpha>().enabled = true;
        Invoke("씬로딩시작", 1.0f);
    }

    public void 링크열기()
    {
        Application.OpenURL("http://hansapp.co.kr/page/5_1.php?code=iphone&part=1");
    }
}
