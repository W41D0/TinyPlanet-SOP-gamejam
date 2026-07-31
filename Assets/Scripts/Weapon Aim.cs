using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponAim : MonoBehaviour
{
    [Header("Targeting")]
    [SerializeField] Transform playerTransform;
    [SerializeField] float radius = 0.25f;

    [Header("Bounce")]
    [SerializeField] float springStiffness = 250f;
    [SerializeField] float damping = 15f;

    [Header("Rotation")]
    [SerializeField] float gunRotateSpeed = 12f;


    Vector2 mouseScreenPosition;
    Vector3 mouseWorldPosition;
    Vector3 directionFromPlayer;
    Vector3 targetPosition;
    Vector3 aimDirection;
    Vector3 displacement;
    Vector3 springForce;
    Vector3 gunVelocity;
    Quaternion targetRotation;
    float angle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        mouseWorldPosition.z = 0;

        directionFromPlayer = mouseWorldPosition - playerTransform.position;
        targetPosition = Vector3.ClampMagnitude(directionFromPlayer, radius);

        displacement = targetPosition - transform.localPosition;

        springForce = displacement * springStiffness;
        
        gunVelocity += springForce * Time.deltaTime;
        gunVelocity -= gunVelocity * damping * Time.deltaTime;

        transform.localPosition += gunVelocity * Time.deltaTime;

        aimDirection = mouseWorldPosition - transform.position;
        angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        
        targetRotation = Quaternion.Euler(0f, 0f, angle);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, gunRotateSpeed * Time.deltaTime);
    }

    public void ApplyRecoil(float recoilStrength)
    {
        // 1. Calculate backward direction away from the mouse
        Vector3 backwardDirection = -directionFromPlayer.normalized;

        // 2. Inject the force directly into your custom math engine
        gunVelocity += backwardDirection * recoilStrength;
    }

    void OnAim(InputValue value)
    {
        mouseScreenPosition = value.Get<Vector2>();
    }
}
