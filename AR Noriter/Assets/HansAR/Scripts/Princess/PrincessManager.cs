using UnityEngine;
using System.Collections;

public class PrincessManager : MonoBehaviour
{
    
    /// <summary>
    /// 문제를 출력할 라벨을 저장.
    /// </summary>
    public UILabel questionText;

    /// <summary>
    /// 인형뽑기 UI
    /// </summary>
    public GameObject princessClawUI;

    /// <summary>
    /// 공주 오브젝트들을 저장.
    /// </summary>
    public GameObject[] princessObj;

    /// <summary>
    /// 집게 오브젝트를 저장.
    /// </summary>
    public GameObject pincer;

    /// <summary>
    /// 찾을 공주 이미지를 출력
    /// </summary>
    public GameObject findPrincess;

    /// <summary>
    /// 인형 행거 저장
    /// </summary>
    public GameObject hanger;

    /// <summary>
    /// 정답 체크 오브젝트
    /// </summary>
    public GameObject answer;

    /// <summary>
    /// 오답 체크 오브젝트
    /// </summary>
    public GameObject wrongAnswer;

    /// <summary>
    /// 재시작 버튼
    /// </summary>
    public GameObject btnRestart;

    /// <summary>
    /// 종료 버튼
    /// </summary>
    public GameObject btnQuit;

    /// <summary>
    /// 집게의 속도를 지정할 변수
    /// </summary>
    public float clawVelocity = 500;

    /// <summary>
    /// 집게가 어느 위치에서 인형을 집을지 비율을 정합니다.
    /// </summary>
    public float dollRatio = 5;
    
    /// <summary>
    /// 집게에 대한 초기 위치값을 저장.
    /// </summary>
    private Vector3 pincerPos;

    /// <summary>
    /// 터치한 인형 오브젝트와 위치를 저장.
    /// </summary>
    private GameObject currentlyPrincess;
    private Vector3 currentlyPrincessPos;

    /// <summary>
    /// 인형뽑기가 시작됐는지 여부
    /// </summary>
    private bool isStart = false;

    /// <summary>
    /// x,y를 분할이동 하기위한 bool값
    /// </summary>
    private bool moveAxis = false;

    /// <summary>
    /// 집게가 되돌아가는 중인지
    /// </summary>
    private bool returnMove = false;

    /// <summary>
    /// 동일한 인형 출제여부를 확인하기 위한 bool값
    /// </summary>
    private bool[] questionNum;

    /// <summary>
    /// 집게가 현재 이동중인지
    /// </summary>
    private bool isMoving = false;

    /// <summary>
    /// 정답의 공주인형 배열번호
    /// </summary>
    private int rightAnswerNum = -1;

    /// <summary>
    /// 선택된 공주인형 배열번호
    /// </summary>
    private int choiceNum = -1;

    /// <summary>
    /// 문제가 출제된 횟수를 저장
    /// </summary>
    private int currentlyProblem = 0;

    public static PrincessManager instance;

    void Awake()
    {
        instance = this;
    }

    //void Start()
    //{
    //    StartCoroutine(QuestionPositionReset());
    //}

    ///// <summary>
    ///// 인형뽑기 맨처음 초기화
    ///// </summary>
    //public IEnumerator QuestionPositionReset()
    //{
    //    princessClawUI.SetActive(true);

    //    yield return new WaitForSeconds(0.1f);

    //    //UI가 켜졌을 시,공주들의 앵커를 꺼줌
    //    for (int i = 0; i < princessObj.Length; i++)
    //    {
    //        princessObj[i].GetComponent<UISprite>().SetAnchor(null, 0, 0, 0, 0);
    //    }

    //    //UI가 켜졌을 시, 집게의 앵커를 꺼주고, 트윈의 초기값을 현재 위치로 변경
    //    pincer.GetComponent<TweenPosition>().from = pincer.transform.localPosition;
    //    pincer.GetComponent<TweenPosition>().to = pincer.transform.localPosition;
    //    pincer.GetComponent<UISprite>().SetAnchor(null, 0, 0, 0, 0);

    //    //집게의 초기 위치값을 저장합니다.(되돌아올 위치값 저장)
    //    if (pincer != null)
    //    {
    //        pincerPos = pincer.transform.localPosition;
    //    }
    //    else
    //    {
    //        Debug.LogError("Pincer is Null");
    //    }

    //    yield return new WaitForSeconds(0.1f);
    //    princessClawUI.SetActive(false);
    //}

