using UnityEngine;
using System.Collections;

public class CircleMenuUI : FileDownloaderUI
{
    public static CircleMenuUI circleInstance;

    void Awake()
    {
        circleInstance = this;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 움직일 프로그래스바 타겟 설정
    /// </summary>
    public void SetProgressbarTarget(GameObject obj)
    {
        m_UpdateText = CircleMenuManager.instance.centerObjSet.mainLabel;
        m_ProgressbarObj = obj.GetComponent<UISlider>();
    }
}
