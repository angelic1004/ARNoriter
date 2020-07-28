using UnityEngine;
using System.Collections;

public class MazeGameUI : MonoBehaviour
{
    /// <summary>
    /// 버튼 UI
    /// </summary>
    public GameObject btnUI;

    /// <summary>
    /// Start Btn
    /// </summary>
    public GameObject startBtn;

    /// <summary>
    /// Return Button
    /// </summary>
    public GameObject returnBtn;

    /// <summary>
    /// Minimap Active Button
    /// </summary>
    public GameObject minimapBtn;

    /// <summary>
    /// Minimap Close Button
    /// </summary>
    public GameObject closeBtn;

    /// <summary>
    /// 갈림길 Front 화살표
    /// </summary>
    public GameObject frontArrow;

    /// <summary>
    /// 갈림길 좌측 화살표
    /// </summary>
    public GameObject leftArrow;

    /// <summary>
    /// 갈림길 우측 화살표
    /// </summary>
    public GameObject rightArrow;

    /// <summary>
    /// 갈림길 되돌아가기 화살표
    /// </summary>
    public GameObject returnArrow;

    /// <summary>
    /// 결과창 UI
    /// </summary>
    public GameObject resultUI;

    /// <summary>
    /// 다음레벨 버튼
    /// </summary>
    public GameObject nextLevelBtn;

    /// <summary>
    /// 일시정지 버튼
    /// </summary>
    public GameObject pauseBtn;

    /// <summary>
    /// 일시정지 UI
    /// </summary>
    public GameObject pauseUI;

    /// <summary>
    /// 현재 상태를 알려주는 Label
    /// </summary>
    public UILabel ExplanationLabel;

    /// <summary>
    /// 미니맵 2D 오브젝트
    /// </summary>
    public GameObject minimapPos;

    /// <summary>
    /// 미니맵을 그릴 Canvas
    /// </summary>
    public GameObject minimapCanvas;

    /// <summary>
    /// 미니맵에 그려지는 Texture를 받는 오브젝트
    /// </summary>
    public UnityEngine.UI.RawImage minimapRawImage;

    /// <summary>
    /// 미니맵에 그려지는 Texture
    /// </summary>
    public RenderTexture renderTexture;

    /// <summary>
    /// 현재 상태창에 사용할 텍스트
    /// </summary>
    private string usingText;

    private string usingLocalizeValue;

    public static MazeGameUI instance;

    void Awake()
    {
        instance = this;

        ExplanationLabel.text = LocalizeText.Value["introText"];
    }

    void Start()
    {
        UICheck(btnUI, true);
        UICheck(startBtn, true);
        UICheck(minimapBtn, true);
        UICheck(returnBtn, false);
        UICheck(resultUI, false);
        UICheck(closeBtn, false);
        UICheck(pauseBtn, true);
        UICheck(pauseUI, false);
    }

    /// <summary>
    /// UI상태를 체크하고 없다면 에러메세지를 띄움
    /// </summary>
    /// <param name="uiObj"></param>
    /// <param name="active"></param>
    private void UICheck(GameObject uiObj, bool active)
    {
        if (uiObj != null)
        {
            uiObj.SetActive(active);
        }
        else
        {
            Debug.LogError("uiObj is NULL : " + uiObj);
        }
    }

    /// <summary>
    /// 미니맵 열기
    /// </summary>
    public void MinimapOpen()
    {
        minimapPos.SetActive(true);
        closeBtn.SetActive(true);
        minimapBtn.SetActive(false);
        btnUI.SetActive(false);
    }

    /// <summary>
    /// 미니맵 닫기
    /// </summary>
    public void MinimapClose()
    {
        closeBtn.SetActive(false);
        minimapPos.SetActive(false);
        minimapBtn.SetActive(true);
        btnUI.SetActive(true);
    }

    /// <summary>
    /// 막다른길에서 돌아가기 버튼 터치이벤트
    /// </summary>
    public void ReturnBtnTouchEvent()
    {
        returnBtn.SetActive(false);
        MazeGameManager.instance.ReturnPreRoad();
    }

    /// <summary>
    /// 시작버튼 터치 이벤트
    /// </summary>
    public void StartBtnTouchEvent()
    {
        startBtn.SetActive(false);

        StartCoroutine("GameStartBtnEvent");
    }

    /// <summary>
    /// 게임 시작 이벤트
    /// </summary>
    /// <returns></returns>
    private IEnumerator GameStartBtnEvent()
    {
        usingText = LocalizeText.Value["startText"];
        yield return StartCoroutine("InputExplanationTextStart");

        yield return new WaitForSeconds(1.0f);

        MazeGameManager.instance.GameStart();
    }

    /// <summary>
    /// 현재상태 Text 변경실행
    /// </summary>
    /// <param name="selectText"></param>
    public void InputExplanationText(string selectText)
    {
        usingLocalizeValue = selectText;
        StopCoroutine("InputExplanationTextStart");
        usingText = LocalizeText.Value[usingLocalizeValue];
        StartCoroutine("InputExplanationTextStart");
    }

