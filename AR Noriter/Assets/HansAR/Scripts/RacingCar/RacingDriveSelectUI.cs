using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RacingDriveSelectUI : MonoBehaviour
{
    #region Public Variable

    /// <summary>
    /// 싱글모드 배틀모드 선택 UI
    /// </summary>
    public GameObject modeSelectUI;

    public GameObject trackSelectUI;

    #endregion

    private RacingDrive driveData;

    public static RacingDriveSelectUI instance;

    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        driveData = RacingDrive.instance;

        modeSelectUI.SetActive(false);
        trackSelectUI.SetActive(false);
    }

    #region UI 초기화 관련

    /// <summary>
    /// 레이싱 운전하기 UI 상태 변경 메서드
    /// </summary>
    /// <param name="value">마커 인식 상태</param>
    public void RacingDriveUISetActive(bool value)
    {
        if (value)
        {
            //메인 UI를 다시 켜줌
            MainUI.메인.UIOpen(MainUI.메인.uiEventLinkManager);

            modeSelectUI.SetActive(false);
            trackSelectUI.SetActive(false);
        }
        else
        {
            //UI 상태 변경
            modeSelectUI.SetActive(false);
            trackSelectUI.SetActive(false);
        }
    }

    #endregion

    #region UI 제어 관련

    /// <summary>
    /// 게임 선택 UI 켜줌
    /// </summary>
    public void GameSelectUIOpen()
    {
        TargetManager.타깃메니저.HideAllModelingContents();

        //UI 상태변경
        RotateUI.회전.회전UI_숨기기();
        MainUI.메인.UIClose(MainUI.메인.uiEventLinkManager);
        MainUI.메인.인식글자UI.SetActive(false);

        modeSelectUI.SetActive(true);
    }
    
    public void TrackSelectUIOpen()
    {
        modeSelectUI.SetActive(false);
        trackSelectUI.SetActive(true);
    }
    
    public void SelectUIClose()
    {
        modeSelectUI.SetActive(false);
        trackSelectUI.SetActive(false);
    }
    #endregion

    #region 버튼 클릭 관련

    /// <summary>
    /// 싱글모드 버튼 클릭
    /// </summary>
    public void SingleModeBtnClick()
    {
        //싱글모드로 설정후 초기화 시작
        RacingDrive.battleMode = false;
        //driveData.DriveInit();
        TrackSelectUIOpen();
    }

    /// <summary>
    /// 배틀모드 버튼 클릭
    /// </summary>
    public void BattleModeBtnClick()
    {
        //배틀모드로 설정후 초기화 시작
        RacingDrive.battleMode = true;
        //driveData.DriveInit();
        TrackSelectUIOpen();
    }

    public void TrackSelectBtnClick(GameObject obj)
    {
        RacingDrive.trackObjName = obj.name;

        RacingDrive.myCarObjName = TargetManager.타깃메니저.에셋번들복제컨텐츠[TargetManager.타깃메니저.복제모델링인덱스].name;

        //Debug.Log(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name+"_" + obj.name);

        RacingDrive.gameSelectSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        UnityEngine.SceneManagement.SceneManager.LoadScene(string.Format(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name + "_" + obj.name).ToLower());
    }

    #endregion
}
