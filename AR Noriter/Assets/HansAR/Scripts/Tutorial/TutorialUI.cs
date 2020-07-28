using UnityEngine;
using System;
using System.Text;
using System.Collections;

public class TutorialUI : MonoBehaviour
{
    //화살표(손가락)표시 여부
    public bool setArrow = true;

    //인식하지 않고도 튜토리얼 내용 실행
    public bool straight = false;

    //지문 한번에 다 보여주기
    public bool touchAllText = true;

    public GameObject setActiveUi;
    
    /// <summary>
    /// 전체 튜토리얼 UI 오브젝트
    /// </summary>
    public GameObject tutorialUI;

    /// <summary>
    /// 튜토리얼 화살표 오브젝트
    /// </summary>
    public GameObject arrowObj;

    public GameObject nextArrowObj;
    public GameObject copyObj;

    private int stringIndex = 0;
    public float textPrintTime = 0.05f;
    private Vector3 textUiSprPos;
    private Vector3 labelBgSprPos;

    /// <summary>
    /// 튜토리얼 텍스트 정보
    /// </summary>
    [Serializable]
    public class TextInfo
    {
        public GameObject bgObj;
        public GameObject textUiObj;
        public UILabel textLabel;
        public GameObject bgBoder;
    }


    [Serializable]
    public class ExplanationList
    {
        public bool nguiPosSet;
        public GameObject targetObj;
        public string targetObjName;
        public string localizeValue;
    }

    public TextInfo textInfo;

    [SerializeField]
    public ExplanationList[] explanList;

    private float textNextTime;
    private float savedNextTime;
    private int explanIndex = 0;

    [HideInInspector]
    public int savedexplanIndex = -1;

    private StringBuilder m_Builder;

    /// <summary>
    /// 텍스트 한글자씩 나오는 코루틴
    /// </summary>
    private Coroutine textCoroutine;

    public static TutorialUI instance;

    void Awake()
    {
        instance = this;
        savedexplanIndex = -1;
    }

