using UnityEngine;
using System.Collections;

using HedgehogTeam.EasyTouch;

public class MiniMapObjectTouchManager : MonoBehaviour
{
    private int modelIndex;                 // 미니맵 모델 인덱스
    public GameObject btnShrink;            // [버튼] 축소

    private bool checkUiTouched;            // UI가 터치되었는지 체크

    private GameObject touchedUI;           // 터치된 UI 오브젝트
    private bool onDragging;                // 드래그 상태 체크
    private bool isZoomInOut;

    private GameObject clickObj;

    Vector2 worldPoint;
    RaycastHit2D hit;

    void Awake()
    {
    }

    void Start()
    {
    }

    void OnEnable()
    {
        EasyTouch.On_TouchUp += TouchUpEvent;
        EasyTouch.On_DragStart += DragStartEvent;
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
        EasyTouch.On_TouchUp -= TouchUpEvent;
        EasyTouch.On_DragStart -= DragStartEvent;
    }

    private void TouchUpEvent(Gesture ges)
    {
        if (MiniMapManager.instance.isMiniMapScaleUp)
        {
            //isZoomInOut = MiniMapManager.instance.CheckDoingZoomInOut();
            isZoomInOut = CameraRotateManager.Instance.isCameraMoving;

            if (!isZoomInOut && !onDragging && ges.pickedObject != null && ges.pickedObject.name != "BodyCollider")
            {
                checkUiTouched = CheckUiTouched(ges);

                clickObj = ges.pickedObject.transform.parent.gameObject;

                if (!checkUiTouched && clickObj != null)
                {
                    Debug.Log("GetMiniMapModelIndex(clickObj) : " + GetMiniMapModelIndex(clickObj));
                    Debug.Log("clickObj : " + clickObj.name);

                    modelIndex = GetMiniMapModelIndex(clickObj);

                    if (MiniMapManager.instance.usedAdditionModel)
                    {
                        MiniMapManager.instance.OnClickPlayStoryAnimation(modelIndex);
                        PointAnimationStart();
                    }
                    else
                    {
                        MiniMapManager.instance.OnClickMiniMapScaleUpDown(btnShrink);
                        // AR 비인식 셋팅 불러오기
                        RecallARSetting();
                    }
                }

                checkUiTouched = false;
            }
        }

        if (onDragging)
        {
            onDragging = false;
        }

        ges = null;
    }

    private void DragStartEvent(Gesture ges)
    {
        if (!onDragging)
        {
            onDragging = true;
        }
    }

    private void PointAnimationStart()
    {
        if(MiniMapManager.instance.usedPointAniamtion)
        {
            PointAnimationStop();

            Animation ani = MiniMapManager.instance.miniMapModeling.GetComponent<MiniMapInfo>().miniMapResourceInfo[modelIndex].miniMapPointAnimationTarget.GetComponent<Animation>();
            string aniName = string.Empty;

            if (ani.GetClipCount() > 0)
            {
                foreach (AnimationState state in ani)
                {
                    aniName = state.clip.name;
                    break;
                }

                ani.clip = ani.GetClip(aniName);
                ani.Play();
            }
        }
    }

    private void PointAnimationStop()
    {
        if (MiniMapManager.instance.usedPointAniamtion)
        {
            Animation ani = MiniMapManager.instance.miniMapModeling.GetComponent<MiniMapInfo>().miniMapResourceInfo[modelIndex].miniMapPointAnimationTarget.GetComponent<Animation>();

            ani.Stop();
        }
    }

    /// <summary>
    /// 2D UI가 터치되었는지 확인합니다.
    /// </summary>
    private bool CheckUiTouched(Gesture ges)
    {
        bool isTouched = false;                 // UI가 터치되었는지 체크

        worldPoint = UICamera.mainCamera.ScreenToWorldPoint(ges.position);
        hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        if (hit.collider != null)
        {
            Debug.LogWarningFormat("collider name = {0}", hit.collider.name);

            if (hit.collider.name != "UI Root")
            {
                isTouched = true;
            }
            else
            {
                isTouched = false;
            }
        }

        return isTouched;
    }

    /// <summary>
    /// 미니맵 모델링 인덱스를 얻어옵니다.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    private int GetMiniMapModelIndex(GameObject obj)
    {
        MiniMapInfo info = MiniMapManager.instance.miniMapModeling.GetComponent<MiniMapInfo>();

        GameObject compareObj;
        int index = 0;

        for (int i = 0; i < info.miniMapResourceInfo.Length; i++)
        {
            for (int j = 0; j < info.miniMapResourceInfo[i].miniMapObjects.Length; j++)
            {
                compareObj = info.miniMapResourceInfo[i].miniMapObjects[j];

                if (obj == compareObj)
                {
                    //index = info.miniMapResourceInfo[i].targetIndex;
                    index = i;
                    break;
                }
            }

            if (index > 0)
            {
                break;
            }
        }        

        return index;
    }

    private void RecallARSetting()
    {
        try
        {
            TargetManager.타깃메니저.복제모델링인덱스 = modelIndex;
            TargetManager.타깃메니저.타깃정보인덱스 = modelIndex + 1;
            TouchEventManager.터치.기준콜라이더 = TargetManager.타깃메니저.에셋번들복제컨텐츠[modelIndex].transform.FindChild("body").gameObject;

            RotateUI.회전.컨텐츠_회전_초기화();
            RotateUI.회전.회전UI_보이기();

            TargetManager.타깃메니저.HideAllModelingContents();
            ColoringManager.컬러링매니저.modelObj.SetActive(true);
            TargetManager.타깃메니저.ShowModelingObj();

            MainUI.메인.애니메이션동작_UI보이기();
            AnimationManager.애니메이션.애니메이션01_재생();
        }
        catch
        {

        }
    }
}