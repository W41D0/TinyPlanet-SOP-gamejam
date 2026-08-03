using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [HideInInspector] public bool isSafeToSpawn = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerSight")) 
        {
            isSafeToSpawn = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerSight"))
        {
            isSafeToSpawn = true;
        }
    }
}