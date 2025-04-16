using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 10f;
    public float lifeTime = 5f;
    public float damage = 1f;

    [HideInInspector] public GameObject attacker;

    protected virtual void Start()
    {
        if (lifeTime > 0f) Destroy(gameObject, lifeTime);
    }

    protected virtual void Update()
    {
        Move();
    }

    protected virtual void Move()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == attacker) return;

        Health health = other.GetComponent<Health>();
        if (health == null || health.isInvulnerable) return;

        health.TakeDamage(damage, transform.position);
        Destroy(gameObject);
    }
}
