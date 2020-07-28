using UnityEngine;
using System.Collections;

public class BtnTweenInfo : MonoBehaviour
{
    public bool playing = false;

    public bool playDirection = true;
    public bool reverse = false;

    public bool sprAni = true;
    public float slideTime = 0.5f;
    public int startSub = 0;

    public float nextTime;
    public float reverseNextTime;

    public Vector3 startScale;
    public Vector3 endScale;

    public Vector3 subStartScale;
    public Vector3 subEndScale;

    public GameObject moveObj;
    public Transform startPos;
    public Transform threeEndPos;
    public Transform fourEndPos;
}
