using UnityEngine;
using System.Collections;

public class TweenManager : MonoBehaviour
{
    //public Vector3 scaleFrom;
    //public Vector3 scaleTo;
    public bool playing = false;

    public bool slideMenuOpen = false;

    public GameObject shadowUi;

    public enum TweenMergeType
    {
        TweenAlpha,
        TweenScale,
        TweenPosition,
        TweenRotation,
        TweenColor,
        None,
    }

    /// <summary>
    /// 트윈이벤트
    /// </summary>
    private TweenAlpha mTweenAlpha;
    private TweenScale mTweenScale;
    private TweenPosition mTweenPosition;
    private TweenRotation mTweenRotation;
    private TweenColor mTweenColor;

    private Coroutine ScaleCoroutine;
    private Coroutine TimerCoroutine;
    private Coroutine SlideCoroutine;

    public AnimationCurve scaleAnimationCurve = new AnimationCurve(
            new Keyframe(0f, 0f, 0f, 1f),
            new Keyframe(0.7f, 1.2f, 1f, 1f),
            new Keyframe(1f, 1f, 1f, 0f));

    public AnimationCurve normalAnimationCurve = new AnimationCurve(
            new Keyframe(0f, 0f, 0f, 1f),
            new Keyframe(1f, 1f, 1f, 0f));

    public AnimationCurve fingerAnimationCurve = new AnimationCurve(
    new Keyframe(0f, 0f, 0f, 1f),
    new Keyframe(0.2f, 1.0f, 0f, 0f),
    new Keyframe(0.35f, 0.2f, -1.0f, -1.0f),
    new Keyframe(0.7f, 0f, 0f, 0f),
    new Keyframe(1f, 0f, 0f, 1f));

    public AnimationCurve subTweenScaleCurve;

    public AnimationCurve[] animationCurveList;
    public string[] spriteAniNameList;

    public GameObject screenShotBtn;
    public GameObject recordStartBtn;
    public GameObject recordStopBtn;
    public GameObject cameraChangeBtn;
    public GameObject previewBtn;

    //TweenScale tt;

    //EventDelegate ed;
    //EventDelegate.Parameter param;



    public static TweenManager tween_Manager;

    void Awake()
    {
        tween_Manager = this;

    }

    void Start()
    {
     //   ed = new EventDelegate();
     //   ed.methodName = "SubButtonTweenScale";
     //   ed.target = this;

      //  param = new EventDelegate.Parameter();
    }

    public void TweenAllDestroy(GameObject obj)
    {
        DestroyImmediate(obj.GetComponent<TweenAlpha>());
        DestroyImmediate(obj.GetComponent<TweenScale>());
        DestroyImmediate(obj.GetComponent<TweenPosition>());
        DestroyImmediate(obj.GetComponent<TweenRotation>());
        DestroyImmediate(obj.GetComponent<TweenColor>());
    }

    public void TweenAlpha(GameObject obj)
    {
        TweenSetting(obj, TweenMergeType.TweenAlpha);
    }
    public void TweenScale(GameObject obj)
    {
        TweenSetting(obj, TweenMergeType.TweenScale);
    }
    public void TweenPosition(GameObject obj)
    {
        TweenSetting(obj, TweenMergeType.TweenPosition);
    }
    public void TweenRotation(GameObject obj)
    {
        TweenSetting(obj, TweenMergeType.TweenRotation);
    }
    public void TweenColor(GameObject obj)
    {
        TweenSetting(obj, TweenMergeType.TweenColor);
    }

    public void TweenColor_Reverse(GameObject obj)
    {
        ReverseTweenSetting(obj, TweenMergeType.TweenColor);
    }

    public void AddTweenAlpha(GameObject obj)
    {
        if (obj.GetComponent<TweenAlpha>() == null)
        {
            obj.AddComponent<TweenAlpha>();
            obj.GetComponent<TweenAlpha>().enabled = false;
        }

        obj.GetComponent<TweenAlpha>().from = 0;
        obj.GetComponent<TweenAlpha>().to = 1;
        obj.GetComponent<TweenAlpha>().style = UITweener.Style.Once;
        obj.GetComponent<TweenAlpha>().animationCurve = normalAnimationCurve;
        obj.GetComponent<TweenAlpha>().duration = 1.0f;

        //    TweenAlpha(obj);
    }

    public void AddTweenAlpha(GameObject obj, float fromFloat, float toFloat)
    {
        if (obj.GetComponent<TweenAlpha>() == null)
        {
            obj.AddComponent<TweenAlpha>();
            obj.GetComponent<TweenAlpha>().enabled = false;
        }
       
        obj.GetComponent<TweenAlpha>().from = fromFloat;
        obj.GetComponent<TweenAlpha>().to = toFloat;
        obj.GetComponent<TweenAlpha>().style = UITweener.Style.Once;
        obj.GetComponent<TweenAlpha>().animationCurve = normalAnimationCurve;
        obj.GetComponent<TweenAlpha>().duration = 1.0f;
       
        //    TweenAlpha(obj);
    }

