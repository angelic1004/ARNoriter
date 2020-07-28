using UnityEngine;
using System.Collections;

public class LanguageIndonesian : MonoBehaviour {

    #region Comment

    // TODO : Add localized value here.
    // You can define constant, static variable or static property.
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

    // 주의사항 : 영어 이외의 언어는 const로 선언할 경우 iOS 디바이스에서 문자열이 잘리는 문제가 있다.
    // 따라서, 영어가 아닌경우 static 또는 static readonly로 선언하거나 const를 간접 참조하도록 해야한다.

    public static readonly string LocalizeSelect = "Bahasa Indonesia";
    public static readonly string SaveLocalizeSetting = "Pilih bahasa";

    // Pop-up
    public static readonly string Exit_Q = "Apakah kamu ingin keluar?";
    public static readonly string Exit_A = "keluar";
    public static readonly string Cancel = "Batal";
    public static readonly string Return_Q = "Apakah kamu ingin kembali ke menu utama?";
    public static readonly string Return_A = "Kembali";
    public static readonly string Start = "??";

    // Contents
    public static readonly string ReadContents = "Sedang membaca konten.";
    public static readonly string LoadContents = "Menunggu konten ...";
    public static readonly string IlluminateImage = "\"Tolong arahkan ke gambar\"";






    // Buttons
    public static readonly string Continue = "Teruskan";
    public static readonly string Question = "Pertanyaan";
    public static readonly string Answer = "Jawaban";

    public static readonly string ENG = "Inggris";
    public static readonly string KOR = "Korea";
    public static readonly string CHN = "China";
    public static readonly string JPN = "Jepang";
    public static readonly string VTN = "Vietnam";
    public static readonly string Indonesian = "Indonesia";

    public static readonly string Listen = "Mendengarkan";
    public static readonly string ListenAndLearn = "Mendengar dan Belajar";

    // 데이터사용
    public static readonly string Wifi_NotConnected = "Wifi tidak tersambung";
    public static readonly string MobileDatacall = "Data ( 3G/LTE, etc.) akan terpakai saat koneksi";
    public static readonly string MobileDatacall_F = @"Data ( 3G/LTE, etc.) akan terpakai saat koneksi \n             (Data: {0}MB)";
    public static readonly string Download = "Mengunduh";
    public static readonly string Back = "Kembali";
    public static readonly string Wifi_NotGood = "Koneksi Wi-FI atau 3G/LTE tidak bagus";

    public static readonly string ShowOff_Q = "Jangan tampilkan lagi";

    // QR코드
    public static readonly string ScanQR = "\"Memindai kode QR.\"";
    public static readonly string CheckingQR = "Memeriksa kode QR ...";
    public static readonly string OK = "Ya";
    public static readonly string Net_Unable = "Tidak bisa terhubung ke internet";
    public static readonly string QR_Unregistered = "Nomor seri tidak terdaftar!\n\nKode QR yang dipindai belum pernah diterbitkan atau sudah dihapus.";
    public static readonly string QR_Expired = "Nomor seri kadaluwarsa!\n\nKode QR yang dipindai telah melebihi jumlah penggunaan yang ditentukan.";
    public static readonly string QR_Invalid = "Nomor seri tidak valid!\n\nKode QR yang dipindai tidak dapat digunakan untuk produk ini!";
    public static readonly string QR_Unsuitable = "Nomor seri tidak kompatibel!\n\nKode QR yang dipindai tidak kompatibel dengan fungsi yang diinginkan.";
    public static readonly string DB_Error = "Kesalahan basis data!";
    public static readonly string Server_Error = "Kesalahan server!";
    public static readonly string QR_Success = "Pendaftaran produk telah selesai.";
    public static readonly string UsedExpChance = "Anda telah menggunakan semua kesempatan pengalaman.";

    public static readonly string Pass_Recognized = "Target ini telah diverifikasi.";
    public static readonly string Pass_Unrecognized = "Target ini belum diverifikasi. Silakan periksa target anda.";

    // AssetDownloadManager.cs
    public static readonly string Net_Unstable = "Jaringan internet tidak stabil";
    public static readonly string Net_ReturnMain = "Apakah anda yakin untuk menghentikan pengunduhan dan kembali ke menu utama?";
    public static readonly string DownloadTotal_F = "Mengunduh konten ( Total {0} MB)";
    public static readonly string DownloadProgress_F = "Mengunduh konten ({0} MB/ {1} MB)";
    public static readonly string DownloadProgress2_F = "Mengunduh konten\\n({0} MB/ {1} MB)";
    public static readonly string Net_Retry = "Koneksi tidak lancar. Mohon coba lagi";
    public static readonly string Net_UpdateFinish = "Perbaharuan telah selesai";
    public static readonly string Net_CheckVer = "Memeriksa versi konten";

