using UnityEngine;

using System;
using System.Collections;

public class MiniMapSlerp : MonoBehaviour {
    public Transform    startTransform;
    public Transform    endTransform;
    public float        durationTime;

    public Vector3[]    initTransformValue { get; set; }
    public bool         isMoveMiniMap { get; set; }

    private float       startTime;

    // Use this for initialization
    void Start () {
        startTime = Time.time;
    }
	
	// Update is called once per frame
	void Update () {
        if (isMoveMiniMap)
        {
            if (startTransform == null || endTransform == null)
            {
                return;
            }

            ApplySlerp();
        }
    }

    private void ApplySlerp()
    {
        Vector3 center                      = Vector3.zero;
        Vector3 startRelCenter              = Vector3.zero;
        Vector3 endRelCenter                = Vector3.zero;
        float fracComplete                  = 0f;

        center                              = (startTransform.position + endTransform.position) * 0.5f;
        center                              -= new Vector3(0f, 1f, 0f);

        startRelCenter                      = startTransform.position - center;
        endRelCenter                        = endTransform.position - center;

        fracComplete                        = (Time.time - startTime) / durationTime;

        //Debug.LogWarning(string.Format("start Time = {0}, startTransform.position = {1}, endTransform.positon = {2}", startTime, startTransform.position, endTransform.position));
        //Debug.LogWarning(string.Format("center = {0}, startRelCenter = {1}, endRelCenter = {2}, fracComplete = {3}", center, startRelCenter, endRelCenter, fracComplete));

        startTransform.transform.position   = Vector3.Slerp(startRelCenter, endRelCenter, fracComplete);
        startTransform.transform.position   += center;
    }
}
