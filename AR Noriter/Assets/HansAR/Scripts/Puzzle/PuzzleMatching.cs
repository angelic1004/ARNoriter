using UnityEngine;
using System.Collections;

public class PuzzleMatching : Singleton<PuzzleMatching>
{
    const int PUZZLE_PIECES_NUM = 10;                         // 퍼즐 조각 개수
    const float INVOKE_DELAY_TIME = 2.0f;                     // INVOKE 호출 딜레이 시간

    // 매칭 상태
    public enum MatchingStatus
    {
        QUIZ_END,                                             // 퀴즈 종료
        QUIZ_SET,                                             // 퀴즈 셋팅 됨
        WAITING_QUIZ_SET,                                     // 퀴즈 셋팅 대기중
        SAME_PIECES,                                          // 같은 조각일 경우
        DIFFERENT_PIECES_MSG1,                                // 다른 조각일 경우 (메세지1)
        DIFFERENT_PIECES_MSG2                                 // 다른 조각일 경우 (메세지2)
    }

    private MatchingStatus matchingStatus;

    private int[] randNumArr;                                // 랜덤 숫자 선택용 배열
    private int matchingCount;                               // 매칭 횟수
    private int quizPieceNum;                                // 질문할 피스 번호
    public UILabel quizLabel;                                // 퀴즈 라벨
    public bool isQuizMode = true;                           // 퀴즈 모드 사용할건지 체크 (Hierarchy에서 설정)
    public bool isRandomMode = false;

    void Start()
    {
        InitMatchingValues();
        InitRandNumArray();

        //RunQuizEvent();           // 외부에서 호출하여 시작 함.
    }

    /// <summary>
    /// 퍼즐 매칭 값들을 초기화 합니다.
    /// </summary>
    private void InitMatchingValues()
    {
        matchingCount = 0;
        quizPieceNum = -1;

        matchingStatus = MatchingStatus.WAITING_QUIZ_SET;
        
        if (isQuizMode)
        {
            ShowQuizLabel();
        }
        else
        {
            HideQuizLabel();
        }
    }

    /// <summary>
    /// 랜덤 배열값을 초기화 합니다.
    /// </summary>
    private void InitRandNumArray()
    {
        randNumArr = new int[PUZZLE_PIECES_NUM];

        for (int i = 0; i < randNumArr.Length; i++)
        {
            randNumArr[i] = i + 1;
        }
    }

    /// <summary>
    /// 퀴즈를 새로 시작합니다.
    /// </summary>
    public void StartNewQuiz()
    {
        InitMatchingValues();
        RunQuizEvent();
    }

    public void InitMatchingData()
    {
        InitMatchingValues();
        ChangeQuizLabelText(string.Empty);

        matchingStatus = MatchingStatus.WAITING_QUIZ_SET;       
    }

    /// <summary>
    /// 퍼즐 매칭을 실행합니다. (외부 호출용)
    /// </summary>
    /// <param name="cogPieceNum">인식된 피스 번호</param>
    public bool RunPuzzleMatching(int cogPieceNum)
    {
        // 매칭 여부
        bool isMatching = false;

        if (matchingStatus != MatchingStatus.QUIZ_END)
        {
            // 매칭 확인
            CheckMatching(cogPieceNum);

            // 매칭이 되었으면
            if (matchingStatus == MatchingStatus.SAME_PIECES)
            {
                isMatching = true;
                //Invoke("RunQuizEvent", 1);
            }
            else if (matchingStatus == MatchingStatus.DIFFERENT_PIECES_MSG1)
            {
                isMatching = false;
            }
            else
            {
                Debug.LogError("isMatching 값 설정이 잘못 되었습니다.");
            }
        }
        else
        {
            ShowQuizLabel();

            Debug.Log("퀴즈가 종료되었습니다.");
        }

        ResetLabelText();

        return isMatching;
    }

    /// <summary>
    /// 질문 이벤트 (외부 호출용)
    /// </summary>
    public void RunQuizEvent()
    {
        if (matchingStatus == MatchingStatus.SAME_PIECES)
        {
            matchingStatus = MatchingStatus.WAITING_QUIZ_SET;
        }

        // 문제가 셋팅되어 있지 않았다면
        if (matchingStatus == MatchingStatus.WAITING_QUIZ_SET)
        {
            if (isRandomMode)
            {
                // 랜덤 숫자 가져옴
                quizPieceNum = GetRandomNumber();
            }
            else
            {
                quizPieceNum = ++matchingCount;
            }

            // 게임 종료 여부 체크
            if (matchingCount > PUZZLE_PIECES_NUM)
            {
                matchingStatus = MatchingStatus.QUIZ_END;
            }
            else
            {
                matchingStatus = MatchingStatus.QUIZ_SET;
            }

            ResetLabelText();
        }
    }

