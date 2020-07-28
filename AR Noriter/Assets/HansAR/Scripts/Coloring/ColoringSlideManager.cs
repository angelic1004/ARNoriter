using UnityEngine;
using System.Collections;

public class ColoringSlideManager : MonoBehaviour
{
    public static ColoringSlideManager csManager;

    // 색칠 슬라이드 사용
    [HideInInspector]
    public bool useColoringSlide;

    // 슬라이드 저장할 전체 크기
    public static int totalCount = 15;

    [HideInInspector]
    public int saveCount = 0;                      // 현재 저장 카운트
    int slideCount = 0;                     // 슬라이드 카운트
    int initCount = 0;                      // 초기화 카운트

    bool isLeft;                            // 왼쪽 오른쪽 구분

    // 슬라이드 저장할 배열
    private int[] slideNumSave = new int[totalCount];

    // 텍스쳐 저장 배열
    private Texture2D[] slideTextureSave = new Texture2D[totalCount];

    public GameObject colorOverlayUI;

	void Start ()
    {
        colorOverlayUI.SetActive(false);

        csManager = this;

        // 배열 초기화
        for (int i = 0; i < slideNumSave.Length; i++)
        {
            slideNumSave[i] = -1;
        }

        if (ColoringManager.컬러링매니저.slideTypeSave)
        {
            // 색칠슬라이드 사용시 타깃메니저 슬라이드 비활성화
            useColoringSlide = true;
            TargetManager.타깃메니저.슬라이드사용 = false;
            MainUI.메인.오버레이UI.SetActive(false);
        }
        else if (TargetManager.타깃메니저.스케치씬사용)
        {
            //if (ColoringManager.컬러링매니저.buttonTypeSave && !TargetManager.타깃메니저.지원공주미니맵사용 && !TargetManager.타깃메니저.UsedMiniMap)
            if (ColoringManager.컬러링매니저.buttonTypeSave)
            {
                TargetManager.타깃메니저.슬라이드사용 = true;
            }
        }
    }

    void FixedUpdate()
    {
        // 마커 인식시 slideCount 초기화
        if (TargetManager.trackableStatus && initCount != slideCount)
        {
            slideCount = initCount;
        }
    }

    // 버튼 스케치 저장 중복체크
    void OverlapCheck()
    {
        if (ColoringManager.컬러링매니저.buttonTypeSave && useColoringSlide)
        {
            Debug.Log("ERROR : 색칠저장 버튼방식과 슬라이드 방식이 동시에 켜졌습니다.");
            Time.timeScale = 0;
        }
    }

    // 텍스쳐 저장
    public void TextureSave(int _index, Texture2D _texture)
    {
        if (initCount == slideNumSave.Length)
        {
            initCount = 0;
        }

        // 저장된 텍스쳐가 있으면 제거
        if(slideTextureSave[initCount] != null)
        {
            Destroy(slideTextureSave[initCount]);
        }

        // 저장
        slideNumSave[initCount] = _index;
        slideTextureSave[initCount] = _texture;

        // 현재 저장 카운트가 배열길이보다 작으면
        if (saveCount < slideNumSave.Length)
        {
            // 저장카운트와 초기화카운트 증가
            saveCount++;
            initCount++;
        }
        else
        {
            initCount++;
        }
    }

    // 왼쪽버튼 클릭함수
    public void ClickLeft()
    {
        // 회전적용
        if (slideCount - 2 < 0)
        {
            slideCount = saveCount + 1;
        }

        int index = slideNumSave[slideCount - 2];
        isLeft = true;

        TargetRefresh(index);
    }

    // 오른쪽버튼 클릭함수
    public void ClickRight()
    {
        // 회전적용
        if (slideCount == saveCount)
        {
            slideCount = 0;
        }

        int index = slideNumSave[slideCount];
        isLeft = false;

        TargetRefresh(index);
    }

    // 클릭한 타겟으로 새로고침 합니다.
    void TargetRefresh(int _index)
    {
        // 타겟 새로 설정
        GameObject 오브젝트 = TargetManager.타깃메니저.에셋번들복제컨텐츠[_index];
        ColoringManager.컬러링매니저.modelObj = 오브젝트.GetComponent<ModelInfo>().애니메이션타겟;
        ColoringManager.컬러링매니저.coloringMat = 오브젝트.GetComponent<ColoringInfo>().색칠하기속성.색칠머테리얼;
        TouchEventManager.터치.기준콜라이더 =
            오브젝트.GetComponent<ColoringInfo>().색칠하기속성.콜라이더;

        // 색칠머테리얼에 적용
        if (isLeft)
        {
            ColoringManager.컬러링매니저.coloringMat.mainTexture = slideTextureSave[slideCount - 2];
        }
        else
        {
            ColoringManager.컬러링매니저.coloringMat.mainTexture = slideTextureSave[slideCount];
        }

        TargetManager.타깃메니저.HideAllModelingContents();
        TargetManager.타깃메니저.슬라이드컨텐츠_보이기(_index);

        DubbingManager.더빙.더빙사운드_중지();
        AnimationManager.애니메이션.애니메이션01_재생_슬라이드_탐색();

        // 슬라이드 카운트 조정
        if (isLeft)
        {
            slideCount--;
        }
        else
        {
            slideCount++;
        }
    }

    // 텍스쳐 파괴
    public void DestoryTextures()
    {
        for (int i = 0; i < slideTextureSave.Length; i++)
        {
            Destroy(slideTextureSave[i]);
        }
    }
}