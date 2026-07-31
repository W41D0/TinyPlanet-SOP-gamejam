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
    Boolean isSprinting;
    Boolean isShooting;
    Boolean isOnShootCooldown;


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

    public void SetIsShooting(bool isShooting)
    {
        this.isShooting = isShooting;
    }

    public void SetIsOnShootCooldown(bool isOnShootCooldown)
    {
        this.isOnShootCooldown = isOnShootCooldown;
    }

    public bool GetIsShooting()
    {
        return isShooting;
    }

    public bool GetIsOnShootCooldown()
    {
        return isOnShootCooldown;
    }
}
