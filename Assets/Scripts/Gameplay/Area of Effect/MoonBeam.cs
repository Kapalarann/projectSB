using UnityEngine;

public class MoonBeam : AOE
{
    public float switchDistance = 0.3f;
    public Transform target;
    private Vector3 dir;

    public override void Update()
    {
        if (target == null) GetNearestEnemy();

        dir = (target.position - transform.position);
        dir.y = 0f;
        if (dir.magnitude <= switchDistance) return;
        Vector3 move = dir.normalized * speed * Time.deltaTime;
        transform.Translate(move, Space.World);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (dir.magnitude <= switchDistance && tickTimer <= 0f)
        {
            GetNearestEnemy();
        }
    }

    private void GetNearestEnemy()
    {
        float shortestDistance = Mathf.Infinity;
        Transform nearestEnemy = null;

        foreach (EnemyStateManager enemy in EnemyStateManager.Enemies)
        {
            if (enemy.transform == target) continue;
            if (enemy == null) continue;

            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestEnemy = enemy.transform;
            }
        }

        if (nearestEnemy != null)
        {
            target = nearestEnemy;
        }
    }

}
