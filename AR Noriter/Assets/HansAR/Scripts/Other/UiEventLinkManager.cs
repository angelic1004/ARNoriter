using UnityEngine;
using System.Collections;

public class UiEventLinkManager : NatCorderRecordScreenUI
{
    public enum CameraCaptureArea
    {
        none,
        threeD,
        All
    }

    public CameraCaptureArea CaptureMode;

    public GameObject RotationCenterBtnObj;
    public GameObject RotationCenterSpriteObj;

    public Camera HudCam;

    public GameObject rotateUI;

    public GameObject blindObj;

    public static UiEventLinkManager uieventmanager;

    void Start()
    {
        if (RotateUI.회전.회전UI != null)
        {
            RotateUI.회전.회전UI_숨기기();
        }

        if (rotateUI != null)
        {
            RotateUI.회전.회전UI = rotateUI;
            RotateUI.회전.회전UI_숨기기();
        }

        CameraDepthInit();
    }

    void Update()
    {

    }

    public void HudCamAreaSet()
    {
        if (CaptureMode == CameraCaptureArea.All)
        {
            HudCamAllView();
        }
        else
        {
            HudCame3dView();
        }
    }

    public void HudCamAreaSet(CameraCaptureArea state)
    {
        CaptureMode = state;

        if (CaptureMode == CameraCaptureArea.All)
        {
            HudCamAllView();
        }
        else
        {
            HudCame3dView();
        }
    }


    private void CameraDepthInit()
    {
        if (Camera.main != null)
        {
            Camera.main.depth = 1;
        }
        UICamera.mainCamera.depth = 4;

        if (MainUI.메인.uiEventLinkManager != null)
        {
            if (MainUI.메인.uiEventLinkManager.GetComponent<UiEventLinkManager>().HudCam != null)
            {
                MainUI.메인.uiEventLinkManager.GetComponent<UiEventLinkManager>().HudCam.depth = 3;
            }
        }
        else
        {
            Debug.Log("MainUI.메인.uiEventLinkManager 부분이 null 입니다.");
        }

        HudCamAreaSet();
    }


    public void HudCamAllView()
    {
        if (HudCam != null)
        {
            _statusLabel.SetActive(false);
            HudCam.depth = 5;
        }
        else
        {
            Debug.Log("UiEventLinkManager에 HudCam 부분이 null 입니다.");
        }
    }

    public void HudCame3dView()
    {
        if (HudCam != null)
        {
            _statusLabel.SetActive(true);
            HudCam.depth = 3;
        }
        else
        {
            Debug.Log("UiEventLinkManager에 HudCam 부분이 null 입니다.");
        }
    }

    public void AudioOn()
    {
        AudioListener.pause = false;
    }

    public void AudioOff()
    {
        AudioListener.pause = true;
    }


    public void 비활성화_함수호출_Link()
    {
        if (TargetManager.타깃메니저.usedSelfiMode)
        {
            MainUI.메인.비활성화_함수호출();

            if (TouchEventManager.터치.기준콜라이더 != null)
            {
                TouchEventManager.터치.기준콜라이더.GetComponent<BoxCollider>().enabled = false;
                TargetManager.EnableTracking = false;
            }
        }
    }

    public void BlindRecordStart()
    {
        if (blindObj != null)
        {
            blindObj.GetComponent<UIWidget>().alpha = 0.4f;
            blindObj.GetComponent<BoxCollider2D>().enabled = true;
        }
        else
        {
            if (TweenManager.tween_Manager.screenShotBtn != null)
            {
                TweenManager.tween_Manager.screenShotBtn.GetComponent<UIWidget>().alpha = 0.4f;
                TweenManager.tween_Manager.screenShotBtn.GetComponent<BoxCollider2D>().enabled = false;
            }
        }
    }

    public void BlindRecordStop()
    {
        if (blindObj != null)
        {
            blindObj.GetComponent<UIWidget>().alpha = 0;
            blindObj.GetComponent<BoxCollider2D>().enabled = false;
        }
        else
        {
            if (TweenManager.tween_Manager.screenShotBtn != null)
            {
                TweenManager.tween_Manager.screenShotBtn.GetComponent<UIWidget>().alpha = 1;
                TweenManager.tween_Manager.screenShotBtn.GetComponent<BoxCollider2D>().enabled = true;
            }
        }
    }

    public void ScreenShot_Save_Link()
    {
        if (CaptureMode == CameraCaptureArea.All)
        {
            ShotManager.shotmanager.ScreenShot_AllSave();
        }
        else
        {
            ShotManager.shotmanager.ScreenShot_Save();
        }
    }

    public void ScreenShotAll_Save_Link()
    {
        ShotManager.shotmanager.ScreenShot_AllSave();
    }

    public void 카메라변경_Link()
    {
        MainUI.메인.카메라변경();
    }

    public void OnPlayPreviewClick_Link()
    {
        TargetManager.타깃메니저.OnPlayPreviewClick();
    }

    public void 컨텐츠_회전_왼쪽_Link()
    {
        콜라이더_비활성화_Link();
        RotateUI.회전.컨텐츠_회전_왼쪽();
    }

    public void 컨텐츠_회전_위_Link()
    {
        콜라이더_비활성화_Link();
        RotateUI.회전.컨텐츠_회전_위();
    }

    public void 컨텐츠_회전_오른쪽_Link()
    {
        콜라이더_비활성화_Link();
        RotateUI.회전.컨텐츠_회전_오른쪽();
    }

    public void 컨텐츠_회전_아래_Link()
    {
        콜라이더_비활성화_Link();
        RotateUI.회전.컨텐츠_회전_아래();
    }

    public void 컨텐츠_회전_중지_Link()
    {
        콜라이더_활성화_Link();
        RotateUI.회전.컨텐츠_회전_중지();
    }

    public void 컨텐츠_회전_초기화_Link()
    {
        콜라이더_활성화_Link();
        RotateUI.회전.컨텐츠_회전_초기화();
    }

    public void AnimationOne()
    {
        AnimationManager.애니메이션.애니메이션01_재생();
    }

    public void AnimationTwo()
    {
        AnimationManager.애니메이션.애니메이션02_재생();
    }

    public void AnimationThree()
    {
        AnimationManager.애니메이션.애니메이션03_재생();
    }

    public void AnimationFour()
    {
        AnimationManager.애니메이션.애니메이션04_재생();
    }

    public void 콜라이더_활성화_Link()
    {
        if (TargetManager.타깃메니저.usedSelfiMode)
        {
            RotateUI.회전.콜라이더_활성화();
            TargetManager.EnableTracking = true;
        }
    }

    public void 콜라이더_비활성화_Link()
    {
        if (TargetManager.타깃메니저.usedSelfiMode)
        {
            if (TouchEventManager.터치.기준콜라이더 != null)
            {
                TouchEventManager.터치.기준콜라이더.GetComponent<BoxCollider>().enabled = false;
                TargetManager.EnableTracking = false;
            }
        }
    }

    public void OnFinishedRotationMotion()
    {
        if (RotationCenterBtnObj == null || RotationCenterSpriteObj == null)
        {
            return;
        }

        if (RotationCenterBtnObj.transform.localScale.x < 1f)
        {
            RotationCenterSpriteObj.GetComponent<UISprite>().spriteName = "ruddercenter_btn";
        }
        else
        {
            RotationCenterSpriteObj.GetComponent<UISprite>().spriteName = "rudder_btn";
        }
    }
}
