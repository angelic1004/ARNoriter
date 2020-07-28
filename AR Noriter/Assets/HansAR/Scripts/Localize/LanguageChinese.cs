using UnityEngine;
using System;

public class LanguageChinese
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

    public static readonly string LocalizeSelect = "语言选择";
    public static readonly string SaveLocalizeSetting = "使用选择的主要语言";

    // Pop-up
    public static readonly string Exit_Q = "你确定要结束程序？";
    public static readonly string Exit_A = "结束";
    public static readonly string Cancel = "取消";
    public static readonly string Return_Q = "你确定要返回到主屏幕？";
    public static readonly string Return_A = "回去";

    // Contents
    public static readonly string ReadContents = "读取内容。";
    public static readonly string LoadContents = "加载内容...";
    public static readonly string IlluminateImage = "\"请照亮图像。\"";

    // Buttons
    public static readonly string Continue = "演替";
    public static readonly string Question = "查询";
    public static readonly string Answer = "答案";

    public static readonly string ENG = "英语";
    public static readonly string KOR = "韩国语";
    public static readonly string CHN = "中文";
    public static readonly string JPN = "日语";
    public static readonly string VTN = "越南语";

    public static readonly string Listen = "听力";
    public static readonly string ListenAndLearn = "听与学";

    // 데이터사용
    public static readonly string Wifi_NotConnected = "WI-FI未连接。";
    public static readonly string MobileDatacall = "移动网络（3G / LTE等）可以在数据呼叫连接期间发生。";
    public static readonly string MobileDatacall_F = "移动网络（3G / LTE等）可以在数据呼叫连接期间发生。(数据量: {0}MB)";
    public static readonly string Download = "下载";
    public static readonly string Back = "回去";
    public static readonly string Wifi_NotGood = "WI-FI，3G / LTE网络连接不好。";

    public static readonly string ShowOff_Q = "为什么不来着？";

    // QR코드
    public static readonly string ScanQR = "\"扫描QR码。\"";
    public static readonly string CheckingQR = "检查QR码......";
    public static readonly string OK = "确认";
    public static readonly string Net_Unable = "无法连接到互联网。";
    public static readonly string QR_Unregistered = "未注册的序列号！\n\n扫描的QR码从未发布过或已被删除。";
    public static readonly string QR_Expired = "过期的序列号！\n\n扫描的QR码超过了指定的使用次数。";
    public static readonly string QR_Invalid = "无效的序列号！\n\n扫描的QR码不能用于本产品。";
    public static readonly string QR_Unsuitable = "不合适的序列号！\n\n扫描的QR码与所需的功能不兼容。";
    public static readonly string DB_Error = "数据库错误！";
    public static readonly string Server_Error = "服务器错误！";
    public static readonly string QR_Success = "产品注册完成。";
    public static readonly string UsedExpChance = "使用感受已经过期。";

    public static readonly string Pass_Recognized = "该目标已经过验证。";
    public static readonly string Pass_Unrecognized = "此目标尚未经过验证。请检查你的目标。";

    // AssetDownloadManager.cs
    public static readonly string Net_Unstable = "网络状态不稳定。";
    public static readonly string Net_ReturnMain = "是否要停止下载，并返回到主屏幕？";
    public static readonly string DownloadTotal_F = "内容的下载。( 共 {0}MB )";
    public static readonly string DownloadProgress_F = "内容的下载... {0}MB / {1}MB进展";
    public static readonly string DownloadProgress2_F = "内容的下载...\\n{0}MB / {1}MB进展";
    public static readonly string Net_Retry = "与服务器的连接不顺畅。请稍后再试。";
    public static readonly string Net_UpdateFinish = "内容更新完成。";
    public static readonly string Net_CheckVer = "检查内容的版本。";

    // RecordScreenUI.cs
    public static readonly string Rec_Screen = "录制屏幕。";
    public static readonly string Rec_Finish = "屏幕录制完成。";
    public static readonly string Rec_GrantRights = "未能授予记录的权利！";
    public static readonly string Rec_NoMic = "没有麦克风设备！";
    public static readonly string Rec_NotSupport = "您的设备不支持屏幕录像！";
    public static readonly string Rec_SaveAudio = "保存的音频文件。";
    public static readonly string Rec_SaveAudioFailed = "无法保存音频文件！";
    public static readonly string Rec_SaveVideoFailed = "无法保存视频文件！";
    public static readonly string Rec_SaveVideo = "保存的视频文件。";
    public static readonly string Rec_NoFolder = "没有保存的视频文件夹！";
    public static readonly string Rec_NoVideo = "没有保存的视频文件！";

    // MainUI.cs
    public static readonly string TargetCount_F = "识别目标数：";

    // ExploreManager.cs
    public static readonly string AutoSearchOn = "自动搜索模式已被设定。";
    public static readonly string AutoSearchOff = "自动搜索模式被关闭。";

    // 튜토리얼
    public static readonly string QRLicence_m =
