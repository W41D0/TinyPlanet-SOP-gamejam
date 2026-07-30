using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    Vector2 moveDirection;
    [SerializeField] float baseMoveSpeed = 5f;
    [SerializeField] float SprintMultiplier = 1.5f;
    float moveSpeed;
    Boolean isSprinting;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        moveSpeed = baseMoveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = moveDirection * moveSpeed;
    }

    void OnMove(InputValue value)
    {
        moveDirection = value.Get<Vector2>();
    }

    void OnSprint (InputValue value)
    {
        float sprintSpeed;
        if (value.isPressed)
        {
            moveSpeed = baseMoveSpeed * SprintMultiplier;
            isSprinting = true;
        }
        else
        {
            moveSpeed = baseMoveSpeed;
            isSprinting = false;
        }
    }
}
