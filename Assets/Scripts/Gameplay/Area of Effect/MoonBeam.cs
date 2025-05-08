using UnityEngine;

public class MoonBeam : AOE
{
    public float switchDistance = 0.3f;
    public float swerveSpeed;
    public Transform target;
    private Vector3 dir;

    private Vector3 currentDirection;

    public override void Start()
    {
        base.Start();

        currentDirection = transform.forward;
    }

    public override void Update()
    {
        if (target == null)
        {
            GetNearestEnemy();
            return;
        }

        dir = (target.position - transform.position);
        dir.y = 0f;
        if (dir.magnitude <= switchDistance) return;

        // Gradually rotate currentDirection toward dir
        currentDirection = Vector3.RotateTowards(currentDirection, dir.normalized, swerveSpeed * Mathf.Deg2Rad * Time.deltaTime, 0.0f);
        currentDirection.Normalize();

        Vector3 move = currentDirection * speed * Time.deltaTime;
        transform.Translate(move, Space.World);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (target == null && tickTimer <= 0f)
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
            if (enemy == null || enemy.transform == target) continue;

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
