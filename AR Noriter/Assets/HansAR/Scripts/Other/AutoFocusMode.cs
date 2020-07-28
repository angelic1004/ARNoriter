using UnityEngine;
using System.Collections;
using Vuforia;

public class AutoFocusMode : Singleton<AutoFocusMode>
{
    private CameraDevice.FocusMode mode = CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO;
    public bool useAutoFocusMode;

    
    void Awake()
    {
        useAutoFocusMode = true;
    }

    void FixedUpdate()
    {
        if (useAutoFocusMode)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                StartCoroutine(SetFocusModeToTriggerAuto());
            }
        }
    }
    

    public void OnOffAutoFucousMode(bool status)
    {
        if(status)
        {
            useAutoFocusMode = true;
        }
        else
        {
            useAutoFocusMode = false;
        }
    }

    private IEnumerator SetFocusModeToTriggerAuto()
    {
        if (CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_TRIGGERAUTO))
        {
            mode = CameraDevice.FocusMode.FOCUS_MODE_TRIGGERAUTO;
        }

        //Debug.Log("Focus Mode Changed To " + mode);

        yield return new WaitForSeconds(1.0f);

        SetFocusModeToContinuousAuto();
    }

    private void SetFocusModeToContinuousAuto()
    {
        if (CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO))
        {
            mode = CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO;
        }

        //Debug.Log("Focus Mode Changed To " + mode);
    }
}
