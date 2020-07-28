using UnityEngine;
using System;

public abstract class BaseMainUI : Singleton<BaseMainUI> {   
    [Serializable]
    public class QRCodeInfo
    {
        public bool                             usedQRScene;
        public string                           nameQRScene;
    }

    [Serializable]
    public class PassTargetInfo {
        public bool                             usedPassScene;
        public string                           namePassScene;
    }


    [Serializable]
    public class MenuListDepthInformation
    {
        public bool                             moveNextDepth;
        public GameObject                       curDepthObject;                
    }

    [Serializable]
    public class CategoryMenuListDepthInformation
    {
        public GameObject                       curDepthObject;
        public GlobalDataManager.CategoryType   categoryType;
    }    

    [Serializable]
    public class MenuListInformation
    {
        public GameObject                       listParentObj;
        public MenuListDepthInformation[]       menuListDepthInformations;
    }


    [Serializable]
    public class MenuDepthRootInformation
    {
        public GameObject curDepthRootObject;
        public GameObject nextDepthRootObject;
    }

    [Serializable]
    public class MenuCategoryListInformation
    {
        public GameObject listParentObj;
        public MenuDepthRootInformation[] menuDepthRootInformation;
    }


    public GameObject                           m_BackButtonObj;
    public GameObject                           m_ClosePopupObj;
        
    public abstract void OnClickPreDepthButton();
    public abstract void OnClickCategoryButton(GameObject obj);   
    
    public void RefreshScrollObject(GameObject scrollObj)
    {
        GlobalDataManager.RefreshScrollView(scrollObj);
    }

    public void ExpirePopupOpen()
    {
        m_ClosePopupObj.GetComponent<UIPanel>().alpha = 0; // 패널 알파 초기화
        TweenAlpha.Begin(m_ClosePopupObj, 0.3f, 1); // 트윈 알파 컨포넌트 추가

        m_ClosePopupObj.SetActive(true);
    }

    public void ExpirePopupClose()
    {
        TweenAlpha tweenAlpha = m_ClosePopupObj.GetComponent<TweenAlpha>();
        Destroy(tweenAlpha);

        m_ClosePopupObj.SetActive(false);
    }

    public void ApplicationExpire()
    {
        ExpirePopupClose();
        Application.Quit();
    }
}
