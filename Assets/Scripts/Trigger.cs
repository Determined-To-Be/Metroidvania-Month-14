using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public struct CollisionEvents 
{
    public UnityEvent<GameObject> collisionEnter;
    public UnityEvent<GameObject> collisionExit;
}

[RequireComponent(typeof(Collider2D))]
// Wraps a Collider and exposes UnityEvents for OnTriggerEnter / Exit 
public class Trigger : MonoBehaviour
{
    [Header("Trigger Events")]
    [SerializeField] public CollisionEvents events;
    
    [Header("Trigger Settings")]
    [SerializeField] public Collider2D colliderComponent;
    [SerializeField] public LayerMask layersThatCanTrigger = ~0;
    
    private readonly List<GameObject> _objectsInTrigger = new List<GameObject>(); 
    private GameObject _collidedObject;

    public bool HasObjectInTrigger(GameObject obj)
    {
        return _objectsInTrigger.Contains(obj);
    }
    
    protected virtual void Awake()
    {
        if (colliderComponent == null)
            colliderComponent = GetComponent<Collider2D>();

        colliderComponent.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _collidedObject = other.gameObject;

        if (ObjectCanTrigger())
            AddObjectAndFireEvent();    
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        _collidedObject = other.gameObject;

        if (ObjectCanTrigger())
            RemoveObjectAndFireEvent();
    }

    private bool ObjectCanTrigger()
    {
        // Make sure the collided object falls within our layer mask.
        return (1 << _collidedObject.layer & layersThatCanTrigger) != 0;
    }

    private void AddObjectAndFireEvent()
    {
        _objectsInTrigger.Add(_collidedObject);
        events.collisionEnter?.Invoke(_collidedObject);
    }
    
    private void RemoveObjectAndFireEvent()
    {
        _objectsInTrigger.Remove(_collidedObject);
        events.collisionExit?.Invoke(_collidedObject);
    }
}