using UnityEngine;
using System;

public class LanguageJapanese
{
    #region 한글 주석

    // TODO : 여기에 지역화한 값을 추가합니다.
    // 상수, 정적 변수 또는 정적 속성을 선언할 수 있습니다.
    //
    // public const string ConfirmDelete = "삭제하시겠습니까?";
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

    public static readonly string LocalizeSelect = "言語の選択";
    public static readonly string SaveLocalizeSetting = "選択した言語を基本言語で使用";

    // Pop-up
    public static readonly string Exit_Q = "アプリを終了しますか？";
    public static readonly string Exit_A = "終了";
    public static readonly string Cancel = "キャンセル";
    public static readonly string Return_Q = "メイン画面に戻りますか？";
    public static readonly string Return_A = "戻る";

    // Contents
    public static readonly string ReadContents = "コンテンツを ロードして います。";
    public static readonly string LoadContents = "コンテンツを ロード中...";
    public static readonly string IlluminateImage = "\"画像を 照らして みて ください\"";

    // Buttons
    public static readonly string Continue = "連続";
    public static readonly string Question = "質問";
    public static readonly string Answer = "回答";

    public static readonly string ENG = "英語";
    public static readonly string KOR = "韓国語";
    public static readonly string CHN = "中国語";
    public static readonly string JPN = "日本語";
    public static readonly string VTN = "ベトナム語";

    public static readonly string Listen = "リスニング";
    public static readonly string ListenAndLearn = "聞いての学習";

    // 데이터사용
    public static readonly string Wifi_NotConnected = "WI-FI接続がされていません。";
    public static readonly string MobileDatacall = "移動通信網（3G/ LTEなど）で接続時のデータ通話料が発生することがあります。";
    public static readonly string MobileDatacall_F = "移動通信網（3G/ LTEなど）で接続時のデータ通話料が発生することがあります。\n             (データ量: {0}MB)";
    public static readonly string Download = "ダウンロード";
    public static readonly string Back = "戻る";
    public static readonly string Wifi_NotGood = "WIFI、3G/ LTEネットワーク接続が良好ません。";

    public static readonly string ShowOff_Q = "再び見ていただけませんか?";

    // QR코드
    public static readonly string ScanQR = "\"QRコードをスキャンしてください。\"";
    public static readonly string CheckingQR = "QRコードをチェック中...";
    public static readonly string OK = "確認";
    public static readonly string Net_Unable = "インターネットに接続できません。";
    public static readonly string QR_Unregistered = "未登録シリアル番号！\n\nスキャンしたQRコードは、発行されたことがないか、すでに削除されました。";
    public static readonly string QR_Expired = "期限切れのシリアル番号！\n\nスキャンしたQRコードは、指定された使用回数を超えました。";
    public static readonly string QR_Invalid = "無効なシリアル番号！\n\nスキャンしたQRコードは、この製品では使用できません。";
    public static readonly string QR_Unsuitable = "不適合シリアル番号！\n\nスキャンしたQRコードはご希望の機能と互換性がありません。";
    public static readonly string DB_Error = "データベースエラー！";
    public static readonly string Server_Error = "サーバーエラー！";
    public static readonly string QR_Success = "製品登録が完了しました。";
    public static readonly string UsedExpChance = "体験回数をすべて使いました。";

    public static readonly string Pass_Recognized = "このターゲットは認識されています。";
    public static readonly string Pass_Unrecognized = "このターゲットは認識されていません。あなたのターゲットを確認してください。";

    // AssetDownloadManager.cs
    public static readonly string Net_Unstable = "現在、ネットワークの状態が不安定になります。";
    public static readonly string Net_ReturnMain = "ダウンロードを中止し、メイン画面に戻りますか？";
    public static readonly string DownloadTotal_F = "コンテンツをダウンロード中です。(総 {0}MB)";
    public static readonly string DownloadProgress_F = "コンテンツの ダウンロード {0}MB / {1}MB 進行中..";
    public static readonly string Net_Retry = "サーバーとの接続状態がスムーズません。しばらくしてからもう一度やり直してください。";
    public static readonly string Net_UpdateFinish = "コンテンツの 更新が 完了しました。";
    public static readonly string Net_CheckVer = "コンテンツの バージョンを 確認して います。";

