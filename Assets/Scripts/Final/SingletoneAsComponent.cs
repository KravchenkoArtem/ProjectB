using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletoneAsComponent<T> : MonoBehaviour where T : SingletoneAsComponent<T>
{
    private static T __Instance;

    protected static SingletoneAsComponent<T> _Instance
    {
        get
        {
            if (!__Instance)
            {
                T[] managers = FindObjectsOfType(typeof(T)) as T[];
                if (managers != null)
                {
                    if (managers.Length == 1)
                    {
                        __Instance = managers[0];
                        return __Instance;
                    }
                    else if (managers.Length > 1)
                    {
                        Debug.LogError("You have more than one " + typeof(T).Name + " in the scene.");
                        for (int i = 0; i < managers.Length; i++)
                        {
                            T manager = managers[i];
                            Destroy(manager.gameObject);
                        }
                    }
                }

                GameObject go = new GameObject(typeof(T).Name, typeof(T));
                __Instance = go.GetComponent<T>();
                DontDestroyOnLoad(__Instance.gameObject);
            }
            return __Instance;
        }
        set
        {
            __Instance = value as T;
        }
    }

    private void Awake()
    {
        if (ShouldBeSetToDontDestroyOnLoadDuringAwake())
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    protected virtual bool ShouldBeSetToDontDestroyOnLoadDuringAwake()
    {
        return false;
    }

    private bool _alive = true;
    public static bool IsAlive
    {
        get
        {
            if (__Instance == null)
                return false;
            return __Instance._alive;
        }
    }

    private void OnDestroy()
    {
        _alive = false;
    }

    private void OnApplicationQuit()
    {
        _alive = false;
    }
}
