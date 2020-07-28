using UnityEngine;

using System;
using System.Collections;

public class PuzzleContentInfo : Singleton<PuzzleContentInfo> {
    [Serializable]
    public class NumberAudioClipInfo
    {
        public AudioClip[] audioClipKorea;
        public AudioClip[] audioClipEngish;
        public AudioClip[] audioClipChina;
    }

    public AudioClip m_AudioClipEffect;

    public NumberAudioClipInfo m_AudioClipNumbers;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
