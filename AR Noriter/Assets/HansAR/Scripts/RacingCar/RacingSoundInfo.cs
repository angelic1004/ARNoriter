using UnityEngine;
using System.Collections;

public class RacingSoundInfo : MonoBehaviour {

    //Common
    public AudioClip startSound;

    //Racing 4D Observe
    [Space]
    public AudioClip idleSound;
    public AudioClip moveSound;
    public AudioClip wakeupSound;

    //Racing 4D Game
    [Space]
    public AudioClip gameBgmSound;
    public AudioClip gameAccelSound;
    public AudioClip gameDecelSound;
    public AudioClip gameFinishSound;
    public AudioClip gameVictorySound;
    public AudioClip gameLoseSound;
    public AudioClip[] gameEngineSound;

    //Racing Drive
    [Space]
    public AudioClip driveBgmSound;
    public AudioClip[] driveEngineSound;
    public AudioClip[] driveCrashSound;
}
