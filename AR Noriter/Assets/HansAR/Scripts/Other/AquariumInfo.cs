using UnityEngine;

using System;
using System.Collections;

public class AquariumInfo : MonoBehaviour {        
    [Serializable]
    public class AquariumResourceInfo
    {
        public Material fishMaterials;
        public GameObject[] fishObjects;
        public bool isShowObject;
    }

    [SerializeField]
    public AquariumResourceInfo[] aquariumResourceInfo;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
