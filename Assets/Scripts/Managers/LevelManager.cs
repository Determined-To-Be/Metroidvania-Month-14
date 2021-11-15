using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Provides logic for loading and unloading scenes from their Level data.
/// </summary>

public class LevelManager : SingletonPattern<LevelManager>
{
    private Dictionary<string, Level> _sceneNamesToLevels = new Dictionary<string, Level>();
    private LinkedList<Level> _activeLevels = new LinkedList<Level>();
    
    /// <summary>
    /// The Level that currently contains the player.
    /// <remarks> The player may be in multiple Levels at the same time due to collider overlap.</remarks>
    /// <remarks> In this case, the ActiveLevel is the most recently entered Level.</remarks>
    /// </summary>
    
    [CanBeNull]
    [PublicAPI]
    public Level ActiveLevel => _activeLevels.Count > 0 ? _activeLevels.First.Value : null;

    protected override void Awake()
    {
        base.Awake();
        
        SetupLevelDictionary();
        SetupActiveLevel();
    }

    private void SetupLevelDictionary()
    {
        _sceneNamesToLevels = Resources
            .LoadAll<Level>("Levels")
            .ToDictionary(level => level.sceneName);
    }
    
    /// <summary>
    /// Finds the Level data associated with a scene.
    /// </summary>
    /// <param name="sceneName">The name of the desired scene.</param>
    /// <returns>A Level that represents a scene and it's neighbors.</returns>
    
    [CanBeNull]
    [PublicAPI]
    public Level GetLevelFromScene(string sceneName)
    {
        bool isInvalidLevel = !_sceneNamesToLevels.ContainsKey(sceneName); 
        
        if (isInvalidLevel)
            Debug.LogError($"No Level could be found with the name \"{sceneName}\" in the directory \"Resources/Levels\".");
        
        return isInvalidLevel 
            ? null 
            : _sceneNamesToLevels[sceneName];
    }
    
    /// <summary>
    /// Finds the Level data associated with a scene.
    /// </summary>
    /// <param name="scene">The desired scene.</param>
    /// <returns>A Level that represents a scene and it's neighbors.</returns>

    [CanBeNull]
    [PublicAPI]
    public Level GetLevelFromScene(Scene scene)
    {
        return GetLevelFromScene(scene.name);
    }

    private void SetupActiveLevel()
    {
        string currentActiveSceneName = SceneManager.GetActiveScene().name;
        Level currentActiveLevel = GetLevelFromScene(currentActiveSceneName);
        
        if (currentActiveLevel != null && !currentActiveLevel.isPersistent)
            AddActiveLevel(currentActiveLevel);
    }

    private void AddActiveLevel(Level level)
    {
        if (!_activeLevels.Contains(level))
        {
            _activeLevels.AddFirst(level);
            
            if (ActiveLevel != null)
                CustomSceneManager.SetActiveScene(ActiveLevel.sceneName);    
        }
    }
    
    private void RemoveActiveLevel(Level level)
    {
        Level oldLevel = ActiveLevel;
        
        _activeLevels.Remove(level);

        if (oldLevel != ActiveLevel && ActiveLevel != null)
            CustomSceneManager.SetActiveScene(ActiveLevel.sceneName);
    }
    
    /// <summary>
    /// Can be used within a coroutine to load a Level and it's neighbors.
    /// </summary>
    /// <param name="level">The Level to load.</param>
    /// <returns>An enumerator to be used within a Coroutine, ending when all Levels have been loaded.</returns>
    
    [PublicAPI]
    public IEnumerator LoadLevel(Level level)
    {
        yield return CustomSceneManager.LoadAdditive(level.sceneName);

        if (!level.isPersistent)
            AddActiveLevel(level);

        foreach (Level neighbor in level.neighbors)
            yield return CustomSceneManager.LoadAdditive(neighbor.sceneName);
    }
    
    /// <summary>
    /// Can be used within a coroutine to unload this Level and it's neighbors.
    /// </summary>
    /// <param name="level">The Level to unload.</param>
    /// <returns>An enumerator to be used within a Coroutine, ending when all Levels have been unloaded.</returns>
    
    [PublicAPI]
    public IEnumerator UnloadLevel(Level level)
    {
        yield return TryToUnload(level);

        RemoveActiveLevel(level);

        foreach (var levelNeighbor in level.neighbors)
            yield return TryToUnload(levelNeighbor);
    }

    private IEnumerator TryToUnload(Level level)
    {
        if (!LevelShouldBeLoaded(level))
            yield return CustomSceneManager.UnloadAdditive(level.sceneName);
    }
    
    private bool LevelShouldBeLoaded(Level level)
    {
        // Ensure that there is always one scene loaded and persistent levels remain loaded.
        if (_activeLevels.Count < 1 || level.isPersistent)
            return true;
        
        // Active levels and their neighbors should always be loaded.
        foreach (var activeLevel in _activeLevels)
        {
            if (level == activeLevel)
                return true;

            if (activeLevel.neighbors.Any(neighborLevel => level == neighborLevel))
                return true;
        }
        
        return false;
    }
}