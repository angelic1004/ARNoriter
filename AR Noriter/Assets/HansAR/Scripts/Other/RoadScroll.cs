using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadScroll : MonoBehaviour
{

    public float ScrollX = -2f;
    public float ScrollY = 0f;

    void Update()
    {
        float OffsetX = Time.time * ScrollX;
        float OffsetY = Time.time * ScrollY;
        GetComponent<Renderer>().material.mainTextureOffset = new Vector2(OffsetX, OffsetY);

    }

}
