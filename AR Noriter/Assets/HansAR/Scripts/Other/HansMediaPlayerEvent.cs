using UnityEngine;
using System.IO;
using System.Collections;

public class HansMediaPlayerEvent : MonoBehaviour
{
    private ModelInfo       modelInfo;
    private MediaPlayerCtrl mediaPlayerCtrl;

    private bool isLoaded;

    public bool isStreammingAssets  = false;
    public bool isLoopPlay          = false;
    public bool UsedEveryPlayAudio  = false;    

    void Awake()
    {
        this.modelInfo          = GetComponent<ModelInfo>();
        this.mediaPlayerCtrl    = GetComponentInChildren<MediaPlayerCtrl>();

        if (this.mediaPlayerCtrl != null)
        {
            this.mediaPlayerCtrl.m_bFullScreen          = false;
            this.mediaPlayerCtrl.m_bAutoPlay            = false;            
            this.mediaPlayerCtrl.m_bLoop                = false;

            this.mediaPlayerCtrl.OnReady                = OnMediaPlayerReady;
            this.mediaPlayerCtrl.OnEnd                  = OnMediaPlayerEnd;
            this.mediaPlayerCtrl.OnVideoError           = OnMediaPlayerVideoError;
            this.mediaPlayerCtrl.OnVideoFirstFrameReady = OnVideoFirstFrameReady;
        }

        this.isLoaded = false;
    }

	// Use this for initialization
	void Start ()
    {
        if (!isStreammingAssets)
        {
            MovieInit();
        }

        MediaPlayerUnload();
        MediaPlayerLoad(GetMediaSource());
    }
	
	// Update is called once per frame
	void Update ()
    {       
           
    }

    void OnDestroy ()
    {

    }

    void OnMediaPlayerReady ()
    {
        this.MediaPlayerPlay();
        this.isLoaded = true;
    }

    void OnMediaPlayerEnd ()
    {
        //if (this.mediaPlayerCtrl.m_bLoop == true)
        if (isLoopPlay)
        {
            this.MediaPlayerPlay();
        }
        else
        {
            this.MediaPlayerStop();
        }
    }

    void OnMediaPlayerVideoError(MediaPlayerCtrl.MEDIAPLAYER_ERROR errorCode, MediaPlayerCtrl.MEDIAPLAYER_ERROR errorCodeExtra)
    {

    }

    void OnVideoFirstFrameReady()
    {
        this.mediaPlayerCtrl.Play();
    }

    private void MovieInit()
    {
        this.mediaPlayerCtrl.m_strFileName = "file://" + Application.persistentDataPath + "/" + this.mediaPlayerCtrl.m_strFileName;
    }

    public void MediaPlayerStop()
    {        
        this.mediaPlayerCtrl.Stop();
        //EffectSoundManager.이펙트.이펙트사운드_중지();                
        DubbingManager.더빙.더빙사운드_중지();
    }

    public void MediaPlayerPlay()
    {
        this.mediaPlayerCtrl.Play();

        //EffectSoundManager.이펙트.EffectSoundPlay(this.modelInfo.이펙트클립);
        DubbingManager.더빙.DubbingSoundPlay(this.modelInfo.더빙클립);
    }

    public void MediaPlayerLoad(string videoUrl)
    {
        this.mediaPlayerCtrl.Load(videoUrl);
    }

    public void MediaPlayerUnload()
    {
        this.mediaPlayerCtrl.UnLoad();
    }

    public string GetMediaSource()
    {
        return this.mediaPlayerCtrl.m_strFileName;
    }
}
