using UnityEngine;
using System;

[DisallowMultipleComponent]
public class LocalizeManager : MonoBehaviour
{
    public SystemLanguage CurrentLanguage = SystemLanguage.Unknown;

    private SystemLanguage previousLanguage = SystemLanguage.Unknown;

    void FixedUpdate()
    {
        if (CurrentLanguage != previousLanguage)
        {
            ApplyLanguage();
        }
    }

    void OnEnable()
    {
        ApplyLanguage();
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (CurrentLanguage != SystemLanguage.Unknown)
        {
            if (LocalizeValue.GetLocalizeType(CurrentLanguage) == null)
            {
                Debug.LogWarning(string.Format("Language type not found : {0}", CurrentLanguage));
                CurrentLanguage = previousLanguage;
            }
        }
    }
#endif

    private void ApplyLanguage()
    {
        previousLanguage = CurrentLanguage;
        LocalizeText.CurrentLanguage = CurrentLanguage;
    }
}
