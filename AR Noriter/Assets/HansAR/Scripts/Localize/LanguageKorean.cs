using UnityEngine;
using System;

public class LanguageKorean
{
    #region 한글 주석

    // TODO : 여기에 지역화한 값을 추가합니다.
    // 상수, 정적 변수 또는 정적 속성을 선언할 수 있습니다.
    //
    // public static readonly string ConfirmDelete = "삭제하시겠습니까?";
    // public static readonly string AnswerYes = "예";
    // public static string AnswerNo { get { return "아니오"; } }
    //
    // 스크립트 내에서 사용하는 방법 :
    // 1. 지역화 ID로 가져오기
    // - 선언된 값 이름을 LocalizeID에 추가합니다.
    // - Debug.Log(LocalizeText.Value[LocalizeID.ConfirmDelete]);
    // 2. 값 이름으로 가져오기
    // - Debug.Log(LocalizeText.Value["ConfirmDelete"]);
    //
    // UI 텍스트를 지역화하는 방법 :
    // 1. NGUI의 UILabel(또는 UnityEngine.UI.Text)에 LocalizeText 컴포넌트를 추가합니다.
    // 2. LocalizeText 컴포넌트의 "Value Name" 속성에 값 이름을 입력합니다.
    // 3. LocalizeManager를 씬에 추가하면 씬의 모든 UI 텍스트가 런타임에 변경됩니다.

    #endregion

    // 주의사항 : 영어 이외의 언어는 const로 선언할 경우 iOS 디바이스에서 문자열이 잘리는 문제가 있다.
    // 따라서, static 또는 static readonly로 선언하거나 const를 간접 참조하도록 해야한다.

    public static readonly string LocalizeSelect = "언어 선택";
    public static readonly string SaveLocalizeSetting = "선택한 언어를 기본언어로 사용";

    // Pop-up
    public static readonly string Exit_Q = "앱을 종료 하시겠습니까?";
    public static readonly string Exit_A = "종료";
    public static readonly string Cancel = "취소";
    public static readonly string Return_Q = "메인화면으로 돌아가시겠습니까?";
    public static readonly string Return_A = "뒤로가기";
    public static readonly string Start = "시작";
    public static readonly string Exit_Restart = "종료 또는 재시작 하시겠습니까?";

    public static readonly string Result_Prev = "이전 단계";
    public static readonly string Result_Restart = "다시 하기";
    public static readonly string Result_End = "끝내기";
    public static readonly string Result_Next = "다음 단계";

    public static readonly string Pause = "일시정지";

    // Contents
    public static readonly string ReadContents = "콘텐츠를 읽고 있습니다.";
    public static readonly string LoadContents = "콘텐츠를 불러오는중...";
    public static readonly string IlluminateImage = "\"이미지를 비추어 보세요\"";

    public static readonly string SketchRecognition_Default = "\"색칠한 그림을 비추어 보세요.\"";
    public static readonly string SketchRecognition_OnTarget = "\"사각형 전체가 보이도록\n그림을 비추어 주세요.\"";
    public static readonly string NotUsedSketchTargetForThisScene = "\"색칠한 그림은 색칠하기를 실행하세요.\"";
    public static readonly string NotUsed4DTargetForThisScene = "\"4D 그림은 4D 체험하기를 실행하세요.\"";

    // Buttons
    public static readonly string Continue = "연속";
    public static readonly string Question = "질문";
    public static readonly string Answer = "답변";

    public static readonly string PreferredLanguage = "원하시는 언어를 선택해주세요.";

    public static readonly string ENG = "영어";
    public static readonly string KOR = "한국어";
    public static readonly string CHN = "중국어";
    public static readonly string JPN = "일본어";
    public static readonly string VTN = "베트남어";
    public static readonly string Indonesian = "인도네시아어";

    public static readonly string Listen = "듣기";
    public static readonly string ListenAndLearn = "듣고익히기";

    // 데이터사용
    public static readonly string Wifi_NotConnected = "WI-FI 연결이 되어 있지 않습니다.";
    public static readonly string MobileDatacall = "이동 통신망(3G/LTE 등)으로 접속시 데이터 통화료가 발생할 수 있습니다.";
    public static readonly string MobileDatacall_F = "이동 통신망(3G/LTE 등)으로 접속시 데이터 통화료가 발생할 수 있습니다.\n             (데이터량: {0}MB)";
    public static readonly string Download = "다운로드";
    public static readonly string Back = "돌아가기";
    public static readonly string Wifi_NotGood = "WI-FI, 3G/LTE 네트워크 연결이 양호하지 않습니다.";

    public static readonly string ShowOff_Q = "다시보지 않으시겠습니까?";

    // ProgressBar
    public static readonly string ProgressBarComment = "잠시만 기다려주세요.";

    // QR코드
    public static readonly string ScanQR = "\"QR코드를 스캔하세요.\"";
    public static readonly string CheckingQR = "QR코드 확인중...";
    public static readonly string OK = "확인";
    public static readonly string Net_Unable = "인터넷에 연결할 수 없습니다.";
    public static readonly string QR_Unregistered = "등록되지 않은 시리얼 번호!\n\n스캔한 QR코드는 발행된 적이 없거나 이미 삭제되었습니다.";
    public static readonly string QR_Expired = "만료된 시리얼 번호!\n\n스캔한 QR코드는 지정된 사용 횟수를 초과했습니다.";
    public static readonly string QR_Invalid = "잘못된 시리얼 번호!\n\n스캔한 QR코드는 이 제품에서는 사용할 수 없습니다.";
    public static readonly string QR_Unsuitable = "적합하지 않은 시리얼 번호!\n\n스캔한 QR코드는 원하시는 기능과 호환되지 않습니다.";
    public static readonly string DB_Error = "데이터베이스 오류!";
    public static readonly string Server_Error = "서버 오류!";
    public static readonly string QR_Success = "제품 등록이 완료되었습니다.";
    public static readonly string UsedExpChance = "체험 횟수를 모두 사용하였습니다.";

    public static readonly string Pass_Recognized = "타겟이 인식 되었습니다.";
    public static readonly string Pass_Unrecognized = "타겟이 인식 되지 않았습니다.";

