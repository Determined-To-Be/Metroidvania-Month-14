using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T _instance;
    
    public static T Get()
    {
        if (_instance == null)
        {
            _instance = new GameObject(typeof(T).Name, typeof(T)).GetComponent<T>();
            DontDestroyOnLoad(_instance);
        }
        
        return _instance;
    }

    protected virtual void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = (T) this;
        }
    }
}