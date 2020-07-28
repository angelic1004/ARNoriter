using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RunwayManager : MonoBehaviour
{
    public static RunwayManager Instance;

    private static int RUNWAY_PATH_COUNT = 6;                   // 런웨이 경로 개수
    private static int TOTAL_TARGET_COUNT = 10;                 // 전체 타겟 개수
    private static int HUMAN_MATERIAL_COUNT = 4;                // 머터리얼 개수

    private float HUMAN_MOVE_SPEED = 2.5f;                      // 런웨이 이동 속도
    

    private GameObject[] leftPath;                              // 왼쪽 이동경로
    private GameObject[] rightPath;                             // 오른쪽 이동경로
    private GameObject leftHuman, rightHuman;

    private float moveSpeed;
    private float rotationSpeed = 2f;

    private Animator leftModelAni, rightModelAni;
    private int leftHumanSavedColorCount, rightHumanSavedColorCount = 0;

    //public List<Material> leftHumanMaterial, rightHumanMaterial;
    private Material[] leftHumanMaterial, rightHumanMaterial;

    [HideInInspector]
    public int[] recognizedTargetNumbers;                      // 인식된 타겟번호들

    public int recognizedTargetIndex;                           // 인식된 타겟 인덱스
    public int currentRunwayNum;                                // 현재 런웨이 인덱스 번호
    private RunwayMinimapInfo runwayInfo;

    private int[] savedColorKeyCount;                           // 저장된 컬러 키값 카운트 (여러 색상 저장됐을 경우 바꿔주기 위함)

    public bool isRunwayRunning;

    private int currentBgmIndex = -1;    

    void Awake()
    {
        Instance = this;

        InitRunwayManager();
        isRunwayRunning = false;
    }

    void Start()
    {
        StartCoroutine(LoopPlayBGM());
    }

    private void InitRunwayManager()
    {
        recognizedTargetIndex = 0;
        currentRunwayNum = 0;

        recognizedTargetNumbers = new int[TOTAL_TARGET_COUNT];
        savedColorKeyCount = new int[TOTAL_TARGET_COUNT];

        leftPath = new GameObject[RUNWAY_PATH_COUNT];
        rightPath = new GameObject[RUNWAY_PATH_COUNT];

        leftHumanMaterial = new Material[HUMAN_MATERIAL_COUNT];
        rightHumanMaterial = new Material[HUMAN_MATERIAL_COUNT];

        ResetRecognizedTargetNumbers();
    }

    public void ResetRunwayManager()
    {
        recognizedTargetIndex = 0;
        currentRunwayNum = 0;

        ResetRecognizedTargetNumbers();
    }

    private void ResetRecognizedTargetNumbers()
    {
        // -1로 초기화
        for (int i = 0; i < recognizedTargetNumbers.Length; i++)
        {
            recognizedTargetNumbers[i] = -1;
        }
    }

    private IEnumerator MoveCharacter(GameObject human, GameObject[] path, Animator ani, bool isLeftSideCharacter)
    {
        int wayPointNum = 0;
        float distance;
        float speed = HUMAN_MOVE_SPEED;

        AnimatorStateInfo animStateInfo = ani.GetCurrentAnimatorStateInfo(0);
        Quaternion rotation;

        while (wayPointNum < path.Length - 1)
        {
            yield return new WaitForEndOfFrame();

            distance = Vector3.Distance(human.transform.position, path[wayPointNum].transform.position);
            rotation = Quaternion.LookRotation(path[wayPointNum].transform.position - human.transform.position);

            human.transform.rotation = Quaternion.Slerp(human.transform.rotation, rotation, Time.deltaTime * rotationSpeed);
            human.transform.position += human.transform.forward * speed * Time.deltaTime;

            if (wayPointNum == 0)
            {
                animStateInfo = ani.GetCurrentAnimatorStateInfo(0);

                if (!animStateInfo.IsName("walk"))
                {
                    ani.SetTrigger("walk");
                }
            }

            //Debug.Log("Speed: " + speed);

            // 정지했다 움직일때 자연스럽게 속도 올려줌
            if (speed < HUMAN_MOVE_SPEED)
            {
                speed += 0.02f;
            }
            else if (speed > HUMAN_MOVE_SPEED + 0.02f)
            {
                speed -= 0.02f;
            }

            //Debug.Log("distance : " + distance);

            if (distance < 0.7 && wayPointNum < path.Length - 1)
            {
                //Debug.Log("index: " + wayPointNum);

                if (wayPointNum != 2)
                {
                    wayPointNum++;
                }
                else
                {
                    speed = 0.2f;

                    if (!animStateInfo.IsName("idle"))
                    {
                        ani.SetTrigger("idle");
                    }

                    yield return new WaitForSeconds(7.0f);

                    wayPointNum++;

                    if(!animStateInfo.IsName("walk"))
                    { 
                        ani.SetTrigger("walk");
                    }

                    // 인식된 타겟이 2개 이상일 때
                    if (recognizedTargetNumbers[1] > -1)
                    {
                        // 왼쪽 사이드 캐릭터인지 체크
                        if (isLeftSideCharacter)
                        {
                            ChangeCurrentRunwayNum();
                            ChangeRunwayModel(recognizedTargetNumbers[currentRunwayNum], false);

                            
                            // 오른쪽 캐릭터 움직임
                            StartCoroutine(MoveCharacter(rightHuman, rightPath, rightModelAni, false));
                        }
                        else
                        {
                            ChangeCurrentRunwayNum();
                            ChangeRunwayModel(recognizedTargetNumbers[currentRunwayNum], true);

                            // 왼쪽 캐릭터 움직임
                            StartCoroutine(MoveCharacter(leftHuman, leftPath, leftModelAni, true));
                        }
                    }
                }
            }
        }

        ani.SetTrigger("idle");

        // 인식된 타겟이 1개 뿐일때
        if (recognizedTargetNumbers[1] == -1)
        {
            ResetMoveCoroutine();


            StartCoroutine(MoveCharacter(leftHuman, leftPath, leftModelAni, true));
        }
        else
        {
            human.SetActive(false);
        }
    }

    private void ResetMoveCoroutine()
    {
        StopAllCoroutines();

        if (leftHuman != null)
        {
            leftHuman.transform.localPosition = leftPath[0].transform.localPosition;
        }

        if(rightHuman != null)
        {
            rightHuman.transform.localPosition = rightPath[0].transform.localPosition;
        }
    }

    private void ChangeCurrentRunwayNum()
    {
        currentRunwayNum++;

        if (currentRunwayNum == recognizedTargetIndex)
        {
            currentRunwayNum = 0;
        }
    }

    /// <summary>
    /// 공주 색상 정보를 딕셔너리로부터 로드
    /// </summary>
    public void LoadPrincessColorInfo(int targetIndex, GameObject targetModel, Material[] bodyMat)
    {
        // TODO : targetModel의 머터리얼 정보 읽어오는 부분

        int colorCount = savedColorKeyCount[targetIndex];

        int keyValue = targetIndex * 10 + colorCount;

        //Debug.Log("KeyValue : " + keyValue);

        // 키값을 가지고 있는지 체크
        bool containsKey = PrincessPaintManager.Instance.CheckPrincessColorDicKey(keyValue);

        if(!containsKey)
        {
            Debug.Log("Key값 없음");
            colorCount = 0;
            savedColorKeyCount[targetIndex] = colorCount;

            keyValue = targetIndex * 10;
        }

        List<Color32> bodyColorList = PrincessPaintManager.Instance.savePrincessColorDic[keyValue];

        for (int i = 0; i < bodyColorList.Count; i++)
        {
            // 공주 모델링에 저장된 색상 적용

            // 모델링 머터리얼에 색상 적용
            bodyMat[i].SetColor("_Color", bodyColorList[i]);
        }

        colorCount++;

        if (savedColorKeyCount[targetIndex] < 2)
        {
            savedColorKeyCount[targetIndex] = colorCount;
        }
        else
        {
            savedColorKeyCount[targetIndex] = 0;
        }
    }

    public void LoadHumanBodyMaterial(int index, GameObject obj, ref Material[] mat)
    {
        Material[] bodyMat = obj.GetComponent<RunwayHumanInfo>().bodyMaterial;

        for (int i = 0; i < bodyMat.Length; i++)
        {
            //leftHumanMaterial.Add(obj.GetComponent<RunwayHumanInfo>().bodyMaterial[i]);
            mat[i] = bodyMat[i];
        }

        //for(int i =0; i< bodyMat.Length; i++)
        //{
        //    leftHumanMaterial.Add(bodyMat[i]);
        //}
    }


    /// <summary>
    /// 실사 런웨이를 시작합니다. (외부 호출용)
    /// </summary>
    public IEnumerator StartRunwayReal()
    {
        PlayRunwaySound();

        int targetNum = recognizedTargetNumbers[0];

        for (int i = 0; i < runwayInfo.leftWayPoints.Length; i++)
        {
            leftPath[i] = runwayInfo.leftWayPoints[i];
            rightPath[i] = runwayInfo.rightWayPoints[i];
        }

        yield return new WaitForSeconds(3.0f);

        //Debug.Log("ISRUNWAY RUN: " + isRunwayRunning);

        if (isRunwayRunning)
        {
            ChangeRunwayModel(targetNum, true);


            StartCoroutine(MoveCharacter(leftHuman, leftPath, leftModelAni, true));
        }
    }

    public void PlayBackgroundMusic(AudioClip clip, bool isLoop)
    {
        // 배경음 재생
        DanceAudioManager.getInstance.PlayBackgroundSound(clip, isLoop);
    }

    public void PlayBGM()
    {
        if (runwayInfo == null)
        {
            return;
        }

        AudioClip bgmSound  = null;

        if (currentBgmIndex == (runwayInfo.bgmSound.Length - 1))
        {
            currentBgmIndex = 0;
        }
        else
        {
            currentBgmIndex = currentBgmIndex + 1;
        }

        bgmSound            = runwayInfo.bgmSound[currentBgmIndex];

        PlayBackgroundMusic(bgmSound, false);
    }

    private void PlayRunwaySound()
    {
        AudioClip runwaySound = runwayInfo.runwaySound;
        PlayBackgroundMusic(runwaySound, true);
    }

    public void StopRunwayBgm()
    {
        DanceAudioManager.getInstance.StopBackgroundSound();
    }

    public void GetRunwayInfo()
    {
        runwayInfo = MiniMapManager.instance.miniMapModeling.GetComponent<RunwayMinimapInfo>();
    }

    public IEnumerator LoopPlayBGM()
    {
        while (true)
        {
            if (DanceAudioManager.getInstance.IsBackgroundSoundPlaying() == false && isRunwayRunning == false)
            {
                PlayBGM();
            }
            
            yield return new WaitForSeconds(0.2f);
        }
    }

    /// <summary>
    /// 미니맵 스케일 다운일때 미니맵 리셋
    /// </summary>
    public void MinimapScaleDown()
    {
        Debug.Log("MinimapScaleDown()");

        StopAllCoroutines();

        if (leftHuman != null)
        {
            leftHuman.SetActive(false);

            leftHuman = null;
        }

        if (rightHuman != null)
        {
            rightHuman.SetActive(false);

            rightHuman = null;
        }
        
        currentRunwayNum = 0;

        ResetHumanObject();
        ResetMoveSpeed();

        //PlayBGM();          // BGM 사운드 재생
    }

    public void ResetRunway()
    {
        StopAllCoroutines();

        if (leftHuman != null)
        {
            leftHuman.SetActive(false);

            leftHuman = null;
        }

        if (rightHuman != null)
        {
            rightHuman.SetActive(false);

            rightHuman = null;
        }
        
        currentRunwayNum = 0;

        ResetMoveSpeed();
    }

    private void ResetHumanObject()
    {
        if(leftHuman != null)
        {
            leftHuman.SetActive(false);
        }

        if(rightHuman != null)
        {
            rightHuman.SetActive(false);
        }
    }

    private void ChangeRunwayModel(int targetNum, bool isLeftSide)
    {
        runwayInfo.princessObj[targetNum].SetActive(true);
        GlobalDataManager.ShaderRefresh(runwayInfo.princessObj[targetNum]);

        if (isLeftSide)
        {
            leftHuman = runwayInfo.princessObj[targetNum];
            leftModelAni = runwayInfo.princessObj[targetNum].GetComponent<Animator>();
            leftHuman.transform.localPosition = leftPath[0].transform.localPosition;
        }
        else
        {
            rightHuman = runwayInfo.princessObj[targetNum];
            rightModelAni = runwayInfo.princessObj[targetNum].GetComponent<Animator>();
            rightHuman.transform.localPosition = rightPath[0].transform.localPosition;
        }
    }

    /// <summary>
    /// 런웨이매니저 업데이트 (외부 호출용)
    /// </summary>
    public void UpdateRunwayRealManager(int targetNumber)
    {
        SaveRecognizedTargetNumber(targetNumber);
    }

    /// <summary>
    /// 인식된 타겟 번호를 저장함
    /// </summary>
    /// <param name="number">인식된 타겟 번호</param>
    private void SaveRecognizedTargetNumber(int number)
    {
        bool alreadyHaveNum = false;

        // 중복검사
        for (int i = 0; i < recognizedTargetIndex; i++)
        {
            if(recognizedTargetNumbers[i] == number)
            {
                alreadyHaveNum = true;
            }
        }

        if (!alreadyHaveNum)
        {
            recognizedTargetNumbers[recognizedTargetIndex] = number;
            recognizedTargetIndex++;
        }
    }

    /// <summary>
    /// 사람의 움직임 속도 제어
    /// </summary>
    public void HumanMoveSpeedControl(bool isSpeedUp)
    {
        if (isSpeedUp)
        {
            HUMAN_MOVE_SPEED += 0.25f;
        }
        else
        {
            HUMAN_MOVE_SPEED -= 0.25f;
        }

        Debug.Log("Move Speed : " + HUMAN_MOVE_SPEED);
    }

    /// <summary>
    /// 사람 움직임 속도 초기화 (참조 : InitMiniMapRotate()에서 참조)
    /// </summary>
    public void ResetMoveSpeed()
    {
        HUMAN_MOVE_SPEED = 2.5f;
    }

    public IEnumerator StartRunwayEvent()
    {
        while (MiniMapManager.instance.miniMapModeling == null)
        {
            yield return new WaitForFixedUpdate();
        }

        GetRunwayInfo();
        //PlayBGM();

        ResetRunwayTextures();
    }

    public void OnClickPrincessFaceButton(GameObject btn)
    {
        int clickNum = FindPrincessTargetNum(btn);

        ResetMoveCoroutine();
        ResetHumanObject();

        currentRunwayNum = clickNum;
        ChangeRunwayModel(recognizedTargetNumbers[currentRunwayNum], true);

        StartCoroutine(MoveCharacter(leftHuman, leftPath, leftModelAni, true));

        GameObject btnOpen = PrincessUI.princessInstance.btnOpen;
        btnOpen.GetComponent<UIPlayTween>().Play(false);
    }

    private int FindPrincessTargetNum(GameObject obj)
    {
        int targetNum = 0;

        for (int i = 0; i < runwayInfo.princessObj.Length; i++)
        {
            if (obj.name == runwayInfo.princessObj[i].name)
            {
                targetNum = i;
                break;
            }
        }

        int currentNum = 0;

        for (int j = 0; j < recognizedTargetIndex; j++)
        {
            if (targetNum == recognizedTargetNumbers[j])
            {
                currentNum = j;
            }
        }

        return currentNum;
    }

    public void ResetRunwayTextures()
    {
        for (int i = 0; i < runwayInfo.princessObj.Length; i++)
        {
            ResetTexture(runwayInfo.princessObj[i], false);
        }
        
    }

    private void ResetTexture(GameObject obj, bool usedRunwayPaint)
    {
        RunwayHumanInfo humanInfo = obj.GetComponent<RunwayHumanInfo>();

        for(int i = 0; i< humanInfo.bodyMaterial.Length; i++)
        {
            if (usedRunwayPaint)
            {
                humanInfo.bodyMaterial[i].mainTexture = humanInfo.paintTexture[i];
            }
            else
            {
                humanInfo.bodyMaterial[i].mainTexture = humanInfo.realTexture[i];
            }
        }
    }
}