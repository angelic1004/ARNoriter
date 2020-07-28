using UnityEngine;
using System.Collections;
using Vuforia;


public class MiniMapRotateUI : RotateUI
{
    public bool isLeftRightReverse { set; get; }
    public static MiniMapRotateUI instance;

    //public GameObject debugViewObj;

    void Awake()
    {
        instance = this;

        isLeftRightReverse = false;
    }

    void Start()
    {
        CallByStartEvent();
    }

    public void InitMiniMapRotate()
    {
        StartCoroutine(InitContentRotate());
        
    }

    public void SetActiveMiniMapRotateUI(bool isShow)
    {
        if (isShow)
        {
            회전UI_보이기();
        }
        else
        {
            회전UI_숨기기();
        }
    }

    public void OnClickRotateLeft()
    {
        컨텐츠_회전_왼쪽();
    }

    public void OnClickRotateRight()
    {
        컨텐츠_회전_오른쪽();
    }

    public void OnClickRotateUp()
    {
        컨텐츠_회전_위();
    }

    public void OnClickRotateDown()
    {
        컨텐츠_회전_아래();
    }

    public void OnClickRotateStop()
    {
        컨텐츠_회전_중지();
    }

    public void DisableMiniMapCollider()
    {
        MiniMapTouchManager.instance.pinchObject.transform.GetChild(0).FindChild("BodyCollider").GetComponent<BoxCollider>().enabled = false;
    }

    public void EnableMiniMapCollider()
    {
        MiniMapTouchManager.instance.RequestDisableCollider();
    }

    private IEnumerator InitContentRotate()
    {
        yield return new WaitForEndOfFrame();

        상하좌우반전         = false;
        isLeftRightReverse  = false;

        Vector3 비인식후_위치값 = Vector3.zero;
        Vector3 비인식후_회전값 = Vector3.zero;
        Vector3 비인식후_크기값 = Vector3.zero;

        if (MiniMapManager.instance.isMiniMapScaleUp)
        {
            비인식후_위치값 = MiniMapManager.instance.miniMapMaxPos;
            비인식후_회전값 = MiniMapManager.instance.miniMapMaxRotate;
            비인식후_크기값 = MiniMapManager.instance.miniMapMaxScale;
        }
        else
        {
            비인식후_위치값 = MiniMapManager.instance.miniMapPortraitPos;
            비인식후_회전값 = MiniMapManager.instance.miniMapRotate;
            비인식후_크기값 = MiniMapManager.instance.miniMapMinScale;
        }

        Camera camera = TargetManager.타깃메니저.AR카메라.GetComponentInChildren<Camera>();
        Matrix4x4 m = camera.projectionMatrix;

        if (m.m11 < 0f)
        {
            비인식후_회전값.x += 180f;
            비인식후_회전값.y += 180f;

            상하좌우반전 = true;

            //비인식후_위치값.x = -비인식후_위치값.x;
            비인식후_위치값.y = -비인식후_위치값.y;
        }
        
        if (m.m00 < 0f)
        {
            비인식후_위치값.x = -비인식후_위치값.x;            
            isLeftRightReverse = true;
        }

        /*
        if (debugViewObj != null)
        {
            debugViewObj.GetComponent<UILabel>().text = string.Format("위치값 = {0}", 비인식후_위치값);
        }
        */

        컨텐츠오브젝트.transform.localPosition = 비인식후_위치값;
        컨텐츠오브젝트.transform.localEulerAngles = 비인식후_회전값;
        컨텐츠오브젝트.transform.localScale = 비인식후_크기값;

        EnableMiniMapCollider();
        MiniMapManager.instance.InitMiniMapBoxColliderSize();
    }
}