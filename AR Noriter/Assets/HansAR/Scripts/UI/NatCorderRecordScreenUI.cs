using UnityEngine;
using System.IO;
using System.Collections;
using System.Runtime.InteropServices;
using NatCorderU.Core;
using NatCorderU.Core.Recorders;

/// <summary>
/// 스크린 녹화 및 마이크 녹음 기능을 위한 UI
/// </summary>
public class NatCorderRecordScreenUI : MonoBehaviour
{
    /// <summary>
    /// 녹화할 카메라 (2D 카메라 : 전체, AR 카메라 : 캠 화면만, HUD 카메라 : 캠 화면 + 증강 컨텐츠)
    /// </summary>
    public Camera _targetCamera;
    /// <summary>
    /// 녹화할 오디오리스너
    /// </summary>
    public AudioListener _audioListener;
    /// <summary>
    /// 녹화 시작 버튼
    /// </summary>
    public GameObject _recordStartButton;
    /// <summary>
    /// 녹화 중지 버튼
    /// </summary>
    public GameObject _recordStopButton;
    /// <summary>
    /// 미리보기 재생 버튼
    /// </summary>
    public GameObject _playPreviewButton;
    /// <summary>
    /// 미리보기 재생 버튼에 알파값 적용 여부
    /// </summary>
    public bool _preBtnAlpha = false;
    /// <summary>
    /// 상태 표시 레이블 (테스트용)
    /// </summary>
    public GameObject _statusLabel;
    /// <summary>
    /// 동영상 미리보기 UI
    /// </summary>
    public MoviePreviewUI _moviePreviewUI;
    /// <summary>
    /// 최대 녹화시간 (초)
    /// </summary>
    public int _maxRecordSeconds = 180;
    /// <summary>
    /// 마이크를 사용하여 녹음할 것인지 여부 (게임 사운드와 함께 녹음할 경우 두 개의 사운드가 믹스됨)
    /// </summary>
    public bool _recordMicAudio = true;
    /// <summary>
    /// 게임 사운드를 녹음할 것인지 여부 (마이크와 함께 녹음할 경우 두 개의 사운드가 믹스됨)
    /// </summary>
    public bool _recordGameSound = true;
    /// <summary>
    /// 사용할 장치가 저사양인지 여부
    /// </summary>
    public bool _isLowEndDevice = false;

    /// <summary>
    /// 카메라 레코더
    /// </summary>
    private CameraRecorder _cameraRecorder;
    /// <summary>
    /// 오디오 레코더
    /// </summary>
    private AudioRecorder _audioRecorder;
    /// <summary>
    /// 녹음한 오디오를 가지고 있는 오디오 소스
    /// </summary>
    private AudioSource _audioSource;
    /// <summary>
    /// 최소 주파수 (음질)
    /// </summary>
    private int _minFreq = 16000;
    /// <summary>
    /// 최대 주파수 (음질)
    /// </summary>
    private int _maxFreq = 48000;
    /// <summary>
    /// 자동 회전 상태를 백업하기 위한 변수
    /// </summary>
    private bool _autorotateToPortrait = false;
    /// <summary>
    /// 자동 회전 상태를 백업하기 위한 변수
    /// </summary>
    private bool _autorotateToPortraitUpsideDown = false;
    /// <summary>
    /// 자동 회전 상태를 백업하기 위한 변수
    /// </summary>
    private bool _autorotateToLandscapeLeft = false;
    /// <summary>
    /// 자동 회전 상태를 백업하기 위한 변수
    /// </summary>
    private bool _autorotateToLandscapeRight = false;
    /// <summary>
    /// 마지막에 저장된 동영상 파일의 전체 경로명
    /// </summary>
    private string _videoFilePath = null;
    /// <summary>
    /// 최대 녹화시간 10분 이상은 녹화불가능!!!
    /// </summary>
    private const int MaxRecordSeconds = 600;
    /// <summary>
    /// 저장 파일명 Prefix
    /// </summary>
    private const string FileNamePrefix = "Recording";
    /// <summary>
    /// 저장 폴더명
    /// </summary>
    private const string CaptureDir = "HansApp";

#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern int saveVideoToGallery(string path);

    [DllImport("__Internal")]
    private static extern int _forceToSpeaker();
#endif

    void Awake()
    {
        // 마이크 녹음을 위한 오디오 소스 설정
        _audioSource = gameObject.GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
        _audioSource.playOnAwake = false;
        _audioSource.loop = false;
    }