@"要使用這個應用程序，你需要的產品註冊。

如果您購買的產品，您可以使用附帶本產品的QR碼。
如果沒有這個產品中，按在產品的底部的下跳过按鈕可以註冊使用。

然而，也有可以註冊中使用的產品的數量有限。
使用後，該限制可用於註冊該產品。

[如何註冊]

在運行應用程序之後QR碼使產品達到公認的準則。
如果確認沒有很好，當你觸摸屏幕對準自動對焦。
該消息已被正確識別顯示後，進入主界面。

產品註冊，請首先使用一次。

欲了解更多信息，請參閱手冊主頁。
網址：http://www.hansapp.co.kr";

    public static readonly string QRFunction_m =
@"要使用此功能，您所需要的产品注册。

如果您购买的产品，您可以使用附带本产品的QR码。
如果没有这个产品中，按在产品底部的跳过按钮可以注册使用。

然而，也有可以注册中使用的产品的数量有限。
使用后，该限制可用于注册该产品。

[如何注册]

在运行应用程序之后QR码使产品达到公认的准则。
如果确认没有很好，当你触摸屏幕对准自动对焦。
该消息已被确认后妥善标识转到下一屏幕。

产品注册，请首先使用一次。

欲了解更多信息，请参阅手册主页。
网址：http://www.hansapp.co.kr﻿";

    // 튜토리얼UI(일반씬)
    public static readonly string CommonTutorial_m =
@"这个程序是一个增强现实应用程序，带有摄像头的图像目标内容运行照亮了脸。

如果您购买的产品，您可以使用该产品的目标。
如果你没有这个产品，你可以地新闻下面的链接按钮，打印出免费样品的目标。
或者，也在主手册目标图像的光线增强现实内容来运行。

[如何使用]

1. 请对准目标图像到相机。
2. 与目标图象相关联的内容将运行。
3. 您可以通过按下导航按钮看其他内容。
4. 您可以通过按学习按钮听到的性格描述。
5. 您可以按方向键旋转字符。
6. 底部菜单可用来拍照，摄像头开关，主页，咖啡馆参观。
7. 您可以通过拖动来移动人物。

欲了解更多信息，请参阅MANUAL在主页上。
网址：http://hansapp.co.kr";

    // 튜토리얼UI(스케치씬)
    public static readonly string SketchTutorial_m =
@"这个程序是一个增强现实应用程序，带有摄像头的图像目标内容运行照亮了脸。

如果您购买的产品，您可以使用该产品的目标。
如果你没有这个产品，你可以地新闻下面的链接按钮，打印出免费样品的目标。
或者，也在主手册目标图像的光线增强现实内容来运行。

[如何使用]

1. 我们必须在图画书的照片着色。
2. 它闪耀着相机图像画。
3. 如果图片被正确识别的红色矩形将变为蓝色。
4. 颜色是披着3D内容。
5. 如果目标是一个新认识的色彩再次穿着。

欲了解更多信息，请参阅MANUAL在主页上。
网址：http://hansapp.co.kr";
}
