using UnityEngine;

public class AOE : Projectile
{
    [Header("AOE Settings")]
    [SerializeField] public float tickRate = 0.5f;
    [SerializeField] public float radius;
    [SerializeField] public float damagePerTick;
    public float tickTimer;

    private void Awake()
    {
        tickTimer = tickRate;
    }

    public virtual void FixedUpdate()
    {
        if (tickTimer <= 0f)
        {
            ApplyDamage();
            tickTimer += tickRate;
        }
        
        tickTimer -= Time.deltaTime;
    }
    protected virtual void ApplyDamage()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider hit in hitColliders)
        {
            var health = hit.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damagePerTick, true);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