    // AssetDownloadManager.cs
    public static readonly string Net_Unstable = "현재 네트워크 상태가 불안정 합니다.";
    public static readonly string Net_ReturnMain = "다운로드를 중지하고, 메인화면으로 돌아가시겠습니까?";
    public static readonly string DownloadTotal_F = "콘텐츠를 다운로드 중입니다. (총 {0}MB)";
    public static readonly string DownloadProgress_F = "콘텐츠 다운로드중 {0}MB / {1}MB 진행중..";
    public static readonly string DownloadProgress2_F = "콘텐츠 다운로드중 \\n {0}MB / {1}MB";
    public static readonly string Net_Retry = "서버와의 연결 상태가 원활하지 않습니다. 잠시 후 다시 시도하십시오.";
    public static readonly string Net_UpdateFinish = "콘텐츠 업데이트가 완료 되었습니다.";
    public static readonly string Net_CheckVer = "콘텐츠 버전을 확인 중입니다.";

    // RecordScreenUI.cs
    public static readonly string Rec_Screen = "화면 녹화중입니다.";
    public static readonly string Rec_Finish = "화면 녹화가 완료되었습니다.";
    public static readonly string Rec_GrantRights = "녹화 권한을 부여받지 못했습니다!";
    public static readonly string Rec_NoMic = "마이크 장치가 없습니다!";
    public static readonly string Rec_NotSupport = "화면 녹화를 지원하지 않는 장치입니다!";
    public static readonly string Rec_SaveAudio = "오디오 파일을 저장하고 있습니다.";
    public static readonly string Rec_SaveAudioFailed = "오디오 파일 저장에 실패하였습니다!";
    public static readonly string Rec_SaveVideoFailed = "동영상 파일 저장에 실패하였습니다!";
    public static readonly string Rec_SaveVideo = "동영상 파일을 저장하고 있습니다.";
    public static readonly string Rec_NoFolder = "저장된 동영상 폴더가 없습니다!";
    public static readonly string Rec_NoVideo = "저장된 동영상 파일이 없습니다!";

    // MainUI.cs
    public static readonly string TargetCount_F = "인식가능 타겟 개수 : ";

    // ExploreManager.cs
    public static readonly string AutoSearchOn = "자동 탐색모드가 설정 되었습니다.";
    public static readonly string AutoSearchOff = "자동 탐색모드가 해제 되었습니다.";

    // 카테고리
    public static readonly string Vehicle = "탈것";
    public static readonly string Animal = "동물";
    public static readonly string Dino = "공룡";
    public static readonly string Insect = "곤충";
    public static readonly string Princess = "공주";
    public static readonly string RacingCar = "레이싱";
    public static readonly string Soccer = "축구";
    public static readonly string City = "도시";
    public static readonly string Alphabet = "알파벳";
    public static readonly string Sea = "바다";

    public static readonly string FourD = "4D 체험하기";
    public static readonly string Coloring = "색칠하기";
    public static readonly string Repeat = "복습하기";
    public static readonly string Puzzle = "퍼즐";
    public static readonly string PuzzleGame = "퍼즐스티커";
    public static readonly string QuizQuiz = "퀴즈퀴즈";
    public static readonly string Sticker = "스티커 붙이기";
    public static readonly string Matching = "매칭";
    public static readonly string Word = "단어 맞추기";
    public static readonly string Maze = "미로";
    public static readonly string Kart = "운전";
    public static readonly string Paint = "페인트";
    public static readonly string PostCard = "엽서";
    public static readonly string Fishing = "낚시";


    public static readonly string Observe = "관찰하기";
    public static readonly string Study = "공부하기";
    public static readonly string Language = "공부하기";
    public static readonly string QnA = "묻고 답하기";
    public static readonly string Conversation = "간단회화";
    public static readonly string MiniMap00 = "미니맵";

    public static readonly string Sketch = "스케치";
    public static readonly string MiniMap01 = "미니맵";
    public static readonly string LookNFind_Vehicle = "우리마을 탈것";
    public static readonly string LookNFind_Animal = "동물원";
    public static readonly string LookNFind_Dino = "공룡시대";
    public static readonly string LookNFind_Princess = "공주 그림자";

    public static readonly string Sticker_Dino = "어떤 공룡일까";
    public static readonly string PrincessCrane = "인형 뽑기";

    public static readonly string SoccerGame = "축구 게임";
    public static readonly string FourDRacingGame = "4D 레이싱 게임";
    public static readonly string RacingDrive = "레이싱 운전하기";
    public static readonly string Video = "동영상";

    public static readonly string Black = "검정";
    public static readonly string Pink = "핑크";
    public static readonly string SkyBox = "배경";

    public static readonly string Observe_Animal = "관찰하기_동물";
    public static readonly string Observe_Object1 = "관찰하기_사물1";
    public static readonly string Observe_Object2 = "관찰하기_사물2";
    public static readonly string Observe_Vegetable = "관찰하기_야채";

    //NavigationUI.cs
    public static readonly string MinimapReal = "4D 체험하기 미니맵";
    public static readonly string MinimapSketch = "스케치 미니맵";
    public static readonly string Color = "색칠하기";
    public static readonly string Other = "그 외";
    public static readonly string RacingGame = "4D 레이싱 게임";

    public static readonly string ArPlay = "AR 플레이";
    public static readonly string DanceDance = "댄스 댄스";
    public static readonly string DanceBattle = "댄스 배틀";
    public static readonly string Runway = "런웨이";
    public static readonly string ColorRunway = "컬러 런웨이";

    public static readonly string Manual = "매뉴얼";
    public static readonly string Category = "카테고리";
    public static readonly string LanguageSelect = "언어 선택";
    public static readonly string Sound = "사운드 설정";

    public static readonly string Dance = "댄스";
    public static readonly string Battle = "배틀";

    public static readonly string Game = "게임";

    //단계표시
    public static readonly string Stage_One = "1단계";
    public static readonly string Stage_Two = "2단계";
    public static readonly string Stage_Three = "3단계";
    public static readonly string Stage_Four = "4단계";
    public static readonly string Stage_Five = "5단계";

