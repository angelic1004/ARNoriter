using UnityEngine;

using System;
using System.Collections;
using System.IO;

using Vuforia;

public class PassTargetManager : MonoBehaviour {
    public PassTargetUI             m_PassTargetInst;

    public string                   m_MainSceneName;
    public GameObject               m_ARCameraParent;
    public GameObject[]             m_ParentsOfTargets;

    public static PassTargetManager m_instance;

    void Awake()
    {
        m_instance = this;
    }

	// Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
	
	}
    
    public void CompareTrackingTarget(string trackableName)
    {
        GameObject parentObj    = null;
        bool compareResult      = false;
        string passComment      = string.Empty;

        parentObj               = GetParentObjCurrentCategory();

        if (parentObj == null)
        {
            return;
        }

        //EnableTracking          = false;
        compareResult           = GetResultFindTarget(parentObj, trackableName);

        if (compareResult)
        {
            // 인증 완료 안내 코멘트
            passComment                                 = LocalizeText.Value["Pass_Recognized"];
            GlobalDataManager.m_ConfirmCertification    = true;
        }
        else
        {
            // 인증 실패 안내 코멘트
            passComment                                 = LocalizeText.Value["Pass_Unrecognized"];
            GlobalDataManager.m_ConfirmCertification    = false;
        }

        m_PassTargetInst.m_ScanMsgObj.text = passComment;
        m_PassTargetInst.ActivePopUp(m_PassTargetInst.m_PassPopUpObj);
    }    

    public void PassResultPopUp()
    {
        if (GlobalDataManager.m_ConfirmCertification)
        {
            GlobalDataManager.m_SelectedSceneName = m_MainSceneName;
            GlobalDataManager.m_SelectedCategoryEnum = GlobalDataManager.CategoryType.None;

            GlobalDataManager.GlobalLoadScene();
        }
    }

    /// <summary>
    /// 타겟들의 부모 오브젝트를 모아놓은 배열에서 트래킹 된 타겟이 있는지 확인 합니다.
    /// </summary>
    /// <param name="parentObj">선택한 카테고리의 타겟 부모 오브젝트</param>
    /// <param name="trackableName">추적 한 타겟 이름</param>
    /// <returns>타겟 있으면 true, 타겟 없으면 false</returns>
    private bool GetResultFindTarget(GameObject parentObj, string trackableName)
    {
        bool isFindTarget = false;

        foreach(Transform child in parentObj.transform)
        {
            if (string.Compare(child.GetComponent<ImageTargetBehaviour>().ImageTarget.Name, trackableName) == 0) {
                isFindTarget = true;
                break;
            }
        }

        return isFindTarget;
    }

    /// <summary>
    /// 선택한 카테고리 값으로 해당 카테고리의 타겟 부모 오브젝트를 반환 합니다.
    /// </summary>
    /// <returns>타겟의 부모 오브젝트</returns>
    private GameObject GetParentObjCurrentCategory()
    {
        GameObject resultObj = null;
        /*
        if (GlobalDataManager.m_ResourceFolderEnum == GlobalDataManager.CategoryType.Princess)
        {
            resultObj = m_ParentsOfTargets[0];
        }
        else if (GlobalDataManager.m_ResourceFolderEnum == GlobalDataManager.CategoryType.PrincessDance)
        {
            resultObj = m_ParentsOfTargets[0];
        }
        else if (GlobalDataManager.m_ResourceFolderEnum == GlobalDataManager.CategoryType.Soccer)
        {
            resultObj = m_ParentsOfTargets[1];
        }
        else if (GlobalDataManager.m_ResourceFolderEnum == GlobalDataManager.CategoryType.RacingCar)
        {
            resultObj = m_ParentsOfTargets[2];
        }
        else if (GlobalDataManager.m_ResourceFolderEnum == GlobalDataManager.CategoryType.WatchCar)
        {
            resultObj = m_ParentsOfTargets[3];
        }
        else if (GlobalDataManager.m_ResourceFolderEnum == GlobalDataManager.CategoryType.NasCar)
        {
            resultObj = m_ParentsOfTargets[4];
        }
        */
        return resultObj;
    }

    /// <summary>
    /// 트레커 제어용 프로퍼티
    /// </summary>
    public bool EnableTracking
    {
        get
        {
            return TrackerManager.Instance.GetTracker<ObjectTracker>().IsActive;
        }
        set
        {
            if (value)
            {
                TrackerManager.Instance.GetTracker<ObjectTracker>().Start();
            }
            else
            {
                TrackerManager.Instance.GetTracker<ObjectTracker>().Stop();
            }
        }
    }
}