    public void AddTweenAlpha(GameObject obj, float fromFloat, float toFloat, float duratonTime)
    {
        if (obj.GetComponent<TweenAlpha>() == null)
        {
            obj.AddComponent<TweenAlpha>();
            obj.GetComponent<TweenAlpha>().enabled = false;
        }

        obj.GetComponent<TweenAlpha>().from = fromFloat;
        obj.GetComponent<TweenAlpha>().to = toFloat;
        obj.GetComponent<TweenAlpha>().style = UITweener.Style.Once;
        obj.GetComponent<TweenAlpha>().animationCurve = normalAnimationCurve;
        obj.GetComponent<TweenAlpha>().duration = duratonTime;

        //    TweenAlpha(obj);
    }

    public void AddTweenAlpha(GameObject obj, float fromFloat, float toFloat, float duratonTime, UITweener.Style style)
    {
        if (obj.GetComponent<TweenAlpha>() == null)
        {
            obj.AddComponent<TweenAlpha>();
            obj.GetComponent<TweenAlpha>().enabled = false;
        }

        obj.GetComponent<TweenAlpha>().from = fromFloat;
        obj.GetComponent<TweenAlpha>().to = toFloat;
        obj.GetComponent<TweenAlpha>().style = style;
        obj.GetComponent<TweenAlpha>().animationCurve = normalAnimationCurve;
        obj.GetComponent<TweenAlpha>().duration = duratonTime;

        //   TweenAlpha(obj);
    }

    public void AddTweenAlpha(GameObject obj, float fromFloat, float toFloat, float duratonTime, UITweener.Style style, AnimationCurve anicuv)
    {
        if (obj.GetComponent<TweenAlpha>() == null)
        {
            obj.AddComponent<TweenAlpha>();
            obj.GetComponent<TweenAlpha>().enabled = false;
        }

        obj.GetComponent<TweenAlpha>().from = fromFloat;
        obj.GetComponent<TweenAlpha>().to = toFloat;
        obj.GetComponent<TweenAlpha>().style = style;
        obj.GetComponent<TweenAlpha>().animationCurve = anicuv;
        obj.GetComponent<TweenAlpha>().duration = duratonTime;

        //    TweenAlpha(obj);
    }

    public void AddTweenAlpha(GameObject obj, float fromFloat, float toFloat, float duratonTime, UITweener.Style style, AnimationCurve anicuv, int group)
    {
        if (obj.GetComponent<TweenAlpha>() == null)
        {
            obj.AddComponent<TweenAlpha>();
            obj.GetComponent<TweenAlpha>().enabled = false;
        }

        obj.GetComponent<TweenAlpha>().from = fromFloat;
        obj.GetComponent<TweenAlpha>().to = toFloat;
        obj.GetComponent<TweenAlpha>().style = style;
        obj.GetComponent<TweenAlpha>().animationCurve = anicuv;
        obj.GetComponent<TweenAlpha>().duration = duratonTime;
        obj.GetComponent<TweenAlpha>().tweenGroup = group;
        //    TweenAlpha(obj);
    }

    public void AddTweenScale(GameObject obj, Vector3 fromVector3, Vector3 toVector3)
    {
        if (obj.GetComponent<TweenScale>() == null)
        {
            obj.AddComponent<TweenScale>();
            obj.GetComponent<TweenScale>().enabled = false;
        }

        obj.GetComponent<TweenScale>().from = fromVector3;
        obj.GetComponent<TweenScale>().to = toVector3;
        obj.GetComponent<TweenScale>().style = UITweener.Style.Once;
        obj.GetComponent<TweenScale>().animationCurve = normalAnimationCurve;
        obj.GetComponent<TweenScale>().duration = 1.0f;

        //   TweenScale(obj);
    }

    public void AddTweenScale(GameObject obj, Vector3 fromVector3, Vector3 toVector3, float duratonTime)
    {
        if (obj.GetComponent<TweenScale>() == null)
        {
            obj.AddComponent<TweenScale>();
            obj.GetComponent<TweenScale>().enabled = false;
        }

        obj.GetComponent<TweenScale>().from = fromVector3;
        obj.GetComponent<TweenScale>().to = toVector3;
        obj.GetComponent<TweenScale>().style = UITweener.Style.Once;
        obj.GetComponent<TweenScale>().animationCurve = normalAnimationCurve;
        obj.GetComponent<TweenScale>().duration = duratonTime;

        //   TweenScale(obj);
    }