    //PrincessManager.cs 
    public static readonly string ClowQuestion = "오른쪽과 같은 공주를 찾아주세요";
    public static readonly string ClowGameOver = "모든 문제를 푸셨습니다. 축하드려요~!";
    public static readonly string ClowSelectAnswer = "정답이에요! 참 잘했어요!!";
    public static readonly string ClowSelectWrong = "정말 아쉽다~ 다시한번 찾아봐요~!";
    public static readonly string ClowRestart = "인형뽑기를 다시 시작해요~!";
    public static readonly string Restart = "재시작";


    //퍼즐 
    public static readonly string MatchingNumber = "맞춘 갯수";
    public static readonly string ChallengeNumber = "뒤집은 횟수";


    // Dance Warning Message
    public static readonly string DanceCharacterSelectMsg = "캐릭터를 선택하기 전에 위치를 먼저 선택 하세요.";

    // 공주 Dance
    public static readonly string ChooseDancerNumber = "댄서 인원을 선택해 주세요.";
    public static readonly string ChooseDancer = "댄서를 선택해 주세요.";
    public static readonly string DanceDancePopupMsg = "타겟이 인식되었습니다. 댄스댄스를 종료합니까?";
    public static readonly string DanceBattlePopupMsg = "타겟이 인식되었습니다. 댄스배틀을 종료합니까?";

    // 공주 스티커
    public static readonly string PrincessShadowSticker = "공주 그림자 스티커";
    public static readonly string PrincessBallroom = "왕국 무도회장에 아름다운 공주님들이 모였어요. 어떤 공주님들이 오셨는지 스티커를 붙여보세요!";
    public static readonly string StickPuzzleSticker = "퍼즐스티커 붙이기";
    public static readonly string SuitablePuzzle = "빈 공간에 알맞은 퍼즐 스티커를 찾아 붙여 그림을 완성해 보세요!";
    public static readonly string IamOdette = "내 이름은 오데트야! 우아한 몸짓으로 아름다운 발레를 할 수 있지.";
    public static readonly string IamMermaid = "지느러미로 바다를 자유롭게 헤엄치는 나는 인어공주야!";

    public static readonly string SnowWhite = "백설공주";
    public static readonly string PrincessNavia = "나비아공주";
    public static readonly string PrincessBari = "바리공주";
    public static readonly string PrincessBell = "벨";
    public static readonly string Cinderella = "신데렐라";
    public static readonly string Cleopatra = "클레오파트라";
    public static readonly string Odette = "오데트";
    public static readonly string Rapunzel = "라푼젤";
    public static readonly string Thumbelina = "엄지공주";
    public static readonly string PrincessMermaid = "인어공주";

    // 4D 레이싱 게임
    public static readonly string RacingWinner = "당신은 1등입니다. 축하드립니다!";
    public static readonly string RacingStart = "타겟에서 카메라가 벗어나면 레이싱 게임이 시작됩니다.";
    public static readonly string LapTime = "Lap Time";
    public static readonly string Speed = "Speed";

    // 축구 게임
    public static readonly string StartSoccerGame = "이미지를 인식해 보세요! 축구 게임이 시작됩니다!";
    public static readonly string StartSoccerVideo = "이미지를 인식해 보세요! 축구 영상을 볼 수 있습니다!";
    public static readonly string SelectGameMode = "원하는 경기 방식을 선택해 주세요!";
    public static readonly string PenaltyKick = "패널티킥";
    public static readonly string FreeKick = "프리킥";
    public static readonly string FreeKick_Explain = "수비벽을 넘어 슛을 성공 시켜야 하는 아찔한 프리킥";
    public static readonly string KickerNumber = "경기에 참가할 키커의 수를 선택해 주세요";

    public static readonly string InputYourTeam = "당신의 팀명을 입력해주세요";
    public static readonly string InputTeam = "팀명을 입력해주세요";
    public static readonly string InputTeamA = "A 팀의 팀명을 입력해주세요";
    public static readonly string InputTeamB = "B 팀의 팀명을 입력해주세요";
    public static readonly string NoTeamname = "입력된 팀명이 없습니다. 다시 입력해 주세요";
    public static readonly string SameTeam = "중복된 팀명 입니다. 다시 입력해 주세요";
    public static readonly string SameTeamA = "입력된 팀명이 A 팀의 팀명과 같습니다 다시 입력해 주세요";
    public static readonly string SameTeamB = "입력된 팀명이 B 팀의 팀명과 같습니다 다시 입력해 주세요";
    public static readonly string SelectUniform = "유니폼을 고르세요";
    public static readonly string TeamAUniform = "A 팀의 유니폼을 고르세요";
    public static readonly string TeamBUniform = "B 팀의 유니폼을 고르세요";
    public static readonly string SameUniform = "상대팀과 같은 유니폼은 선택 할 수 없습니다.";
    public static readonly string SelectNumberPlayer = "플레이어 수 선택";
    public static readonly string Select = "선택";
    public static readonly string ChooseCharacter = "캐릭터 선택";
    public static readonly string SelectPlayer = "선수를 선택해 주세요";
    public static readonly string TeamAPlayer = "A 팀 선수를 선택해 주세요";
    public static readonly string TeamBPlayer = "B 팀 선수를 선택해 주세요";

    public static readonly string Scout = "영입하기";
    public static readonly string ScoutCancel = "영입 취소";

    public static readonly string SoccerRestart = "다시 시작하시겠습니까?";

    public static readonly string Shoot = "슛";
    public static readonly string NormalShoot = "기본";
    public static readonly string GroundShoot = "땅볼";
    public static readonly string ToeShoot = "찍어차기";
    public static readonly string InsideShoot = "감아차기";

    // Watch Car
    public static readonly string WatchCarPlay = "와치카 플레이";
    public static readonly string BattleGame    = "배틀 게임";
    public static readonly string RoadDriving   = "도로 운전하기";

    public static readonly string WarchCarBattlePopupMsg = "타겟이 인식되었습니다. 와치카배틀을 종료합니까?";

