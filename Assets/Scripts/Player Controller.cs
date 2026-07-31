using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    GunScript gun;

    Rigidbody2D rb;
    Vector2 moveDirection;
    [SerializeField] float baseMoveSpeed = 5f;
    [SerializeField] float SprintMultiplier = 1.5f;
    float walkSpeed;
    bool isSprinting;

    bool isShooting;
    public bool IsShooting {get => isShooting; set => isShooting = value;}

    bool isOnShootCooldown;
    public bool IsOnShootCooldown {get => isOnShootCooldown; set => isOnShootCooldown = value;}

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Transform gunTransform = transform.Find("Weapon Pivot/Gun");
        gun = gunTransform.GetComponent<GunScript>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        walkSpeed = baseMoveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = moveDirection * walkSpeed;
    }

    void OnMove(InputValue value)
    {
        if(!isSprinting)
        {
            walkSpeed = baseMoveSpeed;
        }
        moveDirection = value.Get<Vector2>();
    }

    void OnSprint (InputValue value)
    {
        if (value.isPressed)
        {
            walkSpeed = baseMoveSpeed * SprintMultiplier;
            isSprinting = true;
        }
        else
        {
            walkSpeed = baseMoveSpeed;
            isSprinting = false;
        }
    }

    void OnFire(InputValue value)
    {
        if (value.isPressed)
        {
            isShooting = true;
        }
        else
        {
            isShooting = false;
        }
    }
}
