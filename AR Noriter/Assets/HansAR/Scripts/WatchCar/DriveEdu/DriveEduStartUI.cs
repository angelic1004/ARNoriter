using UnityEngine;
using System.Collections;

public class DriveEduStartUI : MonoBehaviour {
    public GameObject StartBtnRootObj       = null;
    public GameObject StartBtnObj           = null;

    public static DriveEduStartUI instance  = null;

    void Awake()
    {
        instance = this;
    }

	// Use this for initialization
	void Start () {
	        
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnEnable()
    {
        TargetManager.DelEventMarkerLost += EventTrackingLost;
    }

    void OnDisable()
    {
        TargetManager.DelEventMarkerLost -= EventTrackingLost;
    }

    public void EventTrackingLost(int index)
    {
        if (TargetManager.타깃메니저.UsedDriveEdu)
        {
            if (StartBtnRootObj != null)
            {
                ActiveDriveEduStartBtn(true);
            }
        }
    }

    public void ActiveDriveEduStartBtn(bool isActive)
    {
        if (StartBtnRootObj != null)
        {
            StartBtnRootObj.SetActive(isActive);
        }        
    }
}
