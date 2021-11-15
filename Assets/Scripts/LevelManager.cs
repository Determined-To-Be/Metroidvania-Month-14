using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Provides logic for loading and unloading scenes from their Level data.
/// </summary>
public class LevelManager : SingletonPattern<LevelManager>
{
    private Dictionary<string, Level> _sceneNamesToLevels = new Dictionary<string, Level>();
    private LinkedList<Level> _activeLevels = new LinkedList<Level>();
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
    
    public Level GetLevel(string sceneName)
    {
        return _sceneNamesToLevels[sceneName];
    }

    private void SetupActiveLevel()
    {
        var activeLevel = GetLevel(SceneManager.GetActiveScene().name);
        AddActiveLevel(activeLevel);
    }

    private void AddActiveLevel(Level level)
    {
        if (!_activeLevels.Contains(level) && !level.isPersistent)
        {
            _activeLevels.AddFirst(level);
            CustomSceneManager.SetActiveScene(ActiveLevel.sceneName);    
        }
    }
    
    private void RemoveActiveLevel(Level level)
    {
        Level oldLevel = ActiveLevel;
        
        _activeLevels.Remove(level);

        if (oldLevel != ActiveLevel)
            CustomSceneManager.SetActiveScene(level.sceneName);
    }
    
    public IEnumerator LoadLevel(Level level)
    {
        yield return CustomSceneManager.LoadAdditive(level.sceneName);

        AddActiveLevel(level);

        foreach (Level neighbor in level.neighbors)
            yield return CustomSceneManager.LoadAdditive(neighbor.sceneName);
    }
    
    
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