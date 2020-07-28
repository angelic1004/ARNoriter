using UnityEngine;
using System.Collections;

public class DriveSoundInfo : MonoBehaviour {

    public AudioSource bg_audio_Source;
    public AudioSource car_audio_Source;
    public AudioSource brake_audio_Source;
    public AudioSource mission_audio_Source;
    public AudioSource collision_audio_Source;
    public AudioSource safetyBelt_audio_Source;

    public AudioClip bg_Clip;
    public AudioClip ready_Clip;
    public AudioClip mission_Clip;
    public AudioClip[] success_Clip;
    public AudioClip[] fail_Clip;
    public AudioClip[] accel_Clip;
    public AudioClip[] brake_Clip;
    public AudioClip[] collision_Clip;
    public AudioClip safetyBelt_Clip;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
