using UnityEngine;

public class SingletonPattern<T> : MonoBehaviour where T : Component
{
    private static T instance;
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

    protected virtual void Awake()
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

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}