    /// <summary>
    /// 현재상태 Text 변경
    /// </summary>
    /// <returns></returns>
    private IEnumerator InputExplanationTextStart()
    {
        ExplanationLabel.gameObject.GetComponent<LocalizeText>().enabled = false;

        for (int i = 0; i <= usingText.Length; i++)
        {
            ExplanationLabel.text = usingText.Substring(0, i);
            yield return new WaitForSeconds(0.1f);
        }

        ExplanationLabel.gameObject.GetComponent<LocalizeText>().ValueName = usingLocalizeValue;
        ExplanationLabel.gameObject.GetComponent<LocalizeText>().enabled = true;

    }

    /// <summary>
    /// 방향버튼을 한번에 켜고 끌수 있는 매서드
    /// </summary>
    /// <param name="active"></param>
    public void ArrowSetActiveAll(bool active)
    {
        frontArrow.SetActive(active);
        leftArrow.SetActive(active);
        rightArrow.SetActive(active);
        returnArrow.SetActive(active);
    }

    /// <summary>
    /// 게임 종료 버튼 터치
    /// </summary>
    public void QuitBtnEvent()
    {
        MainUI.메인.딜레이팝업UI.SetActive(true);
        resultUI.SetActive(false);
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(string.Format("{0}_mazegame", GlobalDataManager.m_ResourceFolderEnum.ToString().ToLower()));
    }

    /// <summary>
    /// 다시하기 버튼 터치
    /// </summary>
    public void RetryBtnEvent()
    {
        StopCoroutine("InputExplanationTextStart");
        resultUI.SetActive(false);

        StartCoroutine("RetryStart");
    }

    /// <summary>
    /// 다시하기 시작
    /// </summary>
    /// <returns></returns>
    private IEnumerator RetryStart()
    {
        usingText = LocalizeText.Value["retryText"];
        yield return StartCoroutine("InputExplanationTextStart");

        ExplanationLabel.text = LocalizeText.Value["introText"];

        MazeGameManager.instance.GameRetry();
    }

    /// <summary>
    /// 다음레벨 버튼 터치
    /// </summary>
    public void NextLevelBtnEvent()
    {
        MainUI.메인.딜레이팝업UI.SetActive(true);
        resultUI.SetActive(false);

        MazeGameManager.instance.NextLevel();
    }

    /// <summary>
    /// 미니맵 초기화 call 함수
    /// </summary>
    public void MinimapInitCall()
    {
        Invoke("MinimapInit", 0.1f);
    }

    /// <summary>
    /// 미니맵 관련 초기화
    /// </summary>
    private void MinimapInit()
    {
        minimapPos.SetActive(true);

        //미니맵의 크기 지정
        Vector2 sizeSave = new Vector2(minimapPos.GetComponent<UISprite>().localSize.x, minimapPos.GetComponent<UISprite>().localSize.y);
        minimapCanvas.GetComponent<RectTransform>().sizeDelta = sizeSave;

        //미니맵을 로컬좌표 zero포지션으로
        minimapCanvas.GetComponent<RectTransform>().localPosition = Vector3.zero;

        minimapRawImage.rectTransform.sizeDelta = minimapCanvas.GetComponent<RectTransform>().sizeDelta;

        minimapRawImage.uvRect = MazeGameManager.instance.map.GetComponent<MazeInfo>().minimapRawImgRect;

        //minimapRawImage.uvRect = driveData.track.GetComponent<RacingTrackInfo>().rawImgRect;

        MazeGameManager.instance.MoveObjMinimapSet();

        //미니맵 카메라의 위치,각도 조정
        //minimapCamera.SetActive(true);
        //minimapCamera.GetComponent<Camera>().targetTexture = renderTexture;
        //minimapCamera.transform.parent = driveData.track.transform;
        //minimapCamera.transform.localPosition = Vector3.zero;
        //minimapCamera.transform.localPosition = new Vector3(0, 45, 0);
        //minimapCamera.transform.LookAt(driveData.track.transform);
        //minimapCamera.transform.localEulerAngles += (Vector3.up * -90);


        minimapPos.SetActive(false);
        MainUI.메인.딜레이팝업UI.SetActive(false);
    }

    /// <summary>
    /// 일시정지 UI Open
    /// </summary>
    public void PauseUIOpen()
    {
        pauseUI.SetActive(true);
        pauseBtn.GetComponent<BoxCollider2D>().enabled = false;

        minimapPos.SetActive(false);
        closeBtn.SetActive(false);
        minimapBtn.SetActive(true);
        minimapBtn.GetComponent<BoxCollider2D>().enabled = false;
        btnUI.SetActive(false);
    }

    /// <summary>
    /// 일시정지 UI Close
    /// </summary>
    public void PauseUIClose()
    {
        pauseUI.SetActive(false);
        pauseBtn.GetComponent<BoxCollider2D>().enabled = true;
        minimapBtn.SetActive(true);
        minimapBtn.GetComponent<BoxCollider2D>().enabled = true;
        btnUI.SetActive(true);
    }
}
