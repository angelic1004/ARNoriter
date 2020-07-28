using UnityEngine;
using System.Collections;

using HansAR;

//public class HMSlideDropAssetDownloader : AssetBundleDownloader
public class HMSlideDropAssetDownloader : DownloadManager
{
    void Start ()
    {
	    
	}

    public override void OnDrawProgressValue(HttpRequestDataSet downloadParam, float progressValue)
    {
        if (FileDownloaderUI.getInstance.GetUseProgressBarValue())
        {
            if (FileDownloaderUI.getInstance.m_ProgressbarObj.gameObject.activeSelf == false)
            {
                FileDownloaderUI.getInstance.m_ProgressbarObj.gameObject.SetActive(true);
            }

            receiveFileSize = downloadCompleteSize + (progressValue * downloadParam.requestFileSize);           
            FileDownloaderUI.getInstance.m_UpdateText.text = string.Format(LocalizeText.Value["DownloadProgress_F"], receiveFileSize.ToString("N2"), downloadAssetTotalSize.ToString("N2"));

            /////////////////// 수정부분
            // 슬라이더 값이 1에서 0으로 작아지도록 함
            FileDownloaderUI.getInstance.m_ProgressbarObj.value = 1 - (receiveFileSize / downloadAssetTotalSize);
            ///////////////////
        }
    }
}