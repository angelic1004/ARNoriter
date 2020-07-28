//#define USE_QRTHREAD // 기존 쓰레드 방식 사용하려면 주석 해제

using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Vuforia;
using ZXing;

public class QRCodeUI : MonoBehaviour
{
    [Serializable]
    public class QRCodeForScenes
    {
        public int 구분번호;
        public string 씬이름;
    }

    public GameObject ARCamera = null;
    public GameObject 뒤로가기;
    public GameObject 메뉴UI;
    public GameObject 메인하위UI;
    private bool 메인하위UI상태 = false;

    public UILabel QR코드스캔_텍스트;
    public GameObject 시리얼입력팝업;
    public UIInput[] 시리얼입력필드;
    public GameObject 결과팝업;
    public UILabel 결과팝업_텍스트;
    public GameObject 종료팝업;
    public UIPanel 도움말;
    private bool 도움말시작 = false;

    public string 시리얼체크_URL = "http://ssl.hansapp.kr/CheckSerial.aspx";

    public bool QR코드_씬설정_사용 = false;
    public QRCodeForScenes[] QR코드_씬설정;

    private string QR구분번호 = string.Empty;

    public static string 씬이름 = string.Empty;
    public static int 메뉴인덱스1단계 = -1;
    public static bool 복합인증사용 = false;
    public static uint 필수부가기능 = 0;

    private int oldResultCode;
    private uint oldResultExtra;

    private const string QRCODE_SERIAL_KEY = "qrcode_serial";
    private const string QRCODE_RESULT_KEY = "qrcode_result";
    private const string QRCODE_RESULT_EXTRA_KEY = "qrcode_result_extra";

    private bool cameraInitialized = false;
    private Image.PIXEL_FORMAT pixelFormat = Image.PIXEL_FORMAT.GRAYSCALE;
    private BarcodeReader barCodeReader;

#if USE_QRTHREAD
    private Thread qrThread;
#else
    private bool bScanning = false;
    private Coroutine _scanCoroutine = null;
#endif
    private bool bScanEnd = false;
    private bool bReadyWWW = false;
    private bool bRequestWWW = false;
    private Result _qrResult = null;


    // add by N.C Park
    public static bool needScanning = true;
    public static string backupSceneName = string.Empty;

    private const string MAIN_SCENE_NAME = "01. HansMain";

    void Awake()
    {
        if (ARCamera == null)
        {
            Debug.LogError("ARCamera not found!");
        }

        if (씬이름_체크())
        {
            /*
            if (뒤로가기 != null)
            {
                if (string.IsNullOrEmpty(QR구분번호))
                {
                    뒤로가기.SetActive(false);
                }
            }
            */

            UI_상태변경(메인하위UI, false);

            // 기존에 스캔한 qr코드 결과값이 없거나 필수부가기능이 없는 경우
            oldResultCode = PlayerPrefs.GetInt(QRCODE_RESULT_KEY + QR구분번호, 0);
            oldResultExtra = (uint)PlayerPrefs.GetInt(QRCODE_RESULT_EXTRA_KEY + QR구분번호, 0);
            if (oldResultCode < 1 || (복합인증사용 && (oldResultExtra & 필수부가기능) == 0))
            {
                // AR카메라 활성화하고
                ARCamera.SetActive(true);
                // QR코드 스캐너 초기화 시키고
                StartCoroutine(InitializeCamera());
                // 쓰레드 시작한다.
                StartThread();
            }
            else
            {
                LoadNextScene();
            }
        }

        씬이름 = string.Empty;
    }

    void Start()
    {
        if (QR코드스캔_텍스트 != null)
        {
            QR코드스캔_텍스트.text = LocalizeText.Value["ScanQR"];
        }
        시리얼입력팝업.SetActive(false);
        결과팝업.SetActive(false);
        종료팝업.SetActive(false);

        Invoke("도움말띄우기", 1.0f);        
    }

