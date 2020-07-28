using UnityEngine;
using System;

public class LanguageVietnamese
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

    public static readonly string LocalizeSelect = "lựa chọn ngôn ngữ";
    public static readonly string SaveLocalizeSetting = "Sử dụng ngôn ngữ chính được lựa chọn";

    // Pop-up
    public static readonly string Exit_Q = "Bạn có chắc chắn muốn bỏ các ứng dụng?";
    public static readonly string Exit_A = "chấm dứt";
    public static readonly string Cancel = "hủy bỏ";
    public static readonly string Return_Q = "Bạn có muốn quay trở lại màn hình chính?";
    public static readonly string Return_A = "trả lại";

    // Contents
    public static readonly string ReadContents = "Nội dung đang tải.";
    public static readonly string LoadContents = "Đang tải nội dung ...";
    public static readonly string IlluminateImage = "\"Hãy cố gắng để làm sáng tỏ những hình ảnh\"";

    // Buttons
    public static readonly string Continue = "sự kế thừa";
    public static readonly string Question = "câu hỏi";
    public static readonly string Answer = "câu trả lời";

    public static readonly string ENG = "Anh";
    public static readonly string KOR = "Hàn Quốc";
    public static readonly string CHN = "Trung Quốc";
    public static readonly string JPN = "Nhật Bản";
    public static readonly string VTN = "tiếng Việt";

    public static readonly string Listen = "nghe";
    public static readonly string ListenAndLearn = "nghe & Học";

    // 데이터사용
    public static readonly string Wifi_NotConnected = "WI-FI không được kết nối.";
    public static readonly string MobileDatacall = "Mạng di động (3G / LTE, vv) có thể xảy ra trong quá trình kết nối cuộc gọi dữ liệu.";
    public static readonly string MobileDatacall_F = "Mạng di động (3G / LTE, vv) có thể xảy ra trong quá trình kết nối cuộc gọi dữ liệu. \n             (Data: {0}MB)";
    public static readonly string Download = "tải về";
    public static readonly string Back = "trở về";
    public static readonly string Wifi_NotGood = "WI-FI, kết nối mạng / LTE 3G là không tốt.";

    public static readonly string ShowOff_Q = "Tại sao không một lần nữa?";

    // QR코드
    public static readonly string ScanQR = "\"Quét mã QR.\"";
    public static readonly string CheckingQR = "Đang kiểm tra mã QR ...";
    public static readonly string OK = "tốt";
    public static readonly string Net_Unable = "Không thể kết nối với Internet.";
    public static readonly string QR_Unregistered = "Số sê-ri chưa đăng ký!\n\nMã QR đã quét chưa bao giờ được phát hành hoặc đã bị xóa.";
    public static readonly string QR_Expired = "Số sê-ri đã hết hạn!\n\nMã QR đã quét đã vượt quá số lần sử dụng được chỉ định.";
    public static readonly string QR_Invalid = "Số sê-ri không hợp lệ!\n\nMã QR được quét không có sẵn cho sản phẩm này.";
    public static readonly string QR_Unsuitable = "Số sê-ri không tương thích!\n\nMã QR được quét không tương thích với tính năng mong muốn.";
    public static readonly string DB_Error = "Lỗi cơ sở dữ liệu!";
    public static readonly string Server_Error = "Lỗi máy chủ!";
    public static readonly string QR_Success = "Đăng ký sản phẩm đã hoàn thành.";
    public static readonly string UsedExpChance = "Bạn đã sử dụng tất cả các cơ hội trải nghiệm.";

    public static readonly string Pass_Recognized = "Mục tiêu của bạn đã được công nhận.";
    public static readonly string Pass_Unrecognized = "Mục tiêu không được nhận dạng. Vui lòng kiểm tra mục tiêu của bạn.";

    // AssetDownloadManager.cs
    public static readonly string Net_Unstable = "Tình trạng hiện tại của mạng không ổn định.";
    public static readonly string Net_ReturnMain = "Dừng tải về và trở về màn hình chính, bạn chắc chắn rằng bạn muốn?";
    public static readonly string DownloadTotal_F = "Nội dung tải về... ( Total {0}MB )";
    public static readonly string DownloadProgress_F = "Nội dung tải về... ( {0}MB / {1}MB )";
    public static readonly string Net_Retry = "Các kết nối đến máy chủ không được mịn màng. Hãy thử lại sau.";
    public static readonly string Net_UpdateFinish = "Bản cập nhật nội dung là hoàn tất.";
    public static readonly string Net_CheckVer = "Kiểm tra phiên bản của nội dung của bạn.";

    // RecordScreenUI.cs
    public static readonly string Rec_Screen = "Buổi ghi hình của màn hình.";
    public static readonly string Rec_Finish = "Thu hình màn hình đã hoàn tất.";
    public static readonly string Rec_GrantRights = "Bạn đã không cho phép ghi âm!";
    public static readonly string Rec_NoMic = "Không có thiết bị micro!";
    public static readonly string Rec_NotSupport = "Các thiết bị không hỗ trợ ghi âm màn hình!";
    public static readonly string Rec_SaveAudio = "Và lưu các tập tin âm thanh.";
    public static readonly string Rec_SaveAudioFailed = "Tập tin âm thanh được lưu trữ trong các thất bại!";
    public static readonly string Rec_SaveVideoFailed = "Không thể lưu các tập tin video!";
    public static readonly string Rec_SaveVideo = "Và lưu các tập tin phim.";
    public static readonly string Rec_NoFolder = "Bạn chưa lưu thư mục Movies của bạn!";
    public static readonly string Rec_NoVideo = "Tôi không có bất kỳ tập tin video!";

    // MainUI.cs
    public static readonly string TargetCount_F = "Số mục tiêu nhận biết: ";

    // ExploreManager.cs
    public static readonly string AutoSearchOn = "Chế độ tìm kiếm tự động đã được thiết lập.";
    public static readonly string AutoSearchOff = "Chế độ tìm kiếm tự động đã được tắt.";

    // 튜토리얼
    public static readonly string QRLicence_m =
