using System;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "Level")]
public class Level : ScriptableObject
{
    [SerializeField, Scene] 
    public string sceneName;

    [SerializeField, Tooltip("Levels in this list will be loaded alongside the main Level, to make gameplay seamless")] 
    public Level[] neighbors = Array.Empty<Level>();

    [SerializeField, Tooltip("Should this scene remain loaded when other scenes load?")]
    public bool isPersistent;

    /// <summary>
    /// The Level that currently contains the player.
    /// <remarks> The player may be in multiple Levels at the same time due to collider overlap.</remarks>
    /// <remarks> In this case, the ActiveLevel is the most recently entered Level.</remarks>
    /// </summary>
    public static Level ActiveLevel => LevelManager.Instance.ActiveLevel;
    
    /// <summary>
    /// Loads this Level and it's neighbors.
    /// </summary>
    public void Load()
    {
        LevelManager.Instance.StartCoroutine(CoroutineLoad());
    }

    /// <summary>
    /// Unloads this Level and it's neighbors.
    /// </summary>
    public void Unload()
    {
        LevelManager.Instance.StartCoroutine(CoroutineUnload());
    }
    
    /// <summary>
    /// Can be used within a coroutine to load this Level and it's neighbors.
    /// </summary>
    /// <returns>An enumerator to be used within a Coroutine, ending when all Levels have been loaded.</returns>
    public IEnumerator CoroutineLoad()
    {
        return LevelManager.Instance.LoadLevel(this);
    }

    /// <summary>
    /// Can be used within a coroutine to unload this Level and it's neighbors.
    /// </summary>
    /// <returns>An enumerator to be used within a Coroutine, ending when all Levels have been unloaded.</returns>
    public IEnumerator CoroutineUnload()
    {
        return LevelManager.Instance.UnloadLevel(this);
    }

    /// <summary>
    /// Finds the Level data associated with a scene.
    /// </summary>
    /// <param name="sceneName">The name of the desired scene.</param>
    /// <returns>A Level that represents a scene and it's neighbors.</returns>
    public static Level GetLevelFromScene(string sceneName)
    {
        return LevelManager.Instance.GetLevel(sceneName);
    }

    public static Level GetLevelFromScene(Scene scene)
    {
        return LevelManager.Instance.GetLevel(scene.name);
    }
}