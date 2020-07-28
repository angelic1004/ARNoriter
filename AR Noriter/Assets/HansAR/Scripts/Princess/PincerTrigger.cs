using UnityEngine;
using System.Collections;

public class PincerTrigger : MonoBehaviour
{

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "princess_doll")
        {
            PrincessManager.instance.TriggerReturnStart();
        }
    }

    void OnEnable()
    {
        StartCoroutine(PrincessManager.instance.InitPincerPosition());
    }
}
