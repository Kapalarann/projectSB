using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duwende_Arrow : MonoBehaviour
{
    public float speed;
    public float lifeTime;
    public float damage;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        Health health = other.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(damage);
        }
        Destroy(gameObject);
    }
}
