using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GroundedCollisionListener : GroundedCheck
{
    public override bool IsGrounded => _collidersToNormals
        .Any(pair => Vector3.Angle(-gravityDirection, pair.Value) <= slopeLimitDegrees);
    
    public override Vector3 ContactNormal => _collidersToNormals.Count > 0 ? _collidersToNormals
        .Select(pair => pair.Value)
        .Aggregate((prevVector, curVector) => prevVector + curVector) / _collidersToNormals.Count : Vector3.zero;
    
    public override Collider2D ConnectedCollider => _collidersToNormals.Count > 0 ? _collidersToNormals.First().Key : null;

    private Dictionary<Collider2D, Vector3> _collidersToNormals = new Dictionary<Collider2D, Vector3>();

    private void UpdateCollisionDictionary(Collider2D targetCollider, Vector3 normal)
    {
        if (_collidersToNormals.ContainsKey(targetCollider) == false)
            _collidersToNormals.Add(targetCollider, normal);

        else _collidersToNormals[targetCollider] = normal;
    }
    
    private void OnCollisionStay2D(Collision2D other)
    {
        UpdateCollisionDictionary(other.collider, other.GetContact(0).normal);
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        _collidersToNormals.Remove(other.collider);
    }
}