    // Use this for initialization
    void Start()
    {
        m_Builder = new StringBuilder();
        m_Builder.Remove(0, m_Builder.Length);
        tutorialUI.GetComponent<UIPanel>().alpha = 0;

        textUiSprPos = textInfo.textUiObj.transform.localPosition;
        labelBgSprPos = textInfo.textLabel.transform.parent.localPosition;

        textInfo.bgBoder = transform.FindChild("BG_border").gameObject;

        if (textInfo.bgBoder != null)
        {
            textInfo.bgBoder.SetActive(false);
            textInfo.bgBoder.GetComponent<UIWidget>().depth = -6;
        }
        textInfo.bgObj.GetComponent<UIWidget>().alpha = 0.6f;
        textInfo.bgObj.GetComponent<UIButton>().tweenTarget = null;
        textInfo.bgObj.GetComponent<UIWidget>().depth = -5;
        textInfo.bgObj.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TutorialStartClick()
    {
        arrowObj.GetComponent<UIWidget>().depth = 50;

        textUiSprPos = textInfo.textUiObj.transform.localPosition;
        labelBgSprPos = textInfo.textLabel.transform.parent.localPosition;

        if (TargetManager.trackableStatus)
        {
            Debug.Log("비인식 상태에서만 튜토리얼 확인이 가능합니다.");
            return;
        }

        if (setActiveUi != null)
        {
            if (!setActiveUi.activeSelf)
            {
                Debug.Log("setActiveUi에 active 상태가 false 입니다.");
                return;
            }
            else
            {
                if (setActiveUi.GetComponent<UIPanel>() != null)
                {
                    if (setActiveUi.GetComponent<UIPanel>().alpha == 0)
                    {
                        Debug.Log("setActiveUi에 Panel 알파값이 0 입니다.");
                        return;
                    }
                }
            }
        }

        if (!straight)
        {
            if (!TargetManager.타깃메니저.첫인식상태)
            {
                Debug.Log("첫인식 후 튜토리얼 가능합니다.");
                return;
            }
        }


        TargetManager.EnableTracking = false;

        if(!TargetManager.타깃메니저.usedWordGame)
        {
            if (!TargetManager.타깃메니저.usedSelfiMode)
            {
                TouchEventManager.터치.기준콜라이더 = null;
            }
            else
            {
                TouchEventManager.터치.기준콜라이더.GetComponent<BoxCollider>().enabled = false;
            }
        }
        

        explanIndex = 0;
        savedexplanIndex = -1;
        //textInfo.bgObj.GetComponent<UIWidget>().alpha = 0;

        TweenManager.tween_Manager.TweenAllDestroy(tutorialUI);
        TweenManager.tween_Manager.AddTweenAlpha(tutorialUI, tutorialUI.GetComponent<UIPanel>().alpha, 1, 0.1f);
        TweenManager.tween_Manager.TweenAlpha(tutorialUI);

        if (setArrow)
        {
            TweenManager.tween_Manager.TweenAllDestroy(arrowObj);
            TweenManager.tween_Manager.AddTweenAlpha(arrowObj, 0, 1, 0.7f, UITweener.Style.PingPong);
            TweenManager.tween_Manager.TweenAlpha(arrowObj);
        }

        if (textInfo.bgBoder != null)
        {
            textInfo.bgBoder.SetActive(true);
        }

        textInfo.bgObj.SetActive(true);

        NextTutorialClick();
    }

    public void TutorialEnd()
    {
        TweenManager.tween_Manager.TweenAllDestroy(tutorialUI);
        TweenManager.tween_Manager.AddTweenAlpha(tutorialUI, tutorialUI.GetComponent<UIPanel>().alpha, 0, 0.3f);
        TweenManager.tween_Manager.TweenAlpha(tutorialUI);

        if (TargetManager.타깃메니저.공부하기사용)
        {
            TargetManager.EnableTracking = true;
        }

        if (TargetManager.타깃메니저.usedSelfiMode)
        {
            TouchEventManager.터치.기준콜라이더 = LetterManager.Instance.letterInfo.entireBody.gameObject;
            TouchEventManager.터치.기준콜라이더.GetComponent<BoxCollider>().enabled = true;
            TargetManager.EnableTracking = true;
        }
        else
        {
            if (!TargetManager.타깃메니저.usedWordGame)
            {
                TouchEventManager.터치.기준콜라이더 = TouchEventManager.터치.지형2D;
            }
        }

        if(TargetManager.타깃메니저.usedFishing)
        {
            FishingGameManager.instance.SideMenuClosePause();
        }

        if (textInfo.bgBoder != null)
        {
            textInfo.bgBoder.SetActive(false);
        }

        stringIndex = 0;
        textNextTime = textPrintTime;

        Invoke("TextPosInit", 0.5f);
        textInfo.bgObj.SetActive(false);
    }

    private void TextPosInit()
    {
        textInfo.textUiObj.transform.localPosition = textUiSprPos;
        textInfo.textLabel.transform.parent.localPosition = labelBgSprPos;
    }

    public void NextTutorialClick()
    {
        //TextBgCollider(false);
        GameObject obj = null;

        if (explanList.Length <= explanIndex)
        {
            Debug.LogWarning("내용이 모두 끝났습니다.");
            TutorialEnd();
            return;
        }
        else if (explanList[explanIndex].targetObjName == string.Empty && explanList[explanIndex].targetObj == null)
        {
            Debug.LogError("오브젝트가 없습니다. 비었습니다.");
            TutorialEnd();
            return;
        }

        Debug.Log("explanList[explanIndex].targetObjName : " + explanList[explanIndex].targetObjName);
        if (textNextTime <= 0 && savedexplanIndex != explanIndex)
        {
            savedexplanIndex = explanIndex;

            if (explanList[explanIndex].targetObj != null)
            {
                obj = explanList[explanIndex].targetObj;
            }
            else if (explanList[explanIndex].targetObjName != string.Empty)
            {
                obj = GameObject.Find(explanList[explanIndex].targetObjName).gameObject;
            }

            // GameObject ob j = GameObject.Find(explanList[explanIndex].targetObjName).gameObject;

            if (explanList[explanIndex].nguiPosSet)
            {
                Vector3 p = Camera.main.WorldToViewportPoint(obj.transform.position);
                p = UICamera.mainCamera.ViewportToWorldPoint(p);
                if (setArrow)
                {
                    arrowObj.transform.position = new Vector3(p.x, p.y, 0);
                }
            }
            else
            {
                if (setArrow)
                {
                    arrowObj.transform.position = obj.transform.position;// new Vector3(obj.transform.position.x, obj.transform.position.y, 0);
                    SprCopy(obj);
                }
            }

            if (arrowObj.transform.localPosition.y >= 0)
            {
                textInfo.textUiObj.transform.localPosition = textUiSprPos;
                textInfo.textLabel.transform.parent.localPosition = labelBgSprPos;
            }
            else
            {
                textInfo.textUiObj.transform.localPosition = new Vector3(textUiSprPos.x, -textUiSprPos.y, textUiSprPos.z);
                textInfo.textLabel.transform.parent.localPosition = new Vector3(labelBgSprPos.x, -labelBgSprPos.y, labelBgSprPos.z);
            }
            stringIndex = 0;
            TextPrintStart(LocalizeText.Value[explanList[explanIndex].localizeValue]);
        }
        else
        {
            textNextTime = 0;
            savedNextTime = 0;

            if(touchAllText)
            {
                stringIndex = LocalizeText.Value[explanList[explanIndex].localizeValue].Length;
            }
        }
    }


    private void SprCopy(GameObject targetObj)
    {
        if (copyObj != null)
        {
            Destroy(copyObj);
        }

        copyObj = (Instantiate(targetObj) as GameObject);

        if (copyObj != null)
        {
            TweenManager.tween_Manager.TweenAllDestroy(copyObj);
            
            //panel이 설정되어 있는 경우 튜토리얼 창보다 아래에 있을 수 있기 때문에 삭제
            if (copyObj.GetComponent<UIPanel>() != null)
            {
                Destroy(copyObj.GetComponent<UIPanel>());
            }

            //복제하여 보여지는 튜토리얼 이미지를 클릭시 이벤트 발생하지 않도록 collider enabled = false
            Transform[] tran = copyObj.GetComponentsInChildren<Transform>(true);

            foreach (Transform child in tran)
            {
                if(child.GetComponent<BoxCollider2D>() !=null)
                {
                    child.GetComponent<BoxCollider2D>().enabled = false;
                }
            }

            copyObj.name = targetObj.name;
            copyObj.transform.parent = this.transform;
            copyObj.transform.position = targetObj.transform.position;
            copyObj.transform.localScale = Vector3.one;
            copyObj.SetActive(true);
        }
    }

    private void TextBgCollider(bool state)
    {
        textInfo.bgObj.GetComponent<BoxCollider2D>().enabled = state;

        if (setArrow)
        {
            nextArrowObj.SetActive(state);
        }
    }

    private void TextPrintStart(string text)
    {
        TextPrintStop();
        textCoroutine = StartCoroutine(TextPrintCor(text));
    }

    private void TextPrintStop()
    {
        if (textCoroutine != null)
        {
            StopCoroutine(textCoroutine);
            textCoroutine = null;
        }
    }

    private string RandomTamiSprite()
    {
        string tamiSprName = string.Empty;

        tamiSprName = string.Format("{0}{1}", "tutorial_tami", UnityEngine.Random.Range(1, 4));

        return tamiSprName;
    }

    private IEnumerator TextPrintCor(string insertText)
    {
        textNextTime = textPrintTime;
        savedNextTime = textNextTime;

        float waitTime = 0.3f;

        textInfo.textLabel.text = string.Empty;
        textInfo.textUiObj.GetComponent<UISprite>().spriteName = RandomTamiSprite();

        TweenManager.tween_Manager.TweenAllDestroy(textInfo.textUiObj);
        TweenManager.tween_Manager.AddTweenAlpha(textInfo.textUiObj, textInfo.textUiObj.GetComponent<UIWidget>().alpha, 1, waitTime);
        TweenManager.tween_Manager.TweenAlpha(textInfo.textUiObj.gameObject);

        yield return new WaitForSeconds(waitTime);

        while (true)
        {
            textNextTime = textNextTime - Time.deltaTime;

            if (textNextTime <= 0)
            {
                if (stringIndex > insertText.Length)
                {
                    //yield return new WaitForSeconds(waitTime * 3);

                    TextBgCollider(true);
                    explanIndex++;

                    Debug.Log("텍스트 코루틴 끝");
                    yield break;
                }

                textInfo.textLabel.text
                     = m_Builder.AppendFormat("{0}", insertText.Substring(0, stringIndex)).ToString();
                m_Builder.Remove(0, m_Builder.Length);
                textNextTime = savedNextTime;
                stringIndex++;
            }
            yield return new WaitForEndOfFrame();
        }

    }

}
