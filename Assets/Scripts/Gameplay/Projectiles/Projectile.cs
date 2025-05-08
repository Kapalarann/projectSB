using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 10f;
    public float lifetime = 5f;
    public float damage = 1f;

    [Header("OnHitEffects")]
    [SerializeField] public Debuff debuff;
    [SerializeField] public int stack = 1;
    [SerializeField] public float duration = 2f;

    [HideInInspector] public GameObject attacker;

    public virtual void Start()
    {
        if (lifetime > 0f) Destroy(gameObject, lifetime);
    }

    public virtual void Update()
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
        if (health == null) return;

        if(debuff != null) OnHitEffect(other);

        health.TakeDamage(damage, transform.position);
        Destroy(gameObject);
    }

    protected virtual void OnHitEffect(Collider other)
    {
        DebuffManager deb = other.GetComponent<DebuffManager>();
        if (other == null) return;

        deb.ApplyDebuff(debuff, stack, attacker,duration);
    }
}
