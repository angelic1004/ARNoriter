using UnityEngine;
using System.Collections;

public class LetterComponentManager : MonoBehaviour
{
    public static LetterComponentManager instance;

    public MediaPlayerCtrlBase mediaPlayerCtrlInst;

    public GameObject videoPlayerRootObj;
    //public GameObject videoPlayerObj;

    public GameObject textLabelRootObj;
            

    void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddVideoObject(GameObject parentObj)
    {
        if (parentObj == null)
        {
            return;
        }

        if (videoPlayerRootObj == null)
        {
            return;
        }

        videoPlayerRootObj.transform.parent = parentObj.transform;

        videoPlayerRootObj.transform.localPosition = Vector3.zero;
        videoPlayerRootObj.transform.localEulerAngles = new Vector3(0f, 180f, 0f);

        if (TargetManager.타깃메니저.스케치씬사용)
        {
            videoPlayerRootObj.transform.localScale = Vector3.one;
        }
        else
        {
            videoPlayerRootObj.transform.localScale = new Vector3(2, 2, 2);
        }

        if (videoPlayerRootObj.activeSelf)
        {
            videoPlayerRootObj.SetActive(false);
        }
    }

    public void SetActiveVideoPlayer(bool isActive)
    {
        if (videoPlayerRootObj == null)
        {
            return;
        }

        videoPlayerRootObj.SetActive(isActive);
    }

    public void PlayLetterVideo(string videoPath, string audioPath)
    {
        if (mediaPlayerCtrlInst == null)
        {
            return;
        }

        if (!videoPlayerRootObj.activeSelf)
        {
            mediaPlayerCtrlInst.gameObject.SetActive(true);
        }

        mediaPlayerCtrlInst.Load(videoPath, audioPath, false, true);
    }

    public void SetVideoFilePath(string path)
    {

    }

    /// <summary>
    /// 편지지의 열림/닫힘 상태에 따라 비디오 플레이어의 상태를 변경 함
    /// 열림 : 비디오 블레이 재생 및 오브젝트 활성화
    /// 닫힘 : 비디오 플레이 멈춤 및 오브젝트 비활성화
    /// </summary>
    /// <param name="letterFaceType">컨텐츠 타입</param>
    /// <param name="isLetterOpened">편지지 상태</param>
    public void ChangeStatusVideoPlayer(LetterManager.LetterFaceType letterFaceType, bool isLetterOpened)
    {

    }

    public void UnloadVideoPlayer()
    {
        if (videoPlayerRootObj.activeSelf)
        {
            mediaPlayerCtrlInst.Unload();
            mediaPlayerCtrlInst.RemoveStrPath();
            SetActiveVideoPlayer(false);
        }
    }

    // Text Label 
    public void AddTextLabelObject(GameObject parentObj)
    {
        if (textLabelRootObj == null)
        {
            return;
        }

        textLabelRootObj.transform.parent           = parentObj.transform;

        textLabelRootObj.transform.localPosition    = Vector3.zero;
        textLabelRootObj.transform.localEulerAngles = Vector3.zero;
        textLabelRootObj.transform.localScale       = new Vector3(1f, 1f, 1f);

        /*
        if (textLabelRootObj.activeSelf)
        {
            textLabelRootObj.SetActive(false);
        }
        */
    }    


}
