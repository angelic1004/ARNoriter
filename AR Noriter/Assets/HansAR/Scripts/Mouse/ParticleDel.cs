using UnityEngine;
using System.Collections;

public class ParticleDel : MonoBehaviour
{

    private ParticleSystem ps;

    void Start()
    {

        ps = GetComponent<ParticleSystem>();

    }

    void Update()
    {

        if (ps)
        {
            if (!ps.IsAlive())
            {
                MouseEventManager.getInstance.mouseClickObj = null;
                Destroy(gameObject);
            }
        }
    }
}
