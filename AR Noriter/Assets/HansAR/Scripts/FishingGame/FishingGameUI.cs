using UnityEngine;
using System;
using System.Collections;

public class FishingGameUI : Singleton<FishingGameUI>
{
    [Serializable]
    public class FishingGaugeUI
    {
        public GameObject fishingGaugeUI;
        public GameObject fishingGaugeBar;
        public GameObject fishingWheelBar;
        public GameObject touchImgUI;
        public GameObject leftTouchImg;
        public GameObject rightTouchImg;
        public GameObject leftShadowTouchImg;
        public GameObject rightShadowTouchImg;
    }

    [Serializable]
    public class RodUI
    {
        public GameObject rodUI;                       // 낚시대 UI
        public GameObject rodBtn;                      // 낚시대 버튼
    }

    [Serializable]
    public class ResultUI
    {
        public GameObject resultUI;                     // 결과창
        public GameObject prevBtn;                      // 이전 stage 버튼
        public GameObject nextBtn;                      // 다음 stage 버튼
    }

    [Serializable]
    public class FishingEndUI
    {
        public GameObject endFishingUI;                 // 낚시 결과UI
        public UILabel endFishLabel;                    // 낚시 결과 물고기 라벨
        public UILabel lengthLabel;                     // 낚시 결과 물고기 크기(길이)
    }

    [Serializable]
    public class FishingEventUI
    {
        public GameObject eventUI;                      // 이벤트 발생 ui
        public GameObject leftTextObj;                  // 좌측 등장 텍스트 img or img
        public GameObject rightTextObj;                 // 우측 등장 텍스트 img or img
        public GameObject leftMoveTargetObj;            // 좌측 등장 텍스트 목적지
        public GameObject rightMoveTargetObj;            // 우측 등장 텍스트 목적지

        public Vector3 leftTextObjFirstPos;             // 좌측 등장 텍스트 img or img pos
        public Vector3 rightTextObjFirstPos;            // 우측 등장 텍스트 img or img pos
    }

    [Serializable]
    public class PauseUI
    {
        public GameObject pausePopUpUI;                  // 일시정지 UI
        public GameObject pauseBtn;                      // 일시정지 버튼
    }

    [Serializable]
    public class EmblemUI
    {
        public GameObject emblemUI;                      // 앰블럼 UI
        public UISprite emblemSpr;                       // 앰블럼 sprite
        public UILabel emblemLabel;                      // 앰블럼 textLabel
        public UISprite emblemInsideBg;                  // 안쪽 배경
        public UISprite emblemOutsideBg;                 // 전체 바탕 배경
    }

    [Serializable]
    public class ObjRootUI
    {
        public GameObject objRootUI;                     // 3D 오브젝트 상위 오브젝트
        public GameObject resultPanelPosObj;             // 3D result 오브젝트 2D에서 부모 오브젝트
        public GameObject CopyTamiPosObj;                // 3D Tami 오브젝트 2D에서 부모 오브젝트
        public GameObject CopyFishPosObj;                // 3D Fish 오브젝트 2D에서 부모 오브젝트
        public GameObject CopyTrapPosObj;                // 3D Trap 오브젝트 2D에서 부모 오브젝트
    }

    [Serializable]
    public class CollectionUI
    {
        public GameObject collectUI;                      // 수집 리스트 UI 
        public GameObject collectBtn;                     // 수집 ui 열기 버튼 
        public GameObject collectListRoot;                // 수집 리스트 위치(부모 오브젝트)
        public GameObject collectTamiRoot;                // 수집 타미 오브젝트 위치
        public GameObject collectShadowRoot;              // 수집 리스트 물고기 그림자 스프라이트
        public GameObject collectNewfishLabelRoot;        // 새로운 물고기 잡았을 경우 리스트에서 new 표시
        public GameObject collectNewfishObj;              // 새로운 물고기 잡았을 경우 표시 오브젝트
    }

    public GameObject fishingUiRoot;                     // UI Root
    public GameObject touchArea;                         // 터치 영역

