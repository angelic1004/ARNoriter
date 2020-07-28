using UnityEngine;
using System;

public class Driving_Sound : MonoBehaviour
{   

    /// <summary>
    /// 사운드이벤트 상태 
    /// </summary>
    public enum SoundType   
    {
        Bg_Sound,
        Accel,
        Brake,
        Mission,
        Success,
        Fail,
        Collision,
        Safety,
        Ready,
        None
    }

    /// <summary>
    /// 사운드 설정 목록
    /// </summary>
    public AudioSource bg_audio_Source;
    public AudioSource car_audio_Source;
    public AudioSource brake_audio_Source;
    public AudioSource mission_audio_Source;
    public AudioSource collision_audio_Source;
    public AudioSource safetyBelt_audio_Source;

    public AudioClip bg_Clip;
    public AudioClip ready_Clip;
    public AudioClip mission_Clip;
    public AudioClip[] success_Clip;
    public AudioClip[] fail_Clip;
    public AudioClip[] accel_Clip;
    public AudioClip[] brake_Clip;
    public AudioClip[] collision_Clip;
    public AudioClip safetyBelt_Clip;

    private bool accel_Check = false;

    public static Driving_Sound driving_Sound;

    void Awake()
    {
        driving_Sound = this;
    }
    /// <summary>
    /// 사운드 play 선택 부분
    /// </summary>
    /// <param name="type"></param>
    /// <param name="num"></param>
    public void DrivingSoundPlay(SoundType type)
    {
        switch (type)
        {
            case SoundType.Bg_Sound:
                BgSoundStart(type);
                break;
                
            case SoundType.Ready:
                BgSoundStart(type);
                break;

            case SoundType.Accel:
                CarBrakeSoundStart(type);
                break;

            case SoundType.Brake:
                CarBrakeSoundStart(type);
                break;

            case SoundType.Mission:
                MissionSoundStart();
                break;

            case SoundType.Success:
                MissionSoundSuccess();
                break;

            case SoundType.Fail:
                MissionSoundFail();
                break;

            case SoundType.Collision:
                CollsionSoundStart();
                break;

            case SoundType.Safety:
                SafetySoundStart();
                break;

            default:
                Debug.Log("SoundType Error");
                return;
        }
    }

    /// <summary>
    /// 사운드 stop 선택 부분
    /// </summary>
    /// <param name="type"></param>
    public void DrivingSoundStop(SoundType type)
    {
        switch (type)
        {
            case SoundType.Bg_Sound:
                BgSoundStop();
                break;

            case SoundType.Ready:
                BgSoundStop();
                break;

            case SoundType.Accel:
                CarSoundStop();
                break;

            case SoundType.Brake:
                BrakeSoundStop();
                break;

            case SoundType.Mission:
                MissionSoundStop();
                break;

            case SoundType.Collision:
                CollsionSoundStop();
                break;

            case SoundType.Safety:
                SafetySoundStop();
                break;
            default:
                Debug.Log("SoundType Error");
                return;
        }
    }


    #region Sound_Start

    private void BgSoundStart(SoundType type)
    {
        if (type == SoundType.Bg_Sound)
        {
            bg_audio_Source.clip = bg_Clip;
        }
        else if(type == SoundType.Ready)
        {
            bg_audio_Source.clip = ready_Clip;
        }
        bg_audio_Source.Play();
    }

    private void CarBrakeSoundStart(SoundType type)
    {
        //악셀 사운드
        if (type == SoundType.Accel)
        {
            car_audio_Source.loop = true;
            if (Car_Ctrl.car_ctrl.speed_Check != 0)
            {
                car_audio_Source.clip = accel_Clip[Car_Ctrl.car_ctrl.speed_Check - 1];
                car_audio_Source.Play();
            }
            else
            {
                car_audio_Source.clip = accel_Clip[0];
                car_audio_Source.Play();
            }
        }
        //브레이크 사운드 
        else if (type == SoundType.Brake)
        {
            accel_Check = false;
            car_audio_Source.loop = false;
            CarSoundStop();
            if (Car_Ctrl.car_ctrl.speed_Check != 0)
            {
                brake_audio_Source.clip = brake_Clip[Car_Ctrl.car_ctrl.speed_Check - 1];
                brake_audio_Source.Play();
            }
            else
            {
                brake_audio_Source.clip = brake_Clip[0];
                brake_audio_Source.Play();
            }
        }
    }

    private void MissionSoundStart()
    {
        mission_audio_Source.clip = mission_Clip;
        mission_audio_Source.Play();
    }

    private void MissionSoundSuccess()
    {
        if (Car_Ctrl.car_ctrl.event_num < success_Clip.Length)
        {
            mission_audio_Source.clip = success_Clip[Car_Ctrl.car_ctrl.event_num];
        }
        else
        {
            mission_audio_Source.clip = success_Clip[0];
        }
        mission_audio_Source.Play();
    }

    private void MissionSoundFail()
    {
        if (Car_Ctrl.car_ctrl.event_num < success_Clip.Length)
        {
            mission_audio_Source.clip = fail_Clip[Car_Ctrl.car_ctrl.event_num];
        }
        else
        {
            mission_audio_Source.clip = fail_Clip[0];
        }

        mission_audio_Source.Play();
    }

    private void CollsionSoundStart()
    {
        if (Car_Ctrl.car_ctrl.speed_Check != 0)
        {
            collision_audio_Source.clip = collision_Clip[Car_Ctrl.car_ctrl.speed_Check - 1];
        }
        else
        {
            collision_audio_Source.clip = collision_Clip[0];
        }
        collision_audio_Source.Play();
    }

    private void SafetySoundStart()
    {
        safetyBelt_audio_Source.clip = safetyBelt_Clip;
        safetyBelt_audio_Source.Play();
    }
    #endregion

    #region Sound_Stop

    private void BgSoundStop()
    {
        bg_audio_Source.Stop();
    }

    private void CarSoundStop()
    {
        car_audio_Source.Stop();
    }

    private void BrakeSoundStop()
    {
        brake_audio_Source.Stop();
    }

    private void MissionSoundStop()
    {
        mission_audio_Source.Stop();
    }

    private void CollsionSoundStop()
    {
        collision_audio_Source.Stop();
    }

    private void SafetySoundStop()
    {
        safetyBelt_audio_Source.Stop();
    }
    #endregion

}
