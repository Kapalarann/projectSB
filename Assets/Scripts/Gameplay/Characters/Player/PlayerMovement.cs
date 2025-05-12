using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public static readonly HashSet<PlayerMovement> Players = new HashSet<PlayerMovement>();

    [Header("Movement")]
    [SerializeField] float _movementSpeed;
    [SerializeField] float _flipDampening = 5f;
    [Range(0.0f, 1.0f)]
    [SerializeField] float _mouseDeadzone;
    [HideInInspector] public Vector3 movementValue;
    private float _originalSpeed;
    private Coroutine _slowCoroutine;
    private float _currentSlowPercent = 0f;
    private bool _isPermanentSlow = false;

    [Header("References")]
    [SerializeField] Animator _animator;
    [SerializeField] GameObject _sprite;

    private Camera _camera;
    private Rigidbody _rb;

    [HideInInspector] public float flipScale = 1f, xScaleMult = 1f;

    private void Awake()
    {
        Players.Add(this);
        _camera = Camera.main;
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;

        xScaleMult = _sprite.transform.localScale.x;
        _originalSpeed = _movementSpeed;
    }

    public void OnMove(InputValue value)
    {
        movementValue = new Vector3(value.Get<Vector2>().x, 0, value.Get<Vector2>().y);
    }

    public void OnMouseMove(InputValue value)
    {
        Vector2 mousePosition = value.Get<Vector2>();
        Ray ray = _camera.ScreenPointToRay(mousePosition);
        int layerMask = 1 << 6;

        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, layerMask);

        if (hits.Length > 0)
        {
            RaycastHit closestHit = hits[0];
            float closestDistance = closestHit.distance;

            foreach (var hit in hits)
            {
                if (hit.distance < closestDistance)
                {
                    closestHit = hit;
                    closestDistance = hit.distance;
                }
            }

            Vector3 targetPosition = closestHit.point;
            targetPosition.y = transform.position.y;
            Vector3 directionToTarget = targetPosition - transform.position;

            float distanceToTarget = new Vector2(directionToTarget.x, directionToTarget.z).magnitude;

            movementValue = distanceToTarget < _mouseDeadzone ? Vector3.zero : directionToTarget.normalized;
        }
    }

    private void FixedUpdate()
    {
        if (_animator.GetBool("isStunned")) return;

        HandleFlipping();
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

    private void HandleMovement()
    {
        Vector3 velocity = movementValue.normalized * _movementSpeed;
        _rb.linearVelocity = new Vector3(velocity.x, _rb.linearVelocity.y, velocity.z);

        float speed = new Vector2(movementValue.x, movementValue.z).magnitude;
        _animator.SetFloat("moveSpeed", speed);

        Audio audio = GetComponent<Audio>();
        if (speed > 0.1f)
            audio.PlayWalkSound();
        else
            audio.StopWalkSound();
    }

    private void OnDestroy()
    {
        Players.Remove(this);
    }

    public void ApplySlow(float slowPercent, float duration)
    {
        if (_isPermanentSlow || slowPercent <= _currentSlowPercent)
            return;

        if (_slowCoroutine != null)
        {
            StopCoroutine(_slowCoroutine);
            _slowCoroutine = null;
        }

        _currentSlowPercent = slowPercent;
        _movementSpeed = _originalSpeed * (1 - _currentSlowPercent);

        if (duration < 0f)
        {
            _isPermanentSlow = true;
        }
        else
        {
            _isPermanentSlow = false;
            _slowCoroutine = StartCoroutine(SlowCoroutine(duration));
        }
    }

    private IEnumerator SlowCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);

        _currentSlowPercent = 0f;
        _movementSpeed = _originalSpeed;
        _slowCoroutine = null;
    }

    public void RemovePermanentSlow()
    {
        if (_isPermanentSlow)
        {
            _currentSlowPercent = 0f;
            _movementSpeed = _originalSpeed;
            _isPermanentSlow = false;
        }
    }
}
