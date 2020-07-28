using UnityEngine;
using System;
using System.Collections;

public class ControlManager : MonoBehaviour
{
    public bool fiveBtnSet = false;

    public GameObject portraitUi;
    public GameObject landScapeUi;

    public GameObject nowControlObj;
    public GameObject nowCameraObj;

    [Serializable]
    public class ControlUi
    {
        public GameObject controlObj;
        public GameObject cameraObj;

        public int fourWidth;
        public int fiveWidth;
    }

    [SerializeField]
    public ControlUi[] screenRotUi;

    private int nowFourWidth;
    private int nowFiveWidth;

    private Coroutine changeCor;
    private bool 테스트 = true;
    public static ControlManager instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        BottomMaenuInit();
    }

    void Update()
    {

    }

    private void BottomMaenuInit()
    {
        ScreenRot(테스트);

        gameObject.GetComponent<BtnTweenInfo>().moveObj.transform.position
            = gameObject.GetComponent<BtnTweenInfo>().startPos.position;

        if (nowControlObj != null)
        {
            nowControlObj.SetActive(true);
            nowControlObj.GetComponent<UIPanel>().alpha = 1;
        }

        if (nowCameraObj != null)
        {
            nowCameraObj.SetActive(true);
            nowCameraObj.GetComponent<UIPanel>().alpha = 0;
        }
    }

    public void 화면전환테스트()
    {
        테스트 = !테스트;
        ScreenRot(테스트);
    }

    private void ScreenRot(bool state)
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        GameObject bgObj = gameObject.GetComponent<BtnTweenInfo>().moveObj;

        if (TweenManager.tween_Manager.slideMenuOpen)
        {
            Debug.Log("엔드");
            bgObj.transform.position = GetComponent<BtnTweenInfo>().fourEndPos.position;

            if (bgObj.GetComponent<TweenPosition>() != null)
            {
                bgObj.GetComponent<TweenPosition>().to = GetComponent<BtnTweenInfo>().fourEndPos.localPosition;
            }

        }
        else
        {
            Debug.Log("스타트");
            bgObj.transform.position = GetComponent<BtnTweenInfo>().startPos.position;

            if (bgObj.GetComponent<TweenPosition>() != null)
            {
                bgObj.GetComponent<TweenPosition>().to = GetComponent<BtnTweenInfo>().startPos.localPosition;
            }
        }

        portraitUi.SetActive(true);
        landScapeUi.SetActive(true);

        for (int i = 0; i < screenRotUi.Length; i++)
        {
            TweenManager.tween_Manager.TweenAllDestroy(screenRotUi[i].controlObj);
            TweenManager.tween_Manager.TweenAllDestroy(screenRotUi[i].cameraObj);

            screenRotUi[i].controlObj.SetActive(true);
            screenRotUi[i].cameraObj.SetActive(true);

            screenRotUi[i].controlObj.GetComponent<UIPanel>().alpha = 0;
            screenRotUi[i].cameraObj.GetComponent<UIPanel>().alpha = 0;
        }

        //가로
        //if (Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeRight)
        if (state)
        {
            nowFourWidth = screenRotUi[1].fourWidth;
            nowFiveWidth = screenRotUi[1].fiveWidth;

            nowControlObj = screenRotUi[1].controlObj;
            nowCameraObj = screenRotUi[1].cameraObj;

            nowControlObj.transform.GetChild(0).GetComponent<UIToggle>().value
                = screenRotUi[0].controlObj.transform.GetChild(0).GetComponent<UIToggle>().value;

            nowControlObj.transform.GetChild(1).GetComponent<UIToggle>().value
              = screenRotUi[0].controlObj.transform.GetChild(1).GetComponent<UIToggle>().value;

            if (fiveBtnSet)
            {
                bgObj.GetComponent<UIWidget>().width = nowFiveWidth;
                nowCameraObj.GetComponent<UIPanel>().alpha = 1;
                nowControlObj.GetComponent<UIPanel>().alpha = 0;
            }
            else
            {
                bgObj.GetComponent<UIWidget>().width = nowFourWidth;
                nowControlObj.GetComponent<UIPanel>().alpha = 1;
                nowCameraObj.GetComponent<UIPanel>().alpha = 0;
            }

        }
        //세로
        //else if (Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown)
        else
        {
            nowFourWidth = screenRotUi[0].fourWidth;
            nowFiveWidth = screenRotUi[0].fiveWidth;

            nowControlObj = screenRotUi[0].controlObj;
            nowCameraObj = screenRotUi[0].cameraObj;

            nowControlObj.transform.GetChild(0).GetComponent<UIToggle>().value
                 = screenRotUi[1].controlObj.transform.GetChild(0).GetComponent<UIToggle>().value;

            nowControlObj.transform.GetChild(1).GetComponent<UIToggle>().value
                 = screenRotUi[1].controlObj.transform.GetChild(1).GetComponent<UIToggle>().value;

            if (fiveBtnSet)
            {
                bgObj.GetComponent<UIWidget>().width = nowFiveWidth;
                nowCameraObj.GetComponent<UIPanel>().alpha = 1;
                nowControlObj.GetComponent<UIPanel>().alpha = 0;
            }
            else
            {
                bgObj.GetComponent<UIWidget>().width = nowFourWidth;
                nowControlObj.GetComponent<UIPanel>().alpha = 1;
                nowCameraObj.GetComponent<UIPanel>().alpha = 0;
            }
        }
    }

    public void BottomMenuBtnClick(GameObject obj)
    {
        Vector3 startScale = Vector3.one;
        Vector3 endScale = Vector3.zero;//new Vector3(0.5f, 0.5f, 0.5f);
        float tweenTime = 0.2f;

        if (obj.GetComponent<TweenScale>() != null)
        {
            //end -> start
            if (obj.GetComponent<TweenScale>().to == endScale)
            {
                TweenManager.tween_Manager.TweenAllDestroy(obj);
                TweenManager.tween_Manager.AddTweenScale(obj,
                                                         obj.transform.localScale,
                                                         startScale,
                                                         tweenTime,
                                                         UITweener.Style.Once,
                                                         TweenManager.tween_Manager.scaleAnimationCurve);
            }
            //start -> end
            else
            {
                TweenManager.tween_Manager.TweenAllDestroy(obj);
                TweenManager.tween_Manager.AddTweenScale(obj, obj.transform.localScale, endScale, tweenTime);
            }
        }
        //start -> end
        else
        {
            TweenManager.tween_Manager.AddTweenScale(obj, obj.transform.localScale, endScale, tweenTime);
        }

        TweenManager.tween_Manager.TweenScale(obj);
    }

    public void ChangeBtnClick()
    {
        ChangeCorStart(gameObject);
    }


    private void ChangeCorStart(GameObject obj)
    {
        ChangeCorStop();

        changeCor = StartCoroutine(ChangeCorPlay(obj));
    }


    private void ChangeCorStop()
    {
        if (changeCor != null)
        {
            StopCoroutine(changeCor);
            changeCor = null;
        }
    }

    private IEnumerator ChangeCorPlay(GameObject obj)
    {
        yield return new WaitForSeconds(gameObject.GetComponent<BtnTweenInfo>().slideTime);

        GameObject bgObj = gameObject.GetComponent<BtnTweenInfo>().moveObj;

        float bgWidth = bgObj.GetComponent<UIWidget>().width;
        float endWidth = 0;
        float contEndWidth = nowFourWidth;
        float camEndWidth = nowFiveWidth;
        float speed = 2000;


        TweenManager.tween_Manager.TweenAllDestroy(nowCameraObj);
        TweenManager.tween_Manager.TweenAllDestroy(nowControlObj);

        if (nowControlObj.GetComponent<UIPanel>().alpha > 0)
        {
            TweenManager.tween_Manager.AddTweenAlpha(nowControlObj, nowControlObj.GetComponent<UIPanel>().alpha, 0, 0.2f);
            TweenManager.tween_Manager.TweenAlpha(nowControlObj);
            endWidth = camEndWidth;
            fiveBtnSet = true;
        }
        else
        {
            TweenManager.tween_Manager.AddTweenAlpha(nowCameraObj, nowCameraObj.GetComponent<UIPanel>().alpha, 0, 0.2f);
            TweenManager.tween_Manager.TweenAlpha(nowCameraObj);
            endWidth = contEndWidth;
            fiveBtnSet = false;
        }

        yield return new WaitForSeconds(0.2f);

        while (true)
        {
            yield return new WaitForEndOfFrame();

            //control -> camera( 아이콘수 : 4 -> 5)
            if (bgObj.GetComponent<UIWidget>().width < endWidth)
            {
                bgWidth += Time.deltaTime * speed;

                bgObj.GetComponent<UIWidget>().width = (int)bgWidth;

                if (bgObj.GetComponent<UIWidget>().width >= endWidth)
                {
                    bgObj.GetComponent<UIWidget>().width = (int)endWidth;
                }
            }
            //camera -> control( 아이콘수 : 5 -> 4)
            else
            {
                bgWidth -= Time.deltaTime * speed;

                bgObj.GetComponent<UIWidget>().width = (int)bgWidth;

                if (bgObj.GetComponent<UIWidget>().width <= endWidth)
                {
                    bgObj.GetComponent<UIWidget>().width = (int)endWidth;
                }
            }


            if (endWidth == bgObj.GetComponent<UIWidget>().width)
            {
                if (nowControlObj != null)
                {
                    //control -> camera( 아이콘수 : 4 -> 5)
                    if (endWidth == camEndWidth)
                    {
                        TweenManager.tween_Manager.AddTweenAlpha(nowCameraObj, nowCameraObj.GetComponent<UIPanel>().alpha, 1, 0.2f);
                        TweenManager.tween_Manager.TweenAlpha(nowCameraObj);
                    }
                    //camera -> control( 아이콘수 : 5 -> 4)
                    else
                    {
                        TweenManager.tween_Manager.AddTweenAlpha(nowControlObj, nowControlObj.GetComponent<UIPanel>().alpha, 1, 0.2f);
                        TweenManager.tween_Manager.TweenAlpha(nowControlObj);
                    }
                }
                else
                {
                    Debug.Log("controlObj 가 없습니다.");
                    yield break;
                }

                yield break;
            }
        }
    }
}
