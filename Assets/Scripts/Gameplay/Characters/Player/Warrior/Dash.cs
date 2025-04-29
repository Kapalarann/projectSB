using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class Dash : Ability
{
    [SerializeField] private AnimationCurve dashSpeedCurve;
    [SerializeField] private float _dashDistance = 5f;
    [SerializeField] private float _dashDuration = 0.3f;

    private Rigidbody _rb;
    private Health _health;
    private Block _block;
    private PlayerMovement _movement;

    private bool isDashing = false;
    private float dashTimeElapsed;
    private Vector3 _dashDir;
    private int defaultLayer;
    private int dashLayer;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _health = GetComponent<Health>();
        _block = GetComponent<Block>();
        _movement = GetComponent<PlayerMovement>();

        defaultLayer = gameObject.layer;
        dashLayer = LayerMask.NameToLayer("Dash");
    }

    public void OnUtility()
    {
        if (_movement.movementValue.sqrMagnitude == 0 || _animator.GetBool("isStunned")) return;

        isDashing = true;
        dashTimeElapsed = 0f;
        _dashDir = _movement.movementValue.normalized;

        _block?.CancelBlock();

        ConsumeStamina();
        _health.isInvulnerable = true;
        _animator.SetTrigger("onDash");
        _animator.SetFloat("dashSpeed", 1f / _dashDuration);

        GetComponent<Audio>().PlayDashSound();
        gameObject.layer = dashLayer;
    }

    private void FixedUpdate()
    {
        if (!isDashing) return;

        dashTimeElapsed += Time.fixedDeltaTime;
        float normalizedTime = dashTimeElapsed / _dashDuration;
        float dashSpeedFactor = dashSpeedCurve.Evaluate(normalizedTime);
        float frameSpeed = (_dashDistance / _dashDuration) * dashSpeedFactor;

        Vector3 dashVelocity = _dashDir * frameSpeed;
        _rb.linearVelocity = new Vector3(dashVelocity.x, _rb.linearVelocity.y, dashVelocity.z);

        if (dashTimeElapsed >= _dashDuration)
        {
            isDashing = false;
            _health.isInvulnerable = false;
            gameObject.layer = defaultLayer;
        }
    }
}
