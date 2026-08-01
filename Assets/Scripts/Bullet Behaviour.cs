using System.Collections;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    [SerializeField] float maxLifeTime;
    [SerializeField] float maxRange;
    [SerializeField] bool gasShrink = false;
    [SerializeField] float gasShrinkAmmount = 0.2f;

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

        if (gasShrink)
        {
            Vector3 currentScale = gameObject.transform.localScale;
            currentScale.x -= gasShrinkAmmount * Time.deltaTime;
            gameObject.transform.localScale = currentScale;
        }
    }

    IEnumerator DestroyAfterCertainTime()
    {
        yield return new WaitForSeconds(maxLifeTime);
        Destroy(gameObject);
    }
}
