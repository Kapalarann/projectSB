using UnityEngine;

public class MoonBeamer : Ability
{
    public RangedAttack rangedAttack;
    private PlayerMovement playerMovement;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    public void OnSpecial()
    {
        if (cooldownTimer > 0f) return;

        if (rangedAttack == null || _animator.GetBool("isStunned")) return;

        rangedAttack.FireInDirection(gameObject, Vector3.zero);
        cooldownTimer = rangedAttack._attackCooldown;
    }
}
