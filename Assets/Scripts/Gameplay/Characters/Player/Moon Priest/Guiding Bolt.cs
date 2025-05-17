using UnityEngine;
using UnityEngine.InputSystem;

public class GuidingBolt : Ability
{
    public RangedAttack rangedAttack;
    private PlayerMovement playerMovement;
    private InputAction inputAction;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        inputAction = GetComponent<PlayerInput>().actions["Secondary"];
    }

    public void OnSecondary()
    {
        if (playerMovement == null || rangedAttack == null || _animator.GetBool("isStunned") || !HasEnoughEnergy() || cooldownTimer > 0f) return;
        bool held = inputAction.ReadValue<float>() > 0.5f;

        if (!held) return; //if release, don't run

        Vector3 direction = playerMovement.movementValue.normalized;

        if (direction == Vector3.zero) direction = Vector3.right * playerMovement.flipScale;

        rangedAttack.FireInDirection(gameObject, direction);
        cooldownTimer = rangedAttack._attackCooldown;

        ConsumeEnergy();
    }
}
