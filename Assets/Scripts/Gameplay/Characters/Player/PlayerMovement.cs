using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public static readonly HashSet<PlayerMovement> Players = new HashSet<PlayerMovement>();

    [Header("Movement")]
    [SerializeField] float _movementSpeed;
    [SerializeField] float _flipDampening = 5f;
    [Range(0.0f, 1.0f)]
    [SerializeField] float _mouseDeadzone;
    Vector3 movementValue;

    [Header("Dash")]
    [SerializeField] private AnimationCurve dashSpeedCurve;
    [SerializeField] private float _dashDistance = 5f;
    [SerializeField] private float _dashDuration = 0.3f;
    bool isDashing = false;
    private float dashTimeElapsed;
    private Vector3 _dashDir;

    [Header("Reference")]
    [SerializeField] Animator _animator;
    [SerializeField] GameObject _sprite;
    [SerializeField] AnimationReciever _receiver;

    private Camera _camera;
    private Rigidbody _rb;
    private Health _health;
    private Block _block;
    float flipScale = 1f, xScaleMult = 1f;
    private void Awake()
    {
        Players.Add(this);
        _camera = Camera.main;
        _rb = GetComponent<Rigidbody>();
        _health = GetComponent<Health>();
        _block = GetComponent<Block>();

        _rb.freezeRotation = true;
        xScaleMult = _sprite.transform.localScale.x;
    }

    public void OnMove(InputValue value)
    {
        movementValue = new Vector3( value.Get<Vector2>().x, 0, value.Get<Vector2>().y);
    }

    public void OnMouseMove(InputValue value)
    {
        Vector2 mousePosition = value.Get<Vector2>();

        Ray ray = _camera.ScreenPointToRay(mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 targetPosition = hit.point;
            targetPosition.y = transform.position.y;
            Vector3 directionToTarget = targetPosition - transform.position;

            float distanceToTarget = new Vector2(directionToTarget.x, directionToTarget.z).magnitude;

            if (distanceToTarget < _mouseDeadzone) movementValue = Vector3.zero;
            else movementValue = directionToTarget.normalized;
        }
    }

    public void OnDash()
    {
        if (movementValue.sqrMagnitude == 0 || _animator.GetBool("isStunned")) return;

        isDashing = true;
        dashTimeElapsed = 0f;
        _dashDir = movementValue.normalized;

        if(_block != null) _block.CancelBlock();

        _health.isInvulnerable = true;
        _animator.SetTrigger("onDash");
        _animator.SetFloat("dashSpeed", 1/_dashDuration);

        GetComponent<Audio>().PlayDashSound();
    }

    void FixedUpdate()
    {
        if (_animator.GetBool("isStunned")) return;

        HandleFlipping();

        if (isDashing)
        {
            Dash();
            return;
        }

        HandleMovement();
    }

    private void HandleFlipping()
    {
        if (movementValue.x > 0) flipScale = 1;
        else if (movementValue.x < 0) flipScale = -1;

        float currentXScale = _sprite.transform.localScale.x;
        float targetXScale = flipScale * xScaleMult;

        if (Mathf.Abs(currentXScale - targetXScale) > 0.01f)
        {
            float smoothedScale = Mathf.Lerp(currentXScale, targetXScale, _flipDampening * Time.fixedDeltaTime);
            _sprite.transform.localScale = new Vector3(smoothedScale, _sprite.transform.localScale.y, _sprite.transform.localScale.z);
        }
    }

    private void Dash()
    {
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
        }
    }

    private void HandleMovement()
    {
        Vector3 velocity = movementValue.normalized * _movementSpeed;
        _rb.linearVelocity = new Vector3(velocity.x, _rb.linearVelocity.y, velocity.z);

        float speed = new Vector2(movementValue.x, movementValue.z).magnitude;
        _animator.SetFloat("moveSpeed", speed);

        if (speed > 0.1f)
            GetComponent<Audio>().PlayWalkSound();
        else
            GetComponent<Audio>().StopWalkSound();
    }

    private void OnDestroy()
    {
        Players.Remove(this);
    }
}