    // RecordScreenUI.cs
    public static readonly string Rec_Screen = "Merekam gambar di layar";
    public static readonly string Rec_Finish = "Rekaman selesai";
    public static readonly string Rec_GrantRights = "Gagal untuk merekam";
    public static readonly string Rec_NoMic = "Tidak ada perangkat mikrofon";
    public static readonly string Rec_NotSupport = "Perangkat anda tidak mendukung untuk merekam layar";
    public static readonly string Rec_SaveAudio = "Menyimpan data suara";
    public static readonly string Rec_SaveAudioFailed = "Gagal menyimpan data suara";
    public static readonly string Rec_SaveVideoFailed = "Gagal menyimpan data video";
    public static readonly string Rec_SaveVideo = "Menyimpan data video";
    public static readonly string Rec_NoFolder = "Tidak ada folder video tersimpan";
    public static readonly string Rec_NoVideo = "Tidak ada data video tersimpan";

    // MainUI.cs
    public static readonly string TargetCount_F = "Mengenali jumlah target : ";

    // ExploreManager.cs
    public static readonly string AutoSearchOn = "Mode pencarian otomatis telah ditetapkan.";
    public static readonly string AutoSearchOff = "Mode pencarian otomatis dimatikan.";

    // 카테고리




    public static readonly string Princess = "Putri";
    public static readonly string RacingCar = "Mobil Balap";
    public static readonly string Soccer = "Sepak Bola";

    public static readonly string FourD = "Pengalaman 4D";
    public static readonly string Coloring = "4D Mewarnai";

    public static readonly string PuzzleGame = "Puzzle";



    public static readonly string Observe = "Observasi";
    public static readonly string Study = "Belajar";















    public static readonly string SoccerGame = "Permainan Sepak Bola";
    public static readonly string FourDRacingGame = "Balap Mobil 4D";
    public static readonly string RacingDrive = "Balapan";
    public static readonly string Video = "??";

    public static readonly string Black = "Black";
    public static readonly string Pink = "Pink";
    public static readonly string SkyBox = "SkyBox";


    //Navigation.cs
    public static readonly string MinimapReal = "Minimap Real";
    public static readonly string MinimapSketch = "Sketsa Minimap";
    public static readonly string Color = "Warna";
    public static readonly string Other = "lain";
    public static readonly string RacingGame = "Balap Mobil 4D";

    public static readonly string ArPlay = "AR PLAY";
    public static readonly string DanceDance = "Dance";
    public static readonly string DanceBattle = "Dance Battle";
    public static readonly string Runway = "Runway";
    public static readonly string ColorRunway = "Color Runway";

    public static readonly string Manual = "Manual";
    public static readonly string Category = "Kategori";
    public static readonly string LanguageSelect = "Bahasa";
    public static readonly string Sound = "Suara";

    //PrincessManager.cs 
    public static readonly string ClowQuestion = "Temukan putri yang tepat.";
    public static readonly string ClowGameOver = "Selamat! Kamu telah menjawab semua pertanyaan.";
    public static readonly string ClowSelectAnswer = "Benar! Kerja bagus!";
    public static readonly string ClowSelectWrong = "Kurang sedikit lagi~ ayo coba lagi~!";
    public static readonly string ClowRestart = "Mengulang permainan pancing boneka. ~!";
    public static readonly string Restart = "Mulai lagi";

    //퍼즐 
    public static readonly string MatchingNumber = "Point";
    public static readonly string ChallengeNumber = "No. of try";

    // Dance Warning Message
    public static readonly string DanceCharacterSelectMsg = "Pilih posisi sebelum memilih karakter";

    // 공주 Dance
    public static readonly string ChooseDancerNumber = "Pilih berapa banyak personil dansa";
    public static readonly string ChooseDancer = "Pilih penari";
    public static readonly string DanceDancePopupMsg = "Target terkenali. Apakah kamu ingin mengakhiri Dance - Dance?";
    public static readonly string DanceBattlePopupMsg = "Target terkenali. Apakah kamu ingin mengakhiri Dance Battle?";

