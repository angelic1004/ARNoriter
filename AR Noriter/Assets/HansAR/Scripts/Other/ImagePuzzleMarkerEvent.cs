using UnityEngine;
using Vuforia;

using System;
using System.Collections;

public class ImagePuzzleMarkerEvent : MonoBehaviour, ITrackableEventHandler
{
    private TrackableBehaviour mTrackableBehaviour;

    // Use this for initialization
    void Start()
    {
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();

        if (mTrackableBehaviour)
        {
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            //인식된마커이름 = mTrackableBehaviour.TrackableName;
            OnTrackingFound();

            PuzzleManager.getInstance.m_RecognitionCount++;
            PuzzleManager.getInstance.m_RecognitionStatus = true;            
            PuzzleManager.getInstance.TrackingFoundPuzzlePiece(mTrackableBehaviour.TrackableName);       
        }
        else
        {
            //비인식된마커이름 = mTrackableBehaviour.TrackableName;
            
            if (PuzzleManager.getInstance.m_RecognitionCount == 0)
            {
                OnTrackingLost();
            }

            PuzzleManager.getInstance.m_RecognitionStatus = false;            
            PuzzleManager.getInstance.TrackingLostPuzzlePiece(mTrackableBehaviour.TrackableName);
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
