using UnityEngine;
using System.Collections;

using HedgehogTeam.EasyTouch;

public class SketchBookTouchEvent : MonoBehaviour {
    public delegate void _SwipeComplete(bool isNextContent);

    public event _SwipeComplete OnSwipeCompleEvent;
    public float m_SwipeMinDistance = 150;

    private Vector2 m_SwipeStartPos;

    void Awake()
    {
        m_SwipeStartPos = Vector2.zero;
    }

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnEnable()
    {
        // Swipe 이벤트 등록
        EasyTouch.On_SwipeStart += OnSwipeStartEvent;        
        EasyTouch.On_SwipeEnd   += OnSwipeEndEvent;            
    }

    void OnDestroy()
    {
        UnsubscribeEvent();
    }

    void OnDisable()
    {
        UnsubscribeEvent();
    }

    void UnsubscribeEvent()
    {
        // Swipe 이벤트 해제
        EasyTouch.On_SwipeStart -= OnSwipeStartEvent;
        EasyTouch.On_SwipeEnd -= OnSwipeEndEvent;
    }

    private void OnSwipeStartEvent(Gesture gesture)
    {
        // Swipe 시작시 시작 위치값을 저장
        m_SwipeStartPos = gesture.position;
    }
    
    private void OnSwipeEndEvent(Gesture gesture)
    {
        float distanceX         = 0f;
        bool isNextDirection    = false;

        if (SketchBookUI.getInstance.DragQuizText.gameObject.activeSelf == false)
        {
            return;
        }

        // start x 좌표와 end x 좌표 차이 값을 구하여 조건에 맞으면 swipe 동작을 적용 함
        distanceX               = m_SwipeStartPos.x - gesture.position.x;

        // 절대값으로 비교하며, swipe 최소값 보다 작으면 동작 하지 않고 리턴 함
        if (Mathf.Abs(distanceX) < m_SwipeMinDistance)
        {
            Debug.LogWarningFormat("The value has not been reached. (value : {0})", Mathf.Abs(distanceX));
            return;
        }

        if (distanceX >= 0)
        {
            isNextDirection = true;
        }
        
        if (OnSwipeCompleEvent != null)
        {
            // callback 함수가 null 이 아닐경우 실행 (SketchBookUI.cs 내 ViewSketchBookContents())
            OnSwipeCompleEvent(isNextDirection);
        }
    }
}
