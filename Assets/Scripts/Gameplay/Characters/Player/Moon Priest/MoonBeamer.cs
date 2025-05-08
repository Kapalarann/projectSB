using UnityEngine;

public class MoonBeamer : Ability
{
    public RangedAttack rangedAttack;
    private PlayerMovement playerMovement;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    public override void Update()
    {
        base.Update();
    }

    public void OnSpecial()
    {
        if (!HasEnoughEnergy() || cooldownTimer > 0f) return;

        if (rangedAttack == null || _animator.GetBool("isStunned")) return;

        ConsumeEnergy();

        rangedAttack.FireInDirection(gameObject, Vector3.zero);
        cooldownTimer = rangedAttack._attackCooldown;

        ResetCooldown();
    }
}
