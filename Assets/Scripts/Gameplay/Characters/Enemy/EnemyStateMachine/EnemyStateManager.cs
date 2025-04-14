using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

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
    [SerializeField] float _movementSpeed;
    [Range(0.0f, 1.0f)]
    [SerializeField] float _flipDampening;
    float gravity = -8.77f;
    private float verticalVelocity = 0f;

    [Header("Attacks")]
    [SerializeField] public RangedAttack rangedAttack;

    [Header("References")]
    [SerializeField] public Animator _animator;
    [SerializeField] public GameObject _sprite;
    [SerializeField] LayerMask _targetLayer;
    [SerializeField] AnimationReciever _receiver;

    [HideInInspector] public float _attackTime = 0f;
    [HideInInspector] public GameObject _target;
    private CharacterController _characterController;
    float flipScale = 1f, xScaleMult = 1f;
    private void Start()
    {
        Enemies.Add(this);
        _characterController = GetComponent<CharacterController>();
        xScaleMult = _sprite.transform.localScale.x;

        currentState = idleState;
        currentState.EnterState(this);

        _receiver.AttackWarning += Warning;
        _receiver.AttackFrame += Shoot;
        _receiver.AttackEnd += FinishAttack;
    }

    private void OnDestroy()
    {
        _receiver.AttackWarning -= Warning;
        _receiver.AttackFrame -= Shoot;
        _receiver.AttackEnd -= FinishAttack;
    }

    private void Update()
    {
        if (gameObject.GetComponent<Health>().isStunned) return;

        if (_characterController.isGrounded && verticalVelocity < 0) verticalVelocity = -2f; // Small downward force to keep grounded
        if (!_characterController.isGrounded && _characterController != null)
        {
            verticalVelocity += gravity * Time.deltaTime;
            _characterController.Move(new Vector3(0f, verticalVelocity * Time.deltaTime, 0f));
        }

        currentState.UpdateState(this);

        if (_attackTime < rangedAttack._attackCooldown) _attackTime += Time.deltaTime;
        if (Mathf.Abs(_sprite.transform.localScale.x - (flipScale * xScaleMult)) > 0.01f) _sprite.transform.localScale = new Vector3(Mathf.Lerp(_sprite.transform.localScale.x, flipScale * xScaleMult, _flipDampening), _sprite.transform.localScale.y, _sprite.transform.localScale.z);
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
        if (movementValue.x > 0) flipScale = 1;
        else if (movementValue.x < 0) flipScale = -1;

        Vector3 move = movementValue * _movementSpeed * Time.deltaTime;
        _characterController.Move(move);

        float speed = new Vector3(movementValue.x, 0, movementValue.z).magnitude;
        _animator.SetFloat("moveSpeed", speed);
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