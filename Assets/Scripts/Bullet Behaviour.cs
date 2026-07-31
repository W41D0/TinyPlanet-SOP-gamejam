using System.Collections;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    [SerializeField] float maxLifeTime;
    [SerializeField] float maxRange;

    Vector3 startPostion;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPostion = transform.position;
        StartCoroutine(DestroyAfterCertainTime());
    }

    // Update is called once per frame
    void Update()
    {
        float distanceTraveled = Vector3.Distance(startPostion, transform.position);
        if(distanceTraveled >= maxRange)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator DestroyAfterCertainTime()
    {
        yield return new WaitForSeconds(maxLifeTime);
        Destroy(gameObject);
    }
}
