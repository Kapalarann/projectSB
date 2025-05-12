using System.Collections.Generic;
using UnityEngine;

public class EnemyStateManager : MonoBehaviour
{
    public static readonly HashSet<EnemyStateManager> Enemies = new HashSet<EnemyStateManager>();

    EnemyBaseState currentState;
    public EnemyIdleState idleState = new EnemyIdleState();
    public EnemyApproachState approachState = new EnemyApproachState();
    public EnemyAttackState attackState = new EnemyAttackState();

    [Header("Behavior")]
    [SerializeField] public float _idleDuration;

    [Header("Movement")]
    [SerializeField] float _movementSpeed = 3f;
    [Range(0.0f, 1.0f)]
    [SerializeField] float _flipDampening = 0.2f;
    [SerializeField] float _steeringSpeed = 5f;
    private Vector3 _currentMoveDirection = Vector3.forward;

    [Header("Boid Behavior")]
    public float separationDistance = 2f;
    public float separationWeight = 2f;
    public float targetWeight = 1.5f;
    public float avoidWallWeight = 3f;
    public float wallDetectionRange = 1.5f;
    [SerializeField] public LayerMask wallLayer;

    [Header("Attacks")]
    [SerializeField] public RangedAttack rangedAttack;

    [Header("References")]
    [SerializeField] public Animator _animator;
    [SerializeField] public GameObject _sprite;
    [SerializeField] LayerMask _targetLayer;
    [SerializeField] AnimationReciever _receiver;

    [HideInInspector] public float _attackTime = 0f;
    [HideInInspector] public GameObject _target;

    private Stamina stamina;
    Rigidbody _rigidbody;
    Vector3 _desiredMove = Vector3.zero;

    float flipScale = 1f, xScaleMult = 1f;

    void Start()
    {
        Enemies.Add(this);
        stamina = GetComponent<Stamina>();
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        _rigidbody.useGravity = true;

        xScaleMult = _sprite.transform.localScale.x;

        currentState = idleState;
        currentState.EnterState(this);

        _receiver.AttackWarning += Warning;
        _receiver.AttackFrame += Shoot;
        _receiver.AttackEnd += FinishAttack;
    }

    void OnDestroy()
    {
        Enemies.Remove(this);

        _receiver.AttackWarning -= Warning;
        _receiver.AttackFrame -= Shoot;
        _receiver.AttackEnd -= FinishAttack;
    }

    void FixedUpdate()
    {
        if (stamina.isStunned) return;

        _desiredMove = Vector3.zero;

        // Update state logic
        currentState.UpdateState(this);

        // Attack cooldown
        if (_attackTime < rangedAttack._attackCooldown)
            _attackTime += Time.fixedDeltaTime;

        // Smooth sprite flip
        if (Mathf.Abs(_sprite.transform.localScale.x - (flipScale * xScaleMult)) > 0.01f)
        {
            _sprite.transform.localScale = new Vector3(
                Mathf.Lerp(_sprite.transform.localScale.x, flipScale * xScaleMult, _flipDampening),
                _sprite.transform.localScale.y,
                _sprite.transform.localScale.z
            );
        }

        // Apply horizontal movement with preserved vertical velocity
        Vector3 currentVelocity = _rigidbody.linearVelocity;
        Vector3 horizontalVelocity = new Vector3(_desiredMove.x, currentVelocity.y, _desiredMove.z);
        _rigidbody.linearVelocity = horizontalVelocity;

        float moveSpeed = new Vector3(_desiredMove.x, 0, _desiredMove.z).magnitude;
        _animator.SetFloat("moveSpeed", moveSpeed);
    }

    public void SwitchState(EnemyBaseState state)
    {
        currentState = state;
        state.EnterState(this);
    }

    public bool SearchTarget()
    {
        if (PlayerMovement.Players.Count <= 0) return false;

        float dist = float.PositiveInfinity;
        foreach (PlayerMovement player in PlayerMovement.Players)
        {
            if (player.gameObject.GetComponent<Health>().isDown) continue;
            float d = Vector3.Distance(transform.position, player.gameObject.transform.position);
            if (d < dist)
            {
                _target = player.gameObject;
                dist = d;
            }
        }
        return true;
    }

    public void Move(Vector3 movementValue)
    {
        if (movementValue.sqrMagnitude < 0.01f) return;

        // Smoothly rotate current direction toward desired direction
        _currentMoveDirection = Vector3.RotateTowards(
            _currentMoveDirection,
            movementValue.normalized,
            _steeringSpeed * Time.fixedDeltaTime,
            float.MaxValue
        );

        _desiredMove = _currentMoveDirection * _movementSpeed;

        if (_desiredMove.x > 0) flipScale = 1;
        else if (_desiredMove.x < 0) flipScale = -1;
    }


    public Vector3 GetBoidDirection()
    {
        Vector3 pos = transform.position;

        // 1. Direction to target
        Vector3 toTarget = _target.transform.position - pos;
        toTarget.y = 0f;
        Vector3 desiredTargetPos = _target.transform.position - toTarget.normalized * rangedAttack._minRange;
        Vector3 targetDir = (desiredTargetPos - pos).normalized;

        // 2. Separation
        Vector3 separation = Vector3.zero;
        foreach (var other in Enemies)
        {
            if (other == this) continue;
            float dist = Vector3.Distance(pos, other.transform.position);
            if (dist < separationDistance)
            {
                Vector3 away = pos - other.transform.position;
                separation += away.normalized / Mathf.Max(dist, 0.01f);
            }
        }

        // 3. Wall avoidance
        Vector3 wallAvoidance = Vector3.zero;
        Vector3[] directions = { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };

        foreach (Vector3 dir in directions)
        {
            if (Physics.Raycast(pos + Vector3.up * 0.5f, dir, out RaycastHit hit, wallDetectionRange, wallLayer))
            {
                wallAvoidance -= dir * (1f - (hit.distance / wallDetectionRange));
            }
        }

        Vector3 combined =
            (targetDir * targetWeight) +
            (separation * separationWeight) +
            (wallAvoidance * avoidWallWeight);

        combined.y = 0f;
        return combined.normalized;
    }


    void Warning(AnimationEvent animationEvent)
    {
        _sprite.GetComponent<SpriteRenderer>().color = Color.red;
    }

    void Shoot(AnimationEvent animationEvent)
    {
        _sprite.GetComponent<SpriteRenderer>().color = Color.white;
        rangedAttack.FireAtTarget(gameObject, _target);
    }

    public void FinishAttack(AnimationEvent animationEvent)
    {
        SwitchState(idleState);
    }
}