    void OnDestroy()
    {
        // 동영상 녹화 중단
        if (NatCorder.IsRecording)
        {
            // 마이크 녹음 중단
            if (_recordMicAudio)
            {
                _audioSource.Stop();
                Microphone.End(null);
            }
            if (_audioRecorder != null)
            {
                _audioRecorder.Dispose();
                _audioRecorder = null;
            }
            if (_cameraRecorder != null)
            {
                _cameraRecorder.Dispose();
                _cameraRecorder = null;
            }
            NatCorder.StopRecording();
        }
    }

    void Start()
    {
        // 저사양 장치 판정 (시스템메모리 1GB 미만, 프로세서 4개 미만인 경우)
        if (SystemInfo.systemMemorySize < 1024 && SystemInfo.processorCount < 4)
        {
            _isLowEndDevice = true;
        }

        if (Microphone.devices.Length > 0)
        {
            Debug.Log(string.Format("마이크 갯수 : {0}", Microphone.devices.Length));

            // 장치 정보를 얻어와서 마이크 음질을 결정한다.
            Microphone.GetDeviceCaps(null, out _minFreq, out _maxFreq);
            if (_maxFreq > 64000)
            {
                _maxFreq = 64000;
            }
            else if (_minFreq >= _maxFreq)
            {
                _maxFreq = 48000;
            }
        }
        else
        {
            Debug.LogWarning("마이크 장치가 없습니다!");
        }

        EnableRecordStartButton(true);
        EnableRecordStopButton(false);
        EnablePlayPreviewButton(false);
    }

    /// <summary>
    /// 녹화 시작 버튼을 활성화/비활성화
    /// </summary>
    private void EnableRecordStartButton(bool enabled)
    {
        if (_recordStartButton != null)
        {
            _recordStartButton.SetActive(enabled);
        }
    }

    /// <summary>
    /// 녹화 중지 버튼을 활성화/비활성화
    /// </summary>
    private void EnableRecordStopButton(bool enabled)
    {
        if (_recordStopButton != null)
        {
            _recordStopButton.SetActive(enabled);
        }
    }

    /// <summary>
    /// 미리보기 재생 버튼을 활성화/비활성화
    /// </summary>
    private void EnablePlayPreviewButton(bool enabled)
    {
        if (_playPreviewButton != null)
        {
            if (_playPreviewButton.GetComponent<UIButton>() != null)
            {
                _playPreviewButton.GetComponent<UIButton>().tweenTarget = null;
            }

            _playPreviewButton.SetActive(enabled);

            if (_preBtnAlpha)
            {
                _playPreviewButton.SetActive(true);
                _playPreviewButton.GetComponent<BoxCollider2D>().enabled = enabled;

                if (enabled)
                {
                    _playPreviewButton.GetComponent<UIWidget>().alpha = 1;
                }
                else
                {
                    _playPreviewButton.GetComponent<UIWidget>().alpha = 0.25f;
                }
            }
        }
    }

    /// <summary>
    /// 상태 레이블의 텍스트를 변경
    /// </summary>
    private void SetStatusText(string text, bool autoHide)
    {
        if (_statusLabel != null)
        {
            UILabel label = _statusLabel.GetComponent<UILabel>();
            if (label != null)
            {
                label.text = text;

                CancelInvoke("ClearStatusText");

                if (autoHide)
                {
                    Invoke("ClearStatusText", 3.0f);
                }
            }
        }
    }

    /// <summary>
    /// 상태 레이블의 텍스트를 지움
    /// </summary>
    private void ClearStatusText()
    {
        if (_statusLabel != null)
        {
            UILabel label = _statusLabel.GetComponent<UILabel>();
            if (label != null)
            {
                label.text = "";
            }
        }
    }

    /// <summary>
    /// 녹화가 시작되었을 때 이벤트
    /// </summary>
    private void RecordingStarted()
    {
        SetStatusText("Recording the screen.", false);
        Debug.Log("화면 녹화가 시작되었습니다.");
        EnableRecordStartButton(false);
        EnableRecordStopButton(true);
        EnablePlayPreviewButton(false);

        // 자동 회전 상태를 백업한다.
        _autorotateToPortrait = Screen.autorotateToPortrait;
        _autorotateToPortraitUpsideDown = Screen.autorotateToPortraitUpsideDown;
        _autorotateToLandscapeLeft = Screen.autorotateToLandscapeLeft;
        _autorotateToLandscapeRight = Screen.autorotateToLandscapeRight;

        // 녹화 도중에는 자동 회전 기능을 끈다.
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToLandscapeLeft = false;
        Screen.autorotateToLandscapeRight = false;
    }

