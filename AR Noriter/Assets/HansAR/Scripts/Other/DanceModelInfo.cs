using UnityEngine;

using System;
using System.Collections;


public class DanceModelInfo : MonoBehaviour {
    /*
    [Serializable]
    public class StudyResourceValue
    {
        public AudioClip    AudioClipEnglish;
        public AudioClip    AudioClipKorean;
        public AudioClip    AudioClipChinese;
        public AudioClip    AudioClipDeutsch;
        public AudioClip    AudioClipJapanese;
        public AudioClip    AudioClipVietnamese;
        public AudioClip    AudioClipIndonesian;

        public string       WordEnglish;
        public string       WordKorean;
        public string       WordChinese;
        public string       WordDeutsch;
        public string       WordJapanese;
        public string       WordVietnamese;
        public string       WordIndonesian;
    } 
    */

    public AudioClip[]  ThemeSoundClips;    
    public AudioClip[]  DanceSoundClips;    
    public AudioClip[]  BattleSoundClips;

    public Animator     DanceAnimator;
    public Animator     BattleAnimator;
    public Animator     EffectAnimator;

    public int          DancerUniformIndex;
    public GameObject   DancerUniformObject;
    public Texture2D[]  DancerUniforms;

    public string[]     NormalAnimatorTriggers;
    public string[]     DanceAnimatorTriggers;

    public bool         isEffectContent;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