@"Để sử dụng ứng dụng này, bạn cần đăng ký sản phẩm.

Nếu bạn đã mua sản phẩm, bạn có thể sử dụng mã QR có trong sản phẩm này.
Nếu bạn không có sản phẩm này, hãy bấm vào nút Skip ở dưới cùng của sản phẩm có thể được sử dụng mà không cần đăng ký.

Tuy nhiên, có một số lượng hạn chế của sản phẩm có thể được sử dụng mà không cần đăng ký.
Sau khi sử dụng, giới hạn có thể được sử dụng để đăng ký sản phẩm.

[Làm thế nào để đăng ký]

Sau khi chạy các ứng dụng QR code gây ra các sản phẩm để đáp ứng các hướng dẫn công nhận.
Nếu công nhận cũng không phải là phù hợp khi bạn chạm vào màn hình để tự động tập trung.
Sau khi tin nhắn đã được công nhận đúng được hiển thị, hãy vào màn hình chính.

Đăng ký sản phẩm, trước tiên hãy chỉ sử dụng một lần.

Để biết thêm thông tin, vui lòng tham khảo trang web của MANUAL.
Địa chỉ web: http://www.hansapp.co.kr";

    public static readonly string QRFunction_m =
@"Để sử dụng tính năng này, bạn cần đăng ký sản phẩm.

Nếu bạn đã mua sản phẩm, bạn có thể sử dụng mã QR có trong sản phẩm này.
Nếu bạn không có sản phẩm này, hãy bấm vào nút Skip ở dưới cùng của sản phẩm có thể được sử dụng mà không cần đăng ký.

Tuy nhiên, có một số lượng hạn chế của sản phẩm có thể được sử dụng mà không cần đăng ký.
Sau khi sử dụng, giới hạn có thể được sử dụng để đăng ký sản phẩm.

[Làm thế nào để đăng ký]

Sau khi chạy các ứng dụng QR code gây ra các sản phẩm để đáp ứng các hướng dẫn công nhận.
Nếu công nhận cũng không phải là phù hợp khi bạn chạm vào màn hình để tự động tập trung.
Sau khi tin nhắn đã được công nhận đánh dấu đúng đắn để đi đến màn hình kế tiếp.

Đăng ký sản phẩm, trước tiên hãy chỉ sử dụng một lần.

Để biết thêm thông tin, vui lòng tham khảo trang web của MANUAL.
Địa chỉ web: http://www.hansapp.co.kr";

    // 튜토리얼UI(일반씬)
    public static readonly string CommonTutorial_m =
@"Ứng dụng này là một ứng dụng thực tế mở rộng mà chiếu sáng khuôn mặt với một nội dung mục tiêu hình ảnh camera để chạy.

Nếu bạn mua sản phẩm, bạn có thể sử dụng các sản phẩm với mục tiêu.
Nếu bạn không có sản phẩm này, nhấn vào nút liên kết dưới đây nếu bạn đang sử dụng một mục tiêu sản lượng mẫu miễn phí.
Hoặc, cũng trong ánh sáng của hình ảnh mục tiêu trong HƯỚNG DẪN chính là nội dung thực tế mở rộng để chạy.

[Làm thế nào]

1. Căn hình ảnh không phải mục tiêu vào máy ảnh.
2. Các nội dung có liên quan đến các mục tiêu đang chạy.
3. Bạn có thể bấm các nút điều hướng để xem nội dung khác nhau.
4. Tìm hiểu từ nút để nghe mô tả của các nhân vật.
5. Bạn có thể thấy các nhân vật trở lại như một nút hướng.
6. Các đơn thấp hơn là có sẵn để chụp ảnh, việc chuyển đổi máy ảnh, gia đình, quán cà phê lần.
7. Bạn có thể di chuyển nhân vật của bạn bằng cách kéo.

Để biết thêm thông tin, vui lòng tham khảo trang web của MANUAL.
Địa chỉ web: http://hansapp.co.kr";

    // 튜토리얼UI(스케치씬)
    public static readonly string SketchTutorial_m =
@"Ứng dụng này là một ứng dụng thực tế mở rộng mà chiếu sáng khuôn mặt với một nội dung mục tiêu hình ảnh camera để chạy.

Nếu bạn mua sản phẩm, bạn có thể sử dụng các sản phẩm với mục tiêu.
Nếu bạn không có sản phẩm này, nhấn vào nút liên kết dưới đây nếu bạn đang sử dụng một mục tiêu sản lượng mẫu miễn phí.
Hoặc, cũng trong ánh sáng của hình ảnh mục tiêu trong HƯỚNG DẪN chính là nội dung thực tế mở rộng để chạy.

[Làm thế nào]

1. màu trong hình ảnh cuốn sách tô màu mà bạn có.
2. Căn không tô màu một bức ảnh với máy ảnh.
3. hình chữ nhật màu đỏ khi các hình ảnh được công nhận đúng cách sẽ thay đổi sang màu xanh.
4. Các màu sắc sẽ gây ra cho nội dung 3D.
5. Nếu mục tiêu là một màu sắc mới được công nhận là mặc lại.

Để biết thêm thông tin, vui lòng tham khảo trang web của MANUAL.
Địa chỉ web: http://hansapp.co.kr";
}