    /// <summary>
    /// 녹화가 중지되었을 때 이벤트
    /// </summary>
    private void RecordingStopped()
    {
        // 자동 회전 기능을 원래대로 복원한다.
        Screen.autorotateToPortrait = _autorotateToPortrait;
        Screen.autorotateToPortraitUpsideDown = _autorotateToPortraitUpsideDown;
        Screen.autorotateToLandscapeLeft = _autorotateToLandscapeLeft;
        Screen.autorotateToLandscapeRight = _autorotateToLandscapeRight;

        //SetStatusText("Screen recording has completed.", true);
        //Debug.Log("화면 녹화가 완료되었습니다.");
        //EnableRecordStartButton(true);
        //EnableRecordStopButton(false);
        //EnablePlayPreviewButton(true);
    }

    /// <summary>
    /// 동영상 녹화가 완료되었을 때 이벤트
    /// </summary>
    /// <param name="path">저장된 동영상 파일의 경로</param>
    private void OnVideoRecorded(string path)
    {
        if (path.StartsWith("file://"))
        {
            path = path.Remove(0, 7);
        }

        _videoFilePath = path;

        // 저장된 파일을 복사한다.
        StartCoroutine(CopyFileWithWWW(path, MoviePreviewUI._shareFilePath));
    }

    /// <summary>
    /// 녹화를 시작하는 코루틴
    /// </summary>
    private IEnumerator StartRecording()
    {
        // 녹화할 카메라와 오디오리스너 설정
        if (_targetCamera == null)
        {
            _targetCamera = Camera.main;
        }
        if (_audioListener == null)
        {
            _audioListener = FindObjectOfType<AudioListener>();
        }

        // 최대 녹화시간을 잘못 설정한 경우 기본값으로 변경
        if (_maxRecordSeconds < 1 || _maxRecordSeconds > MaxRecordSeconds)
        {
            _maxRecordSeconds = 180;
        }
        
        string CapturePath = string.Format("{0}/{1}", Application.persistentDataPath, CaptureDir);
        if (!Directory.Exists(CapturePath))
        {
            Directory.CreateDirectory(CapturePath);
        }

        string date = System.DateTime.Now.ToString("hh-mm-ss_dd-MM-yy");
        // NatCorder는 플랫폼에 따라 동영상 확장자가 다르다.
#if UNITY_IOS
        MoviePreviewUI._shareFilePath = string.Format("{0}/{1}_{2}.mov", CapturePath, FileNamePrefix, date);
#else
        MoviePreviewUI._shareFilePath = string.Format("{0}/{1}_{2}.mp4", CapturePath, FileNamePrefix, date);
#endif

        if (!NatCorder.IsRecording)
        {
            int shortSideSize = 768;
            int targetFrameRate = Application.targetFrameRate;
            // 저사양 녹화/녹음 설정
            if (_isLowEndDevice)
            {
                shortSideSize = 600;
                targetFrameRate = 10;
                _maxFreq = 16000;
            }
            float aspectRatio = (float)Screen.width / Screen.height;
            int videoWidth, videoHeight;
            if (Screen.width > Screen.height) // Landscape
            {
                videoHeight = shortSideSize;
                videoWidth = (int)(shortSideSize * aspectRatio);
            }
            else // Portrait
            {
                videoWidth = shortSideSize;
                videoHeight = (int)(shortSideSize / aspectRatio);
            }

            VideoFormat videoFormat = new VideoFormat(videoWidth, videoHeight, targetFrameRate);
            AudioFormat audioFormat = (_recordMicAudio || _recordGameSound) ? AudioFormat.Unity : AudioFormat.None;

            // 녹화를 시작한 후 카메라레코더와 오디오레코더를 생성한다.
            NatCorder.StartRecording(Container.MP4, videoFormat, audioFormat, OnVideoRecorded);
            _cameraRecorder = CameraRecorder.Create(_targetCamera);
            if (_recordMicAudio)
            {
#if UNITY_IOS
                // Ear Piece를 사용중인 경우 녹음이 되지 않으므로 스피커로 강제 전환
                if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    _forceToSpeaker();
                }
#endif
                _audioSource.clip = Microphone.Start(null, true, _maxRecordSeconds, _maxFreq);
                while (Microphone.GetPosition(null) <= 0) ;
                _audioSource.Play();

                if (_recordGameSound)
                {
                    _audioRecorder = AudioRecorder.Create(_audioListener, _audioSource);
                }
                else
                {
                    _audioRecorder = AudioRecorder.Create(_audioSource, true);
                }
            }
            else if (_recordGameSound)
            {
                _audioRecorder = AudioRecorder.Create(_audioListener);
            }
            RecordingStarted();

            if (NatCorder.IsRecording)
            {
                // 녹화 시간만큼 대기한 후 녹화를 중지한다.
                yield return new WaitForSeconds(_maxRecordSeconds);
            }
            else
            {
                SetStatusText("Your device does not support screen recording!", true);
                Debug.LogWarning("화면 녹화를 지원하지 않는 장치입니다!");
            }

            StartCoroutine(StopRecording());
        }
        yield break;
    }

    /// <summary>
    /// 녹화를 중지하는 코루틴
    /// </summary>
    private IEnumerator StopRecording()
    {
        if (NatCorder.IsRecording)
        {
            // 마이크 녹음 중단
            if (_recordMicAudio)
            {
                _audioSource.Stop();
                Microphone.End(null);
            }
            if (_audioRecorder != null)
            {
                _audioRecorder.Dispose();
                _audioRecorder = null;
            }
            if (_cameraRecorder != null)
            {
                _cameraRecorder.Dispose();
                _cameraRecorder = null;
            }
            NatCorder.StopRecording();
            RecordingStopped();
        }
        yield break;
    }

    /// <summary>
    /// 미리보기를 재생하는 코루틴
    /// </summary>
    private IEnumerator StartPreview()
    {
        if (!NatCorder.IsRecording)
        {
            if (_moviePreviewUI != null)
            {
                // NatCorder의 경우 파일을 분리해서 녹음할 필요가 없고, iOS에서 동영상이 뒤집히는 문제도 없다.
                _moviePreviewUI.Load(MoviePreviewUI._shareFilePath, null, false);
            }
        }
        yield break;
    }

    /// <summary>
    /// 파일을 복사하기 위한 코루틴
    /// </summary>
    /// <param name="sourceFilePath">원본 파일경로</param>
    /// <param name="targetFilePath">대상 파일경로</param>
    /// <returns></returns>
    private IEnumerator CopyFileWithWWW(string sourceFilePath, string targetFilePath)
    {
        SetStatusText("Saving the video file.", false);
        EnableRecordStartButton(false);
        EnableRecordStopButton(false);
        EnablePlayPreviewButton(false);
        yield return new WaitForSeconds(0.1f);

        sourceFilePath = sourceFilePath.Replace("\\", "/");

        WWW www = new WWW("file://" + sourceFilePath);
        yield return www;

        if (string.IsNullOrEmpty(www.error))
        {
            File.WriteAllBytes(targetFilePath, www.bytes);

            // 동영상을 갤러리에 추가한다.
#if UNITY_IOS
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                saveVideoToGallery(targetFilePath);
                UnityEngine.iOS.Device.SetNoBackupFlag(targetFilePath);
            }
#elif UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidJavaClass pluginClass = new AndroidJavaClass("com.hansapp.savetogallery.UnityPlugin");
                pluginClass.CallStatic<int>("saveVideoToGallery", targetFilePath);
            }
