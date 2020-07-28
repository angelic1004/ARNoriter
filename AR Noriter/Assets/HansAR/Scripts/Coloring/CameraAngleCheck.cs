using UnityEngine;
using System.Collections;

public class CameraAngleCheck : MonoBehaviour
{
    public Transform AR카메라;
    public float 앵글각도;

    void Start()
    {
        StartCoroutine(앵글각도구하기());
    }

    void Update()
    {
    }

    IEnumerator 앵글각도구하기()
    {
        while(true)
        {
            // AR카메라와의 각도 구하기
            transform.rotation = Quaternion.Euler(0, AR카메라.transform.localEulerAngles.y, 0);
            앵글각도 = Quaternion.Angle(transform.rotation, AR카메라.rotation);

            yield return new WaitForSeconds(0.2f);
        }
    }
}