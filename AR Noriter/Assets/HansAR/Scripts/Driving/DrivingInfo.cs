using UnityEngine;
using System.Collections;

public class DrivingInfo : MonoBehaviour {

    public Avatar driveAvatar;
    public RuntimeAnimatorController AniController;

    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;

    //충돌 및 이벤트를 위한 오브젝트의 앞부분 포지션(z값만 사용)
    public Vector3 headPosition;
}