#endif
            try
            {
                File.Delete(sourceFilePath);
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }

            SetStatusText("Screen recording has completed.", true);
            Debug.Log("화면 녹화가 완료되었습니다.");
        }
        else
        {
            SetStatusText("Failed to save the video file!", true);
            Debug.LogError("동영상 파일 저장에 실패하였습니다!");
        }

        EnableRecordStartButton(true);
        EnableRecordStopButton(false);
        EnablePlayPreviewButton(true);

        www.Dispose();
        www = null;
        yield break;
    }

    /// <summary>
    /// 동영상 공유를 시작하는 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShareMovieFile()
    {
        ShareWithApp.ShareVideo(MoviePreviewUI._shareFilePath);
        yield break;
    }

    /// <summary>
    /// 녹화 시작 버튼을 눌렀을 때 이벤트
    /// </summary>
    public void OnStartRecord()
    {
        StartCoroutine(StartRecording());
    }

    /// <summary>
    /// 녹화 중지 버튼을 눌렀을 때 이벤트
    /// </summary>
    public void OnStopRecord()
    {
        StartCoroutine(StopRecording());
    }

    /// <summary>
    /// 미리보기 재생 버튼을 눌렀을 때 이벤트
    /// </summary>
    public void OnPlayPreview()
    {
        StartCoroutine(StartPreview());
    }

    /// <summary>
    /// 동영상 공유 버튼을 눌렀을 때 이벤트
    /// </summary>
    public void OnShareMovie()
    {
        StartCoroutine(ShareMovieFile());
    }
}
