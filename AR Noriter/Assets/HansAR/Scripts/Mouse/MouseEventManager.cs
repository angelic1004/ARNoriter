using UnityEngine;
using System.Collections;

public class MouseEventManager : Singleton<MouseEventManager>
{
    public bool m_eventCheck;
    public bool m_eventUiCheck;
    public bool m_eventSoundPlay;
    public bool m_eventUiSoundPlay;
    public GameObject[] m_evnectUiList;
    public AudioSource mouseSource;
    public AudioClip mouseClickSoundClip;
    public Camera nguiCam;
    public GameObject clickPrefab;

    public float m_UiTimer;

    public GameObject mouseClickObj;
    private float m_savedTimerTime;

    private bool timerSet = false;

    void Awake()
    {
        nguiCam = UICamera.mainCamera;
    }

    void Start()
    {
        m_savedTimerTime = m_UiTimer;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (nguiCam != null)
            {
                Ray ray = nguiCam.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

                TimerReset();

                if (hit.collider != null && !TargetManager. 타깃메니저.usedSelfiMode)
                {
                    //Debug.Log("hit : " + hit)
                    if (hit.transform.gameObject.GetComponent<BoxCollider2D>() != null)
                    {
                        /*
                        if (m_eventCheck)
                        {
                            MouseClick();
                        }
                        else if (m_eventUiCheck)
                        {
                            mouseClickObj = hit.transform.gameObject;
                            MouseUiClickEvent(mouseClickObj);
                        }
                        */
                        if (TouchEventManager.터치.기준콜라이더 != null)
                        {
                            TouchEventManager.터치.기준콜라이더.GetComponent<BoxCollider>().enabled = false;
                            //TargetManager.EnableTracking = false;
                        }
                    }
                }
            }
        }

        if (Input.GetMouseButtonUp(0) && !TargetManager.타깃메니저.usedSelfiMode)
        {
            RotateUI.회전.콜라이더_활성화();
            //TargetManager.EnableTracking = true;
        }

        if (!Input.GetMouseButton(0))
        {
            if (!timerSet)
            {
                m_UiTimer -= Time.deltaTime;

                if (m_UiTimer < 0)
                {
                    TweenManager.tween_Manager.StartTimerCoroutine(false);
                    timerSet = true;
                }
            }
        }

    }

    public void TimerReset()
    {
        TweenManager.tween_Manager.StartTimerCoroutine(true);
        m_UiTimer = m_savedTimerTime;
        timerSet = false;
    }

    /// <summary>
    /// m_eventCheck = true 의 경우 마우스 클릭이벤트
    /// </summary>
    public void MouseClick()
    {
        if (nguiCam != null)
        {
            Vector3 nowMousePosition = nguiCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z));
            MouseClickEvent(nowMousePosition);
        }
        else
        {
            Debug.Log("ngui 카메라가 없습니다.");
        }
    }

    /// <summary>
    /// m_eventUiCheck = true 의 경우 마우스 클릭지점에 있는 버튼이 m_evnectUiList에 포함되는지 확인 후
    /// m_evnectUiList에 포함 될 경우 particle prefab 생성
    /// </summary>
    /// <param name="obj"> 클릭 버튼 </param>
    private void MouseUiClickEvent(GameObject obj)
    {
        if (m_evnectUiList.Length > 0)
        {
            if (clickPrefab != null)
            {
                for (int i = 0; i < m_evnectUiList.Length; i++)
                {
                    if (m_evnectUiList[i] == obj)
                    {
                        GameObject ShowPrefab = Instantiate(clickPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                        ShowPrefab.transform.parent = obj.transform;
                        ShowPrefab.transform.localPosition = Vector3.zero;
                        MouseUiClickSound();
                        return;
                    }
                }
                Debug.Log("클릭 ui오브젝트에 포함되지 않은 ui 오브젝트입니다.");
            }
            else
            {
                Debug.Log("클릭 prefab이 없습니다.");
            }
        }
        else
        {
            Debug.Log("m_evnectUiList의 길이가 0입니다.");
        }

        MouseClickSound();
    }

    /// <summary>
    /// m_eventCheck = true 의 경우 마우스 클릭지점에서 particle prefab 생성
    /// </summary>
    private void MouseClickEvent(Vector3 nowMousePosition)
    {
        if (clickPrefab != null)
        {
            GameObject ShowPrefab = Instantiate(clickPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            ShowPrefab.transform.localPosition = nowMousePosition;
        }
        else
        {
            Debug.Log("클릭 prefab이 없습니다.");
        }

        MouseClickSound();
    }

    /// <summary>
    /// m_eventCheck = true 의 경우 사운드 플레이
    /// </summary>
    private void MouseClickSound()
    {
        if (m_eventSoundPlay)
        {
            if (mouseSource != null)
            {
                if (mouseClickSoundClip != null)
                {
                    mouseSource.Stop();
                    mouseSource.clip = null;
                    mouseSource.clip = mouseClickSoundClip;
                    mouseSource.Play();
                }
                else
                {
                    Debug.Log("클릭사운드clip이 없습니다.");
                }
            }
            else
            {
                Debug.Log("클릭사운드Source가 없습니다.");
            }
        }
    }

    /// <summary>
    /// m_eventUiCheck = true 이며
    /// m_evnectUiList에 포함 될 경우 particle prefab 생성
    /// </summary>
    private void MouseUiClickSound()
    {
        if (m_eventUiSoundPlay)
        {
            if (mouseSource != null)
            {
                if (mouseClickSoundClip != null)
                {
                    mouseSource.Stop();
                    mouseSource.clip = null;
                    mouseSource.clip = mouseClickSoundClip;
                    mouseSource.Play();
                }
                else
                {
                    Debug.Log("클릭사운드clip이 없습니다.");
                }
            }
            else
            {
                Debug.Log("클릭사운드Source가 없습니다.");
            }
        }
    }
}
