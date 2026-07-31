using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    GunScript gun;

    [Header("Bullets")]
    [SerializeField] GameObject solidBullet;
    [SerializeField] float solidBulletSpeed = 10f;
    [SerializeField] float timeBetweenSolidBullets = 0.5f;
    [SerializeField] float solidBulletRecoil = 2f;

    Rigidbody2D rb;
    Vector2 moveDirection;
    [SerializeField] float baseMoveSpeed = 5f;
    [SerializeField] float SprintMultiplier = 1.5f;
    float moveSpeed;
    Boolean isSprinting;
    Boolean isShooting;
    Boolean isOnShootCooldown;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Transform gunTransform = transform.Find("Weapon Pivot/Gun");
        gun = gunTransform.GetComponent<GunScript>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        moveSpeed = baseMoveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = moveDirection * moveSpeed;
        if(isShooting && !isOnShootCooldown)
        {
            StartCoroutine(shootMethod(timeBetweenSolidBullets));
        }

    }

    void OnMove(InputValue value)
    {
        moveDirection = value.Get<Vector2>();
    }

    void OnSprint (InputValue value)
    {
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

    IEnumerator shootMethod(float shootCooldown)
    {
        gun.Shoot(solidBullet, solidBulletSpeed, solidBulletRecoil);
        isOnShootCooldown = true;
        yield return new WaitForSeconds(shootCooldown);
        isOnShootCooldown = false;
    }
}
