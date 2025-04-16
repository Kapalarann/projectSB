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

    [Header("Attacks")]
    [SerializeField] public RangedAttack rangedAttack;

    [Header("References")]
    [SerializeField] public Animator _animator;
    [SerializeField] public GameObject _sprite;
    [SerializeField] LayerMask _targetLayer;
    [SerializeField] AnimationReciever _receiver;

    [HideInInspector] public float _attackTime = 0f;
    [HideInInspector] public GameObject _target;

    Rigidbody _rigidbody;
    Vector3 _desiredMove = Vector3.zero;

    float flipScale = 1f, xScaleMult = 1f;

    void Start()
    {
        Enemies.Add(this);
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
        _receiver.AttackWarning -= Warning;
        _receiver.AttackFrame -= Shoot;
        _receiver.AttackEnd -= FinishAttack;
    }

    void FixedUpdate()
    {
        if (GetComponent<Health>().isStunned)
        {
            Vector3 currentVel = _rigidbody.linearVelocity;
            _rigidbody.linearVelocity = new Vector3(0f, currentVel.y, 0f);
            return;
        }

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
        foreach (var player in PlayerMovement.Players)
        {
            float d = Vector3.Distance(transform.position, player.transform.position);
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
        _desiredMove = movementValue.normalized * _movementSpeed;

        if (movementValue.x > 0) flipScale = 1;
        else if (movementValue.x < 0) flipScale = -1;
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
