using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class FishingControllUI : MonoBehaviour {
    public enum StickBarStatus {
        Stop = 0,
        Left,
        Right
    }

    private StickBarStatus  curStickBarStatus;
    private Coroutine       fishingStickBarCoroutine;
    private Coroutine       fishingTimerCoroutine;

    private float           initStickBarXPos;

    public GameObject       FishingBtnObject;
    public GameObject       FishingUIObject;    
    public GameObject       StickObject;

    public GameObject       FishingTimerObject;
    public GameObject       ResultMessageObject;

    public int              FishingLimitedTime;

    public float            StickMoveSpeed;


    public static string FishingSuccessMsg;
    public static string FishingFailMsg;
    public static string FishingTimeOverMsg;
    public static string FishingTouchMiss;

    public static FishingControllUI Instance;

    void Awake()
    {
        Instance            = this;

        FishingSuccessMsg   = string.Format("낚시를 성공 했습니다.");
        FishingFailMsg      = string.Format("낚시를 실패 했습니다.");
        FishingTimeOverMsg  = string.Format("제한 시간을 초과 했습니다.");
        FishingTouchMiss    = string.Format("노란선 안쪽에서 터치 하세요.");
    }

	// Use this for initialization
	void Start () {
        initStickBarXPos = StickObject.transform.localPosition.x;
        InitFishingUIStatus();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void FixedUpdate()
    {

    }
    
    private IEnumerator BetweenSideWall(GameObject moveObj, float speed)
    {
        float addXPos = 0f;
        
        moveObj.transform.localPosition = new Vector3(initStickBarXPos, moveObj.transform.localPosition.y, moveObj.transform.localPosition.z);
        FishingUIObject.SetActive(true);

        while (curStickBarStatus != StickBarStatus.Stop)
        {
            addXPos = Time.fixedDeltaTime * speed;            
            
            if (curStickBarStatus == StickBarStatus.Left)
            {
                moveObj.transform.localPosition = new Vector3(moveObj.transform.localPosition.x - addXPos, moveObj.transform.localPosition.y, moveObj.transform.localPosition.z);
            }
            else if (curStickBarStatus == StickBarStatus.Right)
            {
                moveObj.transform.localPosition = new Vector3(moveObj.transform.localPosition.x + addXPos, moveObj.transform.localPosition.y, moveObj.transform.localPosition.z);
            }            

            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator FishingLimitedTimer()
    {
        float timerPlayTime     = 0f;
        UILabel timerLabel      = null;

        timerLabel              = FishingTimerObject.GetComponent<UILabel>();
        timerPlayTime           = FishingLimitedTime;        

        while (timerPlayTime > 0)
        {
            timerLabel.text = string.Format("{0:0.0}", timerPlayTime);
            timerPlayTime -= Time.fixedDeltaTime;
        
            yield return new WaitForFixedUpdate();
        }

        StopFishingTimer(false);        
    }

    
    public void OnMessageDirectionChange(GameObject wallObject)
    {
        if (wallObject.name == "LeftWall")
        {
            // curStickBarStatus 를 right 로 변경 
            curStickBarStatus = StickBarStatus.Right;
        }
        else if (wallObject.name == "RightWall")
        {
            // curStickBarStatus 를 left 로 변경
            curStickBarStatus = StickBarStatus.Left;
        }        
    }

    public void ShowFishingGameUI()
    {
        if (curStickBarStatus != StickBarStatus.Stop)
        {
            InitFishingUIStatus();
        }

        curStickBarStatus = StickBarStatus.Right;
        fishingStickBarCoroutine = StartCoroutine(BetweenSideWall(StickObject, StickMoveSpeed));        
    }

    public StickBarStatus GetStickBarStatus()
    {
        return curStickBarStatus;
    }

    public void InitFishingUIStatus()
    {
        curStickBarStatus = StickBarStatus.Stop;

        if (fishingStickBarCoroutine != null)
        {
            StopCoroutine(fishingStickBarCoroutine);
            fishingStickBarCoroutine = null;
        }

        FishingUIObject.SetActive(false);
        FishingTimerObject.GetComponent<UILabel>().text = FishingLimitedTime.ToString();
    }

    public void PlayFishingTimer()
    {
        fishingTimerCoroutine = StartCoroutine(FishingLimitedTimer());
    }

    public void ShowFishingResultMessgae(string msg)
    {
        UILabel label       = null;
        TweenAlpha alpha    = null;        

        label               = ResultMessageObject.GetComponent<UILabel>();
        label.text          = msg;

        if (ResultMessageObject.activeSelf == false)
        {
            ResultMessageObject.SetActive(true);
        }

        alpha = ResultMessageObject.GetComponent<TweenAlpha>();

        alpha.ResetToBeginning();
        alpha.PlayForward();
    }

    public void StopFishingTimer(bool isFishingComplete)
    {
        // 코루틴 멈추고
        if (fishingTimerCoroutine != null)
        {
            StopCoroutine(fishingTimerCoroutine);
            fishingTimerCoroutine = null;
        }

        if (isFishingComplete == false)
        {
            // 낚시를 제 시간에 못 끝냈으면           
            ShowFishingResultMessgae(FishingTimeOverMsg);           
        }

        InitFishingUIStatus();
        //AquariumManager.Instance.AllHideFishModeling();        
    }    
}
