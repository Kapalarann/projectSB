using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 10f;
    public float lifeTime = 5f;
    public float damage = 1f;

    [Header("OnHitEffects")]
    [SerializeField] public Debuff debuff;
    [SerializeField] public int stack = 1;
    [SerializeField] public float duration = 2f;

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

        if(debuff != null) OnHitEffect(other);

        health.TakeDamage(damage, transform.position);
        Destroy(gameObject);
    }

    protected virtual void OnHitEffect(Collider other)
    {
        DebuffManager deb = other.GetComponent<DebuffManager>();
        if (other == null) return;

        deb.ApplyDebuff(debuff, stack, duration);
    }
}
