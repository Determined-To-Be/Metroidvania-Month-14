using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MVMXIV;

public class PlayerController : SingletonPattern<PlayerController>
{
    Rigidbody2D _rb;

    [SerializeField]
    Transform groundCheck;
    [SerializeField]
    float groundCheckRadius = 0.05f;
    [SerializeField]
    float speed = 5.0f;
    [SerializeField]
    float jumpForce = 10f;
    [SerializeField]
    LayerMask collisionMask;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float x = InputManager.Instance.GetAxis(0);
        bool jump = InputManager.Instance.GetButtonDown(Buttons.UP);

        _rb.velocity = new Vector2(x * speed, _rb.velocity.y);

        if (jump && IsGrounded())
        {
            _rb.velocity = new Vector2(_rb.velocity.x, jumpForce);
        }

        if (x != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(x), 1, 1);
        }
    }

    bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, collisionMask);
    }
}
