using UnityEngine;
using System.Collections;

public class AutoDestroy : MonoBehaviour
{

    private float DestroyTime;// = 2.0f;

    // Use this for initialization
    void Start()
    {
        DestroyTime = gameObject.GetComponent<ParticleSystem>().startLifetime;
        Destroy(gameObject, DestroyTime);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
