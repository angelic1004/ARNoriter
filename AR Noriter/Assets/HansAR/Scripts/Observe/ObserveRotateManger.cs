using UnityEngine;
using System;
using System.Collections;
using HedgehogTeam.EasyTouch;

public class ObserveRotateManger : MonoBehaviour
{
    public static ObserveRotateManger instance;

    public bool rotateSet = true;

    public GameObject 기준콜라이더;

    [Serializable]
    public class CameraCullingMask
    {
        public string[] arCamCog;
        public string[] arCamNonCog;
        public string[] RotCamCog;
        public string[] RotCamNonCog;
    }

    [SerializeField]
    public CameraCullingMask cullingSet;

    public Transform contentsRoot;

    public Vector3 contentsPos;
    public Vector3 contentsRot;
    public Vector3 contentsScale;

    public Vector3 rotCamPos;

    public GameObject centerObj;
    public Camera rotCam;

    public float dragSensitivity = 1.8f;

    public float 확대축소감도 = 2.0f;
    public float 회전감도 = 1.0f;

    public float 비인식후_최소크기;
    public float 비인식후_최대크기;

    private bool 모델링클릭 = false;

    private float 위치거리차이x;
    private float 위치거리차이y;

    private Vector3 이동좌표저장;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (모델링클릭)
        {
            위치거리차이x = Mathf.Abs(이동좌표저장.x - Input.mousePosition.x);
            위치거리차이y = Mathf.Abs(이동좌표저장.y - Input.mousePosition.y);

            if (위치거리차이x > 0)
            {
                for (int i = 0; i < 위치거리차이x / 4; i++)
                {
                    if (Mathf.Abs(이동좌표저장.x - Input.mousePosition.x) > 1)
                    {
                        if ((이동좌표저장.x - Input.mousePosition.x) > 0)
                        {
                            if (MainUI.방향상태 == 0 || MainUI.방향상태 == 2)
                            {
                                centerObj.transform.Rotate(Vector3.down * 회전감도 * Time.deltaTime);
                                //centerObj.transform.RotateAround(centerObj.transform.position, Vector3.down, 회전감도);
                            }
                            else if (MainUI.방향상태 == 1)//|| MainUI.방향상태 == 2)
                            {
                                centerObj.transform.Rotate(Vector3.down * 회전감도 * Time.deltaTime);
                                //centerObj.transform.RotateAround(centerObj.transform.position, Vector3.down, 회전감도);
                            }
                        }
                        else
                        {
                            if (MainUI.방향상태 == 0 || MainUI.방향상태 == 2)
                            {
                                centerObj.transform.Rotate(Vector3.up * 회전감도 * Time.deltaTime);
                               // centerObj.transform.RotateAround(centerObj.transform.position, Vector3.up, 회전감도);
                            }
                            //      전면가로,                전면세로
                            else if (MainUI.방향상태 == 1)//|| MainUI.방향상태 == 2)
                            {
                                centerObj.transform.Rotate(Vector3.up * 회전감도 * Time.deltaTime);
                                //centerObj.transform.RotateAround(centerObj.transform.position, Vector3.up, 회전감도);
                            }
                        }
                    }
                }
            }

            if (위치거리차이y > 0)
            {
                for (int i = 0; i < 위치거리차이y / 4; i++)
                {
                    if (Mathf.Abs(이동좌표저장.y - Input.mousePosition.y) > 1)
                    {
                        if ((이동좌표저장.y - Input.mousePosition.y) > 0)
                        {
                            if (MainUI.방향상태 == 0 || MainUI.방향상태 == 2)
                            {
                                centerObj.transform.Rotate(Vector3.right * 회전감도 * Time.deltaTime);
                                //centerObj.transform.RotateAround(centerObj.transform.position, Vector3.right, 회전감도);
                            }
                            else if (MainUI.방향상태 == 1)
                            {
                                centerObj.transform.Rotate(Vector3.right * 회전감도 * Time.deltaTime);
                               // centerObj.transform.RotateAround(centerObj.transform.position, Vector3.left, 회전감도);
                            }

                        }
                        else
                        {
                            if (MainUI.방향상태 == 0 || MainUI.방향상태 == 2)
                            {
                                centerObj.transform.Rotate(Vector3.left * 회전감도 * Time.deltaTime);
                               // centerObj.transform.RotateAround(centerObj.transform.position, Vector3.left, 회전감도);
                            }
                            else if (MainUI.방향상태 == 1)
                            {
                                centerObj.transform.Rotate(Vector3.left * 회전감도 * Time.deltaTime);
                               // centerObj.transform.RotateAround(centerObj.transform.position, Vector3.right, 회전감도);
                            }
                        }
                    }
                }
            }

            이동좌표저장 = Input.mousePosition;
        }
    }

    void OnEnable()
    {

        EasyTouch.On_Drag += EasyTouch_On_Drag;
        EasyTouch.On_TouchStart += EasyTouch_On_TouchStart;
        EasyTouch.On_TouchUp += EasyTouch_On_TouchUp;
    }

    void OnDisable()
    {
        UnsubscribeEvent();
    }

    void OnDestroy()
    {
        UnsubscribeEvent();
    }

    void UnsubscribeEvent()
    { 

        EasyTouch.On_Drag -= EasyTouch_On_Drag;
        EasyTouch.On_TouchStart -= EasyTouch_On_TouchStart;
        EasyTouch.On_TouchUp -= EasyTouch_On_TouchUp;
    }


    public void CognitiveCam()
    {
        Camera.main.cullingMask = LayerMask.GetMask(cullingSet.arCamCog);
        rotCam.cullingMask = LayerMask.GetMask(cullingSet.RotCamCog);

        TargetManager.타깃메니저.모델링오브젝트.transform.localScale = Vector3.one;
    }

    public void NonCognitiveCam()
    {
        Camera.main.cullingMask = LayerMask.GetMask(cullingSet.arCamNonCog);
        rotCam.cullingMask = LayerMask.GetMask(cullingSet.RotCamNonCog);
        rotCam.fieldOfView = Camera.main.fieldOfView;
        rotCam.nearClipPlane = Camera.main.nearClipPlane;
        rotCam.farClipPlane = Camera.main.farClipPlane;

        TargetManager.타깃메니저.컨텐츠최상위오브젝트.transform.parent = contentsRoot;

        CenterObjTrInit();
    }

    public void CenterObjTrInit()
    {
      //  centerObj.transform.localScale = contentsScale;

        centerObj.transform.parent = TargetManager.타깃메니저.컨텐츠최상위오브젝트.transform;

        centerObj.transform.localPosition = contentsPos;
        centerObj.transform.localEulerAngles = contentsRot;
        
        rotCam.transform.localPosition = rotCamPos;
        TargetManager.타깃메니저.모델링오브젝트.transform.localScale = Vector3.one;

        RotateUI.회전.컨텐츠_회전_초기화();
    }

    public void CollderDisable()
    {
        기준콜라이더 = null;
        TargetManager.EnableTracking = false;
    }

    public void CollderEnable()
    {
        기준콜라이더 = TouchEventManager.터치.지형2D;
        TargetManager.EnableTracking = true;
    }

    public void RotateModeClick()
    {
        rotateSet = true;
        기준콜라이더 = TouchEventManager.터치.지형2D;
    }

    public void MoveModeClick()
    {
        rotateSet = false;
        기준콜라이더 = TouchEventManager.터치.지형2D;
    }

    private void EasyTouch_On_TouchStart(Gesture gesture)
    {
        if (TouchEventManager.터치.지형2D == gesture.pickedObject && gesture.touchCount == 1)
        {
            if (기준콜라이더 != null)
            {
                if (rotateSet)
                {
                    이동좌표저장 = Input.mousePosition;
                    모델링클릭 = true;
                }
            }
        }
    }

    private void EasyTouch_On_TouchUp(Gesture 제스처)
    {
        if (기준콜라이더 != null)
        {
            모델링클릭 = false;
        }
    }

    private void EasyTouch_On_Drag(Gesture gesture)
    {
        try
        {
            if (gesture.pickedObject == null)
            {
                return;
            }

            // 드래그 이동 (카메라 이동 방식)
            if (!rotateSet)
            {
                if (기준콜라이더 != null)
                {
                    if (TouchEventManager.터치.지형2D == gesture.pickedObject && gesture.touchCount == 1)
                    {
                        Vector3 dragCamPos = new Vector3(gesture.deltaPosition.x * (dragSensitivity / 100), gesture.deltaPosition.y * (dragSensitivity / 100), 0);
                        rotCam.transform.localPosition -= dragCamPos;
                    }
                }
            }
        }
        catch
        {

        }
    }



}