    // 공주 스티커
    public static readonly string PrincessShadowSticker = "Princess Shadow Sticker";
    public static readonly string PrincessBallroom = "Putri cantik berkumpul di ballroom kerajaan. Ayo kita lihat siapa yang datang dan meletakkan stiker di ballroom.";
    public static readonly string StickPuzzleSticker = "Tempelkan stiker puzzle";
    public static readonly string SuitablePuzzle = "Ayo kita letakkan stiker puzzle yang sesuai ke ruang kosong dan lengkapi lukisan.";
    public static readonly string IamOdette = "Namaku Odette! Aku menari balet dengan gerakan anggun.";
    public static readonly string IamMermaid = "Aku Putri Duyung, aku dapat berenang bebas dengan siripku!";

    public static readonly string SnowWhite = "Putri Salju";
    public static readonly string PrincessNavia = "Putri Navia";
    public static readonly string PrincessBari = "Putri Bari";
    public static readonly string PrincessBell = "Putri Bell";
    public static readonly string Cinderella = "Cinderella";
    public static readonly string Cleopatra = "Cleopatra";
    public static readonly string Odette = "Odette";
    public static readonly string Rapunzel = "Rapunzel";
    public static readonly string Thumbelina = "Thumbelina";
    public static readonly string PrincessMermaid = "Putri Duyung";

    // 4D 레이싱 게임
    public static readonly string RacingWinner = "Selamat! Anda memenangkan juara pertama";
    public static readonly string RacingStart = "Saat gambar tersorot kamera, game balap akan dimulai.";
    public static readonly string LapTime = "Waktu Putaran";
    public static readonly string Speed = "Kecepatan";

    // 축구 게임
    public static readonly string StartSoccerGame = "Arahkan ke gambar, Anda bisa memulai permainan sepak bola!";
    public static readonly string StartSoccerVideo = "Scan gambar dan kamu dapat melihat video sepak bola!";
    public static readonly string SelectGameMode = "Pilih mode game!";
    public static readonly string PenaltyKick = "Tendangan Penalti";
    public static readonly string FreeKick = "Tendangan Bebas";
    public static readonly string FreeKick_Explain = "Tendangan bebas mendebarkan, tendang bola gol di atas tembok";
    public static readonly string KickerNumber = "Silahkan pilih jumlah pemain";

    public static readonly string InputYourTeam = "Masukkan nama tim kamu";
    public static readonly string InputTeam = "Masukkan nama tim";
    public static readonly string InputTeamA = "Masukkan nama tim A";
    public static readonly string InputTeamB = "Masukkan nama tim B";
    public static readonly string NoTeamname = "Tidak ada nama team yang tertulis. Tolong tulis ulang";
    public static readonly string SameTeam = "Nama sudah ada. Mohon pilih nama lain";
    public static readonly string SameTeamA = "Nama tim sama dengan tim A. Silahkan tulis ulang";
    public static readonly string SameTeamB = "Nama tim sama dengan tim B. Silahkan tulis ulang";
    public static readonly string SelectUniform = "Pilih seragam";
    public static readonly string TeamAUniform = "Pilih seragam tim A";
    public static readonly string TeamBUniform = "Pilih seragam tim B";
    public static readonly string SameUniform = "Kamu tidak bisa memilih seragam yang sama dengan tim lawan.";
    public static readonly string SelectNumberPlayer = "Pilih nomor pemain";
    public static readonly string Select = "Pilih";
    public static readonly string ChooseCharacter = "Pilih pemain";
    public static readonly string SelectPlayer = "Pilih pemain";
    public static readonly string TeamAPlayer = "Pilih pemain tim A";
    public static readonly string TeamBPlayer = "Pilih pemain tim B";

    public static readonly string Scout = "Mencari";
    public static readonly string ScoutCancel = "Batalkan pencarian";

    public static readonly string SoccerRestart = "Apakah kamu ingin mengulang?";

    public static readonly string Shoot = "Tendang";
    public static readonly string NormalShoot = "Normal";
    public static readonly string GroundShoot = "Dasar";
    public static readonly string ToeShoot = "Ujung Kaki";
    public static readonly string InsideShoot = "Dalam";

    // Watch Car
    public static readonly string WatchCarPlay = "WatchCar Play";
    public static readonly string BattleGame    = "Battle Game";
    public static readonly string RoadDriving   = "Road Driving";

    public static readonly string WarchCarBattlePopupMsg = "Target terkenali. Apakah kamu ingin mengakhiri WarchCar Battle?";

    // Watch Car Edu
    public static readonly string WrongWay = "Wrong way";
    public static readonly string DriveAgain = "Drive again";

    // Nas Car
    public static readonly string NasCar = "NasCar";
    
