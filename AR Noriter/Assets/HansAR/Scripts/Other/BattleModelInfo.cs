using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class BattleModelInfo : MonoBehaviour {
    [Serializable]
    public class AnimationInfo
    {
        public AnimationClip[] AniClips;
    }

    [Serializable]
    public class TouchModelingInfo
    {
        public GameObject       TouchObject;
        public string           TouchCharacterNames;
    }

    public GameObject           AnimationTarget;
    public AnimationInfo        AniInfo;

    public Texture2D[]          DancerUniforms;

    public AudioClip[]          ThemeSoundClips;
    public AudioClip[]          BattleSoundClips;

    public TouchModelingInfo[]  TouchModelInfo;    

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
