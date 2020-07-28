using UnityEngine;
using System.Collections;

public class SkyBoxManager : MonoBehaviour
{
    public float rotSpeed = 2.0f;

    private Skybox sky;
    private float rot = 0;
    
    void Awake()
    {
        sky = UICamera.mainCamera.GetComponent<Skybox>();
    }
    void Start()
    {

    }

    void FixedUpdate()
    {
        rot += Time.deltaTime * rotSpeed;
        rot %= 360;
        sky.material.SetFloat("_Rotation", rot);
    }
}
