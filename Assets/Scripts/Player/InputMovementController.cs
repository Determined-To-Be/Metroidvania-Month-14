using UnityEngine;

public class InputMovementController : MonoBehaviour
{
    [Header("Settings")] 
    [SerializeField] private KeyCode forwardKey = KeyCode.W;
    [SerializeField] private KeyCode backwardKey = KeyCode.S;
    [SerializeField] private KeyCode leftKey = KeyCode.A;
    [SerializeField] private KeyCode rightKey = KeyCode.D;
    // [SerializeField] private KeyCode upKey = KeyCode.Space;
    // [SerializeField] private KeyCode downKey = KeyCode.LeftShift;

    [Header("Dependencies")] 
    [SerializeField] private MovementSystem2D movementSystem2D;
    [SerializeField] private Transform lookDirection;

    private void Update()
    {
        movementSystem2D.MovementDirection = GetMovementDirection();
    }
    
    private Vector3 GetMovementDirection()
    {
        float forwardAxis = CalculateAxis(forwardKey, backwardKey);
        float rightAxis = CalculateAxis(rightKey, leftKey);
        // float upAxis = CalculateAxis(upKey, downKey);
        float upAxis = 0;
        
        Vector3 forward = lookDirection.forward * forwardAxis;
        Vector3 right = lookDirection.right * rightAxis;
        Vector3 up = lookDirection.up * upAxis;

        return (forward + right + up).normalized;
    }

    private static float CalculateAxis(KeyCode positive, KeyCode negative)
    {
        float result = 0;
        
        if (Input.GetKey(positive))
            result += 1;
            
        if (Input.GetKey(negative))
            result -= 1;

        return result;
    }
}