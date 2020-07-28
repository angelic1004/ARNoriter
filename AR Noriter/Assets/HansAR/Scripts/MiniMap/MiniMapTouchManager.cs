using UnityEngine;

using System;
using System.Collections;

using HedgehogTeam.EasyTouch;

public class MiniMapTouchManager : MonoBehaviour {
    private Vector3                     touchStartValue;

    public float                        pinchSensitivity;           // 확대 / 축소 민감도 값 (값이 작을 수록 민감함)
    public float                        pinchZoomMin;
    public float                        pinchZoomMax;

    public GameObject                   ColliderObject2D;
    public GameObject                   pinchObject;

    public Camera                       camera2D;

    public static MiniMapTouchManager   instance;

    void Awake ()
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
        // 터치 이벤트 등록
        //EasyTouch.On_PinchIn += EasyTouch_On_PinchIn;
        //EasyTouch.On_PinchOut += EasyTouch_On_PinchOut;
        //EasyTouch.On_SimpleTap += EasyTouch_On_SimpleTap;

        //EasyTouch.On_DoubleTap += EasyTouch_On_DoubleTap;

        EasyTouch.On_TouchStart += EasyTouch_On_TouchStart;
        EasyTouch.On_TouchDown += EasyTouch_On_TouchDown;
    }

    void UnsubscribeEvent()
    {
        // 터치 이벤트 해제
        //EasyTouch.On_PinchIn -= EasyTouch_On_PinchIn;
        //EasyTouch.On_PinchOut -= EasyTouch_On_PinchOut;

        //EasyTouch.On_SimpleTap -= EasyTouch_On_SimpleTap;
        //EasyTouch.On_DoubleTap -= EasyTouch_On_DoubleTap;

        EasyTouch.On_TouchStart -= EasyTouch_On_TouchStart;
        EasyTouch.On_TouchDown -= EasyTouch_On_TouchDown;
    }

    void OnDestroy()
    {
        UnsubscribeEvent();
    }

    void OnDisable()
    {
        UnsubscribeEvent();
    }

    /*
    private void EasyTouch_On_PinchIn(Gesture gesture)
    {
        if (gesture.pickedObject == null)
        {
            return;
        }

        float minValue = 0f;

        if (gesture.touchCount == 2)
        {
            minValue = (Time.deltaTime * (gesture.deltaPinch / pinchSensitivity));
            Vector3 objSize = pinchObject.transform.localScale;

            //Debug.LogWarning(string.Format("deltaTime={0}, deltaPinch={1}, sensitivity={2}, minValue={3}", Time.deltaTime, gesture.deltaPinch, PinchSensitivity, minValue));

            if ((objSize.x - minValue) > pinchZoomMin)
            {
                pinchObject.transform.localScale = new Vector3((objSize.x - minValue), (objSize.y - minValue), (objSize.z - minValue));
            }
        }
    }

    private void EasyTouch_On_PinchOut(Gesture gesture)
    {
        if (gesture.pickedObject == null)
        {
            return;
        }

        float maxValue = 0f;

        if (gesture.touchCount == 2)
        {
            maxValue = (Time.deltaTime * (gesture.deltaPinch / pinchSensitivity));
            Vector3 objSize = pinchObject.transform.localScale;

            if (objSize.x + maxValue < pinchZoomMax)
            {
                pinchObject.transform.localScale = new Vector3((objSize.x + maxValue), (objSize.y + maxValue), (objSize.z + maxValue));
            }
        }
    }
     
    private void EasyTouch_On_SimpleTap(Gesture gesture)
    {
        try
        {
            if (gesture.pickedObject == null)
            {
                //Debug.LogWarning("gesture.pickedObject is null");
                return;
            }
            else
            {
                //Debug.LogWarning(string.Format("gesture.pickedObject name = {0}", gesture.pickedObject.name));
            }

            Ray2D ray2d = new Ray2D(camera2D.ScreenToWorldPoint(gesture.startPosition), Vector2.zero);            
            RaycastHit2D hit2d = Physics2D.Raycast(ray2d.origin, ray2d.direction);

            if (hit2d.collider != null)
            {
                //Debug.LogWarning(string.Format("RaycastHit collider name : {0}", hit2d.collider.name));
                return;
            }
            else
            {
                //Debug.LogWarning("hit2d.collider is null");
                                
                if (MiniMapManager.instance.isCaptureBtnOnMiniMap)
                {
                    MiniMapManager.instance.isCaptureBtnOnMiniMap = false;
                    return;
                }                
            }

            if (gesture.pickedObject.transform.parent.parent.name.Equals(MiniMapManager.instance.GetMiniMapRootObjName()))
            {
                MiniMapManager.instance.MiniMapZoomInOut(gesture.pickedObject.transform.parent.parent.gameObject);
            }
        }
        catch { }
    }

    private void EasyTouch_On_DoubleTap(Gesture gesture)
    {

    }
    */

    private void EasyTouch_On_TouchStart(Gesture gesture)
    {
        try
        {
            if (gesture.pickedObject == null)
            {
                return;
            }

            if (gesture.pickedObject.transform.parent.name.Equals(MiniMapManager.instance.GetMiniMapModelName()))
            {
                Vector3 touchPosition = gesture.GetTouchToWorldPoint(gesture.pickedObject.transform.position);
                touchStartValue = (touchPosition - pinchObject.transform.position);
            }
        }
        catch
        {

        }
    }

    private void EasyTouch_On_TouchDown(Gesture gesture)
    {
        if (gesture.pickedObject == null || MiniMapManager.instance.isMiniMapScaleUp == false)
        {
            return;
        }

        //Debug.LogWarning(string.Format("pickedObject name = {0}, parent ={1}", gesture.pickedObject.name, gesture.pickedObject.transform.parent.name));

        if (gesture.pickedObject.transform.parent.name.Equals(MiniMapManager.instance.GetMiniMapModelName()) && gesture.touchCount == 1)
        {
            Vector3 touchPosition = gesture.GetTouchToWorldPoint(gesture.pickedObject.transform.position);
            pinchObject.transform.position = (touchPosition - touchStartValue);
        }
    }

    public void RequestDisableCollider()
    {
        StartCoroutine(DisableTouchCollider());
    }

    IEnumerator DisableTouchCollider()
    {
        if (pinchObject != null)
        {
            pinchObject.transform.GetChild(0).FindChild("BodyCollider").GetComponent<BoxCollider>().enabled = false;
            yield return new WaitForSeconds(0.01f);            
            pinchObject.transform.GetChild(0).FindChild("BodyCollider").GetComponent<BoxCollider>().enabled = true;
        }
    }
}
