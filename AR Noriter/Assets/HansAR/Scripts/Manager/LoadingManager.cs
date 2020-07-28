using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    public GameObject loadingBackImg    = null;
    public UISlider progresBarObj       = null;

    void Awake()
    {
        InitDeviceOrientation();
    }

    void Start()
    {
        ApplyBackgroundImage();
        RequestLoadScene();

        //Invoke("RequestLoadScene", 1.0f);
    }

    void Update()
    {
 
    }

    private void InitDeviceOrientation()
    {
        Screen.orientation                      = ScreenOrientation.LandscapeLeft;
        Screen.autorotateToPortrait             = false;
        Screen.autorotateToPortraitUpsideDown   = false;
        Screen.autorotateToLandscapeLeft        = true;
        Screen.autorotateToLandscapeRight       = false;
    }

    private void ApplyBackgroundImage()
    {
        if (loadingBackImg != null)
        {
            UISprite sprite = loadingBackImg.GetComponent<UISprite>();

            switch (LocalizeText.CurrentLanguage)
            {
                case SystemLanguage.Korean:
                    sprite.spriteName = "splash";
                    break;

                case SystemLanguage.English:
                    sprite.spriteName = "splash_en";
                    break;

                default:
                    sprite.spriteName = "splash_en";
                    break;
            }
        }
    }

    private IEnumerator AsyncLoadScene(string sceneName)
    {
        AsyncOperation async = null;

        async = SceneManager.LoadSceneAsync(sceneName);
        async.allowSceneActivation = false;

        while (async.isDone == false)
        {
            progresBarObj.value = async.progress / 0.9f;

            Debug.LogWarningFormat("progress value = {0}, apply value = {1}", async.progress, progresBarObj.value);

            if (async.progress >= 0.9f)
            {
                progresBarObj.value = 1f;
                async.allowSceneActivation = true;
            }

            yield return new WaitForFixedUpdate();
        }
    }

    private void RequestLoadScene()
    {
        if (!string.IsNullOrEmpty(GlobalDataManager.m_SelectedSceneName))
        {
            StartCoroutine(AsyncLoadScene(GlobalDataManager.m_SelectedSceneName));
        }        
    }
}