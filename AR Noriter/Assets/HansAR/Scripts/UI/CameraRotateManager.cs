using UnityEngine;
using System.Collections;
using HedgehogTeam.EasyTouch;

public class CameraRotateManager : MonoBehaviour
{
    public static CameraRotateManager Instance;

    public Camera rotateCam;                        // 회전 카메라
    public GameObject centerObj;

    public float moveSpeed = 1.0f;
    public float zoomInOutMoveSpeed = 0f;

    //public float defaultWidthSize = 1920f;
    private float defaultWidthSize;

    private bool isMoving = false;
    private Vector3 rotateDirection;                // 회전 방향

    public Vector3 zoomOutPos;
    public Vector3 zoomInPos;
    private Vector3 deltaMove;

    private GameObject character;
    public Vector3 characterZoomOutPos;

    public Vector3 centerObjRot = new Vector3(30, 0, 0);

    private Vector3 dragStartPos;
    public float dragSensitivity = 3;                   // 드래그 감도 (기본값 3)

    public bool isCameraMoving = false;

    private bool isMiniMapScaleUp;


    void Awake()
    {
        Instance = this;

        defaultWidthSize = Screen.width;
        //Invoke("InitRotateCam", 1f);
        StartCoroutine(InitRotateCam());
    }

	void FixedUpdate()
    {
        // TODO : 이 부분 코루틴으로 변경할 것 (확대시 : 코루틴 실행, 축소시 : 코루틴 종료 하도록)
        if(isMoving)
        {
            centerObj.transform.Rotate(rotateDirection, moveSpeed);
        }
	}

    void OnEnable()
    {
        EasyTouch.On_Drag += EasyTouch_On_Drag;
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
    }

    private IEnumerator InitRotateCam()
    {
        GameObject minimapModeling = null;
        bool loadedMinimap = false;

        while (loadedMinimap == false)
        {
            yield return new WaitForSeconds(0.3f);

            try
            {
                if (MiniMapManager.instance.miniMapModeling != null)
                {
                    loadedMinimap = true;
                }
            }
            catch
            {
                //Debug.Log("MiniMap 로딩 안됨");
            }
        }

        // RotateCam X좌표 값 수정 적용
        float destPosX = 0f;
        float destPosY = 0f;
        float destPosZ = 0f;

        destPosX = GetChangeDestValue(zoomOutPos.x, false);
        destPosY = GetChangeDestValue(zoomOutPos.y, true);
        destPosZ = GetChangeDestValue(zoomOutPos.z, true);

        rotateCam.transform.localPosition = new Vector3(destPosX, destPosY, destPosZ);        

        minimapModeling = MiniMapManager.instance.miniMapModeling;
        minimapModeling.transform.localScale = new Vector3(1, 1, 1);
        minimapModeling.transform.Rotate(new Vector3(-90, 0, 0), Space.Self);

        ChangeLayerName(minimapModeling, "Minimap");

        yield return new WaitForEndOfFrame();
    }

    private float GetChangeDestValue(float originValue, bool isContrary)
    {
        float increaseValue = 0f;
        float destValue = 0f;

        /*
        float ratioValue = 0f;
        float middleValue = 0f;

        ratioValue = defaultWidthSize / Screen.width;
        middleValue = Mathf.Abs(originValue) * ratioValue;
        */

        increaseValue = (Mathf.Abs(originValue) - (Mathf.Abs(originValue) * (defaultWidthSize / (float)Screen.width))) * 2;
        //increaseValue = (Mathf.Abs(originValue) - (Mathf.Abs(originValue) * (defaultWidthSize / 2048))) * 2;

        if (originValue < 0f)
        {
            if (isContrary)
            {
                destValue = originValue - increaseValue;
            }
            else
            {
                destValue = originValue + increaseValue;
            }
        }
        else
        {
            if (isContrary)
            {
                destValue = originValue + increaseValue;
            }
            else
            {
                destValue = originValue - increaseValue;
            }            
        }

        return destValue;
    }

    public void StopCamMove()
    {
        isMoving = false;
        rotateDirection = Vector3.zero;
    }

    public void MoveCamLeft()
    {
        rotateDirection = Vector3.down;
        isMoving = true;
    }

    public void MoveCamRight()
    {
        rotateDirection = Vector3.up;
        isMoving = true;
    }

    public void MoveCameUp()
    {
        rotateDirection = Vector3.left;
        isMoving = true;
    }

    public void MoveCameDown()
    {
        rotateDirection = Vector3.right;
        isMoving = true;
    }

