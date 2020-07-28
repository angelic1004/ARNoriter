using UnityEngine;
using System.Collections;

public class HouseMenuUI : FileDownloaderUI
{
    public static HouseMenuUI houseInstance;

    void Awake()
    {
        houseInstance = this;
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    /// <summary>
    /// 움직일 프로그래스바 타겟 설정
    /// </summary>
    public void SetProgressbarTarget(GameObject obj)
    {
        m_UpdateText = HouseMenuManager.instance.centerObjSet.mainLabel;
        m_ProgressbarObj = obj.GetComponent<UISlider>();
    }
}
