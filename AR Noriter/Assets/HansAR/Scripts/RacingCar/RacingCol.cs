using UnityEngine;
using System.Collections;

public class RacingCol : MonoBehaviour
{
    //충돌 이벤트
    void OnCollisionEnter(Collision col)
    {
        if (this.gameObject == RacingDrive.instance.myCarObj)
        {
            RacingDrive.instance.CollisionEnter(col);
        }
        else
        {
            Debug.Log("Ghost Car Collision");
        }
    }

    void OnTriggerEnter(Collider col)
    {
        Debug.Log(this.gameObject);

        if (this.gameObject == RacingDrive.instance.myCarObj)
        {
            RacingDrive.instance.TriggerEnter(col,true, this.gameObject);
        }
        else
        {
            RacingDrive.instance.TriggerEnter(col, false, this.gameObject);
        }
    }
}