    public void AddTweenScale(GameObject obj, Vector3 fromVector3, Vector3 toVector3, float duratonTime, UITweener.Style style)
    {
        if (obj.GetComponent<TweenScale>() == null)
        {
            obj.AddComponent<TweenScale>();
            obj.GetComponent<TweenScale>().enabled = false;
        }

        obj.GetComponent<TweenScale>().from = fromVector3;
        obj.GetComponent<TweenScale>().to = toVector3;
        obj.GetComponent<TweenScale>().style = style;
        obj.GetComponent<TweenScale>().animationCurve = normalAnimationCurve;
        obj.GetComponent<TweenScale>().duration = duratonTime;

        //   TweenScale(obj);
    }

    public void AddTweenScale(GameObject obj, Vector3 fromVector3, Vector3 toVector3, float duratonTime, UITweener.Style style, AnimationCurve anicuv)
    {
        if (obj.GetComponent<TweenScale>() == null)
        {
            obj.AddComponent<TweenScale>();
            obj.GetComponent<TweenScale>().enabled = false;
        }

        obj.GetComponent<TweenScale>().from = fromVector3;
        obj.GetComponent<TweenScale>().to = toVector3;
        obj.GetComponent<TweenScale>().style = style;
        obj.GetComponent<TweenScale>().animationCurve = anicuv;
        obj.GetComponent<TweenScale>().duration = duratonTime;

        //   TweenScale(obj);
    }

    public void AddTweenScale(GameObject obj, Vector3 fromVector3, Vector3 toVector3, float duratonTime, UITweener.Style style, AnimationCurve anicuv, int group)
    {
        if (obj.GetComponent<TweenScale>() == null)
        {
            obj.AddComponent<TweenScale>();
            obj.GetComponent<TweenScale>().enabled = false;
        }

        obj.GetComponent<TweenScale>().from = fromVector3;
        obj.GetComponent<TweenScale>().to = toVector3;
        obj.GetComponent<TweenScale>().style = style;
        obj.GetComponent<TweenScale>().animationCurve = anicuv;
        obj.GetComponent<TweenScale>().duration = duratonTime;
        obj.GetComponent<TweenScale>().tweenGroup = group;
        //   TweenScale(obj);
    }


    public void AddTweenPosition(GameObject obj, Vector3 fromVector3, Vector3 toVector3)
    {
        if (obj.GetComponent<TweenPosition>() == null)
        {
            obj.AddComponent<TweenPosition>();
            obj.GetComponent<TweenPosition>().enabled = false;
        }

        obj.GetComponent<TweenPosition>().from = fromVector3;
        obj.GetComponent<TweenPosition>().to = toVector3;
        obj.GetComponent<TweenPosition>().style = UITweener.Style.Once;
        obj.GetComponent<TweenPosition>().animationCurve = normalAnimationCurve;
        obj.GetComponent<TweenPosition>().duration = 1.0f;

        //   TweenPosition(obj);
    }

    public void AddTweenPosition(GameObject obj, Vector3 fromVector3, Vector3 toVector3, float duratonTime)
    {
        if (obj.GetComponent<TweenPosition>() == null)
        {
            obj.AddComponent<TweenPosition>();
            obj.GetComponent<TweenPosition>().enabled = false;
        }

        obj.GetComponent<TweenPosition>().from = fromVector3;
        obj.GetComponent<TweenPosition>().to = toVector3;
        obj.GetComponent<TweenPosition>().style = UITweener.Style.Once;
        obj.GetComponent<TweenPosition>().animationCurve = normalAnimationCurve;
        obj.GetComponent<TweenPosition>().duration = duratonTime;

        //  TweenPosition(obj);
    }

    public void AddTweenPosition(GameObject obj, Vector3 fromVector3, Vector3 toVector3, float duratonTime, UITweener.Style style)
    {
        if (obj.GetComponent<TweenPosition>() == null)
        {
            obj.AddComponent<TweenPosition>();
            obj.GetComponent<TweenPosition>().enabled = false;
        }

        obj.GetComponent<TweenPosition>().from = fromVector3;
        obj.GetComponent<TweenPosition>().to = toVector3;
        obj.GetComponent<TweenPosition>().style = style;
        obj.GetComponent<TweenPosition>().animationCurve = normalAnimationCurve;
        obj.GetComponent<TweenPosition>().duration = duratonTime;

        //   TweenPosition(obj);
    }

    public void AddTweenPosition(GameObject obj, Vector3 fromVector3, Vector3 toVector3, float duratonTime, UITweener.Style style, AnimationCurve anicuv)
    {
        if (obj.GetComponent<TweenPosition>() == null)
        {
            obj.AddComponent<TweenPosition>();
            obj.GetComponent<TweenPosition>().enabled = false;
        }

        obj.GetComponent<TweenPosition>().from = fromVector3;
        obj.GetComponent<TweenPosition>().to = toVector3;
        obj.GetComponent<TweenPosition>().style = style;
        obj.GetComponent<TweenPosition>().animationCurve = anicuv;
        obj.GetComponent<TweenPosition>().duration = duratonTime;

        //   TweenPosition(obj);
    }

