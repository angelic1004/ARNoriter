using UnityEngine;
using System.Collections;
using Vuforia;
using System;
using System.IO;

/// <summary>
/// 색칠하기 관련 속성값을 저장합니다.
/// </summary>
public class ColoringInfo : MonoBehaviour {

    [Serializable]
    public class 색칠하기속성값
    {
        public Material 색칠머테리얼;
        public Texture2D 색칠텍스쳐;
        public GameObject 콜라이더;
    }

    public 색칠하기속성값 색칠하기속성;

	// Use this for initialization
	void Start () {
	
	}
}