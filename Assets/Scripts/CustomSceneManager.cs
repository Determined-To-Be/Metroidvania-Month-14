using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomSceneManager : SceneManager
{
    public static void SetActiveScene(string sceneName)
    {
        SceneManager.SetActiveScene(GetSceneByName(sceneName));
    }
    
    public static bool IsLoaded(string sceneName)
    {
        return GetSceneByName(sceneName).IsValid();
    }
    
    public static YieldInstruction LoadAdditive(string sceneName)
    {
        if (!IsLoaded(sceneName))
            return LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        return null;
    }
    
    public static YieldInstruction UnloadAdditive(string sceneName)
    {
        if (IsLoaded(sceneName))
            return UnloadSceneAsync(sceneName);

        return null;
    }
}