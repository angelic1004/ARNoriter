using UnityEngine;
using System.Collections;

public class PrincessUI : MonoBehaviour
{
    public GameObject btnOpen;
    public GameObject btnList;
    /// <summary>
    /// UI 상태 표시 true = 오픈 false = 클로즈
    /// </summary>
    private bool switchState = false;

    public static PrincessUI princessInstance;

    void Awake()
    {
        princessInstance = this;
    }

    void Update()
    {
    }

    public void buttonSwitch(GameObject actionBtn)
    {
        if (switchState)
        {
            actionBtn.GetComponent<UISprite>().spriteName = "btn_out";
        }
        else
        {
            actionBtn.GetComponent<UISprite>().spriteName = "btn_in";
        }

        switchState = !switchState;
    }

    /// <summary>
    /// 버튼이름과 현재 인식된 타겟오브젝트의 이름이 같다면 버튼이 활성화 되게끔 변경
    /// </summary>
    /// <param name="targetName"></param>
    public void activePrincessBtn(string targetName)
    {
        if (btnList.transform.FindChild(targetName))
        {
            GameObject tempObj = btnList.transform.FindChild(targetName).gameObject;
            //tempObj.GetComponent<UISprite>().spriteName = "btn_japan";
            //tempObj.GetComponent<UIButton>().normalSprite = "btn_japan";

            tempObj.GetComponent<UISprite>().alpha          = 1f;
            tempObj.GetComponent<UIButton>().defaultColor   = new Color(255, 255, 255, 255);
            tempObj.GetComponent<UIButton>().hover          = new Color(255, 255, 255, 255);
            tempObj.GetComponent<UIButton>().pressed        = new Color(255, 255, 255, 255);
            tempObj.GetComponent<UIButton>().disabledColor  = new Color(255, 255, 255, 255);


            tempObj.GetComponent<BoxCollider2D>().enabled = true;
        }
        else
        {
            Debug.LogError("There is no button with the same name : " + targetName);
        }
    }

    public void OnClickAniPlayBtn(GameObject obj)
    {
        MiniMapAnimationControl.instance.SaveContentsIndex(obj.name);
    }
}