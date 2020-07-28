using UnityEngine;
using System.Collections;
using Vuforia;
using System;
using System.IO;


/// <summary>
/// 권기영 : 듣기 말하기 
/// </summary>
public class ListenInfo : MonoBehaviour {


    [Serializable]
    public class 듣고익히기속성값
    {
        public AudioClip 알파벳사운드;
        public AudioClip[] 한글낱말사운드;
        
    }

    public 듣고익히기속성값 듣고익히기속성;

    void Start () {
	
	}
}
