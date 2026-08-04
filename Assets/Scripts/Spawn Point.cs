using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [HideInInspector] public bool isSafeToSpawn = true;

    private void Start()
    {
        // Explicitly check for overlap on frame 1 before any spawners run
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            Collider2D[] overlaps = Physics2D.OverlapBoxAll(transform.position, col.bounds.size, 0f);
            foreach (var overlap in overlaps)
            {
                if (overlap.CompareTag("PlayerSight"))
                {
                    isSafeToSpawn = false;
                    break;
                }
            }
        }
    }

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