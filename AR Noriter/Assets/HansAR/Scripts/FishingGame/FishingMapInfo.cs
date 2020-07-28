using UnityEngine;
using System;
using System.Collections;

public class FishingMapInfo : MonoBehaviour
{
    [Serializable]
    public class TamiInfo
    {
        public GameObject normalTami;
        public GameObject iceTami;
        public GameObject typhoonTami;

        public string idleTriggerName = "idle";
        public string castTriggerName = "cast";
        public string actionTriggerName = "action";
        public string fishingTriggerName = "fishing";
        public string successTriggerName = "success";
        public string failTriggerName = "fail";
        public string collectTriggerName = "collect";
    }

    [Serializable]
    public class ResultPanelInfo
    {
        public GameObject resultPanel;
        public Texture2D catchKorTex;
        public Texture2D catchEnTex;
        public Texture2D failKorTex;
        public Texture2D failEnTex;
    }

    public GameObject shipObj;
    public GameObject seaObj;
    public GameObject fishingFloatCursor;
    public GameObject fishingFloatFish;

    [SerializeField]
    public TamiInfo tamiInfo;

    [SerializeField]
    public ResultPanelInfo resultPanelInfo;

    public GameObject[] mapLevelObj;

    public GameObject[] fishPrefabs;

    public GameObject[] trapPrefans;

    public GameObject fishingPar;
}
