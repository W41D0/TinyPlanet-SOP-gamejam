using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponAim : MonoBehaviour
{
    [Header("Floaty Settings")]
    [SerializeField] Transform playerTransform;
    [SerializeField] float radius = 0.25f;
    [SerializeField] float gunFollowSpeed = 15f;
    [SerializeField] float gunRotateSpeed = 12f;



    Vector2 mouseScreenPosition;
    Vector3 mouseWorldPosition;
    Vector3 directionFromPlayer;
    Vector3 targetPosition;
    Vector3 aimDirection;
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
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, gunFollowSpeed * Time.deltaTime);

        aimDirection = mouseWorldPosition - transform.position;
        angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        
        targetRotation = Quaternion.Euler(0f, 0f, angle);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, gunRotateSpeed * Time.deltaTime);
    }

    void OnAim(InputValue value)
    {
        mouseScreenPosition = value.Get<Vector2>();
    }
}
