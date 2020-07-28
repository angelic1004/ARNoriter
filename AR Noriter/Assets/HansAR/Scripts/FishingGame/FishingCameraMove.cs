using UnityEngine;
using System.Collections;

public class FishingCameraMove : MonoBehaviour
{
    public Camera targetCam;
    public GameObject targetObj;

    private Vector3 savedFirtPos;
    private Vector3 savedFirtRot;
    private Vector3 savedFirtScale;

    public Vector3 camStartPos;
    public float camStartMoveTime;
    public float shakeTime = 0.2f;
    private float shake = 0f;

    public float shakeAmount = 0.002f;
    public float decreaseFactor = 1.0f;
    public bool shake_State = false;

    private Coroutine startCamMoveCor;
    private Coroutine shakeCor;

    public static FishingCameraMove instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        savedFirtPos = targetCam.transform.localPosition;
        savedFirtRot = targetCam.transform.localEulerAngles;
        savedFirtScale = targetCam.transform.localScale;
    }


    void Update()
    {

    }

    private void InitCamTranform()
    {
        targetCam.transform.localPosition = savedFirtPos;
        targetCam.transform.localEulerAngles = savedFirtRot;
        targetCam.transform.localScale = savedFirtScale;
    }

    public void StartCamMove()
    {
        StartCamMoveCorStart();
    }

    private void StartCamMoveCorStart()
    {
        if(FishingGameManager.instance.tamiAnimator.gameObject != null)
        {
            targetObj = FishingGameManager.instance.tamiAnimator.gameObject;
        }
        StartCamMoveCorStop();
        startCamMoveCor = StartCoroutine(StartCamMoveCall());
    }


    private void StartCamMoveCorStop()
    {
        if(startCamMoveCor != null)
        {
            StopCoroutine(startCamMoveCor);
            startCamMoveCor = null;
        }
    }

    private IEnumerator StartCamMoveCall()
    {

      //  yield return new WaitForEndOfFrame();
        targetCam.transform.LookAt(targetObj.transform);
        targetCam.transform.localPosition = camStartPos;// new Vector3(-1.2f,0.8f,1.6f);
        targetCam.transform.LookAt(targetObj.transform);
        yield return new WaitForSeconds(0.5f);
        TweenManager.tween_Manager.TweenAllDestroy(targetCam.gameObject);
        TweenManager.tween_Manager.AddTweenPosition(targetCam.gameObject,
                                                    targetCam.transform.localPosition,
                                                    savedFirtPos,
                                                    camStartMoveTime);
        

        TweenManager.tween_Manager.AddTweenRotation(targetCam.gameObject,
                                                    targetCam.transform.localEulerAngles,
                                                    savedFirtRot,
                                                    camStartMoveTime);

        TweenManager.tween_Manager.TweenPosition(targetCam.gameObject);
        TweenManager.tween_Manager.TweenRotation(targetCam.gameObject);

        yield break;
    }


    public void ShakeCamStart()
    {
        shake = shakeTime;
        shake_State = true;

        ShakeCamCorStart();
    }

    public void ShakeCamPowerSetStart(float shake_power)
    {
        shake = shake_power;
        shake_State = true;
        ShakeCamCorStart();
    }

    private void ShakeCamCorStart()
    {
        ShakeCamCorStop();
        shakeCor = StartCoroutine(ShakeCorCall());
    }

    private void ShakeCamCorStop()
    {
        if (shakeCor != null)
        {
            StopCoroutine(shakeCor);
            shakeCor = null;
        }
    }

    private IEnumerator ShakeCorCall()
    {
        while(true)
        {
            yield return new WaitForEndOfFrame();
            if (shake_State)
            {
                if (shake > 0)
                {
                 //   savedFirtPos = targetCam.transform.localPosition;
                    targetCam.transform.localPosition = savedFirtPos + Random.insideUnitSphere * shakeAmount;

                    shake -= Time.deltaTime * decreaseFactor;
                }
                else
                {
                    shake = 0f;
                    targetCam.transform.localPosition = savedFirtPos;
                    shake_State = false;
                }
            }

        }
    }

}