    /// <summary>
    /// 인형뽑기 갈고리 초기화
    /// </summary>
    public IEnumerator InitPincerPosition()
    {
        yield return new WaitForSeconds(0.2f);

        //트윈의 초기값을 현재 위치로 변경
        pincer.GetComponent<TweenPosition>().from = pincer.transform.localPosition;
        pincer.GetComponent<TweenPosition>().to = pincer.transform.localPosition;

        //집게의 초기 위치값을 저장합니다.(되돌아올 위치값 저장)
        if (pincer != null)
        {
            pincerPos = pincer.transform.localPosition;
        }
        else
        {
            Debug.LogError("Pincer is Null");
        }
    }

    /// <summary>
    /// 인형뽑기 초기화
    /// </summary>
    public void QuestionInit()
    {
        princessClawUI.SetActive(true);

        questionNum = new bool[princessObj.Length];

        //UI가 켜졌을 시,공주들의 앵커를 꺼줌
        for (int i = 0; i < princessObj.Length; i++)
        {
            questionNum[i] = false;
            princessObj[i].GetComponent<BoxCollider2D>().enabled = true;
        }
                
        currentlyProblem = 0;
        btnRestart.SetActive(false);
        answer.SetActive(false);
        wrongAnswer.SetActive(false);

        ClawStart();
    }

    /// <summary>
    /// 인형뽑기 시작
    /// </summary>
    private void ClawStart()
    {
        //문제 출제 최대횟수를 초과하지 않았을경우
        if (currentlyProblem < 5)
        {
            while (true)
            {
                int i = Random.Range(0, questionNum.Length - 1);

                //출제되지 않은 인형일 경우
                if (questionNum[i] == false)
                {
                    findPrincess.GetComponent<UISprite>().spriteName = "Princess0" + (1 + i);
                    rightAnswerNum = i;
                    questionText.text = string.Format(LocalizeText.Value["ClowQuestion"]);
                    questionNum[i] = true;
                    break;
                }
            }
        }
        else
        {
            for(int i=0; i<princessObj.Length; i++)
            {
                princessObj[i].GetComponent<BoxCollider2D>().enabled = false;
            }

            isStart = false;
            btnRestart.SetActive(true);

            questionText.text = string.Format(LocalizeText.Value["ClowGameOver"]);
        }
    }

    public void pickedPrincess(GameObject pickedObj)
    {
        isStart = true;

        //집게가 이동중이지 않을경우.
        if (isMoving == false)
        {
            for (int i = 0; i < princessObj.Length; i++)
            {
                //선택한 공주의 번호와 오브젝트를 저장
                if (pickedObj == princessObj[i])
                {
                    choiceNum = i;
                    currentlyPrincess = princessObj[i];
                }
                //선택한 공주를 제외한 공주를 전부 터치가 안되게끔 변경
                else
                {
                    princessObj[i].GetComponent<BoxCollider2D>().enabled = false;
                }
            }

            if (currentlyPrincess != null)
            {
                //선택한 공주쪽으로 집게 이동 시작.
                movePincerPosition(currentlyPrincess.transform.localPosition.x, pincerPos.y, true);
            }

            isMoving = true;
        }
    }

    /// <summary>
    /// 집게의 위치를 특정시킨 위치로 이동시킵니다.
    /// </summary>
    /// <param name="xPoint">이동할 x좌표</param>
    /// <param name="yPoint">이동할 y좌표</param>
    /// <param name="whatAxis">몇번째 이동중인지 true : 첫번째 false : 두번째</param>
    private void movePincerPosition(float xPoint, float yPoint, bool whatMove)
    {
        //이동시킬 위치값을 tween에 지정
        pincer.GetComponent<TweenPosition>().to = new Vector3(xPoint, yPoint, pincerPos.z);

        ClawSpeedCalc();

        //이동 시작
        pincer.GetComponent<TweenPosition>().ResetToBeginning();
        pincer.GetComponent<TweenPosition>().PlayForward();

        moveAxis = whatMove;
    }

