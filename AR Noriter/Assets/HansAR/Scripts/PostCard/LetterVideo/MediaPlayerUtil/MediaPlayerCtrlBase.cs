using UnityEngine;

using System.IO;
using System.Collections;

public class MediaPlayerCtrlBase : MonoBehaviour
{
    public string _shareFilePath { set; get; }
    
    /// <summary>
    /// 반복 재생 여부
    /// </summary>
    public bool _Loop = false;

    /// <summary>
    /// 동영상 플레이어 컨트롤
    /// </summary>
    private MediaPlayerCtrl _mediaPlayerCtrl;
    /// <summary>
    /// 재생할 오디오를 가지고 있는 오디오 소스
    /// </summary>
    private AudioSource _audio;
    /// <summary>
    /// 비디오가 로드되었는지 여부
    /// </summary>
    private bool _isVideoLoaded = false;
    /// <summary>
    /// 오디오가 로드되었는지 여부
    /// </summary>
    private bool _isAudioLoaded = false;
    /// <summary>
    /// 현재 재생중인지 여부
    /// </summary>
    private bool _isPlaying = false;
    /// <summary>
    /// 동영상을 세로로 뒤집을 것인지 여부
    /// </summary>
    private bool _verticalFlip = false;
    
    void Awake()
    {
        _mediaPlayerCtrl = GetComponentInChildren<MediaPlayerCtrl>();

        // 동영상 플레이어 설정
        if (_mediaPlayerCtrl != null)
        {
            _mediaPlayerCtrl.m_bFullScreen  = false;
            _mediaPlayerCtrl.m_bAutoPlay    = false;
            _mediaPlayerCtrl.m_bLoop        = false;
            _mediaPlayerCtrl.OnReady        = OnMediaPlayerReady;
            _mediaPlayerCtrl.OnEnd          = OnMediaPlayerEnd;
            _mediaPlayerCtrl.OnVideoError   = OnMediaPlayerVideoError;
        }

        // 오디오 소스 설정
        _audio = gameObject.GetComponent<AudioSource>();

        if (_audio == null)
        {
            _audio = gameObject.AddComponent<AudioSource>();
        }

        _audio.playOnAwake  = false;
        _audio.loop         = false;


        
    }

    void OnDestroy()
    {
        // 동영상 플레이어 설정
        if (_mediaPlayerCtrl != null)
        {
            _mediaPlayerCtrl.OnReady    = null;
            _mediaPlayerCtrl.OnEnd      = null;
        }
    }

    void Start()
    {
     
    }
   
    /// <summary>
    /// 동영상이 준비되었을 때 이벤트
    /// </summary>
    private void OnMediaPlayerReady()
    {
        /*
        // 동영상 크기에 따라 뷰 영역의 크기를 조절한다.
        int panelWidth = 1080;
        int panelHeight = 1080;
        int videoWidth = _mediaPlayerCtrl.GetVideoWidth();
        int videoHeight = _mediaPlayerCtrl.GetVideoHeight();

        if (videoWidth > 0 && videoHeight > 0)
        {
            float aspect = videoHeight / (float)videoWidth;
            if (aspect > 1.0f)
            {
                // 세로가 더 큰 경우
                _mediaPlayerCtrl.transform.localScale = new Vector3(panelWidth / aspect, panelHeight, _mediaPlayerCtrl.transform.localScale.z);
            }
            else
            {
                // 가로가 더 큰 경우
                _mediaPlayerCtrl.transform.localScale = new Vector3(panelWidth, panelHeight * aspect, _mediaPlayerCtrl.transform.localScale.z);
            }
        }
        */

        _isVideoLoaded = true;
    }

    /// <summary>
    /// 동영상 재생이 끝났을 때 이벤트
    /// </summary>
    private void OnMediaPlayerEnd()
    {
        if (_Loop)
        {
            Play();
        }
        else
        {
            Stop();
        }
    }

    /// <summary>
    /// 동영상 오류가 발생했을 때 이벤트
    /// </summary>
    private void OnMediaPlayerVideoError(MediaPlayerCtrl.MEDIAPLAYER_ERROR errorCode, MediaPlayerCtrl.MEDIAPLAYER_ERROR errorCodeExtra)
    {
        Debug.LogError("비디오 파일 로드에 실패하였습니다!");
        _isVideoLoaded = true;
    }

    /// <summary>
    /// 비디오 파일을 로드하기 위한 코루틴
    /// </summary>
    private IEnumerator LoadVideoFile(string filePath)
    {
        if (_mediaPlayerCtrl != null)
        {
            _mediaPlayerCtrl.Load("file://" + filePath);
            while (!_isVideoLoaded)
            {
                yield return null;
            }
        }
        yield break;
    }

    /// <summary>
    /// 오디오 파일을 로드하기 위한 코루틴
    /// </summary>
    private IEnumerator LoadAudioFile(string filePath)
    {
        WWW www = new WWW("file://" + filePath);
        yield return www;

        if (string.IsNullOrEmpty(www.error))
        {
            _audio.clip = www.GetAudioClip(false);
            _isAudioLoaded = true;
        }
        else
        {
            Debug.LogError("오디오 파일 로드에 실패하였습니다!");
        }

        www.Dispose();
        www = null;
        yield break;
    }

