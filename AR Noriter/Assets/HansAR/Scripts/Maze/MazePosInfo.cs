using UnityEngine;
using System.Collections;

/// <summary>
/// 미로의 각 갈림길에다 넣을 
/// </summary>
public class MazePosInfo : MonoBehaviour
{

    public bool isPassed;

    #region 오브젝트를 기준으로 상하좌우에 대한 길을 알려줄 pos값

    /// <summary>
    /// 위쪽 통행길
    /// </summary>
    public GameObject upPos;

    /// <summary>
    /// 아래쪽 통행길
    /// </summary>
    public GameObject downPos;

    /// <summary>
    /// 좌측 통행길
    /// </summary>
    public GameObject leftPos;

    /// <summary>
    /// 우측 통행길
    /// </summary>
    public GameObject rightPos;
    #endregion

    /// <summary>
    /// 막다른 지점인지 아닌지
    /// </summary>
    public bool deadEndPos;

    /// <summary>
    /// 골지점
    /// </summary>
    public bool goalPos;

    public int roadCount = 0;

    void OnEnable()
    {
        isPassed = false;
    }

    void Start()
    {
        if (upPos != null)
        {
            roadCount += 1;
        }
        if (downPos != null)
        {
            roadCount += 1;
        }
        if (leftPos != null)
        {
            roadCount += 1;
        }
        if (rightPos != null)
        {
            roadCount += 1;
        }
    }
}
