using UnityEngine;
using System.Collections;

public class DrivingUI : MonoBehaviour {

    public GameObject safetyBelt_UI;
    public UISprite safetyBelt_BG;
    public GameObject nguiCam;
    public GameObject logoCam;

    private UISpriteAnimation safety_Animtion;
    private bool safetyBelt_State = false;

    public static DrivingUI drivingUI;

    void Awake()
    {
        drivingUI = this;
    }

    void Start()
    {
        safety_Animtion = safetyBelt_BG.GetComponent<UISpriteAnimation>();
        safetyBelt_UI.SetActive(true);
        Driving_Sound.driving_Sound.DrivingSoundPlay(Driving_Sound.SoundType.Ready);
    }


    void FixedUpdate()
    {
        if (safetyBelt_State && !safety_Animtion.isPlaying)
        {
            safetyBelt_State = false;
            //safetyBelt_BG.GetComponent<BoxCollider2D>().enabled = false;
            StopCoroutine(SafetyBeltClose());
            StartCoroutine(SafetyBeltClose());
        }
    }

    public void SafetyBeltWear(GameObject obj)
    {
        safety_Animtion.enabled = true;
        safetyBelt_State = true;
        obj.GetComponent<TweenAlpha>().enabled = true;
        obj.GetComponent<BoxCollider2D>().enabled = false;
        Driving_Sound.driving_Sound.DrivingSoundPlay(Driving_Sound.SoundType.Safety);
    }

    public void DrivingScreenShotUI()
    {
        //nguiCam.SetActive(!nguiCam.activeSelf);
        logoCam.SetActive(!logoCam.activeSelf);
    }

    private IEnumerator SafetyBeltClose()
    {
        Driving_Sound.driving_Sound.DrivingSoundPlay(Driving_Sound.SoundType.Bg_Sound);

        while (true)
        {
            safetyBelt_BG.GetComponent<UISprite>().alpha -= Time.deltaTime * 2.0f;

            if (safetyBelt_BG.GetComponent<UISprite>().alpha <= 0)
            {
                safetyBelt_UI.SetActive(false);
                Car_Ctrl.car_ctrl.event_Text.text = "[b]자 ~ 이제 출발해 볼까요?[/b]";
                Car_Ctrl.car_ctrl.event_Text.GetComponent<TweenAlpha>().enabled = true;
                yield break;
            }
            yield return new WaitForEndOfFrame();
        }
    }
}