    public void AddTweenPosition(GameObject obj, Vector3 fromVector3, Vector3 toVector3, float duratonTime, UITweener.Style style, AnimationCurve anicuv, int group)
    {
        if (obj.GetComponent<TweenPosition>() == null)
        {
            obj.AddComponent<TweenPosition>();
            obj.GetComponent<TweenPosition>().enabled = false;
        }

        obj.GetComponent<TweenPosition>().from = fromVector3;
        obj.GetComponent<TweenPosition>().to = toVector3;
        obj.GetComponent<TweenPosition>().style = style;
        obj.GetComponent<TweenPosition>().animationCurve = anicuv;
        obj.GetComponent<TweenPosition>().duration = duratonTime;
        obj.GetComponent<TweenPosition>().tweenGroup = group;

        //   TweenPosition(obj);
    }

    public void AddTweenRotation(GameObject obj, Vector3 fromVector3, Vector3 toVector3)
    {
        if (obj.GetComponent<TweenRotation>() == null)
        {
            obj.AddComponent<TweenRotation>();
            obj.GetComponent<TweenRotation>().enabled = false;
        }

        obj.GetComponent<TweenRotation>().from = fromVector3;
        obj.GetComponent<TweenRotation>().to = toVector3;
        obj.GetComponent<TweenRotation>().style = UITweener.Style.Once;
        obj.GetComponent<TweenRotation>().animationCurve = normalAnimationCurve;
        obj.GetComponent<TweenRotation>().duration = 1.0f;

        //   TweenRotation(obj);
    }

    public void AddTweenRotation(GameObject obj, Vector3 fromVector3, Vector3 toVector3, float duratonTime)
    {
        if (obj.GetComponent<TweenRotation>() == null)
        {
            obj.AddComponent<TweenRotation>();
            obj.GetComponent<TweenRotation>().enabled = false;
        }

        obj.GetComponent<TweenRotation>().from = fromVector3;
        obj.GetComponent<TweenRotation>().to = toVector3;
        obj.GetComponent<TweenRotation>().style = UITweener.Style.Once;
        obj.GetComponent<TweenRotation>().animationCurve = normalAnimationCurve;
        obj.GetComponent<TweenRotation>().duration = duratonTime;

        //    TweenRotation(obj);
    }

    public void AddTweenRotation(GameObject obj, Vector3 fromVector3, Vector3 toVector3, float duratonTime, UITweener.Style style)
    {
        if (obj.GetComponent<TweenRotation>() == null)
        {
            obj.AddComponent<TweenRotation>();
            obj.GetComponent<TweenRotation>().enabled = false;
        }

        obj.GetComponent<TweenRotation>().from = fromVector3;
        obj.GetComponent<TweenRotation>().to = toVector3;
        obj.GetComponent<TweenRotation>().style = style;
        obj.GetComponent<TweenRotation>().animationCurve = normalAnimationCurve;
        obj.GetComponent<TweenRotation>().duration = duratonTime;

        //   TweenRotation(obj);
    }

    public void AddTweenRotation(GameObject obj, Vector3 fromVector3, Vector3 toVector3, float duratonTime, UITweener.Style style, AnimationCurve anicuv)
    {
        if (obj.GetComponent<TweenRotation>() == null)
        {
            obj.AddComponent<TweenRotation>();
            obj.GetComponent<TweenRotation>().enabled = false;
        }

        obj.GetComponent<TweenRotation>().from = fromVector3;
        obj.GetComponent<TweenRotation>().to = toVector3;
        obj.GetComponent<TweenRotation>().style = style;
        obj.GetComponent<TweenRotation>().animationCurve = anicuv;
        obj.GetComponent<TweenRotation>().duration = duratonTime;

        //     TweenRotation(obj);
    }

    public void AddTweenRotation(GameObject obj, Vector3 fromVector3, Vector3 toVector3, float duratonTime, UITweener.Style style, AnimationCurve anicuv, int group)
    {
        if (obj.GetComponent<TweenRotation>() == null)
        {
            obj.AddComponent<TweenRotation>();
            obj.GetComponent<TweenRotation>().enabled = false;
        }

        obj.GetComponent<TweenRotation>().from = fromVector3;
        obj.GetComponent<TweenRotation>().to = toVector3;
        obj.GetComponent<TweenRotation>().style = style;
        obj.GetComponent<TweenRotation>().animationCurve = anicuv;
        obj.GetComponent<TweenRotation>().duration = duratonTime;
        obj.GetComponent<TweenRotation>().tweenGroup = group;

        //     TweenRotation(obj);
    }

