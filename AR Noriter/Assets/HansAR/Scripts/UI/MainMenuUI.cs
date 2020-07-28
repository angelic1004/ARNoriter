using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class MainMenuUI : BaseMainUI
{
    private int m_ListDepthIndex;

    public MenuListInformation[] m_MenuListInformation;

    void Awake()
    {
        m_ListDepthIndex = 0;
    }

    // Use this for initialization
    void Start()
    {
        if (GlobalDataManager.m_SelectedCategoryEnum == GlobalDataManager.CategoryType.None)
        {
            return;
        }

        ChangeCategorySpriteName();
        
        if (GlobalDataManager.m_MainMenuDepthValue >= 0)
        {
            m_ListDepthIndex = GlobalDataManager.m_MainMenuDepthValue;
        }

        HideDepthMenuAll();
        m_MenuListInformation[m_ListDepthIndex].listParentObj.SetActive(true);

        ScrollViewRefresh();
    }

    // Update is called once per frame
    void Update()
    {

    }
        
    private int GetFindObjectIndex(string objName, int curDepth)
    {
        int objFindIndex = 0;
        MenuListDepthInformation[] menuListDepthInfo = null;

        menuListDepthInfo = m_MenuListInformation[curDepth].menuListDepthInformations;

        for (int idx = 0; idx < menuListDepthInfo.Length; idx++)
        {
            if (objName.Equals(menuListDepthInfo[idx].curDepthObject.name))
            {
                objFindIndex = idx;
                break;
            }
        }

        return objFindIndex;
    }

    private void HideDepthMenuAll()
    {
        foreach(MenuListInformation info in m_MenuListInformation)
        {
            info.listParentObj.SetActive(false);
        }
    }

    private void ApplyNextMotion(int clickIndex, ButtonInfo btnInfo)
    {
        MenuListDepthInformation menuListDepthInfo = null;

        try
        {
            menuListDepthInfo = m_MenuListInformation[m_ListDepthIndex].menuListDepthInformations[clickIndex];

            if (menuListDepthInfo.moveNextDepth)
            {
                // 다음 뎁스의 메뉴 리스트
                m_MenuListInformation[m_ListDepthIndex].listParentObj.SetActive(false);
                m_ListDepthIndex = m_ListDepthIndex + 1;
                m_MenuListInformation[m_ListDepthIndex].listParentObj.SetActive(true);

                ScrollViewRefresh();
            }
            else
            {
                GlobalDataManager.m_MainMenuDepthValue = m_ListDepthIndex;

                if (btnInfo != null)
                {
                    GlobalDataManager.m_SelectedSceneStateEnum = btnInfo.sceneState;
                    GlobalDataManager.m_SelectedSceneName    = string.Format("{0}_{1}", GlobalDataManager.m_SelectedCategoryEnum.ToString(), btnInfo.loadSceneName);
                    GlobalDataManager.m_AssetBundlePartName  = btnInfo.assetBundleName;
                }

                //GlobalDataManager.GlobalInvokeLoadScene(0.3f);   // 씬 이동            
                GlobalDataManager.GlobalLoadScene();
            }
        }
        catch (Exception msg)
        {
            Debug.LogError(msg.Message);
            throw;
        }        
    }

    private void ScrollViewRefresh()
    {
        RefreshScrollObject(m_MenuListInformation[m_ListDepthIndex].listParentObj);
    }

    public override void OnClickCategoryButton(GameObject obj)
    {
        if (obj == null)
        {
            return;
        }

        int clickObjectIndex = -1;
        ButtonInfo btnInfo = null;

        clickObjectIndex = GetFindObjectIndex(obj.name, m_ListDepthIndex);

        if (clickObjectIndex == -1)
        {
            return;
        }

        if (m_MenuListInformation[m_ListDepthIndex].menuListDepthInformations[clickObjectIndex].moveNextDepth == false)
        {
            btnInfo = obj.GetComponent<ButtonInfo>();
        }

        ApplyNextMotion(clickObjectIndex, btnInfo);
    }

    public override void OnClickPreDepthButton()
    {
        if (m_ListDepthIndex == 0)
        {
            GlobalDataManager.m_MainMenuDepthValue = 0;
            GlobalDataManager.m_SelectedSceneName = string.Format("00. product");
            //GlobalDataManager.GlobalInvokeLoadScene(0.3f);
            GlobalDataManager.GlobalLoadScene();

            return;
        }

        m_MenuListInformation[m_ListDepthIndex].listParentObj.SetActive(false);
        m_ListDepthIndex = m_ListDepthIndex - 1;
        m_MenuListInformation[m_ListDepthIndex].listParentObj.SetActive(true);
        
        ScrollViewRefresh();
    }
    
    /// <summary>
    /// 카테고리 스프라이트 이름을 변경합니다.
    /// </summary>
    public void ChangeCategorySpriteName()
    {
        UISprite[] category = Resources.FindObjectsOfTypeAll<UISprite>();

        foreach (UISprite sprite in category)
        {
            // category 태그가 지정되었을 경우
            if (sprite.gameObject.tag == "category")
            {
                // ProductEnum을 뒤에 붙여서 이름을 변경
                sprite.spriteName = string.Format("{0}_{1}", sprite.spriteName, GlobalDataManager.m_SelectedCategoryEnum.ToString().ToLower());
            }
        }
    }
}