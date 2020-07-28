using UnityEngine;
using System.Collections;

public class DubbingManager : MonoBehaviour
{
    /// <summary>
    /// 사운드를 출력할 오브젝트를 에디터에서 지정해줍니다.
    /// </summary>
    public GameObject 더빙사운드;

    /// <summary>
    /// 알아보기 모드를 사용 할지 안할지 여부
    /// </summary>
    public bool 알아보기사용여부 = false;

    public AudioClip[] 다중사운드클립;

    public static DubbingManager 더빙;

    void Awake()
    {
        더빙 = this;
    }

    void Start()
    {
        
    }

    void Update()
    {

    }

    public void 더빙사운드_클릭재생()
    {
        if (더빙사운드 != null)
        {
            int 인덱스 = TargetManager.타깃메니저.복제모델링인덱스;
            GameObject 모델링 = TargetManager.타깃메니저.에셋번들복제컨텐츠[인덱스];

            AudioClip 오디오클립 = 모델링.GetComponent<ModelInfo>().더빙클립;
            AudioSource 오디오소스 = 더빙사운드.GetComponent<AudioSource>();

            if (오디오클립 != null)
            {

                오디오소스.Stop();

                오디오소스.loop = true;
                오디오소스.clip = 오디오클립;
                오디오소스.Play();
            }
            //권기영 : 더빙사운드가 없을경우 
            else
            {
                //이전에 실행된 오디오클립이 지속 재생되기 때문에 정지시킵니다.
                오디오소스.Stop();
            }
        }
    }

    public void 더빙사운드_중지()
    {
        if (더빙사운드 != null)
        {
            더빙사운드.GetComponent<AudioSource>().Stop();
        }
    }

    public void DubbingSoundPlay(AudioClip audioClip)
    {
        if (더빙사운드 != null)
        {
            AudioSource 오디오소스 = 더빙사운드.GetComponent<AudioSource>();

            if (오디오소스.isPlaying == true)
            {
                오디오소스.Stop();
            }

            오디오소스.loop = false;
            오디오소스.clip = audioClip;
            오디오소스.Play();
        }
    }

    /// <summary>
    /// 다중사운드 설정
    /// </summary>
    /// <param name="soundNum"></param>
    public void FingeringSound(int soundNum)
    {
        int 인덱스 = TargetManager.타깃메니저.복제모델링인덱스;
        GameObject 오브젝트 = TargetManager.타깃메니저.에셋번들복제컨텐츠[인덱스];
        다중사운드클립 = 오브젝트.GetComponent<ModelInfo>().다중사운드클립;

        if (다중사운드클립.Length > 1)
        {
            AudioSource 오디오소스 = 더빙사운드.GetComponent<AudioSource>();

            if (오디오소스.isPlaying == true)
            {
                오디오소스.Stop();
            }

            오디오소스.loop = false;
            오디오소스.clip = 다중사운드클립[soundNum];
            오디오소스.Play();
        }
    }

}
