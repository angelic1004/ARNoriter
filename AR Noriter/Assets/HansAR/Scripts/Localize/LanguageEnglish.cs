using UnityEngine;
using System;

public class LanguageEnglish
{
    #region Comment

    // TODO : Add localized value here.
    // You can define static readonlyant, static variable or static property.
    //
    // public static readonly string ConfirmDelete = "Are you sure to delete?";
    // public static readonly string AnswerYes = "Yes";
    // public static string AnswerNo { get { return "No"; } }
    //
    // How to use within script :
    // 1. Read value by localize ID
    // - Add defined value name to LocalizeID.
    // - Debug.Log(LocalizeText.Value[LocalizeID.ConfirmDelete]);
    // 2. Read value by name
    // - Debug.Log(LocalizeText.Value["ConfirmDelete"]);
    //
    // How to localize UI text :
    // 1. Add LocalizeText component to UILabel of NGUI (or UnityEngine.UI.Text).
    // 2. Enter defined value name to "Value Name" property of LocalizeText component.
    // 3. If you add LocalizeManager to the scene, all UI text of scene will be changed at runtime.

    #endregion

    // 주의사항 : 영어 이외의 언어는 static readonly로 선언할 경우 iOS 디바이스에서 문자열이 잘리는 문제가 있다.
    // 따라서, 영어가 아닌경우 static 또는 static readonly로 선언하거나 static readonly를 간접 참조하도록 해야한다.

    public static readonly string LocalizeSelect = "Language selection";
    public static readonly string SaveLocalizeSetting = "Remember selected language.";

    // Pop-up
    public static readonly string Exit_Q = "Are you sure you want to exit the app?";
    public static readonly string Exit_A = "Exit";
    public static readonly string Cancel = "Cancel";
    public static readonly string Return_Q = "Would you like to return to the main screen?";
    public static readonly string Return_A = "Return";
    public static readonly string Start = "Start";
    public static readonly string Exit_Restart = "Do you want to exit or restart?";

    public static readonly string Result_Prev = "Pre stage";
    public static readonly string Result_Restart = "Restart";
    public static readonly string Result_End = "End";
    public static readonly string Result_Next = "Next stage";

    public static readonly string Pause = "Pause";

    // Contents
    public static readonly string ReadContents = "Reading contents.";
    public static readonly string LoadContents = "Loading contents ...";
    public static readonly string IlluminateImage = "\"Please illuminate the image.\"";

    public static readonly string SketchRecognition_Default = "\"Please illuminate the painted picture.\"";
    public static readonly string SketchRecognition_OnTarget = "\"Illuminate the picture \nso that the entire rectangle is visible.\"";
    public static readonly string NotUsedTargetForThisScene = "\"Coloring images, please do coloring.\"";
    public static readonly string NotUsed4DTargetForThisScene = "\"4D images, please do 4D experience\"";

    // Buttons
    public static readonly string Continue = "Continuity";
    public static readonly string Question = "Question";
    public static readonly string Answer = "Answer";

    public static readonly string PreferredLanguage = "Please select your preferred language.";

    public static readonly string ENG = "English";
    public static readonly string KOR = "Korean";
    public static readonly string CHN = "Chinese";
    public static readonly string JPN = "Japanese";
    public static readonly string VTN = "Vietnamese";
    public static readonly string Indonesian = "Indonesian";

    public static readonly string Listen = "Listening";
    public static readonly string ListenAndLearn = "Listen & Learn";

    // 데이터사용
    public static readonly string Wifi_NotConnected = "WI-FI is not connected.";
    public static readonly string MobileDatacall = "Mobile networks (3G/LTE, etc.) can occur data call during connection.";
    public static readonly string MobileDatacall_F = @"Mobile networks (3G/LTE, etc.) can occur data call during connection. \n             (Data: {0}MB)";
    public static readonly string Download = "Download";
    public static readonly string Back = "Go back";
    public static readonly string Wifi_NotGood = "WI-FI or 3G/LTE network connection is not good.";

    public static readonly string ShowOff_Q = "Don't show this window again.";

    // ProgressBar
    public static readonly string ProgressBarComment = "Please wait a moment.";

    // QR코드
    public static readonly string ScanQR = "\"Scan the QR code.\"";
    public static readonly string CheckingQR = "Checking the QR code ...";
    public static readonly string OK = "OK";
    public static readonly string Net_Unable = "Unable to connect to the Internet.";
    public static readonly string QR_Unregistered = "Unregistered serial number!\n\nScanned QR code has never been issued or has already been deleted.";
    public static readonly string QR_Expired = "Expired serial number!\n\nScanned QR code has exceeded specified usage count.";
    public static readonly string QR_Invalid = "Invalid serial number!\n\nScanned QR code can not be used for this product.";
    public static readonly string QR_Unsuitable = "Unsuitable serial number!\n\nScanned QR code is not compatible with desired feature.";
    public static readonly string DB_Error = "Database error!";
    public static readonly string Server_Error = "Server error!";
    public static readonly string QR_Success = "Product registration is completed.";
    public static readonly string UsedExpChance = "You have used all the experience chance.";

