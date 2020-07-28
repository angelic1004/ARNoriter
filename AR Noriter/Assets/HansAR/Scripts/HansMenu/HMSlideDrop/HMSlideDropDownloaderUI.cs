using UnityEngine;
using System.Collections;

public class HMSlideDropDownloaderUI : FileDownloaderUI
{
    private GameObject parentIconObj = null;

    public static HMSlideDropDownloaderUI Instance;    

    void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// 움직일 프로그래스바 타겟 설정
    /// </summary>
    public void SetProgressbarTarget(GameObject obj)
    {
        GameObject childObj;
        childObj = obj.transform.FindChild("Black Slide").gameObject;

        m_ProgressbarObj = childObj.GetComponent<UISlider>();
    }

    /// <summary>
    /// 다운로드 상태 아이콘 변경
    /// </summary>
    public void ChangeDownloadStatusIcon(GameObject obj)
    {
        parentIconObj = obj.transform.FindChild("Download Button").gameObject;

        UISprite downloadIcon = parentIconObj.GetComponent<UISprite>();
        TweenRotation iconRotation = parentIconObj.GetComponent<TweenRotation>();

        if (downloadIcon.spriteName == "icon_download")
        {
            downloadIcon.spriteName = "icon_downloading";
            iconRotation.PlayForward();
        }
        else
        {
            downloadIcon.spriteName = "icon_downloaded";
            iconRotation.ResetToBeginning();
            iconRotation.enabled = false;
        }
    }

    /// <summary>
    /// 다운로드 취소 시 다운로드 상태 아이콘을 초기화 합니다.
    /// </summary>    
    public void InitDownloadStatusIcon()
    {
        if (parentIconObj == null)
        {
            return;
        }

        UISprite downloadIcon = parentIconObj.GetComponent<UISprite>();
        TweenRotation iconRotation = parentIconObj.GetComponent<TweenRotation>();

        downloadIcon.spriteName = "icon_download";

        iconRotation.ResetToBeginning();
        iconRotation.enabled = false;
    }
}