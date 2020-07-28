using UnityEngine;

using System;
using System.Collections;

public class PuzzleUI : Singleton<PuzzleUI> {
    [Serializable]
    public class PuzzleUseDataSet
    {
        public GameObject           puzzleRoot;
        public GameObject           puzzleBackground;
        public GameObject           puzzlePieces;

        [HideInInspector]
        public GameObject[]         puzzleChildPieces;
    }

    // public variables
    public GameObject               m_ContentObject;

    public GameObject               m_RootUI;
    public GameObject               m_ParentPuzzleUI;    
    public GameObject               m_RecognitionTextUI;

    public GameObject               m_PuzzleNormalStartBtnUI;
    public GameObject               m_PuzzleRandomStartBtnUI;
    public GameObject               m_PuzzleRetryBtnUI;
    public GameObject               m_PuzzleShow3dBtnUI;

    public GameObject               m_MoveMultiTargetBtnUI;

    [SerializeField]
    public PuzzleUseDataSet[]       m_PuzzleDataSet;

    void AWake()
    {
        m_ParentPuzzleUI            = null;        
    }

	// Use this for initialization
	void Start () {
        m_ParentPuzzleUI.SetActive(true);   
                     
        ConfigChildPieces();        
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    private int GetChildPiecesCount(int index)
    {
        int pieceCount = 0;

        foreach(Transform trans in m_PuzzleDataSet[index].puzzlePieces.transform)
        {
            pieceCount = pieceCount + 1;
        }

        return pieceCount;
    }

    private void ConfigChildPieces()
    {
        // 각 퍼즐의 자식 조각의 게임 오브젝트를 배열에 넣어 관리 합니다.
        int pieceCount = 0;
        int pieceIndex = 0;

        try
        {
            for (int idx = 0; idx < m_PuzzleDataSet.Length; idx++)
            {
                if (m_PuzzleDataSet[idx].puzzleChildPieces.Length == 0)
                {
                    pieceCount = GetChildPiecesCount(idx);
                    if (pieceCount > 0)
                    {
                        m_PuzzleDataSet[idx].puzzleChildPieces = new GameObject[pieceCount];

                        foreach (Transform trans in m_PuzzleDataSet[idx].puzzlePieces.transform)
                        {
                            m_PuzzleDataSet[idx].puzzleChildPieces[pieceIndex] = trans.gameObject;
                            pieceIndex = pieceIndex + 1;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }
}
