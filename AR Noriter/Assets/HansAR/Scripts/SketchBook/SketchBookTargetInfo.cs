using UnityEngine;
using System.Collections;

public class SketchBookTargetInfo : MonoBehaviour {


    /// <summary>
    /// 문제 번호
    /// </summary>
    public enum QuizNum
    {
        NONE,
        ONE,
        TWO,
        THREE,
        FOUR
    }

    public QuizNum m_QuizMarkerState;
    public QuizNum m_StickerMarkerState;
    public QuizNum m_MapMarkerState;
    public QuizNum m_SketchPuzzleMarkerState;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
