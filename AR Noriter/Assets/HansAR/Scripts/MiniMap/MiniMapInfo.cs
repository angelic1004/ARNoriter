using UnityEngine;

using System;
using System.Collections;

public class MiniMapInfo : MonoBehaviour
{
    [Serializable]
    public class MiniMapResourceInfo
    {
        public string               miniMapCompareModelName;
        public int                  targetIndex;
        public Material[]           miniMapMaterials;
        public GameObject[]         miniMapObjects;
        public AnimationClip[]      miniMapAnimations;
        
        public GameObject           miniMapPointAnimationTarget;
        public AudioClip[]          miniMapAnimationAudio;
        public GameObject           miniMapAnimatorTargetObj;

        public bool                 isShowObject;
    }

    [SerializeField]
    public MiniMapResourceInfo[]    miniMapResourceInfo;

    public GameObject               miniMapCollider;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