    // RecordScreenUI.cs
    public static readonly string Rec_Screen = "画面録画中です。";
    public static readonly string Rec_Finish = "画面録画が完了しました。";
    public static readonly string Rec_GrantRights = "録画権限を与えられていませんでした！";
    public static readonly string Rec_NoMic = "マイクがありません！";
    public static readonly string Rec_NotSupport = "画面録画をサポートしていないデバイスです！";
    public static readonly string Rec_SaveAudio = "オーディオファイルを保存しています。";
    public static readonly string Rec_SaveAudioFailed = "オーディオファイルの保存に失敗しました！";
    public static readonly string Rec_SaveVideoFailed = "動画ファイルの保存に失敗しました！";
    public static readonly string Rec_SaveVideo = "動画ファイルを保存しています。";
    public static readonly string Rec_NoFolder = "保存された動画フォルダがありません！";
    public static readonly string Rec_NoVideo = "保存された動画ファイルがありません！";

    // MainUI.cs
    public static readonly string TargetCount_F = "認識可能ターゲット数 : ";

    // ExploreManager.cs
    public static readonly string AutoSearchOn = "自動 探索モードが 設定されました。";
    public static readonly string AutoSearchOff = "自動 探索モードが 解除されました。";

    // 튜토리얼
    public static readonly string QRLicence_m =
@"このアプリを使用するためには製品登録が必要です。

製品を購入した場合には該当製品に同梱されているQRコードを使用すればいいです。
もし製品がなければ、下記のSkipボタンを押して製品を登録なしに使用することができます。

ただし、製品を登録をしないで使用できる回数が制限されています。 
使用制限以降は製品登録をしなければ使うことができます。

[登録方法]

アプリを実行した上で製品QRコードをガイドラインに合わせて認識させます。
認識がうまくいかない場合、画面をタッチすると、自動的に焦点が合わせられます。
正常に認識されたというメッセージが表示された後、メイン画面に移行ます。

製品を登録は初使用時1回だけしてくださればいいです。

詳しい情報は、ホームページのMANUALを参考にしてください。
ホームページ住所 : http://www.hansapp.co.kr";

    public static readonly string QRFunction_m =
@"この機能を使用するためには製品登録が必要です。

製品を購入した場合には該当製品に同梱されているQRコードを使用すればいいです。
もし製品がなければ、下記のSkipボタンを押して製品を登録なしに使用することができます。

ただし、製品を登録をしないで使用できる回数が制限されています。 
使用制限以降は製品登録をしなければ使うことができます。

[登録方法]

アプリを実行した上で製品QRコードをガイドラインに合わせて認識させます。
認識がうまくいかない場合、画面をタッチすると、自動的に焦点が合わせられます。
正常に認識されたというメッセージが表示された後、次の画面に移行ます。

製品を登録は初使用時1回だけしてくださればいいです。

詳しい情報は、ホームページのMANUALを参考にしてください。
ホームページ住所 : http://www.hansapp.co.kr";

    // 튜토리얼UI(일반씬)
    public static readonly string CommonTutorial_m =
@"このアプリはカメラでイメージターゲットを照らすと、コンテンツが実行される拡張現実のアプリです。

製品を購入した場合には該当製品をターゲットに使うことができます。
もし製品がなければ、下記のLinkボタンを押して無料サンプルターゲットを出力して使用すればいいです。
または、ホームページMANUALのターゲットイメージを照らしても増強現実のコンテンツが実行されます。

[使用方法]

1.イメージターゲットをカメラに映すます。
2.ターゲットに連結されたコンテンツが実行されます。
3.探索ボタンをクリックすると、他のコンテンツを見ることができます。
4.調べるボタンを通じて、該当キャラクターに対する説明を聞くことができます。
5.方向ボタンでキャラクターを回して見ることができます。
6.下段メニューは写真撮影、カメラ転換、ホームページ、カフェの訪問が可能です。
7.ドラッグを通じてキャラクターを動かすことができます。

詳しい情報は、ホームページのMANUALを参考にしてください。
ホームページ住所:http://hansapp.co.kr";

    // 튜토리얼UI(스케치씬)
    public static readonly string SketchTutorial_m =
@"このアプリはカメラでイメージターゲットを照らすと、コンテンツが実行される拡張現実のアプリです。

製品を購入した場合には該当製品をターゲットに使うことができます。
もし製品がなければ、下記のLinkボタンを押して無料サンプルターゲットを出力して使用すればいいです。
または、ホームページMANUALのターゲットイメージを照らしても増強現実のコンテンツが実行されます。

[使用方法]

1.持っていた色、北朝鮮の絵に色をします。
2.色絵をカメラに映すます。
3.絵が正しく認識されれば、赤い四角形が青色に変わります。
4.3Dコンテンツに色が描かれます。
5.ターゲットを新たに認識したらまた色が描かれます。

詳しい情報は、ホームページのMANUALを参考にしてください。
ホームページ住所 : http://hansapp.co.kr";
}