    /// <summary>
    /// 동영상을 로드하기 위한 코루틴
    /// </summary>
    /// <param name="videoFilePath"></param>
    /// <param name="audioFilePath"></param>
    /// <returns></returns>
    private IEnumerator LoadFiles(string videoFilePath, string audioFilePath, bool isPlay)
    {
        // 동영상을 언로드한다.
        //Unload();

        // 비디오 파일을 로드한다.
        if (!string.IsNullOrEmpty(videoFilePath) && File.Exists(videoFilePath))
        {
            yield return StartCoroutine(LoadVideoFile(videoFilePath));
        }

        // 오디오 파일을 로드한다.
        if (!string.IsNullOrEmpty(audioFilePath) && File.Exists(audioFilePath))
        {
            yield return StartCoroutine(LoadAudioFile(audioFilePath));
        }
        
        
        // 동영상을 재생한다.
        Play();
        yield return new WaitForSeconds(0.2f);               

        // 동영상을 세로로 뒤집는다.
        if (_verticalFlip && _mediaPlayerCtrl.transform.localScale.y > 0)
        {
            _mediaPlayerCtrl.transform.localScale = new Vector3(_mediaPlayerCtrl.transform.localScale.x, -_mediaPlayerCtrl.transform.localScale.y, _mediaPlayerCtrl.transform.localScale.z);
        }

        if (isPlay == false)
        {
            Pause();
            yield break;
        }

        yield break;
    }

    /// <summary>
    /// 동영상을 로드합니다.
    /// </summary>
    /// <param name="videoFilePath"></param>
    /// <param name="audioFilePath"></param>
    /// <param name="verticalFlip"></param>
    public void Load(string videoFilePath, string audioFilePath, bool verticalFlip, bool isPlay = true)
    {
        // 동영상을 언로드한다.
        Unload();
        _verticalFlip = verticalFlip;
        StartCoroutine(LoadFiles(videoFilePath, audioFilePath, isPlay));
    }

    /// <summary>
    /// 동영상을 재생합니다.
    /// </summary>
    public void Play()
    {
        if (_audio.clip != null)
        {
            _audio.Play();
        }
        if (_mediaPlayerCtrl != null)
        {
            _mediaPlayerCtrl.Play();

            // 동영상을 세로로 뒤집는다.
            if (_verticalFlip & _mediaPlayerCtrl.transform.localScale.y > 0)
            {
                _mediaPlayerCtrl.transform.localScale = new Vector3(_mediaPlayerCtrl.transform.localScale.x, -_mediaPlayerCtrl.transform.localScale.y, _mediaPlayerCtrl.transform.localScale.z);
            }
        }
        _isPlaying = true;        
    }

    /// <summary>
    /// 동영상을 일시정지합니다.
    /// </summary>
    public void Pause()
    {
        if (_audio.clip != null)
        {
            if (_isPlaying)
            {
                _audio.Pause();
            }
        }
        if (_mediaPlayerCtrl != null)
        {
            if (_isPlaying)
            {
                _mediaPlayerCtrl.Pause();
            }
        }
        _isPlaying = false;        
    }

    /// <summary>
    /// 동영상을 중지합니다.
    /// </summary>
    public void Stop()
    {
        if (_audio.clip != null)
        {
            _audio.Stop();
        }
        if (_mediaPlayerCtrl.GetCurrentState() == MediaPlayerCtrl.MEDIAPLAYER_STATE.PLAYING || _mediaPlayerCtrl.GetCurrentState() == MediaPlayerCtrl.MEDIAPLAYER_STATE.PAUSED)
        {
            _mediaPlayerCtrl.Stop();
            _mediaPlayerCtrl.SeekTo(0);
        }

        // 동영상을 세로로 뒤집는다.
        if (_verticalFlip && _mediaPlayerCtrl.transform.localScale.y > 0)
        {
            _mediaPlayerCtrl.transform.localScale = new Vector3(_mediaPlayerCtrl.transform.localScale.x, -_mediaPlayerCtrl.transform.localScale.y, _mediaPlayerCtrl.transform.localScale.z);
        }

        _isPlaying = false;        
    }

    /// <summary>
    /// 동영상을 언로드합니다.
    /// </summary>
    public void Unload()
    {
        if (_isAudioLoaded)
        {
            if (_audio.clip != null)
            {
                _audio.Stop();
                Destroy(_audio.clip);
                _audio.clip = null;
            }
            _isAudioLoaded = false;
        }
        if (_isVideoLoaded)
        {
            if (_mediaPlayerCtrl != null)
            {
                _mediaPlayerCtrl.Stop();
                _mediaPlayerCtrl.UnLoad();
            }
            _isVideoLoaded = false;
        }
        _isPlaying = false;        
    }

    /// <summary>
    /// 동영상을 언로드
    /// </summary>
    public void Close()
    {
        // 동영상을 언로드한다.
        Unload();        
    }

    public string GetCurrentFileName()
    {
        if (_mediaPlayerCtrl == null)
        {
            return null;    
        }

        return _mediaPlayerCtrl.m_strFileName;
    }

    public bool IsPlaying()
    {
        return _isPlaying;
    }

    public void RemoveStrPath()
    {
        _mediaPlayerCtrl.m_strFileName = "";
    }
}
