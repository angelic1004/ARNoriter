using UnityEngine;
using Vuforia;

public class ImageMarkerEvent : MonoBehaviour, ITrackableEventHandler
{
    /// <summary>
    /// 증강된 상태인지 아닌지를 체크 합니다.
    /// </summary>
    //public static bool 마커인식상태 = false;

    /// <summary>
    /// 마커가 인식된 카운트 입니다. (최초 한번 인식을 처리 하기 위해)
    /// </summary>
    //public static int 인식된마커카운트 = 0;

    /// <summary>
    /// 발견된 마커 이름 입니다.
    /// </summary>
    //public static string 인식된마커이름 = string.Empty;
    //public static string 비인식된마커이름 = string.Empty;

    private TrackableBehaviour mTrackableBehaviour;

    void Awake()
    {

    }

    void Start()
    {
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();

        if (mTrackableBehaviour)
        {
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
        }
    }

    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            OnTrackingFound();
            
            
            TargetManager.trackableStatus = true;
            if (TargetManager.타깃메니저.PreMarkerFound(mTrackableBehaviour.TrackableName))            
            {
                //TargetManager.trackingFoundName = mTrackableBehaviour.TrackableName;               
                TargetManager.타깃메니저.MarkerEventFound();
            }
        }
        else
        {
            if (TargetManager.trackableStatus)
            {
                TargetManager.trackableStatus = false;
                if (TargetManager.타깃메니저.PreMarkerLost())
                {
                    TargetManager.타깃메니저.MarkerEventLost();
                }
            }
            else
            {
                // 3d 모델의 renderer 를 비활성화 함.
                // 비인식 일 경우 화면에 3d 모델이 보여야 하기 때문에 인식 한 이후 비인식으로 동작 하는 경우는 OnTrackingLost() 동작하지 않도록 함.
                OnTrackingLost();
            }
        }
    }

    /// <summary>
    /// 마커 인식 상태
    /// </summary>
    private void OnTrackingFound()
    {
        Renderer[] rendererComponents = GetComponentsInChildren<Renderer>(true);
        Collider[] colliderComponents = GetComponentsInChildren<Collider>(true);

        // Enable rendering:
        foreach (Renderer component in rendererComponents)
        {
            component.enabled = true;
        }

        // Enable colliders:
        foreach (Collider component in colliderComponents)
        {
            component.enabled = true;
        }
    }

    /// <summary>
    /// 마커 비인식 상태
    /// </summary>
    private void OnTrackingLost()
    {
        Renderer[] rendererComponents = GetComponentsInChildren<Renderer>(true);
        Collider[] colliderComponents = GetComponentsInChildren<Collider>(true);

        // Disable rendering:
        foreach (Renderer component in rendererComponents)
        {
            component.enabled = false;            
        }

        // Disable colliders:
        foreach (Collider component in colliderComponents)
        {
            component.enabled = false;            
        }
    }
}

