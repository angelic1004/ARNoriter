using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;

public class FileDownloaderUI : Singleton<FileDownloaderUI> {
    public bool m_UseProgressbar;
    public UISlider m_ProgressbarObj;

    public GameObject m_MessagePopupObj;
    public GameObject m_DataPopupObj;
    public GameObject m_ErrorPopupObj;

    public UILabel m_MessgaePopupText;
    public UILabel m_DataPopupText;
    public UILabel m_ErrorPopupText;
    public UILabel m_UpdateText;    

    void Awake()
    {

    }

	// Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ClosePopupWindow()
    {
        TweenAlpha tweenAlpha1 = m_MessagePopupObj.GetComponent<TweenAlpha>();
        TweenAlpha tweenAlpha2 = m_DataPopupObj.GetComponent<TweenAlpha>();
        TweenAlpha tweenAlpha3 = m_ErrorPopupObj.GetComponent<TweenAlpha>();

        Destroy(tweenAlpha1);
        Destroy(tweenAlpha2);
        Destroy(tweenAlpha3);

        m_MessagePopupObj.SetActive(false);
        m_DataPopupObj.SetActive(false);
        m_ErrorPopupObj.SetActive(false);


        //  HouseMenuManager.instance.MainMenuColliderEnabled(false);

        if (DownloadManager.getInstance.circleMenuManager != null)
        {
            CircleMenuManager.instance.MainMenuColliderEnabled(false);
        }
        else if(DownloadManager.getInstance.houseMenuManager != null)
        {
            HouseMenuManager.instance.MainMenuColliderEnabled(false);
        }
        else
        {
            HMSlideDropUIManager.Instance.clickPrevention.SetActive(false);
        }
    }

    // 함수명을 수정해야 함
    public void MoveProductScene()
    {
        ClosePopupWindow();

        GlobalDataManager.m_SelectedCategoryEnum = GlobalDataManager.CategoryType.None;
        GlobalDataManager.m_SelectedSceneName = string.Format("00. Product");
        //GlobalDataManager.GlobalInvokeLoadScene(0.3f);        
        GlobalDataManager.GlobalLoadScene();
    }

    /// <summary>
    /// 어플리케이션을 종료 합니다.
    /// </summary>
    public void CloseApplication()
    {
        ClosePopupWindow();
        Application.Quit();
    }

    public bool GetUseProgressBarValue()
    {
        return m_UseProgressbar;
    }


    public void OpenPopup()
    {
        //"현재 네트워크 상태가 불안정 합니다."
        m_UpdateText.text = LocalizeText.Value["Net_Unstable"];
        m_MessagePopupObj.GetComponent<UIPanel>().alpha = 0; // 패널 알파 초기화
        TweenAlpha.Begin(m_MessagePopupObj, 0.3f, 1); // 트윈 알파 컨포넌트 추가

        m_MessagePopupObj.SetActive(true);
    }

    public void OpenDataPopup()
    {
        m_DataPopupObj.GetComponent<UIPanel>().alpha = 0; // 패널 알파 초기화
        TweenAlpha.Begin(m_DataPopupObj, 0.3f, 1); // 트윈 알파 컨포넌트 추가

        m_DataPopupObj.SetActive(true);
    }

    public void OepnErrorPopup(string msg)
    {
        m_ErrorPopupText.text = msg;
        m_ErrorPopupObj.GetComponent<UIPanel>().alpha = 0; // 패널 알파 초기화
        TweenAlpha.Begin(m_ErrorPopupObj, 0.3f, 1); // 트윈 알파 컨포넌트 추가

        m_ErrorPopupObj.SetActive(true);
    }
}
