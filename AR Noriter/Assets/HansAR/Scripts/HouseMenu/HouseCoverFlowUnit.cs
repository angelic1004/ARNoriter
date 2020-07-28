using UnityEngine;
using System;
using System.Collections;

public class HouseCoverFlowUnit : MonoBehaviour
{

    /// <summary>
    /// 패널의 위치
    /// </summary>
    UIPanel m_Panel;

    //unit의 가로 크기.
    private float m_cellWidth;

    private float m_downAlpha;
    private float pos;
    private float dist;

    void Start()
    {
        m_Panel = transform.parent.parent.GetComponent<UIPanel>();

        //초기값 입력.
        m_cellWidth = transform.parent.GetComponent<UIGrid>().cellWidth;
        m_downAlpha = 1;

        pos = transform.localPosition.x - m_Panel.clipOffset.x;
        dist = Mathf.Clamp(Mathf.Abs(pos), 0f, m_cellWidth);

        this.GetComponent<UIWidget>().alpha = Convert.ToInt32(((m_cellWidth - dist * m_downAlpha) / m_cellWidth) * m_cellWidth) / m_cellWidth;
    }



    void FixedUpdate()
    {
            //중심점과 거리가 얼마나 멀어졌는지 확인.
            pos = transform.localPosition.x - m_Panel.clipOffset.x;
            dist = Mathf.Clamp(Mathf.Abs(pos), 0f, m_cellWidth);

            this.GetComponent<UIWidget>().alpha = Convert.ToInt32(((m_cellWidth - dist * m_downAlpha) / m_cellWidth) * m_cellWidth) / m_cellWidth;
    }
}
