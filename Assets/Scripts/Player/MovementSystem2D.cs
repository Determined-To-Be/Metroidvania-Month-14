using JetBrains.Annotations;
using UnityEngine;

public class MovementSystem2D : MonoBehaviour
{
    [Header("Settings")] 
    [SerializeField] protected bool showDebug;
    [SerializeField] private float baseMovementSpeed;

    [Header("Dependencies")] 
    [SerializeField] private Rigidbody2D targetRigidbody;
    [SerializeField] private GroundedCheck groundCheck;
    [SerializeField] private MovementStrategy strategy;

    [PublicAPI] public bool IsGrounded => groundCheck.IsGrounded;
    [PublicAPI] public float BaseMovementSpeed => baseMovementSpeed;
    [PublicAPI] public float TimeSpentOnGround => groundCheck.TimeSpentGrounded;
    [PublicAPI] public float CurrentMaxSpeed { get; set; }
    [PublicAPI] public Vector3 MovementDirection { get; set; }
    [PublicAPI] public Vector3 Velocity => targetRigidbody.velocity;
    
    private void Start()      { SetupVariables(); }

    private void SetupVariables()
    {
        strategy.Initialize(this);
        CurrentMaxSpeed = baseMovementSpeed;
    }

    public float GetCurrentSpeed()
    {
        Vector3 velocity = Velocity;
        velocity.y = 0;

        return velocity.magnitude;
    }
    
    private void Update()
    {
        Vector3 targetVelocity = CustomUpdateVelocity();
        
        ApplyVelocity(targetVelocity); 
    }

    private Vector3 CustomUpdateVelocity()
    {
        return strategy.CalculateVelocity();
    }

    private void ApplyVelocity(Vector3 targetVelocity)
    {
        targetRigidbody.velocity = targetVelocity; 
    }

    private void OnGUI()
    {
        if (showDebug)
            DrawDebugUI();
    }

    private void DrawDebugUI()
    {
        GUILayout.Label($"Movement Direction: {MovementDirection}");
        GUILayout.Label($"Current Speed: {GetCurrentSpeed()}");
        GUILayout.Label($"Current Velocity: {Velocity}");
        GUILayout.Label($"Current Max Speed: {CurrentMaxSpeed}");
    }
}