    // Watch Car Edu
    public static readonly string WrongWay = "역주행 중입니다.";
    public static readonly string DriveAgain = "다시 운전해 주세요.";

    // Nas Car
    public static readonly string NasCar = "나스카";
    
    // Track
    public static readonly string RacingDrive_RaceTrack01 = "레이싱 운전하기 - 1번 트랙";
    public static readonly string RacingDrive_RaceTrack02 = "레이싱 운전하기 - 2번 트랙";

    //카메라, 회전 튜토리얼
    public static readonly string tuto_camera = "카메라 메뉴를 이용해 사진도 찍고 영상 촬영도 가능해요.";
    public static readonly string tuto_rot = "타깃 인식한 다음에 콘텐츠를 돌려서 살펴볼 수 있어요.";

    //알아보기 튜토리얼
    public static readonly string tuto_lang = "언어 선택 버튼을 클릭하면 여러 나라 언어를 공부할 수 있어요.";
    public static readonly string tuto_ob = "돋보기를 클릭해 무엇인지 알아봐요.";

    //매칭 튜토리얼
    public static readonly string tuto_suc = "맞춘 카드수를 알 수 있어요.";
    public static readonly string tuto_try = "카드 뒤집은 횟수를 알 수 있어요.";
    public static readonly string tuto_cardPos = "카드를 뒤집어 같은 모양을 맞춰봐요.";
    public static readonly string tuto_restart = "종료 혹은 다시 하기를 선택할 수 있어요.";

    //퍼즐 튜토리얼
    public static readonly string tuto_bg = "퍼즐을 채워 넣을 퍼즐 판 이예요.";
    public static readonly string tuto_dragPuzzle = "끌어다 놓을 수 있는 퍼즐 조각이에요.";
    public static readonly string tuto_stage = "현재 단계를 확인할 수 있어요.";

    //색칠하기 튜토리얼
    public static readonly string tuto_colorReset = "마음에 들지 않으면 다시 지울 수 있어요.";

    //페인트 튜토리얼
    public static readonly string tuto_paintSave = "색칠한 그림을 저장할 수 있어요.";
    public static readonly string tuto_paintSelectColor = "색상을 선택하여 색칠할 수 있어요.";
    public static readonly string tuto_paintMiniMap = "색칠한 그림을 저장하고 미니맵에서 확인할 수 있어요.";

    //미니맵 튜토리얼
    public static readonly string tuto_collect = "미니맵에 넣어 볼 수 있어요.";
    public static readonly string tuto_minimapSize = "미니맵을 크고 작게 불 수 있어요.";

    //단어 맞추기 튜토리얼
    public static readonly string tuto_wordList = "단어 제시 부분이에요. 아래 큐브를 클릭해 맞춰보세요!";

    //미로게임 튜토리얼
    public static readonly string tuto_mazeMinimap = "미니맵을 열어 현재 위치를 확인할 수 있어요!";
    public static readonly string tuto_pause = "일시정지 버튼을 누르면 다시 시작하거나 종료할 수 있어요.";

    //카트게임 튜토리얼
    public static readonly string tuto_kartStage = "현재 단계를 확인할 수 있어요.";
    public static readonly string tuto_kartScore = "현재 점수를 확인할 수 있어요.";
    public static readonly string tuto_kartFruit = "먹은 과일 양을 보여줘요. 과일을 모두 모으면 다음 단계로 넘어가요!";
    public static readonly string tuto_kartGauge = "남은 연료량을 표시해줘요. 연료가 다 떨어지면 미션 실패!!";

    //포스트카드 튜토리얼
    public static readonly string tuto_postcardTakePic = "사진촬영 모드 버튼이에요.";
    public static readonly string tuto_postcardAlbumPic = "앨범에서 사진을 가져와요.";
    public static readonly string tuto_postcardAlbumAvi = "앨범에서 동영상을 가져와요.";


    //편지 텍스트박스 StartValue
    public static readonly string letter_textbox = "이곳을 터치하여\n텍스트를 입력해보세요.";

    //MazeSelectUI
    public static readonly string selectMaze = "미로 선택하기";
    public static readonly string mazeCave = "동굴 미로";
    public static readonly string mazeGlassLand = "초원 미로";
    public static readonly string mazeGlacier = "빙하 미로";

    public static readonly string mazeSurface = "수면 미로";
    public static readonly string mazeCrystal = "수정 미로";
    public static readonly string mazeSubmarine= "해저 미로";

    public static readonly string mazeForest = "숲 미로";
    public static readonly string mazeDesert = "사막 미로";
    public static readonly string mazeDungeon = "던전 미로";

    //ResultUI
    public static readonly string retry = "다시하기";
    public static readonly string quitGame = "종료하기";
    public static readonly string nextMaze = "다음미로";
    public static readonly string nextQuestion = "다음문제";

    //MazeGameUI
    public static readonly string introText = "미로에 도착했어요\n 시작버튼을 눌러 게임을 시작해 보아요";
    public static readonly string startText = "게임을 시작합니다~";
    public static readonly string moveText = "이동중이에요~";
    public static readonly string selectArrowText = "갈림길이 나타났어요! 어디로 갈까??";
    public static readonly string deadEndText = "막다른길로 와버렸어요.. 돌아가도록 해요~";
    public static readonly string goalText = "미로를 탈출했어요~! 축하해요!!";
    public static readonly string retryText = "게임을 다시 시작합니다~";

    //fishingGame
    public static readonly string BlowFish = "복어";
    public static readonly string LionFish = "쏠배감펭";
    public static readonly string Raccoon_ButterFly = "라쿤버터플라이";
    public static readonly string ClowFish = "흰동가리";
    public static readonly string BlueTang = "블루탱";
    public static readonly string Beluga = "벨루가";
    public static readonly string NapoleonFish = "나폴레옹피쉬";
    public static readonly string Grey_Nursh_Shark = "청새리상어";
    public static readonly string Porcupine_Fish = "가시복";
    public static readonly string Giant_Grouper = "자이언트 그루퍼";
    public static readonly string Manta_Ray = "만타 가오리";
    public static readonly string BlackTip_Shark = "블랙팁 샤크";