    public static readonly string Pass_Recognized = "This target has been verified.";
    public static readonly string Pass_Unrecognized = "This target has not been verified. Please check your target.";

    // AssetDownloadManager.cs
    public static readonly string Net_Unstable = "Current network status is unstable.";
    public static readonly string Net_ReturnMain = "Do you want to stop downloading and return to the main screen?";
    public static readonly string DownloadTotal_F = "Contents downloading... (Total {0}MB)";
    public static readonly string DownloadProgress_F = "Contents downloading... ({0}MB / {1}MB)";
    public static readonly string DownloadProgress2_F = "Contents downloading...\\n({0}MB / {1}MB)";
    public static readonly string Net_Retry = "Connection to the server is not smooth. Please try again after a while.";
    public static readonly string Net_UpdateFinish = "Update of the content has completed.";
    public static readonly string Net_CheckVer = "Checking the version of the content.";

    // RecordScreenUI.cs
    public static readonly string Rec_Screen = "Recording the screen.";
    public static readonly string Rec_Finish = "Screen recording has completed.";
    public static readonly string Rec_GrantRights = "Failed to grant rights for recording!";
    public static readonly string Rec_NoMic = "There is no microphone device!";
    public static readonly string Rec_NotSupport = "Your device does not support screen recording!";
    public static readonly string Rec_SaveAudio = "Saving the audio file.";
    public static readonly string Rec_SaveAudioFailed = "Failed to save the audio file!";
    public static readonly string Rec_SaveVideoFailed = "Failed to save the video file!";
    public static readonly string Rec_SaveVideo = "Saving the video file.";
    public static readonly string Rec_NoFolder = "There is no saved video folder!";
    public static readonly string Rec_NoVideo = "There is no saved video file!";

    // MainUI.cs    public static readonly string TargetCount_F = "Recognizable target number : ";

    // ExploreManager.cs
    public static readonly string AutoSearchOn = "Automatic search mode has been set.";
    public static readonly string AutoSearchOff = "Automatic search mode is turned off.";

    // 카테고리
    public static readonly string Vehicle = "Vehicles";
    public static readonly string Animal = "Animals";
    public static readonly string Dino = "Dinos";
    public static readonly string Insect = "Bugs";
    public static readonly string Princess = "Princess";
    public static readonly string RacingCar = "RacingCar";
    public static readonly string Soccer = "Soccer";
    public static readonly string City = "City";
    public static readonly string Alphabet = "Alphabet";
    public static readonly string Sea = "Sea";

    public static readonly string FourD = "4D Experience";
    public static readonly string Coloring = "Coloring";
    public static readonly string Repeat = "Repeat";
    public static readonly string Puzzle = "Puzzle";
    public static readonly string PuzzleGame = "Puzzle";
    public static readonly string QuizQuiz = "QuizQuiz";
    public static readonly string Sticker = "Sticker";
    public static readonly string Matching = "Matching";
    public static readonly string Word = "Word";
    public static readonly string Maze = "Maze";
    public static readonly string Kart = "Kart";
    public static readonly string Paint = "Paint";
    public static readonly string PostCard = "PostCard";
    public static readonly string Fishing = "Fishing";


    public static readonly string Observe = "Observation";
    public static readonly string Study = "Study";
    public static readonly string Language = "Study";
    public static readonly string QnA = "QnA";
    public static readonly string Conversation = "Conversation";
    public static readonly string MiniMap00 = "Mini Map";

    public static readonly string Sketch = "Sketch";
    public static readonly string MiniMap01 = "Mini Map";
    public static readonly string LookNFind_Vehicle = "Look & Find";
    public static readonly string LookNFind_Animal = "Look & Find";
    public static readonly string LookNFind_Dino = "Look & Find";
    public static readonly string LookNFind_Princess = "Look & Find";

    public static readonly string Sticker_Dino = "Sticker";
    public static readonly string PrincessCrane = "Doll Crane";

    public static readonly string SoccerGame = "Soccer Game";
    public static readonly string FourDRacingGame = "4D Racing Game";
    public static readonly string RacingDrive = "Racing Drive";
    public static readonly string Video = "Video";

    public static readonly string Black = "Black";
    public static readonly string Pink = "Pink";
    public static readonly string SkyBox = "SkyBox";

    public static readonly string Observe_Animal = "Observation_Animal";
    public static readonly string Observe_Object1 = "Observation_Object1";
    public static readonly string Observe_Object2 = "Observation_Object2";
    public static readonly string Observe_Vegetable = "Observation_Vegetable";

    //NavigationUI.cs
    public static readonly string MinimapReal = "4D Experience Minimap";
    public static readonly string MinimapSketch = "Sketch Minimap";
    public static readonly string Color = "Coloring";
    public static readonly string Other = "Other";
    public static readonly string RacingGame = "4D Racing Game";

