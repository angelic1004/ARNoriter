using UnityEngine;
using System.Collections;

public class FollowObj : MonoBehaviour
{

    public Transform objTr;

    void FixedUpdate()
    {
        transform.position = objTr.position;
        transform.rotation = objTr.rotation;
    }
}
