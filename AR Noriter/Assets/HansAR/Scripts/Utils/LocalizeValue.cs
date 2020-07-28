using UnityEngine;
using System;
using System.Reflection;

public class LocalizeValue
{
    public SystemLanguage DefaultLanguage = SystemLanguage.English;
    public SystemLanguage CurrentLanguage = SystemLanguage.Korean;

    public string this[string name]
    {
        get
        {
            return GetString(name);
        }
    }

    public string this[LocalizeID id]
    {
        get
        {
            return GetString(id);
        }
    }

    public static Type GetLocalizeType(SystemLanguage language)
    {
        return Type.GetType(string.Format("Language{0}", language), false, true);
    }

    public Type GetValueType(SystemLanguage language, string name)
    {
        if (!string.IsNullOrEmpty(name))
        {
            Type type = GetLocalizeType(language);
            if (type == null)
            {
                if (language != DefaultLanguage)
                {
                    return GetValueType(DefaultLanguage, name);
                }
            }
            else
            {
                FieldInfo fi = type.GetField(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
                PropertyInfo pi = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
                if (fi != null)
                {
                    return fi.FieldType;
                }
                else if (pi != null && pi.CanRead)
                {
                    return pi.PropertyType;
                }
                else
                {
                    if (language != DefaultLanguage)
                    {
                        return GetValueType(DefaultLanguage, name);
                    }
                }
            }
        }
        return null;
    }

    public Type GetValueType(string name)
    {
        return GetValueType(CurrentLanguage, name);
    }

    public T GetValue<T>(SystemLanguage language, string name)
    {
        if (!string.IsNullOrEmpty(name))
        {
            Type type = GetLocalizeType(language);
            if (type == null)
            {
                if (language != DefaultLanguage)
                {
                    return GetValue<T>(DefaultLanguage, name);
                }
            }
            else
            {
                FieldInfo fi = type.GetField(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
                PropertyInfo pi = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
                if (fi != null)
                {
                    if (typeof(T) == fi.FieldType)
                    {
                        return (T)fi.GetValue(null);
                    }
                    Debug.LogWarning(string.Format("Different field type : {0} and {1}", fi.FieldType, typeof(T)));
                }
                else if (pi != null && pi.CanRead)
                {
                    if (typeof(T) == pi.PropertyType)
                    {
                        return (T)pi.GetValue(null, null);
                    }
                    Debug.LogWarning(string.Format("Different property type : {0} and {1}", pi.PropertyType, typeof(T)));
                }
                else
                {
                    if (language != DefaultLanguage)
                    {
                        return GetValue<T>(DefaultLanguage, name);
                    }
                    Debug.LogWarning(string.Format("Cannot find localize value : {0}", name));
                }
            }
        }
        return default(T);
    }

    public T GetValue<T>(SystemLanguage language, LocalizeID id)
    {
        return GetValue<T>(language, id.ToString());
    }

    public T GetValue<T>(string name)
    {
        return GetValue<T>(CurrentLanguage, name);
    }

    public T GetValue<T>(LocalizeID id)
    {
        return GetValue<T>(CurrentLanguage, id.ToString());
    }

    public string GetString(SystemLanguage language, string name)
    {
        return GetValue<string>(language, name);
    }

    public string GetString(SystemLanguage language, LocalizeID id)
    {
        return GetValue<string>(language, id.ToString());
    }

    public string GetString(string name)
    {
        return GetValue<string>(CurrentLanguage, name);
    }

    public string GetString(LocalizeID id)
    {
        return GetValue<string>(CurrentLanguage, id.ToString());
    }
}