    // Track
    public static readonly string RacingDrive_RaceTrack01 = "Balapan - Track 1";
    public static readonly string RacingDrive_RaceTrack02 = "Balapan - Track 2";

    // 튜토리얼
    public static readonly string QRLicence_m =
@"Untuk menggunakan aplikasi ini, Anda membutuhkan registrasi produk.

Jika Anda telah membeli produk, Anda dapat menggunakan kode QR yang disertakan dengan produk.
Jika Anda tidak memiliki produk, tekan tombol Lewati di bagian bawah, Anda dapat menggunakan tanpa registrasi.

Namun, ada sejumlah produk yang dapat digunakan tanpa registrasi.
Setelah pembatasan penggunaan, Anda harus mendaftar produk.

[Cara mendaftar]

Setelah menjalankan aplikasi, QR code dari produk dapat dikenali oleh menyelaraskan dengan pedoman.
Sentuh layar untuk auto-fokus saat pengakuan tidak baik selaras.
Jika benar diakui, pesan akan ditampilkan dan pergi ke layar utama.

registrasi produk diperlukan hanya untuk penggunaan pertama.

Untuk informasi lebih lanjut, silakan merujuk MANUAL dari website kami.
Alamat website: http://www.hansapp.co.kr";

    public static readonly string QRFunction_m =
@"Untuk menggunakan fungsi ini, Anda membutuhkan registrasi produk.

Jika Anda telah membeli produk, Anda dapat menggunakan kode QR yang disertakan dengan produk.
Jika Anda tidak memiliki produk, tekan tombol Lewati di bagian bawah, Anda dapat menggunakan tanpa registrasi.

Namun, ada sejumlah produk yang dapat digunakan tanpa registrasi.
Setelah pembatasan penggunaan, Anda harus mendaftar produk.

[Cara mendaftar]

Setelah menjalankan aplikasi, QR code dari produk dapat dikenali oleh menyelaraskan dengan pedoman.
Sentuh layar untuk auto-fokus saat pengakuan tidak baik selaras.
Jika benar diakui, pesan akan ditampilkan dan pergi ke layar berikutnya.

registrasi produk diperlukan hanya untuk penggunaan pertama.

Untuk informasi lebih lanjut, silakan merujuk MANUAL dari website kami.
Alamat website: http://www.hansapp.co.kr";

    // 튜토리얼UI(일반씬)
    public static readonly string CommonTutorial_m =
@"Aplikasi ini, dan menumpahkan sasaran gambar di kamera, adalah augmented reality app bahwa konten sedang berjalan.

Jika Anda membeli sebuah produk, Anda dapat menggunakan produk sebagai target.
Jika mereka yang tidak produk, tekan tombol link berikut untuk menggunakan dan output target sampel gratis.
Atau, bahkan dalam cahaya dari gambar target halaman rumah MANUAL, konten augmented reality akan dieksekusi.

[bagaimana cara menggunakan]

1. menerangi target gambar di kamera.
2. konten yang telah terhubung ke target dijalankan.
3. Tekan tombol navigasi, Anda dapat melihat konten lainnya.
4. Gunakan disini tombol, Anda dapat mendengar deskripsi karakter.
5. Anda dapat melihat kembali karakter di tombol arah.
6. menu di bagian bawah pemotretan, Anda dapat mengganti kamera, rumah, cafe kunjungan.
7. Anda dapat memindahkan karakter dengan menggunakan drag.

Untuk informasi lebih lanjut, silakan merujuk ke MANUAL dari halaman rumah.
Alamat website: http://hansapp.co.kr";

    // 튜토리얼UI(스케치씬)
    public static readonly string SketchTutorial_m =
@"Aplikasi ini adalah sebuah aplikasi augmented reality yang meluncurkan konten saat kamera menargetkan gambar.

Jika Anda membeli produk, Anda dapat menggunakannya sebagai target.
Jika Anda tidak memiliki produk, Anda dapat mencetak target sampel gratis dengan mengklik tombol link di bawah ini.
Atau, konten augmented reality dijalankan bahkan jika target citra homepage MANUAL menyala.

[Bagaimana cara menggunakan]

1. Warnai gambar Anda.
2. Bersinar gambar dicat dengan kamera.
3. Bila gambar tersebut diakui dengan benar, alun-alun merah menjadi biru.
4. konten 3D berwarna.
5. Jika target ini baru diakui, itu akan berwarna lagi.

Untuk informasi lebih lanjut, silakan lihat MANUAL pada homepage.
Alamat website: http://hansapp.co.kr";
}