    public void AddTweenColor(GameObject obj, Color32 fromColor, Color32 toColor)
    {
        if (obj.GetComponent<TweenColor>() == null)
        {
            obj.AddComponent<TweenColor>();
            obj.GetComponent<TweenColor>().enabled = false;
        }

        obj.GetComponent<TweenColor>().from = fromColor;
        obj.GetComponent<TweenColor>().to = toColor;
        obj.GetComponent<TweenColor>().style = UITweener.Style.Once;
        obj.GetComponent<TweenColor>().animationCurve = normalAnimationCurve;
        obj.GetComponent<TweenColor>().duration = 1.0f;


        //   TweenColor(obj);
    }

    public void AddTweenColor(GameObject obj, Color32 fromColor, Color32 toColor, float durationTime)
    {
        if (obj.GetComponent<TweenColor>() == null)
        {
            obj.AddComponent<TweenColor>();
            obj.GetComponent<TweenColor>().enabled = false;
        }

        obj.GetComponent<TweenColor>().from = fromColor;
        obj.GetComponent<TweenColor>().to = toColor;
        obj.GetComponent<TweenColor>().style = UITweener.Style.Once;
        obj.GetComponent<TweenColor>().animationCurve = normalAnimationCurve;
        obj.GetComponent<TweenColor>().duration = durationTime;

        //   TweenColor(obj);
    }

    public void AddTweenColor(GameObject obj, Color32 fromColor, Color32 toColor, float durationTime, UITweener.Style style)
    {
        if (obj.GetComponent<TweenColor>() == null)
        {
            obj.AddComponent<TweenColor>();
            obj.GetComponent<TweenColor>().enabled = false;
        }

        obj.GetComponent<TweenColor>().from = fromColor;
        obj.GetComponent<TweenColor>().to = toColor;
        obj.GetComponent<TweenColor>().style = style;
        obj.GetComponent<TweenColor>().animationCurve = normalAnimationCurve;
        obj.GetComponent<TweenColor>().duration = durationTime;

        //  TweenColor(obj);
    }

    public void AddTweenColor(GameObject obj, Color32 fromColor, Color32 toColor, float durationTime, UITweener.Style style, AnimationCurve anicuv)
    {
        if (obj.GetComponent<TweenColor>() == null)
        {
            obj.AddComponent<TweenColor>();
            obj.GetComponent<TweenColor>().enabled = false;
        }

        obj.GetComponent<TweenColor>().from = fromColor;
        obj.GetComponent<TweenColor>().to = toColor;
        obj.GetComponent<TweenColor>().style = style;
        obj.GetComponent<TweenColor>().animationCurve = anicuv;
        obj.GetComponent<TweenColor>().duration = durationTime;

        // TweenColor(obj);
    }

    public void AddTweenColor(GameObject obj, Color32 fromColor, Color32 toColor, float durationTime, UITweener.Style style, AnimationCurve anicuv, int group)
    {
        if (obj.GetComponent<TweenColor>() == null)
        {
            obj.AddComponent<TweenColor>();
            obj.GetComponent<TweenColor>().enabled = false;
        }

        obj.GetComponent<TweenColor>().from = fromColor;
        obj.GetComponent<TweenColor>().to = toColor;
        obj.GetComponent<TweenColor>().style = style;
        obj.GetComponent<TweenColor>().animationCurve = anicuv;
        obj.GetComponent<TweenColor>().duration = durationTime;
        obj.GetComponent<TweenColor>().tweenGroup = group;

        // TweenColor(obj);
    }


    /// <summary>
    /// 트윈 컨트롤
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="tween"></param>
    private void TweenSetting(GameObject obj, TweenMergeType _type)
    {
        resetToBeginning(obj, _type);

        if (!obj.activeSelf)
        {
            obj.SetActive(true);
        }
        switch (_type)
        {
            case TweenMergeType.TweenScale:
                if (mTweenScale == null)
                {
                    mTweenScale = obj.GetComponent<TweenScale>();
                    playTweenForward();
                }
                break;
            case TweenMergeType.TweenRotation:
                if (mTweenRotation == null)
                {
                    mTweenRotation = obj.GetComponent<TweenRotation>();
                    playTweenForward();
                }
                break;
            case TweenMergeType.TweenPosition:
                if (mTweenPosition == null)
                {
                    mTweenPosition = obj.GetComponent<TweenPosition>();
                    playTweenForward();
                }
                break;
            case TweenMergeType.TweenAlpha:
                if (mTweenAlpha == null)
                {
                    mTweenAlpha = obj.GetComponent<TweenAlpha>();
                    playTweenForward();
                }
                break;
            case TweenMergeType.TweenColor:
                if (mTweenColor == null)
                {
                    mTweenColor = obj.GetComponent<TweenColor>();
                    playTweenForward();
                }
                break;
            default:
                Debug.Log("Tween Type Error");
                return;
        }
    }


