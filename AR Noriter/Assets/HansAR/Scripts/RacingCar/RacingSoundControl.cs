using UnityEngine;
using System.Collections;

public class RacingSoundControl : MonoBehaviour {

    public enum whatCategory
    {
        Observe = 0,
        Game = 1,
        Racing = 2
    }

    public string soundDataName;

    public GameObject rootObj;

    public whatCategory category;

    public static RacingSoundControl instance;

    void Awake()
    {
        instance = this;
    }

    public void GetSoundData(GameObject prefab)
    {
        RacingSoundInfo soundInfo = prefab.GetComponent<RacingSoundInfo>();

        if (category == whatCategory.Observe)
        {
            RacingFourD soundData = RacingFourD.instance;

            soundData.idleSound = soundInfo.idleSound;
            soundData.startSound = soundInfo.wakeupSound;

            soundData.FourDMoveAudio.clip = soundInfo.moveSound;
        }
        else if(category == whatCategory.Game)
        {
            RacingGame soundData = RacingGame.instance;

            soundData.engineSound = soundInfo.gameEngineSound;
            soundData.startSound = soundInfo.startSound;
            soundData.accelSound = soundInfo.gameAccelSound;
            soundData.decelSound = soundInfo.gameDecelSound;
            soundData.finishSound = soundInfo.gameFinishSound;
            soundData.victorySound = soundInfo.gameVictorySound;
            soundData.gameoverSound = soundInfo.gameLoseSound;

            soundData.bgmAudio.clip = soundInfo.gameBgmSound;
        }
        else if(category == whatCategory.Racing)
        {
            RacingDrive soundData = RacingDrive.instance;

            soundData.startSound = soundInfo.startSound;
            soundData.engineSound = soundInfo.driveEngineSound;
            soundData.crashSound = soundInfo.driveCrashSound;

            soundData.bgmAudio.clip = soundInfo.driveBgmSound;
        }
        else
        {
            Debug.LogError("Please Select Category in RacingSoundControl");
        }
    }
}
