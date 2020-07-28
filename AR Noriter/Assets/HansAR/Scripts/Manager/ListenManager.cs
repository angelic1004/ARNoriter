using UnityEngine;
using System.Collections;

public class ListenManager : MonoBehaviour
{

    /// <summary>
    /// 권기영 : 듣고익히기에 필요한 사운드 속성값입니다.
    /// </summary>
    ListenInfo.듣고익히기속성값 듣고익히기속성;

    /// <summary>
    /// 권기영 : 듣고익히기시에 공부하기 사운드가 필요해서 가져옵니다.
    /// </summary>
    ModelInfo.공부하기속성값 공부하기속성;

    /// <summary>
    /// 권기영 : 한글과 알파벳설정을 위한 bool변수 입니다.
    /// </summary>
    public bool 한글 = false;
    public bool 알파벳 = false;


    /// <summary>
    /// 권기영 : 한글모드에서 듣고익히기가 재생되는 사운드오브젝트입니다.
    /// </summary>
    public GameObject 듣고익히기사운드;

    /// <summary>
    /// 권기영 : 한글모드에서 사용되는 듣기, 듣고익히기 버튼오브젝트입니다.
    /// </summary>
    public GameObject 듣기버튼;
    public GameObject 듣고익히기버튼;

    /// <summary>
    /// 권기영 : 한글모드에서 현재 재생될 낱말사운드 인덱스값입니다.
    /// </summary>
    private int 현재낱말사운드 = 0;

    /// <summary>
    /// 권기영 : 한글모드에서 낱말의 총개수입니다.
    /// </summary>
    private int 사운드개수;

    /// <summary>
    /// 권기영 : 알파벳모드에서 사용되는 영어, 한국어, 중국어 버튼오브젝트입니다.
    /// </summary>
    public GameObject 영어버튼;
    public GameObject 한국어버튼;
    public GameObject 중국어버튼;

    /// <summary>
    /// 권기영 : 알파벳모드에서 알파벳, 단어 순서확인 변수입니다.
    /// </summary>
    private bool 알파벳사운드순서체크 = false;

    /// <summary>
    /// 권기영 : 딜레이값을 적용할 변수입니다.
    /// </summary>
    public float 딜레이시간;
    private float 딜레이시간_보관용;

    /// <summary>
    /// 권기영 : 속성값이 들어왔는지 확인을 위한 변수
    /// </summary>
    private bool 인식체크;

    public static ListenManager 듣고익히기매니저;

    void Awake()
    {
        //권기영 : 시작전에 딜레이 초기화 
        딜레이시간_보관용 = 딜레이시간;
        딜레이시간 = 0;
        듣고익히기매니저 = this;
    }

    void Start()
    {

    }


    void FixedUpdate()
    {
        //권기영 : 인식하고 속성값이 들어왔을경우
        if (인식체크 == true)
        {
            //한글 알파벳 구분
            if (한글)
            {
                한글구분();
            }
            else
            {
                알파벳구분();
            }
        }
    }


    /// <summary>
    /// 권기영 : 증강 컨텐츠 활상화시 속성값을 가져와서 선택(한글,알파벳)에 따른 디폴트값을 호출합니다.
    /// </summary>
    /// <param name="속성값"></param>
    private void 속성값호출(ListenInfo.듣고익히기속성값 듣고익히기속성값, ModelInfo.공부하기속성값 공부하기속성값)
    {
        듣고익히기속성 = 듣고익히기속성값;
        공부하기속성 = 공부하기속성값;
        인식체크 = true;

        //한글일 경우
        if (한글 && !알파벳)
        {
            알파벳오브젝트상태();
            듣기사운드출력();

        }
        //알파벳일 경우
        else if (알파벳 && !한글)
        {
            알파벳오브젝트상태();
            알파벳사운드출력();
        }
    }

    /// <summary>
    /// 권기영 : 한글모드에서 듣고 익히기가 체크되어있을 경우 컨텐츠 인식시 
    ///          모델링에 해당하는 속성값호출 함수를 부릅니다.
    /// </summary>
    public void 듣고익히기_정보출력()
    {
        if (TargetManager.타깃메니저.듣고익히기사용)
        {
            int 인덱스 = TargetManager.타깃메니저.복제모델링인덱스;
            GameObject 모델링 = TargetManager.타깃메니저.에셋번들복제컨텐츠[인덱스];

            속성값호출(모델링.GetComponent<ListenInfo>().듣고익히기속성, 모델링.GetComponent<ModelInfo>().공부하기속성);
        }
    }

    /// <summary>
    /// 권기영 : 한글모드에서 한국어 단어 사운드출력 함수입니다.
    /// </summary>
    public void 듣기사운드출력()
    {
        현재낱말사운드 = -1;
        딜레이시간 = 딜레이시간_보관용;
        듣고익히기사운드.GetComponent<AudioSource>().clip = 공부하기속성.한국어;
        듣고익히기사운드.GetComponent<AudioSource>().Play();
    }

    /// <summary>
    /// 권기영 : 한글모드에서 낱말 사운드출력 함수입니다.
    /// </summary>
    public void 듣고익히기사운드출력()
    {
        현재낱말사운드 = 0;
        딜레이시간 = 딜레이시간_보관용;
        한글낱말사운드(현재낱말사운드);
    }


    /// <summary>
    /// 권기영 : 알파벳모드에서 알파벳(ex : a, b ,c사운드)사운드출력 함수입니다.
    /// </summary>
    public void 알파벳사운드출력()
    {
        딜레이시간 = 딜레이시간_보관용;
        알파벳사운드(듣고익히기속성.알파벳사운드);
    }