    public static readonly string Can = "캔";
    public static readonly string Clam = "조개";
    public static readonly string Shoes = "신발";
    public static readonly string StarFish = "불가사리";

    public static readonly string normalMap = "호수";
    public static readonly string iceMap = "빙하";
    public static readonly string typhoonMap = "태풍";

    public static readonly string fishing_tuto_00 = "버튼을 눌러 낚시를 시작해요.";
    public static readonly string fishing_tuto_01 = "화면을 연타해 일정 게이지 이상 넘겨야 물고기를 잡아요!";
    public static readonly string fishing_tuto_02 = "물고기를 잡으면 왼쪽에 물고기가 채워져요.";
    public static readonly string fishing_tuto_03 = "잡은 물고기는 어항을 눌러서 확인해볼 수 있어요!";

    //PauseUIs
    public static readonly string askQuit = "종료하시겠습니까?";

    //WordUI
    public static readonly string loading = "로딩중입니다...";

    //RestartPopUp
    public static readonly string RestartGame = "게임을 재시작 하시겠습니까?";

    public static readonly string Saved = "저장 되었습니다.";

    // 알아보기_도시
    public static readonly string Observe_AirPlane =
@"비행기

날개를 이용해 인공적으로 하늘을 나는 능력을 지닌 항공기를 말해요.";

    public static readonly string Observe_Ambulance=
@"구급차

환자를 병원으로 신속하게 병원으로 이송할 때 사용하는 자동차에요.";

    public static readonly string Observe_Helicopter =
@"헬리콥터

날개를 회전시켜서 비행하는 형식의 항공기를 말해요.";

    public static readonly string Observe_DumpTruck =
@"덤프트럭

적재함이 달린 대형 트럭으로 대량 수송에 용이한 트럭이에요.";

    public static readonly string Observe_Excavator =
@"굴착기

땅이나 암석 파내는 기계를 말해요.";

    public static readonly string Observe_FireTruck =
@"소방차

소방 업무를 신속하게 수행할 때 사용하는 트럭이에요.";

    public static readonly string Observe_FruitTruck =
@"과일트럭

화물차에 과일을 싣고 다니며 판매하는 트럭이에요.";

    public static readonly string Observe_MixerTruck =
@"믹서트럭

공사장에서 사용될 콘크리트를 운반하는 트럭을 말해요.";

    public static readonly string Observe_PoliceCar =
@"경찰차

범인을 잡고 우리 동네를 지켜주는 경찰관들이 타고 다니는 자동차에요.";

    public static readonly string Observe_SchoolBus =
@"스쿨버스

어린이가 유치원이나 학원 또는 학생이 학교를 통학할 때 타는 버스에요.";

    public static readonly string Observe_Ship =
@"배 

사람이나 물건 등을 물 위로 실어 나를 수 있는 자동차를 말해요.";

    public static readonly string Observe_SportsCar =
@"스포츠카

강력한 엔진으로 엄청나게 빨리 달리는 자동차에요.";

    public static readonly string Observe_Taxi =
@"택시

승객이 가고자 하는 목적지까지 기사가 데려다 주는 자동차에요.";

    public static readonly string Observe_TowTruck =
@"견인차

움직일 수 없는 자동차를 다른 곳으로 옮기는 데 쓰이는 트럭을 말해요.";

    public static readonly string Observe_Train =
@"기차

한 대 이상의 차들이 서로 일렬로 연결되어 있는 긴 차를 말해요.";

    public static readonly string Observe_Jet_Plane =
@"제트기

높은 고도(10,000m에서 12,000m 사이)로 승객과 화물을 수송하는 장거리 항공기.";


    //알아보기_동물
    public static readonly string Observe_Lion =
@"사자

밀림의 왕 사자. 암사자는 먹이를 사냥해오고 수사자는 새끼들을 지켜요.";

    public static readonly string Observe_Panda =
@"판다

귀여운 판다는 주로 대나무를 먹고 하루 10~12시간 동안이나 대나무를 먹어요.";

    public static readonly string Observe_Rhinoceros =
@"코뿔소

코끼리 다음으로 큰 대형 육상동물로 피부가 두껍고 딱딱해요. 멋진 뿔을 가지고 있어요.";

    public static readonly string Observe_Koala =
@"코알라

캥거루처럼 주머니가 있어요. 귀여운 코알라는 행동이 느리고 성격이 순해요.";

    public static readonly string Observe_Bear =
@"곰

몸집이 크고 튼튼하며 강한 뒷다리를 이용해 똑바로 설 수도 있어요. 겨울에는 겨울잠을 자요.";

    public static readonly string Observe_Crocodile =
@"악어

피부가 딱딱하고 몸이 길어요. 물속에서 생활하고 큰 입으로 사냥을 해요.";

    public static readonly string Observe_Frog =
@"개구리

개구리는 축축하고 습한 곳에 살기 때문에 논두렁, 개울가에 많이 보여요.";

    public static readonly string Observe_Giraffe =
@"기린

육상 포유류 중 키가 가장 큰 동물이에요. 수컷 기린의 목 길이는 약 2미터랍니다.";

    public static readonly string Observe_Hippo =
@"하마

머리와 몸집이 크고 다리는 짧아요. 발가락 사이에 물갈퀴가 있어 수영을 잘 해요.";

    public static readonly string Observe_Hyena =
@"하이에나

동물 찌꺼기를 모두 먹어치우는 하이에나. 알고 보면 사냥도 매우 잘 해요.";

    public static readonly string Observe_Monkey =
@"원숭이

바나나를 좋아하는 원숭이는 주로 나무에서 생활해요. 손도 매우 잘 쓴답니다.";

    public static readonly string Observe_Rabbit =
@"토끼

깡충깡충 뛰어다니는 토끼. 귀가 크고 달리기가 빨라요.";

    public static readonly string Observe_Tiger =
@"호랑이

멋진 가로 줄무늬가 특징인 호랑이는 고양잇과 동물 중에 몸집이 가장 커요.";

