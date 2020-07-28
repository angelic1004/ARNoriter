using UnityEngine;
using System;
using System.Collections;

public class ModelLabelInfo : MonoBehaviour
{
    [Serializable]
    public class LabelPos
    {
        public Transform targetObjTr;
        public Vector3 setPos;
        public string localizeValue;
    }

    [SerializeField]
    public LabelPos labelPosList;

}
