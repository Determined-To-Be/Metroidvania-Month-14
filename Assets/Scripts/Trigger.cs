using System.Collections.Generic;
using JetBrains.Annotations;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Wraps a Collider and exposes UnityEvents for OnTriggerEnter / Exit
/// </summary>

[RequireComponent(typeof(Collider2D))]
public class Trigger : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Only GameObjects on this layer will cause this Trigger to update its state.")]
    public LayerMask layersThatCanTrigger = ~0;
    
    [Foldout("Collision Events")]
    [Tooltip("This event is invoked when an object enters this collider.")]
    public UnityEvent<GameObject> collisionEnter;
    
    [Foldout("Collision Events")]
    [Tooltip("This event is invoked when an object exits this collider.")]
    public UnityEvent<GameObject> collisionExit;
    
    private readonly List<GameObject> _objectsInTrigger = new List<GameObject>(); 
    
    /// <summary>
    /// Checks if a GameObject is currently inside this Trigger.
    /// </summary>
    /// <param name="obj">The GameObject to check for.</param>
    /// <returns>True if the requested GameObject is inside the Trigger, false otherwise.</returns>
    
    [PublicAPI]
    public bool HasObjectInTrigger(GameObject obj)
    {
        return _objectsInTrigger.Contains(obj);
    }
    
    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject obj = other.gameObject;
        
        if (ObjectCanTrigger(obj))
            AddObjectAndFireEvent(obj);    
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        GameObject obj = other.gameObject;
        
        if (ObjectCanTrigger(obj))
            RemoveObjectAndFireEvent(obj);
    }

    private bool ObjectCanTrigger(GameObject obj)
    {
        // Make sure the collided object falls within our layer mask.
        return (1 << obj.layer & layersThatCanTrigger) != 0;
    }

    private void AddObjectAndFireEvent(GameObject obj)
    {
        _objectsInTrigger.Add(obj);
        collisionEnter?.Invoke(obj);
    }
    
    private void RemoveObjectAndFireEvent(GameObject obj)
    {
        _objectsInTrigger.Remove(obj);
        collisionExit?.Invoke(obj);
    }
}