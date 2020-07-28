using UnityEngine;
using System.Collections;

public class FollowCam : MonoBehaviour {

    /// <summary>
    /// 따라갈 타겟
    /// </summary>
    public Transform target;
    /// <summary>
    /// 거리
    /// </summary>
    public float dist = 10.0f;
    /// <summary>
    /// 높이
    /// </summary>
    public float height = 5.0f;
    /// <summary>
    /// 회전값 가속도
    /// </summary>
    public float dampRotate = 5.0f;
    /// <summary>
    /// 적용되는 각도 값
    /// </summary>
    private float currYAngle;

    private Quaternion cam_Rot;
    private Transform tr;

    private Transform camTransform;
    private Vector3 originalPos;
    private float shake = 0f;

    public float shakeAmount = 0.05f;
    public float decreaseFactor = 1.0f;
    public bool shake_State = false;

    public static FollowCam followCam;

    void Awake()
    {
        followCam = this;

        camTransform = gameObject.transform;
        if (camTransform == null)
        {
            camTransform = GetComponent(typeof(Transform)) as Transform;
        }
    }

    // Use this for initialization
    void Start()
    {
        tr = GetComponent<Transform>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (target != null)
        {
            currYAngle = Mathf.LerpAngle(tr.eulerAngles.y, target.eulerAngles.y, dampRotate * Time.deltaTime);

            cam_Rot = Quaternion.Euler(0, currYAngle, 0);

            //임시로 운전하는 자동차를 따라가는 카메라에 대한 좌표값을 수정해놓았음.수정이 필요할것으로 판단
            tr.position = target.position - ((cam_Rot * Vector3.forward * dist) * 3) + ((Vector3.up * height) / 3);
            tr.LookAt(target);
        }

        if (shake_State)
        {
            if (shake > 0)
            {
                originalPos = camTransform.localPosition;
                camTransform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;

                shake -= Time.deltaTime * decreaseFactor;
            }
            else
            {
                shake = 0f;
                camTransform.localPosition = originalPos;
                shake_State = false;
            }
        }
        
    }

    public void ShakeStart(float shake_power)
    {
        shake = shake_power;
        shake_State = true;
    }
    /*
    private IEnumerator ShakeCam()
    {
        while (true)
        {
            shake -= Time.deltaTime * decreaseFactor;

            if (shake > 0)
            {
                originalPos = camTransform.localPosition;
                camTransform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;
            }
            else
            {
                shake = 0f;
                camTransform.localPosition = originalPos;
                //  shake_State = false;
                yield break;
            }
            yield return new WaitForEndOfFrame();
        }
    }
    */
}
