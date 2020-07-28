using UnityEngine;
using Vuforia;

public class ImageMarkerPass : MonoBehaviour, ITrackableEventHandler
{
    private bool mIsPossibleTracking = false;

    private TrackableBehaviour mTrackableBehaviour;

    // Use this for initialization
    void Start ()
    {
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();

        if (mTrackableBehaviour)
        {
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            OnTrackingFound();

            Debug.LogWarningFormat("trackerable name = {0}, select category name = {1}", mTrackableBehaviour.TrackableName, GlobalDataManager.m_ResourceFolderEnum.ToString());
            PassTargetManager.m_instance.CompareTrackingTarget(mTrackableBehaviour.TrackableName);                        
        }
        else
        {
            OnTrackingLost();
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