    public static readonly string Observe_Zebra =
@"얼룩말

까맣고 하얀 줄무늬를 가진 얼룩말. 줄무늬는 사람의 지문처럼 모두 조금씩 달라요.";

    public static readonly string Observe_Penguin =
@"펭귄

남극의 신사 펭귄은 날지 못하는 바닷새예요. 물속을 헤엄쳐서 먹이를 사냥해요.";

    public static readonly string Observe_Dog =
@"강아지

후각이 매우 예민한 육식 포유동물. 선사시대부터 집지키기, 보호하기, 탐지하기, 물건 나르기, 사냥하기 등의 많은 임무를 수행하도록 길들여지고 훈련받아 왔다.";

    public static readonly string Observe_Mouse =
@"쥐

영리한 잡식성의 설치류. 인간에게 어떤 종류의 바이러스와 세균을 옮길 수 있다. 애완용으로 길러지거나 연구실의 실험용으로 이용되는 종도 있다.";

    public static readonly string Observe_Owl =
@"부엉이

북아메리카 숲에서 발견되는 야행성 맹금. 머리의 양쪽 각각에 튀어나온 깃털 다발이 있다.";

    public static readonly string Observe_Parrot =
@"앵무새

남북 아메리카의 열대우림에서 발견되는 나뭇가지에 앉는 새. 시끄럽고 색상이 화려하며, 주로 씨앗과 열매를 먹는다.";

    public static readonly string Observe_Snake =
@"뱀

매우 긴 원통형 몸과 꼬리를 가진 다리 없는 파충류. 꿈틀거리며 움직인다. 약 2,700종이 있다.";  


    //알아보기_공룡
    public static readonly string Observe_Allo =
@"알로사우루스(Allosaurus)는 1억 5500만 ~ 1억 5000만 년 전 쥐라기 후기에 살았던 거대한 수각류 육식 공룡이다.";

    public static readonly string Observe_Ankylo = @"안킬로사우루스(Ankylosaurus)는  후기 백악기(6650만 년 전 ~ 6600만 년 전)에 살았던 곡룡류 초식 공룡이다.";

    public static readonly string Observe_Apato = @"아파토사우루스(Apatosaurus)는 후기 쥐라기(1억 5400만 년 전 ~ 1억 5000만 년 전)에 살았던 용각류 초식 공룡이다";

    public static readonly string Observe_Brachio = @"브라키오사우루스(Brachiosaurus)는 후기 쥐라기(1억 5400만 년 전 ~ 1억 5300만 년 전)에 살았던 용각류 초식 공룡이다.";

    public static readonly string Observe_Carnoto = @"카르노타우루스(Carnotaurus)는 후기 백악기(1억 년 전 ~ 6600만 년 전)에 살았으며 아르헨티나에서 발견된 육식 공룡이다.";

    public static readonly string Observe_Compsog = @"콤프소그나투스(Compsognathus)는 후기 쥐라기(1억 5100만 년 전 ~ 1억 5000만 년 전)에 독일, 프랑스에서 발견된 육식공룡이다.";

    public static readonly string Observe_Gallimimus = @"갈리미무스(Gallimimus)는 후기 백악기(7000만 년 전)에 살았으며 몽골 지역에서 발견된 수각류 잡식 공룡이다.";

    public static readonly string Observe_Pachycepha = @"파키케팔로사우루스(Pachycephalosaurus)는 후기 백악기(7000만 년 전 ~ 6600만 년 전)에 미국 서부와 캐나다에서 발견된 초식 공룡이다.";

    public static readonly string Observe_Parasau = @"파라사우롤로푸스(Parasaurolophus)는 후기 백악기(7650만 년 전 ~ 7300만 년 전)에 살았으며 미국 서부에서 발견된 초식공룡이다.";

    public static readonly string Observe_Ptera = @"프테라노돈(Pteranodon)은 후기 백악기(7000만 년 전)에 살았으며, 익룡중에서 가장 유명한 공룡이다.";

    public static readonly string Observe_Sinocera = @"시노케라톱스(Sinoceratops)는 후기 백악기(7200만 년 ~ 6600만 년 전)에 중국에서 발견된 각룡류 초식 공룡이다.";

    public static readonly string Observe_Stego =
@"스테고사우루스(Stegosaurus)는 후기 쥐라기(1억 5500만 년 전 ~ 1억 5000만 년 전)에 살았으며 미국에서 발견된 초식 공룡이다.";

    public static readonly string Observe_Tyrano = @"티라노사우루스(Tyrannosaurus)는 후기 백악기(6700만 년 전 ~ 6500만 년 전)에 살았던 수각류 육식 공룡이다.";

    public static readonly string Observe_Tricera = @"트리케라톱스(Triceratops)는 후기 백악기(6800만 년 전 ~ 6500만 년 전)에 살았던 북아메리카에서 발견된 각룡류인 초식공룡이다.";

    public static readonly string Observe_Velo = @"벨로키랍토르(Velociraptor)는 후기 백악기(7500만 년 전 ~ 7100만 년 전)에 살았으며 몽골과 중국 지역에서 발견된 육식공룡입니다.";

    
    //알아보기_바다
    public static readonly string Observe_Shark =
@"상어는 연골어류에 속하는 어류이다.교어(鮫魚), 사어(鯊魚)로도 부른다.";

    public static readonly string Observe_Blacktip =
@"흑기흉상어

등지느러미와 꼬리지느러미 끝이 뚜렷한 검은색인 것이 특징인 상어에요.";

    public static readonly string Observe_Beluga =
@"흰돌고래

북극 주변에 서식하는 고래이며 부리 모양이 독특한 고래에요.";

    public static readonly string Observe_Turtle =
@"거북이

등이 단단한 껍질로 싸여 있고 아주 느리게 움직여요. 종류에 따라서는 200~300년까지 오래 살아요.";

    public static readonly string Observe_Moray =
@"곰치

뱀과 비슷하게 생겨 산호나 암초의 갈라진 틈 또는 구멍에 숨어서 살아요. 성격이 난폭해요.";

    public static readonly string Observe_Giant_Grouper =
@"자이언트 그루퍼

입이 크고 어릴 때부터 탐식성이 강해 소형 상어까지도 잡아먹어요.";

