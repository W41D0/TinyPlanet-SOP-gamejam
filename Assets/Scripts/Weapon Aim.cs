using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponAim : MonoBehaviour
{
    Vector2 mouseScreenPosition;
    Vector3 mouseWorldPosition;
    Vector3 aimDirection;
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

        aimDirection = mouseWorldPosition - transform.position;

        angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void OnAim(InputValue value)
    {
        mouseScreenPosition = value.Get<Vector2>();
    }
}