    /// <summary>
    /// 캠 위치를 리셋합니다.
    /// </summary>
    public void ResetCamPosition()
    {
        rotateDirection = Vector3.zero;
        //centerObj.transform.rotation = Quaternion.identity;

        centerObj.transform.localRotation = Quaternion.Euler(centerObjRot);
        rotateCam.transform.localPosition = zoomInPos;
    }

    private void MoveCenterObj(Vector3 direction)
    {
        centerObj.transform.Rotate(direction, moveSpeed);
    }

    /// <summary>
    /// 오브젝트의 레이어 이름을 변경합니다.
    /// </summary>
    /// <param name="obj">해당 오브젝트</param>
    /// <param name="layerName">바꿀 Layer 이름</param>
    public void ChangeLayerName(GameObject obj, string layerName)
    {
        Transform[] tran = obj.GetComponentsInChildren<Transform>(true);

        // 하위 레이어까지 전부 변경
        foreach (Transform child in tran)
        {
            child.gameObject.layer = LayerMask.NameToLayer(layerName);
        }
    }

    /// <summary>
    /// RotateCam 켜고 끄기
    /// </summary>
    public void OnOffRotateCam(bool turnOnCam)
    {
        if(turnOnCam)
        {
            rotateCam.gameObject.SetActive(true);
        }
        else
        {
            rotateCam.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Rotate Cam을 ZoomIn 합니다. (오브젝트 확대)
    /// </summary>
    public IEnumerator ZoomInRotateCam()
    {
        float time = zoomInOutMoveSpeed;

        isCameraMoving = true;

        while (rotateCam.transform.localPosition.x < zoomInPos.x)
        {
            rotateCam.transform.localPosition = Vector3.MoveTowards(rotateCam.transform.localPosition, zoomInPos, Time.fixedDeltaTime * time);
            yield return new WaitForFixedUpdate();
        }

        isCameraMoving = false;
        MiniMapManager.instance.ChangeMiniMapScaleControlBtn(false);
        isMiniMapScaleUp = true;
    }

    /// <summary>
    /// Rotate Cam을 ZoomOut 합니다. (오브젝트 축소)
    /// </summary>
    public IEnumerator ZoomOutRotateCam()
    {
        float time = zoomInOutMoveSpeed;
        ResetCamPosition();
        isCameraMoving = true;

        Vector3 destZoomPos = Vector3.zero;
        destZoomPos = new Vector3(GetChangeDestValue(zoomOutPos.x, false), GetChangeDestValue(zoomOutPos.y, true), GetChangeDestValue(zoomOutPos.z, true));

        while (rotateCam.transform.localPosition.x > GetChangeDestValue(zoomOutPos.x, false))
        {
            rotateCam.transform.localPosition = Vector3.MoveTowards(rotateCam.transform.localPosition, destZoomPos, Time.fixedDeltaTime * time);
            yield return new WaitForFixedUpdate();
        }

        isCameraMoving = false;

        MiniMapManager.instance.ChangeMiniMapScaleControlBtn(true);
        isMiniMapScaleUp = false;
    }

    /// <summary>
    /// 캐릭터를 ZoomOut 합니다. (오브젝트 축소)
    /// </summary>
    public IEnumerator ZoomOutCharacter(GameObject character)
    {
        Vector3 savePos = character.transform.localPosition;

        float time = 20f;

        while (character.transform.localPosition.z < characterZoomOutPos.z)

        {
            character.transform.localPosition = Vector3.MoveTowards(character.transform.localPosition, characterZoomOutPos, Time.fixedDeltaTime * time);
            yield return new WaitForFixedUpdate();
        }

        character.SetActive(false);
        character.transform.localPosition = savePos;
    }

    private void EasyTouch_On_Drag(Gesture gesture)
    {
        try
        {
            if (gesture.pickedObject == null)
            {
                return;
            }

            // 미니맵 드래그 이동 (카메라 회전 방식)
            if (isMiniMapScaleUp && gesture.pickedObject.transform.parent.name.Equals(MiniMapManager.instance.GetMiniMapModelName()))
            {
                Vector3 dragCamPos = new Vector3(gesture.deltaPosition.x * (dragSensitivity / 100), gesture.deltaPosition.y * (dragSensitivity / 100), 0);
                rotateCam.transform.localPosition -= dragCamPos;
            }
        }
        catch
        {

        }
    }

    /// <summary>
    /// 미니맵 확대 (카메라를 이용해서 확대함)
    /// </summary>
    public void OnClickMiniMapZoomIn()
    {
        float z = rotateCam.transform.localPosition.z;

        if (z < -30)
        {
            rotateCam.transform.Translate(0, 0, 10);
        }
    }

    public void OnClickMiniMapZoomOut()
    {
        float z = rotateCam.transform.localPosition.z;

        if (z > -300)
        {
            rotateCam.transform.Translate(0, 0, -10);
        }
    }
}