    public static readonly string ArPlay = "AR PLAY";
    public static readonly string DanceDance = "Dance Dance";
    public static readonly string DanceBattle = "Dance Battle";
    public static readonly string Runway = "Runway";
    public static readonly string ColorRunway = "Color Runway";

    public static readonly string Manual = "Manual";
    public static readonly string Category = "Category";
    public static readonly string LanguageSelect = "Language";
    public static readonly string Sound = "Sound";

    public static readonly string Dance = "Dance";
    public static readonly string Battle = "Battle";

    public static readonly string Game = "Game";

    //단계표시
    public static readonly string Stage_One = "Stage One";
    public static readonly string Stage_Two = "Stage Two";
    public static readonly string Stage_Three = "Stage Three";
    public static readonly string Stage_Four = "Stage Four";
    public static readonly string Stage_Five = "Stage Five";

    //PrincessManager.cs 
    public static readonly string ClowQuestion = "Find the right princess.";
    public static readonly string ClowGameOver = "You've solved all the question. congratulations~!";
    public static readonly string ClowSelectAnswer = "That's correct! good job!!";
    public static readonly string ClowSelectWrong = "It is regrettable~ Let's look again~!";
    public static readonly string ClowRestart = "Restart the claw machine game.~!";
    public static readonly string Restart = "Restart";


    //퍼즐 
    public static readonly string MatchingNumber = "Point";
    public static readonly string ChallengeNumber = "No. of try";

    // Dance Warning Message
    public static readonly string DanceCharacterSelectMsg = "Choose position before character selection.";

    // 공주 Dance
    public static readonly string ChooseDancerNumber = "Choose a number of dancer playing";
    public static readonly string ChooseDancer = "Choose a dancer";
    public static readonly string DanceDancePopupMsg = "Recognized Target. Would you like to end Dance Dance?";
    public static readonly string DanceBattlePopupMsg = "Recognized Target. Would you like to end Dance Battle?";

    // 공주 스티커
    public static readonly string PrincessShadowSticker = "Princess Shadow Sticker";
    public static readonly string PrincessBallroom = "Beautiful princess are gathered in the Kingdom's ballroom. Let's see who came and put sticker on the ballroom.";
    public static readonly string StickPuzzleSticker = "Stick the puzzle sticker";
    public static readonly string SuitablePuzzle = "Let's place suitable puzzle stickers on to the emty space and complete painting.";
    public static readonly string IamOdette = "My name is Odette! I do ballet with graceful motions.";
    public static readonly string IamMermaid = "I am Princess Mermaid, I can freely roam around with my fins!";

    public static readonly string SnowWhite = "Snow White";
    public static readonly string PrincessNavia = "Princess Navia";
    public static readonly string PrincessBari = "Princess Bari";
    public static readonly string PrincessBell = "Princess Bell";
    public static readonly string Cinderella = "Cinderella";
    public static readonly string Cleopatra = "Cleopatra";
    public static readonly string Odette = "Odette";
    public static readonly string Rapunzel = "Rapunzel";
    public static readonly string Thumbelina = "Thumbelina";
    public static readonly string PrincessMermaid = "Princess Mermaid";

    // 4D 레이싱 게임
    public static readonly string RacingWinner = "Congratulations! You win the first place.";
    public static readonly string RacingStart = "When image goes beyond camera place, the racing game will start.";
    public static readonly string LapTime = "Lap Time";
    public static readonly string Speed = "Speed";

    // 축구 게임
    public static readonly string StartSoccerGame = "Illuminate the image, you can start soccer game!";
    public static readonly string StartSoccerVideo = "Scan the image, you can watch soccer video clips!";
    public static readonly string SelectGameMode = "Please select game mode!";
    public static readonly string PenaltyKick = "Penalty Kick";
    public static readonly string FreeKick = "Free Kick";
    public static readonly string FreeKick_Explain = "Thrilling free-kick, to shoot a goal over the wall";
    public static readonly string KickerNumber = "Please choose a number of kicker entering game.";

    public static readonly string InputYourTeam = "Input your team name";
    public static readonly string InputTeam = "Input team name";
    public static readonly string InputTeamA = "Input A team name";
    public static readonly string InputTeamB = "Input B team name";
    public static readonly string NoTeamname = "No team name written. Please type in again.";
    public static readonly string SameTeam = "The name is duplicated. Please write down again";
    public static readonly string SameTeamA = "Written team name is same as A team. Please write again.";
    public static readonly string SameTeamB = "Written team name is same as B team. Please write again.";
    public static readonly string SelectUniform = "Please select uniform";
    public static readonly string TeamAUniform = "Please choose A team uniform";
    public static readonly string TeamBUniform = "Please choose B team uniform";
    public static readonly string SameUniform = "You cannot choose same uniform as the opposing team.";
    public static readonly string SelectNumberPlayer = "Select Number of Player";
    public static readonly string Select = "Select";
    public static readonly string ChooseCharacter = "Choose Character";
    public static readonly string SelectPlayer = "Please select player";
    public static readonly string TeamAPlayer = "Choose A team player";
    public static readonly string TeamBPlayer = "Choose B team player";

