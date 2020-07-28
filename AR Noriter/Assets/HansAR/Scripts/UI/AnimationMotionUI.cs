using UnityEngine;
using System.Collections;

public class AnimationMotionUI : Singleton<AnimationMotionUI> {    
    public GameObject   AnimationListBtnObj;
    public GameObject[] AnimationNumberBtnObj;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ChangeAnimationButtonList()
    {

    }

    public void HideAnimationListBtn()
    {
        AnimationListBtnObj.SetActive(false);
    }

    // Animation Motion Btn
    public void OnClickMotionToggleBtn()
    {
        if (AnimationListBtnObj.activeSelf)
        {
            AnimationListBtnObj.SetActive(false);
        }
        else
        {
            AnimationListBtnObj.SetActive(true);
        }
    }

    public int GetIndexAnimationButton(GameObject obj)
    {
        int index = -1;

        for (int idx = 0; idx < AnimationNumberBtnObj.Length; idx++)
        {
            if (string.Compare(AnimationNumberBtnObj[idx].name, obj.name) == 0) {
                index = idx;
                break;
            }
        }

        return index;
    }
}