    //public GameObject fishingGauge;                      // 낚시 게이지
    //public GameObject fishingRodBtn;                     // 낚싯대 버튼
    public GameObject fishRemnantUI;                     // 남은 물고기 표시 ui obj
    public UILabel fishRemnantLabel;                     // 남은 물고기 표시 label
    //public GameObject zoomCamObj;                        // zoom cam 오브젝트
    public UISprite weatherScreen;                       // 태풍맵의 경우에 화면을 어둡게 해주기 위한 sprite
    public GameObject catchFishBg;                       // 잡지않은 물고기 표시(물고기 수)

    [SerializeField]
    public RodUI rodUi;

    [SerializeField]
    public ResultUI result;

    [SerializeField]
    public ObjRootUI rootUI;

    [SerializeField]
    public CollectionUI collectionUI;

    [SerializeField]
    public FishingEndUI sucFishingEndUi;

    [SerializeField]
    public FishingEndUI failFishingEndUi;

    [SerializeField]
    public FishingEndUI trapFishingEndUi;

    [SerializeField]
    public FishingEventUI eventUi;

    [SerializeField]
    public PauseUI pauseUi;

    [SerializeField]
    public EmblemUI emblemUi;

    [SerializeField]
    public FishingGaugeUI fishingGauage;

    public GameObject[] catchFish;                      // 잡은 물고기 UI


    private void Start()
    {
        initUi();
        EventUiPosSave();
    }

    public void InitFishingGameUI()
    {
        for (int i = 0; i < catchFish.Length; i++)
        {
            catchFish[i].SetActive(false);
        }

        catchFishBg.SetActive(false);
    }

    public void initUi()
    {
        pauseUi.pausePopUpUI.SetActive(false);
        pauseUi.pauseBtn.SetActive(false);
        //zoomCamObj.SetActive(false);
        touchArea.SetActive(false);
        fishingGauage.fishingGaugeUI.SetActive(false);
        fishingGauage.touchImgUI.SetActive(false);
        rodUi.rodBtn.SetActive(false);
        rootUI.objRootUI.SetActive(false);
        fishRemnantUI.SetActive(false);
        
        result.resultUI.SetActive(false);
        emblemUi.emblemUI.SetActive(false);

        collectionUI.collectUI.SetActive(false);
        collectionUI.collectBtn.SetActive(false);
        collectionUI.collectNewfishObj.SetActive(false);
        
        sucFishingEndUi.endFishingUI.SetActive(false);
        failFishingEndUi.endFishingUI.SetActive(false);
        trapFishingEndUi.endFishingUI.SetActive(false);
        
        weatherScreen.alpha = 0;

        InitFishingGameUI();
    }

    public void StartUiSet()
    {
        pauseUi.pausePopUpUI.SetActive(false);
        //zoomCamObj.SetActive(false);
        touchArea.SetActive(false);
        fishingGauage.fishingGaugeUI.SetActive(false);
        fishingGauage.touchImgUI.SetActive(false);

        result.resultUI.SetActive(false);
        emblemUi.emblemUI.SetActive(false);

        collectionUI.collectUI.SetActive(false);
        
        sucFishingEndUi.endFishingUI.SetActive(false);
        failFishingEndUi.endFishingUI.SetActive(false);
        trapFishingEndUi.endFishingUI.SetActive(false);
        rodUi.rodBtn.SetActive(false);

        InitFishingGameUI();

        fishingUiRoot.SetActive(true);
        
        fishRemnantUI.SetActive(true);
        pauseUi.pauseBtn.SetActive(true);

        collectionUI.collectBtn.SetActive(true);
        collectionUI.collectNewfishObj.SetActive(false);

        catchFishBg.SetActive(true);
        rootUI.objRootUI.SetActive(true);
    }

    private void EventUiPosSave()
    {
        eventUi.rightTextObjFirstPos = eventUi.rightTextObj.transform.localPosition;
        eventUi.leftTextObjFirstPos = eventUi.leftTextObj.transform.localPosition;
    }
}