    public static readonly string Scout = "Scout";
    public static readonly string ScoutCancel = "Canceling Scout";

    public static readonly string SoccerRestart = "Do you want to restart?";

    public static readonly string Shoot = "SHOOT";
    public static readonly string NormalShoot = "Normal";
    public static readonly string GroundShoot = "Ground";
    public static readonly string ToeShoot = "Toe";
    public static readonly string InsideShoot = "Inside";

    // Watch Car
    public static readonly string WatchCarPlay = "WatchCar Play";
    public static readonly string BattleGame    = "Battle Game";
    public static readonly string RoadDriving   = "Road Driving";

    public static readonly string WarchCarBattlePopupMsg = "Recognized Target. Would you like to end WarchCar Battle?";

    // Watch Car Edu
    public static readonly string WrongWay = "Wrong way";
    public static readonly string DriveAgain = "Drive again";

    // Nas Car
    public static readonly string NasCar = "NasCar";

    // Track
    public static readonly string RacingDrive_RaceTrack01 = "Racing Drive - Track 1";
    public static readonly string RacingDrive_RaceTrack02 = "Racing Drive - Track 2";

    //카메라, 회전 튜토리얼
    public static readonly string tuto_camera = "Using camera menu, you can take photo and record video.";
    public static readonly string tuto_rot = "Once scanning the target image, you can rotate and look around the 3D contents.";

    //알아보기 튜토리얼
    public static readonly string tuto_lang = "When you click the language selection button, you can study different languages. ";
    public static readonly string tuto_ob = "Click the magnifying glass to see explanation.";

    //매칭 튜토리얼
    public static readonly string tuto_suc = "You can check the number of matched cards.";
    public static readonly string tuto_try = "You can check how many time you flipped the cards.";
    public static readonly string tuto_cardPos = "Let's flip cards and match the same image.";
    public static readonly string tuto_restart = "You can choose 'END' or 'RE-START'";

    //퍼즐 튜토리얼
    public static readonly string tuto_bg = "Puzzle board to fill the puzzle pieces.";
    public static readonly string tuto_dragPuzzle = "Puzzle pieces you can drag and drop.";
    public static readonly string tuto_stage = "You can check current status.";

    //색칠하기 튜토리얼
    public static readonly string tuto_colorReset = "You can re-set if you don't like colored image.";

    //페인트 튜토리얼
    public static readonly string tuto_paintSave = "You can save image you painted.";
    public static readonly string tuto_paintSelectColor = "You can pick the color and paint.";
    public static readonly string tuto_paintMiniMap = "You can save painted image and see them on MINI MAP.";

    //미니맵 튜토리얼
    public static readonly string tuto_collect = "You can put  the contents and see in mini map.";
    public static readonly string tuto_minimapSize = "You can zoom in and out the mini map.";

    //단어 맞추기 튜토리얼
    public static readonly string tuto_wordList = "To complete a full word, click the below cube.";

    //미로게임 튜토리얼
    public static readonly string tuto_mazeMinimap = "You can open the mini map to see current location.";
    public static readonly string tuto_pause = "If you press pause button, you can restart or end the game.";

    //카트게임 튜토리얼
    public static readonly string tuto_kartStage = "Check the current level.";
    public static readonly string tuto_kartScore = "Check the current score.";
    public static readonly string tuto_kartFruit = "It shows how many fruits you ate, it goes to the next level if you eat all fruits.";
    public static readonly string tuto_kartGauge = "It shows remained fuel. If run out of fuel, misson failed.";

    //포스트카드 튜토리얼
    public static readonly string tuto_postcardTakePic = "Button for taking photo";
    public static readonly string tuto_postcardAlbumPic = "Brings picture from gallery";
    public static readonly string tuto_postcardAlbumAvi = "Brings video from gallery";

    //편지 텍스트박스 StartValue
    public static readonly string letter_textbox = "Touch here to enter text";

    //MazeSelectUI
    public static readonly string selectMaze = "Choosing a Labyrinth";
    public static readonly string mazeCave = "Dungeon Labyrinth";
    public static readonly string mazeGlassLand = "GrassLand Labyrinth";
    public static readonly string mazeGlacier = "Glacier Labyrinth";

    public static readonly string mazeSurface = "Surface Labyrinth";
    public static readonly string mazeCrystal = "Crystal Labyrinth";
    public static readonly string mazeSubmarine = "Submarine Labyrinth";

    public static readonly string mazeForest = "Forest Labyrinth";
    public static readonly string mazeDesert = "Desert Labyrinth";
    public static readonly string mazeDungeon = "Dungeon Labyrinth";

