using System;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "Level")]
public class Level : ScriptableObject
{
    [SerializeField, Scene] 
    public string sceneName;
    
    [SerializeField, Tooltip("Levels in this list will be loaded alongside the main level, to make gameplay seamless")] 
    public Level[] levelsToPreload = Array.Empty<Level>();
    
    [SerializeField, Tooltip("Can the player ever be inside this scene? (Important: used for saving)")] 
    public bool isGameplayLevel = true;

    [SerializeField, Tooltip("Should this scene remain loaded when other scenes load?")]
    public bool isPersistent;
}