    public static readonly string Observe_Xiphias =
@"황새치

칼처럼 길고 납작한 주둥이가 앞으로 뻗어 있는 것이 특징이다.";

    public static readonly string Observe_Puffer =
@"복어

복어는 적이 다가오면 순식간에 몸을 크게 부풀려요.";
    public static readonly string Observe_RaccoonButterFly =
@"라쿤나비고기

너구리 무늬를 가진 나비고기예요. 보통 열대, 아열대 해역에서 살아요.";
    public static readonly string Observe_JellyFish =
@"해파리

몸의 거의 대부분이 물로 이루어져 있으며 독이 있어 해수욕장에서 조심해야 해요.";
    public static readonly string Observe_NapoleonFish =
@"나폴레옹 피쉬

튀어나온 이마가 마치 모자를 쓴 나폴레옹을 닮았기 때문에 나폴레옹 피쉬라고 불려요.";
    public static readonly string Observe_MantaRay =
@"만타 가오리

유영하는 모습이 마치 넓적한 모포가 둥둥 떠다니는 것처럼 보여 만타(Manta)란 이름이 붙여졌어요.";
    public static readonly string Observe_ClownFish =
@"흰동가리

주황색 흰색 배열이 귀여운 흰동가리예요. 영화 '니모'의 주인공으로 잘 알려져 있어요.";
    public static readonly string Observe_LionFish =
@"쏠배감펭

가시가 많이 나 있으며 여러 갈래의 뾰족한 지느러미에는 독샘이 있어 조심해야 해요.";
    public static readonly string Observe_BlueTang =
@"블루탱

선명한 파란색에 노란색과 검정색이 섞여 아름다운 블루탱이예요.";
    public static readonly string Observe_PorcupineFish =
@"가시복

가시복은 위협을 느끼면 몸을 크게 부풀려 가시를 세워요. 이 가시는 움직일 수도 있어요.";
    public static readonly string Observe_GreyNurseShark =
@"모래뱀상어

무서운 외모와는 다르게 성격은 조용하며 온순해요. 하지만 계속 자극하면 화낼 수 있어요!";


    // 알아보기_과일야채
    public static readonly string Observe_Apple =
@"사과

7,500품종이 알려져 있다. 사과주를 만들거나 날것으로도 먹으며, 주스, 젤리, 콩포트, 파이로 만들거나 스트루들과 같은 디저트로도 만든다.";

    public static readonly string Observe_Grape =
@"포도

주로 포도주 제조나 식용으로 재배하는 덩굴식물.";


    // 알아보기_사물
    public static readonly string Observe_Cake =
@"케이크

밀가루, 물, 소금, 부풀어오르게 하는 첨가제(효모 또는 이스트)를 넣어 만든 음식.";

    public static readonly string Observe_Egg =
@"달걀

오늘날까지 가장 널리 식용으로 이용되고 있으며, 달걀 그대로 또는 다른 요리에 추가하여 요리한다. 흔히 덧붙이는 말 없이 '알'이라고만 하면 달걀을 의미한다.";

    public static readonly string Observe_House =
@"집

주거로서 건축물. 사람에게 편안하고 안전한 생활을 제공하도록 설비되어 있다.";

    public static readonly string Observe_Ice =
@"얼음

물의 고체상태를 가리키는 용어이다.";

    public static readonly string Observe_Ketchup =
@"케첩

향이 보통 정도인 토마토 퓌레";

    public static readonly string Observe_Night =
@"밤

해가 진 이후 다음날 동틀 때까지의 어두운 시간";

    public static readonly string Observe_Queen =
@"여왕

왕국에서 성별이 여성인 군주를 가리키는 칭호이다.";

    public static readonly string Observe_Robot =
@"로봇

사람과 유사한 모습과 기능을 가진 기계, 또는 무엇인가 스스로 작업하는 능력을 가진 기계를 말한다.";

    public static readonly string Observe_Ukulele =
@"우쿨렐레

현약기로 크기는 기타의 1/4 이며, 19세기 포르투칼 이민자들을 통해 하와이에 전해졌다.";

    public static readonly string Observe_Vase =
@"꽃병

꽃을 꽂아두는 병";

    public static readonly string Observe_Water =
@"물

산소와 수소의 화합물로서 순수한 상태에서는 냄새나 색깔, 맛이 없는 투명한 액체다. 인체의 70% 가 물로 구성되어 있고, 생물체에게는 없어서는 안될 생존 필수품이다.";

    public static readonly string Observe_Yoghurt =
@"요거트

우유를 유산균을 이용하여 발효시킨 것이다.";


    // 알아보기_곤충
    public static readonly string Observe_Bee =
@"벌

매우 복잡한 사회 체제를 이루고 사는 곤충. 양식을 저장하기 위해 본능적으로 꿀을 생산한다.";


    // 튜토리얼
    public static readonly string QRLicence_m = 
@"이 앱을 사용하기 위해서는 제품 등록이 필요합니다.

제품을 구매하신 경우에는 해당 제품에 동봉되어 있는 QR코드를 사용하시면 됩니다.
만약 제품이 없으시면 아래의 Skip 버튼을 눌러 제품 등록 없이 사용하실 수 있습니다.

단, 제품 등록을 하지 않고 사용 할 수 있는 횟수가 제한 되어 있습니다. 
사용 제한 이후에는 제품 등록을 하셔야 사용하실 수 있습니다.

[등록 방법]

앱을 실행한 후 제품 QR코드를 가이드라인에 맞춰 인식시킵니다.
인식이 잘 되지 않을 경우 화면을 터치하면 자동으로 초점이 맞춰집니다.
정상적으로 인식되었다는 메세지가 표시된 후 메인 화면으로 넘어갑니다.

제품 등록은 최초 사용시 1회만 하시면 됩니다.

자세한 정보는 홈페이지의 MANUAL을 참고하여 주십시오.
홈페이지 주소 : http://www.hansapp.co.kr";

