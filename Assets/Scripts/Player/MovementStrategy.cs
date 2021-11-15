using UnityEngine;

public abstract class MovementStrategy : ScriptableObject
{
    protected MovementSystem2D MovementSystem2D;
    
    public void Initialize(MovementSystem2D movementSystem2D)
    {
        MovementSystem2D = movementSystem2D;
    }
    
    public abstract Vector3 CalculateVelocity();
}