    /// <summary>
    /// 퀴즈가 끝났는지 체크합니다. (외부 호출용)
    /// </summary>
    public bool CheckEndOfQuiz()
    {
        bool isEndOfQuiz = false;

        if (matchingStatus == MatchingStatus.QUIZ_END)
        {
            isEndOfQuiz = true;
        }
        else
        {
            isEndOfQuiz = false;
        }

        return isEndOfQuiz;
    }

    /// <summary>
    /// 중복되지 않은 난수를 선택합니다.
    /// </summary>
    /// <returns>randNum : 모든 난수 선택시 -1 리턴</returns>
    private int GetRandomNumber()
    {
        int randNum = -1;

        // 총 매칭 횟수가 퍼즐 조각 개수보다 작을때
        if (matchingCount < PUZZLE_PIECES_NUM)
        {
            int idx = Random.Range(0, randNumArr.Length - matchingCount);

            // 랜덤 숫자 선택
            randNum = randNumArr[idx];
            SwapInteger(ref randNumArr[idx], ref randNumArr[randNumArr.Length - matchingCount - 1]);
        }
        else
        {
            Debug.Log("모든 난수 선택 완료");
        }

        matchingCount++;

        return randNum;
    }

    /// <summary>
    /// 두 정수를 SWAP 합니다.
    /// </summary>
    private void SwapInteger(ref int a, ref int b)
    {
        int temp = a;

        a = b;
        b = temp;
    }

    /// <summary>
    /// 매칭 되었는지 확인합니다.
    /// </summary>
    private void CheckMatching(int cogPieceNum)
    {
        // 인식된 조각번호와 퀴즈번호가 같은지 비교
        if (cogPieceNum == quizPieceNum)
        {
            matchingStatus = MatchingStatus.SAME_PIECES;
        }
        else
        {
            matchingStatus = MatchingStatus.DIFFERENT_PIECES_MSG1;
        }
    }

    /// <summary>
    /// QUIZ 라벨 텍스트를 변경합니다.
    /// </summary>
    private void ChangeQuizLabelText(string text)
    {
        quizLabel.text = text;
    }

    public void ClearLabelText(string text)
    {
        ChangeQuizLabelText(text);
    }
    
    /// <summary>
    /// 라벨 텍스트를 리셋합니다. (외부 호출용)
    /// </summary>
    public void ResetLabelText()
    {
        switch (matchingStatus)
        {
            case MatchingStatus.SAME_PIECES:
                ChangeQuizLabelText("같은 조각을 찾았습니다. 잘했어요.");
                break;

            case MatchingStatus.DIFFERENT_PIECES_MSG1:
                ChangeQuizLabelText(string.Format("다른 조각입니다.", quizPieceNum.ToString()));

                matchingStatus = MatchingStatus.DIFFERENT_PIECES_MSG2;
                Invoke("ResetLabelText", INVOKE_DELAY_TIME);
                break;

            case MatchingStatus.DIFFERENT_PIECES_MSG2:
                ChangeQuizLabelText(string.Format("{0}번 퍼즐 조각을 다시 찾아주세요.", quizPieceNum.ToString()));
                break;

            case MatchingStatus.QUIZ_SET:
                ChangeQuizLabelText(string.Format("{0}번 퍼즐 조각을 찾아주세요.", quizPieceNum.ToString()));
                break;

            case MatchingStatus.WAITING_QUIZ_SET:
                ChangeQuizLabelText(string.Format("{0}번 퍼즐 조각을 찾아주세요.", quizPieceNum.ToString()));
                break;

            case MatchingStatus.QUIZ_END:
                ChangeQuizLabelText("축하합니다. 모든 조각을 맞추었습니다.");
                break;

            default:
                Debug.LogError("MatchingStatus Enum값 추가가 필요합니다.");
                break;
        }
    }

    /// <summary>
    /// 퀴즈 라벨을 보입도록 합니다.
    /// </summary>
    private void ShowQuizLabel()
    {
        quizLabel.gameObject.SetActive(true);
    }

    /// <summary>
    /// 퀴즈 라벨을 숨깁니다.
    /// </summary>
    private void HideQuizLabel()
    {
        quizLabel.gameObject.SetActive(false);
    }

    ///// <summary>
    ///// 테스트용 스크립트
    ///// </summary>
    //public void Test(GameObject obj)
    //{
    //    int num = int.Parse(obj.name);

    //    RunPuzzleMatching(num);
    //}

    public int GetCurrentPieceNumber()
    {
        return quizPieceNum - 1;
    }
}