    public static readonly string QRFunction_m = 
@"이 기능을 사용하기 위해서는 제품 등록이 필요합니다.

제품을 구매하신 경우에는 해당 제품에 동봉되어 있는 QR코드를 사용하시면 됩니다.
만약 제품이 없으시면 아래의 Skip 버튼을 눌러 제품 등록 없이 사용하실 수 있습니다.

단, 제품 등록을 하지 않고 사용 할 수 있는 횟수가 제한 되어 있습니다. 
사용 제한 이후에는 제품 등록을 하셔야 사용하실 수 있습니다.

[등록 방법]

앱을 실행한 후 제품 QR코드를 가이드라인에 맞춰 인식시킵니다.
인식이 잘 되지 않을 경우 화면을 터치하면 자동으로 초점이 맞춰집니다.
정상적으로 인식되었다는 메세지가 표시된 후 다음 화면으로 넘어갑니다.

제품 등록은 최초 사용시 1회만 하시면 됩니다.

자세한 정보는 홈페이지의 MANUAL을 참고하여 주십시오.
홈페이지 주소 : http://www.hansapp.co.kr";

    // 튜토리얼UI(일반씬)
    public static readonly string CommonTutorial_m = 
@"이 앱은 카메라로 이미지 타겟을 비추면 콘텐츠가 실행되는 증강현실 앱입니다.

제품을 구매하신 경우에는 해당 제품을 타겟으로 사용하실 수 있습니다.
만약 제품이 없으시면 아래의 Link 버튼을 눌러 무료 샘플 타겟을 출력하여 사용하시면 됩니다.
또는, 홈페이지 MANUAL의 타겟 이미지를 비추어도 증강현실 콘텐츠가 실행됩니다.

[사용 방법]

1. 이미지 타겟을 카메라로 비춥니다.
2. 타겟에 연결된 콘텐츠가 실행됩니다.
3. 탐색 버튼을 누르면 다른 콘텐츠를 볼 수 있습니다.
4. 알아보기 버튼을 통해 해당 캐릭터에 대한 설명을 들을 수 있습니다.
5. 방향 버튼으로 캐릭터를 돌려 볼 수 있습니다.
6. 하단 메뉴는 사진촬영, 카메라전환, 홈페이지, 카페 방문이 가능합니다.
7. 드래그를 통해 캐릭터를 움직일 수 있습니다.

자세한 정보는 홈페이지의 MANUAL을 참고하여 주십시오.
홈페이지 주소 : http://hansapp.co.kr";

    // 튜토리얼UI(스케치씬)
    public static readonly string SketchTutorial_m = 
@"이 앱은 카메라로 이미지 타겟을 비추면 콘텐츠가 실행되는 증강현실 앱입니다.

제품을 구매하신 경우에는 해당 제품을 타겟으로 사용하실 수 있습니다.
만약 제품이 없으시면 아래의 Link 버튼을 눌러 무료 샘플 타겟을 출력하여 사용하시면 됩니다.
또는, 홈페이지 MANUAL의 타겟 이미지를 비추어도 증강현실 콘텐츠가 실행됩니다.

[사용 방법]

1. 가지고 계신 색칠북 그림에 색칠을 합니다.
2. 색칠한 그림을 카메라로 비춥니다.
3. 그림이 올바르게 인식되면 붉은색 사각형이 파란색으로 바뀝니다.
4. 3D 콘텐츠에 색이 입혀집니다.
5. 타겟을 새롭게 인식하면 다시 색이 입혀집니다.

자세한 정보는 홈페이지의 MANUAL을 참고하여 주십시오.
홈페이지 주소 : http://hansapp.co.kr";

    // 튜토리얼UI(미니맵 실사 씬)
    public static readonly string MiniMapRealTutorial_m =
@"이 앱은 카메라로 이미지 타겟을 비추면 콘텐츠가 실행되는 증강현실 앱입니다.

제품을 구매하신 경우에는 해당 제품을 타겟으로 사용하실 수 있습니다.
만약 제품이 없으시면 아래의 Link 버튼을 눌러 무료 샘플 타겟을 출력하여 사용하시면 됩니다.
또는, 홈페이지 MANUAL의 타겟 이미지를 비추어도 증강현실 콘텐츠가 실행됩니다.

[사용 방법]

1. 이미지 타겟을 카메라로 비춥니다.
2. 타겟에 연결된 콘텐츠가 실행됩니다.
3. 타겟을 비인식 시키면 미니맵 버튼이 나타납니다.
4. 하면 하단의 미니맵 버튼을 눌러 해당 콘텐츠를 미니맵에 모을 수 있습니다.
5. 화면 우상단의 맵 버튼을 눌러서 미니맵을 확대/축소 시키실 수 있습니다.

자세한 정보는 홈페이지의 MANUAL을 참고하여 주십시오.
홈페이지 주소 : http://hansarapp.com";

    // 튜토리얼UI(미니맵 스케치 씬)
    public static readonly string MiniMapSketchTutorial_m =
@"이 앱은 카메라로 이미지 타겟을 비추면 콘텐츠가 실행되는 증강현실 앱입니다.

제품을 구매하신 경우에는 해당 제품을 타겟으로 사용하실 수 있습니다.
만약 제품이 없으시면 아래의 Link 버튼을 눌러 무료 샘플 타겟을 출력하여 사용하시면 됩니다.
또는, 홈페이지 MANUAL의 타겟 이미지를 비추어도 증강현실 콘텐츠가 실행됩니다.

[사용 방법]

1. 가지고 계신 색칠북 그림에 색칠을 합니다.
2. 색칠한 그림을 카메라로 비춥니다.
3. 그림이 올바르게 인식되면 붉은색 사각형이 파란색으로 바뀝니다.
4. 3D 콘텐츠에 색이 입혀집니다.
5. 타겟을 비인식 시키면 미니맵 버튼이 나타납니다.
6. 하면 하단의 미니맵 버튼을 눌러 해당 콘텐츠를 미니맵에 모을 수 있습니다.
7. 화면 우상단의 맵 버튼을 눌러서 미니맵을 확대/축소 시키실 수 있습니다.

자세한 정보는 홈페이지의 MANUAL을 참고하여 주십시오.
홈페이지 주소 : http://hansarapp.com";
}
