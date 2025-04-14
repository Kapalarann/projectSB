using UnityEngine;

[System.Serializable]
public class RangedAttack
{
    [Header("Attack Settings")]
    public float _attackCooldown;
    public float _minRange = 5f;
    public float _maxRange = 7f;

    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public Transform attackPoint;
    public float damage = 1f;
    public float speed = 10f;
    public float lifeTime = 5f;

    public void FireAtTarget(GameObject attacker, GameObject target)
    {
        if (target == null || projectilePrefab == null || attackPoint == null) return;

        Vector3 direction = (target.transform.position - attacker.transform.position).normalized;

        SetProjectileStats(attacker, direction);
    }

    public void FireInDirection(GameObject attacker, Vector3 direction)
    {
        if (projectilePrefab == null || attackPoint == null) return;

        SetProjectileStats(attacker, direction);
    }

    private void SetProjectileStats(GameObject attacker, Vector3 dir)
    {
        GameObject projectile = GameObject.Instantiate(projectilePrefab, attackPoint.position, Quaternion.LookRotation(dir));
        var proj = projectile.GetComponent<Projectile>();
        if (proj != null)
        {
            proj.attacker = attacker;
            proj.speed = speed;
            proj.lifeTime = lifeTime;
            proj.damage = damage;
        }
    }
}