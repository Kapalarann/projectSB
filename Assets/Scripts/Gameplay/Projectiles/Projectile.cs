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

        bool isReflected = false;
        float damageMult = 1f;
        health.TryBlock(damage, transform.position, out isReflected, out damageMult);
        if (isReflected)
        {
            Reflect(other.gameObject);
        }
        else
        {
            health.TakeDamage(damage * damageMult, transform.position);
            Destroy(gameObject);
        }
    }

    protected virtual void OnHitEffect(Collider other)
    {
        DebuffManager deb = other.GetComponent<DebuffManager>();
        if (other == null) return;

        deb.ApplyDebuff(debuff, stack, attacker,duration);
    }

    protected virtual void Reflect(GameObject reflector)
    {
        attacker = reflector;

        transform.Rotate(Vector3.up * 180);
    }
}