    //ResultUI
    public static readonly string retry = "Retry";
    public static readonly string quitGame = "Quit Game";
    public static readonly string nextMaze = "Next Maze";
    public static readonly string nextQuestion = "Next Question";

    //MazeGameUI
    public static readonly string introText = "You've reached the maze\nPress the Start button to start the game!";
    public static readonly string startText = "Start the game!";
    public static readonly string moveText = "I'm on the move";
    public static readonly string selectArrowText = "A fork is on the way! where should we go??";
    public static readonly string deadEndText = "You came to a dead end. Let's go back~";
    public static readonly string goalText = "I escaped the maze! Congratulations!!";
    public static readonly string retryText = "Restart the game~";


    //fishingGame
    public static readonly string BlowFish = "BlowFish";
    public static readonly string LionFish = "LionFish";
    public static readonly string Raccoon_ButterFly = "Raccoon ButterFly";
    public static readonly string ClowFish = "ClowFish";
    public static readonly string BlueTang = "BlueTang";
    public static readonly string Beluga = "Beluga";
    public static readonly string NapoleonFish = "NapoleonFish";
    public static readonly string Grey_Nursh_Shark = "GreyNursh Shark";
    public static readonly string Porcupine_Fish = "PorcupineFish";
    public static readonly string Giant_Grouper = "Giant Grouper";
    public static readonly string Manta_Ray = "Manta Ray";
    public static readonly string BlackTip_Shark = "BlackTip Shark";

    public static readonly string Can = "Can";
    public static readonly string Clam = "Clam";
    public static readonly string Shoes = "Shoes";
    public static readonly string StarFish = "StarFish";

    public static readonly string normalMap = "Lake";
    public static readonly string iceMap = "Ice";
    public static readonly string typhoonMap = "Typhoon";

    public static readonly string fishing_tuto_00 = "버튼을 눌러 낚시를 시작해요.";
    public static readonly string fishing_tuto_01 = "화면을 연타해 일정 게이지 이상 넘겨야 물고기를 잡아요!";
    public static readonly string fishing_tuto_02 = "물고기를 잡으면 왼쪽에 물고기가 채워져요.";
    public static readonly string fishing_tuto_03 = "잡은 물고기는 어항을 눌러서 확인해볼 수 있어요!";


    //PauseUI
    public static readonly string askQuit = "Are you sure you want to quit??";
    
    //WordUI
    public static readonly string loading = "Loading...";

    //RestartPopUp
    public static readonly string RestartGame = "Do you want to restart the game?";

    public static readonly string Saved = "SAVED.";

    // 알아보기_도시
    public static readonly string Observe_AirPlane =
@"Airplane

Airplane is a craft with a special ability of flying in the air.";

    public static readonly string Observe_Ambulance =
@"Ambulance 

Ambulance is a car transporting patient to hospital promptly at emergency.";

    public static readonly string Observe_Helicopter =
@"Helicopter 

Helicopter is a kind of rotorcraft flying by wing rotation.";

    public static readonly string Observe_DumpTruck =
@"Dump Truck

Dump Truck has a cargo box and it makes very easy for mass transportation.";

    public static readonly string Observe_Excavator =
@"Excavator 

Excavator is a equipment digging such as land and rock.";

    public static readonly string Observe_FireTruck =
@"Fire Truck

Fire Truck is a truck being used to put out fire fast and effectively.";

    public static readonly string Observe_FruitTruck =
@"Fruit Truck

Fruit Truck is a car load and sells fruit.";

    public static readonly string Observe_MixerTruck =
@"Mixer Truck

Mixer Truck carries concrete which will be used in construction site.";

    public static readonly string Observe_PoliceCar =
@"Police Car 

Police Car is what police officer ride to catch criminals and guard our town.";

    public static readonly string Observe_SchoolBus =
@"School Bus

School Bus is a vehicle used for kids or student commute to Kindergarten or school.";

    public static readonly string Observe_Ship =
@"Ship

Ship is a vehicle that transport human or stuffs across the water.";

    public static readonly string Observe_SportsCar =
@"Sports Car

Sports Car races incredibly fast with a powerful engine.";

    public static readonly string Observe_Taxi =
@"Taxi

Taxi is a car which a driver ride passenger one's destination.";

    public static readonly string Observe_TowTruck =
@"Tow Truck

Tow Truck is a truck that transport immovable car to other place.";

    public static readonly string Observe_Train =
@"Train

Train is a long motor, which one or more car is linked with in a row.";

    public static readonly string Observe_Jet_Plane =
@"Jet Plane

Aircraft that transports passengers and cargo traveling long distances at high altitudes (between 10,000 and12,000m).";


    //알아보기_동물
    public static readonly string Observe_Lion =
@"Lion

King of the jungle. The lioness hunts the prey and the lion keeps the young.";