    /// <summary>
    /// Tween의 이동이 끝났을때 발생할 이벤트 메서드
    /// </summary>
    public void PincerMoveFinished()
    {
        if (isStart == true)
        {
            //현재 위치를 tween의 처음위치로 지정
            pincer.GetComponent<TweenPosition>().from = pincer.GetComponent<TweenPosition>().to;

            #region Y축 이동부분
            if (moveAxis)
            {
                //돌아가는 중 이라면
                if (returnMove)
                {
                    //선택한 인형이 정답일 경우
                    if (rightAnswerNum == choiceNum)
                    {
                        //다른 인형보다 depth를 앞으로 떙김.
                        currentlyPrincess.GetComponent<UISprite>().depth = 6;

                        //이동중 터치 안되게 변경
                        currentlyPrincess.GetComponent<BoxCollider2D>().enabled = false;

                        //되돌아갈 수 있도록 인형의 원래 위치를 저장
                        currentlyPrincessPos = currentlyPrincess.transform.localPosition;

                        //집게와 같이 움직이도록 집게의 자식으로 이동
                        currentlyPrincess.transform.parent = pincer.transform;
                    }

                    movePincerPosition(pincer.transform.localPosition.x, pincerPos.y, false);
                }
                else
                {
                    movePincerPosition(pincer.transform.localPosition.x, currentlyPrincess.transform.localPosition.y, true);
                    returnMove = true;
                }
            }
            #endregion

            #region X축 이동부분
            else
            {
                //돌아가는 중 이라면
                if (returnMove)
                {
                    movePincerPosition(pincerPos.x, pincer.transform.localPosition.y, false);
                    returnMove = false;
                }
                else
                {
                    //선택한 인형이 정답일 경우
                    if (rightAnswerNum == choiceNum)
                    {
                        questionText.text = string.Format(LocalizeText.Value["ClowSelectAnswer"]);
                        answer.SetActive(true);
                    }
                    else
                    {
                        questionText.text = string.Format(LocalizeText.Value["ClowSelectWrong"]);
                        wrongAnswer.SetActive(true);
                    }

                    Invoke("NextQuestionReady", 2.0f);
                }
            }
            #endregion
        }
    }

    /// <summary>
    /// 트리거로 인한 되돌아가기 시작
    /// </summary>
    public void TriggerReturnStart()
    {
        moveAxis = true;
        returnMove = true;

        // 현재위치를 집게의 시작위치로 변경
        pincer.GetComponent<TweenPosition>().from = pincer.transform.localPosition;

        //인형머리 위쪽으로 집게의 위치를 이동
        pincer.GetComponent<TweenPosition>().to = new Vector3(pincer.transform.localPosition.x, 
            currentlyPrincess.transform.localPosition.y - (currentlyPrincess.GetComponent<UISprite>().localSize.y / dollRatio)  , 
            pincer.transform.localPosition.z);

        //인형을 집을떄의 속도 입니다(이건 집게이동속도와 달리 직접 수정해야함)
        pincer.GetComponent<TweenPosition>().duration = 0.5f;

        pincer.GetComponent<TweenPosition>().ResetToBeginning();
        //pincer.GetComponent<TweenPosition>().PlayForward();
    }

    /// <summary>
    /// 다음 문제 출제 준비
    /// </summary>
    private void NextQuestionReady()
    {
        //인형이 터치가능하도록 변경
        for (int i = 0; i < princessObj.Length; i++)
        {
            princessObj[i].GetComponent<BoxCollider2D>().enabled = true;
        }

        isMoving = false;

        if (rightAnswerNum == choiceNum)
        {
            //집게를 행거의 밑으로 되돌림.
            currentlyPrincess.transform.parent = hanger.transform;

            //원래 위치로 되돌림.
            currentlyPrincess.transform.localPosition = currentlyPrincessPos;

            //뎁스값 원상복구
            currentlyPrincess.GetComponent<UISprite>().depth = 5;

            answer.SetActive(false);

            currentlyProblem += 1;
            rightAnswerNum = -1;

            ClawStart();
        }
        else
        {
            questionText.text = string.Format(LocalizeText.Value["ClowQuestion"]);
            wrongAnswer.SetActive(false);
        }

        choiceNum = -1;
    }

    /// <summary>
    /// 가로 세로를 이동할때 동일한 속도도 집게가 움직일 수있도록 속도를 계산해 줍니다.
    /// </summary>
    private void ClawSpeedCalc()
    {
        if (moveAxis)
        {
            pincer.GetComponent<TweenPosition>().duration = Mathf.Abs((pincer.GetComponent<TweenPosition>().from.y - pincer.GetComponent<TweenPosition>().to.y)/ clawVelocity);
        }
        else
        {
            pincer.GetComponent<TweenPosition>().duration = Mathf.Abs((pincer.GetComponent<TweenPosition>().from.x - pincer.GetComponent<TweenPosition>().to.x)/ clawVelocity);
        }
    }

    public void RestartBtnClick()
    {
        questionText.text = string.Format(LocalizeText.Value["ClowRestart"]);

        Invoke("QuestionInit", 2.0f);
    }

    public void ClawGameQuit()
    {
        for (int i = 0; i < princessObj.Length; i++)
        {
            princessObj[i].GetComponent<BoxCollider2D>().enabled = false;
        }

        isStart = false;

        TargetManager.타깃메니저.HideAllModelingContents();
        MainUI.메인.공주인형뽑기UI.SetActive(false);        
        RotateUI.회전.회전UI_숨기기();

        //MainUI.메인.인식글자UI.SetActive(true);
    }
}