    /// <summary>
    /// Tween이벤트 실행
    /// </summary>
    private void playTweenForward()
    {
        if (mTweenAlpha != null)
        {
            mTweenAlpha.PlayForward();
        }

        if (mTweenScale != null)
        {
            mTweenScale.PlayForward();
        }

        if (mTweenPosition != null)
        {
            mTweenPosition.PlayForward();
        }

        if (mTweenRotation != null)
        {
            mTweenRotation.PlayForward();
        }

        if (mTweenColor != null)
        {
            mTweenColor.PlayForward();
        }
    }

    /// <summary>
    /// 트윈 컨트롤
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="tween"></param>
    private void ReverseTweenSetting(GameObject obj, TweenMergeType _type)
    {
        resetToBeginning(obj, _type);

        if (!obj.activeSelf)
        {
            obj.SetActive(true);
        }
        switch (_type)
        {
            case TweenMergeType.TweenScale:
                if (mTweenScale == null)
                {
                    mTweenScale = obj.GetComponent<TweenScale>();
                    playTweenReverse();
                }
                break;
            case TweenMergeType.TweenRotation:
                if (mTweenRotation == null)
                {
                    mTweenRotation = obj.GetComponent<TweenRotation>();
                    playTweenReverse();
                }
                break;
            case TweenMergeType.TweenPosition:
                if (mTweenPosition == null)
                {
                    mTweenPosition = obj.GetComponent<TweenPosition>();
                    playTweenReverse();
                }
                break;
            case TweenMergeType.TweenAlpha:
                if (mTweenAlpha == null)
                {
                    mTweenAlpha = obj.GetComponent<TweenAlpha>();
                    playTweenReverse();
                }
                break;
            case TweenMergeType.TweenColor:
                if (mTweenColor == null)
                {
                    mTweenColor = obj.GetComponent<TweenColor>();
                    playTweenReverse();
                }
                break;
            default:
                Debug.Log("Tween Type Error");
                return;
        }
    }


    /// <summary>
    /// Tween이벤트 실행
    /// </summary>
    private void playTweenReverse()
    {
        if (mTweenAlpha != null)
        {
            mTweenAlpha.PlayReverse();
        }

        if (mTweenScale != null)
        {
            mTweenScale.PlayReverse();
        }

        if (mTweenPosition != null)
        {
            mTweenPosition.PlayReverse();
        }

        if (mTweenRotation != null)
        {
            mTweenRotation.PlayReverse();
        }

        if (mTweenColor != null)
        {
            mTweenColor.PlayReverse();
        }

    }

    /// <summary>
    /// Tween이벤트 리셋
    /// </summary>
    public void resetToBeginning(GameObject obj, TweenMergeType _type)
    {
        switch (_type)
        {
            case TweenMergeType.TweenScale:
                if (mTweenScale != null)
                {
                    mTweenScale = obj.GetComponent<TweenScale>();
                    mTweenScale.ResetToBeginning();
                    mTweenScale = null;
                }
                break;
            case TweenMergeType.TweenRotation:
                if (mTweenRotation != null)
                {
                    mTweenRotation = obj.GetComponent<TweenRotation>();
                    mTweenRotation.ResetToBeginning();
                    mTweenRotation = null;
                }
                break;
            case TweenMergeType.TweenPosition:
                if (mTweenPosition != null)
                {
                    mTweenPosition = obj.GetComponent<TweenPosition>();
                    mTweenPosition.ResetToBeginning();
                    mTweenPosition = null;
                }
                break;
            case TweenMergeType.TweenAlpha:
                if (mTweenAlpha != null)
                {
                    mTweenAlpha = obj.GetComponent<TweenAlpha>();
                    mTweenAlpha.ResetToBeginning();
                    mTweenAlpha = null;
                }
                break;
            case TweenMergeType.TweenColor:
                if (mTweenColor != null)
                {
                    mTweenColor = obj.GetComponent<TweenColor>();
                    mTweenColor.ResetToBeginning();
                    mTweenColor = null;
                }
                break;
            default:
                Debug.Log("Tween Type Error");
                return;
        }
    }


    public void MainButtonTweenScale(GameObject obj)//GameObject obj)
    {
        if (!playing)
        {
            StartCoroutine(MainButtonTweenScaleStart(obj));
        }
    }

    public void MainBtnSlide(GameObject obj)
    {
        CamSubBtnAlphaSet(true);
        SlideCoroutineStart(obj);
    }

    public void MainBtnRecordingEnd(GameObject obj) 
    {
        StartCoroutine(MainButtonRecordingEndTween(obj));
    }

    public void SubButtonTweenScale(GameObject obj)
    {
        int subObjCount = obj.transform.childCount;

        StartCoroutine(SubButtonTweenScaleStart(obj, subObjCount, obj.GetComponent<BtnTweenInfo>().nextTime));
    }


    public void StartTimerCoroutine(bool state)
    {
        StopTimerCoroutine();
        TimerCoroutine = StartCoroutine(UiTimer(state));
    }

