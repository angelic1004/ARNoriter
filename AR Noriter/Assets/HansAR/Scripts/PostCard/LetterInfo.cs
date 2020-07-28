using UnityEngine;
using System.Collections;

public class LetterInfo : MonoBehaviour
{
    public Collider entireBody;             // 전체 콜라이더
    public Material letterMat;              // 엽서 머터리얼

    //public Material dearMat;                // dear 머터리얼
    public Material photoMat;               // 사진 머터리얼
    public Collider photoBox;               // 사진박스 콜라이더
    public Collider takePicBtn;             // TakePic 버튼 콜라이더
    public Collider albumBtn;               // 앨범 버튼 콜라이더
    public Collider resetBtn;               // 리셋 버튼 콜라이더

    public GameObject TakePicBtnObj;
    public GameObject AlbumBtnObj;
    public GameObject ResetBtnObj;

    public GameObject faceTextureObj;
    public GameObject videoPlayerObj;

    public GameObject textPosLeftTop;
    public GameObject textPosRightBottom;

    public GameObject faceboxLeftTop;
    public GameObject faceboxRightBottom;
    public Texture2D[] textureList;

    public float letterTextOpenFrame;

    public int textboxWidth;
    public int textboxHeight;

    public int faceboxWidth;
    public int faceboxHeight;
}