    private bool 씬이름_체크()
    {
        if (!string.IsNullOrEmpty(씬이름))
        {
            if (QR코드_씬설정_사용)
            {
                for (int i = 0; i < QR코드_씬설정.Length; i++)
                {
                    if (string.Compare(씬이름, QR코드_씬설정[i].씬이름) == 0)
                    {
                        if (QR코드_씬설정[i].구분번호 > 0)
                        {
                            QR구분번호 = string.Format("#{0}", QR코드_씬설정[i].구분번호);
                            Debug.Log("QRCodeUI 들어옴 : " + QR구분번호);
                        }
                        else
                        {
                            QR구분번호 = string.Empty;
                            Debug.Log("QRCodeUI 들어옴");
                        }
                        return true;
                    }
                }
            }

            Debug.Log("QRCodeUI 들어옴 : 씬설정을 사용안하거나 등록되어있지 않은 씬이므로 건너뜀");            
            SceneManager.LoadScene(MAIN_SCENE_NAME);

            //SceneManager.LoadScene("02. Update");
            return false;
        }
        else if (메뉴인덱스1단계 != -1)
        {
            QR구분번호 = string.Format("#{0}", 메뉴인덱스1단계 + 1);
            Debug.Log("QRCodeUI 들어옴 : " + QR구분번호);
            return true;
        }

        QR구분번호 = string.Empty;
        Debug.Log("QRCodeUI 들어옴");
        return true;
    }