    public void RotateBtnChangeSprite(GameObject obj)
    {
        if (obj.GetComponent<UIButton>() != null)
        {
            obj.GetComponent<UIButton>().tweenTarget = null;
        }

        if (obj.GetComponent<UISprite>().spriteName == "rotatebtn_next")
        {
            obj.GetComponent<UISprite>().spriteName = "rotatebtn_prev";
        }
        else
        {
            obj.GetComponent<UISprite>().spriteName = "rotatebtn_next";
        }
    }


    private void StopScaleCoroutine()
    {
        if (ScaleCoroutine != null)
        {
            StopCoroutine(ScaleCoroutine);
            ScaleCoroutine = null;
        }
    }

    private void StopTimerCoroutine()
    {
        if (TimerCoroutine != null)
        {
            StopCoroutine(TimerCoroutine);
            TimerCoroutine = null;
        }
    }

    private void SlideCoroutineStart(GameObject obj)
    {
        SlideCoroutineStop();
        SlideCoroutine = StartCoroutine(MainButtonTweenSlideStart(obj));
    }

    private void SlideCoroutineStop()
    {
        if(SlideCoroutine != null)
        {
            StopCoroutine(SlideCoroutine);
            SlideCoroutine = null;
        }
    }

    private void CamSubBtnAlphaSet(bool state)
    {
        if (screenShotBtn == null)
        {
            Debug.Log("screenShotBtn 이 null 입니다.");
            return;
        }

        screenShotBtn.GetComponent<UIButton>().tweenTarget = null;

        if (state)
        {

            screenShotBtn.GetComponent<UIWidget>().alpha = 1;
        }
        else
        {
            screenShotBtn.GetComponent<UIWidget>().alpha = 0;
        }
    }


    private IEnumerator SpriteAniStart(GameObject obj,float endTime)
    {
        float time = 0;
        int spriteIndex = 0;

        while (true)
        {
            yield return new WaitForEndOfFrame();

            if (spriteAniNameList.Length > 0)
            {
                if (spriteIndex >= spriteAniNameList.Length)
                {
                    spriteIndex = 0;
                }

                obj.GetComponent<UISprite>().spriteName = spriteAniNameList[spriteIndex];

                time += Time.deltaTime;

                if (time >= endTime)
                {
                    obj.GetComponent<UISprite>().spriteName = spriteAniNameList[0];

                    yield break;
                }

                spriteIndex++;
            }
            else
            {
                yield break;
            }
        }
    }

    private IEnumerator MainButtonRecordingEndTween(GameObject obj)
    {
        float tweenTime = 0.2f;

        StartCoroutine(SpriteAniStart(obj, tweenTime));

        TweenAllDestroy(obj);
        AddTweenPosition(obj.GetComponent<BtnTweenInfo>().moveObj,
                         obj.GetComponent<BtnTweenInfo>().moveObj.transform.localPosition,
                         obj.GetComponent<BtnTweenInfo>().fourEndPos.localPosition,
                         tweenTime);

        TweenPosition(obj.GetComponent<BtnTweenInfo>().moveObj);

        yield return new WaitForSeconds(tweenTime);

        yield break;
    }


    private IEnumerator MainButtonTweenSlideStart(GameObject obj)
    {
        float tweenTime = 0.5f;

        if (obj.GetComponent<BtnTweenInfo>() != null)
        {
            tweenTime = obj.GetComponent<BtnTweenInfo>().slideTime;
        }
        
        if (obj.GetComponent<BtnTweenInfo>().sprAni)
        {
            StartCoroutine(SpriteAniStart(obj, tweenTime));
        }

        if (obj.GetComponent<BtnTweenInfo>().moveObj.GetComponent<TweenPosition>() != null)
        {
            if (obj.GetComponent<BtnTweenInfo>().moveObj.GetComponent<TweenPosition>().to != obj.GetComponent<BtnTweenInfo>().startPos.localPosition)
            {
                TweenAllDestroy(obj);
                AddTweenPosition(obj.GetComponent<BtnTweenInfo>().moveObj,
                                obj.GetComponent<BtnTweenInfo>().moveObj.transform.localPosition,
                                obj.GetComponent<BtnTweenInfo>().startPos.localPosition,
                                tweenTime);

                TweenPosition(obj.GetComponent<BtnTweenInfo>().moveObj);

                slideMenuOpen = false;

                yield return new WaitForSeconds(tweenTime);

                yield break;
            }
        }

        int objCount = 0;

        for (int i = 0; i < obj.transform.childCount; i++)
        {
            if (obj.transform.GetChild(i).gameObject.activeSelf)
            {
                objCount++;
            }
        }

        TweenAllDestroy(obj);

        if (objCount <= 3)
        {
            AddTweenPosition(obj.GetComponent<BtnTweenInfo>().moveObj,
                             obj.GetComponent<BtnTweenInfo>().moveObj.transform.localPosition,
                             obj.GetComponent<BtnTweenInfo>().threeEndPos.localPosition,
                             tweenTime);
        }
        else
        {
            AddTweenPosition(obj.GetComponent<BtnTweenInfo>().moveObj,
                             obj.GetComponent<BtnTweenInfo>().moveObj.transform.localPosition,
                             obj.GetComponent<BtnTweenInfo>().fourEndPos.localPosition,
                             tweenTime);
        }

        TweenPosition(obj.GetComponent<BtnTweenInfo>().moveObj);

        slideMenuOpen = true;

        yield return new WaitForSeconds(tweenTime);

        yield break;
    }


