using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class DanceAudioManager : Singleton<DanceAudioManager> {
    //public bool             IsShuffle;
    public float            DefaultVolumeValue;
    public AudioSource      BackgroundAudioSource;       // 배경 사운드 및 댄스댄스, 댄스배틀의 댄스곡을 재생 할 AudioSource 
    
    void Awake()
    {
        
    }

	// Use this for initialization
	void Start () {        
        
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    /*
    // Background Audio Source
    private void InitThemeShuffleArray(int themeSoundCount)
    {
        if (themeShuffleArray != null)
        {
            themeShuffleArray = null;
        }

        themeShuffleArray = new int[themeSoundCount];
        
        for (int idx = 0; idx < themeSoundCount; idx++)
        {
            themeShuffleArray[idx] = idx;
        }
    }

    private int GetThemeShuffleValue()
    {
        int idx             = -1;
        int shuffleValue    = -1;

        if (themeSoundPlayCount < themeShuffleArray.Length)
        {
            idx = Random.Range(0, (themeShuffleArray.Length - themeSoundPlayCount));

            // 랜덤 숫자 선택
            shuffleValue = themeShuffleArray[idx];
            SwapInteger(ref themeShuffleArray[idx], ref themeShuffleArray[themeShuffleArray.Length - themeSoundPlayCount - 1]);
        }

        themeSoundPlayCount++;

        return shuffleValue;
    }

    /// <summary>
    /// 두 정수를 SWAP 합니다.
    /// </summary>
    private void SwapInteger(ref int a, ref int b)
    {
        int temp = a;

        a = b;
        b = temp;
    }
    
    private IEnumerator PlayBackgroundSoundShuffle()
    {
        int shuffleIndex = -1;
        
        while (true)
        {
            if (BackgroundAudioSource.isPlaying)
            {
                yield return null;
            }
            else
            {
                shuffleIndex = GetThemeShuffleValue();

                if (shuffleIndex == -1)
                {
                    InitThemeShuffleArray();
                    shuffleIndex = GetThemeShuffleValue();
                }

                PlayBackgroundSound(DanceThemes[shuffleIndex]);
            }
        }
    }
    */

    private IEnumerator SoundFadeOut(AudioSource audioSource, float fadeTime)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeTime;

            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;        
    }
    
    private void BackgroundAudoPlay()
    {
        try
        {
            if (BackgroundAudioSource.isPlaying)
            {
                BackgroundAudioSource.Stop();                
            }

            if (BackgroundAudioSource == null || BackgroundAudioSource.clip == null)
            {
                return;
            }

            BackgroundAudioSource.time = 0;
            BackgroundAudioSource.Play();
        }
        catch
        {

        }        
    }

    public void SetAudioBackgroundClip(AudioClip audioClip)
    {
        if (BackgroundAudioSource == null)
        {
            return;
        }

        BackgroundAudioSource.clip = audioClip;
    }

    public void PlayBackgroundSound()
    {
        if (BackgroundAudioSource == null)
        {
            return;
        }

        if (BackgroundAudioSource.clip == null)
        {
            return;
        }

        BackgroundAudoPlay();
    }

    public void PlayBackgroundSound(AudioClip audioClip)
    {
        if (BackgroundAudioSource == null)
        {
            return;
        }

        SetAudioBackgroundClip(audioClip);
        BackgroundAudoPlay();
    }

    public void PlayBackgroundSound(AudioClip audioClip, bool isLoop)
    {
        if (BackgroundAudioSource == null)
        {
            return;
        }

        SetAudioBackgroundClip(audioClip);
        BackgroundAudioSource.loop = isLoop;

        BackgroundAudoPlay();
    }

    public void PlayBackgroundSound(AudioClip audioClip, float startValue)
    {
        if (BackgroundAudioSource == null)
        {
            return;
        }

        SetAudioBackgroundClip(audioClip);

        BackgroundAudioSource.time = startValue;
        BackgroundAudioSource.Play();
    }

    public void SetBackgroundAudioVolumeValue(float volume)
    {
        if (BackgroundAudioSource == null)
        {
            return;
        }

        BackgroundAudioSource.volume = volume;
    }

    public void StopBackgroundSound()
    {
        StartCoroutine(SoundFadeOut(BackgroundAudioSource, 1f));
    }

    public bool IsBackgroundSoundPlaying()
    {
        return BackgroundAudioSource.isPlaying;
    }
    //-- Bakcground Audio Source




    
}
