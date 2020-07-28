using UnityEngine;
using System.Collections;

using HedgehogTeam.EasyTouch;

public class LetterPreviewTouchManager : MonoBehaviour
{

    private Vector3 touchStartValue;

    public float pinchSensitivity;           // 확대 / 축소 민감도 값 (값이 작을 수록 민감함)
    public float pinchZoomMin;
    public float pinchZoomMax;

    public GameObject ColliderObject2D;
    //public GameObject pinchObject;

    public Camera camera2D;

    public static LetterPreviewTouchManager instance;

    void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnEnable()
    {
        // 터치 이벤트 등록
        EasyTouch.On_PinchIn += EasyTouch_On_PinchIn;
        EasyTouch.On_PinchOut += EasyTouch_On_PinchOut;
        EasyTouch.On_SimpleTap += EasyTouch_On_SimpleTap;

        //EasyTouch.On_DoubleTap += EasyTouch_On_DoubleTap;

        EasyTouch.On_TouchStart += EasyTouch_On_TouchStart;
        EasyTouch.On_TouchDown += EasyTouch_On_TouchDown;
    }

    void UnsubscribeEvent()
    {
        // 터치 이벤트 해제
        EasyTouch.On_PinchIn -= EasyTouch_On_PinchIn;
        EasyTouch.On_PinchOut -= EasyTouch_On_PinchOut;

        EasyTouch.On_SimpleTap -= EasyTouch_On_SimpleTap;
        //EasyTouch.On_DoubleTap -= EasyTouch_On_DoubleTap;

        EasyTouch.On_TouchStart -= EasyTouch_On_TouchStart;
        EasyTouch.On_TouchDown -= EasyTouch_On_TouchDown;
    }

    void OnDestroy()
    {
        UnsubscribeEvent();
    }

    private void EasyTouch_On_SimpleTap(Gesture gesture)
    {
        if (gesture.pickedObject == null)
        {
            return;
        }

        if (gesture.pickedObject.name.Equals("PreviewTextureClose") || gesture.pickedObject.name.Equals("PreviewVideoClose"))
        {

            gesture.pickedObject.transform.parent.gameObject.SetActive(false);

            LetterManager.Instance.isPreview = false;
            LetterManager.Instance.ShowHideMainModeling(true);
            LetterManager.Instance.OnOffOpenedLetterUI(true);
            RotateUI.회전.회전UI_보이기();

            LetterManager.Instance.targetObj.GetComponent<LetterInfo>().entireBody.gameObject.SetActive(true);

            if (LetterManager.Instance.albumAvailable)
            {
                LetterManager.Instance.StartSetLetterPaper();
            }

            if (gesture.pickedObject.name.Equals("PreviewVideoClose"))
            {
                gesture.pickedObject.transform.parent.GetComponent<MediaPlayerCtrl>().UnLoad();
                LetterComponentManager.instance.mediaPlayerCtrlInst.Stop();
                LetterComponentManager.instance.mediaPlayerCtrlInst.Play();
            }
        }

        if(gesture.pickedObject.name.Equals(LetterManager.Instance.textBox.name))
        {
            LetterManager.Instance.touchTextbox();
        }
    }


    private void EasyTouch_On_PinchIn(Gesture gesture)
    {
        if (gesture.pickedObject == null)
        {
            return;
        }

        float minValue = 0f;

        if (gesture.touchCount == 2 && (gesture.pickedObject.name.Equals("PreviewTexture") || gesture.pickedObject.name.Equals("PreviewVideo")))
        {
            minValue = (Time.deltaTime * (gesture.deltaPinch / pinchSensitivity));
            Vector3 objSize = gesture.pickedObject.transform.localScale;

            //Debug.LogWarning(string.Format("deltaTime={0}, deltaPinch={1}, sensitivity={2}, minValue={3}", Time.deltaTime, gesture.deltaPinch, pinchSensitivity, minValue));

            if ((objSize.x - minValue) > pinchZoomMin)
            {
                gesture.pickedObject.transform.localScale = new Vector3((objSize.x - minValue), (objSize.y - minValue), (objSize.z - minValue));
            }
        }
    }

    private void EasyTouch_On_PinchOut(Gesture gesture)
    {
        if (gesture.pickedObject == null)
        {
            return;
        }

        float maxValue = 0f;

        if (gesture.touchCount == 2 && (gesture.pickedObject.name.Equals("PreviewTexture") || gesture.pickedObject.name.Equals("PreviewVideo")))
        {
            maxValue = (Time.deltaTime * (gesture.deltaPinch / pinchSensitivity));
            Vector3 objSize = gesture.pickedObject.transform.localScale;

            if (objSize.x + maxValue < pinchZoomMax)
            {
                gesture.pickedObject.transform.localScale = new Vector3((objSize.x + maxValue), (objSize.y + maxValue), (objSize.z + maxValue));
            }
        }
    }

    private void EasyTouch_On_TouchStart(Gesture gesture)
    {
        try
        {
            if (gesture.pickedObject == null)
            {
                return;
            }

            if (gesture.pickedObject.name.Equals("PreviewTexture") || gesture.pickedObject.name.Equals("PreviewVideo"))
            {
                Vector3 touchPosition = gesture.GetTouchToWorldPoint(gesture.pickedObject.transform.position);
                touchStartValue = (touchPosition - gesture.pickedObject.transform.position);
            }
        }
        catch
        {

        }
    }

    private void EasyTouch_On_TouchDown(Gesture gesture)
    {
        if (gesture.pickedObject == null)
        {
            return;
        }

        //Debug.LogWarning(string.Format("pickedObject name = {0}, parent ={1}", gesture.pickedObject.name, gesture.pickedObject.transform.parent.name));


        if ((gesture.pickedObject.name.Equals("PreviewTexture") || gesture.pickedObject.name.Equals("PreviewVideo")) && gesture.touchCount == 1)
        {
            Vector3 touchPosition = gesture.GetTouchToWorldPoint(gesture.pickedObject.transform.position);
            gesture.pickedObject.transform.position = (touchPosition - touchStartValue);
        }
    }
}
