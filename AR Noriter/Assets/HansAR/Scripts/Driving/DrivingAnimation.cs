using UnityEngine;
using System.Collections;
using System;
public class DrivingAnimation : MonoBehaviour {

    /// <summary>
    /// 애니메이션 컨트롤러
    /// </summary>
    public RuntimeAnimatorController ani_Controller;
    public Avatar ani_Avatar;

    /// <summary>
    /// 애니메이션 
    /// </summary>
    [Serializable]
    public class MapAnimationSetting
    {
        public AnimationClip[] animation;
    }

    [SerializeField]
    public MapAnimationSetting[] event_Number;

    public Animator driving_Animator;

    /// <summary>
    /// 현재 애니메이션 
    /// </summary>
    public string nowAni_Name;

    /// <summary>
    /// 맵
    /// </summary>
    public GameObject map;

    public static DrivingAnimation driving_Anim;

    void Start()
    {
        driving_Anim = this;
    }

    void Update()
    {
        
    }

    /// <summary>
    /// 스케치 오브젝트인지 여부에 따른 Animator 추가 및 설정
    /// </summary>
    public void AnimationInit()
    {
        if (MainUI.sketch_Car != null)
        {
            MainUI.sketch_Car.AddComponent<Animator>();
            MainUI.sketch_Car.GetComponent<Animator>().runtimeAnimatorController = ani_Controller;
            MainUI.sketch_Car.GetComponent<Animator>().avatar = ani_Avatar;
            driving_Animator = MainUI.sketch_Car.GetComponent<Animator>();
        }
        else
        {
            Car_Ctrl.car_ctrl.null_Car.GetComponent<Animator>().runtimeAnimatorController = ani_Controller;
            Car_Ctrl.car_ctrl.null_Car.GetComponent<Animator>().avatar = ani_Avatar;
            driving_Animator = Car_Ctrl.car_ctrl.null_Car.GetComponent<Animator>();
        }

        MainUI.메인.운전하기UI.SetActive(true);
        MainUI.메인.딜레이팝업UI.SetActive(false);
    }

    #region 차량 애니메이션 호출

    public void IdleAnimation()
    {
        AnimationStart("Idle");
    }

    public void LeftAnimation()
    {
        AnimationStart("Left");
    }

    public void RightAnimation()
    {
        AnimationStart("Right");
    }

    public void ForwardAnimation()
    {
        AnimationStart("Forward");
    }

    private void AnimationStart(string ani_Name)
    {
        if (nowAni_Name == "" || nowAni_Name != ani_Name)
        {
            nowAni_Name = ani_Name;
            driving_Animator.SetTrigger(ani_Name);
            driving_Animator.speed = 1.0f;
        }
    }
    #endregion

    #region 맵 애니메이션 호출

    public void MapAnimationStart(int ani_Num)
    {
        MapAnimationCall(Car_Ctrl.car_ctrl.event_num, ani_Num);
    }

    public void MapAnimationStop()
    {
        MapAnimationUnCalled();
    }

    private void MapAnimationCall(int event_Num, int ani_Num)
    {
        Animation map_animation = map.GetComponent<Animation>();
        if (event_Number[event_Num].animation.Length > ani_Num)
        {
            map_animation.AddClip(event_Number[event_Num].animation[ani_Num], event_Number[event_Num].animation[ani_Num].name);
            map_animation.clip = event_Number[event_Num].animation[ani_Num];
            map_animation.Stop();
            map_animation.Play();
        }
        else if(event_Number[event_Num].animation.Length > 0)
        {
            map_animation.clip = event_Number[event_Num].animation[0];
            map_animation.Stop();
            map_animation.Play();
        }
    }

    private void MapAnimationUnCalled()
    {
        Animation map_animation = map.GetComponent<Animation>();
        map_animation.Stop();
    }
    #endregion

}
