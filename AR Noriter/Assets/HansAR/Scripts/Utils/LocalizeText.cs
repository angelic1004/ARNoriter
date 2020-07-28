using UnityEngine;
using System;
using System.IO;

[DisallowMultipleComponent]
public class LocalizeText : MonoBehaviour
{
    public string ValueName = string.Empty;

    private static LocalizeValue localizeValue = null;

    public static LocalizeValue Value
    {
        get
        {
            if (localizeValue == null)
            {
                localizeValue = new LocalizeValue();
            }
            return localizeValue;
        }
    }

    public static SystemLanguage DefaultLanguage
    {
        get
        {
            return Value.DefaultLanguage;
        }
        set
        {
            Value.DefaultLanguage = value;
        }
    }

    public static SystemLanguage CurrentLanguage
    {
        get
        {
            return Value.CurrentLanguage;
        }
        set
        {
            Value.CurrentLanguage = ChangeLanguage(value);
        }
    }

#if UNITY_EDITOR
    private bool wasEnabled = false;

    void Awake()
    {
        wasEnabled = enabled;
    }

    void OnEnable()
    {
        if (!wasEnabled)
        {
            ApplyLanguage();
        }
        wasEnabled = true;
    }

    void OnDisable()
    {
        wasEnabled = false;
    }
#endif

    public void ApplyLanguage()
    {
        SystemLanguage language = Value.CurrentLanguage;
        if (language == SystemLanguage.Unknown)
        {
            language = Value.CurrentLanguage;
        }
        ApplyLanguage(language);
    }

    private void ApplyLanguage(SystemLanguage language)
    {
        if (enabled)
        {
            string text = Value.GetString(language, ValueName);
            if (text != null)
            {
                UnityEngine.UI.Text uguiLabel = GetComponent<UnityEngine.UI.Text>();
                if (uguiLabel != null)
                {
                    uguiLabel.text = text;
                    return;
                }

                UILabel nguiLabel = GetComponent<UILabel>();
                if (nguiLabel != null)
                {
                    UIInput nguiInput = GetComponentInParent<UIInput>();
                    if (nguiInput != null && nguiInput.label == nguiLabel)
                    {
                        nguiInput.defaultText = text;
                    }
                    else
                    {
                        nguiLabel.text = text;
                    }
                    return;
                }
            }
        }
    }

    private static SystemLanguage ChangeLanguage(SystemLanguage language)
    {
        SystemLanguage previousLanguage = Value.CurrentLanguage;
        if (language == SystemLanguage.Unknown)
        {
            language = Value.CurrentLanguage;
        }

        Type type = LocalizeValue.GetLocalizeType(language);
        if (type == null)
        {
            if (language != SystemLanguage.Unknown)
            {
                Debug.LogWarning(string.Format("Language type not found : {0}", language));
            }
            return previousLanguage;
        }

        LocalizeText[] localizes = Resources.FindObjectsOfTypeAll<LocalizeText>();
        foreach (LocalizeText localize in localizes)
        {
            localize.ApplyLanguage(language);
        }

        string atlasDir = string.Format("UI/{0}", language);
#if UNITY_EDITOR
        if (Directory.Exists(string.Format("{0}/Resources/{1}", Application.dataPath, atlasDir)))
        {
#endif
            UISprite[] nguiSprites = Resources.FindObjectsOfTypeAll<UISprite>();
            foreach (UISprite sprite in nguiSprites)
            {
                if (sprite.atlas != null && !string.IsNullOrEmpty(sprite.spriteName))
                {
                    UIAtlas atlas = Resources.Load<UIAtlas>(string.Format("{0}/{1}", atlasDir, sprite.atlas.name));
                    if (atlas != null && atlas.GetSprite(sprite.spriteName) != null)
                    {
                        sprite.atlas = atlas;
                    }
                }
            }
#if UNITY_EDITOR
        }
        else
        {
            Debug.Log(string.Format("UI Atlas not found : {0}", atlasDir));
        }
#endif

        if (previousLanguage == language)
        {
            Debug.Log(string.Format("Language has updated : {0}", language));
        }
        else
        {
            Debug.Log(string.Format("Language has changed : {0} -> {1}", previousLanguage, language));
        }
        return language;
    }
}
