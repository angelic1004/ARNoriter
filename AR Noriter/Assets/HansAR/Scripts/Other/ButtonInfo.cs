using UnityEngine;
using System.Collections;

public class ButtonInfo : MonoBehaviour
{
    //권기영 : 다운 받을 에셋번들 이름(.hans 제외)
    public string assetBundleName;

    //권기영 : 로드 할 씬이름
    public string loadSceneName;

    //권기영 : 동요, 동화 사용여부 확인
    public bool story;

    public GlobalDataManager.SceneState sceneState;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
