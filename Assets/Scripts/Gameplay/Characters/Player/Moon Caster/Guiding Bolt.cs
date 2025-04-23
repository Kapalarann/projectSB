using UnityEngine;

public class GuidingBolt : MonoBehaviour
{
    public RangedAttack rangedAttack;
    private PlayerMovement playerMovement;
    private float cooldownTimer = 0f;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;
    }

    public void OnBlock()
    {
        if (playerMovement == null || rangedAttack == null) return;
        if (cooldownTimer > 0f) return;

        Vector3 direction = playerMovement.movementValue.normalized;

        if (direction != Vector3.zero)
        {
            rangedAttack.FireInDirection(gameObject, direction);
            cooldownTimer = rangedAttack._attackCooldown;
        }
    }
}