    public static readonly string Observe_Panda =
@"Panda

Panda's essential food is bamboo. It eats up bamboo for 10 - 12 hours on a day.";

    public static readonly string Observe_Rhinoceros =
@"Rhinoceros

Rhinoceros is the second biggest land animal after elephant, which has thick and hard skin.";

    public static readonly string Observe_Koala =
@"Koala

Koala has a pocket like Kangaroo. Such a cutie Koala act slow and meek.";

    public static readonly string Observe_Bear =
@"Bear

Bear has big and strong body and it can sit up straight by using its powerful hind legs.";

    public static readonly string Observe_Crocodile =
@"Crocodile

Crocodile has hard skin and long body. It lives in water and hunt with big mouth.";

    public static readonly string Observe_Frog =
@"Frog

The frogs live in wet and humid places, so they look a lot in the rice paddies and streams.";

    public static readonly string Observe_Giraffe =
@"Giraffe

A giraffe is the tallest animal in the land mammal. The length of a male giraffe is about 2 meters.";

    public static readonly string Observe_Hippo =
@"Hippo

The hippo's head and size are large and the legs are short. There is a webbed between the toes so I can swim.";

    public static readonly string Observe_Hyena =
@"Hyena

Hyenas eat up all the animal waste. If you know, I hunt very well.";

    public static readonly string Observe_Monkey =
@"Monkey

Monkeys like bananas live mainly in trees. My hands are very well written.";

    public static readonly string Observe_Rabbit =
@"Rabbit

A bunny rabbit running around. Ears are big and running fast.";

    public static readonly string Observe_Tiger =
@"Tiger

The tiger is characterized by its stunning horizontal stripes and is the largest of its kind in the feline.";

    public static readonly string Observe_Zebra =
@"Zebra

A zebra with black and white stripes. Stripes are a little different, like fingerprints of a person.";

    public static readonly string Observe_Penguin =
@"Penguin

Antarctic gentleman penguins are seabirds that can not fly. I swim in the water and hunt the food.";

    public static readonly string Observe_Dog =
@"Dog

Carnivorous mammal with an excellent sense of smell; it has been domesticated since prehistoric times and trained to perform a number of tasks: guarding and protecting, detecting, carrying and hunting.";

    public static readonly string Observe_Mouse =
@"Mouse

Omnivorous rodent characterized byits intelligence; it can transmitcertain viruses and bacteria tohumans. Some species are kept aspets and used for laboratoryexperiments.";

    public static readonly string Observe_Owl =
@"Owl

Nocturnal bird of prey found in theforests of North America, with a protruding tuft of feathers on eachside of its head.";

    public static readonly string Observe_Parrot =
@"Parrot

Noisy brightly colored perching birdfound in the tropical forests of theAmericas; it feeds mainly on seedsand fruit.";

    public static readonly string Observe_Snake =
@"Snake

Legless reptile with a very long cylindrical body and tail, moving by undulation; there are about 2,700 species.";   


    //알아보기_공룡
    public static readonly string Observe_Allo =
@"Allosaurus is a large herbivorous carnivorous dinosaur that lived in the late Jurassic period between 155 million and 150 million years ago.";

    public static readonly string Observe_Ankylo = @"Ankylosaurus is a gyuryeonggang dinosaur that lived in the late Cretaceous (66.5 million to 66 million years ago)";

    public static readonly string Observe_Apato = @"Apatosaurus is a sauerkraut dinosaur that lived in late Jurassic (154 million to 150 million years ago)";

    public static readonly string Observe_Brachio = @"Brachiosaurus is a sauerkraut dinosaur that lived in late Jurassic (154 million to 15 53 million years ago)";

    public static readonly string Observe_Carnoto = @"Carnotaurus was a carnivorous dinosaur found in Argentina and lived in the late Cretaceous (100 million to 66 million years ago)";

    public static readonly string Observe_Compsog = @"Compsognathus is a carnivorous dinosaur discovered in Germany and France in late Jurassic (151-150 million years ago)";

    public static readonly string Observe_Gallimimus = @"Gallimimus is an omnivorous dinosaur that was found in the Mongolian area and lived in the late Cretaceous (70 million years ago).";

    public static readonly string Observe_Pachycepha = @"Pachycephalosaurus is a herbivorous dinosaur found in the western United States and Canada in the late Cretaceous (70 million to 66 million years ago).";

    public static readonly string Observe_Parasau = @"Parasaurolophus lived in the late Cretaceous (76.5 million to 73 million years ago) and is a herbivorous dinosaur found in the western United States.";

    public static readonly string Observe_Ptera = @"Pteranodon lived in the late Cretaceous (70 million years ago) and is the most famous dinosaur of the pterosaurs.";

    public static readonly string Observe_Sinocera = @"Sinoceratops are dinosaur dinosaurs found in China in the late Cretaceous (72 million to 66 million years ago)";

    public static readonly string Observe_Stego =
@"Stegosaurus lived in late Jurassic (155 million to 150 million years ago) and is a herbivorous dinosaur found in the United States.";

