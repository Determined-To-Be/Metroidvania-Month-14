﻿using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

public class JumpingSystem : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool showDebug;
    [SerializeField] private JumpSettings settings;

    [Header("Dependencies")] 
    [SerializeField] private Rigidbody2D targetRigidbody;
    [SerializeField] private GroundedCheck groundCheck;
    [SerializeField] private CustomGravity customGravity;

    [Header("Events")] 
    [SerializeField] private UnityEvent onJump;

    private Vector3 Velocity => targetRigidbody.velocity;
    private bool IsGrounded => groundCheck.IsGrounded;
    private float TimeSpentFalling => groundCheck.TimeSpentFalling;

    private bool WantsToJump => settings.holdAndJump
        ? _holdingJump
        : _holdingJump && !_wasHoldingJump;

    private bool _coyoteAvailable;
    private bool _holdingJump = true;
    private bool _wasHoldingJump;
    private int _remainingAirJumps;
    private Vector3 _currentVelocity;

    private Buffer _jumpBuffer = new Buffer();

    [PublicAPI] public bool HoldingJump { get; set; }

    private void OnEnable()
    {
        groundCheck.CollisionEvents.onEnterCollision.AddListener(OnLand);
    }

    private void OnDisable()
    {
        groundCheck.CollisionEvents.onEnterCollision.RemoveListener(OnLand);
    }

    private void OnLand(Collider2D col)
    {
        _remainingAirJumps = settings.airJumps;
        _coyoteAvailable = true;
    }

    private void Update()
    {
        _wasHoldingJump = _holdingJump;
        _holdingJump = HoldingJump;

        if (WantsToJump)
            _jumpBuffer.Queue();

        ApplyCustomGravity();
    }

    private void ApplyCustomGravity()
    {
        bool rising = Velocity.y > 0;

        if (settings.enableFastFall && _holdingJump == false)
        {
            customGravity.gravity = rising
                ? settings.FastFallGravityRising
                : settings.FastFallGravityFalling;
        }
        else
        {
            customGravity.gravity = rising
                ? settings.StandardGravityRising
                : settings.StandardGravityFalling;
        }
    }

    private void FixedUpdate()
    {
        _currentVelocity = Velocity;

        if (ShouldJump())
            ApplyJump();

        ApplyVelocity();
    }

    private bool ShouldJump()
    {
        if (_jumpBuffer.IsQueued(settings.jumpBufferTime) == false)
            return false;

        if (IsGrounded)
            return true;

        if (_coyoteAvailable && TimeSpentFalling < settings.coyoteTime)
            return true;

        return _remainingAirJumps > 0;
    }

    private void ApplyJump()
    {
        _jumpBuffer.Clear();

        _coyoteAvailable = false;
        _currentVelocity.y = settings.JumpSpeed;
        onJump.Invoke();
    }

    private void ApplyVelocity()
    {
        targetRigidbody.velocity = _currentVelocity;
    }

    private void OnGUI()
    {
        if (showDebug)
            DrawDebugUI();
    }

    private void DrawDebugUI()
    {
        GUILayout.Label($"Holding Jump: {_holdingJump}");
        GUILayout.Label($"Current Velocity: {_currentVelocity}");
        GUILayout.Label($"Coyote Available: {_coyoteAvailable}");
        GUILayout.Label($"Jump Queued: {_jumpBuffer.IsQueued(settings.jumpBufferTime)}");
        GUILayout.Label($"Remaining Air Jumps: {_remainingAirJumps}");
    }
}