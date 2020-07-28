using UnityEngine;
using System.Collections;

public class AnimationPlayManager : Singleton<AnimationPlayManager> {

    private Animator    animatorObj;
    private Animation   animationObj;
    


    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// 애니메이션을 실행하는 함수
    /// 이게 필요하려나....
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="trigger"></param>
    public void ChangeAnimationWithTrigger(Animator animator, string trigger)
    {
        if (animator == null)
        {
            return;
        }

        animatorObj = animator;
        animatorObj.SetTrigger(trigger);
    }

    public void RequestAnimationPlay(Animation animation, AnimationClip clip)
    {
        animationObj = animation;
        animationObj.clip = clip;

        animationObj.Play();
    }

    
    
}
