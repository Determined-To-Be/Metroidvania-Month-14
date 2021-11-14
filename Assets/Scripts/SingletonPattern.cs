using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MVMXIV;

public class SingletonPattern<T> : MonoBehaviour where T : Component
{
    static T instance;
    public static T Instance
    {
        get
        {
            // If there's no T, find it
            if (instance == null)
            {
                instance = FindObjectOfType<T>();
            }
            // If there's still no T, make one
            if (instance == null)
            {
                GameObject obj = new GameObject();
                obj.name = typeof(T).Name;
                instance = obj.AddComponent<T>();
            }
            return instance;
        }
    }

    void Awake()
    {
        // If there's no T, find it
        if (instance == null)
        {
            instance = GetComponent<T>();
        }
        // If there's still no T, this is the T now
        if (instance == null)
        {
            instance = this as T;
        }
        // If there is a T and this is not THE T, this T commits sudoku
        if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}
