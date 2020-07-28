using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T instance = null;
    public static T getInstance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(T)) as T;

                if (instance == null)
                {
                    Debug.LogWarning("Nothing " + instance.ToString());
                    return null;
                }
            }

            return instance;
        }
    }
}