    /// <summary>
    /// 권기영 : 알파벳, 한글 모드에 따른 초기체크박스 설정 및 UI 상태 초기화 함수입니다.
    /// </summary>
    private void 알파벳오브젝트상태()
    {
        if (한글)
        {
            MainUI.메인.한글듣고익히기UI.SetActive(true);
            듣기버튼.GetComponent<UIToggle>().value = true;
        }
        else
        {
            MainUI.메인.알파벳듣고익히기UI.SetActive(true);
            영어버튼.GetComponent<UIToggle>().value = true;
        }
    }


    /// <summary>
    /// 권기영 : 한글모드에서 한글낱말사운드 출력함수
    /// </summary>
    /// <param name="사운드번호"></param>
    private void 한글낱말사운드(int 사운드번호)
    {
        AudioClip[] 한글낱말 = 듣고익히기속성.한글낱말사운드;

        if (한글낱말 != null)
        {
            사운드개수 = 한글낱말.Length - 1;

            듣고익히기사운드.GetComponent<AudioSource>().clip = 한글낱말[사운드번호];
            듣고익히기사운드.GetComponent<AudioSource>().Play();
        }
    }



    /// <summary>
    /// 권기영 : 알파벳모드에서 사용되어지는 알파벳, 단어에 대한 사운드출력 함수입니다.
    /// </summary>
    /// <param name="단어"></param>
    private void 알파벳사운드(AudioClip 단어)
    {
        if (단어 != null)
        {
            듣고익히기사운드.GetComponent<AudioSource>().clip = 단어;
            듣고익히기사운드.GetComponent<AudioSource>().Play();

            //권기영 : 현재 사운드가 알파벳사운드일 경우
            if (단어 == 듣고익히기속성.알파벳사운드)
            {
                알파벳사운드순서체크 = true;
            }
            //권기영 : 현재 사운드가 단어사운드일 경우
            else
            {
                알파벳사운드순서체크 = false;
            }
        }
    }


    /// <summary>
    /// 권기영 : 알파벳모드에서 사운드를 순서대로 출력해주는 함수입니다. 
    /// </summary>
    private void 알파벳구분()
    {
        //권기영 : 현재 듣고익히기사운드가 출력중이 아닐경우 
        if (듣고익히기사운드.GetComponent<AudioSource>().isPlaying == false)
        {
            //권기영 : 현재 재생 사운드가 알파벳사운드일 경우
            if (알파벳사운드순서체크 == true)
            {
                딜레이시간 -= Time.fixedDeltaTime;
                //권기영 : 딜레이시간 후 체크되어있는 체크박스에 해당하는 사운드 출력
                if (딜레이시간 <= 0)
                {
                    if (영어버튼.GetComponent<UIToggle>().value == true)
                    {
                        알파벳사운드(공부하기속성.영어);
                    }
                    else if (한국어버튼.GetComponent<UIToggle>().value == true)
                    {
                        알파벳사운드(공부하기속성.한국어);
                    }
                    else if (중국어버튼.GetComponent<UIToggle>().value == true)
                    {
                        알파벳사운드(공부하기속성.중국어);
                    }
                }
            }
            //권기영 : 현재 재생 사운드가 단어사운드일 경우 
            else
            {
                //권기영 : 현재 듣고익히기사운드가 출력중이 아닐경우 
                if (듣고익히기사운드.GetComponent<AudioSource>().isPlaying == false)
                {
                    //권기영 : 세개의 언어 체크박스의 상태중 1개라도 체크되어 있다면
                    if (영어버튼.GetComponent<UIToggle>().value || 한국어버튼.GetComponent<UIToggle>().value || 중국어버튼.GetComponent<UIToggle>().value)
                    {
                        //권기영 : 딜레이 초기화 후 알파벳사운드를 출력하는 함수실행
                        알파벳사운드출력();
                    }
                }
            }
        }
    }

    /// <summary>
    /// 권기영 : 한글모드에서 사운드를 순서대로 출력해주는 함수입니다. 
    /// </summary>
    private void 한글구분()
    {
        // 권기영 : 듣고 익히기 버튼이 체크되어 있을 경우
        if (듣고익히기버튼.GetComponent<UIToggle>().value == true)
        {
            // 권기영 : 현재 듣고익히기 사운드가 멈춰있다면
            if (듣고익히기사운드.GetComponent<AudioSource>().isPlaying == false)
            {
                // 권기영 : 현재 낱말사운드가 총사운드개수보다 낮은 인덱스라면
                if (현재낱말사운드 < 사운드개수)
                {
                    // 권기영 : 듣고익히기 마지막에 단어사운드 출력 후
                    if (현재낱말사운드 == -1)
                    {
                        딜레이시간 -= Time.fixedDeltaTime;

                        //권기영 : 딜레시간이 지난 후 다시 0번 낱말사운드 실행
                        if (딜레이시간 <= 0)
                        {
                            현재낱말사운드++;
                            한글낱말사운드(현재낱말사운드);
                        }
                    }
                    // 권기영 : 다음 낱말 사운드 실행
                    else
                    {
                        현재낱말사운드++;
                        한글낱말사운드(현재낱말사운드);
                    }
                }
                // 권기영 : 낱말사운드를 모두 재생했다면 단어 사운드 실행
                else
                {
                    //권기영 : 단어 사운드 실행, 현재낱말사운드값 -1로 초기화
                    듣기사운드출력();
                }
            }
        }
        // 권기영 : 듣고익히기버튼이 비활성화(듣기 버튼이 체크되어 있는 경우)일 경우
        else 
        {
            if(듣고익히기사운드.GetComponent<AudioSource>().isPlaying == false)
            딜레이시간 -= Time.fixedDeltaTime;

            //권기영 : 듣기사운드 출력
            if (딜레이시간 <= 0)
            {
                듣기사운드출력();
            }
        }
    }



}

