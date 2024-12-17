using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public static readonly HashSet<PlayerMovement> Players = new HashSet<PlayerMovement>();

    [Header("Movement")]
    [SerializeField] float _movementSpeed;
    [Range(0.0f, 1.0f)]
    [SerializeField] float _flipDampening;
    [Range(0.0f, 1.0f)]
    [SerializeField] float _mouseDeadzone;
    Vector3 movementValue;
    float gravity = -8.77f;
    private float verticalVelocity = 0f;

    [Header("Dash")]
    [SerializeField] float _dashSpeedMultiplier;
    [SerializeField] float _dashDuration;
    bool isDashing = false;
    Vector3 _dashDir;

    [Header("Reference")]
    [SerializeField] Animator _animator;
    [SerializeField] GameObject _sprite;
    [SerializeField] AnimationReciever _receiver;

    private Camera _camera;
    private CharacterController _characterController;
    float flipScale = 1f, xScaleMult = 1f;
    private void Awake()
    {
        Players.Add(this);
        _characterController = GetComponent<CharacterController>();
        _camera = Camera.main;
        xScaleMult = _sprite.transform.localScale.x;

        _receiver.DashEnd += DashEnd;
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
        if (movementValue.sqrMagnitude == 0) return;
        isDashing = true;
        this.GetComponent<Health>().isInvulnerable = true;
        _dashDir = movementValue;
        _animator.SetTrigger("onDash");
        _animator.SetFloat("dashSpeed",_dashDuration);

        gameObject.GetComponent<Audio>().PlayDashSound();
    }
    void Update()
    {
        if (_characterController.isGrounded && verticalVelocity < 0) verticalVelocity = -2f; // Small downward force to keep grounded
        if (!_characterController.isGrounded) verticalVelocity += gravity * Time.deltaTime;

        if (movementValue.x > 0) flipScale = 1;
        else if (movementValue.x < 0) flipScale = -1;
        if (Mathf.Abs(_sprite.transform.localScale.x - (flipScale * xScaleMult)) > 0.01f) _sprite.transform.localScale = new Vector3(Mathf.Lerp(_sprite.transform.localScale.x, flipScale * xScaleMult, _flipDampening), _sprite.transform.localScale.y, _sprite.transform.localScale.z);
        
        if (isDashing)
        {
            Vector3 dash = movementValue * _movementSpeed * _dashSpeedMultiplier * Time.deltaTime;
            _characterController.Move(dash);
            return;
        }

        if (movementValue.magnitude > 0) gameObject.GetComponent<Audio>().PlayWalkSound();
        else gameObject.GetComponent<Audio>().StopWalkSound();

        Vector3 moveH = movementValue * _movementSpeed;
        Vector3 move = new Vector3(moveH.x, verticalVelocity, moveH.z) * Time.deltaTime;
        _characterController.Move(move);

        float speed = new Vector3(movementValue.x, 0, movementValue.z).magnitude;
        _animator.SetFloat("moveSpeed", speed);
    }
    private void OnDestroy()
    {
        Players.Remove(this);
        _receiver.DashEnd -= DashEnd;
    }

    public void DashEnd(AnimationEvent animationEvent)
    {
        isDashing = false;
        this.GetComponent<Health>().isInvulnerable = false;
    }
}
