using UnityEngine;
using System.Collections;
using Vuforia;
using System;
using System.IO;

/// <summary>
/// 성대혁 작성 : 증강될 모델링에 들어가는 모든 속성값을 저장합니다.
/// </summary>
public class ModelInfo : MonoBehaviour
{
    /// <summary>
    /// 증강될 컨텐츠에 저장될 공부하기 속성값을 저장합니다.
    /// </summary>
    [Serializable]
    public class 공부하기속성값
    {
        public AudioClip 영어;

        public AudioClip 한국어;

        public AudioClip 중국어;

        public AudioClip 독일어;

        public AudioClip 일본어;

        public AudioClip 베트남어;

        public AudioClip 인도네시아어;

        public string 영어_텍스트;

        public string 한국어_텍스트;

        public string 중국어_텍스트;

        public string 독일어_텍스트;

        public string 일본어_텍스트;

        public string 베트남어_텍스트;

        public string 인도네시아어_텍스트;

    }

    [Serializable]
    public class 질문답변속성값
    {
        public AudioClip 질문;

        public AudioClip 답변;

        public string 질문_텍스트;

        public string 답변_텍스트;
    }

    [Serializable]
    public class 애니정보
    {
        public AnimationClip[] 애니클립;
    }

    public GameObject 애니메이션타겟;
    public GameObject[] 애니메이션다중타겟;

    // true 이면 애니메이션 정보 ---> 애니클립[0] 은 wakeup animation 할당
    public bool useWakeUpAniClip;

    public bool passiveTransfomrSet;
    public Vector3 모델포지션;
    public Vector3 모델각도;


    /// <summary>
    /// 속성값을 저장할 변수
    /// </summary>
    public 애니정보 애니메이션정보;

    public AudioClip 이펙트클립;
    public AudioClip 더빙클립;

    public bool UsedEveryPlayAudio;

    public AudioClip 동요클립;

    public 공부하기속성값 공부하기속성;

    public 질문답변속성값 질문답변속성;

    public AudioClip[] 다중사운드클립;

    void Start()
    {

    }
}