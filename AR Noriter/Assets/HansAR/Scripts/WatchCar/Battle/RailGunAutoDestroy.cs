using UnityEngine;
using System.Collections;

public class RailGunAutoDestroy : MonoBehaviour
{
    public float m_destroyTime;
    private float m_time = 0;
    private bool m_reserve = false;

    void Start()
    {
        //   gameObject.transform.localScale = 
        //      new Vector3(gameObject.transform.localScale.x, 
        //                 gameObject.transform.localScale.y, 
        //                 0);
    }

    void FixedUpdate()
    {
        m_time += Time.deltaTime;
        if (gameObject.transform.localScale.z < 1)
        {
            gameObject.transform.localScale =
               new Vector3(gameObject.transform.localScale.x,
                           gameObject.transform.localScale.y,
                           m_time * 6);
        }
        if (m_time >= m_destroyTime)
        {
            Destroy(gameObject);
        }
    }
}