    public static readonly string Observe_Tyrano = @"Tyrannosaurus is a herbivorous carnivorous dinosaur that lived in the late Cretaceous (67 million to 65 million years ago)";

    public static readonly string Observe_Tricera = @"Triceratops is a heron-eating herbivorous dinosaur found in North America that lived in the late Cretaceous (68 million to 65 million years ago)";

    public static readonly string Observe_Velo = @"Velociraptor lived in the late Cretaceous (75 million to 71 million years ago) and is a carnivorous dinosaur discovered in Mongolia and China.";


    //알아보기_바다
    public static readonly string Observe_Shark =
@"Shark

상어는 연골어류에 속하는 어류이다.교어(鮫魚), 사어(鯊魚)로도 부른다.";

    public static readonly string Observe_Blacktip =
@"Blacktip

Blacktip Reef Shark has special feature of having vivid black ends of dorsal and caudal fin.";

    public static readonly string Observe_Beluga =
@"Beluga

Beluga is a whale kind which lives around the North Pole has a remarkable beak shape.";

    public static readonly string Observe_Turtle =
@"Turtle

Turtle moves very slow and its' back is surrounded by hard shall. It generally lives long and some kinds even for 200 - 300 years.";

    public static readonly string Observe_Moray =
@"Moray

Moray looks similar to snake and lives in the gap between reef and hide in a cavity. It is very aggressive fish kind.";

    public static readonly string Observe_Giant_Grouper =
@"Giant Groupe

Giant Grouper has a big mouth. Since young, it is strongly gluttonous even it can eat shark up.";

    public static readonly string Observe_Xiphias =
@"Xiphias

The swordfish is named after its pointed, flat bill, which resembles a sword.";

    public static readonly string Observe_Puffer =
@"Puffer

Puffer is inflating the body in an instant when the enemy approaches.";
    public static readonly string Observe_RaccoonButterFly =
@"RaccoonButterFlyFish

It is butterfly meat with a raccoon pattern. I usually live in tropical, subtropical waters.";
    public static readonly string Observe_JellyFish =
@"JellyFish

Most of the body is made up of water and poisonous.";
    public static readonly string Observe_NapoleonFish =
@"NapoleonFish

The protruding forehead is called Napoleon Fish because it resembles Napoleon with a hat.";
    public static readonly string Observe_MantaRay =
@"MantaRay

It looks as if the floating blanket is floating like a swimming thing, and it is named Manta.";
    public static readonly string Observe_ClownFish =
@"ClownFish

The orange white arrangement is a cute white clownfish. It is well known as the main character of the film 'Nimo'.";
    public static readonly string Observe_LionFish =
@"LionFish

There are a lot of thorns, and there are poisonous fins on several prongs, so be careful.";
    public static readonly string Observe_BlueTang =
@"BlueTang

It is a beautiful bluetang with bright blue, yellow and black.";
    public static readonly string Observe_PorcupineFish =
@"PorcupineFish

If you feel threatened, you will inflate your body so much that you will set your thorns. This thorn can move.";
    public static readonly string Observe_GreyNurseShark =
@"GreyNurseShark

Unlike a horrible appearance, personality is quiet and gentle. But if you keep on stimulating, you can be angry!";

    // 알아보기_과일야채
    public static readonly string Observe_Apple =
@"Apple

There are 7,500 known varieties; itis used to make cider and is alsoeaten raw or made into juice, jelly, compote or desserts, such aspie or strudel.";

    public static readonly string Observe_Grape =
@"Grape

Climbing plant usually cultivated for wine making or for the table.";


    // 알아보기_사물
    public static readonly string Observe_Cake =
@"Cake

Food made from flour, water and salt, often containing an agent (leaven or yeast) that makes it rise.";

    public static readonly string Observe_Egg =
@"Egg

By far the most commonly eaten, it is cooked as is or added torecipes; used alone, the word 'egg' refers to a hen's egg.";

    public static readonly string Observe_House =
@"House

Structure built as a dwelling and equipped to provide a comfortable and secure life for people.";

    public static readonly string Observe_Ice =
@"Ice

Ice is water frozen into a solid state.";

    public static readonly string Observe_Ketchup =
@"Ketchup

Medium spicytomato pur";

    public static readonly string Observe_Night =
@"Night

Night or nighttime is the period from sunset to sunrise in each twenty-four hours, when the Sun is below the horizon.";

    public static readonly string Observe_Queen =
@"Queen

the female ruler of an independent state, especially one who inherits the position by right of birth.";

    public static readonly string Observe_Robot =
@"Robot

A robot is a machine designed to execute one or more tasks automatically with speed and precision.";

    public static readonly string Observe_Ukulele =
@"Ukulele

A small guitar or banjo with four strings";

    public static readonly string Observe_Vase =
@"Vase

a container for holding flowers or for decoration";

