using UnityEngine;
using System.Collections;

public class ToastMessageUI : MonoBehaviour
{
    /// <summary>
    /// 토스트 UI 오브젝트
    /// </summary>
    public GameObject 토스트UI;

    /// <summary>
    /// 토스트텍스트 오브젝트
    /// </summary>
    public GameObject 토스트텍스트;

    public static ToastMessageUI 토스트;

    void Awake()
    {
        토스트 = this;
    }

    void Start()
    {
        토스트UI.SetActive(false);
    }

    void Update()
    {

    }

    public void 토스트메시지_출력(string 메시지)
    {
        if (토스트UI != null && 토스트텍스트 != null)
        {
            토스트텍스트.GetComponent<UILabel>().text = 메시지;
            메시지_보이기();
        }
    }

    private void 메시지_보이기()
    {
        CancelInvoke("메시지_숨기기");
        토스트UI.SetActive(true);
        Invoke("메시지_숨기기", 2.0f);
    }

    private void 메시지_숨기기()
    {
        토스트UI.SetActive(false);
    }

    //void OnGUI()
    //{
    //    if (GUI.Button(new Rect(0, 0, 180, 60), "토스트 메시지"))
    //    {
    //        토스트메시지_출력("자동 탐색모드가 설정 되었습니다.");
    //    }
    //}
}
