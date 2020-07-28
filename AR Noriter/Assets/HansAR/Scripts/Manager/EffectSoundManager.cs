using UnityEngine;

using System;
using System.Collections;
using System.Reflection;

public class EffectSoundManager : MonoBehaviour
{
    /// <summary>
    /// 사운드를 출력할 오브젝트를 에디터에서 지정해줍니다.
    /// </summary>
    public GameObject 이펙트사운드;

    public static EffectSoundManager 이펙트;

    public bool 이펙트사용 = false;

    /// <summary>
    /// 사운드 반복재생 여부를 선택합니다.
    /// </summary>
    public bool 루프여부 = false;

    public bool 색칠사용여부 = false;

    public bool 음소거사용여부 = false;

    private bool 이펙트사운드실행여부 = false;
    private bool 음소거확인 = false;
    private float 이펙트볼륨;
    private float 더빙볼륨;

    void Awake()
    {
        이펙트 = this;
    }

    void Start()
    {
        이펙트볼륨 = 이펙트사운드.GetComponent<AudioSource>().volume;
        더빙볼륨 = DubbingManager.더빙.더빙사운드.GetComponent<AudioSource>().volume;
    }

    void Update()
    {

    }

    void FixedUpdate()
    {


    }
   
    private bool FindTargetManagerObject(string name)
    {
        object findObj = null;

        try
        {
            findObj = GameObject.Find(name).GetComponent<TargetManager>();

            if (findObj != null)
            {
                return true;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(string.Format("Function:{0}, Message:{1}", MethodBase.GetCurrentMethod().Name, ex.Message));
            throw;
        }

        return false;
    }

    public void 이펙트사운드_재생(GameObject 모델링)
    {
        if (FindTargetManagerObject(string.Format("AR Viewer")))
        {
            if (TargetManager.타깃메니저.스케치씬사용)
            {
                if (색칠사용여부)
                {
                    if (이펙트사운드 != null && 이펙트사용)
                    {
                        AudioClip 오디오클립 = 모델링.GetComponent<ModelInfo>().이펙트클립;
                        AudioSource 오디오소스 = 이펙트사운드.GetComponent<AudioSource>();

                        if (오디오클립 != null)
                        {
                            오디오소스.Stop();

                            오디오소스.loop = 루프여부;
                            오디오소스.clip = 오디오클립;
                            오디오소스.Play();
                            이펙트사운드실행여부 = true;
                        }
                    }
                }
            }
            else
            {
                if (이펙트사운드 != null && 이펙트사용)
                {
                    AudioClip 오디오클립 = 모델링.GetComponent<ModelInfo>().이펙트클립;
                    AudioSource 오디오소스 = 이펙트사운드.GetComponent<AudioSource>();

                    if (오디오클립 != null)
                    {
                        오디오소스.Stop();

                        오디오소스.loop = 루프여부;
                        오디오소스.clip = 오디오클립;
                        오디오소스.Play();
                        이펙트사운드실행여부 = true;
                    }
                }
            }
        }
        else
        {
            if (이펙트사운드 != null && 이펙트사용)
            {
                AudioClip 오디오클립 = 모델링.GetComponent<ModelInfo>().이펙트클립;
                AudioSource 오디오소스 = 이펙트사운드.GetComponent<AudioSource>();

                if (오디오클립 != null)
                {
                    오디오소스.Stop();

                    오디오소스.loop = 루프여부;
                    오디오소스.clip = 오디오클립;
                    오디오소스.Play();
                    이펙트사운드실행여부 = true;
                }
            }
        }


        /*
        if (TargetManager.타깃메니저.스케치씬사용)
        {
            if (색칠사용여부)
            {
                if (이펙트사운드 != null && 이펙트사용)
                {
                    AudioClip 오디오클립 = 모델링.GetComponent<ModelInfo>().이펙트클립;
                    AudioSource 오디오소스 = 이펙트사운드.GetComponent<AudioSource>();

                    if (오디오클립 != null)
                    {
                        오디오소스.Stop();

                        오디오소스.loop = 루프여부;
                        오디오소스.clip = 오디오클립;
                        오디오소스.Play();
                        이펙트사운드실행여부 = true;
                    }
                }
            }
        }
        else
        {
            if (이펙트사운드 != null)
            {
                AudioClip 오디오클립 = 모델링.GetComponent<ModelInfo>().이펙트클립;
                AudioSource 오디오소스 = 이펙트사운드.GetComponent<AudioSource>();

                if (오디오클립 != null)
                {
                    오디오소스.Stop();

                    오디오소스.loop = 루프여부;
                    오디오소스.clip = 오디오클립;
                    오디오소스.Play();
                    이펙트사운드실행여부 = true;
                }
            }
        }
        */
    }

    public void 이펙트사운드_중지()
    {
        if (이펙트사운드 != null)
        {
            이펙트사운드.GetComponent<AudioSource>().Stop();
        }
    }

    /// <summary>
    /// 음소거 버튼 이미지 변경
    /// </summary>
    /// <param name="obj"></param>
    private void ChangeSoundImg(GameObject obj)
    {
        if (obj != null)
        {
            if (음소거확인)
            {
                UISprite img = obj.GetComponent<UISprite>();
                img.spriteName = "sound_mute_btn";
                obj.GetComponent<UIButton>().normalSprite = "sound_mute_btn";
            }
            else
            {
                UISprite img = obj.GetComponent<UISprite>();
                img.spriteName = "sound_btn";
                obj.GetComponent<UIButton>().normalSprite = "sound_btn";
            }
        }
    }

}
