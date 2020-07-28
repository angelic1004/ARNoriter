using UnityEngine;
using System.Collections;

public class ScriptOrderManager : MonoBehaviour
{
    public GameObject scriptDisableList;
    public GameObject[] scriptObjList;


    private void Awake()
    {
        InitScriptOrderManager();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void InitScriptOrderManager()
    {
        scriptDisableList.SetActive(false);

        for (int i = 0; i < scriptObjList.Length; i++)
        {
            scriptObjList[i].SetActive(false);
        }

        StartCoroutine(ScriptCheck());
    }

    private IEnumerator ScriptCheck()
    {
        while (TargetManager.타깃메니저 == null)
        {
            yield return new WaitForSeconds(0.1f);
        }

        TargetManager.EnableTracking = false;

        scriptDisableList.SetActive(true);
        EnableScriptObjects();
    }

    /// <summary>
    /// Script 오브젝트들을 켜줌
    /// </summary>
    private void EnableScriptObjects()
    {
        for (int i = 0; i < scriptObjList.Length; i++)
        {
            scriptObjList[i].SetActive(true);
        }
    }
}
