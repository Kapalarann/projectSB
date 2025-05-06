using UnityEngine;

public class GuidingBolt : Ability
{
    public RangedAttack rangedAttack;
    private PlayerMovement playerMovement;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    public void OnSecondary()
    {
        if (playerMovement == null || rangedAttack == null || _animator.GetBool("isStunned") || !HasEnoughEnergy()) return;

        Vector3 direction = playerMovement.movementValue.normalized;

        if (direction != Vector3.zero)
        {
            rangedAttack.FireInDirection(gameObject, direction);
            cooldownTimer = rangedAttack._attackCooldown;
        }

        ConsumeEnergy();
    }
}
