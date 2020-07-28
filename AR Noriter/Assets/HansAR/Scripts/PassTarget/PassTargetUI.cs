using UnityEngine;
using System.Collections;

public class PassTargetUI : MonoBehaviour
{
    public GameObject m_BackPopUpObj;
    public GameObject m_PassPopUpObj;

    public GameObject m_BlackBgObj;

    public UILabel m_ScanMsgObj;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ActivePopUp(GameObject obj)
    {
        if (obj == null)
        {
            return;
        }
        
        PassTargetManager.m_instance.EnableTracking = false;

        obj.GetComponent<UIPanel>().alpha = 0;   // 패널 알파 초기화
        TweenAlpha.Begin(obj, 0.3f, 1);          // 트윈 알파 컨포넌트 추가

        obj.SetActive(true);
        m_BlackBgObj.SetActive(true);
    }

    public void MoveMainMenu()
    {
        GlobalDataManager.m_SelectedSceneName   = "01. HansMain";
        GlobalDataManager.m_SelectedCategoryEnum = GlobalDataManager.CategoryType.None;
        GlobalDataManager.m_ResourceFolderEnum  = GlobalDataManager.CategoryType.None;

        GlobalDataManager.GlobalLoadScene();
    }

    public void ClosePopUp(GameObject obj)
    {
        if (obj == null)
        {
            return;
        }

        TweenAlpha TweenAlpha = obj.GetComponent<TweenAlpha>();
        Destroy(TweenAlpha);

        obj.SetActive(false);
        m_BlackBgObj.SetActive(false);

        PassTargetManager.m_instance.EnableTracking = true;
    }
}