    public IEnumerator InitializeCamera()
    {
        barCodeReader = new BarcodeReader();

        // Waiting a little seem to avoid the Vuforia's crashes.
        yield return new WaitForSeconds(1.25f);

        bool isFrameFormatSet = CameraDevice.Instance.SetFrameFormat(pixelFormat, true);
        //Debug.Log(String.Format("FrameFormatSet : {0}", isFrameFormatSet.ToString()));

        // Force autofocus.
        bool isAutoFocus = CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
        if (!isAutoFocus)
        {
            CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_NORMAL);
        }
        //Debug.Log(String.Format("AutoFocus : {0}", isAutoFocus));
        cameraInitialized = true;
    }

    public Result GetResult()
    {
        if (cameraInitialized)
        {
            try
            {
                Image cameraFeed = CameraDevice.Instance.GetCameraImage(pixelFormat);
                if (cameraFeed == null)
                {
                    return null;
                }

                // 픽셀 포맷에 맞게 비트맵 포맷도 바꿔줌
                RGBLuminanceSource.BitmapFormat bitmapFormat = RGBLuminanceSource.BitmapFormat.RGB24;
                if (cameraFeed.PixelFormat == Image.PIXEL_FORMAT.RGB565)
                    bitmapFormat = RGBLuminanceSource.BitmapFormat.RGB565;
                else if (cameraFeed.PixelFormat == Image.PIXEL_FORMAT.GRAYSCALE)
                    bitmapFormat = RGBLuminanceSource.BitmapFormat.Gray8;

                return barCodeReader.Decode(cameraFeed.Pixels, cameraFeed.BufferWidth, cameraFeed.BufferHeight, bitmapFormat);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
        return null;
    }

    void OnDestroy()
    {
        AbortThread();
    }

    void OnApplicationQuit()
    {
        // 에디터에서 재생 모드를 중지했을 때, OnDestroy()가 호출되지 않으므로
        // 두 번째 재생하면 에디터가 응답없음이 된다. 그래서 여기서 쓰레드 종료를 해준다.
#if UNITY_EDITOR
        AbortThread();
#endif
    }

#if USE_QRTHREAD
    private void StartThread()
    {
        if (!bScanEnd)
        {
            qrThread = new Thread(QRScanThread);
            qrThread.Start();
        }
    }

    private void AbortThread()
    {
        if (qrThread != null)
        {
#if UNITY_ANDROID
            try
            {
                qrThread.Abort();
            }
            catch (ThreadAbortException ex)
            {
                Debug.Log("QRThread aborted.");
            }
#endif
            qrThread = null;
        }
    }

    public void QRScanThread()
    {
        while (true)
        {
            Thread.Sleep(1000); // 1초마다 새로 스캔한다.

            lock (this)
            {
                // QR코드 무한 반복 스캔 방지용
                if (bReadyWWW == true || bRequestWWW == true)
                    continue;

                _qrResult = GetResult();

                if (_qrResult == null)
                    continue;

                // QRCode detected.
                Debug.Log(string.Format("QRCode detected : {0}", _qrResult.Text));

                bReadyWWW = true;
            }
        }
    }
#else
    private void StartThread()
    {
        if (!bScanEnd)
        {
            bScanning = true;
            _scanCoroutine = StartCoroutine(QRCodeScan());
        }
    }

    private void AbortThread()
    {
        bScanning = false;
        if (_scanCoroutine != null)
        {
            StopCoroutine(_scanCoroutine);
        }
    }

    public IEnumerator QRCodeScan()
    {
        while (bScanning)
        {
            yield return new WaitForSeconds(1.0f); // 1초마다 새로 스캔한다.

            // QR코드 무한 반복 스캔 방지용
            if (bReadyWWW == false && bRequestWWW == false && bScanning == true)
            {
                _qrResult = GetResult();

                if (_qrResult != null)
                {
                    // QRCode detected.
                    Debug.Log(string.Format("QRCode detected : {0}", _qrResult.Text));

                    bScanning = false;
                    bReadyWWW = true;
                }
            }
        }
        _scanCoroutine = null;
    }
#endif
    void Update()
    {       
        if (도움말시작)
        {
            if (도움말.alpha >= 1)
            {
                도움말시작 = false;
            }
            else
            {
                도움말.alpha += Time.deltaTime * 2;

            }
        }
        else
        {
            if (도움말.alpha != 0 && 도움말.alpha != 1)
            {
                도움말.alpha -= Time.deltaTime * 2;

                if (도움말.alpha == 0)
                {
                    도움말.gameObject.SetActive(false);
                }
            }
        }        

        // WWW 할 준비가 되어있고 아직 요청 전이라면
        if (bReadyWWW == true && bRequestWWW == false)
        {
            if (_qrResult != null)
            {
                PostCheckSerial(_qrResult.Text);
            }
        }        
    }

    private void PostCheckSerial(string serial)
    {
        if (QR코드스캔_텍스트 != null)
        {
            // "스캔된 QR코드를 확인중입니다."
            QR코드스캔_텍스트.text = LocalizeText.Value["CheckingQR"];
        }

        // 인터넷에 연결할 수 없는 상태이면
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            bReadyWWW = false;
            // "인터넷에 연결할 수 없습니다."
            결과팝업_열기(LocalizeText.Value["Net_Unable"]);
            return;
        }

        // 요청할 데이터
        ServerRequest request = new ServerRequest();
        request.Serial = serial;
        request.AppName = Application.bundleIdentifier + QR구분번호;
        if (oldResultCode > 0)
        {
            request.OldExtra = oldResultExtra;
        }
        request.ReqExtra = 필수부가기능;
#if UNITY_EDITOR
        //request.NoCount = true; // 에디터에서 실행할 경우 시리얼 사용횟수를 증가시키지 않음
#endif

        string postData = JsonUtility.ToJson(request);

        // http 헤더를 정의
        Dictionary<string, string> header = new Dictionary<string, string>();
        header.Add("Content-Type", "application/json");
        header.Add("Content-Length", postData.Length.ToString());        

        // 서버에 요청
        WWW www = new WWW(시리얼체크_URL, Encoding.UTF8.GetBytes(postData), header);
        StartCoroutine(WaitForRequest(www));

        bRequestWWW = true;
        PlayerPrefs.SetString(QRCODE_SERIAL_KEY + QR구분번호, serial);
    }

    private IEnumerator WaitForRequest(WWW www)
    {
        yield return www;

        if (www.error == null)
        {
            Debug.Log(string.Format("WWW Result : {0}", www.text));

            ServerResult result = JsonUtility.FromJson<ServerResult>(www.text);

            // 0 : 체험용 시리얼 번호 (최대 3회, 애플 심사 통과용)
            // 1~5 : 정상적인 시리얼 번호, 현재까지 사용횟수 (최대 사용횟수는 서버에서 설정, 기본값 3회)
            if (result.ResultCode < 0)
            {
                if (result.ResultCode == -1)
                {
                    // "등록되지 않은 시리얼 번호!"
                    결과팝업_열기(LocalizeText.Value["QR_Unregistered"]);
                }
                else if (result.ResultCode == -2)
                {
                    // "만료된 시리얼 번호!"
                    결과팝업_열기(LocalizeText.Value["QR_Expired"]);
                }
                else if (result.ResultCode == -3)
                {
                    // "유효하지 않은 시리얼 번호!"
                    결과팝업_열기(LocalizeText.Value["QR_Invalid"]);
                }
                else if (result.ResultCode == -4)
                {
                    // "적합하지 않은 시리얼 번호!"
                    결과팝업_열기(LocalizeText.Value["QR_Unsuitable"]);
                }
                else if (result.ResultCode == -100)
                {
                    // "데이터베이스 오류!"
                    결과팝업_열기(LocalizeText.Value["DB_Error"]);
                }
                else
                {
                    // "서버 오류!"
                    결과팝업_열기(LocalizeText.Value["Server_Error"]);
                }
            }
            else
            {
                int resultCode = result.ResultCode;
                uint resultExtra = result.ResultExtra;

                // 체험용 QR코드 사용
                if (resultCode == 0)
                {
                    if (oldResultCode > 0)
                    {
                        // 이미 인증된 경우 체험용 QR코드 사용불가
                        resultCode = -10;
                    }
                    else
                    {
                        // 체험 횟수 변경
                        resultCode = oldResultCode - 1;
                    }
                }
                else
                {
                    // 기존 인증된 부가기능에 새로 인증한 QR코드의 부가기능을 추가
                    resultExtra |= oldResultExtra;
                }

                // 체험용 QR코드 3회 모두 사용
                if (resultCode < -3)
                {
                    // "체험 횟수를 모두 사용하였습니다."
                    결과팝업_열기(LocalizeText.Value["UsedExpChance"]);
                }
                else
                {
                    PlayerPrefs.SetInt(QRCODE_RESULT_KEY + QR구분번호, resultCode);
                    PlayerPrefs.SetInt(QRCODE_RESULT_EXTRA_KEY + QR구분번호, (int)resultExtra);
                    PlayerPrefs.Save();

                    bScanEnd = true;

                    // 정상적인 시리얼 번호 (체험용 QR코드 또는 Skip 하는 경우 제외)
                    if (resultCode > 0)
                    {
                        // "제품 등록이 완료되었습니다."
                        결과팝업_열기(LocalizeText.Value["QR_Success"]);
                        yield return new WaitForSeconds(1.0f);
                    }

                    needScanning = false;
                    LoadNextScene();
                }
            }
        }
        else
        {
            Debug.Log(string.Format("WWW Error : {0}", www.error));
            // "서버 오류!"
            결과팝업_열기(LocalizeText.Value["Server_Error"]);
        }

        bReadyWWW = false;
        bRequestWWW = false;
    }

    private void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(QR구분번호))
        {
            //SceneManager.LoadScene("02. Update");
            SceneManager.LoadScene(MAIN_SCENE_NAME);
        }
        else
        {
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadSceneAsync(nextSceneIndex);
            }
            else
            {
                SceneManager.LoadSceneAsync(0);
            }
        }
    }

    private void 결과팝업_열기(string resultText)
    {
        AbortThread();

        결과팝업_텍스트.text = resultText;
        시리얼입력팝업.SetActive(false);
        결과팝업.SetActive(true);
    }

    public void 결과팝업_닫기()
    {
        결과팝업.SetActive(false);

        if (QR코드스캔_텍스트 != null)
        {
            QR코드스캔_텍스트.text = LocalizeText.Value["ScanQR"];
        }

        StartThread();
    }

    public void 시리얼입력팝업_열기()
    {
        AbortThread();

        시리얼입력팝업.SetActive(true);
        //시리얼입력필드[0].isSelected = true;
    }

    public void 시리얼입력_변경(UIInput 필드)
    {
        // 4글자를 모두 입력하면 다음 필드로 포커스를 넘긴다.
        string fieldValue = 필드.value;
        if (!string.IsNullOrEmpty(fieldValue) && fieldValue.Length == 4)
        {
            for (int i = 0; i < 3; i++)
            {
                if (필드 == 시리얼입력필드[i])
                {
                    시리얼입력필드[i + 1].isSelected = true;
                }
            }
        }
    }

    public void 시리얼입력_체크()
    {
        string serial = "";
        for (int i = 0; i < 4; i++)
        {
            if (string.IsNullOrEmpty(시리얼입력필드[i].value))
            {
                시리얼입력팝업_닫기();
                return;
            }
            if (i > 0)
            {
                serial += "-";
            }
            serial += 시리얼입력필드[i].value;
        }

        for (int i = 0; i < 4; i++)
        {
            시리얼입력필드[i].value = "";
        }

        PostCheckSerial(serial);
    }

    public void 시리얼입력팝업_닫기()
    {
        for (int i = 0; i < 4; i++)
        {
            시리얼입력필드[i].value = "";
        }

        시리얼입력팝업.SetActive(false);

        StartThread();
    }

    public void 프로그램종료()
    {
        종료팝업_닫기();
        Application.Quit();
    }

    public void 종료팝업_닫기()
    {
        TweenAlpha 트윈알파 = 종료팝업.GetComponent<TweenAlpha>();
        Destroy(트윈알파);

        종료팝업.SetActive(false);
    }

    public void 종료팝업_열기()
    {
        종료팝업.GetComponent<UIPanel>().alpha = 0; // 패널 알파 초기화
        TweenAlpha.Begin(종료팝업, 0.3f, 1); // 트윈 알파 컨포넌트 추가

        종료팝업.SetActive(true);
    }

    public void 도움말띄우기()
    {
        도움말.gameObject.SetActive(true);
        도움말시작 = true;
    }

    public void 도움말닫기()
    {
        도움말.alpha -= 0.1f;
    }

    public void MoveNextScene()
    {
        // 체험용 QR코드 사용
        PostCheckSerial("IHAT-ESTE-VEJO-BS11");
    }

    public void 메인하위UI_컨트롤()
    {
        UI_상태변경(메인하위UI, 메인하위UI상태 = !메인하위UI상태);
    }

    private void UI_상태변경(GameObject 오브젝트, bool 상태)
    {
        if (오브젝트 != null)
        {
            오브젝트.SetActive(상태);
        }
    }

    private void NGUI카메라UI_보이기()
    {
        //한스로고UI.SetActive(false);
        메뉴UI.SetActive(true);
    }

    public void 스크린샷저장()
    {

        메인하위UI_컨트롤();

        메뉴UI.SetActive(false);
        //한스로고UI.SetActive(true);

        if (Application.platform == RuntimePlatform.WindowsEditor ||
            Application.platform == RuntimePlatform.OSXEditor)
        {
            Application.CaptureScreenshot("HansScreenShot.jpg");
        }
        else
        {
            // 파일이름, 앨범이름, 저장형식
            ScreenshotManager.SaveScreenshot("HansScreenShot", "HansApp", ".jpg");
        }

        Invoke("NGUI카메라UI_보이기", 1.0f);
    }

    public void 카메라변경()
    {
        메인하위UI_컨트롤();

        // QR코드 씬에서 이 기능은 필요없으므로 구현하지 않음
    }

    public void 홈페이지이동()
    {

        메인하위UI_컨트롤();

        Application.OpenURL("http://www.hansapp.co.kr/");
    }

    public void 카페이동()
    {
        메인하위UI_컨트롤();
        Application.OpenURL("http://cafe.naver.com/hansarworld");
    }

    public void 뒤로가기_메인()
    {
        needScanning                                = true;
        backupSceneName                             = string.Empty;
        GlobalDataManager.m_SelectedCategoryEnum    = GlobalDataManager.CategoryType.None;

        if (string.IsNullOrEmpty(QR구분번호))
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            SceneManager.LoadScene(MAIN_SCENE_NAME);
        }        
    }

    public static uint NumberToFlag(int number)
    {
        if (number >= 1 && number <= 32)
        {
            return (uint)(1 << number);
        }
        return 0;
    }

    public static bool HasFlagSet(uint flag, int number)
    {
        if (number >= 1 && number <= 32)
        {
            return ((flag & (1 << number)) != 0);
        }
        return false;
    }

    public static bool CheckCertResult()
    {
        int resultValue = PlayerPrefs.GetInt(QRCODE_RESULT_KEY, 0);
        return (resultValue > 0);
    }

    public static bool CheckCertResult(int code)
    {
        int resultValue = PlayerPrefs.GetInt(string.Format("{0}#{1}", QRCODE_RESULT_KEY, (code + 1)), 0);
        return (resultValue > 0);
    }

    public static uint GetCertResultExtra()
    {
        return (uint)PlayerPrefs.GetInt(QRCODE_RESULT_EXTRA_KEY, 0);
    }

    public static uint GetCertResultExtra(int code)
    {
        return (uint)PlayerPrefs.GetInt(string.Format("{0}#{1}", QRCODE_RESULT_EXTRA_KEY, (code + 1)), 0);
    }

    [Serializable]
    public class ServerRequest
    {
        public string Serial;
        public string AppName;
        public uint OldExtra;
        public uint ReqExtra;
        public bool NoCount;
    }

    [Serializable]
    public class ServerResult
    {
        public int ResultCode = -1;
        public string ResultMessage;
        public uint ResultExtra;
    }
}