    private IEnumerator MainButtonTweenScaleStart(GameObject obj)
    {
        playing = true;
        float waitTime = 0.3f;

        TweenAllDestroy(obj);
        AddTweenScale(obj, obj.transform.localScale, obj.GetComponent<BtnTweenInfo>().endScale, waitTime, UITweener.Style.Once, scaleAnimationCurve);
        TweenScale(obj);

        //yield return new WaitForSeconds(waitTime);

        SubButtonTweenScale(obj);

        yield break;
    }

    private IEnumerator SubButtonTweenScaleStart(GameObject obj, int subObjCount, float initNextTime)
    {
        int nowObjCount = 0;
        float nextTime = initNextTime;
        float savedNextTime = nextTime;
        float tweenTime = 0.2f;

        Vector3 startScale = obj.GetComponent<BtnTweenInfo>().startScale;
        Vector3 subStartScale = obj.GetComponent<BtnTweenInfo>().subStartScale;
        Vector3 subEndScale = obj.GetComponent<BtnTweenInfo>().subEndScale;

        GameObject targetObj = null;

        if (obj.GetComponent<BtnTweenInfo>().reverse)
        {
            nextTime = obj.GetComponent<BtnTweenInfo>().reverseNextTime;
            savedNextTime = nextTime;
        }

        while (true)
        {
            if (nowObjCount < subObjCount)
            {
                nextTime = nextTime - Time.deltaTime;

                if (nextTime < 0)
                {
                    if (obj.GetComponent<BtnTweenInfo>().reverse)
                    {
                        targetObj = obj.transform.GetChild((subObjCount - nowObjCount) - 1).gameObject;

                        if (!targetObj.activeSelf)
                        {
                            targetObj.transform.localScale = subStartScale;
                        }
                        else
                        {
                            TweenAllDestroy(targetObj);
                            AddTweenScale(targetObj, targetObj.transform.localScale, subStartScale, tweenTime, UITweener.Style.Once, subTweenScaleCurve);
                            TweenScale(targetObj);
                        }
                    }
                    else
                    {
                        targetObj = obj.transform.GetChild(nowObjCount).gameObject;

                        if (!targetObj.activeSelf)
                        {
                            targetObj.transform.localScale = subEndScale;
                        }
                        else
                        {
                            TweenAllDestroy(targetObj);
                            AddTweenScale(targetObj, targetObj.transform.localScale, subEndScale, tweenTime, UITweener.Style.Once, subTweenScaleCurve);
                            TweenScale(targetObj);
                        }
                    }

                    nextTime = savedNextTime;
                    nowObjCount++;
                }
            }
            else
            {

                if (obj.GetComponent<BtnTweenInfo>().reverse)
                {
                    TweenAllDestroy(obj.transform.gameObject);

                    AddTweenScale(obj.transform.gameObject,
                                  obj.transform.gameObject.transform.localScale,
                                  startScale,
                                  tweenTime,
                                  UITweener.Style.Once,
                                  subTweenScaleCurve);

                    TweenScale(obj.transform.gameObject);
                }

                obj.GetComponent<BtnTweenInfo>().reverse = !obj.GetComponent<BtnTweenInfo>().reverse;


                obj.GetComponent<BtnTweenInfo>().playDirection = !obj.GetComponent<BtnTweenInfo>().playDirection;
                playing = false;
                yield break;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator UiTimer(bool state)
    {
        while (true)
        {
            if (state)
            {
                shadowUi.GetComponent<UIPanel>().alpha += Time.deltaTime;

                if (shadowUi.GetComponent<UIPanel>().alpha >= 1)
                {
                    shadowUi.GetComponent<UIPanel>().alpha = 1.0f;
                    yield break;
                }
            }
            else
            {
                shadowUi.GetComponent<UIPanel>().alpha -= Time.deltaTime / 2;

                if (shadowUi.GetComponent<UIPanel>().alpha <= 0)
                {
                    shadowUi.GetComponent<UIPanel>().alpha = 0;
                    yield break;
                }

            }

            yield return new WaitForEndOfFrame();
        }
    }
}