    public static readonly string Observe_Water =
@"Water

Water is a transparent, tasteless, odorless, and nearly colorless chemical substance, which is the main constituent of Earth's streams, lakes, and oceans, and the fluids of most living organisms. ";

    public static readonly string Observe_Yoghurt =
@"Yoghurt

a semisolid sourish food prepared from milk fermented by added bacteria, often sweetened and flavored.";


    // 알아보기_곤충
    public static readonly string Observe_Bee =
@"bee

Insect living in a highly complex social order; it instinctively produces honey as a food reserve.";


    // 튜토리얼
    public static readonly string QRLicence_m =
@"To use this app, you need the product registration.

If you have purchased the product, you can use the QR code that is included with the product.
If you do not have the product, press the Skip button at the bottom, you can use without registration.

However, there are a limited number of products that can be used without registration.
After use restrictions, you have to register the product.

[How to register]

After running the app, QR code of the product can be recognized by align to the guidelines.
Touch the screen for auto-focus when recognition is not well aligned.
If it recognized properly, message will be displayed and go to the main screen.

Product registration is required only for first use.

For more information, please refer the MANUAL of our website.
Website address: http://www.hansapp.co.kr";

    public static readonly string QRFunction_m =
@"To use this function, you need the product registration.

If you have purchased the product, you can use the QR code that is included with the product.
If you do not have the product, press the Skip button at the bottom, you can use without registration.

However, there are a limited number of products that can be used without registration.
After use restrictions, you have to register the product.

[How to register]

After running the app, QR code of the product can be recognized by align to the guidelines.
Touch the screen for auto-focus when recognition is not well aligned.
If it recognized properly, message will be displayed and go to the next screen.

Product registration is required only for first use.

For more information, please refer the MANUAL of our website.
Website address: http://www.hansapp.co.kr";

    // 튜토리얼UI(일반씬)
    public static readonly string CommonTutorial_m =
@"This app, and shed image target in the camera, is augmented reality app that content is running.

If you purchased a product, you can use the product as a target.
If those who do not the product, press the Link button of the following to use and outputs a target of free samples.
Or, even in light of the target image of the home page MANUAL, augmented reality content will be executed.

[how to use]

1. illuminate the target of the image in the camera.
2. content that has been connected to the target is run.
3. Press the navigation button, you can see other content.
4. Use the here button, you can hear the description of the character.
5. You can see returns the character in the direction button.
6. menu at the bottom of the photo shoot, you can switch the camera, home, cafe visit.
7. You can move the characters by using the drag.

For more information, please refer to the MANUAL of the home page.
Website address: http://hansapp.co.kr";

    // 튜토리얼UI(스케치씬)
    public static readonly string SketchTutorial_m =
@"This app is an augmented reality app that launches content when the camera targets the image.

If you purchased the product, you can use it as a target.
If you do not have the product, you can print out the free sample target by clicking the Link button below.
Or, the augmented reality content is executed even if the target image of the homepage MANUAL is illuminated.

[How to use]

1. Colorize your drawing.
2. Shine a painted picture with the camera.
3. When the picture is recognized correctly, the red square turns blue.
4. 3D content is colored.
5. If the target is newly recognized, it will be colored again.

For more information, please refer to MANUAL on the homepage.
Homepage address: http://hansapp.co.kr";

    // 튜토리얼UI(미니맵 실사 씬)
    public static readonly string MiniMapRealTutorial_m =
@"This app is an augmented reality app that launches content when the camera targets the image.

If you purchased the product, you can use it as a target.
If you do not have the product, you can print out the free sample target by clicking the Link button below.
Or, the augmented reality content is executed even if the target image of the homepage MANUAL is illuminated.

[How to use]

1. Point the camera at the image target.
2. The content associated with the target runs.
3. When you do not recognize the target, a mini-map button appears.
4. You can push the mini-map button at the bottom of the screen to collect the contents in the mini-map.
5. You can zoom in / out the mini map by pressing the map button at the top right of the screen.

For more information, please refer to MANUAL on the homepage.
Homepage address: http://hansarapp.com";

    // 튜토리얼UI(미니맵 스케치 씬)
    public static readonly string MiniMapSketchTutorial_m =
@"This app is an augmented reality app that launches content when the camera targets the image.

If you purchased the product, you can use it as a target.
If you do not have the product, you can print out the free sample target by clicking the Link button below.
Or, the augmented reality content is executed even if the target image of the homepage MANUAL is illuminated.

[How to use]

1. Colorize your picture book.
2. Shine a painted picture with the camera.
3. When the picture is recognized correctly, the red square turns blue.
4. 3D content is colored.
5. When you do not recognize the target, a mini-map button appears.
6. You can push the mini-map button at the bottom of the screen to collect the contents in the mini-map.
7. You can zoom in / out the mini-map by pressing the map button at the top right of the screen.

For more information, please refer to MANUAL on the homepage.
Homepage address: http://hansarapp.com";
}