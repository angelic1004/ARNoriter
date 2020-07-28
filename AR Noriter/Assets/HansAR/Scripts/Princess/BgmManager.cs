using UnityEngine;
using System.Collections;

public class BgmManager : MonoBehaviour
{
    public static BgmManager Instance;

    void Awake()
    {
        Instance = this;
    }

    public void BgmStart(GameObject obj)
    {
        AudioClip bgm = obj.GetComponent<BgmInfo>().bgm;
        DanceAudioManager.getInstance.PlayBackgroundSound(bgm, true);
    }

    public IEnumerator StartBgm()
    {
        GameObject targetObj = null;

        while (targetObj == null)
        {
            targetObj = TargetManager.타깃메니저.에셋번들복제컨텐츠[0];

            yield return new WaitForFixedUpdate();
        }

        BgmStart(targetObj);
    }
}