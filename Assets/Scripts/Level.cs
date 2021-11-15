using System;
using System.Collections;
using JetBrains.Annotations;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "Level")]
public class Level : ScriptableObject
{
    [Scene]
    [Tooltip("The scene that should be loaded for this Level.")] 
    public string sceneName;

    [Tooltip("Should this scene remain loaded when other scenes load?")]
    public bool isPersistent;
    
    [Space]
    [ShowIf("IsNotPersistent")]
    [Tooltip("Levels in this list will be loaded alongside the main Level, to make gameplay seamless.")]
    public Level[] neighbors = Array.Empty<Level>();
    
    [UsedImplicitly]
    private bool IsNotPersistent => !isPersistent;
    
    /// <summary>
    /// The Level that currently contains the player.
    /// <remarks> The player may be in multiple Levels at the same time due to collider overlap.</remarks>
    /// <remarks> In this case, the ActiveLevel is the most recently entered Level.</remarks>
    /// </summary>
    
    [CanBeNull]
    [PublicAPI]
    public static Level ActiveLevel => LevelManager.Instance.ActiveLevel;
    
    /// <summary>
    /// Loads this Level and it's neighbors.
    /// </summary>
    
    [PublicAPI]
    public void Load()
    {
        LevelManager.Instance.StartCoroutine(CoroutineLoad());
    }

    /// <summary>
    /// Unloads this Level and it's neighbors.
    /// </summary>
    
    [PublicAPI] 
    public void Unload()
    {
        LevelManager.Instance.StartCoroutine(CoroutineUnload());
    }
    
    /// <summary>
    /// Can be used within a coroutine to load this Level and it's neighbors.
    /// </summary>
    /// <returns>An enumerator to be used within a Coroutine, ending when all Levels have been loaded.</returns>
    
    [PublicAPI] 
    public IEnumerator CoroutineLoad()
    {
        return LevelManager.Instance.LoadLevel(this);
    }

    /// <summary>
    /// Can be used within a coroutine to unload this Level and it's neighbors.
    /// </summary>
    /// <returns>An enumerator to be used within a Coroutine, ending when all Levels have been unloaded.</returns>
    
    [PublicAPI] 
    public IEnumerator CoroutineUnload()
    {
        return LevelManager.Instance.UnloadLevel(this);
    }

    /// <summary>
    /// Finds the Level data associated with a scene.
    /// </summary>
    /// <param name="sceneName">The name of the desired scene.</param>
    /// <returns>A Level that represents a scene and it's neighbors.</returns>
    
    [CanBeNull]
    [PublicAPI]
    public static Level GetLevelFromScene(string sceneName)
    {
        return LevelManager.Instance.GetLevelFromScene(sceneName);
    }

    /// <summary>
    /// Finds the Level data associated with a scene.
    /// </summary>
    /// <param name="scene">The desired scene.</param>
    /// <returns>A Level that represents a scene and it's neighbors.</returns>
    
    [CanBeNull]
    [PublicAPI]
    public static Level GetLevelFromScene(Scene scene)
    {
        return LevelManager.Instance.GetLevelFromScene(scene.name);
    }

    #region Editor Stuff
    #if UNITY_EDITOR

    [Button("Load")]
    [UsedImplicitly]
    private void InspectorLoad()
    {
        OpenScene(sceneName, isPersistent);
    }

    [ShowIf("IsNotPersistent")]
    [Button("Load Neighbors")]
    [UsedImplicitly]
    private void InspectorLoadNeighbors()
    {
        OpenScene(sceneName, isPersistent);

        foreach (Level neighbor in neighbors)
            OpenScene(neighbor.sceneName, true);
    }
    
    private static void OpenScene(string targetSceneName, bool additive)
    {
        if (Application.isPlaying)
        {
            SceneManager.LoadScene(targetSceneName, LoadSceneMode.Additive);
        }
        else
        {
            string[] results = UnityEditor.AssetDatabase.FindAssets(targetSceneName, new [] { "Assets/Scenes" });
            string scenePath = UnityEditor.AssetDatabase.GUIDToAssetPath(results[0]);
            
            var openSceneMode = additive
                ? UnityEditor.SceneManagement.OpenSceneMode.Additive
                : UnityEditor.SceneManagement.OpenSceneMode.Single;
            
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scenePath, openSceneMode);
        }
    }
    
    #endif    
    #endregion
}