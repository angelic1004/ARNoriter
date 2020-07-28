using UnityEngine;
using System.Collections;

public class MissileAutoDestroy : MonoBehaviour
{
    public float m_destroyTime;

    void Start()
    {
        Destroy(gameObject, m_destroyTime);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
